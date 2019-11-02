using System;
using System.IO;
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
                await HandleExceptionAsync(httpContext, new ReportableException("an unexpected exception has occurred", ex));
            }
        }
    
        private Task HandleExceptionAsync(HttpContext context, ReportableException exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = exception.status;

            LogRequest(context);
            string exceptionJSON = exception.ToJSON();
            _logger.LogError("{status} - {content}", context.Response.StatusCode, exceptionJSON);
            _logger.LogDebug(exception, "an exception was catched");

            return context.Response.WriteAsync(exceptionJSON);
        }

        private void LogRequest(HttpContext httpContext)
        {
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader( httpContext.Request.Body );
                string body = reader.ReadToEnd();
                _logger.LogInformation("{method} {path} {content}", httpContext.Request.Method,
                                                                    httpContext.Request.Path,
                                                                    body );
        }
        
    }
}