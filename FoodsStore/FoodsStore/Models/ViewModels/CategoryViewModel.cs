using System.ComponentModel.DataAnnotations;

namespace FoodsStore.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được bỏ trống")]
        public string Title { get; set; }
    }
}
