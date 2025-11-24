using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodsStore.ViewComponents
{
    public class AdminProductCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public AdminProductCountViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = await _db.Items.CountAsync();

            return View("Default", count);
        }
    }
}
