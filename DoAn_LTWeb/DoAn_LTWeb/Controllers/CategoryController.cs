using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class CategoryController : Controller
    {

        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();
        private bool IsAdmin()
        {
            return Session["VaiTro"]?.ToString() == "Admin";
        }

        public ActionResult Index(string keyword)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var categories = db.DanhMucs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                categories = categories.Where(c => c.TenDanhMuc.Contains(keyword));
            }

            ViewBag.Keyword = keyword;
            return View(categories.ToList());
        }


        [HttpGet]
        public ActionResult Add()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(DanhMuc category)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (ModelState.IsValid)
            {
                db.DanhMucs.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            var category = db.DanhMucs.Find(id);
            if (category == null) return HttpNotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DanhMuc category)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            var category = db.DanhMucs.Find(id);
            if (category != null)
            {
                db.DanhMucs.Remove(category);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}