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
        public double Discount { get; set; } = 0; // sẽ cập nhật khi áp mã
        public double GrandTotal => Math.Max(0, SubTotal - Discount);

        // Định dạng VND nhanh gọn
        public string ToCurrency(double amount) =>
            string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:c0}", amount);

        // (tùy chọn) handler áp mã giảm giá – demo giảm 10%
        public IActionResult OnPostApplyCoupon(string coupon)
        {
            // ở demo: nếu nhập WELCOME10 => giảm 10%
            if (!string.IsNullOrWhiteSpace(coupon) && coupon.Trim().ToUpper() == "WELCOME10")
            {
                // lưu tạm vào TempData để show (hoặc có thể lưu Session)
                TempData["couponApplied"] = "WELCOME10";
            }
            return RedirectToPage();
        }

        public async Task OnGetAsync()
        {
            // 1) Đã đăng nhập => lấy từ DB
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            // 2) Chưa đăng nhập => lấy từ Session
            var sess = HttpContext.Session.GetJson<List<CartLineVM>>("cart_guest") ?? new List<CartLineVM>();
            Lines = sess;

            // ===== DEMO: nếu giỏ khách đang trống thì thêm vài item mẫu để test UI =====
            if (sess.Count == 0)
            {
                sess.Add(new CartLineVM
                {
                    ItemId = 1,
                    Title = "Mì trộn sa tế",
                    Price = 10000,
                    Count = 1,
                    ImageUrl = "~/images/items/mi_tron.png"
                });
                sess.Add(new CartLineVM
                {
                    ItemId = 3,
                    Title = "Trà chanh mát lạnh",
                    Price = 12000,
                    Count = 2,
                    ImageUrl = "~/images/items/nestle_milo.png"
                });

                // lưu lại vào Session để trang reload vẫn còn
                HttpContext.Session.SetJson("cart_guest", sess);
            }

            // gán ra View
            Lines = sess;
            return;
        }

        // Xóa một dòng (áp dụng cho cả DB và Session)
        public async Task<IActionResult> OnPostDeleteAsync(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var row = await _context.Carts.FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ItemId == itemId);
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

        // Cập nhật số lượng
        public async Task<IActionResult> OnPostUpdateAsync(int itemId, int count)
        {
            if (count < 1) count = 1;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var row = await _context.Carts.FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ItemId == itemId);
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
                    HttpContext.Session.SetJson("cart_guest", sess); // PHẢI set lại
                }
            }
            return RedirectToPage();
        }
    }
}