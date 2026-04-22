CREATE DATABASE QuanLyQuanAo
Go
USE QuanLyQuanAo
Go


---- BẢNG NGƯỜI DÙNG   --> (IDENTITY GIÚP MÃ TẠO TỰ ĐỘNG)
CREATE TABLE NguoiDung (
    MaNguoiDung INT IDENTITY PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
	TaiKhoan NVARCHAR(100) NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(20),
    DiaChi NVARCHAR(200),
    VaiTro NVARCHAR(20) CHECK (VaiTro IN ('Admin','NhanVien', 'KhachHang')) DEFAULT 'KhachHang',
    NgayTao DATETIME DEFAULT GETDATE(),
	TrangThai BIT DEFAULT 1
);

--- BẢNG DANH MỤC
CREATE TABLE DanhMuc (
    MaDanhMuc INT IDENTITY PRIMARY KEY,
    TenDanhMuc NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(200)
);

--- BẢNG SẢN PHẨM
CREATE TABLE SanPham (
    MaSanPham INT IDENTITY PRIMARY KEY,
    TenSanPham NVARCHAR(150) NOT NULL,
    MaDanhMuc INT,
    Gia DECIMAL(12,2) CHECK (Gia >= 0),
    SoLuongTon INT DEFAULT 0,
    MoTa NVARCHAR(500),
    HinhAnhChinh NVARCHAR(255),
    NgayThem DATETIME DEFAULT GETDATE(),
	GiaKhuyenMai DECIMAL(12, 2),
	FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc)
);

--- BẢNG SIZE
CREATE TABLE Size (
    MaSize INT IDENTITY(1,1) PRIMARY KEY,
    TenSize NVARCHAR(10) NOT NULL
);

--- BẢNG MÀU SẮC (có mã HEX)
CREATE TABLE MauSac (
    MaMau INT IDENTITY PRIMARY KEY,
    TenMau NVARCHAR(50) NOT NULL,
    MaMauHex CHAR(7) NOT NULL  -- Mã màu HTML, ví dụ: #FFFFFF
);

--- BẢNG BIẾN THỂ SẢN PHẨM (Size + Màu)
CREATE TABLE SanPhamBienThe (
    MaBienThe INT IDENTITY PRIMARY KEY,
    MaSanPham INT NOT NULL,
    MaSize INT NOT NULL,
    MaMau INT NOT NULL,
    SoLuongTon INT DEFAULT 0 CHECK (SoLuongTon >= 0),
    GiaBan DECIMAL(12,2) CHECK (GiaBan >= 0),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
    FOREIGN KEY (MaSize) REFERENCES Size(MaSize),
    FOREIGN KEY (MaMau) REFERENCES MauSac(MaMau)
);

--- BẢNG HÌNH ẢNH SẢN PHẨM
CREATE TABLE HinhAnhSanPham (
    MaHinhAnh INT IDENTITY PRIMARY KEY,
    MaSanPham INT,
    DuongDan NVARCHAR(255) NOT NULL,
	FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

--- BẢNG ÐƠN HÀNG
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY PRIMARY KEY,
    MaNguoiDung INT,
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(12,2),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN (N'Chờ xác nhận', N'Đang giao', N'Hoàn thành', N'Hủy')) DEFAULT N'Chờ xác nhận',
	FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

--- BẢNG CHI TIẾT ĐƠN HÀNG (ĐÃ CẬP NHẬT)
CREATE TABLE ChiTietDonHang (
    MaChiTiet INT IDENTITY PRIMARY KEY,
    MaDonHang INT NOT NULL,
    MaBienThe INT NOT NULL,
    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(12,2),
    ThanhTien AS (SoLuong * DonGia) PERSISTED,
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
    FOREIGN KEY (MaBienThe) REFERENCES SanPhamBienThe(MaBienThe)
);

--- THÊM BẢNG LIÊN HỆ
CREATE TABLE LienHe (
    ID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Message NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- BẢNG PHẢN HỒI LIÊN HỆ
CREATE TABLE PhanHoiLienHe (
    MaPhanHoi INT IDENTITY PRIMARY KEY,          -- Mã phản hồi tự tăng
    MaLienHe INT NOT NULL,                        -- Liên hệ mà admin phản hồi
    MaNguoiDung INT NOT NULL,                     -- Admin phản hồi
    NoiDung NVARCHAR(MAX) NOT NULL,              -- Nội dung phản hồi
    NgayPhanHoi DATETIME DEFAULT GETDATE(),      -- Ngày phản hồi
    FOREIGN KEY (MaLienHe) REFERENCES LienHe(ID) ON DELETE CASCADE,
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);


----- BẢNG ĐÁNH GIÁ
CREATE TABLE DanhGia (
    MaDG INT IDENTITY PRIMARY KEY,              -- Mã đánh giá
    MaNguoiDung INT NOT NULL,                   -- Khách hàng đánh giá
    MaSanPham INT NOT NULL,                     -- Sản phẩm được đánh giá
	MaDonHang INT NOT NULL,
    Sao INT CHECK(Sao BETWEEN 1 AND 5),        -- Số sao 1-5
    NoiDung NVARCHAR(500) NULL,                -- Nội dung đánh giá
    NgayDG DATETIME DEFAULT GETDATE(),         -- Ngày đánh giá
	TrangThai BIT DEFAULT 1, -- 1: Hiển thị | 0: Ẩn
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham),
	FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)
);


