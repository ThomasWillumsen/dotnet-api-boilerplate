using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Boilerplate.Core.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Api.ErrorHandling
{
    /// <summary>
    /// middleware to catch exceptions and format the api response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ApiErrorResponse errorResponse = null;

            if (exception is BusinessRuleException)
            {
                var e = (BusinessRuleException)exception;
                errorResponse = new ApiErrorResponse(
                    e.ReasonText,
                    e.ErrorCode
                );
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (exception is KeyNotFoundException)
            {
                var e = (KeyNotFoundException)exception;
                errorResponse = new ApiErrorResponse(
                    e.Message,
                    nameof(ApiErrorsConstants.NOT_FOUND)
                );
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                errorResponse = new ApiErrorResponse(
                    ApiErrorsConstants.INTERNAL_SERVER_ERROR,
                    nameof(ApiErrorsConstants.INTERNAL_SERVER_ERROR)
                );
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
            logger.LogWarning("ExceptionMiddleware {errorResponse}", JsonConvert.SerializeObject(errorResponse));

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}
