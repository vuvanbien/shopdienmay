using System;
using System.Collections.Generic;

namespace myshop.Models;

public partial class LoaiTk
{
    public int MaLoaiTk { get; set; }

    public string? TenLoaiTk { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
