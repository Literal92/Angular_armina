using Microsoft.AspNetCore.Builder;

namespace shop.Extension
{
    
    public static class ErrorHandlingMiddlewareExtension
    {
        public static IApplicationBuilder ErrorHandlingMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}