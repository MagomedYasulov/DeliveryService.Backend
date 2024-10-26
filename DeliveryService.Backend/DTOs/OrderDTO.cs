using DeliveryService.Backend.Data.Entities;

namespace DeliveryService.Backend.DTOs
{
    public class OrderDTO : BaseDTO
    {
        /// <summary>
        /// Вес заказа в килограммах
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Район заказа
        /// </summary>
        public string CityDistrict { get; set; } = string.Empty;

        /// <summary>
        /// Время доставки заказа
        /// </summary>
        public DateTime DeliveryTime { get; set; }
    }
}
