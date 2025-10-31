using FoodsStore.Infrastructure;
using FoodsStore.Models;
using FoodsStore.Models.ViewModel;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CartEntity = FoodsStore.Models.Cart;

namespace FoodsStore.Pages.Cart
{
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty] public int ItemId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Nếu đăng nhập -> DB
            if (!string.IsNullOrEmpty(userId))
            {
                var existing = await _context.Carts
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ItemId == ItemId);

                if (existing != null) existing.Count++;
                else _context.Carts.Add(new CartEntity { ApplicationUserId = userId, ItemId = ItemId, Count = 1 });

                await _context.SaveChangesAsync();
                return RedirectToPage("/Cart/Index");
            }

            // Nếu chưa đăng nhập -> Session
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == ItemId);
            if (item == null) return RedirectToPage("/Items/Index");

            var sess = HttpContext.Session.GetJson<List<CartLineVM>>("cart_guest") ?? new List<CartLineVM>();
            var line = sess.FirstOrDefault(x => x.ItemId == ItemId);
            if (line != null) line.Count++;
            else sess.Add(new CartLineVM { ItemId = item.Id, Title = item.Title, Price = item.Price, Count = 1 });

            HttpContext.Session.SetJson("cart_guest", sess);
            return RedirectToPage("/Cart/Index");
        }
    }
}