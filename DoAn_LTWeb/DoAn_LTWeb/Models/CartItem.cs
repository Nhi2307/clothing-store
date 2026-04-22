using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class CartItem
    {
        public int MaBienThe { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }
        public string TenSize { get; set; }
        public string TenMau { get; set; }
        public decimal Gia { get; set; }
        public decimal DonGia { get; set; }   
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
        public bool IsSelected { get; set; }

    }
}