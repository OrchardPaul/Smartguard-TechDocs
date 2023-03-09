//using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using GadjIT_ClientContext.Models;

namespace GadjIT_ClientAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private ILogger<ExceptionMiddleware> Logger;
        private readonly RequestDelegate Next;

        public ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger)
        {
            Next = _next;
            Logger = _logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {          
            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ExceptionModel exceptionDetails = GetExceptionViewModel(ex);
            
            Logger.LogError(ex, "Controller Error - Method: {0}; Message: {1}; Stack Trace: {2}",exceptionDetails.Method,exceptionDetails.Message, exceptionDetails.StackTrace);

            var result = JsonConvert.SerializeObject(exceptionDetails);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }

            return context.Response.WriteAsync(result);

        }
        
        private ExceptionModel GetExceptionViewModel(Exception ex)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Microsoft.Extensions.Hosting.Environments.Development;

            string methodName = "";
            methodName = ex.TargetSite.ReflectedType.FullName;
                
            return new ExceptionModel()
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Method = !isDevelopment ? "Exception" : methodName,
                Message = !isDevelopment ? "Internal Server Error" : ex.Message,
                StackTrace = !isDevelopment ? "" : ex.StackTrace
            };
        }
    }

    
}