using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IfcCreator.HTTP
{
    public class ResponseHelper: IResponseHelper
    {
        private ILogger _logger;

        public ResponseHelper(ILogger<ResponseHelper> logger)
        {
            _logger = logger;
        }

        public async Task PostCommand<T>(Func<T, Task> function,
                                         T input,
                                         HttpContext httpContext)
        {
            LogRequest(httpContext);
            await function.Invoke(input);
            httpContext.Response.StatusCode = 200;
            LogResponse(httpContext, "");
            return;
        }

        private void LogRequest(HttpContext httpContext)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader( httpContext.Request.Body );
                string body = reader.ReadToEnd();
                _logger.LogDebug("{method} {path} {content}", httpContext.Request.Method,
                                                              httpContext.Request.Path,
                                                              body );
            }
        }

        private void LogResponse(HttpContext httpContext, object response)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                 _logger.LogDebug("{status} - {content}", httpContext.Response.StatusCode, JsonConvert.SerializeObject(response));
            }
        }
    }
}