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
                var user = new ApplicationUser { UserName = model.UserName };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Default role = Customer
                    await _userManager.AddToRoleAsync(user, "Customer");

                    return RedirectToAction("Login");
                }

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
                    true,
                    false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "Home", new { area = "Admin" });


                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
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
