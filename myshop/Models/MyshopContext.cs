using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace myshop.Models;

public partial class MyshopContext : DbContext
{
    public MyshopContext()
    {
    }

    public MyshopContext(DbContextOptions<MyshopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietHd> ChiTietHds { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<LoaiSp> LoaiSps { get; set; }

    public virtual DbSet<LoaiTk> LoaiTks { get; set; }

    public virtual DbSet<NhaSanXuat> NhaSanXuats { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TinTuc> TinTucs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-HO14S58R;Initial Catalog=myshop;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHd>(entity =>
        {
            entity.HasKey(e => e.MaCt);

            entity.ToTable("ChiTietHD");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHd)
                .HasConstraintName("FK_ChiTietHD_HoaDon");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK_ChiTietHD_SanPham");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd);

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.Ngay).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Sdt)
                .HasMaxLength(50)
                .HasColumnName("SDT");
            entity.Property(e => e.TenKh)
                .HasMaxLength(250)
                .HasColumnName("TenKH");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<LoaiSp>(entity =>
        {
            entity.HasKey(e => e.MaLoaiSp);

            entity.ToTable("LoaiSP");

            entity.Property(e => e.MaLoaiSp).HasColumnName("MaLoaiSP");
            entity.Property(e => e.TenLoaiSp)
                .HasMaxLength(250)
                .HasColumnName("TenLoaiSP");
        });

        modelBuilder.Entity<LoaiTk>(entity =>
        {
            entity.HasKey(e => e.MaLoaiTk);

            entity.ToTable("LoaiTK");

            entity.Property(e => e.MaLoaiTk).HasColumnName("MaLoaiTK");
            entity.Property(e => e.TenLoaiTk)
                .HasMaxLength(250)
                .HasColumnName("TenLoaiTK");
        });

        modelBuilder.Entity<NhaSanXuat>(entity =>
        {
            entity.HasKey(e => e.MaNsx);

            entity.ToTable("NhaSanXuat");

            entity.Property(e => e.MaNsx).HasColumnName("MaNSX");
            entity.Property(e => e.TenNsx)
                .HasMaxLength(250)
                .HasColumnName("TenNSX");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp);

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaKm)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("GiaKM");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaLoaiSp).HasColumnName("MaLoaiSP");
            entity.Property(e => e.MaNsx).HasColumnName("MaNSX");
            entity.Property(e => e.TenSp)
                .HasMaxLength(250)
                .HasColumnName("TenSP");

            entity.HasOne(d => d.MaLoaiSpNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoaiSp)
                .HasConstraintName("FK_SanPham_LoaiSP");

            entity.HasOne(d => d.MaNsxNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNsx)
                .HasConstraintName("FK_SanPham_NhaSanXuat");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk);

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.MaTk).HasColumnName("MaTK");
            entity.Property(e => e.Diachi).HasMaxLength(500);
            entity.Property(e => e.MaLoaiTk).HasColumnName("MaLoaiTK");
            entity.Property(e => e.MatKhau).HasMaxLength(250);
            entity.Property(e => e.Sdt).HasMaxLength(250);
            entity.Property(e => e.TenDn)
                .HasMaxLength(250)
                .HasColumnName("TenDN");
            entity.Property(e => e.TenKh).HasMaxLength(250);

            entity.HasOne(d => d.MaLoaiTkNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaLoaiTk)
                .HasConstraintName("FK_TaiKhoan_LoaiTK1");
        });

        modelBuilder.Entity<TinTuc>(entity =>
        {
            entity.HasKey(e => e.MaTt);

            entity.ToTable("TinTuc");

            entity.Property(e => e.MaTt).HasColumnName("MaTT");
            entity.Property(e => e.Anh).HasMaxLength(500);
            entity.Property(e => e.TenTt).HasColumnName("TenTT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
