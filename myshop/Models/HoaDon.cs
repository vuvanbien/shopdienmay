using System;
using System.Collections.Generic;

namespace myshop.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateTime? Ngay { get; set; }

    public string? Note { get; set; }

    public int? TrangThai { get; set; }

    public int? ThanhToan { get; set; }

    public string? TenKh { get; set; }

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();
}
