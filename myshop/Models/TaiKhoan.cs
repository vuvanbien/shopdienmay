using System;
using System.Collections.Generic;

namespace myshop.Models;

public partial class TaiKhoan
{
    public int MaTk { get; set; }

    public string? TenDn { get; set; }

    public string? MatKhau { get; set; }

    public int? MaLoaiTk { get; set; }

    public string? TenKh { get; set; }

    public string? Sdt { get; set; }

    public string? Diachi { get; set; }

    public virtual LoaiTk? MaLoaiTkNavigation { get; set; }
}