----- BẢNG KHUYẾN MÃI
CREATE TABLE KhuyenMai (
    MaKM INT IDENTITY PRIMARY KEY,             -- Mã khuyến mãi
    TenKM NVARCHAR(100) NOT NULL,              -- Tên khuyến mãi
    MucGiam DECIMAL(5,2) NOT NULL,            -- Mức giảm (%), ví dụ: 10 = giảm 10%
    NgayBD DATETIME NOT NULL,                  -- Ngày bắt đầu
    NgayKT DATETIME NOT NULL,                  -- Ngày kết thúc
    MoTa NVARCHAR(500) NULL,                   -- Mô tả chi tiết
    MaSanPham INT NULL,                        -- Áp dụng cho sản phẩm cụ thể (null = tất cả)
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

------ BẢNG PHẢN HỒI ĐÁNH GIÁ
CREATE TABLE PhanHoiDanhGia (
    MaPhanHoi INT IDENTITY PRIMARY KEY,
    MaDG INT NOT NULL,
    MaNguoiDung INT NOT NULL, -- ADMIN
    NoiDung NVARCHAR(MAX) NOT NULL,
    NgayPhanHoi DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaDG) REFERENCES DanhGia(MaDG),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

SELECT * FROM NguoiDung
SELECT * FROM DonHang
SELECT * FROM ChiTietDonHang
SELECT * FROM DanhGia
SELECT * FROM KhuyenMai
SELECT * FROM LienHe

--- DỮ LIỆU NGƯỜI DÙNG
INSERT INTO NguoiDung (HoTen, Email, TaiKhoan, MatKhau, SoDienThoai, DiaChi, VaiTro) VALUES
(N'Nguyễn Văn B', 'VanB@gmail.com', 'admin', 'admin1', '0909997777', N'HCM', 'Admin'),
(N'Trần Văn A', 'VanA@gmail.com', 'khachang', '123456', '0901000001', N'Đà Nẵng', 'KhachHang'),
(N'Nguyễn Văn C', 'VanC@gmail.com', 'nhanvien01', 'nv123456', '0911111111', N'TP Hồ Chí Minh', 'NhanVien'),
(N'Nguyễn Văn D', 'VanD@gmail.com', 'nhanvien02', 'nv123456', '0911111112', N'Vũng Tàu', 'NhanVien');

select * from NguoiDung

--- DỮ LIỆU DANH MỤC
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES
(N'Áo Khoác Nam', N'Các loại áo khoác gió, áo blazer, áo bomber dành cho nam.'),
(N'Áo Khoác Nữ', N'Áo khoác nữ thời trang, phù hợp cho đi làm và dạo phố.'),
(N'Áo Len Nữ', N'Áo len ấm áp, phong cách trẻ trung cho nữ.'),
(N'Áo Polo Nam', N'Áo polo nam cao cấp, lịch sự và dễ phối đồ.'),
(N'Áo Sơ Mi Nam', N'Áo sơ mi nam công sở, thanh lịch và hiện đại.'),
(N'Áo Sơ Mi Nữ', N'Áo sơ mi nữ thời trang, phù hợp nhiều phong cách.'),
(N'Áo Thun Nam', N'Áo thun nam năng động, thoải mái cho hàng ngày.'),
(N'Áo Thun Nữ', N'Áo thun nữ trẻ trung, nhiều mẫu mã đa dạng.'),
(N'Đầm Nữ', N'Các loại đầm dự tiệc, dạo phố và công sở dành cho nữ.'),
(N'Phụ Kiện', N'Thắt lưng, ví da, túi xách, mũ và nhiều phụ kiện khác.'),
(N'Quần Jean Nam', N'Quần jean nam cá tính, chất liệu bền đẹp.'),
(N'Quần Short Nam', N'Quần short nam thoải mái, phù hợp đi chơi và thể thao.'),
(N'Quần Tây Nam', N'Quần tây nam lịch lãm, phù hợp công sở và sự kiện.'),
(N'Váy Nữ', N'Váy thời trang nữ, phong cách đa dạng từ công sở đến dự tiệc.');

SELECT * FROM DanhMuc

--- DỮ LIỆU SẢN PHẨM
INSERT INTO SanPham (TenSanPham, MaDanhMuc, Gia, SoLuongTon, MoTa, HinhAnhChinh) VALUES
-- 1. Áo Khoác Nam
(N'Áo khoác bomber nam đen', 1, 650000, 20, N'Chất liệu kaki cao cấp, giữ ấm tốt.', N'aokhoacnam_1.jpg'),
(N'Áo blazer nam lịch lãm', 1, 780000, 15, N'Phong cách công sở, form slimfit.', N'aokhoacnam_2.jpg'),
(N'Áo khoác gió nam chống nước', 1, 720000, 25, N'Dễ giặt, khô nhanh, phù hợp đi chơi.', N'aokhoacnam_3.jpg'),
(N'Áo khoác da nam cá tính', 1, 950000, 10, N'Chất liệu da PU mềm, sang trọng.', N'aokhoacnam_4.jpg'),
(N'Áo khoác kaki nam cổ đứng', 1, 690000, 18, N'Phong cách trẻ trung, lịch lãm.', N'aokhoacnam_5.jpg'),
(N'Áo khoác có nón dây kéo', 1, 569000, 20, N'Chất liệu kaki cao cấp, giữ ấm tốt.', N'aokhoacnam_6.jpg'),
(N'Áo khoác bomber nam da lộn', 1, 785000, 30, N'Phong cách công sở, form slimfit.', N'aokhoacnam_7.jpg'),
(N'Áo khoác nam dài tay chần bông có túi form loose', 1, 981000, 35, N'Dễ giặt, khô nhanh, phù hợp đi chơi.', N'aokhoacnam_8.jpg'),
(N'Áo khoác không cổ chần bông mỏng form loose', 1, 686000, 50, N'Chất liệu da PU mềm, sang trọng.', N'aokhoacnam_9.jpg'),
(N'Áo khoác denim nam form loose', 1, 512000, 25, N'Phong cách trẻ trung, lịch lãm.', N'aokhoacnam_10.jpg'),

-- 2. Áo Khoác Nữ
(N'Áo khoác dạ nữ dài', 2, 890000, 18, N'Kiểu dáng Hàn Quốc, tôn dáng.', N'aokhoacnu_1.jpg'),
(N'Áo khoác gió nữ có mũ', 2, 590000, 30, N'Chống nắng, chống nước, nhẹ.', N'aokhoacnu_2.jpg'),
(N'Áo khoác len nữ oversize', 2, 620000, 22, N'Phong cách trẻ trung, dễ phối đồ.', N'aokhoacnu_3.jpg'),
(N'Áo khoác lông nữ thời trang', 2, 970000, 12, N'Giữ ấm cực tốt, mềm mịn.', N'aokhoacnu_4.jpg'),
(N'Áo khoác dù nữ dáng ngắn', 2, 650000, 25, N'Phù hợp cho mùa thu, năng động.', N'aokhoacnu_5.jpg'),
(N'Áo Khoác Tay Dài Nữ Cao Cổ Phối Màu Form Loosei', 2, 490000, 18, N'Kiểu dáng Hàn Quốc, tôn dáng.', N'aokhoacnu_6.jpg'),
(N'Áo khoác thể thao nữ chống tia UV bo chun loose', 2, 501000, 30, N'Chống nắng, chống nước, nhẹ.', N'aokhoacnu_7.jpg'),
(N'Áo phao nữ chần bông cổ rời form regular', 2, 686000, 25, N'Phong cách trẻ trung, dễ phối đồ.', N'aokhoacnu_8.jpg'),
(N'Áo khoác nữ cổ vest form Regular cropped', 2, 149000, 12, N'Giữ ấm cực tốt, mềm mịn.', N'aokhoacnu_9.jpg'),
(N'Áo khoác nữ túi đắp', 2, 490000, 45, N'Phù hợp cho mùa thu, năng động.', N'aokhoacnu_10.jpg'),

-- 3. Áo Len Nữ
(N'Áo len cổ lọ nữ', 3, 480000, 25, N'Giữ ấm, phù hợp mùa đông.', N'aolennu_1.jpg'),
(N'Áo len dệt kim tay dài', 3, 420000, 28, N'Chất len mềm, thoải mái.', N'aolennu_2.jpg'),
(N'Áo len nữ form rộng', 3, 460000, 20, N'Hợp thời trang, dễ phối quần jean.', N'aolennu_3.jpg'),
(N'Áo len croptop nữ', 3, 390000, 26, N'Phong cách năng động, trẻ trung.', N'aolennu_4.jpg'),
(N'Áo len cổ tròn nữ pastel', 3, 440000, 24, N'Màu nhẹ nhàng, tinh tế.', N'aolennu_5.jpg'),
(N'Áo dệt kim tay ngắn nữ cổ tròn kẻ sọc. Fitted', 3, 274000, 50, N'Giữ ấm, phù hợp mùa đông.', N'aolennu_6.jpg'),
(N'Áo dệt kim nữ phối lai tay', 3, 422000, 28, N'Chất len mềm, thoải mái.', N'aolennu_7.jpg'),

-- 4. Áo Polo Nam
(N'Áo polo nam trơn cổ bẻ', 4, 350000, 40, N'Vải cotton co giãn 4 chiều.', N'aopolonam_1.jpg'),
(N'Áo polo nam sọc caro', 4, 380000, 30, N'Thiết kế hiện đại, thoáng mát.', N'aopolonam_2.jpg'),
(N'Áo polo nam tay ngắn', 4, 320000, 35, N'Phù hợp đi làm và dạo phố.', N'aopolonam_3.jpg'),
(N'Áo polo nam phối màu', 4, 370000, 25, N'Phong cách trẻ trung, nam tính.', N'aopolonam_4.jpg'),
(N'Áo polo nam thể thao', 4, 400000, 28, N'Thiết kế thoáng khí, co giãn tốt.', N'aopolonam_5.jpg'),
(N'Áo Polo dệt kim tay dài nam', 4, 687000, 40, N'Vải cotton co giãn 4 chiều.', N'aopolonam_6.jpg'),
(N'Áo polo tay ngắn nam phối cổ', 4, 589000, 30, N'Thiết kế hiện đại, thoáng mát.', N'aopolonam_7.jpg'),
(N'Áo polo premium nam bo tay lịch lãm', 4, 349000, 35, N'Phù hợp đi làm và dạo phố.', N'aopolonam_8.jpg'),
(N'Áo polo nam tay ngắn sọc phối patch', 4, 618000, 25, N'Phong cách trẻ trung, nam tính.', N'aopolonam_9.jpg'),
(N'Áo Polo Nam Tay Ngắn Cotton Túi Đắp Phối Thêu', 4, 490000, 28, N'Thiết kế thoáng khí, co giãn tốt.', N'aopolonam_10.jpg'),

-- 5. Áo Sơ Mi Nam
(N'Áo sơ mi nam trắng công sở', 5, 420000, 40, N'Form ôm body, vải mát.', N'aosominam_1.jpg'),
(N'Áo sơ mi nam caro đỏ', 5, 450000, 25, N'Phong cách năng động, lịch sự.', N'aosominam_2.jpg'),
(N'Áo sơ mi nam denim', 5, 480000, 20, N'Vải jean mềm, cổ bẻ.', N'aosominam_3.jpg'),
(N'Áo sơ mi nam tay dài đen', 5, 440000, 30, N'Phù hợp môi trường làm việc.', N'aosominam_4.jpg'),
(N'Áo sơ mi nam họa tiết sọc dọc', 5, 460000, 28, N'Tạo cảm giác cao ráo, hiện đại.', N'aosominam_5.jpg'),
(N'Áo sơ mi tay dài nam vải flannel', 5, 667000, 40, N'Form ôm body, vải mát.', N'aosominam_6.jpg'),
(N'Áo Sơ Mi Nam Smart shirt Tay Dài Kẻ Sọc. Boxy', 5, 540000, 25, N'Phong cách năng động, lịch sự.', N'aosominam_7.jpg'),
(N'Áo Sơ Mi Nam Smart shirt Tay Dài Cổ Button Down', 5, 399000, 20, N'Vải jean mềm, cổ bẻ.', N'aosominam_8.jpg'),
(N'Áo Sơ Mi Nam Smart shirt Tay Ngắn Cổ Gài Nút', 5, 349000, 30, N'Phù hợp môi trường làm việc.', N'aosominam_9.jpg'),
(N'Áo sơ mi nam dài tay cotton cổ trụ form fitted', 5, 469000, 28, N'Tạo cảm giác cao ráo, hiện đại.', N'aosominam_10.jpg'),

-- 6. Áo Sơ Mi Nữ
(N'Áo sơ mi nữ tay phồng', 6, 390000, 25, N'Thiết kế thanh lịch, hiện đại.', N'aosominu_1.jpg'),
(N'Áo sơ mi nữ cổ nơ', 6, 420000, 20, N'Phù hợp công sở và dự tiệc.', N'aosominu_2.jpg'),
(N'Áo sơ mi nữ trắng cổ tròn', 6, 370000, 28, N'Chất liệu mát, dễ giặt.', N'aosominu_3.jpg'),
(N'Áo sơ mi nữ họa tiết hoa', 6, 410000, 24, N'Phong cách nhẹ nhàng, nữ tính.', N'aosominu_4.jpg'),
(N'Áo sơ mi nữ cổ bèo', 6, 430000, 22, N'Thiết kế điệu đà, tôn dáng.', N'aosominu_5.jpg'),
(N'Áo sơ mi tay ngắn nữ vải texture', 6, 249000, 25, N'Thiết kế thanh lịch, hiện đại.', N'aosominu_6.jpg'),
(N'Áo sơ mi tay phồng nữ', 6, 441000, 20, N'Phù hợp công sở và dự tiệc.', N'aosominu_7.jpg'),
(N'Áo Sơ Mi Nữ Crop Tay Dài Cotton Kẻ Sọc Form Rộng', 6, 471000, 28, N'Chất liệu mát, dễ giặt.', N'aosominu_8.jpg'),
(N'Áo Sơ Mi Crop Nữ Tay Dài Kẻ Sọc Caro Dây Cột Eo', 6, 308000, 24, N'Phong cách nhẹ nhàng, nữ tính.', N'aosominu_9.jpg'),
(N'Áo Sơ Mi Nữ Tay Dài Cotton Kẻ Sọc Form Loose', 6, 431000, 22, N'Thiết kế điệu đà, tôn dáng.', N'aosominu_10.jpg'),

-- 7. Áo Thun Nam
(N'Áo thun nam trơn cổ tròn', 7, 250000, 50, N'Vải cotton thoáng mát.', N'aothunnam_1.jpg'),
(N'Áo thun nam in hình', 7, 270000, 40, N'In 3D hiện đại, bền màu.', N'aothunnam_2.jpg'),
(N'Áo thun nam cổ tim', 7, 260000, 35, N'Đơn giản, dễ phối đồ.', N'aothunnam_3.jpg'),
(N'Áo thun nam thể thao', 7, 290000, 30, N'Thoát mồ hôi nhanh, co giãn tốt.', N'aothunnam_4.jpg'),
(N'Áo thun nam basic trơn', 7, 240000, 42, N'Phong cách tối giản, dễ mặc.', N'aothunnam_5.jpg'),
(N'Áo thun tay dài nam sọc ngang', 7, 638000, 50, N'Vải cotton thoáng mát.', N'aothunnam_6.jpg'),
(N'Áo thun nam tay ngắn sọc phối patch', 7, 589000, 40, N'In 3D hiện đại, bền màu.', N'aothunnam_7.jpg'),
(N'Áo thun tay ngắn nam túi đắp hình thêu', 7, 392000, 35, N'Đơn giản, dễ phối đồ.', N'aothunnam_8.jpg'),
(N'Áo thun giá tốt basic trơn tay ngắn', 7, 147000, 30, N'Thoát mồ hôi nhanh, co giãn tốt.', N'aothunnam_9.jpg'),
(N'Áo thun tay ngắn nam nỉ phối vai', 7, 441000, 42, N'Phong cách tối giản, dễ mặc.', N'aothunnam_10.jpg'),

-- 8. Áo Thun Nữ
(N'Áo thun nữ croptop', 8, 260000, 40, N'Phong cách cá tính, năng động.', N'aothunnu_1.jpg'),
(N'Áo thun nữ tay ngắn', 8, 240000, 35, N'Mềm mại, co giãn tốt.', N'aothunnu_2.jpg'),
(N'Áo thun nữ in chữ', 8, 250000, 30, N'Thiết kế trẻ trung, thời trang.', N'aothunnu_3.jpg'),
(N'Áo thun nữ cổ tim', 8, 230000, 28, N'Tối giản, dễ phối quần jean.', N'aothunnu_4.jpg'),
(N'Áo thun nữ oversize pastel', 8, 270000, 33, N'Màu nhẹ, trẻ trung, dễ thương.', N'aothunnu_5.jpg'),
(N'Áo thun tay dài nữ', 8, 441000, 40, N'Phong cách cá tính, năng động.', N'aothunnu_6.jpg'),
(N'Áo polo Slimme phối viền', 8, 471000, 35, N'Mềm mại, co giãn tốt.', N'aothunnu_7.jpg'),
(N'Áo thun tay ngắn nữ nhãn metal', 8, 294000, 30, N'Thiết kế trẻ trung, thời trang.', N'aothunnu_8.jpg'),
(N'Áo polo Slimme diễu tay', 8, 343000, 28, N'Tối giản, dễ phối quần jean.', N'aothunnu_9.jpg'),
(N'Áo thun Slimme Cổ vuông thêu logo', 8, 342000, 33, N'Màu nhẹ, trẻ trung, dễ thương.', N'aothunnu_10.jpg'),

-- 9. Đầm Nữ
(N'Đầm công sở ôm body', 9, 580000, 22, N'Chất vải cao cấp, tôn dáng.', N'dam_1.jpg'),
(N'Đầm maxi đi biển', 9, 620000, 18, N'Họa tiết hoa nhí, nhẹ nhàng.', N'dam_2.jpg'),
(N'Đầm suông tay ngắn', 9, 540000, 20, N'Dễ mặc, phù hợp nhiều hoàn cảnh.', N'dam_3.jpg'),
(N'Đầm dự tiệc ánh kim', 9, 850000, 10, N'Lấp lánh, sang trọng, cuốn hút.', N'dam_4.jpg'),
(N'Đầm xếp ly midi', 9, 590000, 25, N'Phong cách thanh lịch, dịu dàng.', N'dam_5.jpg'),
(N'Đầm thun Slimme raglan xẻ trước', 9, 378000, 22, N'Chất vải cao cấp, tôn dáng.', N'dam_6.jpg'),
(N'Đầm thun Slimme cổ V phối viền', 9, 569000, 18, N'Họa tiết hoa nhí, nhẹ nhàng.', N'dam_7.jpg'),
(N'Đầm dài nữ tay ngắn cotton có túi form a line', 9, 199000, 20, N'Dễ mặc, phù hợp nhiều hoàn cảnh.', N'dam_8.jpg'),
(N'Đầm dài sơ mi nữ tay ngắn kẻ sọc form loose', 9, 638000, 10, N'Lấp lánh, sang trọng, cuốn hút.', N'dam_9.jpg'),
(N'Đầm polo nữ form straight', 9, 189000, 25, N'Phong cách thanh lịch, dịu dàng.', N'dam_10.jpg'),

-- 10. Phụ Kiện (nón và túi)
(N'Nón da nam cao cấp', 10, 380000, 30, N'Chất liệu da mềm, chống thấm nhẹ, tôn vẻ lịch lãm cho phái mạnh.', N'non_1.jpg'),
(N'Nón jean phong cách', 10, 420000, 25, N'Thiết kế cá tính, dễ phối đồ, phù hợp đi chơi và dạo phố.', N'non_2.jpg'),
(N'Nón bucket thời trang', 10, 250000, 35, N'Kiểu dáng unisex, thích hợp cho cả nam và nữ, che nắng hiệu quả.', N'non_3.jpg'),
(N'Nón tay bèo rộng phong cách', 10, 340000, 28, N'Phù hợp cho du lịch biển, tạo phong cách nữ tính và trẻ trung.', N'non_4.jpg'),
(N'Nón lưỡi trai thời trang', 10, 460000, 26, N'Thiết kế năng động, giúp bảo vệ tốt dưới ánh nắng mặt trời.', N'non_5.jpg'),
(N'Túi da nam cao cấp', 10, 38000, 30, N'Chất da thật bền bỉ, thiết kế sang trọng, phù hợp môi trường công sở.', N'tui_1.jpg'),
(N'Túi thắt lưng da trơn', 10, 42000, 25, N'Kiểu dáng nhỏ gọn, tiện lợi khi di chuyển, tôn phong cách nam tính.', N'tui_2.jpg'),
(N'Túi bucket thời trang', 10, 25000, 35, N'Phong cách trẻ trung, có dây rút, thích hợp đi chơi hoặc dạo phố.', N'tui_3.jpg'),
(N'Túi đeo chéo mini', 10, 34000, 28, N'Gọn nhẹ, tiện lợi để điện thoại, ví tiền, phù hợp cho cả nam và nữ.', N'tui_4.jpg'),
(N'Túi balo canvas trẻ trung', 10, 46000, 26, N'Chất vải dày bền, phong cách năng động, phù hợp học sinh và dân văn phòng.', N'tui_5.jpg'),
(N'Túi Tote Vải Canvas Size Vừa In Hình', 10, 49000, 28, N'Gọn nhẹ, tiện lợi để điện thoại, ví tiền, phù hợp cho cả nam và nữ.', N'tui_6.jpg'),
(N'Túi Tote Brewing Dream', 10, 780000, 26, N'Chất vải dày bền, phong cách năng động, phù hợp học sinh và dân văn phòng.', N'tui_7.jpg'),

-- 11. Quần Jean Nam
(N'Quần jean nam ống đứng', 11, 480000, 25, N'Phong cách cổ điển, dễ phối đồ.', N'quanjeannam_1.jpg'),
(N'Quần jean nam rách gối', 11, 510000, 20, N'Trẻ trung, năng động.', N'quanjeannam_2.jpg'),
(N'Quần jean nam skinny', 11, 520000, 22, N'Co giãn tốt, ôm dáng.', N'quanjeannam_3.jpg'),
(N'Quần jean nam xanh nhạt', 11, 490000, 24, N'Mềm mại, thoải mái khi mặc.', N'quanjeannam_4.jpg'),
(N'Quần jean nam dáng suông', 11, 530000, 23, N'Hợp mốt, năng động.', N'quanjeannam_5.jpg'),
(N'Quần denim nam. Slim', 11, 589000, 25, N'Phong cách cổ điển, dễ phối đồ.', N'quanjeannam_6.jpg'),
(N'Quần denim nam. Baggy', 11, 638000, 20, N'Trẻ trung, năng động.', N'quanjeannam_7.jpg'),
(N'Quần denim nam. Regular', 11, 560000, 22, N'Co giãn tốt, ôm dáng.', N'quanjeannam_8.jpg'),
(N'Quần denim nam thêu nhỏ. Straight', 11, 638000, 24, N'Mềm mại, thoải mái khi mặc.', N'quanjeannam_9.jpg'),
(N'Quần denim nam thêu nhỏ. Straight', 11, 650000, 23, N'Hợp mốt, năng động.', N'quanjeannam_10.jpg'),

-- 12. Quần Short Nam
(N'Quần short kaki nam', 12, 370000, 30, N'Chất liệu nhẹ, dễ vận động.', N'quanshortnam_1.jpg'),
(N'Quần short jean nam', 12, 390000, 25, N'Thiết kế trẻ trung, năng động.', N'quanshortnam_2.jpg'),
(N'Quần short thể thao nam', 12, 350000, 28, N'Co giãn, thoáng khí.', N'quanshortnam_3.jpg'),
(N'Quần short túi hộp nam', 12, 410000, 20, N'Tiện dụng, phong cách đường phố.', N'quanshortnam_4.jpg'),
(N'Quần short vải linen nam', 12, 360000, 27, N'Thoáng mát, mềm mại.', N'quanshortnam_5.jpg'),
(N'Quần short khaki nam. Straight', 12, 471000, 30, N'Chất liệu nhẹ, dễ vận động.', N'quanshortnam_6.jpg'),
(N'Quần short nam túi hộp cargo.Straight', 12, 395000, 25, N'Thiết kế trẻ trung, năng động.', N'quanshortnam_7.jpg'),
(N'Quần short nam túi khoá kéo.Relax', 12, 250000, 28, N'Co giãn, thoáng khí.', N'quanshortnam_8.jpg'),
(N'Quần short nam túi khoá kéo sườn.Straight', 12, 415000, 20, N'Tiện dụng, phong cách đường phố.', N'quanshortnam_9.jpg'),
(N'Quần Short Cargo Nam Cotton Spandex Form Loose', 12, 520000, 27, N'Thoáng mát, mềm mại.', N'quanshortnam_10.jpg'),

-- 13. Quần Tây Nam
(N'Quần tây nam đen ôm', 13, 520000, 30, N'Lịch sự, phù hợp công sở.', N'quantaynam_1.jpg'),
(N'Quần tây nam xanh than', 13, 540000, 25, N'Form slimfit, dễ phối áo.', N'quantaynam_2.jpg'),
(N'Quần tây nam vải co giãn', 13, 560000, 22, N'Thoải mái cả ngày dài.', N'quantaynam_3.jpg'),
(N'Quần tây nam kẻ caro', 13, 580000, 18, N'Phong cách châu Âu, sang trọng.', N'quantaynam_4.jpg'),
(N'Quần tây nam xám nhạt', 13, 550000, 24, N'Tinh tế, thanh lịch.', N'quantaynam_5.jpg'),

-- 14. Váy Nữ
(N'Váy xòe tay phồng', 14, 490000, 20, N'Phong cách công chúa nhẹ nhàng.', N'vay_1.jpg'),
(N'Váy chữ A caro', 14, 460000, 25, N'Trẻ trung, dễ phối áo.', N'vay_2.jpg'),
(N'Váy hai dây satin', 14, 520000, 18, N'Mềm mịn, quyến rũ.', N'vay_3.jpg'),
(N'Váy midi công sở', 14, 550000, 22, N'Phù hợp làm việc và dạo phố.', N'vay_4.jpg'),
(N'Váy hoa vintage', 14, 500000, 26, N'Họa tiết cổ điển, dịu dàng.', N'vay_5.jpg'),
(N'Váy Nữ Dài Vải Dù Lưng Thun Xẻ Tà 2 Bên', 14, 569000, 20, N'Phong cách công chúa nhẹ nhàng.', N'vay_6.jpg'),
(N'váy nữ cotton lưng thun hai lớp Aline', 14, 383000, 25, N'Trẻ trung, dễ phối áo.', N'vay_7.jpg'),
(N'Váy Denim Nữ Ngắn Thắt Lưng Form A Line', 14, 349000, 22, N'Phù hợp làm việc và dạo phố.', N'vay_8.jpg'),
(N'Chân Váy Nữ Dài Xếp Ly 2 Lớp Mềm Mịn Form A Line', 14, 667000, 26, N'Họa tiết cổ điển, dịu dàng.', N'vay_10.jpg');

--- DỮ LIỆU SIZE
INSERT INTO Size (TenSize) VALUES 
-- Size quần áo
(N'S'), (N'M'), (N'L'), (N'XL'), (N'XXL'),
-- Size phụ kiện (NÓN/TÚI)
(N'Free Size');

--- DỮ LIỆU MÀU SẮC
INSERT INTO MauSac (TenMau, MaMauHex) VALUES
(N'Xám', N'#808080'),
(N'Nâu', N'#8B4513'),
(N'Hồng', N'#FFC0CB'),
(N'Xanh rêu', N'#556B2F'),
(N'Vàng', N'#FFD700'),
(N'Tím', N'#800080'),
(N'Cam', N'#FFA500'),
(N'Xanh ngọc', N'#48D1CC'),
(N'Be', N'#F5F5DC'),
(N'Đỏ đô', N'#8B0000'),
(N'Xanh biển', N'#0000FF'),
(N'Xanh lá', N'#008000'),
(N'Nâu nhạt', N'#CD853F'),
(N'Đen bóng', N'#000000'),
(N'Trắng kem', N'#FFFDD0'),
(N'Hồng pastel', N'#FFD1DC'),
(N'Xanh tím than', N'#191970'),
(N'Vàng đồng', N'#B8860B'),
(N'Xám bạc', N'#C0C0C0'),
(N'Xanh navy', N'#000080');

--- DỮ LIỆU SẢN PHẨM BIẾN THỂ
INSERT INTO SanPhamBienThe (MaSanPham, MaSize, MaMau, SoLuongTon, GiaBan) VALUES
-- 1. Áo khoác Nam
(1, 1, 10, 10, 650000),
(1, 3, 1, 15, 650000),
(1, 1, 19, 12, 650000),
(1, 2, 15, 8, 650000),
(1, 1, 20, 5, 650000),
(2, 4, 3, 10, 780000),
(2, 2, 9, 15, 780000),
(2, 1, 12, 8, 780000),
(3, 4, 1, 12, 720000),
(3, 1, 5, 10, 720000),
(3, 5, 4, 8, 720000),
(4, 3, 14, 5, 950000),
(4, 1, 2, 3, 950000),
(4, 3, 4, 2, 950000),
(5, 2, 4, 10, 690000),
(5, 2, 7, 12, 690000),
(5, 1, 16, 8, 690000),
(6, 4, 3, 10, 569000),
(6, 5, 6, 12, 569000),
(6, 1, 7, 8, 569000),
(7, 2, 10, 15, 785000),
(7, 3, 15, 10, 785000),
(7, 2, 6, 5, 785000),
(8, 4, 10, 12, 981000),
(8, 1, 20, 15, 981000),
(8, 4, 13, 10, 981000),
(9, 2, 14, 20, 686000),
(9, 4, 2, 15, 686000),
(9, 1, 10, 10, 686000),
(10, 3, 6, 12, 512000),
(10, 5, 8, 15, 512000),
(10, 3, 11, 10, 512000),

-- 2. Áo khoác Nữ
(11, 2, 14, 5, 890000),
(11, 3, 1, 7, 890000),
(11, 2, 4, 4, 890000),
(11, 1, 7, 2, 890000),
(11, 4, 9, 1, 890000),
(12, 5, 11, 10, 590000),
(12, 1, 10, 12, 590000),
(12, 2, 15, 8, 590000),
(13, 3, 6, 10, 620000),
(13, 1, 8, 12, 620000),
(13, 2, 16, 8, 620000),
(14, 1, 1, 5, 970000),
(14, 4, 5, 4, 970000),
(14, 2, 18, 3, 970000),
(15, 4, 20, 10, 650000),
(15, 1, 7, 12, 650000),
(15, 3, 1, 8, 650000),
(16, 5, 3, 8, 490000),
(16, 3, 5, 7, 490000),
(16, 1, 8, 3, 490000),
(17, 2, 1, 12, 501000),
(17, 3, 11, 10, 501000),
(17, 2, 16, 8, 501000),
(18, 1, 19, 10, 686000),
(18, 4, 10, 8, 686000),
(18, 5, 20, 6, 686000),
(19, 1, 12, 5, 149000),
(19, 2, 16, 4, 149000),
(19, 3, 19, 3, 149000),
(20, 1, 14, 20, 490000),
(20, 2, 12, 15, 490000),
(20, 1, 11, 10, 490000),

-- 3. Áo Len Nữ
(21, 1, 1, 5, 480000),
(21, 3, 12, 7, 480000),
(21, 1, 2, 6, 480000),
(21, 2, 4, 4, 480000),
(21, 1, 6, 3, 480000),
(22, 4, 7, 6, 420000),
(22, 2, 9, 8, 420000),
(22, 1, 1, 7, 420000),
(22, 3, 6, 5, 420000),
(23, 1, 7, 7, 460000),
(23, 5, 10, 6, 460000),
(23, 3, 19, 5, 460000),
(24, 1, 16, 8, 390000),
(24, 3, 12, 10, 390000),
(24, 4, 13, 8, 390000),
(25, 2, 17, 6, 440000),
(25, 1, 16, 7, 440000),
(25, 5, 4, 6, 440000),
(25, 1, 1, 5, 440000),
(26, 1, 8, 10, 274000),
(26, 2, 16, 15, 274000),
(26, 3, 6, 12, 274000),
(26, 1, 1, 8, 274000),
(26, 3, 20, 5, 274000),
(27, 1, 1, 10, 422000),
(27, 4, 4, 8, 422000),
(27, 2, 7, 6, 422000),

-- 4. Áo Polo Nam
(28, 1, 12, 10, 350000),
(28, 2, 3, 15, 350000),
(28, 3, 19, 10, 350000),
(28, 4, 7, 5, 350000),
(28, 5, 18, 3, 350000),
(29, 2, 4, 10, 380000),
(29, 3, 20, 8, 380000),
(29, 4, 14, 6, 380000),
(30, 1, 1, 10, 320000),
(30, 2, 11, 12, 320000),
(30, 3, 5, 8, 320000),
(31, 2, 13, 8, 370000),
(31, 3, 10, 6, 370000),
(31, 4, 6, 5, 370000),
(32, 2, 9, 10, 400000),
(32, 3, 2, 8, 400000),
(32, 4, 17, 5, 400000),
(33, 2, 8, 10, 687000),
(33, 3, 15, 12, 687000),
(33, 4, 16, 8, 687000),
(34, 2, 12, 8, 589000),
(34, 3, 3, 6, 589000),
(34, 4, 19, 5, 589000),
(35, 1, 7, 8, 349000),
(35, 2, 18, 10, 349000),
(35, 3, 4, 7, 349000),
(36, 2, 20, 5, 618000),
(36, 3, 14, 4, 618000),
(36, 4, 1, 3, 618000),
(37, 2, 11, 8, 490000),
(37, 3, 5, 6, 490000),
(37, 4, 13, 5, 490000),

-- 5. Áo Sơ Mi Nam
(38, 1, 10, 10, 420000),
(38, 2, 6, 12, 420000),
(38, 3, 9, 8, 420000),
(38, 4, 2, 5, 420000),
(39, 2, 17, 6, 450000),
(39, 3, 8, 5, 450000),
(39, 4, 15, 4, 450000),
(40, 2, 16, 8, 480000),
(40, 3, 12, 6, 480000),
(40, 4, 3, 4, 480000),
(41, 2, 19, 8, 440000),
(41, 3, 7, 6, 440000),
(41, 4, 18, 5, 440000),
(42, 2, 4, 6, 460000),
(42, 3, 20, 5, 460000),
(42, 4, 14, 4, 460000),
(43, 2, 1, 10, 667000),
(43, 3, 11, 8, 667000),
(43, 4, 5, 6, 667000),
(44, 2, 13, 6, 540000),
(44, 3, 10, 5, 540000),
(44, 4, 6, 4, 540000),
(45, 2, 9, 5, 399000),
(45, 3, 2, 4, 399000),
(45, 4, 17, 3, 399000),
(46, 2, 8, 6, 349000),
(46, 3, 15, 5, 349000),
(46, 4, 16, 4, 349000),
(47, 2, 12, 6, 469000),
(47, 3, 3, 5, 469000),
(47, 4, 19, 4, 469000),

-- 6. Áo Sơ Mi Nữ
(48, 1, 7, 5, 390000),
(48, 2, 18, 7, 390000),
(48, 3, 4, 6, 390000),
(49, 2, 20, 6, 420000),
(49, 3, 14, 5, 420000),
(49, 4, 1, 4, 420000),
(50, 1, 11, 8, 370000),
(50, 2, 5, 10, 370000),
(50, 3, 13, 7, 370000),
(51, 2, 10, 6, 410000),
(51, 3, 6, 5, 410000),
(51, 4, 9, 4, 410000),
(52, 2, 2, 5, 430000),
(52, 3, 17, 4, 430000),
(52, 4, 8, 3, 430000),
(53, 2, 15, 10, 249000),
(53, 3, 16, 8, 249000),
(53, 4, 12, 6, 249000),
(54, 2, 3, 6, 441000),
(54, 3, 19, 5, 441000),
(54, 4, 7, 4, 441000),
(55, 2, 18, 8, 471000),
(55, 3, 4, 7, 471000),
(55, 4, 20, 6, 471000),
(56, 2, 14, 6, 308000),
(56, 3, 1, 5, 308000),
(56, 4, 11, 4, 308000),
(57, 2, 5, 6, 431000),
(57, 3, 13, 5, 431000),
(57, 4, 10, 4, 431000),

-- 7. Áo Thun Nam
(58, 1, 6, 10, 250000),
(58, 2, 9, 15, 250000),
(58, 3, 2, 12, 250000),
(58, 4, 17, 8, 250000),
(59, 1, 8, 8, 270000),
(59, 2, 15, 12, 270000),
(59, 3, 16, 10, 270000),
(60, 1, 12, 8, 260000),
(60, 2, 3, 10, 260000),
(60, 3, 19, 7, 260000),
(61, 2, 7, 8, 290000),
(61, 3, 18, 6, 290000),
(61, 4, 4, 5, 290000),
(62, 1, 20, 10, 240000),
(62, 2, 14, 12, 240000),
(62, 3, 1, 10, 240000),
(63, 2, 11, 10, 638000),
(63, 3, 5, 8, 638000),
(63, 4, 13, 6, 638000),
(64, 2, 10, 8, 589000),
(64, 3, 6, 6, 589000),
(64, 4, 9, 5, 589000),
(65, 2, 2, 6, 392000),
(65, 3, 17, 5, 392000),
(65, 4, 8, 4, 392000),
(66, 1, 15, 10, 147000),
(66, 2, 16, 12, 147000),
(66, 3, 12, 8, 147000),
(67, 2, 3, 10, 441000),
(67, 3, 19, 8, 441000),
(67, 4, 7, 6, 441000),

-- 8. Áo Thun Nữ
(68, 1, 18, 8, 260000),
(68, 2, 4, 10, 260000),
(68, 3, 20, 7, 260000),
(69, 1, 14, 7, 240000),
(69, 2, 1, 8, 240000),
(69, 3, 11, 6, 240000),
(70, 1, 5, 6, 250000),
(70, 2, 13, 8, 250000),
(70, 3, 10, 5, 250000),
(71, 1, 6, 6, 230000),
(71, 2, 9, 7, 230000),
(71, 3, 2, 5, 230000),
(72, 2, 17, 8, 270000),
(72, 3, 8, 6, 270000),
(72, 4, 15, 5, 270000),
(73, 2, 16, 10, 441000),
(73, 3, 12, 8, 441000),
(73, 4, 3, 6, 441000),
(74, 2, 19, 8, 471000),
(74, 3, 7, 6, 471000),
(74, 4, 18, 5, 471000),
(75, 2, 4, 6, 294000),
(75, 3, 20, 5, 294000),
(75, 4, 14, 4, 294000),
(76, 2, 1, 6, 343000),
(76, 3, 11, 5, 343000),
(76, 4, 5, 4, 343000),
(77, 2, 13, 8, 342000),
(77, 3, 10, 6, 342000),
(77, 4, 6, 5, 342000),

-- 9. Đầm Nữ
(78, 1, 9, 5, 580000),
(78, 2, 2, 7, 580000),
(78, 3, 17, 6, 580000),
(79, 2, 8, 5, 620000),
(79, 3, 15, 4, 620000),
(79, 4, 16, 3, 620000),
(80, 2, 12, 6, 540000),
(80, 3, 3, 5, 540000),
(80, 4, 19, 4, 540000),
(81, 2, 7, 3, 850000),
(81, 3, 18, 2, 850000),
(81, 4, 4, 1, 850000),
(82, 2, 20, 6, 590000),
(82, 3, 14, 5, 590000),
(82, 4, 1, 4, 590000),
(83, 2, 11, 6, 378000),
(83, 3, 5, 5, 378000),
(83, 4, 13, 4, 378000),
(84, 2, 10, 5, 569000),
(84, 3, 6, 4, 569000),
(84, 4, 9, 3, 569000),
(85, 2, 2, 6, 199000),
(85, 3, 17, 5, 199000),
(85, 4, 8, 4, 199000),
(86, 2, 15, 3, 638000),
(86, 3, 16, 2, 638000),
(86, 4, 12, 1, 638000),
(87, 2, 3, 6, 189000),
(87, 3, 19, 5, 189000),
(87, 4, 7, 4, 189000),

-- 10. Phụ Kiện (nón và túi)
(88, 6, 14, 30, 380000), 
(89, 6, 11, 25, 420000), 
(90, 6, 11, 35, 250000), 
(91, 6, 16, 28, 340000), 
(92, 6, 14, 26, 460000), 
(93, 6, 14, 30, 38000), 
(94, 6, 14, 25, 42000), 
(95, 6, 16, 35, 25000), 
(96, 6, 16, 28, 34000), 
(97, 6, 11, 26, 46000), 
(98, 6, 16, 28, 49000), 
(99, 6, 17, 26, 780000), 

-- 11. Quần Jean Nam
(100, 2, 10, 8, 480000), 
(100, 3, 11, 7, 480000), 
(100, 4, 4, 5, 480000), 
(101, 2, 8, 6, 510000), 
(101, 3, 9, 5, 510000), 
(101, 4, 13, 4, 510000), 
(102, 2, 7, 7, 520000), 
(102, 3, 19, 6, 520000), 
(102, 4, 20, 5, 520000), 
(103, 2, 18, 8, 490000), 
(103, 3, 16, 6, 490000), 
(103, 4, 13, 5, 490000), 
(104, 2, 11, 7, 530000), 
(104, 3, 10, 6, 530000), 
(104, 4, 6, 5, 530000), 
(105, 2, 1, 8, 589000), 
(105, 3, 11, 7, 589000), 
(105, 4, 2, 6, 589000), 
(106, 2, 5, 6, 638000), 
(106, 3, 4, 5, 638000), 
(106, 4, 10, 4, 638000), 
(107, 2, 14, 7, 560000), 
(107, 3, 17, 6, 560000), 
(107, 4, 18, 5, 560000), 
(108, 2, 12, 8, 638000), 
(108, 3, 9, 7, 638000), 
(108, 4, 20, 6, 638000), 
(109, 2, 9, 7, 650000), 
(109, 3, 5, 6, 650000), 
(109, 4, 9, 5, 650000), 

-- 12. Quần Short Nam
(110, 2, 4, 8, 370000),
(110, 3, 15, 7, 370000),
(110, 4, 9, 6, 370000),
(111, 2, 1, 7, 390000),
(111, 3, 18, 6, 390000),
(111, 4, 6, 5, 390000),
(112, 2, 12, 8, 350000),
(112, 3, 7, 7, 350000),
(112, 4, 3, 6, 350000),
(113, 2, 19, 6, 410000),
(113, 3, 2, 5, 410000),
(113, 4, 13, 4, 410000),
(114, 2, 10, 7, 360000),
(114, 3, 5, 6, 360000),
(114, 4, 8, 5, 360000),
(115, 2, 14, 8, 471000),
(115, 3, 20, 7, 471000),
(115, 4, 11, 6, 471000),
(116, 2, 2, 7, 395000),
(116, 3, 17, 6, 395000),
(116, 4, 1, 5, 395000),
(117, 2, 7, 8, 250000),
(117, 3, 3, 7, 250000),
(117, 4, 16, 6, 250000),
(118, 2, 18, 6, 415000),
(118, 3, 12, 5, 415000),
(118, 4, 4, 4, 415000),
(119, 2, 5, 7, 520000),
(119, 3, 8, 6, 520000),
(119, 4, 15, 5, 520000),

-- 13. Quần Tây Nam
(120, 2, 3, 10, 520000),
(120, 3, 14, 10, 520000),
(120, 4, 7, 10, 520000),
(121, 2, 11, 8, 540000),
(121, 3, 5, 8, 540000),
(121, 4, 18, 9, 540000),
(122, 2, 9, 7, 560000),
(122, 3, 16, 8, 560000),
(122, 4, 12, 7, 560000),
(123, 2, 10, 6, 580000),
(123, 3, 2, 6, 580000),
(123, 4, 17, 6, 580000),
(124, 2, 1, 8, 550000),
(124, 3, 19, 8, 550000),
(124, 4, 4, 8, 550000),

-- 14. Váy Nữ
(125, 1, 6, 5, 490000),
(125, 2, 2, 5, 490000),
(125, 3, 18, 5, 490000),
(126, 2, 9, 6, 460000),
(126, 3, 14, 6, 460000),
(126, 4, 20, 6, 460000),
(127, 2, 1, 6, 520000),
(127, 3, 8, 6, 520000),
(127, 4, 13, 6, 520000),
(128, 2, 7, 6, 550000),
(128, 3, 12, 6, 550000),
(128, 4, 3, 6, 550000),
(129, 2, 16, 6, 500000),
(129, 3, 5, 7, 500000),
(129, 4, 15, 7, 500000),
(130, 2, 20, 6, 569000),
(130, 3, 9, 6, 569000),
(130, 4, 11, 6, 569000),
(131, 2, 4, 6, 383000),
(131, 3, 17, 6, 383000),
(131, 4, 7, 6, 383000),
(132, 2, 19, 6, 349000),
(132, 3, 8, 6, 349000),
(132, 4, 2, 6, 349000),
(133, 2, 14, 6, 667000),
(133, 3, 6, 6, 667000),
(133, 4, 10, 6, 667000);

--- DỮ LIỆU HÌNH ẢNH SẢN PHẨM
INSERT INTO HinhAnhSanPham (MaSanPham, DuongDan) VALUES
(1, N'aokhoacnam_1.jpg'),
(2, N'aokhoacnu_1.jpg'),
(3, N'aolennu_1.jpg'),
(4, N'aopolonam_1.jpg'),
(5, N'aosominam_1.jpg'),
(6, N'aosominu_1.jpg'),
(7, N'aothunnam_1.jpg'),
(8, N'aothunnu_1.jpg'),
(9, N'dam_1.jpg'),
(10, N'non_1.jpg'),
(11, N'tui_1.jpg'),
(12, N'quanjeannam_1.jpg'),
(13, N'quanshortnam_1.jpg'),
(14, N'quantaynam_1.jpg'),
(15, N'vay_1.jpg');

--- DỮ LIỆU ÐƠN HÀNG
INSERT INTO DonHang (MaNguoiDung, NgayDat, TongTien, TrangThai) VALUES
(1, '2025-02-05', 2500000, N'Hoàn thành'),
(2, '2025-02-06', 1890000, N'Chờ xác nhận'),
(1, '2025-02-07', 720000, N'Hủy'),
(2, '2025-02-08', 1350000, N'Đang giao');

--- DỮ LIỆU CHI TIẾT ĐƠN HÀNG
INSERT INTO ChiTietDonHang(MaDonHang, MaBienThe, SoLuong, DonGia) VALUES
(1, 1, 2, 650000),
(1, 2, 1, 370000),
(2, 3, 1, 890000),
(2, 4, 1, 620000);


--PHÂN QUYỀN CHO ADMIN VÀ NHÂN VIÊN

--Hàm kiểm tra quyền Admin
CREATE FUNCTION fn_IsAdmin(@MaNguoiDung INT)
RETURNS BIT
AS
BEGIN
    RETURN (
        SELECT CASE 
            WHEN EXISTS (
                SELECT 1 FROM NguoiDung 
                WHERE MaNguoiDung = @MaNguoiDung AND VaiTro = 'Admin'
            ) THEN 1 ELSE 0 END
    );
END;
GO

--Hàm kiểm tra quyền Nhân viên
CREATE FUNCTION fn_IsStaff(@MaNguoiDung INT)
RETURNS BIT
AS
BEGIN
    RETURN (
        SELECT CASE 
            WHEN EXISTS (
                SELECT 1 FROM NguoiDung
                WHERE MaNguoiDung = @MaNguoiDung
                  AND VaiTro IN ('Admin','NhanVien')
            ) THEN 1 ELSE 0 END
    );
END;
GO

--Hàm thêm sản phẩm chỉ dành cho Admin
CREATE PROCEDURE sp_Product_Insert
    @MaNguoiDung INT,
    @TenSanPham NVARCHAR(150),
    @MaDanhMuc INT,
    @Gia DECIMAL(12,2),
    @SoLuongTon INT,
    @MoTa NVARCHAR(500),
    @HinhAnhChinh NVARCHAR(255)
AS
BEGIN
    IF dbo.fn_IsAdmin(@MaNguoiDung) = 0
        THROW 50001, N'Chỉ Admin được thêm sản phẩm', 1;

    INSERT INTO SanPham
    VALUES (@TenSanPham,@MaDanhMuc,@Gia,@SoLuongTon,@MoTa,@HinhAnhChinh,GETDATE(),NULL);
END;
GO

-- Hàm sửa sản phẩm dành cho Admin và Nhân viên
CREATE PROCEDURE sp_Product_Update
    @MaNguoiDung INT,
    @MaSanPham INT,
    @TenSanPham NVARCHAR(150),
    @MaDanhMuc INT,
    @Gia DECIMAL(12,2),
    @SoLuongTon INT,
    @MoTa NVARCHAR(500),
    @HinhAnhChinh NVARCHAR(255)
AS
BEGIN
    IF dbo.fn_IsStaff(@MaNguoiDung) = 0
        THROW 50002, N'Không có quyền sửa sản phẩm', 1;

    IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSanPham = @MaSanPham)
        THROW 50003, N'Sản phẩm không tồn tại', 1;

    UPDATE SanPham
    SET TenSanPham=@TenSanPham,
        MaDanhMuc=@MaDanhMuc,
        Gia=@Gia,
        SoLuongTon=@SoLuongTon,
        MoTa=@MoTa,
        HinhAnhChinh=@HinhAnhChinh
    WHERE MaSanPham=@MaSanPham;
