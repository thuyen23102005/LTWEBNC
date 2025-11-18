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

        // Inject UserManager và SignInManager (ASP.NET Identity)
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Hiển thị trang đăng ký
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
                    // Có thể đăng nhập luôn nếu muốn:
                    // await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Login", "Account");
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

        // Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
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
                    isPersistent: true,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Đăng nhập không thành công. Kiểm tra lại tài khoản hoặc mật khẩu.");
            return View(model);
        }
    }
}