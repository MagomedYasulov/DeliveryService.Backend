namespace DeliveryService.Backend.Data.Entities
{
    public class Order : BaseEntity
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
