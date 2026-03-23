namespace lab1_AS.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<Order>? Orders { get; set; }
        public ICollection<OrderStatusHistory>? StatusHistories { get; set; }
    }
}
