using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            HttpStatusCode statusCode;
            string title;

            switch (ex)
            {
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Invalid request.";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    title = "Resource not found.";
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.Conflict;
                    title = "Conflict.";
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    title = "An unexpected error occurred.";
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = statusCode == HttpStatusCode.InternalServerError ? "An unexpected error occurred." : ex.Message
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}