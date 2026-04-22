using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class ChiTietDonHangVM
    {
        public int MaSanPham { get; set; }
        public int MaBienThe { get; set; }

        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        public bool DaDanhGia { get; set; }
    }
}