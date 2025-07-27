using FluentValidation;
using MediatR;

namespace API.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : 
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);
            
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            
            var validationResults = await Task.WhenAll(
                validators.Select(v => 
                    v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
            
            var failures = validationResults
                .Where(result => result.Errors.Count > 0)
                .SelectMany(result => result.Errors)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }
        }
        
        return await next().ConfigureAwait(false);
    }
}