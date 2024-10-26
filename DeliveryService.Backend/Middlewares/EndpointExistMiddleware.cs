using DeliveryService.Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DeliveryService.Backend.Middlewares
{
    public class EndpointExistMiddleware
    {
        private readonly RequestDelegate _next;

        public EndpointExistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                await _next(context);
                return;
            }

            var options = context.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
            var serializeSettings = context.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value.SerializerSettings;

            var problemDetails = new ProblemDetails()
            {
                Type = options.Value.ClientErrorMapping[404].Link,
                Title = localizer["ApiNotFound"],
                Status = StatusCodes.Status404NotFound,
                Detail = localizer["ApiNotFoundDetail", context.Request.Path],
                Instance = context.Request.Path
            };
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, serializeSettings));
        }
    }

    public static class EndpointExistMiddlewareeExtensions
    {
        public static IApplicationBuilder UseEndpointExist(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EndpointExistMiddleware>();
        }
    }
}
