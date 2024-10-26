namespace DeliveryService.Backend.Models
{
    public class NotFoundDescription
    {
        public NotFoundDescription() { }

        public NotFoundDescription(string? title = null, string? detail = null)
        {
            Title = title;
            Detail = detail;
        }

        public string? Title { get; set; }

        public string? Detail { get; set; }
    }
}
