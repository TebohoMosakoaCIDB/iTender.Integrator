using iTender.Integrator.Demo.Components;
using iTender.Integrator.Demo.Configuration;
using iTender.Integrator.Demo.Infrastructure;
using iTender.Integrator.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddOptions<IntegratorApiOptions>()
    .Bind(builder.Configuration.GetSection(
        IntegratorApiOptions.SectionName))
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.BaseUrl),
        "IntegratorApi:BaseUrl must be configured.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.ApiKey),
        "IntegratorApi:ApiKey must be configured.")
    .ValidateOnStart();

builder.Services.AddTransient<IntegratorApiKeyHandler>();

builder.Services.AddHttpClient<IntegratorApiClient>(
    (serviceProvider, client) =>
    {
        var options =
            serviceProvider
                .GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<IntegratorApiOptions>>()
                .Value;

        client.BaseAddress = new Uri(
            options.BaseUrl.EndsWith("/")
                ? options.BaseUrl
                : options.BaseUrl + "/");

        client.Timeout = TimeSpan.FromSeconds(60);
    })
    .AddHttpMessageHandler<IntegratorApiKeyHandler>();
builder.Services.AddScoped<ContractorAuthService>();

builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
