using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductStorage.BLL.Exceptions;

namespace ProductStorage.Api.Http.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            var errors = e.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

            await WriteProblem(context, new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Ошибка валидации запроса.",
            });
        }
        catch (ProductNotFoundException e)
        {
            await WriteProblem(context, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Товар не найден.",
                Detail = e.Message,
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Необработанная ошибка при запросе {Method} {Path}", context.Request.Method, context.Request.Path);

            await WriteProblem(context, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Внутренняя ошибка сервера.",
            });
        }
    }

    private static async Task WriteProblem(HttpContext context, ProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json");
    }
}
