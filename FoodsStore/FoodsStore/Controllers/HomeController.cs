using System.Diagnostics;
using FoodsStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        private static List<FoodItem> foods = new List<FoodItem>
        {
            new FoodItem { Id = 1, Name = "Pizza Ý", Description = "Pizza đậm vị Ý", ImageUrl = "/images/pizza.jpg", Price = 120000 },
            new FoodItem { Id = 2, Name = "Bún bò Huế", Description = "Đậm đà hương vị miền Trung", ImageUrl = "/images/bunbo.jpg", Price = 60000 },
            new FoodItem { Id = 3, Name = "Sushi Nhật", Description = "Tươi ngon, chuẩn vị Nhật", ImageUrl = "/images/sushi.jpg", Price = 90000 },
            new FoodItem { Id = 4, Name = "Cơm tấm", Description = "Cơm tấm sườn bì chả", ImageUrl = "/images/comtam.jpg", Price = 55000 },
            new FoodItem { Id = 5, Name = "Phở bò", Description = "Phở bò truyền thống", ImageUrl = "/images/phobo.jpg", Price = 65000 },
            new FoodItem { Id = 6, Name = "Gỏi cuốn", Description = "Gỏi cuốn tôm thịt", ImageUrl = "/images/goicuon.jpg", Price = 40000 },
            new FoodItem { Id = 7, Name = "Bánh mì", Description = "Bánh mì pate đặc biệt", ImageUrl = "/images/banhmi.jpg", Price = 30000 },
            new FoodItem { Id = 8, Name = "Mì xào", Description = "Mì xào hải sản", ImageUrl = "/images/mixao.jpg", Price = 70000 },
            new FoodItem { Id = 9, Name = "Lẩu thái", Description = "Lẩu thái chua cay", ImageUrl = "/images/lauthai.jpg", Price = 150000 },
            new FoodItem { Id = 10, Name = "Cháo gà", Description = "Cháo gà hành tiêu", ImageUrl = "/images/chaoga.jpg", Price = 45000 },
            new FoodItem { Id = 11, Name = "Bánh xèo", Description = "Bánh xèo miền Tây", ImageUrl = "/images/banhxeo.jpg", Price = 50000 },
            new FoodItem { Id = 12, Name = "Kem tươi", Description = "Kem vani mát lạnh", ImageUrl = "/images/kem.jpg", Price = 25000 },
        };

        public IActionResult Index() => View("~/Views/Food/Index.cshtml", foods);

        public IActionResult Details(int id)
        {
            var item = foods.FirstOrDefault(f => f.Id == id);
            if (item == null) return NotFound();
            return View("~/Views/Food/Detail.cshtml", item);
        }

        public IActionResult AddToCart(int id)
        {
            HttpContext.Session.Remove("Cart"); // reset nếu có dữ liệu sai

            var item = foods.FirstOrDefault(f => f.Id == id);
            if (item != null)
            {
                var cart = HttpContext.Session.GetObject<List<FoodItem>>("Cart") ?? new List<FoodItem>();
                cart.Add(item);
                HttpContext.Session.SetObject("Cart", cart);
            }
            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObject<List<FoodItem>>("Cart") ?? new List<FoodItem>();
            return View("~/Views/Food/Cart.cshtml", cart);
        }
    }
}
