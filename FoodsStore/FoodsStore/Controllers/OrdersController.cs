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

        public IActionResult Details(int id)
        {
            var order = _context.OrderHeaders
                .Include(o => o.ApplicationUser)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            var details = _context.OrderDetails
                .Include(d => d.Item)
                .Where(d => d.OrderHeaderId == id)
                .ToList();

            ViewBag.OrderDetails = details;

            return View(order);
        }
    }
}
