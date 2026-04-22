using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class DanhGiaVM
    {

        public int MaDG { get; set; }
        public string HoTen { get; set; }          
        public DateTime NgayDG { get; set; }


        public int MaSanPham { get; set; }

        public int MaDonHang { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }


        public int Sao { get; set; }
        public string NoiDung { get; set; }


        public List<PhanHoiDanhGiaVM> PhanHois { get; set; }
            = new List<PhanHoiDanhGiaVM>();
    }
}