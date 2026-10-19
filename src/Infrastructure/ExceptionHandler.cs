using Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            DomainException => StatusCodes.Status422UnprocessableEntity,
            ArgumentException or InvalidOperationException or BadHttpRequestException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var title = exception switch
        {
            UnauthorizedAccessException => "Não autorizado",
            DbUpdateConcurrencyException => "Conflito de concorrência",
            DomainException => "Regra de negócio violada",
            ArgumentException or InvalidOperationException or BadHttpRequestException => "Requisição inválida",
            KeyNotFoundException => "Recurso não encontrado",
            _ => "Ocorreu um erro no servidor"
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = exception.GetType().Name,
            Detail = exception.Message
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}