using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FoodsStore.Infrastructure;
using FoodsStore.Models;
using FoodsStore.Models.ViewModel;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutModel(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===== ViewModel =====
        public List<CartLineVM> Lines { get; set; } = new();
        public double SubTotal { get; set; }
        public double GrandTotal => SubTotal;   // hiện tại chưa áp dụng coupon

        // Thông tin khách nhập
        public class CheckoutInput
        {
            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            [Display(Name = "Họ và tên")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            [Phone]
            [Display(Name = "Số điện thoại")]
            public string Phone { get; set; } = string.Empty;

            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }

            [Display(Name = "Thành phố")]
            public string City { get; set; }

            [Display(Name = "Mã bưu chính")]
            public string PostalCode { get; set; }
        }

        [BindProperty]
        public CheckoutInput Input { get; set; } = new CheckoutInput();

        public string ToCurrency(double value)
            => string.Format("{0:N0} đ", value);

        // ====== Helpers ======
        private async Task LoadCartAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                // giỏ của user đã đăng nhập (DB)
                Lines = await _context.Carts
                    .Include(c => c.Item)
                    .Where(c => c.ApplicationUserId == userId)
                    .Select(c => new CartLineVM
                    {
                        ItemId = c.ItemId,
                        Title = c.Item.Title,
                        Description = c.Item.Description,
                        Price = c.Item.Price,
                        Count = c.Count,
                        ImageUrl = c.Item.ImageUrl
                    })
                    .ToListAsync();
            }
            else
            {
                // giỏ guest (Session)
                Lines = HttpContext.Session
                            .GetJson<List<CartLineVM>>("cart_guest")
                        ?? new List<CartLineVM>();
            }

            SubTotal = Lines.Sum(l => l.LineTotal);
        }

        // ====== GET: /Cart/Checkout ======
        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login?returnUrl=/Cart/Checkout");
            }

            await LoadCartAsync();

            if (Lines == null || !Lines.Any())
                return RedirectToPage("/Cart/Index");

            // Lấy Id của user đang đăng nhập từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                // Lấy thẳng từ bảng AspNetUsers
                var user = await _context.Users.FindAsync(userId);

                if (user != null)
                {
                    Input = new CheckoutInput
                    {
                        Name = string.IsNullOrWhiteSpace(user.Name)
                                        ? user.UserName
                                        : user.Name,
                        Phone = user.PhoneNumber ?? string.Empty,
                        Address = user.Address ?? string.Empty,
                        City = user.City ?? string.Empty,
                        PostalCode = user.PostalCode ?? string.Empty
                    };

                    // Đảm bảo TagHelper dùng giá trị mới này
                    ModelState.Clear();
                }
            }

            return Page();
        }

        // ====== POST: /Cart/Checkout ======
        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login?returnUrl=/Cart/Checkout");
            }

            await LoadCartAsync();

            if (Lines == null || !Lines.Any())
                return RedirectToPage("/Cart/Index");

            if (!ModelState.IsValid)
                return Page();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Tạo OrderHeader (bảng master) :contentReference[oaicite:1]{index=1}
            var header = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                DateOfPick = DateTime.Now,
                TimeofPick = DateTime.Now,
                SubTotal = SubTotal,
                OrderTotal = GrandTotal,
                TransId = Guid.NewGuid().ToString(),
                OrderStatus = "Đang xử lý",
                PaymentStatus = "Chưa thanh toán",
                Name = Input.Name,
                Phone = Input.Phone
            };

            _context.OrderHeaders.Add(header);
            await _context.SaveChangesAsync();  // để có header.Id

            // Tạo OrderDetails cho từng dòng
            foreach (var l in Lines)
            {
                var detail = new OrderDetails
                {
                    OrderHeaderId = header.Id,
                    ItemId = l.ItemId,
                    Name = l.Title,
                    Description = l.Description,
                    Count = l.Count,
                    Price = l.Price
                };
                _context.OrderDetails.Add(detail);
            }

            await _context.SaveChangesAsync();

            // Xoá giỏ hàng sau khi đặt
            if (!string.IsNullOrEmpty(userId))
            {
                var cartRows = _context.Carts.Where(c => c.ApplicationUserId == userId);
                _context.Carts.RemoveRange(cartRows);
                await _context.SaveChangesAsync();
            }
            else
            {
                HttpContext.Session.Remove("cart_guest");
            }

            // Chuyển tới trang cảm ơn (bạn đã có Cart/Success.cshtml)
            return RedirectToPage("/Cart/Success", new { orderId = header.Id });
        }
    }
}
