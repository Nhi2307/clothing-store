using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class AdminController : Controller
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


        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var soSanPham = db.SanPhams.Count();

            var soDonHang = db.DonHangs.Count();

            var soKhachHang = db.NguoiDungs.Count(u => u.VaiTro == "KhachHang");

            var tongDoanhThu = db.DonHangs
                                .Where(d => d.TrangThai == "Hoàn thành") 
                                .Sum(d => (decimal?)d.TongTien) ?? 0m;

            var donHangMoi = db.DonHangs
                                .OrderByDescending(d => d.NgayDat)
                                .Take(5) 
                                .ToList();

            ViewBag.TotalProducts = soSanPham;
            ViewBag.TotalOrders = soDonHang;
            ViewBag.TotalCustomers = soKhachHang;
            ViewBag.TotalRevenue = tongDoanhThu;
            ViewBag.RecentOrders = donHangMoi;

            return View();
        }
        public ActionResult Profile()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            int id = Convert.ToInt32(Session["MaNguoiDung"]);
            var admin = db.NguoiDungs.Find(id);
            return View(admin);
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            int id = Convert.ToInt32(Session["MaNguoiDung"]);
            var admin = db.NguoiDungs.Find(id);
            return View(admin);
        }

        [HttpPost]
        public ActionResult EditProfile(NguoiDung model)
        {
            if (!ModelState.IsValid) return View(model);

            var admin = db.NguoiDungs.Find(model.MaNguoiDung);
            if (admin == null) return HttpNotFound();

            admin.HoTen = model.HoTen;
            admin.SoDienThoai = model.SoDienThoai;
            admin.DiaChi = model.DiaChi;
            admin.Email = model.Email;

            db.SaveChanges();
            TempData["Success"] = "Cập nhật hồ sơ admin thành công!";
            return RedirectToAction("Profile");
        }

    }
}