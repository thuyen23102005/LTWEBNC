using FoodsStore.Models;
using FoodsStore.Models.ViewModels;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        // Inject UserManager và SignInManager (ASP.NET Identity)
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        // ---------------- REGISTER -------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Xử lý khi người dùng bấm nút Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng ApplicationUser mới với đầy đủ thông tin từ form
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.UserName,   // nếu UserName là email, giữ dòng này; nếu không thì bỏ
                    Name = model.Name,
                    City = model.City,
                    Address = model.Address,
                    PostalCode = model.PostalCode
                };

                // Tạo user trong database
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Default role = Customer
                    await _userManager.AddToRoleAsync(user, "Customer");

                    return RedirectToAction("Login");
                }

                // Nếu có lỗi, thêm vào ModelState để hiển thị ra form
                foreach (var error in result.Errors)
                {
                    string message = error.Description;

                    if (message.Contains("non alphanumeric"))
                        message = "Mật khẩu phải có ít nhất 1 ký tự đặc biệt.";
                    else if (message.Contains("lowercase"))
                        message = "Mật khẩu phải có ít nhất 1 chữ thường (a–z).";
                    else if (message.Contains("uppercase"))
                        message = "Mật khẩu phải có ít nhất 1 chữ hoa (A–Z).";
                    else if (message.Contains("digit"))
                        message = "Mật khẩu phải có ít nhất 1 chữ số (0–9).";
                    else if (message.Contains("at least"))
                        message = "Mật khẩu quá ngắn.";

                    ModelState.AddModelError("", message);
                }
            }

            // Nếu không hợp lệ, quay lại view và hiển thị lỗi
            return View(model);
        }

        // ---------------- LOGIN -------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ---------------- LOGOUT -------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // ---------------- ACCESS DENIED -------------------
        public IActionResult AccessDenied()
        {
            return Content("Bạn không có quyền truy cập!");
        }

        // Xử lý khi người dùng bấm nút Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.PasswordUser,
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    // Nếu là Admin → chuyển đến Admin Area
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Items", new { area = "Admin" });
                    }

                    // Nếu là Customer → về trang chủ
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
            }
            return View(model);
        }

        // ----------------- MANAGE USER PROFILE --------------------
        [Authorize(Roles = "Admin")]
        public IActionResult Manage(string? search)
        {
            // Query tất cả user
            var usersQuery = _userManager.Users.AsQueryable();

            // Nếu có từ khoá => lọc
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                usersQuery = usersQuery.Where(u =>
                    (!string.IsNullOrEmpty(u.Name) && u.Name.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.City) && u.City.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(u.Address) && u.Address.ToLower().Contains(keyword))
                );
            }

            // Danh sách sau khi lọc
            var users = usersQuery.ToList();

            // Tổng user thật trong hệ thống (không theo filter)
            ViewBag.TotalUsers = _userManager.Users.Count();
            ViewBag.Search = search;

            return View(users);
        }

        // ----------------- VIEW USER PROFILE --------------------
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewUser(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var orders = await _db.OrderHeaders
                .Where(o => o.ApplicationUserId == id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var vm = new UserProfileVM
            {
                User = user,
                Orders = orders
            };

            return View(vm);
        }

        // ----------------- USER PROFILE --------------------
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var orders = await _db.OrderHeaders
                .Where(o => o.ApplicationUserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var model = new UserProfileVM
            {
                User = user,
                Orders = orders
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            user.Name = model.Name;
            user.Address = model.Address;
            user.City = model.City;
            user.PostalCode = model.PostalCode;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile");
        }
    }
}