using Microsoft.AspNetCore.Http;

namespace BLL.Helper
{
    public  class HttpContextHelper
    {
        
        public static string? GetToken(HttpContext httpContext)
        {
            string? token = httpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            return token;
        }
        public static string? GetTokenHub(HttpContext httpContext)
        {
            string? token = httpContext.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            return token;
        }
    }
}
