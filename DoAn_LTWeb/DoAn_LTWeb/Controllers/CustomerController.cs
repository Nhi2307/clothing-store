using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class CustomerController : Controller
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();

        public ActionResult Index()
        {
            var customers = db.NguoiDungs
                              .Where(x => x.VaiTro == "KhachHang")
                              .OrderByDescending(x => x.NgayTao)
                              .ToList();

            return View(customers);
        }

        public ActionResult Details(int id)
        {
            var kh = db.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == id);
            if (kh == null)
            {
                return HttpNotFound();
            }

            return View(kh);
        }

        public ActionResult Delete(int id)
        {
            var kh = db.NguoiDungs.FirstOrDefault(x => x.MaNguoiDung == id);
            if (kh == null)
            {
                return HttpNotFound();
            }

            db.NguoiDungs.Remove(kh);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // KHÓA TÀI KHOẢN KHÁCH HÀNG
        public ActionResult Khoa(int id)
        {
            var nd = db.NguoiDungs.Find(id);
            nd.TrangThai = false;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult MoKhoa(int id)
        {
            var nd = db.NguoiDungs.Find(id);
            nd.TrangThai = true;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }
    }
}