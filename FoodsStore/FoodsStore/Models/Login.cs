using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodsStore.Models
{
    public class Login
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tài khoản")]
        public string UserName { get; set; }

        public string RoleUser { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string PasswordUser { get; set; }
    }
}