using MediatR;

namespace Bolt.Application.Behaviors.Logging;

/// <summary>
/// Pipeline behavior that logs all commands and queries.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        Console.WriteLine($"[APP-LOG] Handling {requestName}");

        try
        {
            var response = await next();
            Console.WriteLine($"[APP-LOG] {requestName} handled successfully");
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[APP-LOG] {requestName} failed: {ex.Message}");
            throw;
        }
    }
}
