using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using ProductStorage.BLL.Exceptions;

namespace ProductStorage.Api.Grpc.Interceptors;

public class ErrorHandlingInterceptor(ILogger<ErrorHandlingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (ValidationException e)
        {
            var errors = string.Join("; ", e.Errors.Select(error => $"{error.PropertyName}: {error.ErrorMessage}"));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }
        catch (ProductNotFoundException e)
        {
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Необработанная ошибка в gRPC {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера."));
        }
    }
}
