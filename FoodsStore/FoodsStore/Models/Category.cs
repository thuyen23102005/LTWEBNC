using System.ComponentModel.DataAnnotations;

namespace FoodsStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
