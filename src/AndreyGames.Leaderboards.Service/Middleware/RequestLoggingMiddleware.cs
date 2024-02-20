using System;
using System.Threading.Tasks;
using AndreyGames.Leaderboards.Service.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace AndreyGames.Leaderboards.Service.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
                
                _logger.LogInformation(
                    "{method} {fullUrl} {url} => {statusCode}",
                    context.Request?.Method,
                    context.Request?.GetDisplayUrl(),
                    context.Request?.Path.Value,
                    context.Response?.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{method} {fullUrl} {url} => {statusCode}, exception: {exception}",
                    context.Request?.Method,
                    context.Request?.GetDisplayUrl(),
                    context.Request?.Path.Value,
                    context.Response?.StatusCode,
                    ex);
            }
        }
    }
}