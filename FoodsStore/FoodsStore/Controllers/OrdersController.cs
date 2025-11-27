using FoodsStore.Models;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách đơn hàng (có phân trang)
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10; // số đơn hàng mỗi trang

            var ordersQuery = _context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OrderDate);

            int totalOrders = ordersQuery.Count();
            int totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            var orders = ordersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View(orders);
        }

        // Chi tiết đơn hàng
        public IActionResult Details(int id)
        {
            // Lấy header của đơn + thông tin user
            var order = _context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            // --- BẢO VỆ ĐƠN HÀNG ---
            // Nếu user không phải admin thì chỉ được xem đơn của chính mình
            if (!User.IsInRole("Admin"))
            {
                // Lấy id user đang đăng nhập
                var currentUserId = _context.Users
                    .FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;

                if (currentUserId == null || order.ApplicationUserId != currentUserId)
                {
                    return Forbid(); // chặn truy cập
                }
            }
            // ------------------------

            // Load OrderDetails
            var details = _context.OrderDetails
                .Include(d => d.Item)
                .Where(d => d.OrderHeaderId == id)
                .ToList();

            // Truyền details sang view
            ViewBag.OrderDetails = details;

            return View(order);
        }

        // Xóa đơn hàng
        public IActionResult Delete(int id)
        {
            // Lấy header
            var order = _context.OrderHeaders
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Lấy toàn bộ chi tiết đơn liên quan
            var details = _context.OrderDetails
                .Where(d => d.OrderHeaderId == id)
                .ToList();

            // Xóa chi tiết trước (tránh lỗi khóa ngoại)
            _context.OrderDetails.RemoveRange(details);

            // Xóa header
            _context.OrderHeaders.Remove(order);

            _context.SaveChanges();

            TempData["Success"] = $"Đã xóa đơn hàng #{id}.";

            return RedirectToAction(nameof(Index));
        }
    }
}
