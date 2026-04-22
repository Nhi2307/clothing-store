using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class OrderController : Controller
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();
        private bool IsKhachHang()
        {
            return Session["MaNguoiDung"] != null &&
                   Session["VaiTro"]?.ToString() == "KhachHang";
        }
        private bool IsStaffOrAdmin()
        {
            if (Session["MaNguoiDung"] == null)
                return false;

            string role = Session["VaiTro"]?.ToString();
            return role == "Admin" || role == "NhanVien";
        }


        public ActionResult Index(string status)
        {
            var orders = db.DonHangs
                           .Include("NguoiDung")
                           .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.TrangThai == status);
            }

            return View(orders
                        .OrderByDescending(o => o.NgayDat)
                        .ToList());
        }
        public ActionResult Details(int id)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var donhang = db.DonHangs.Find(id);
            if (donhang == null) return HttpNotFound();

            var chitiet = db.ChiTietDonHangs
                            .Where(c => c.MaDonHang == id)
                            .ToList();

            ViewBag.ChiTiet = chitiet;

            return View(donhang);
        }


        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var order = db.DonHangs.Find(id);
            if (order == null) return HttpNotFound();

            order.TrangThai = status;
            db.SaveChanges();

            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Details", new { id });
        }


        public ActionResult Delete(int id)
        {
            var order = db.DonHangs.Find(id);
            if (order == null) return HttpNotFound();

            if (!string.Equals(order.TrangThai?.Trim(),
                               "Hủy",
                               StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Chỉ có thể xóa đơn hàng đã bị hủy!";
                return RedirectToAction("Index");
            }

            var ct = db.ChiTietDonHangs
                       .Where(c => c.MaDonHang == id)
                       .ToList();

            db.ChiTietDonHangs.RemoveRange(ct);
            db.DonHangs.Remove(order);

            db.SaveChanges();

            TempData["Success"] = "Xóa đơn hàng thành công!";
            return RedirectToAction("Index");
        }


        public ActionResult LichSuDonHang()
        {
            if (!IsKhachHang())
                return RedirectToAction("DangNhap", "Account");

            int maNguoiDung = (int)Session["MaNguoiDung"];

            var donHangs = db.DonHangs
                .Where(d => d.MaNguoiDung == maNguoiDung)
                .OrderByDescending(d => d.NgayDat)
                .Select(d => new DonHangVM
                {
                    MaDonHang = d.MaDonHang,
                    NgayDat = (DateTime)d.NgayDat,
                    TongTien = (decimal)d.TongTien,
                    TrangThai = d.TrangThai,

                    ChiTiet = d.ChiTietDonHangs.Select(ct => new ChiTietDonHangVM
                    {
                        MaBienThe = ct.MaBienThe,
                        MaSanPham = ct.SanPhamBienThe.MaSanPham,

                        TenSanPham = ct.SanPhamBienThe.SanPham.TenSanPham,
                        HinhAnh = ct.SanPhamBienThe.SanPham.HinhAnhChinh,

                        SoLuong = ct.SoLuong ?? 0,
                        DonGia = ct.DonGia ?? 0,
                        ThanhTien = ct.ThanhTien ?? 0,

                        DaDanhGia = db.DanhGias.Any(dg =>
                            dg.MaNguoiDung == maNguoiDung &&
                            dg.MaSanPham == ct.SanPhamBienThe.MaSanPham &&
                            dg.MaDonHang == d.MaDonHang
                        )

                    }).ToList()
                }).ToList();

            return View(donHangs);
        }

        public ActionResult ChiTietDonHang(int id)
        {
            var don = db.DonHangs.Find(id);
            if (don == null) return HttpNotFound();

            var ct = db.ChiTietDonHangs.Where(x => x.MaDonHang == id).ToList();
            ViewBag.CTDH = ct;

            return View(don);
        }

        public ActionResult InHoaDon(int id)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var don = db.DonHangs.Find(id);
            if (don == null)
                return HttpNotFound();

            var ct = db.ChiTietDonHangs
                       .Where(x => x.MaDonHang == id)
                       .ToList();

            ViewBag.CTDH = ct;
            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyDon(int id)
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap", "Account");

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var donHang = db.DonHangs.Find(id);
                    if (donHang == null)
                        throw new Exception("Không tìm thấy đơn hàng");

                    if (!string.Equals(donHang.TrangThai?.Trim(),
                                       "Chờ xác nhận",
                                       StringComparison.OrdinalIgnoreCase))
                    {
                        tran.Rollback();
                        TempData["Error"] = "Không thể hủy đơn ở trạng thái hiện tại!";
                        return RedirectToAction("LichSuDonHang");
                    }

                    var chiTiet = db.ChiTietDonHangs
                                    .Where(x => x.MaDonHang == id)
                                    .ToList();

                    foreach (var ct in chiTiet)
                    {
                        var bienThe = db.SanPhamBienThes
                                        .FirstOrDefault(x => x.MaBienThe == ct.MaBienThe);

                        bienThe.SoLuongTon += ct.SoLuong ?? 0;

                        var sanPham = db.SanPhams.Find(bienThe.MaSanPham);
                        if (sanPham != null)
                        {
                            sanPham.SoLuongTon = db.SanPhamBienThes
                                .Where(bt => bt.MaSanPham == sanPham.MaSanPham)
                                .Sum(bt => bt.SoLuongTon);
                        }

                    }

                    donHang.TrangThai = "Hủy";

                    db.SaveChanges();
                    tran.Commit();

                    TempData["Success"] = "Hủy đơn hàng thành công!";
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    TempData["Error"] = "Lỗi hủy đơn: " + ex.Message;
                }
            }

            return RedirectToAction("LichSuDonHang");
        }

    }
}