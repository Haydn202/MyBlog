using System.Text.Json;

namespace IntegrationTests.Helpers;

internal static class UserListSnapshot
{
    private static readonly string[] DisplayPropertyNames =
        ["id", "userName", "email", "role"];

    /// <summary>
    /// Parses GET /Users JSON, keeps id / userName / email / role as strings, sorts by email for Verify snapshots.
    /// </summary>
    /// <param name="includeEmail">When not null, only rows whose email matches are included (shared DB across tests).</param>
    public static List<Dictionary<string, string?>> BuildSortedDisplayRows(
        string getUsersResponseBody,
        Func<string?, bool>? includeEmail = null)
    {
        using var doc = JsonDocument.Parse(getUsersResponseBody);
        if (doc.RootElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var rows = new List<Dictionary<string, string?>>();
        foreach (var el in doc.RootElement.EnumerateArray())
        {
            var row = new Dictionary<string, string?>();
            foreach (var name in DisplayPropertyNames)
            {
                row[name] = ReadPropertyAsDisplayString(el, name);
            }

            if (includeEmail is not null && !includeEmail(row.GetValueOrDefault("email")))
            {
                continue;
            }

            rows.Add(row);
        }

        rows.Sort(
            (a, b) => string.CompareOrdinal(
                a.GetValueOrDefault("email") ?? "",
                b.GetValueOrDefault("email") ?? ""));

        return rows;
    }

    private static string? ReadPropertyAsDisplayString(JsonElement parent, string name)
    {
        if (!parent.TryGetProperty(name, out var p))
        {
            return null;
        }

        return p.ValueKind switch
        {
            JsonValueKind.String => p.GetString(),
            JsonValueKind.Number => p.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => null,
            _ => p.GetRawText(),
        };
    }
}
