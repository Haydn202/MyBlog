using System.Net.Http.Headers;
using System.Text;

namespace IntegrationTests.Helpers;

internal static class HttpSnapshotSanitizer
{
    private static readonly string[] VolatileHeaderNames =
        ["Date", "Server", "Transfer-Encoding", "Content-Length"];

    /// <summary>
    /// Prepares an <see cref="HttpResponseMessage"/> for Verify: strips transport-specific headers,
    /// normalizes the request URI, and rewrites outbound <see cref="HttpContent"/> as <see cref="StringContent"/>
    /// so snapshots match in-process TestServer (buffered body + Content-Length).
    /// </summary>
    public static async Task ApplyForHttpResponseSnapshotAsync(HttpResponseMessage response)
    {
        RemoveVolatileHeaders(response.Headers);
        RemoveVolatileHeaders(response.Content.Headers);

        var request = response.RequestMessage;
        if (request is null)
        {
            return;
        }

        RemoveVolatileHeaders(request.Headers);
        if (request.Content is not null)
        {
            RemoveVolatileHeaders(request.Content.Headers);
        }

        NormalizeRequestUri(request);

        if (request.Content is not null)
        {
            var body = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            var mediaType = request.Content.Headers.ContentType?.MediaType ?? "application/json";
            request.Content = new StringContent(body, Encoding.UTF8, mediaType);
        }
    }

    public static Dictionary<string, string[]> FilterForSnapshot(HttpHeaders headers) =>
        headers
            .Where(h => !IsVolatileHeader(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToArray());

    private static void RemoveVolatileHeaders(HttpHeaders headers)
    {
        foreach (var name in VolatileHeaderNames)
        {
            TryRemove(headers, name);
        }
    }

    private static void TryRemove(HttpHeaders headers, string name)
    {
        try
        {
            headers.Remove(name);
        }
        catch (InvalidOperationException)
        {
            // Header is not valid on this HttpHeaders subtype (e.g. Content-Length on HttpResponseHeaders).
        }
    }

    private static bool IsVolatileHeader(string name)
    {
        foreach (var n in VolatileHeaderNames)
        {
            if (name.Equals(n, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static void NormalizeRequestUri(HttpRequestMessage request)
    {
        if (request.RequestUri is null)
        {
            return;
        }

        var builder = new UriBuilder(request.RequestUri)
        {
            Host = "localhost",
            Port = -1
        };
        request.RequestUri = builder.Uri;
    }
}
