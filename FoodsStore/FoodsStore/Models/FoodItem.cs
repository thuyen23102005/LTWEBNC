using Microsoft.AspNetCore.Mvc;

namespace FoodsStore.Models
{
    public class FoodItem : Controller
    {
        public int Id { get; set; } // để định danh món ăn
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public FoodItem() { } // constructor mặc định
    }
}
