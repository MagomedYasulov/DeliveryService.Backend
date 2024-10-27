using AutoMapper;
using DeliveryService.Backend.Controllers;
using DeliveryService.Backend.Data;
using DeliveryService.Backend.Data.Entities;
using DeliveryService.Backend.DTOs;
using DeliveryService.Backend.Models;
using DeliveryService.Backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace DeliveryService.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ApplicationContext _dbContext;
        private readonly IRepository _repository;
        private readonly IStringLocalizer<BaseController> _localizer;
        private readonly Mock<ILogger<OrdersController>> _logger = new();

        public OrdersControllerTests() 
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseInMemoryDatabase(databaseName: "OrdersControllerTests");
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));

            _dbContext = new ApplicationContext(optionsBuilder.Options);

            _mapper = new Mapper(mapperConfig);
            _repository = new EFRepository<ApplicationContext>(_dbContext, _mapper);

            var configurationManager = new ConfigurationManager();
            _configuration = configurationManager.AddJsonFile("appsettings.json").Build();

            _localizer = Common.GetLocalizedStrings();

            Common.SetDataInDB(_dbContext);
        }

        [Fact]
        public void Get_Order_By_Id()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);

            // Act
            var result = ordersController.GetOrder(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orderDTO = Assert.IsType<OrderDTO>(okResult.Value);
            Assert.NotNull(orderDTO);
        }

        [Fact]
        public void Get_Not_Exist_Order_By_Id()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            ordersController.ControllerContext.HttpContext = Common.GetHttpContext(_localizer);

            // Act
            var result = ordersController.GetOrder(999999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var problemDetails = Assert.IsAssignableFrom<ProblemDetails>(notFoundResult.Value);

            Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
            Assert.Equal(_localizer["NotFoundOrder"], problemDetails.Title);
            Assert.Equal(_localizer["NotFoundOrderDesc", 999999], problemDetails.Detail);
            Assert.Equal(Common.testRequestPath, problemDetails.Instance);
            Assert.Equal(Common.link404, problemDetails.Type);
        }

        [Fact]
        public void Get_All_Orders()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            var model = new OrderFilterViewModel();
           
            // Act
            var result = ordersController.GetOrders(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var ordersDTO = Assert.IsAssignableFrom<IEnumerable<OrderDTO>>(okResult.Value);
            Assert.NotNull(ordersDTO);
            Assert.All(ordersDTO, Assert.NotNull);
            Assert.Equal(_repository.GetAll<Order>().Count(), ordersDTO.Count());
        }

        [Fact]
        public void Get_Orders_By_CityDistrict()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            var model = new OrderFilterViewModel()
            { 
                CityDistrict = "Район 0"
            };


            // Act
            var result = ordersController.GetOrders(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var ordersDTO = Assert.IsAssignableFrom<IEnumerable<OrderDTO>>(okResult.Value);
            Assert.NotNull(ordersDTO);
            Assert.All(ordersDTO, Assert.NotNull);
            Assert.Equal(_repository.Get<Order>(o => o.CityDistrict == model.CityDistrict).Count(), ordersDTO.Count());
        }

        [Fact]
        public void Get_Orders_By_FirstDeliveryDateTime()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            var model = new OrderFilterViewModel()
            {
                 FirstDeliveryDateTime = DateTime.UtcNow + TimeSpan.FromHours(1)
            };


            // Act
            var result = ordersController.GetOrders(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var ordersDTO = Assert.IsAssignableFrom<IEnumerable<OrderDTO>>(okResult.Value);
            Assert.NotNull(ordersDTO);
            Assert.All(ordersDTO, Assert.NotNull);

            var endDeliveryTime = model.FirstDeliveryDateTime + model.TimeOffset;
            Assert.Equal(_repository.Get<Order>(o => o.DeliveryTime >= model.FirstDeliveryDateTime && o.DeliveryTime <= endDeliveryTime).Count(), ordersDTO.Count());
        }

        [Fact]
        public void Get_Orders_By_CityDistrict_And_FirstDeliveryDateTime()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            var model = new OrderFilterViewModel()
            {
                CityDistrict = "Район 0",
                FirstDeliveryDateTime = DateTime.UtcNow + TimeSpan.FromHours(1)
            };


            // Act
            var result = ordersController.GetOrders(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var ordersDTO = Assert.IsAssignableFrom<IEnumerable<OrderDTO>>(okResult.Value);
            Assert.NotNull(ordersDTO);
            Assert.All(ordersDTO, Assert.NotNull);

            var endDeliveryTime = model.FirstDeliveryDateTime + model.TimeOffset;
            Assert.Equal(_repository.Get<Order>(o => o.CityDistrict == model.CityDistrict && o.DeliveryTime >= model.FirstDeliveryDateTime && o.DeliveryTime <= endDeliveryTime).Count(), ordersDTO.Count());
        }


        [Fact]
        public void Create_Order()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            var model = new OrderViewModel()
            {
                CityDistrict = "Test CityDistrict",
                DeliveryTime = DateTime.UtcNow + TimeSpan.FromMinutes(45),
                Weight = 1.34
            };

            // Act
            var result = ordersController.Create(model);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            var orderDTO = Assert.IsAssignableFrom<OrderDTO>(createdResult.Value);

            Assert.Equal(model.CityDistrict, orderDTO.CityDistrict);
            Assert.Equal(model.DeliveryTime, orderDTO.DeliveryTime);
            Assert.Equal(model.Weight, orderDTO.Weight);
            Assert.NotEqual(0, orderDTO.Id);

            Assert.True(_repository.Any<Order>(o => o.Id == orderDTO.Id));
        }

        [Fact]
        public void Update_Order()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            var model = new OrderViewModel()
            {
                CityDistrict = "Updated CityDistrict",
                DeliveryTime = DateTime.UtcNow + TimeSpan.FromMinutes(25),
                Weight = 1.24
            };

            // Act
            var result = ordersController.Update(1, model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orderDTO = Assert.IsAssignableFrom<OrderDTO>(okResult.Value);

            Assert.Equal(model.CityDistrict, orderDTO.CityDistrict);
            Assert.Equal(model.DeliveryTime, orderDTO.DeliveryTime);
            Assert.Equal(model.Weight, orderDTO.Weight);
            Assert.Equal(1, orderDTO.Id);

            var order = _repository.GetById<Order>(1);
            Assert.Equal(model.CityDistrict, order!.CityDistrict);
            Assert.Equal(model.DeliveryTime, order.DeliveryTime);
            Assert.Equal(model.Weight, order.Weight);
        }


        [Fact]
        public void Delete_Order()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);

            // Act
            var result = ordersController.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.False(_repository.Any<Order>(c => c.Id == 1));
        }

        [Fact]
        public void Delete_Not_Exist_Order()
        {
            // Arrange
            var ordersController = new OrdersController(_repository, _mapper, _localizer, _logger.Object);
            ordersController.ControllerContext.HttpContext = Common.GetHttpContext(_localizer);

            // Act
            var result = ordersController.Delete(99999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var problemDetails = Assert.IsAssignableFrom<ProblemDetails>(notFoundResult.Value);

            Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
            Assert.Equal(_localizer["NotFoundOrder"], problemDetails.Title);
            Assert.Equal(_localizer["NotFoundOrderDesc", 99999], problemDetails.Detail);
            Assert.Equal(Common.testRequestPath, problemDetails.Instance);
            Assert.Equal(Common.link404, problemDetails.Type);
        }
    }

    
}
