using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class NhanVienVM
    {
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [RegularExpression(
            @"^[^@\s]+@[^@\s]+\.(com|vn|net|org)$",
            ErrorMessage = "Email không đúng định dạng (vd: abc@gmail.com)"
        )]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string TaiKhoan { get; set; }

        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }
}