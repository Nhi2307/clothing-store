using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class DanhMucController : Controller
    {

        Models.QuanLyQuanAoEntities7 db = new Models.QuanLyQuanAoEntities7();
        public ActionResult IndexDM()
        {
            var now = DateTime.Now;
            var sanPhams = db.SanPhams.ToList()
                .Select(sp =>
                {

                    var km = sp.KhuyenMais
                               .Where(k => k.NgayBD <= now && k.NgayKT >= now)
                               .OrderByDescending(k => k.MucGiam)
                               .FirstOrDefault();

                    return new SanPhamDMViewModel
                    {
                        MaSanPham = sp.MaSanPham,
                        TenSanPham = sp.TenSanPham,
                        HinhAnhChinh = sp.HinhAnhChinh,
                        GiaGoc = sp.Gia ?? 0,
                        GiaKM = km != null ? sp.Gia.Value * (100 - km.MucGiam) / 100 : sp.Gia ?? 0,
                        MucGiam = km?.MucGiam ?? 0
                    };
                })
                .ToList();

            return View(sanPhams);
        }
        public ActionResult DanhMucPartial()
        {
            DBDanhMuc dm = new DBDanhMuc();
            return PartialView(dm.DSDanhMuc());
        }
        public ActionResult SanPhamTheoDM(int id)
        {
            var now = DateTime.Now;

            var sanPhams = db.SanPhams
                              .Where(sp => sp.MaDanhMuc == id)
                              .ToList()
                              .Select(sp =>
                              {
                                  var km = sp.KhuyenMais
                                             .Where(k => k.NgayBD <= now && k.NgayKT >= now)
                                             .OrderByDescending(k => k.MucGiam)
                                             .FirstOrDefault();

                                  return new SanPhamDMViewModel
                                  {
                                      MaSanPham = sp.MaSanPham,
                                      TenSanPham = sp.TenSanPham,
                                      HinhAnhChinh = sp.HinhAnhChinh,
                                      GiaGoc = sp.Gia ?? 0,
                                      GiaKM = km != null ? sp.Gia.Value * (100 - km.MucGiam) / 100 : sp.Gia ?? 0,
                                      MucGiam = km?.MucGiam ?? 0
                                  };
                              })
                              .ToList();

            var dm = db.DanhMucs.Find(id);
            ViewBag.DanhMuc = dm != null ? dm.TenDanhMuc : "Không xác định";

            return View(sanPhams);
        }


        [HttpGet]
        public ActionResult TimKiem(string tuKhoa)
        {
            if (string.IsNullOrEmpty(tuKhoa))
            {
                return RedirectToAction("IndexDM");
            }

            var now = DateTime.Now;

            var sanPhams = db.SanPhams
                             .Where(s => s.TenSanPham.Contains(tuKhoa))
                             .ToList()
                             .Select(sp =>
                             {
                                 var km = sp.KhuyenMais
                                            .Where(k => k.NgayBD <= now && k.NgayKT >= now)
                                            .OrderByDescending(k => k.MucGiam)
                                            .FirstOrDefault();

                                 return new SanPhamDMViewModel
                                 {
                                     MaSanPham = sp.MaSanPham,
                                     TenSanPham = sp.TenSanPham,
                                     HinhAnhChinh = sp.HinhAnhChinh,
                                     GiaGoc = sp.Gia ?? 0,
                                     GiaKM = km != null ? sp.Gia.Value * (100 - km.MucGiam) / 100 : sp.Gia ?? 0,
                                     MucGiam = km?.MucGiam ?? 0
                                 };
                             })
                             .ToList();

            ViewBag.TuKhoa = tuKhoa;
            return View(sanPhams);
        }
    }
}