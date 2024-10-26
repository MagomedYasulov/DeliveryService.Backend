namespace DeliveryService.Backend.ViewModels
{
    public class OrderViewModel
    {
        public double Weight { get; set; }

        public string CityDistrict { get; set; } = string.Empty;

        /// <summary>
        /// Время доставки заказа, в UTC
        /// </summary>
        public DateTime DeliveryTime { get; set; }
    }
}
