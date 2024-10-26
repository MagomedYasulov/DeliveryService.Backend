using AutoMapper;
using DeliveryService.Backend.Data;
using DeliveryService.Backend.Data.Entities;
using DeliveryService.Backend.DTOs;
using DeliveryService.Backend.Models;
using DeliveryService.Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;

namespace DeliveryService.Backend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : BaseController
    {
        public OrdersController(IRepository repository, IMapper mapper, IStringLocalizer<BaseController> localizer) : base(repository,localizer,mapper)
        {

        }

        [HttpGet("{orderId}")]
        public ActionResult<OrderDTO> GetOrder(int orderId)
        {
            var order = _repository.GetById<Order, OrderDTO>(orderId, o => o.CityDistrict);
            return Ok(order);
        }

        [HttpGet]
        public ActionResult<OrderDTO[]> GetOrders(OrderFilterViewModel model)
        {
            var endDeliveryTime = model.FirstDeliveryDateTime + model.TimeOffset; 
            var isCityDistrict = model.CityDistrict == null;
            var isFirstDeliveryDateTime = model.FirstDeliveryDateTime == null;
   
            Expression<Func<Order, bool>> exp = o => (isCityDistrict || o.CityDistrict == model.CityDistrict) &&
                                                     (isFirstDeliveryDateTime || (o.DeliveryTime >= model.FirstDeliveryDateTime && o.DeliveryTime <= endDeliveryTime));
            
            var orders = _repository.Get<Order, OrderDTO>(exp);
            return Ok(orders);
        }

        [HttpPost]
        public ActionResult<OrderDTO> Create(OrderViewModel model)
        {
            var order = new Order()
            {
                Weight = model.Weight,  
                DeliveryTime = model.DeliveryTime,
                CityDistrict = model.CityDistrict,
                CreatedAt = DateTime.UtcNow,
            };
            _repository.Create(order);
            _repository.Save();

            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }


        [HttpPut("{orderId}")]
        public ActionResult<OrderDTO> Update(int orderId,OrderViewModel model)
        {
            var order = _repository.GetById<Order>(orderId);
            if (order == null)
                return NotFound(new NotFoundDescription(_localizer["NotFoundOrder"], _localizer["NotFoundOrderDesc", orderId]));

            order.Weight = model.Weight;
            order.DeliveryTime = model.DeliveryTime;
            order.CityDistrict = model.CityDistrict;
            _repository.Update(order);
            _repository.Save();

            var orderDTO = _mapper.Map<OrderDTO>(order);
            return Ok(orderDTO);
        }

        [HttpDelete("{orderId}")]
        public IActionResult Delete(int orderId)
        {
            if (!_repository.Any<Order>(o => o.Id == orderId))
                return NotFound(new NotFoundDescription(_localizer["NotFoundOrder"], _localizer["NotFoundOrderDesc", orderId]));

            _repository.Delete<Order>(o => o.Id == orderId);
            _repository.Save();

            return Ok();
        }
    }
}
