using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAn_LTWeb.Controllers
{
    public class AccountController : Controller
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();
        DBNguoiDung dbNguoiDung = new DBNguoiDung();

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(NguoiDung nd, string matKhauXacNhan)
        {
            if (!dbNguoiDung.EmailHopLe(nd.Email))
            {
                TempData["Error"] = "Email không đúng định dạng!";
                return View(nd);
            }

            if (dbNguoiDung.EmailDaTonTai(nd.Email))
            {
                TempData["Error"] = "Email đã tồn tại!";
                return View(nd);
            }

            if (nd.MatKhau != matKhauXacNhan)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                return View(nd);
            }

            bool result = dbNguoiDung.DangKy(nd);

            if (!result)
            {
                TempData["Error"] = "Đăng ký thất bại, vui lòng thử lại!";
                return View(nd);
            }

            TempData["DangKyThanhCong"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public ActionResult DangNhap(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(string email, string matKhau, string returnUrl = null)
        {
            var user = dbNguoiDung.DangNhap(email, matKhau);

            if (user == null)
            {
                TempData["ErrorLogin"] = "Email hoặc mật khẩu không đúng!";
                return RedirectToAction("DangNhap");
            }

            // Kiểm tra trạng thái tài khoản
            if (user.TrangThai == false)
            {
                TempData["ErrorLogin"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên!";
                return RedirectToAction("DangNhap");
            }

            Session["MaNguoiDung"] = user.MaNguoiDung;
            Session["HoTen"] = user.HoTen;
            Session["VaiTro"] = user.VaiTro.Trim();

            switch (user.VaiTro.Trim())
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");

                case "NhanVien":
                    return RedirectToAction("Index", "NhanVien");

                default:
                    return RedirectToAction("IndexDM", "DanhMuc");
            }
        }


        public ActionResult DangXuat()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap");

            string role = Session["VaiTro"]?.ToString()?.Trim();

            switch (role)
            {
                case "Admin":
                    ViewBag.BackUrl = Url.Action("Index", "Admin");
                    break;

                case "NhanVien":
                    ViewBag.BackUrl = Url.Action("Index", "NhanVien");
                    break;

                case "KhachHang":
                    ViewBag.BackUrl = Url.Action("Profile", "NguoiDung");
                    break;

                default:
                    ViewBag.BackUrl = Url.Action("IndexDM", "DanhMuc");
                    break;
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap");

            if (!ModelState.IsValid)
                return View(model);

            int userId = (int)Session["MaNguoiDung"];
            var user = db.NguoiDungs.Find(userId);

            if (user == null)
                return HttpNotFound();

            if (user.MatKhau != model.MatKhauCu)
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng!");
                return View(model);
            }

            user.MatKhau = model.MatKhauMoi;
            db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";

            string role = user.VaiTro?.Trim();

            switch (role)
            {
                case "Admin":
                    ViewBag.RedirectUrl = Url.Action("Index", "Admin");
                    break;

                case "NhanVien":
                    ViewBag.RedirectUrl = Url.Action("Index", "NhanVien");
                    break;

                case "KhachHang":
                    ViewBag.RedirectUrl = Url.Action("Profile", "NguoiDung");
                    break;

                default:
                    ViewBag.RedirectUrl = Url.Action("IndexDM", "DanhMuc");
                    break;
            }

            return View();
        }

    }
}