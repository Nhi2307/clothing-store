using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string MatKhauCu { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string MatKhauMoi { get; set; }

        [Required]
        [Compare("MatKhauMoi", ErrorMessage = "Xác nhận mật khẩu không khớp")]
        public string XacNhanMatKhau { get; set; }
    }
}