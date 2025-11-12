namespace Banhang.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int Stock { get; set; }
        public int CategoryID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // view helper
        public string? CategoryName { get; set; }
    }
}
