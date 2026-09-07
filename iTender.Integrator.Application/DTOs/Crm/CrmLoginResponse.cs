namespace iTender.Integrator.Application.DTOs.Crm
{
    public class CrmLoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
