using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class SanPhamVM
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }

        public decimal Gia { get; set; }               // Giá gốc
        public decimal GiaKhuyenMai { get; set; }     
        public int PhanTramGiam { get; set; }           

        public string MoTa { get; set; }
        public string HinhAnhChinh { get; set; }
    }
}