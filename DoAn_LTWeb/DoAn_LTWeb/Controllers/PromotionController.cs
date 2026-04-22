using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class PromotionController : Controller
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
        private bool IsStaffOrAdmin()
        {
            if (Session["MaNguoiDung"] == null)
                return false;

            string vaiTro = Session["VaiTro"]?.ToString();
            return vaiTro == "Admin" || vaiTro == "NhanVien";
        }


        public ActionResult Index()
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");
            var km = db.KhuyenMais
                .Include(k => k.SanPham)
                .OrderByDescending(k => k.NgayBD)
                .ToList();
            return View(km);
        }


        public ActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");
            ViewBag.MaSanPham = new SelectList(db.SanPhams, "MaSanPham", "TenSanPham");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhuyenMai km)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            if (km.NgayKT < km.NgayBD)
            {
                ModelState.AddModelError("", "Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

            bool daTonTai = db.KhuyenMais.Any(k =>
                k.MaSanPham == km.MaSanPham &&
                k.NgayBD <= km.NgayKT &&
                k.NgayKT >= km.NgayBD
            );

            if (daTonTai)
            {
                ModelState.AddModelError("", "Sản phẩm đã có khuyến mãi trong thời gian này");
            }

            if (ModelState.IsValid)
            {
                db.KhuyenMais.Add(km);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaSanPham = new SelectList(db.SanPhams, "MaSanPham", "TenSanPham", km.MaSanPham);
            return View(km);
        }

        public ActionResult Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");
            var km = db.KhuyenMais
               .Include(k => k.SanPham)
               .FirstOrDefault(k => k.MaKM == id);

            if (km == null)
                return HttpNotFound();


            ViewBag.MaSanPham = new SelectList(
                db.SanPhams.ToList(),
                "MaSanPham",
                "TenSanPham",
                km.MaSanPham
            );

            return View(km);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KhuyenMai km)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");
            if (km.NgayKT < km.NgayBD)
            {
                ModelState.AddModelError("", "Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

            if (ModelState.IsValid)
            {
                db.Entry(km).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.MaSanPham = new SelectList(
                db.SanPhams.ToList(),
                "MaSanPham",
                "TenSanPham",
                km.MaSanPham
            );

            return View(km);
        }


        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");
            var km = db.KhuyenMais.Include(k => k.SanPham)
                                  .FirstOrDefault(k => k.MaKM == id);
            if (km == null) return HttpNotFound();
            return View(km);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int MaKM)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");
            var km = db.KhuyenMais.Include(k => k.SanPham).FirstOrDefault(k => k.MaKM == MaKM);
            if (km == null) return HttpNotFound();


            if (km.SanPham != null)
            {

                km.MaSanPham = null;
            }

            db.KhuyenMais.Remove(km);
            db.SaveChanges();

            TempData["Success"] = "Xóa khuyến mãi thành công!";
            return RedirectToAction("Index");
        }
    }
}