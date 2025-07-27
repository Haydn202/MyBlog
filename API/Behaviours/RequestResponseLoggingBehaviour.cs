using System.Text.Json;
using MediatR;

namespace API.Behaviours;

public class RequestResponseLoggingBehaviour<TRequest, TResponse>(
    ILogger<RequestResponseLoggingBehaviour<TRequest, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?.Items["X-Correlation-ID"]?.ToString();

        var requestJson = JsonSerializer.Serialize(request);
        
        logger.LogInformation("Handling request {CorrelationId} {Request}", correlationId, requestJson);
        
        var response = await next();
        
        var responseJson = JsonSerializer.Serialize(response);
        
        logger.LogInformation("Response for {CorrelationId} {Response}", correlationId, responseJson);
        
        return response;
    }
}