namespace API.Behaviours;

public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers.TryGetValue(HeaderName, out Microsoft.Extensions.Primitives.StringValues value) 
            ? value.ToString()
            : Guid.NewGuid().ToString();

        context.Items[HeaderName] = id;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = id;
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object> { [HeaderName] = id }))
        {
            await next(context);
        }
    }
}