namespace API.Helper
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
       
    }
}
