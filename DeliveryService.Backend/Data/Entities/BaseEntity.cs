namespace DeliveryService.Backend.Data.Entities
{
    public class BaseEntity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
