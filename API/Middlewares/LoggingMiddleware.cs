using Azure.Core;
using BLL.Helper;
using DAL.Model;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;
using System.Net;
using System.Security.Claims;

namespace API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
  

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
           
        }

        public async Task Invoke(HttpContext context, UserManager<User> userManager)
        {
            var token = HttpContextHelper.GetToken(context);
            string? UserId=null;
            if (token!=null)
            {
                UserId = HttpContextHelper.GetUserIdFromToken(token);
            }
            User? user = null;
            if (UserId != null)
            {
                user = await userManager.FindByIdAsync(UserId);
            }
            try
            {


                #region Request 
                var requestBodyStream = new MemoryStream();
                var originalRequestBody = context.Request.Body;
                await context.Request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                var url = UriHelper.GetDisplayUrl(context.Request);
                var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
                Log.Information($"REQUEST METHOD: {context.Request.Method}\n REQUEST BODY: {requestBodyText}\n REQUEST URL: {url}");
                Log.Information($"UserName of him is : {user?.UserName ?? "Anonymous"}\n ");
                
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;
                #endregion


                #region Response
                var bodyStream = context.Response.Body;
                var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;
                await _next(context);
                context.Request.Body = originalRequestBody;
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                Log.Information($"RESPONSE LOG: {responseBody}");

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(bodyStream);

                Log.Information("=====================================================\n");
                #endregion
            }
            catch (Exception ex)
            {

                Log.Error($"Unhandled exception: {ex.Message}\n");
                Log.Error($"Endpoint: {context.GetEndpoint()?.DisplayName ?? "Unknown endpoint"}\n");
                Log.Error($"UserName: {user?.UserName ?? "Anonymous"}\n");
                Log.Error($"Request Method: {context.Request.Method} \n");
                Log.Error($"Request Path: {context.Request.Path} \n");
                Log.Error(ex, "Exception details\n");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Log.Information("=====================================================\n");
            }
        }
    }
}
