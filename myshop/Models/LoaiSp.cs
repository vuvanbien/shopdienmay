using System;
using System.Collections.Generic;

namespace myshop.Models;

public partial class LoaiSp
{
    public int MaLoaiSp { get; set; }

    public string? TenLoaiSp { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
