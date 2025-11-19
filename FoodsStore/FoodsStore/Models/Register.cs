using System.ComponentModel.DataAnnotations;

namespace FoodsStore.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản.")]
        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        // Họ và tên
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }

        // Thành phố
        [Required(ErrorMessage = "Vui lòng nhập thành phố.")]
        [Display(Name = "Thành phố")]
        public string City { get; set; }

        // Địa chỉ
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        // Mã bưu điện
        [Required(ErrorMessage = "Vui lòng nhập mã bưu điện.")]
        [Display(Name = "Mã bưu điện")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}