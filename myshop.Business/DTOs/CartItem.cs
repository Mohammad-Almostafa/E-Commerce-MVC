namespace myshop.Business.DTOs
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }

        // إجمالي سعر هذا العنصر
        public decimal Total => Price * Quantity;
    }

    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // المجموع الكلي لجميع العناصر في السلة
        public decimal OrderTotal => Items.Sum(x => x.Total);
    }
}