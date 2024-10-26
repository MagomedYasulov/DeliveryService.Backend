using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Backend.ViewModels
{
    public class OrderFilterViewModel
    {
        /// <summary>
        /// Район заказа. Опционально
        /// </summary>
        [FromQuery]
        public string? CityDistrict { get; set; }

        /// <summary>
        /// Время первого заказа. Опционально
        /// </summary>
        [FromQuery]
        public DateTime? FirstDeliveryDateTime { get; set; }

        /// <summary>
        /// Ближайшее время после первого заказа. Значение по умолчанию 30 минут. Опционально
        /// </summary>
        [FromQuery]
        public TimeSpan TimeOffset { get; set; } = TimeSpan.FromMinutes(30);
    }
}
