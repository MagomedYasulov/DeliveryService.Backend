using DeliveryService.Backend.ViewModels;
using FluentValidation;

namespace DeliveryService.Backend.Validators
{
    public class OrderValidator : AbstractValidator<OrderViewModel>
    {
        public OrderValidator() 
        {
            RuleFor(o => o.Weight).GreaterThan(0);
            RuleFor(o => o.DeliveryTime).GreaterThan(DateTime.UtcNow);
            RuleFor(o => o.CityDistrict).NotEmpty();
        }
    }
}