END;
GO

--Hàm xóa sản phẩm chỉ dành cho Admin
CREATE PROCEDURE sp_Product_Delete
    @MaNguoiDung INT,
    @MaSanPham INT
AS
BEGIN
    IF dbo.fn_IsAdmin(@MaNguoiDung) = 0
        THROW 50004, N'Chỉ Admin được xóa sản phẩm', 1;

    DELETE FROM HinhAnhSanPham WHERE MaSanPham=@MaSanPham;
    DELETE FROM SanPhamBienThe WHERE MaSanPham=@MaSanPham;
    DELETE FROM SanPham WHERE MaSanPham=@MaSanPham;
END;
GO

--Xem đơn hàng dành cho Admin và Nhân viên
CREATE PROCEDURE sp_Order_GetAll
    @MaNguoiDung INT
AS
BEGIN
    IF dbo.fn_IsStaff(@MaNguoiDung)=0
        THROW 50005, N'Không có quyền xem đơn hàng',1;

    SELECT * FROM DonHang ORDER BY NgayDat DESC;
END;
GO

--Cập nhật trạng thái dành cho admin và nhân viên, nhân viên không được hủy đơn hàng
CREATE PROCEDURE sp_Order_UpdateStatus
    @MaNguoiDung INT,
    @MaDonHang INT,
    @TrangThai NVARCHAR(50)
