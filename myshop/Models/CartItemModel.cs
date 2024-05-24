namespace myshop.Models
{
    public class CartItemModel
    {
        public int MaSp { get; set; }
        public string? TenSp { get; set; }
        public string? Anh { get; set; }
        public double? GiaBan { get; set; }
        public double? GiaKm { get; set; }
        public int? SoLuong { get; set; }
        public double? thanhTien
        {
            get { return SoLuong * GiaKm; }
        }
        public CartItemModel()
        {

        }
        // Khởi tạo giỏ hàng
        public CartItemModel(SanPham sanpham)
        {
            MaSp = sanpham.MaSp;
            TenSp = sanpham.TenSp;
            Anh = sanpham.Anh;

            // Chuyển đổi giá bán
            double giaBanValue;
            if (sanpham.GiaBan != null && double.TryParse(sanpham.GiaBan.ToString(), out giaBanValue))
            {
                GiaBan = giaBanValue;
            }
            else
            {
                GiaBan = 0.0; // Hoặc giá trị mặc định khác nếu phù hợp
            }

            // Chuyển đổi giá khuyến mãi
            double giaKmValue;
            if (sanpham.GiaKm != null && double.TryParse(sanpham.GiaKm.ToString(), out giaKmValue))
            {
                GiaKm = giaKmValue;
            }
            else
            {
                GiaKm = 0.0; // Hoặc giá trị mặc định khác nếu phù hợp
            }

            SoLuong = 1;
        }
    }
}
