using System.Diagnostics;
using MediatR;

namespace Boilerplate.Api.Domain.PipelineBehaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        TResponse response;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            response = await next();
        }
        finally
        {
            stopwatch.Stop();
            var requestName = request.GetType().FullName!.Split(".").Last();
            _logger.LogInformation("{Request} finished in {Elapsed} ms", requestName, stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}