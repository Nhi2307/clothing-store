## Clothing Store - Website Bán Quần Áo

## Giới thiệu
Website quản lý bán quần áo được xây dựng trong khuôn khổ đồ án môn Lập trình Web - Năm 3.

## Công nghệ sử dụng
- ASP.NET MVC
- SQL Server
- Entity Framework
- HTML / CSS / Bootstrap
- 
## Chức năng chính
- Quản lý sản phẩm (thêm, sửa, xóa)
- Quản lý đơn hàng
- Quản lý khách hàng và in hóa đơn
- Phân quyền người dùng: khách hàng, nhân viên, chủ shop
- Thống kê doanh thu

## Nhóm thực hiện
- Nhóm 3 người
- Môn: Lập trình Web - Đại học Công Thương TP.HCM
- Năm học: 2025 - 2026

## Cài đặt và chạy dự án

**Yêu cầu:** Visual Studio 2019/2022, SQL Server, 
SQL Server Management Studio (SSMS)

1. Clone repo về máy
   git clone https://github.com/Nhi2307/clothing-store.git

2. Tạo database
   - Mở SSMS, tạo database mới tên QuanLyQuanAo
   - Mở file .sql trong repo và chạy toàn bộ để tạo bảng và dữ liệu mẫu

3. Cập nhật connection string
   - Mở Web.config trong project
   - Tìm phần <connectionStrings> và sửa Data Source thành tên SQL Server trên máy bạn
   - Đảm bảo Initial Catalog = QuanLyQuanAo

4. Mở và chạy project
   - Mở file .sln bằng Visual Studio
   - Chuột phải vào Solution → Restore NuGet Packages
   - Nhấn Ctrl + F5 để chạy
