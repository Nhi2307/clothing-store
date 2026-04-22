using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class DanhGiaController : Controller
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

        private bool IsKhachHang()
        {
            return Session["MaNguoiDung"] != null &&
                   Session["VaiTro"]?.ToString() == "KhachHang";
        }

        public ActionResult Index()
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var ds = db.DanhGias
                .Include("NguoiDung")
                .Include("SanPham")
                .Include("PhanHoiDanhGias")
                .OrderByDescending(d => d.NgayDG)
                .ToList();

            ViewBag.ThongKeSao = ds
                .GroupBy(d => d.Sao)
                .Select(g => new
                {
                    Sao = g.Key,
                    SoLuong = g.Count()
                }).ToList();

            ViewBag.TongDanhGia = ds.Count;

            return View(ds);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var dg = db.DanhGias.Find(id);
            if (dg == null) return HttpNotFound();

            var ph = db.PhanHoiDanhGias.Where(p => p.MaDG == id).ToList();
            db.PhanHoiDanhGias.RemoveRange(ph);

            db.DanhGias.Remove(dg);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int maDG, string noiDung)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            int maNguoiDung = (int)Session["MaNguoiDung"];


            var ph = new PhanHoiDanhGia
            {
                MaDG = maDG,
                MaNguoiDung = maNguoiDung,
                NoiDung = noiDung,
                NgayPhanHoi = DateTime.Now
            };

            db.PhanHoiDanhGias.Add(ph);
            db.SaveChanges();

            TempData["Success"] = "Phản hồi đánh giá thành công!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var dg = db.DanhGias.Find(id);
            if (dg == null) return HttpNotFound();

            dg.TrangThai = !dg.TrangThai;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ThemDanhGia(int id)
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap", "Account");

            int maNguoiDung = (int)Session["MaNguoiDung"];

            bool daMuaVaHoanThanh = db.ChiTietDonHangs.Any(ct =>
                ct.SanPhamBienThe.MaSanPham == id &&
                ct.DonHang.MaNguoiDung == maNguoiDung &&
                ct.DonHang.TrangThai == "Hoàn thành"
            );

            if (!daMuaVaHoanThanh)
            {
                TempData["Error"] = "Bạn chỉ có thể đánh giá sản phẩm khi đơn hàng đã hoàn thành!";
                return RedirectToAction("LichSuDonHang", "Order");
            }

            bool daDanhGia = db.DanhGias.Any(dg =>
                dg.MaNguoiDung == maNguoiDung &&
                dg.MaSanPham == id
            );

            if (daDanhGia)
            {
                TempData["Error"] = "Bạn đã đánh giá sản phẩm này rồi!";
                return RedirectToAction("LichSuDonHang", "Order");
            }

            int maDonHang = db.ChiTietDonHangs
                .Where(ct =>
                    ct.SanPhamBienThe.MaSanPham == id &&
                    ct.DonHang.MaNguoiDung == maNguoiDung &&
                    ct.DonHang.TrangThai == "Hoàn thành")
                .Select(ct => ct.MaDonHang)
                .FirstOrDefault();

            return View(new DanhGiaVM
            {
                MaSanPham = id,
                MaDonHang = maDonHang
            });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDanhGia(DanhGiaVM model)
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap", "Account");

            int maNguoiDung = (int)Session["MaNguoiDung"];

            if (model.Sao <= 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn số sao!");
                return View(model);
            }

            bool daDanhGia = db.DanhGias.Any(x =>
                x.MaNguoiDung == maNguoiDung &&
                x.MaSanPham == model.MaSanPham
            );

            if (daDanhGia)
            {
                TempData["Error"] = "Bạn đã đánh giá sản phẩm này rồi!";
                return RedirectToAction("LichSuDonHang", "Order");
            }

            var dg = new DanhGia
            {
                MaSanPham = model.MaSanPham,
                MaNguoiDung = maNguoiDung,
                MaDonHang = model.MaDonHang,
                Sao = model.Sao,
                NoiDung = model.NoiDung,
                NgayDG = DateTime.Now,
                TrangThai = true
            };

            db.DanhGias.Add(dg);
            db.SaveChanges();

            TempData["Success"] = "Gửi đánh giá thành công!";
            return RedirectToAction("LichSuDonHang", "Order");
        }

        public ActionResult DanhGia_PhanHoi()
        {
            if (!IsKhachHang())
                return RedirectToAction("DangNhap", "Account");

            int maNguoiDung = (int)Session["MaNguoiDung"];

            var ds = db.DanhGias
                .Where(d => d.MaNguoiDung == maNguoiDung)
                .OrderByDescending(d => d.NgayDG)
                .Select(d => new DanhGiaVM
                {
                    MaDG = d.MaDG,
                    MaSanPham = d.MaSanPham,
                    TenSanPham = d.SanPham.TenSanPham,
                    HinhAnh = d.SanPham.HinhAnhChinh,

                    Sao = d.Sao ?? 0,
                    NoiDung = d.NoiDung,
                    NgayDG = d.NgayDG ?? DateTime.Now,

                    PhanHois = d.PhanHoiDanhGias
                        .Select(p => new PhanHoiDanhGiaVM
                        {
                            NoiDung = p.NoiDung,
                            TenNguoiDung = p.NguoiDung.HoTen,
                            NgayPhanHoi = p.NgayPhanHoi ?? DateTime.Now
                        }).ToList()
                })
                .ToList();

            return View(ds);
        }

    }
}