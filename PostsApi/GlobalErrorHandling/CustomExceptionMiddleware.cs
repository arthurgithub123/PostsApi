using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace PostsApi.GlobalErrorHandling
{
    public class CustomExceptionMiddleware
    {
        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private readonly RequestDelegate _next;

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

                if(httpContext.Response.StatusCode == 401)
                {
                    await handleResponseStatusCode(httpContext, "Você não possui autorização");
                }

                if(httpContext.Response.StatusCode == 404)
                {
                    await handleResponseStatusCode(httpContext, "Endpoint inexistente");
                }
            }
            catch(Exception exception)
            {
                await HandleExceptionAsync(httpContext, exception);
            }
        }
        
        private Task handleResponseStatusCode(HttpContext context, string message)
        {
            ErrorModel errorModel = new ErrorModel
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            return context.Response.WriteAsync(errorModel.ToString());
        }

        private Task HandleExceptionAsync(HttpContext context, Exception thrownException)
        {
            ErrorModel errorModel;
            HttpResponseException httpResponseException;

            if (thrownException is HttpResponseException)
            {
                httpResponseException = thrownException as HttpResponseException;

                errorModel = new ErrorModel
                {
                    StatusCode = httpResponseException.StatusCode,
                    Message = httpResponseException.Message
                };

                context.Response.StatusCode = httpResponseException.StatusCode;
            }
            else
            {
                errorModel = new ErrorModel
                {
                    StatusCode = 500,
                    Message = "Erro interno no servidor"
                };
                
                context.Response.StatusCode = 500;
            }

            return context.Response.WriteAsync(errorModel.ToString());
        }
    }
}
