using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class SanPhamDMViewModel
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnhChinh { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal GiaKM { get; set; } 
        public decimal MucGiam { get; set; } 
    }
}