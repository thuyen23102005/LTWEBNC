using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FoodsStore.ViewComponents
{
    public class AdminCategoryCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public AdminCategoryCountViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = await _db.Categories.CountAsync();
            return View("Default", count);
        }
    }
}
