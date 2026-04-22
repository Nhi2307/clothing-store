using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class KhuyenMaiVM
    {
        public int MaKM { get; set; }
        public string TenKM { get; set; }
        public decimal MucGiam { get; set; }
        public string MoTa { get; set; }
        public DateTime NgayKT { get; set; }


        public int? MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string AnhBia { get; set; }
        public decimal? GiaBan { get; set; }

        public decimal? GiaSauGiam
        {
            get
            {
                if (GiaBan == null) return null;
                return GiaBan - (GiaBan * MucGiam / 100);
            }
        }
    }
}