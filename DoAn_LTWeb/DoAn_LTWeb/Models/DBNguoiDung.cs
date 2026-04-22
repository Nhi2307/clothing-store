using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DoAn_LTWeb.Models
{
    public class DBNguoiDung
    {
        QuanLyQuanAoEntities7 db = new QuanLyQuanAoEntities7();

        public DBNguoiDung() { }


        public List<NguoiDung> DSNguoiDung()
        {
            return db.NguoiDungs.ToList();
        }


        public NguoiDung DangNhap(string email, string matKhau)
        {
            return db.NguoiDungs
                     .FirstOrDefault(u => u.Email == email && u.MatKhau == matKhau);
        }


        public bool EmailDaTonTai(string email)
        {
            return db.NguoiDungs.Any(u => u.Email == email);
        }


        public bool EmailHopLe(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }


        public bool DangKy(NguoiDung nd)
        {
            try
            {
                nd.VaiTro = "KhachHang";
                nd.NgayTao = DateTime.Now;

                db.NguoiDungs.Add(nd);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}