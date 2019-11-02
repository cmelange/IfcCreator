using System;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfcCreator.ExceptionHandling
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }
    
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ReportableException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"unexpected server error: {ex}");
                await HandleExceptionAsync(httpContext, new ReportableException());
            }
        }
    
        private Task HandleExceptionAsync(HttpContext context, ReportableException exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = exception.status;

            string exceptionJSON = exception.ToJSON();
            _logger.LogTrace("{status} - {content}", context.Response.StatusCode, exceptionJSON);

            return context.Response.WriteAsync(exceptionJSON);
        }
        
    }
}