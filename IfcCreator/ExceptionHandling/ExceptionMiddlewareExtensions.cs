using Microsoft.AspNetCore.Builder;

namespace IfcCreator.ExceptionHandling
{
    public static class ExceptionMidlewareExtensions
    {
        public static void ConfigureGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}