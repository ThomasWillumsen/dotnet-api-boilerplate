using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;

namespace Boilerplate.Api.ErrorHandling
{
    /// <summary>
    /// Catch invalid model states and format a proper response to the client
    /// </summary>
    public static class ModelStateValidationBehaviour
    {
        public static IMvcBuilder ConfigureApiBehaviorOptions(this IMvcBuilder builder) =>
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiErrorResponse(
                        ApiErrorsConstants.MODEL_VALIDATION_ERROR,
                        nameof(ApiErrorsConstants.MODEL_VALIDATION_ERROR),
                        context.ModelState.Keys
                            .SelectMany(key => context.ModelState[key].Errors
                                .Select(x => new ModelValidationError(key, x.ErrorMessage)))
                            .ToArray());

                    var result = new BadRequestObjectResult(errorResponse);
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);

                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogWarning("ModelStateValidationError {errorResponse}", JsonConvert.SerializeObject(errorResponse));

                    return result;
                };
                // necessary to disable the defualt ProblemDetails model in Swagger
                options.SuppressMapClientErrors = true;
            });

    }
}
