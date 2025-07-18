using System.Net;
using FiTrack.Application.Constants;
using FiTrack.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Shared.Constants;
using Shared.Helpers;

namespace FiTrack.Application.Exceptions;
public sealed class GlobalExceptionHandler(IHostEnvironment hostEnvironment) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(
       HttpContext httpContext,
       Exception exception,
       CancellationToken cancellationToken)
    {
        var responseMessage = $"Title:{GetTitle(exception)}: Message:{exception.Message}";
        var (httpStatusCode, writeLog) = exception switch
        {
            ArgumentNullException or ModelIsNullException or ArgumentException
                => (HttpStatusCode.BadRequest, false),
            NotFoundException or FileNotFoundException
                => (HttpStatusCode.NotFound, false),
            ValidationException
                => (HttpStatusCode.BadRequest, true),
            AlreadyExistException or ConflictException or JsonSerializationException
                => (HttpStatusCode.Conflict, true),
            AccessDeniedException
                => (HttpStatusCode.Forbidden, true),
            ErrorFromClientException
                => (HttpStatusCode.BadRequest, true),
            RefreshTokenExpiredException
                => (HttpStatusCode.Unauthorized, false),
            _ => (HttpStatusCode.InternalServerError, true)
        };

        if (writeLog)
        {

            var userId = httpContext.User.Claims.FirstOrDefault(x => x.Type == StaticClaims.UserId)?.Value ?? "Unauthorized user";

            var errorMessageForTelegram = $"""

                              🚨Error Log🚨

                              📱App: {hostEnvironment.ApplicationName}:{hostEnvironment.EnvironmentName}.

                              ⚠️Exception Type: {exception.GetType().Name}.

                              📝Message: {exception.Message}

                              🔗Path: {httpContext.Request.Path}.

                              👤User Id: {userId}.

                              """;


            Log.Error("[Exception in {App}:{Env}. Type: {ExceptionType}. Path: {Path}. Message: {Message}. UserId: {UserId}]",
                hostEnvironment.ApplicationName,
                hostEnvironment.EnvironmentName,
                exception.GetType().Name,
                httpContext.Request.Path,
                exception.Message,
                userId);


            TelegramLog.LogError(errorMessageForTelegram);

        }

        var response = new ResponseData<string>(responseMessage, httpStatusCode);
        httpContext.Response.StatusCode = (int)httpStatusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);

        return true;
    }

    private static string GetTitle(Exception exception) => exception switch
    {
        ArgumentNullException or ModelIsNullException or ArgumentException =>  "Argument Error",
        ValidationException => "Validation Error",
        AlreadyExistException or ConflictException => "Conflict Error",
        AccessDeniedException => "Access Denied",
        FileNotFoundException or NotFoundException => "Not Found",
        ErrorFromClientException => "Client Error",
        RefreshTokenExpiredException => "Session Timeout",
        DbUpdateException => "Database Error",
        _ => "Server Error"
    };
}
