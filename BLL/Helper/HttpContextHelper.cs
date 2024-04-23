﻿using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

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
        public static string? GetTokenHub(HttpContext? httpContext)
        {
            if (httpContext == null) return null;
            string? token = httpContext.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            return token;
        }
        public static string? GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();


            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);


                var userId = jwtToken.Payload["uid"]?.ToString();

                return userId;
            }
            else
            {
                return null;
            }
        }
    }
}
