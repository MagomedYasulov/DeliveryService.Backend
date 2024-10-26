using AutoMapper;
using DeliveryService.Backend.Data;
using DeliveryService.Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace DeliveryService.Backend.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly IRepository _repository;
        protected readonly IStringLocalizer<BaseController> _localizer;

        public BaseController(IRepository repository, IStringLocalizer<BaseController> localizer, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _localizer = localizer;
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

        public override ConflictObjectResult Conflict(ModelStateDictionary modelState)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Type = options.Value.ClientErrorMapping[409].Link,
                Title = _localizer["Title409"],
                Status = StatusCodes.Status409Conflict,
                Detail = _localizer["Detail"],
                Instance = HttpContext.Request.Path
            };
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return new ConflictObjectResult(problemDetails);
        }

        public override NotFoundObjectResult NotFound([ActionResultObjectValue] object? value)
        {
            var description = value as NotFoundDescription;
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            var problemDetails = new ProblemDetails()
            {
                Type = options.Value.ClientErrorMapping[404].Link,
                Title = description?.Title ?? _localizer["Title404"],
                Status = StatusCodes.Status404NotFound,
                Detail = description?.Detail ?? _localizer["Title404"],
                Instance = HttpContext.Request.Path
            };
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return new NotFoundObjectResult(problemDetails);
        }
    }
}
