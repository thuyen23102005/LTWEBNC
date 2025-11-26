using FoodsStore.Infrastructure;
using FoodsStore.Models;
using FoodsStore.Models.ViewModel;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodsStore.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<CartLineVM> Lines { get; set; } = new();
        public double SubTotal => Lines.Sum(l => l.LineTotal);
        public double GrandTotal => Math.Max(0, SubTotal);

        public string ToCurrency(double amount) =>
            string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c0}", amount);

        // GET: lấy giỏ hàng hiện tại
        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Đã đăng nhập -> lấy từ DB
            if (!string.IsNullOrEmpty(userId))
            {
                var rows = await _context.Carts
                    .Include(c => c.Item)
                    .Where(c => c.ApplicationUserId == userId)
                    .ToListAsync();

                Lines = rows.Select(r => new CartLineVM
                {
                    ItemId = r.ItemId,
                    Title = r.Item?.Title ?? "",
                    Price = r.Item?.Price ?? 0,
                    Count = r.Count,
                    ImageUrl = !string.IsNullOrEmpty(r.Item!.ImageUrl)
                        ? r.Item.ImageUrl
                        : "/images/placeholder.png"
                }).ToList();

                return;
            }

            // 2. Khách vãng lai -> lấy từ Session
            var sess = HttpContext.Session.GetJson<List<CartLineVM>>("cart_guest") ?? new List<CartLineVM>();
            Lines = sess;
        }

        // POST: xóa 1 dòng
        public async Task<IActionResult> OnPostDeleteAsync(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var row = await _context.Carts
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ItemId == itemId);

                if (row != null)
                {
                    _context.Carts.Remove(row);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var sess = HttpContext.Session.GetJson<List<CartLineVM>>("cart_guest") ?? new List<CartLineVM>();
                var line = sess.FirstOrDefault(x => x.ItemId == itemId);
                if (line != null)
                {
                    sess.Remove(line);
                    HttpContext.Session.SetJson("cart_guest", sess);
                }
            }

            return RedirectToPage();
        }

        // POST: tăng/giảm số lượng
        public async Task<IActionResult> OnPostUpdateAsync(int itemId, int count)
        {
            if (count < 1) count = 1;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var row = await _context.Carts
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ItemId == itemId);

                if (row != null)
                {
                    row.Count = count;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var sess = HttpContext.Session.GetJson<List<CartLineVM>>("cart_guest") ?? new();
                var line = sess.FirstOrDefault(x => x.ItemId == itemId);
                if (line != null)
                {
                    line.Count = count;
                    HttpContext.Session.SetJson("cart_guest", sess);
                }
            }

            return RedirectToPage();
        }
    }
}
