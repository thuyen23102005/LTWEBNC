using System.ComponentModel.DataAnnotations;

namespace FoodsStore.Models
{
    public class Login
    {
   

        [Required(ErrorMessage = "Bạn chưa nhập tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string PasswordUser { get; set; }
    }
}