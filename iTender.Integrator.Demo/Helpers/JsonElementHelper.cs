using System.Text.Json;

namespace iTender.Integrator.Demo.Helpers
{
    public static class JsonElementHelper
    {
        private static readonly string[] CollectionProperties =
        [
            "releases",
        "items",
        "results",
        "data",
        "value"
        ];

        public static IReadOnlyList<JsonElement> GetItems(
            JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Array)
            {
                return root.EnumerateArray().ToList();
            }

            if (root.ValueKind != JsonValueKind.Object)
            {
                return [];
            }

            foreach (var propertyName in CollectionProperties)
            {
                if (!TryGetPropertyIgnoreCase(
                        root,
                        propertyName,
                        out var value))
                {
                    continue;
                }

                if (value.ValueKind == JsonValueKind.Array)
                {
                    return value.EnumerateArray().ToList();
                }
            }

            return [root];
        }

        public static string GetString(
            JsonElement element,
            params string[] paths)
        {
            foreach (var path in paths)
            {
                var current = element;

                var segments = path.Split(
                    '.',
                    StringSplitOptions.RemoveEmptyEntries);

                var found = true;

                foreach (var segment in segments)
                {
                    if (!TryGetPropertyIgnoreCase(
                            current,
                            segment,
                            out current))
                    {
                        found = false;
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }

                if (current.ValueKind == JsonValueKind.String)
                {
                    return current.GetString() ?? string.Empty;
                }

                if (current.ValueKind is
                    JsonValueKind.Number or
                    JsonValueKind.True or
                    JsonValueKind.False)
                {
                    return current.ToString();
                }
            }

            return string.Empty;
        }

        public static string FormatJson(JsonElement element)
        {
            return JsonSerializer.Serialize(
                element,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }

        private static bool TryGetPropertyIgnoreCase(
            JsonElement element,
            string propertyName,
            out JsonElement value)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (string.Equals(
                            property.Name,
                            propertyName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }
    }
}
