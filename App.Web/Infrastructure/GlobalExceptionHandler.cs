using App.Shared.Exceptions;
using App.Shared.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace App.Web.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var response = new StdResponse
        {
            Success = false
        };

        if (exception is BusinessException businessException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ErrorCode = businessException.ErrorCode.Code;
            response.ErrorCode = businessException.ErrorCode.Code;
            response.ErrorMessage = businessException.ErrorCode.Message;
        }
        else if (exception is UnauthorizedAccessException)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.ErrorCode = "UNAUTHORIZED";
            response.ErrorMessage = "Unauthorized access";
        }
        else
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR.Code;
            response.ErrorMessage = ErrorCodes.INTERNAL_SERVER_ERROR.Message;
        }

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
