namespace lab1_AS.Models
{
    public class Review
    {
        public int Id { get; set; }
        int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
