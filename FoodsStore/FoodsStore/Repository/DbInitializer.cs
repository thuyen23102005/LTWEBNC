using FoodsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Repository
{
    public class DbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public void Initializer()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (_context.Roles.Any(x => x.Name == "Admin")) return;
            _roleManager.CreateAsync(new IdentityRole("Manager")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
            var user = new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Admin",
                City = "Xyz",
                Address = "Xyz",
                PostalCode = "333333"
            };

            _userManager.CreateAsync(user, "Admin@123").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, "Admin");

            // Seed cart sample only if empty
            if (!_context.Carts.Any())
            {
                // KHÔNG dùng lại tên 'user' nữa → đổi tên
                var admin = _context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");

                // Lấy item cần seed
                var miTron = _context.Items.FirstOrDefault(i => i.Title == "Mì trộn sa tế");
                var traChanh = _context.Items.FirstOrDefault(i => i.Title == "Trà chanh");

                if (admin != null && miTron != null && traChanh != null)
                {
                    _context.Carts.AddRange(
                        new Cart { ApplicationUserId = admin.Id, ItemId = miTron.Id, Count = 1 },
                        new Cart { ApplicationUserId = admin.Id, ItemId = traChanh.Id, Count = 2 }
                    );
                    _context.SaveChanges();
                }
            }
        }
    }
}
