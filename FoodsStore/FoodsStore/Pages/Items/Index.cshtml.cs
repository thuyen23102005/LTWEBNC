using FoodsStore.Models;
using FoodsStore.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FoodsStore.Pages.Items
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Item> Items { get; set; }

        public async Task OnGetAsync()
        {
            Items = await _context.Items.Include(i => i.Category).ToListAsync();
        }
    }
}