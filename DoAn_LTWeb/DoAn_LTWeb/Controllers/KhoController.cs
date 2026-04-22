using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class KhoController : Controller
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();
        private bool IsAdmin()
        {
            if (Session["MaNguoiDung"] == null)
                return false;

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            return db.NguoiDungs.Any(u =>
                u.MaNguoiDung == maNguoiDung &&
                u.VaiTro == "Admin");
        }
        public ActionResult Index(string keyword = "", int? maDanhMuc = null)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var query = from sp in db.SanPhams
                        join dm in db.DanhMucs on sp.MaDanhMuc equals dm.MaDanhMuc
                        select new KhoSanPhamVM
                        {
                            MaSanPham = sp.MaSanPham,
                            TenSanPham = sp.TenSanPham,

                            MaDanhMuc = sp.MaDanhMuc,
                            TenDanhMuc = dm.TenDanhMuc,

                            Gia = sp.Gia,
                            SoLuongTon = sp.SoLuongTon,
                            NgayThem = sp.NgayThem
                        };

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.TenSanPham.Contains(keyword));

            if (maDanhMuc.HasValue)
            {
                query = query.Where(x => x.MaDanhMuc == maDanhMuc.Value);
            }

            ViewBag.DanhMuc = db.DanhMucs.ToList();

            return View(query.OrderBy(x => x.SoLuongTon).ToList());
        }
        [HttpPost]
        public ActionResult CapNhatSoLuong(int maSanPham, int soLuongMoi)
        {
            var sp = db.SanPhams.Find(maSanPham);
            if (sp == null)
                return HttpNotFound();

            sp.SoLuongTon = soLuongMoi;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}