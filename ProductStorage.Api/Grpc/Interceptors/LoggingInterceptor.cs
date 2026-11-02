using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProductStorage.Api.Grpc.Interceptors;

public class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("gRPC {Method} request: {Request}", context.Method, request);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await continuation(request, context);
            logger.LogInformation("gRPC {Method} response ({ElapsedMs} ms): {Response}",
                context.Method, stopwatch.ElapsedMilliseconds, response);
            return response;
        }
        catch (RpcException e)
        {
            logger.LogWarning("gRPC {Method} error ({ElapsedMs} ms): {StatusCode} {Detail}",
                context.Method, stopwatch.ElapsedMilliseconds, e.StatusCode, e.Status.Detail);
            throw;
        }
    }
}
