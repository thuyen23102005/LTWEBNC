using FoodsStore.Models.ViewModels;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;

        public ItemsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var items = _context.Items.Include(x=>x.Category).ToList();
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            ItemViewModel vm = new ItemViewModel();
            ViewBag.Category = new SelectList(_context.Categories,"Id","Title");
            
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Create(ItemViewModel vm)
        {
            

            return View(vm);
        }
    }
}
