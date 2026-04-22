using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class ProductController : Controller
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

        private bool IsNhanVien()
        {
            return Session["VaiTro"]?.ToString() == "NhanVien";
        }

        private bool IsStaffOrAdmin()
        {
            return IsAdmin() || IsNhanVien();
        }


        public ActionResult Index(string keyword)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var sanPhams = db.SanPhams
                .Include("DanhMuc")
                .Include("SanPhamBienThes")
                .AsQueryable();


            if (!string.IsNullOrEmpty(keyword))
            {
                sanPhams = sanPhams.Where(sp =>
                    sp.TenSanPham.Contains(keyword) ||
                    sp.DanhMuc.TenDanhMuc.Contains(keyword)
                );
            }

            var result = sanPhams.ToList();

            ViewBag.Keyword = keyword;

            return View(result);
        }


        [HttpGet]
        public ActionResult Add()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            ViewBag.DanhMucs = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.Sizes = db.Sizes.ToList();
            ViewBag.MauSacs = db.MauSacs.ToList();

            return View(new ThemSanPhamVM());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ThemSanPhamVM model, HttpPostedFileBase HinhAnhChinh)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (model.SoLuongBienThe == null || !model.SoLuongBienThe.Any())
            {
                ModelState.AddModelError("", "Phải nhập số lượng cho ít nhất 1 biến thể.");
            }

            int tongBienThe = model.SoLuongBienThe?.Values.Sum() ?? 0;

            if (model.SanPham.SoLuongTon != tongBienThe)
            {
                ModelState.AddModelError("", "Tổng số lượng tồn không khớp với tổng các biến thể!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucs = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc");
                ViewBag.Sizes = db.Sizes.ToList();
                ViewBag.MauSacs = db.MauSacs.ToList();
                return View(model);
            }

            if (HinhAnhChinh != null && HinhAnhChinh.ContentLength > 0)
            {
                var fileName = Path.GetFileName(HinhAnhChinh.FileName);
                HinhAnhChinh.SaveAs(Server.MapPath("~/Content/HinhAnh/" + fileName));
                model.SanPham.HinhAnhChinh = fileName;
            }

            db.SanPhams.Add(model.SanPham);
            db.SaveChanges();

            foreach (var item in model.SoLuongBienThe.Where(x => x.Value > 0))
            {
                var parts = item.Key.Split('_');

                db.SanPhamBienThes.Add(new SanPhamBienThe
                {
                    MaSanPham = model.SanPham.MaSanPham,
                    MaSize = int.Parse(parts[0]),
                    MaMau = int.Parse(parts[1]),
                    SoLuongTon = item.Value
                });
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!IsStaffOrAdmin())
                return RedirectToAction("DangNhap", "Account");

            var product = db.SanPhams.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.DanhMucs = new SelectList(
                db.DanhMucs, "MaDanhMuc", "TenDanhMuc", product.MaDanhMuc);

            return View(product);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham product, HttpPostedFileBase HinhAnhChinh)
        {
            if (!IsStaffOrAdmin()) return RedirectToAction("DangNhap", "Account");

            if (ModelState.IsValid)
            {
                int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

                if (HinhAnhChinh != null && HinhAnhChinh.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(HinhAnhChinh.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), fileName);
                    HinhAnhChinh.SaveAs(path);
                    product.HinhAnhChinh = fileName;
                }


                var sp = db.SanPhams.Find(product.MaSanPham);
                if (sp == null) return HttpNotFound();

                sp.TenSanPham = product.TenSanPham;
                sp.MaDanhMuc = product.MaDanhMuc;
                sp.Gia = product.Gia;
                sp.SoLuongTon = product.SoLuongTon ?? 0;
                sp.MoTa = product.MoTa;

                if (HinhAnhChinh != null && HinhAnhChinh.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(HinhAnhChinh.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnh/"), fileName);
                    HinhAnhChinh.SaveAs(path);
                    sp.HinhAnhChinh = fileName;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DanhMucs = new SelectList(db.DanhMucs, "MaDanhMuc", "TenDanhMuc", product.MaDanhMuc);
            return View(product);
        }

        public ActionResult Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "Account");

            var sp = db.SanPhams
                       .Include("SanPhamBienThes")
                       .Include("DanhGias")
                       .Include("KhuyenMais")
                       .Include("HinhAnhSanPhams")
                       .FirstOrDefault(x => x.MaSanPham == id);

            if (sp == null)
                return HttpNotFound();


            bool daCoDonHang = db.ChiTietDonHangs
                                 .Any(ct => ct.SanPhamBienThe.MaSanPham == id);

            if (daCoDonHang)
            {
                TempData["Error"] = "Không thể xóa sản phẩm đã có trong đơn hàng!";
                return RedirectToAction("Index");
            }


            db.SanPhamBienThes.RemoveRange(sp.SanPhamBienThes);
            db.DanhGias.RemoveRange(sp.DanhGias);
            db.KhuyenMais.RemoveRange(sp.KhuyenMais);
            db.HinhAnhSanPhams.RemoveRange(sp.HinhAnhSanPhams);


            db.SanPhams.Remove(sp);
            db.SaveChanges();

            TempData["Success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }


        public ActionResult XemChiTiet(int id)
        {

            var sanpham = db.SanPhams.FirstOrDefault(s => s.MaSanPham == id);
            if (sanpham == null) return HttpNotFound();


            var bienThe = db.SanPhamBienThes
                .Where(bt => bt.MaSanPham == id)
                .ToList();

            var cacBienTheSanCo = bienThe.Select(bt => new BienTheDTO
            {
                MaMau = bt.MaMau,
                MaSize = bt.MaSize,
                SoLuongTon = bt.SoLuongTon ?? 0
            }).ToList();

            var mauSacs = bienThe
                .Select(bt => new MauSacVM
                {
                    MaMau = bt.MauSac.MaMau,
                    TenMau = bt.MauSac.TenMau,
                    MaMauHex = bt.MauSac.MaMauHex
                })
                .Distinct()
                .ToList();

            var sizes = bienThe
                .Select(bt => new SizeVM
                {
                    MaSize = bt.Size.MaSize,
                    TenSize = bt.Size.TenSize
                })
                .Distinct()
                .ToList();


            var now = DateTime.Now;

            var cungDanhMuc = db.SanPhams
                .Where(s => s.MaDanhMuc == sanpham.MaDanhMuc && s.MaSanPham != id)
                .Take(4)
                .Select(s => new SanPhamVM
                {
                    MaSanPham = s.MaSanPham,
                    TenSanPham = s.TenSanPham,
                    Gia = s.Gia ?? 0m,
                    GiaKhuyenMai = s.Gia ?? 0m,  
                    PhanTramGiam = 0,
                    HinhAnhChinh = s.HinhAnhChinh,
                    MoTa = s.MoTa
                })
                .ToList();

            foreach (var sp in cungDanhMuc)
            {
                var km = db.KhuyenMais.FirstOrDefault(k =>
                    k.MaSanPham == sp.MaSanPham &&
                    k.NgayBD <= now &&
                    k.NgayKT >= now);

                if (km != null && km.MucGiam > 0)
                {
                    sp.PhanTramGiam = (int)km.MucGiam;
                    sp.GiaKhuyenMai = Math.Round(
                        sp.Gia * (100 - km.MucGiam) / 100, 0
                    );
                }
            }



            decimal giaGoc = sanpham.Gia ?? 0m;
            decimal giaSauGiam = giaGoc;
            decimal phanTramGiam = 0;
            bool dangKhuyenMai = false;

            var khuyenMai = db.KhuyenMais.FirstOrDefault(k =>
                k.MaSanPham == id &&
                k.NgayBD <= now &&
                k.NgayKT >= now);

            if (khuyenMai != null && khuyenMai.MucGiam > 0)
            {
                dangKhuyenMai = true;
                phanTramGiam = khuyenMai.MucGiam;
                giaSauGiam = Math.Round(giaGoc * (100 - phanTramGiam) / 100, 0);
            }


            var vm = new ChiTietSanPhamVM
            {
                SanPham = new SanPhamVM
                {
                    MaSanPham = sanpham.MaSanPham,
                    TenSanPham = sanpham.TenSanPham,
                    Gia = giaGoc,
                    MoTa = sanpham.MoTa,
                    HinhAnhChinh = sanpham.HinhAnhChinh
                },


                GiaGoc = giaGoc,
                GiaSauGiam = giaSauGiam,
                DangKhuyenMai = dangKhuyenMai,
                PhanTramGiam = phanTramGiam,

                MauSacs = mauSacs,
                Sizes = sizes,
                CungDanhMuc = cungDanhMuc,
                CacBienTheSanCo = cacBienTheSanCo
            };


            var danhGias = db.DanhGias
                        .Where(d => d.MaSanPham == id)
                        .OrderByDescending(d => d.NgayDG)
                        .Select(d => new DanhGiaVM
                        {
                            MaDG = d.MaDG,
                            MaSanPham = d.MaSanPham,
                            HoTen = d.NguoiDung.HoTen,
                            Sao = d.Sao ?? 0,
                            NoiDung = d.NoiDung,
                            NgayDG = d.NgayDG ?? DateTime.Now,

                            PhanHois = d.PhanHoiDanhGias
                                .OrderBy(p => p.NgayPhanHoi)
                                .Select(p => new PhanHoiDanhGiaVM
                                {
                                    NoiDung = p.NoiDung,
                                    TenNguoiDung = p.NguoiDung.HoTen,
                                    NgayPhanHoi = p.NgayPhanHoi ?? DateTime.Now
                                })
                                .ToList()
                        })
                        .ToList();

            vm.DanhGias = danhGias;

            vm.TongSoDanhGia = danhGias.Count;
            vm.SaoTrungBinh = danhGias.Any()
                ? Math.Round(danhGias.Average(x => x.Sao), 1)
                : 0;



            return View(vm);
        }

        public ActionResult DanhSachSanPham()
        {
            var now = DateTime.Now;

            var dsSanPham = db.SanPhams
                .Select(sp => new SanPhamVM
                {
                    MaSanPham = sp.MaSanPham,
                    TenSanPham = sp.TenSanPham,
                    Gia = sp.Gia ?? 0m,
                    GiaKhuyenMai = sp.Gia ?? 0m, 
                    PhanTramGiam = 0,
                    MoTa = sp.MoTa,
                    HinhAnhChinh = sp.HinhAnhChinh
                })
                .ToList();

            foreach (var sp in dsSanPham)
            {
                var km = db.KhuyenMais.FirstOrDefault(k =>
                    k.MaSanPham == sp.MaSanPham &&
                    k.NgayBD <= now &&
                    k.NgayKT >= now);

                if (km != null && km.MucGiam > 0)
                {
                    sp.PhanTramGiam = (int)km.MucGiam;
                    sp.GiaKhuyenMai = Math.Round(
                        sp.Gia * (100 - km.MucGiam) / 100, 0
                    );
                }
            }
            return View(dsSanPham);
        }

    }
}