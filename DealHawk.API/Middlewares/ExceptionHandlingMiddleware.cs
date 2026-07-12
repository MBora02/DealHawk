using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DealHawk.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                _logger.LogInformation("[API Request] Path: {Path}, AuthHeader: {AuthHeader}", context.Request.Path, authHeader.Substring(0, Math.Min(authHeader.Length, 25)) + "...");
            }
            else
            {
                _logger.LogInformation("[API Request] Path: {Path}, AuthHeader: NULL", context.Request.Path);
            }
            try
            {
                await _next(context);
                if (context.Response.StatusCode == 401)
                {
                    _logger.LogWarning("[API Request] Path: {Path} returned 401 Unauthorized", context.Request.Path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };
            context.Response.StatusCode = (int)statusCode;
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Detailed = exception.InnerException?.Message ?? string.Empty
            };
            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
