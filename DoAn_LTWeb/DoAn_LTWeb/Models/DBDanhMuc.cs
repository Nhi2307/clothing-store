using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class DBDanhMuc
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();

        public List<DanhMuc> DSDanhMuc()
        {
            return db.DanhMucs.Select(t => t).OrderBy(t => t.TenDanhMuc).Skip(0).Take(14).ToList();
        }
        public List<SanPham> DSSanPham()
        {
            return db.SanPhams.Select(s => s).OrderBy(s => s.TenSanPham).Skip(0).Take(24).ToList<SanPham>();
        }
        public List<SanPham> SanPham_TheoDanhMuc(int pMaDM)
        {
            return db.SanPhams.Where(t => t.MaDanhMuc == pMaDM).ToList();
        }
        public string TenDM_DanhMuc(int pMaDM)
        {
            return db.DanhMucs.Where(t => t.MaDanhMuc == pMaDM).FirstOrDefault().TenDanhMuc.ToString();
        }
    }
}