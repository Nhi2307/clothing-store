using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class DonHangVM
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }

        public List<ChiTietDonHangVM> ChiTiet { get; set; }
    }
}