AS
BEGIN
    IF dbo.fn_IsStaff(@MaNguoiDung)=0
        THROW 50006, N'Không có quyền',1;

    IF @TrangThai=N'Hủy' AND dbo.fn_IsAdmin(@MaNguoiDung)=0
        THROW 50007, N'Nhân viên không được hủy đơn',1;

    UPDATE DonHang SET TrangThai=@TrangThai WHERE MaDonHang=@MaDonHang;
END;
GO

--Hàm hủy đơn hàng chỉ dành cho admin
CREATE PROCEDURE sp_Order_Cancel
    @MaNguoiDung INT,
    @MaDonHang INT
AS
BEGIN
    IF dbo.fn_IsAdmin(@MaNguoiDung)=0
        THROW 50008, N'Chỉ Admin được hủy đơn',1;

    BEGIN TRAN;
        UPDATE S
        SET SoLuongTon += C.SoLuong
        FROM SanPhamBienThe S
        JOIN ChiTietDonHang C ON S.MaBienThe=C.MaBienThe
        WHERE C.MaDonHang=@MaDonHang;

        UPDATE DonHang SET TrangThai=N'Hủy' WHERE MaDonHang=@MaDonHang;
    COMMIT;
END;
GO

--Liên hệ
CREATE PROCEDURE sp_Contact_GetAll
    @MaNguoiDung INT
