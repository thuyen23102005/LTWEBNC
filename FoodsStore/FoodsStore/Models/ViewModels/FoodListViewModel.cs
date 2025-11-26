using FoodsStore.Models;

public class FoodListViewModel
{
    public List<Item> ItemsOnPage { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string CurrentCategory { get; set; } = "";
    public List<string> AllCategories { get; set; } = new();
    public string CurrentSearchTerm { get; set; } = "";
}