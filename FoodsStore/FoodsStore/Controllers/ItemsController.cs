using FoodsStore.Models;
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

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult Index(string search, string sort, int page = 1)
        {
            int pageSize = 5; // số sản phẩm mỗi trang
            var items = _context.Items.Include(x => x.Category)
                .Select(model => new ItemViewModel()
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    ImagePath = model.ImageUrl
                });

            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x =>
                    x.Title.Contains(search) ||
                    x.Description.Contains(search)
                );
            }
            // Sắp xếp giá
            ViewBag.Sort = sort;

            switch (sort)
            {
                case "price_asc":
                    items = items.OrderBy(x => x.Price);
                    break;

                case "price_desc":
                    items = items.OrderByDescending(x => x.Price);
                    break;

                default:
                    // mặc định không sort
                    break;
            }

            // Tính toán số lượng trang
            int totalItems = items.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy danh sách theo trang hiện tại
            var pagedItems = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền thông tin phân trang qua ViewBag
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Search = search; // giữ lại từ khóa khi bấm phân trang

            return View(pagedItems);
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
                    var uploadDir = @"images";
                    var filename = Guid.NewGuid().ToString() + "-" + vm.ImageUrl.FileName;
                    var path = Path.Combine(_environment.WebRootPath, uploadDir, filename);
                    await vm.ImageUrl.CopyToAsync(new FileStream(path, FileMode.Create));
                    model.ImageUrl = "/" + uploadDir + "/" + filename;
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
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
                return NotFound();

            var vm = new ItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId,
                ImagePath = item.ImageUrl
            };

            ViewBag.Category = new SelectList(_context.Categories, "Id", "Title", item.CategoryId);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ItemViewModel vm)
        {
            // Nếu người dùng không chọn file mới, bỏ validation error cho ImageUrl
            if (vm.ImageUrl == null || vm.ImageUrl.Length == 0)
            {
                ModelState.Remove("ImageUrl");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Category = new SelectList(_context.Categories, "Id", "Title", vm.CategoryId);
                return View(vm);
            }

            var item = _context.Items.Find(vm.Id);
            if (item == null)
                return NotFound();

            // Cập nhật thông tin cơ bản
            item.Title = vm.Title;
            item.Description = vm.Description;
            item.Price = vm.Price;
            item.CategoryId = vm.CategoryId;

            // Nếu có ảnh mới thì xử lý xóa + lưu
            if (vm.ImageUrl != null && vm.ImageUrl.Length > 0)
            {
                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, item.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        // Giải phóng file đang bị giữ trước khi xóa
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Lưu ảnh mới
                var uploadDir = "images";
                var filename = Guid.NewGuid().ToString() + "-" + vm.ImageUrl.FileName;
                var newPath = Path.Combine(_environment.WebRootPath, uploadDir, filename);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await vm.ImageUrl.CopyToAsync(stream);
                }

                item.ImageUrl = "/" + uploadDir + "/" + filename;
            }

            // Nếu không có ảnh mới -> giữ nguyên item.Image
            _context.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
