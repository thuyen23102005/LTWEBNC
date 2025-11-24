using FoodsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FoodsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // ---------------- REGISTER -------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
<<<<<<< HEAD
                var user = new ApplicationUser { UserName = model.UserName };
=======
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
>>>>>>> ec3b59adc5b22753f875eae824a4a0d2978043a6

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
<<<<<<< HEAD
                    // Default role = Customer
                    await _userManager.AddToRoleAsync(user, "Customer");

                    return RedirectToAction("Login");
                }

=======
                    // Có thể đăng nhập luôn nếu muốn:
                    // await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Login", "Account");
                }

                // Nếu có lỗi, thêm vào ModelState để hiển thị ra form
>>>>>>> ec3b59adc5b22753f875eae824a4a0d2978043a6
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // ---------------- LOGIN -------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.PasswordUser,
<<<<<<< HEAD
                    true,
                    false);
=======
                    isPersistent: true,
                    lockoutOnFailure: false);
>>>>>>> ec3b59adc5b22753f875eae824a4a0d2978043a6

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "Home", new { area = "Admin" });


                    return RedirectToAction("Index", "Home");
                }
            }

<<<<<<< HEAD
            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
=======
            ModelState.AddModelError("", "Đăng nhập không thành công. Kiểm tra lại tài khoản hoặc mật khẩu.");
>>>>>>> ec3b59adc5b22753f875eae824a4a0d2978043a6
            return View(model);
        }

        // ---------------- LOGOUT -------------------
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ---------------- CREATE ADMIN QUICK -------------------
        // Chạy 1 lần tại /Account/CreateAdmin
        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            if (await _userManager.FindByNameAsync("admin") != null)
                return Content("Admin đã tồn tại!");

            var adminUser = new ApplicationUser { UserName = "admin" };
            var result = await _userManager.CreateAsync(adminUser, "123456");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                return Content("Tạo admin thành công: user=admin pass=123456");
            }

            return Content("Tạo Admin thất bại!");
        }

        // ---------------- ACCESS DENIED -------------------
        public IActionResult AccessDenied()
        {
            return Content("Bạn không có quyền truy cập!");
        }

    }
}