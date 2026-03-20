namespace lab1_AS.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