AS
BEGIN
    IF dbo.fn_IsStaff(@MaNguoiDung)=0
        THROW 50009, N'Không có quyền xem liên hệ',1;

    SELECT * FROM LienHe ORDER BY CreatedAt DESC;
END;
GO

---trigger ngan nguoi dung thay doi quyen 
CREATE TRIGGER trg_KhongChoSuaVaiTro
ON NguoiDung
FOR UPDATE
AS
BEGIN
    IF UPDATE(VaiTro)
    BEGIN
        DECLARE @OldRole NVARCHAR(50),
                @NewRole NVARCHAR(50);

        SELECT @OldRole = d.VaiTro, @NewRole = i.VaiTro
        FROM deleted d
        JOIN inserted i ON d.MaNguoiDung = i.MaNguoiDung;

        -- Nếu role bị thay đổi -> chặn
        IF (@OldRole <> @NewRole)
        BEGIN
            RAISERROR('Không được phép thay đổi vai trò người dùng!', 16, 1);
            ROLLBACK TRANSACTION;
        END
    END
END

---function kiem tra quyen 
CREATE FUNCTION dbo.fn_KiemTraQuyen
(
    @MaNguoiDung INT,
    @VaiTroCanKiemTra NVARCHAR(50)
)
RETURNS BIT
AS
BEGIN
    DECLARE @VaiTroUser NVARCHAR(50);
    DECLARE @KetQua BIT = 0;

    SELECT @VaiTroUser = VaiTro
    FROM NguoiDung
    WHERE MaNguoiDung = @MaNguoiDung;

    IF (@VaiTroUser IS NOT NULL AND @VaiTroUser = @VaiTroCanKiemTra)
        SET @KetQua = 1;

    RETURN @KetQua;
END

-- Lấy tất cả user là Admin
SELECT *
FROM NguoiDung
WHERE dbo.fn_KiemTraQuyen(MaNguoiDung, 'Admin') = 1;

-- Lấy tất cả user là Admin hoặc KhachHang
SELECT *
FROM NguoiDung
WHERE dbo.fn_KiemTraQuyen(MaNguoiDung, 'Admin,KhachHang') = 1;

