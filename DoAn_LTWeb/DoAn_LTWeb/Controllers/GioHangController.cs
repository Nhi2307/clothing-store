using DoAn_LTWeb.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_LTWeb.Controllers
{
    public class GioHangController : Controller
    {

        Models.QuanLyQuanAoEntities7 db = new Models.QuanLyQuanAoEntities7();
        public ActionResult IndexGH()
        {
            var cart = Session["GioHang"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemGioHang(int MaSanPham, int MaSize, int MaMau, int SoLuong)
        {
            if (Session["MaNguoiDung"] == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng.",
                    redirectUrl = Url.Action("DangNhap", "Account")
                });
            }

            var bienThe = db.SanPhamBienThes
                 .FirstOrDefault(bt => bt.MaSanPham == MaSanPham
                                    && bt.MaSize == MaSize
                                    && bt.MaMau == MaMau);

            if (bienThe == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });


            decimal giaGoc = bienThe.SanPham.Gia ?? 0m;
            decimal giaSauGiam = giaGoc;

            var now = DateTime.Now;
            var khuyenMai = db.KhuyenMais.FirstOrDefault(k =>
                k.MaSanPham == MaSanPham &&
                k.NgayBD <= now &&
                k.NgayKT >= now);

            if (khuyenMai != null)
            {
                giaSauGiam = giaGoc * (1 - khuyenMai.MucGiam / 100);
            }

            var cart = Session["GioHang"] as List<CartItem> ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.MaBienThe == bienThe.MaBienThe);
            if (item != null)
            {
                int tongSoLuong = item.SoLuong + SoLuong;

                if (tongSoLuong > (bienThe.SoLuongTon ?? 0))
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Chỉ còn {bienThe.SoLuongTon} sản phẩm trong kho!"
                    });
                }

                item.SoLuong = tongSoLuong;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaBienThe = bienThe.MaBienThe,
                    TenSanPham = bienThe.SanPham.TenSanPham,
                    HinhAnh = "~/Content/HinhAnh/" + (bienThe.SanPham.HinhAnhChinh ?? ""),
                    TenSize = bienThe.Size.TenSize,
                    TenMau = bienThe.MauSac.TenMau,
                    Gia = giaSauGiam,      
                    DonGia = giaSauGiam, 
                    SoLuong = SoLuong
                });
            }

            Session["GioHang"] = cart;

            return Json(new { success = true, cartCount = cart.Sum(c => c.SoLuong) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaGioHang(int MaBienThe)
        {
            var cart = Session["GioHang"] as List<CartItem>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.MaBienThe == MaBienThe);
                if (item != null)
                    cart.Remove(item);

                Session["GioHang"] = cart;
                return Json(new { success = true, cartCount = cart.Sum(c => c.SoLuong) });
            }
            return Json(new { success = false, message = "Không tìm thấy giỏ hàng" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatGioHang(int MaBienThe, int SoLuong)
        {
            var cart = Session["GioHang"] as List<CartItem>;
            if (cart == null)
                return Json(new { success = false, message = "Giỏ hàng không tồn tại!" });

            var item = cart.FirstOrDefault(c => c.MaBienThe == MaBienThe);
            if (item == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ!" });

            var tonKho = db.SanPhamBienThes
                           .Where(x => x.MaBienThe == MaBienThe)
                           .Select(x => x.SoLuongTon)
                           .FirstOrDefault() ?? 0;

            if (tonKho <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Sản phẩm đã hết hàng!"
                });
            }

            if (SoLuong > tonKho)
            {
                return Json(new
                {
                    success = false,
                    message = $"Số lượng vượt quá tồn kho (chỉ còn {tonKho} sản phẩm)"
                });
            }

            if (SoLuong <= 0)
                SoLuong = 1;

            item.SoLuong = SoLuong;
            Session["GioHang"] = cart;

            return Json(new
            {
                success = true,
                cartCount = cart.Sum(c => c.SoLuong)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ThemGioHangAjax(int MaSanPham, int MaSize, int MaMau, int SoLuong)
        {
            if (Session["MaNguoiDung"] == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng.",
                    redirectUrl = Url.Action("DangNhap", "Account")
                });
            }

            var bienThe = db.SanPhamBienThes
                .FirstOrDefault(bt => bt.MaSanPham == MaSanPham
                                   && bt.MaSize == MaSize
                                   && bt.MaMau == MaMau);

            if (bienThe == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }


            decimal giaGoc = bienThe.SanPham.Gia ?? 0m;
            decimal giaSauGiam = giaGoc;

            var now = DateTime.Now;
            var khuyenMai = db.KhuyenMais.FirstOrDefault(k =>
                k.MaSanPham == MaSanPham &&
                k.NgayBD <= now &&
                k.NgayKT >= now);

            if (khuyenMai != null)
            {
                giaSauGiam = giaGoc * (1 - khuyenMai.MucGiam / 100);
            }


            var cart = Session["GioHang"] as List<CartItem> ?? new List<CartItem>();

            var item = cart.FirstOrDefault(c => c.MaBienThe == bienThe.MaBienThe);
            if (item != null)
            {
                int tongSoLuong = item.SoLuong + SoLuong;

                if (tongSoLuong > (bienThe.SoLuongTon ?? 0))
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Chỉ còn {bienThe.SoLuongTon} sản phẩm trong kho!"
                    });
                }

                item.SoLuong = tongSoLuong;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaBienThe = bienThe.MaBienThe,
                    TenSanPham = bienThe.SanPham.TenSanPham,
                    HinhAnh = "~/Content/HinhAnh/" + (bienThe.SanPham.HinhAnhChinh ?? ""),
                    TenSize = bienThe.Size.TenSize,
                    TenMau = bienThe.MauSac.TenMau,
                    Gia = giaSauGiam,      
                    DonGia = giaSauGiam,  
                    SoLuong = SoLuong
                });
            }

            Session["GioHang"] = cart;

            return Json(new
            {
                success = true,
                message = $"Đã thêm {SoLuong} sản phẩm vào giỏ hàng!",
                cartCount = cart.Sum(c => c.SoLuong)
            });
        }

        public ActionResult ThanhToan(int[] selectedItems)
        {
            var cart = Session["GioHang"] as List<CartItem>;
            if (cart == null || !cart.Any())
                return RedirectToAction("IndexGH");

            if (selectedItems == null || selectedItems.Length == 0)
                return RedirectToAction("IndexGH");

            var selectedCart = cart
                .Where(x => selectedItems.Contains(x.MaBienThe))
                .ToList();

            return View(selectedCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(string HoTen, string DiaChi, string SDT, int? MaPTTT, int[] selectedItems)
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap", "Account");

            if (MaPTTT == null)
            {
                TempData["Error"] = "Vui lòng chọn phương thức thanh toán!";
                return RedirectToAction("ThanhToan");
            }

            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán!";
                return RedirectToAction("IndexGH");
            }

            var fullCart = Session["GioHang"] as List<CartItem>;
            var cart = fullCart
                .Where(x => selectedItems.Contains(x.MaBienThe))
                .ToList();

            if (cart == null || !cart.Any())
                return RedirectToAction("IndexGH");

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);


            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in cart)
                    {
                        var bienThe = db.SanPhamBienThes.FirstOrDefault(x => x.MaBienThe == item.MaBienThe);
                        if (bienThe == null)
                        {
                            TempData["Error"] = "Sản phẩm không tồn tại!";
                            return RedirectToAction("IndexGH");
                        }

                        int tonKho = bienThe.SoLuongTon ?? 0;
                        if (item.SoLuong > tonKho)
                        {
                            TempData["Error"] = $"Sản phẩm {item.TenSanPham} chỉ còn {tonKho} cái trong kho!";
                            return RedirectToAction("IndexGH");
                        }
                    }

                    var donHang = new DonHang
                    {
                        MaNguoiDung = maNguoiDung,
                        NgayDat = DateTime.Now,
                        TongTien = cart.Sum(x => x.ThanhTien),
                        TrangThai = "Chờ xác nhận",
                    };

                    db.DonHangs.Add(donHang);
                    db.SaveChanges();

                    foreach (var item in cart)
                    {
                        db.ChiTietDonHangs.Add(new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaBienThe = item.MaBienThe,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia
                        });

                        var bienThe = db.SanPhamBienThes
                                        .First(x => x.MaBienThe == item.MaBienThe);

                        bienThe.SoLuongTon -= item.SoLuong;

                        var sanPham = db.SanPhams.Find(bienThe.MaSanPham);
                        if (sanPham != null)
                        {
                            sanPham.SoLuongTon = db.SanPhamBienThes
                                .Where(bt => bt.MaSanPham == sanPham.MaSanPham)
                                .Sum(bt => bt.SoLuongTon);
                        }
                    }


                    db.SaveChanges();
                    transaction.Commit();

                    Session["GioHang"] = fullCart
                                        .Where(x => !selectedItems.Contains(x.MaBienThe))
                                        .ToList();

                    TempData["Success"] = "Đặt hàng thành công!";

                    return RedirectToAction("ChiTietDonHang", "Order", new { id = donHang.MaDonHang });

                }
                catch
                {
                    transaction.Rollback();
                    TempData["Error"] = "Lỗi thanh toán!";
                    return RedirectToAction("IndexGH");
                }
            }
        }  
    }
}