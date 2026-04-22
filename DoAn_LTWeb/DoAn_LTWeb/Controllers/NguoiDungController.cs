using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAn_LTWeb.Controllers
{
    public class NguoiDungController : Controller
    {

        Models.QuanLyQuanAoEntities7 db = new Models.QuanLyQuanAoEntities7();

        private bool IsKhachHang()
        {
            return Session["MaNguoiDung"] != null &&
                   Session["VaiTro"]?.ToString() == "KhachHang";
        }

        public ActionResult Profile()
        {
            if (!IsKhachHang())
                return RedirectToAction("DangNhap", "Account");

            int id = Convert.ToInt32(Session["MaNguoiDung"]);
            var user = db.NguoiDungs.Find(id);
            return View(user);
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            if (!IsKhachHang())
                return RedirectToAction("DangNhap", "Account");

            int id = Convert.ToInt32(Session["MaNguoiDung"]);
            var user = db.NguoiDungs.Find(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(NguoiDung model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = db.NguoiDungs.Find(model.MaNguoiDung);
            if (user == null)
                return HttpNotFound();

            user.HoTen = model.HoTen;
            user.SoDienThoai = model.SoDienThoai;
            user.DiaChi = model.DiaChi;
            user.Email = model.Email;

            db.SaveChanges();
            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Profile");
        }
    }
}