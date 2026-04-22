using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class DoanhThuController : Controller
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
        public ActionResult Reports(int? year)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            int selectedYear = year ?? DateTime.Now.Year;

            var donHangHoanThanh = db.DonHangs
                .Where(d =>
                    d.TrangThai == "Hoàn thành" &&
                    d.NgayDat.HasValue &&
                    d.NgayDat.Value.Year == selectedYear
                )
                .ToList();

            ViewBag.TotalRevenueCompleted = donHangHoanThanh.Sum(d => d.TongTien ?? 0m);
            ViewBag.TotalOrdersCompleted = donHangHoanThanh.Count;
            ViewBag.SelectedYear = selectedYear;

            var doanhThuTheoThang = donHangHoanThanh
                .GroupBy(d => d.NgayDat.Value.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien ?? 0m)
                })
                .OrderBy(x => x.Thang)
                .ToList();

            ViewBag.ChartLabels = Enumerable.Range(1, 12)
                .Select(m => "Tháng " + m)
                .ToList();

            ViewBag.ChartData = Enumerable.Range(1, 12)
                .Select(m =>
                    doanhThuTheoThang.FirstOrDefault(x => x.Thang == m)?.DoanhThu ?? 0
                )
                .ToList();

            return View(donHangHoanThanh.OrderByDescending(d => d.NgayDat));
        }
    }
}