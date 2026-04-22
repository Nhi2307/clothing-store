using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class LienHeController : Controller
    {
        Models.QuanLyQuanAoEntities7 db = new Models.QuanLyQuanAoEntities7();

        private bool IsKhachHang()
        {
            return Session["MaNguoiDung"] != null &&
                   Session["VaiTro"]?.ToString() == "KhachHang";
        }
        private bool IsAdminOrNhanVien()
        {
            return Session["MaNguoiDung"] != null &&
                   (Session["VaiTro"]?.ToString() == "Admin"
                 || Session["VaiTro"]?.ToString() == "NhanVien");
        }

        public ActionResult Index()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Contact model)
        {
            if (Session["MaNguoiDung"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để gửi liên hệ!";
                return RedirectToAction("DangNhap", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin!";
                return View(model);
            }

            string userName = Session["HoTen"]?.ToString();

            if (!model.Name.Trim().Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Họ tên không khớp với tài khoản đang đăng nhập!";
                return View(model);
            }

            try
            {
                var lh = new LienHe
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    CreatedAt = DateTime.Now
                };

                db.LienHes.Add(lh);
                db.SaveChanges();

                TempData["Success"] = "Gửi liên hệ thành công!";
                return RedirectToAction("Success");
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra, vui lòng thử lại!";
                return View(model);
            }
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult DSLienHe()
        {
            var lienHes = db.LienHes.OrderByDescending(l => l.CreatedAt).ToList();
            return View(lienHes);
        }

        public ActionResult ChiTiet(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var lienHe = db.LienHes.Find(id);
            if (lienHe == null)
                return HttpNotFound();

            var phanHois = lienHe.PhanHoiLienHes
                                 .OrderBy(p => p.NgayPhanHoi)
                                 .Select(p => new PhanHoiLienHeVM
                                 {
                                     MaPhanHoi = p.MaPhanHoi,
                                     MaLienHe = p.MaLienHe,
                                     NoiDung = p.NoiDung,
                                     TenAdmin = p.NguoiDung.HoTen,
                                     NgayPhanHoi = (DateTime)p.NgayPhanHoi
                                 }).ToList();

            ViewBag.PhanHois = phanHois;

            return View(lienHe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PhanHoi(int maLienHe, string noiDung)
        {
            if (!IsAdminOrNhanVien())
            {
                TempData["Error"] = "Bạn không có quyền thực hiện!";
                return RedirectToAction("DSLienHe");
            }

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                TempData["Error"] = "Nội dung phản hồi không được để trống!";
                return RedirectToAction("ChiTiet", new { id = maLienHe });
            }

            try
            {
                int maNguoiDung = (int)Session["MaNguoiDung"];

                var ph = new PhanHoiLienHe
                {
                    MaLienHe = maLienHe,
                    MaNguoiDung = maNguoiDung, // Admin hoặc Nhân viên
                    NoiDung = noiDung,
                    NgayPhanHoi = DateTime.Now
                };

                db.PhanHoiLienHes.Add(ph);
                db.SaveChanges();

                TempData["Success"] = "Phản hồi liên hệ thành công!";
            }
            catch
            {
                TempData["Error"] = "Có lỗi xảy ra khi phản hồi!";
            }

            return RedirectToAction("ChiTiet", new { id = maLienHe });
        }


        public ActionResult LienHe_PhanHoi()
        {
            if (!IsKhachHang())
                return RedirectToAction("DangNhap", "Account");

            int maNguoiDung = (int)Session["MaNguoiDung"];

            var emailKH = db.NguoiDungs.Where(u => u.MaNguoiDung == maNguoiDung)
                                        .Select(u => u.Email)
                                        .FirstOrDefault();

            var lienHes = db.LienHes
                .Where(l => l.Email == emailKH)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new LienHeVM
                {
                    ID = l.ID,
                    Name = l.Name,
                    Email = l.Email,
                    Message = l.Message,
                    CreatedAt = l.CreatedAt ?? DateTime.Now,
                    PhanHois = l.PhanHoiLienHes.OrderBy(p => p.NgayPhanHoi)
                                .Select(p => new PhanHoiLienHeVM
                                {
                                    MaPhanHoi = p.MaPhanHoi,
                                    MaLienHe = p.MaLienHe,
                                    NoiDung = p.NoiDung,
                                    TenAdmin = p.NguoiDung.HoTen,
                                    NgayPhanHoi = (DateTime)p.NgayPhanHoi
                                }).ToList()
                }).ToList();

            return View(lienHes);
        }


        public ActionResult Xoa(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var lienHe = db.LienHes.Find(id);
            if (lienHe == null)
                return HttpNotFound();

            db.LienHes.Remove(lienHe);
            db.SaveChanges();

            TempData["Success"] = "Xóa liên hệ thành công!";
            return RedirectToAction("DSLienHe");
        }
    }
}