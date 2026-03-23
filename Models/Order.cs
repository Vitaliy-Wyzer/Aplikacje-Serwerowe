namespace lab1_AS.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public List<OrderItem>? OrderItems { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; } = null!;
        public ICollection<OrderStatusHistory>? StatusHistory { get; set; }
    }
}
