using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class KhoSanPhamVM
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int? MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }

        public decimal? Gia { get; set; }

        public int? SoLuongTon { get; set; }
        public DateTime? NgayThem { get; set; }
    }
}