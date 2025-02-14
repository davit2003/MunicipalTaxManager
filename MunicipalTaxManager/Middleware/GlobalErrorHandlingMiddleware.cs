using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MunicipalTaxManager.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle EF database issues
                await HandleExceptionAsync(context, dbEx, HttpStatusCode.InternalServerError,
                    "A database error occurred.");
            }
            catch (Exception ex)
            {
                // Handle all other unanticipated errors
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            HttpStatusCode statusCode,
            string userFriendlyMessage)
        {
            var problemDetails = new
            {
                status = (int)statusCode,
                title = userFriendlyMessage,
                //detail = exception.Message,
                instance = context.Request?.Path.Value
            };

            var json = JsonSerializer.Serialize(problemDetails);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(json);
        }
    }
}
