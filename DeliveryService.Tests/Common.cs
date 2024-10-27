using DeliveryService.Backend.Controllers;
using DeliveryService.Backend.Data;
using DeliveryService.Backend.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Tests
{
    public class Common
    {
        public const string testRequestPath = "/test/path";
        public const string link404 = "http://linkfornotfound";
        public const string link400 = "http://linkforbadrequest";
        public const string link409 = "http://linkforconflict";

        public static void SetDataInDB(ApplicationContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            if (!dbContext.Orders.Any())
            {
                for (var i = 0; i < 400; i++)
                {
                    var order = new Order()
                    {
                        Weight = Math.Round(GetRandomNumber(0, 10), 3),
                        CityDistrict = "Район " + i % 10,
                        CreatedAt = DateTime.UtcNow,
                        DeliveryTime = DateTime.UtcNow + TimeSpan.FromSeconds(i * 20),
                    };
                    dbContext.Orders.Add(order);
                }
                dbContext.SaveChanges();
            }

            DetachAllEntities(dbContext);
        }

        public static HttpContext GetHttpContext(IStringLocalizer<BaseController> localizer)
        {
            var httpContext = new Mock<HttpContext>();
            var serviceProvider = new Mock<IServiceProvider>();
            var serilizeOptions = new Mock<IOptions<MvcNewtonsoftJsonOptions>>();
            var request = new Mock<HttpRequest>();

            request.Setup(r => r.Path).Returns(testRequestPath);

            var apiBehaviorOptions = new Mock<IOptions<ApiBehaviorOptions>>();
            var apo = new ApiBehaviorOptions();
            apo.ClientErrorMapping[409] = new ClientErrorData() { Link = link409 };
            apo.ClientErrorMapping[400] = new ClientErrorData() { Link = link400 };
            apo.ClientErrorMapping[404] = new ClientErrorData { Link = link404 };
            apo.InvalidModelStateResponseFactory = (ActionContext actionContext) =>
            {
                var localizer = actionContext.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
                var problemDetails = new ValidationProblemDetails(actionContext.ModelState)
                {
                    Type = apo.ClientErrorMapping[400].Link,
                    Title = localizer["Title400"],
                    Status = StatusCodes.Status400BadRequest,
                    Detail = localizer["Detail"],
                    Instance = actionContext.HttpContext.Request.Path
                };
                problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier);
                return new BadRequestObjectResult(problemDetails);
            };

            apiBehaviorOptions.Setup(a => a.Value).Returns(apo);
            serilizeOptions.Setup(so => so.Value).Returns(new MvcNewtonsoftJsonOptions());

            serviceProvider.Setup(s => s.GetService(typeof(IOptions<MvcNewtonsoftJsonOptions>))).Returns(serilizeOptions.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IOptions<ApiBehaviorOptions>))).Returns(apiBehaviorOptions.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IStringLocalizer<BaseController>))).Returns(localizer);

            httpContext.Setup(c => c.RequestServices).Returns(serviceProvider.Object);
            httpContext.Setup(c => c.Request).Returns(request.Object);

            return httpContext.Object;
        }

        public static IStringLocalizer<BaseController> GetLocalizedStrings()
        {
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            var localizer = new StringLocalizer<BaseController>(factory);
            return localizer;
        }

        private static void DetachAllEntities(ApplicationContext dbContext)
        {
            var undetachedEntriesCopy = dbContext.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in undetachedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
