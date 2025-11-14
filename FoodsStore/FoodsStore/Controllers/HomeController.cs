using FoodsStore.Models;
using Microsoft.AspNetCore.Mvc;
using FoodsStore.Infrastructure;
using System.Diagnostics;

namespace FoodsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private static List<FoodItem> foods = new List<FoodItem>
        {
            new FoodItem { Id = 1, Name = "Pizza Ý", Description = "Pizza đậm vị Ý", ImageUrl = "/images/banana.jpg", Price = 120000m, Category = "Pizza & Pasta" },
            new FoodItem { Id = 2, Name = "Bún bò Huế", Description = "Đậm đà hương vị miền Trung", ImageUrl = "/images/banana.jpg", Price = 60000m, Category = "Món Việt" },
            new FoodItem { Id = 3, Name = "Sushi Nhật", Description = "Tươi ngon, chuẩn vị Nhật", ImageUrl = "/images/banana.jpg", Price = 90000m, Category = "Món Nhật" },
            new FoodItem { Id = 4, Name = "Cơm tấm", Description = "Cơm tấm sườn bì chả", ImageUrl = "/images/banana.jpg", Price = 55000m, Category = "Món Việt" },
            new FoodItem { Id = 5, Name = "Phở bò", Description = "Phở bò truyền thống", ImageUrl = "/images/banana.jpg", Price = 65000m, Category = "Món Việt" },
            new FoodItem { Id = 6, Name = "Gỏi cuốn", Description = "Gỏi cuốn tôm thịt", ImageUrl = "/images/banana.jpg", Price = 40000m, Category = "Salad & Gỏi" },
            new FoodItem { Id = 7, Name = "Bánh mì", Description = "Bánh mì pate đặc biệt", ImageUrl = "/images/banana.jpg", Price = 30000m, Category = "Bánh & Fast Food" },
            new FoodItem { Id = 8, Name = "Mì xào", Description = "Mì xào hải sản", ImageUrl = "/images/banana.jpg", Price = 70000m, Category = "Pizza & Pasta" },
            new FoodItem { Id = 9, Name = "Lẩu thái", Description = "Lẩu thái chua cay", ImageUrl = "/images/banana.jpg", Price = 150000m, Category = "Món Thái" },
            new FoodItem { Id = 10, Name = "Cháo gà", Description = "Cháo gà hành tiêu", ImageUrl = "/images/banana.jpg", Price = 45000m, Category = "Món Việt" },
            new FoodItem { Id = 11, Name = "Bánh xèo", Description = "Bánh xèo miền Tây", ImageUrl = "/images/banana.jpg", Price = 50000m, Category = "Món Việt" },
            new FoodItem { Id = 12, Name = "Kem tươi", Description = "Kem vani mát lạnh", ImageUrl = "/images/banana.jpg", Price = 25000m, Category = "Drinks & Dessert" },
            new FoodItem { Id = 13, Name = "Steak Bò Mỹ", Description = "Thịt thăn bò Mỹ, sốt nấm Truffle.", ImageUrl = "/images/banana.jpg", Price = 250000m, Category = "Steak & Burger" },
            new FoodItem { Id = 14, Name = "Burger Gà Phô Mai", Description = "Thịt gà giòn, phô mai Cheddar tan chảy.", ImageUrl = "/images/banana.jpg", Price = 85000m, Category = "Steak & Burger" },
            new FoodItem { Id = 15, Name = "Salad Ceasar", Description = "Salad xà lách Romaine, sốt Ceasar, croutons.", ImageUrl = "/images/banana.jpg", Price = 75000m, Category = "Salad & Gỏi" },
            new FoodItem { Id = 16, Name = "Súp Bí Đỏ Kem", Description = "Súp bí đỏ nhung mịn, ấm nóng.", ImageUrl = "/images/banana.jpg", Price = 40000m, Category = "Món Khai Vị" },
            new FoodItem { Id = 17, Name = "Nước Ép Dưa Hấu", Description = "Nước ép dưa hấu tươi 100%.", ImageUrl = "/images/banana.jpg", Price = 35000m, Category = "Drinks & Dessert" },
            new FoodItem { Id = 18, Name = "Trà Sữa Trân Châu", Description = "Trà sữa Đài Loan truyền thống, trân châu đen.", ImageUrl = "/images/banana.jpg", Price = 45000m, Category = "Drinks & Dessert" },
            new FoodItem { Id = 19, Name = "Mì Ý Sốt Kem Nấm", Description = "Mì Ý sốt kem nấm béo ngậy.", ImageUrl = "/images/banana.jpg", Price = 110000m, Category = "Pizza & Pasta" },
            new FoodItem { Id = 20, Name = "Cơm Chiên Hải Sản", Description = "Cơm chiên tôm, mực, rau củ.", ImageUrl = "/images/banana.jpg", Price = 75000m, Category = "Món Việt" },
            new FoodItem { Id = 21, Name = "Kem Chocolate", Description = "Kem Chocolate Bỉ đậm đặc.", ImageUrl = "/images/banana.jpg", Price = 30000m, Category = "Drinks & Dessert" },
            new FoodItem { Id = 22, Name = "Sữa Chua Trái Cây", Description = "Sữa chua nhà làm, topping trái cây tươi.", ImageUrl = "/images/banana.jpg", Price = 45000m, Category = "Drinks & Dessert" },
            new FoodItem { Id = 23, Name = "Gỏi Ngó Sen Tôm Thịt", Description = "Gỏi ngó sen giòn, tôm, thịt ba chỉ luộc.", ImageUrl = "/images/banana.jpg", Price = 90000m, Category = "Salad & Gỏi" },
            new FoodItem { Id = 24, Name = "Bánh Flan Caramel", Description = "Bánh flan mềm mịn, nước caramel đậm đà.", ImageUrl = "/images/banana.jpg", Price = 30000m, Category = "Drinks & Dessert" }
        };

        // ACTION INDEX MỚI: ĐÃ THÊM THAM SỐ searchTerm
        public IActionResult Index(int pageIndex = 1, string category = "All", string searchTerm = "")
        {
            var allProducts = foods;
            var filteredProducts = allProducts.AsEnumerable(); // Khởi tạo danh sách sản phẩm

            // 1. ÁP DỤNG BỘ LỌC TÌM KIẾM
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term)
                );
            }

            // 2. ÁP DỤNG BỘ LỌC DANH MỤC (trên kết quả tìm kiếm)
            if (category != "All")
            {
                filteredProducts = filteredProducts.Where(p => p.Category == category);
            }

            // Lấy tất cả danh mục hiện có (dùng danh sách gốc foods)
            var allCategories = new List<string> { "All" };
            allCategories.AddRange(foods.Select(p => p.Category).Distinct().OrderBy(c => c));

            if (pageIndex < 1) pageIndex = 1;

            int pageSize = 8;
            int totalItems = filteredProducts.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Điều chỉnh pageIndex nếu vượt quá giới hạn
            if (pageIndex > totalPages && totalPages > 0) pageIndex = totalPages;
            else if (totalPages == 0) pageIndex = 1;

            int skipAmount = (pageIndex - 1) * pageSize;

            // 3. PHÂN TRANG
            var itemsOnPage = filteredProducts
                .Skip(skipAmount)
                .Take(pageSize)
                .ToList();

            // 4. Tạo FoodListViewModel (ĐÃ THÊM CurrentSearchTerm)
            var viewModel = new FoodListViewModel
            {
                ItemsOnPage = itemsOnPage,
                CurrentPage = pageIndex,
                TotalPages = totalPages,
                CurrentCategory = category,
                AllCategories = allCategories,
                CurrentSearchTerm = searchTerm // Truyền từ khóa tìm kiếm hiện tại
            };

            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        public IActionResult Details(int id)
        {
            var item = foods.FirstOrDefault(f => f.Id == id);
            if (item == null) return NotFound();
            return View("~/Views/Home/Detail.cshtml", item);
        }

        public IActionResult AddToCart(int id)
        {
            var item = foods.FirstOrDefault(f => f.Id == id);
            if (item == null) return NotFound();

            // Lấy giỏ từ Session (guest)
            var cart = HttpContext.Session.GetJson<List<FoodItem>>("cart")
                       ?? new List<FoodItem>();

            cart.Add(item);

            // Lưu lại vào Session
            HttpContext.Session.SetJson("cart", cart);

            // Về trang giỏ hàng (Razor Page: /Pages/Cart/Index.cshtml)
            return RedirectToPage("/Cart/Index");
        }

        public IActionResult Cart()
        {
            return RedirectToPage("/Cart/Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
