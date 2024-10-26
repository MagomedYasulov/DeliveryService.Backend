
using DeliveryService.Backend.Controllers;
using DeliveryService.Backend.Data;
using DeliveryService.Backend.Data.Entities;
using DeliveryService.Backend.Middlewares;
using DeliveryService.Backend.Models;
using DeliveryService.Backend.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;

namespace DeliveryService.Backend
{
    public class Program
    {
        private static readonly CultureInfo[] supportedCultures = [new CultureInfo("en"), new CultureInfo("ru")];

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region DB
            builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IRepository, EFRepository<ApplicationContext>>();
            #endregion

            #region Controllers
            builder.Services.AddRouting(r => r.LowercaseUrls = true);
            builder.Services.AddControllers().AddNewtonsoftJson();
            #endregion

            #region Swagger
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "0.00:00:00", Reference = null, Nullable = false });
            });
            #endregion

            #region AutoMapper

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            #endregion

            #region Fluent validation

            builder.Services.AddFluentValidationRulesToSwagger();
            builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
            builder.Services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });

            #endregion

            #region Localization 

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            #endregion

            #region Exception Handler

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            #endregion

            #region Invalid ModelState Response

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var localizer = actionContext.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<BaseController>>();
                    var problemDetails = new ValidationProblemDetails(actionContext.ModelState)
                    {
                        Type = options.ClientErrorMapping[400].Link,
                        Title = localizer["Title400"],
                        Status = StatusCodes.Status400BadRequest,
                        Detail = localizer["Detail"],
                        Instance = actionContext.HttpContext.Request.Path
                    };
                    problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier);
                    return new BadRequestObjectResult(problemDetails);
                };
            });

            #endregion

            #region Logging
            builder.Services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();  // NLog.Extensions.Logging v5 will automatically load from appsettings.json
            });
            #endregion

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                if(!repository.Any<Order>())
                {
                    var rnd = new Random();
                    for(var i=0;i< 400; i++)
                    {
                        var order = new Order()
                        {
                            Weight = Math.Round(GetRandomNumber(0, 10), 3),
                            CityDistrict = "Район " + i % 10,
                            CreatedAt = DateTime.UtcNow,
                            DeliveryTime = DateTime.UtcNow + TimeSpan.FromSeconds(i * 20),                                
                        };
                        repository.Create(order);
                    }
                    repository.Save();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseEndpointExist();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
