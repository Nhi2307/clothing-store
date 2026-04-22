using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class NhanVienController : Controller
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();
        private bool IsNhanVien()
        {
            if (Session["MaNguoiDung"] == null)
                return false;

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            return db.NguoiDungs.Any(u =>
                u.MaNguoiDung == maNguoiDung &&
                (u.VaiTro == "NhanVien" || u.VaiTro == "Admin"));
        }

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
            if (!IsNhanVien())
                return RedirectToAction("DangNhap", "Account");

            ViewBag.TotalProducts = db.SanPhams.Count();

            ViewBag.TotalOrders = db.DonHangs.Count();

            ViewBag.PendingOrders = db.DonHangs
                                      .Count(d => d.TrangThai == "Chờ xác nhận");

            ViewBag.PendingOrderList = db.DonHangs
                                         .Where(d => d.TrangThai == "Chờ xác nhận")
                                         .OrderByDescending(d => d.NgayDat)
                                         .ToList();

            ViewBag.RecentOrders = db.DonHangs
                                     .Include("NguoiDung")
                                     .OrderByDescending(d => d.NgayDat)
                                     .Take(5)
                                     .ToList();


            return View();
        }


        public ActionResult DonHang()
        {
            if (!IsNhanVien())
                return RedirectToAction("DangNhap", "Account");

            return View(db.DonHangs.OrderByDescending(d => d.NgayDat).ToList());
        }

        [HttpPost]
        public ActionResult CapNhatTrangThai(int maDonHang, string trangThai)
        {
            if (!IsNhanVien())
                return RedirectToAction("DangNhap", "Account");

            var donHang = db.DonHangs.Find(maDonHang);
            if (donHang == null)
                return HttpNotFound();

            donHang.TrangThai = trangThai;
            db.SaveChanges();

            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("DonHang");
        }

        public ActionResult Profile()
        {
            if (!IsNhanVien())
                return RedirectToAction("DangNhap", "Account");

            int id = Convert.ToInt32(Session["MaNguoiDung"]);
            return View(db.NguoiDungs.Find(id));
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (!IsNhanVien())
                return RedirectToAction("DangNhap", "Account");

            int id = Convert.ToInt32(Session["MaNguoiDung"]);
            var nhanVien = db.NguoiDungs.Find(id);

            if (nhanVien == null)
                return HttpNotFound();

            return View(nhanVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(NguoiDung model)
        {
            if (!IsNhanVien())
                return RedirectToAction("DangNhap", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var nhanVien = db.NguoiDungs.Find(model.MaNguoiDung);
            if (nhanVien == null)
                return HttpNotFound();

            nhanVien.HoTen = model.HoTen;
            nhanVien.SoDienThoai = model.SoDienThoai;
            nhanVien.DiaChi = model.DiaChi;
            nhanVien.Email = model.Email;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật hồ sơ nhân viên thành công!";
            return RedirectToAction("Profile");
        }
        public ActionResult DSNhanVien(string keyword)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var query = db.NguoiDungs
                          .Where(u => u.VaiTro == "NhanVien");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();

                query = query.Where(u =>
                    u.HoTen.ToLower().Contains(" " + keyword) || 
                    u.HoTen.ToLower().StartsWith(keyword)        
                );
            }

            ViewBag.Keyword = keyword;
            return View(query.ToList());
        }


        [HttpGet]
        public ActionResult ThemNhanVien()
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNhanVien(NhanVienVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            if (!ModelState.IsValid)
                return View(vm);

            // ❗ Kiểm tra trùng tài khoản / email
            if (db.NguoiDungs.Any(u => u.TaiKhoan == vm.TaiKhoan))
            {
                ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại");
                return View(vm);
            }

            if (db.NguoiDungs.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(vm);
            }

            var nv = new NguoiDung
            {
                HoTen = vm.HoTen,
                Email = vm.Email,
                TaiKhoan = vm.TaiKhoan,
                SoDienThoai = vm.SoDienThoai,
                DiaChi = vm.DiaChi,
                VaiTro = "NhanVien",
                NgayTao = DateTime.Now,
                MatKhau = ""
            };

            db.NguoiDungs.Add(nv);
            db.SaveChanges();

            TempData["Success"] = "Thêm nhân viên thành công!";
            return RedirectToAction("DSNhanVien");
        }


        [HttpGet]
        public ActionResult SuaNhanVien(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var nv = db.NguoiDungs.Find(id);
            if (nv == null || nv.VaiTro != "NhanVien")
                return HttpNotFound();

            var vm = new NhanVienVM
            {
                MaNguoiDung = nv.MaNguoiDung,
                HoTen = nv.HoTen,
                Email = nv.Email,
                TaiKhoan = nv.TaiKhoan,
                SoDienThoai = nv.SoDienThoai,
                DiaChi = nv.DiaChi
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaNhanVien(NhanVienVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            if (!ModelState.IsValid)
                return View(vm);

            var nv = db.NguoiDungs.Find(vm.MaNguoiDung);
            if (nv == null)
                return HttpNotFound();

            if (db.NguoiDungs.Any(u => u.Email == vm.Email && u.MaNguoiDung != vm.MaNguoiDung))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(vm);
            }

            nv.HoTen = vm.HoTen;
            nv.Email = vm.Email;
            nv.TaiKhoan = vm.TaiKhoan;
            nv.SoDienThoai = vm.SoDienThoai;
            nv.DiaChi = vm.DiaChi;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật nhân viên thành công!";
            return RedirectToAction("DSNhanVien");
        }



        public ActionResult XoaNhanVien(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var nv = db.NguoiDungs.Find(id);
            if (nv == null || nv.VaiTro != "NhanVien")
                return HttpNotFound();

            db.NguoiDungs.Remove(nv);
            db.SaveChanges();

            TempData["Success"] = "Xóa nhân viên thành công!";
            return RedirectToAction("DSNhanVien");
        }
    }
}