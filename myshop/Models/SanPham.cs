using System;
using System.Collections.Generic;

namespace myshop.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string? TenSp { get; set; }

    public string? Mota { get; set; }

    public decimal? GiaBan { get; set; }

    public decimal? GiaNhap { get; set; }

    public decimal? GiaKm { get; set; }

    public int? SoLuong { get; set; }

    public string? Anh { get; set; }

    public int? MaLoaiSp { get; set; }

    public int? MaNsx { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual LoaiSp? MaLoaiSpNavigation { get; set; }

    public virtual NhaSanXuat? MaNsxNavigation { get; set; }
}
