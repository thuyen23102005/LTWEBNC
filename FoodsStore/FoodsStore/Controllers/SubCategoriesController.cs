using FoodsStore.Models;
using FoodsStore.Models.ViewModels;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Controllers
{
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var subCategory = _context.SubCategories.Include(x => x.Category).ToList();
            return View(subCategory);
        }
        [HttpGet]
        public IActionResult Create()
        {
            SubCategoryViewModel vm = new SubCategoryViewModel();
            ViewBag.category = new SelectList(_context.Categories, "Id","Title");

            return View(vm);
        }
        [HttpPost]
        public IActionResult Create(SubCategoryViewModel vm)
        {
            SubCategory model = new SubCategory();
            if (ModelState.IsValid)
            {
                model.Title = vm.Title;
                model.CategoryId = vm.CategoryId;
                _context.SubCategories.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            SubCategoryViewModel vm = new SubCategoryViewModel();
            var subCategory = _context.SubCategories.Where(x => x.Id == id).FirstOrDefault();
            if (subCategory != null)
            {
                vm.Id = subCategory.Id;
                vm.Title = subCategory.Title;
                ViewBag.category = new SelectList(_context.Categories, "Id", "Title", subCategory.CategoryId);
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(SubCategoryViewModel vm)
        {
            SubCategory model = new SubCategory();
            if (ModelState.IsValid)
            {
                model.Title = vm.Title;
                model.CategoryId = vm.CategoryId;
                _context.SubCategories.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var subCategory = _context.SubCategories.Where(x => x.Id == id).FirstOrDefault();
            if (subCategory != null)
            {
                _context.SubCategories.Remove(subCategory);
                _context.SaveChanges();
                
            }
            return RedirectToAction("Index");
        }
    }
}
