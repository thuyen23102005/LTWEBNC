using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodsStore.ViewComponents
{
    public class AdminOrderCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public AdminOrderCountViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = await _db.OrderHeaders.CountAsync();
            return View("Default", count);
        }
    }
}
