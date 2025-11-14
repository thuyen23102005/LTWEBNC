namespace FoodsStore.Models
{
    public class FoodListViewModel
    {
        public List<FoodItem> ItemsOnPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string CurrentCategory { get; set; }
        public List<string> AllCategories { get; set; }

        public string CurrentSearchTerm { get; set; } // THUỘC TÍNH MỚI
    }
}