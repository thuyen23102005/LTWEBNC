using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodsStore.Pages.Cart
{
    public class SuccessModel : PageModel
    {
        public int OrderId { get; set; }

        public void OnGet(int orderId)
        {
            OrderId = orderId;
        }
    }
}
