using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class KhuyenMaiController : Controller
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();
        public ActionResult Index()
        {
            var today = DateTime.Now;

            var list = db.KhuyenMais
                .Where(km => km.NgayBD <= today && km.NgayKT >= today)
                .Select(km => new KhuyenMaiVM
                {
                    MaKM = km.MaKM,
                    TenKM = km.TenKM,
                    MucGiam = km.MucGiam,
                    MoTa = km.MoTa,
                    NgayKT = km.NgayKT,
                    MaSanPham = km.MaSanPham,
                    TenSanPham = km.SanPham != null ? km.SanPham.TenSanPham : null,
                    AnhBia = km.SanPham != null ? km.SanPham.HinhAnhChinh : null,
                    GiaBan = km.SanPham != null ? km.SanPham.Gia : null
                }).ToList();

            return View(list);
        }
    }
}