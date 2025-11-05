using FoodsStore.Models;
using FoodsStore.Models.ViewModels;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FoodsStore.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var items = _context.Items.Include(x=>x.Category)
                .Select(model => new ItemViewModel()
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId
                })
                .ToList();
            return View(items);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ItemViewModel vm = new ItemViewModel();
            ViewBag.Category = new SelectList(_context.Categories,"Id","Title");
            
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel vm)
        {
            Item model = new Item();
            if (ModelState.IsValid) 
            {
                if (vm.ImageUrl != null && vm.ImageUrl.Length > 0) 
                {
                    var uploadDir = @"Images";
                    var filename = Guid.NewGuid().ToString() + "-" + vm.ImageUrl.FileName;
                    var path = Path.Combine(_environment.WebRootPath, uploadDir, filename);
                    await vm.ImageUrl.CopyToAsync(new FileStream(path, FileMode.Create));
                    model.Image = "/" + uploadDir + "/" + filename;
                }
                model.Price = vm.Price;
                model.Description = vm.Description;
                model.Title = vm.Title;
                model.CategoryId = vm.CategoryId;
                _context.Items.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vm);
        }
    }
}
