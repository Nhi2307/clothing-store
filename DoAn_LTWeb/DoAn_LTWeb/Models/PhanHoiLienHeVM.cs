using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class PhanHoiLienHeVM
    {
        public int MaPhanHoi { get; set; }
        public int MaLienHe { get; set; }
        public string NoiDung { get; set; }
        public string TenAdmin { get; set; }
        public DateTime NgayPhanHoi { get; set; }
    }

    public class LienHeVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PhanHoiLienHeVM> PhanHois { get; set; } = new List<PhanHoiLienHeVM>();
    }
}