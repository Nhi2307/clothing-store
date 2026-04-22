using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class BienTheDTO
    {
        public int MaMau { get; set; }
        public int MaSize { get; set; }
        public int SoLuongTon { get; set; }

    }

    public class ChiTietSanPhamVM
    {
        public SanPhamVM SanPham { get; set; }

        public List<MauSacVM> MauSacs { get; set; } = new List<MauSacVM>();
        public List<SizeVM> Sizes { get; set; } = new List<SizeVM>();
        public List<SanPhamVM> CungDanhMuc { get; set; } = new List<SanPhamVM>();
        public List<BienTheDTO> CacBienTheSanCo { get; set; } = new List<BienTheDTO>();


        public List<DanhGiaVM> DanhGias { get; set; } = new List<DanhGiaVM>();
        public bool CoTheDanhGia { get; set; } 
        public int TongSoDanhGia { get; set; }
        public double SaoTrungBinh { get; set; }


        public decimal GiaGoc { get; set; }
        public decimal GiaSauGiam { get; set; }
        public bool DangKhuyenMai { get; set; }
        public decimal PhanTramGiam { get; set; }

    }
}