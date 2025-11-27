namespace FoodsStore.Models.ViewModel
{
    public class CartLineVM
    {
        public int ItemId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public string? ImageUrl { get; set; }
        public double LineTotal => Price * Count;
    }
}