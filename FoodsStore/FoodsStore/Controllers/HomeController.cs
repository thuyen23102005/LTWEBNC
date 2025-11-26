using FoodsStore.Infrastructure;
using FoodsStore.Models;
using FoodsStore.Models.ViewModel;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CartEntity = FoodsStore.Models.Cart;
using NuGet.Packaging.Signing;
using System.Security.Claims;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index(int pageIndex = 1, string category = "All", string searchTerm = "")
    {
        const int pageSize = 9;

        // Lấy query Item từ DB
        var query = _context.Items
                            .Include(x => x.Category)
                            .AsQueryable();

        // Lọc theo category
        if (!string.IsNullOrEmpty(category) && category != "All")
        {
            query = query.Where(x => x.Category.Title == category);
        }

        // Lọc theo search
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(x => x.Title.Contains(searchTerm));
        }

        // Phân trang
        var totalItems = query.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var itemsOnPage = query
            .OrderBy(x => x.Title) // hoặc Id
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Lấy danh sách category cho filter
        var allCategories = _context.Categories.Select(c => c.Title).ToList();

        var viewModel = new FoodListViewModel
        {
            ItemsOnPage = itemsOnPage,        
            CurrentPage = pageIndex,
            TotalPages = totalPages,
            CurrentCategory = category,
            AllCategories = allCategories,
            CurrentSearchTerm = searchTerm
        };

        return View("~/Views/Home/Index.cshtml", viewModel);
    }

    public IActionResult Detail(int id)
    {
        var item = _context.Items
            .Include(x => x.Category)
            .FirstOrDefault(x => x.Id == id);

        if (item == null) return NotFound();

        return View(item);
    }

    public async Task<IActionResult> AddToCart(int id)
    {
        // Lấy user hiện tại (nếu đăng nhập)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Đã đăng nhập -> lưu vào DB (table Carts)
        if (!string.IsNullOrEmpty(userId))
        {
            var existing = await _context.Carts
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ItemId == id);

            if (existing != null)
            {
                existing.Count++;
            }
            else
            {
                _context.Carts.Add(new CartEntity
                {
                    ApplicationUserId = userId,
                    ItemId = id,
                    Count = 1
                });
            }

            await _context.SaveChangesAsync();
        }
        else
        {
            // 2. Khách vãng lai -> lưu vào Session "cart_guest"
            var item = await _context.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            var sess = HttpContext.Session.GetJson<List<CartLineVM>>("cart_guest")
                       ?? new List<CartLineVM>();

            var line = sess.FirstOrDefault(x => x.ItemId == id);

            if (line != null)
            {
                line.Count++;
            }
            else
            {
                sess.Add(new CartLineVM
                {
                    ItemId = item.Id,
                    Title = item.Title,
                    Price = item.Price,
                    Count = 1,
                    ImageUrl = string.IsNullOrEmpty(item.ImageUrl)
                                ? "/images/placeholder.png"
                                : item.ImageUrl
                });
            }

            HttpContext.Session.SetJson("cart_guest", sess);
        }

        // Xong thì chuyển sang trang giỏ hàng
        return RedirectToPage("/Cart/Index");
    }
}
