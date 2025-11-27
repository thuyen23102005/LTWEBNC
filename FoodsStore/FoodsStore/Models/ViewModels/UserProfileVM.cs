namespace FoodsStore.Models.ViewModels
{
    public class UserProfileVM
    {
        public ApplicationUser User { get; set; }
        public List<OrderHeader> Orders { get; set; }
    }
}