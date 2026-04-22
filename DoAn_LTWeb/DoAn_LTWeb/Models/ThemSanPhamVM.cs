
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class ThemSanPhamVM
    {
        public SanPham SanPham { get; set; }

        public List<int> SelectedSizes { get; set; }
        public List<int> SelectedColors { get; set; }

        public Dictionary<string, int> SoLuongBienThe { get; set; }

    }
}