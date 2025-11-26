using FoodsStore.Infrastructure;
using FoodsStore.Models;
using FoodsStore.Models.ViewModel;
using FoodsStore.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodsStore.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCartCountAsync(ClaimsPrincipal user, HttpContext httpContext)
        {
            // Nếu đã đăng nhập → Lấy từ DB
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                return await _context.Carts
                    .Where(x => x.ApplicationUserId == userId)
                    .SumAsync(x => x.Count);
            }

            // Nếu chưa login → lấy từ Session
            var cart = httpContext.Session.GetJson<List<CartLineVM>>("cart_guest");
            return cart?.Sum(x => x.Count) ?? 0;
        }
    }
}