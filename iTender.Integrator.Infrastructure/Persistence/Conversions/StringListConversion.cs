using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace iTender.Integrator.Infrastructure.Persistence.Conversions
{
    public static class StringListConversion
    {
        public static ValueConverter<IReadOnlyCollection<string>, string> Converter { get; } = new(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        public static ValueComparer<IReadOnlyCollection<string>> Comparer { get; } = new(
            (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
            v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s)),
            v => v.ToList());
    }
}
