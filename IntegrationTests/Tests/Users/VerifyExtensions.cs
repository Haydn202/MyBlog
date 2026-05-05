using IntegrationTests.Helpers;

namespace IntegrationTests.Tests.Users;

public static class VerifyExtensions
{
    public static async Task Verify(this Task<HttpResponseMessage> responseTask)
    {
        var response = await responseTask;
        await HttpSnapshotSanitizer.ApplyForHttpResponseSnapshotAsync(response);
        await Verifier.Verify(response);
    }

    /// <summary>
    /// Verifies status, all headers, and body (use when the body is stable after global scrubbers).
    /// </summary>
    public static async Task VerifyResponseOnly(this Task<HttpResponseMessage> responseTask)
    {
        var r = await responseTask;
        var body = await r.Content.ReadAsStringAsync();
        await Verifier.Verify(new
        {
            StatusCode = (int)r.StatusCode,
            ReasonPhrase = r.ReasonPhrase,
            ResponseHeaders = HttpSnapshotSanitizer.FilterForSnapshot(r.Headers),
            ContentHeaders = HttpSnapshotSanitizer.FilterForSnapshot(r.Content.Headers),
            Body = body
        });
    }

    /// <summary>
    /// Verifies status and headers only (no outbound request, no response body). Use when JSON bodies
    /// include per-run user names or other values that are not scrubbed inside string payloads.
    /// </summary>
    public static async Task VerifyResponseMetaOnly(this Task<HttpResponseMessage> responseTask)
    {
        var r = await responseTask;
        await Verifier.Verify(new
        {
            StatusCode = (int)r.StatusCode,
            ReasonPhrase = r.ReasonPhrase,
            ResponseHeaders = HttpSnapshotSanitizer.FilterForSnapshot(r.Headers),
            ContentHeaders = HttpSnapshotSanitizer.FilterForSnapshot(r.Content.Headers)
        });
    }
}
