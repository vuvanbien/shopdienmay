using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using myshop.Models.ViewModels;
using myshop.Models;
using X.PagedList;

namespace myshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly MyshopContext _context;

        public SanPhamController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Admin/SanPham
        public async Task<IActionResult> Index(string searchTen, string lsp, string nsx, decimal minban, decimal maxban, decimal minnhap, decimal maxnhap, int page = 1, int pageSize = 5)
        {
            ViewBag.ListLoaiSp = await _context.LoaiSps.ToListAsync();
            ViewBag.ListNcc = await _context.NhaSanXuats.ToListAsync();

            // Get data
            var query = from sp in _context.SanPhams
                        join nsxs in _context.NhaSanXuats on sp.MaNsx equals nsxs.MaNsx
                        join ls in _context.LoaiSps on sp.MaLoaiSp equals ls.MaLoaiSp
                        select new ProductViewModel()
                        {
                            MaSp = sp.MaSp,
                            TenSp = sp.TenSp,
                            Mota = sp.Mota,
                            GiaBan = sp.GiaBan,
                            GiaNhap = sp.GiaNhap,
                            GiaKm = sp.GiaKm,
                            Anh = sp.Anh,
                            SoLuong = sp.SoLuong,
                            MaLoaiSp = ls.MaLoaiSp,
                            TenLoaiSp = ls.TenLoaiSp,
                            MaNsx = nsxs.MaNsx,
                            TenNsx = nsxs.TenNsx
                        };

            // Search
            if (!string.IsNullOrEmpty(searchTen))
                query = query.Where(s => s.TenSp != null && s.TenSp.Contains(searchTen));

            if (!string.IsNullOrEmpty(lsp))
                query = query.Where(s => s.TenLoaiSp == lsp);

            if (!string.IsNullOrEmpty(nsx))
                query = query.Where(s => s.TenNsx == nsx);



            if (maxban != 0)
            {
                query = query.Where(item => item.GiaBan < maxban && item.GiaBan > minban);
            }

            if (maxnhap != 0)
            {
                query = query.Where(item => item.GiaNhap < maxnhap && item.GiaNhap > minnhap);
            }

            var totalItemCount = query.Count();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pagedList = new StaticPagedList<ProductViewModel>(model, page, pageSize, totalItemCount);
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.TotalItemCount = totalItemCount;
            ViewBag.Page = page;
            ViewBag.searchTen = searchTen;
            ViewBag.lsp = lsp;
            ViewBag.nsx = nsx;
            ViewBag.minban = minban;
            ViewBag.maxban = maxban;
            ViewBag.minnhap = minnhap;
            ViewBag.maxnhap = maxnhap;
            return View(pagedList);

        }

        // GET: Admin/SanPham/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiSpNavigation)
                .Include(s => s.MaNsxNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/SanPham/Create
        public IActionResult Create()
        {
            ViewBag.ListLoaiSp = _context.LoaiSps.ToList();
            ViewBag.ListNcc = _context.NhaSanXuats.ToList();
            return View();
        }

        // POST: Admin/SanPham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sp, IFormFile image1, string LoaiSp, string Ncc)
        {
            var spmoi = new SanPham();
            spmoi.TenSp = sp.TenSp;
            spmoi.Mota = sp.Mota;
            spmoi.GiaKm = sp.GiaKm;
            spmoi.GiaNhap = sp.GiaNhap;
            spmoi.GiaBan = sp.GiaBan;
            spmoi.SoLuong = sp.SoLuong;


            if (image1 != null && image1.Length > 0)
            {
                string fileName = Path.GetFileName(image1.FileName);
                string uploadPath = Path.Combine("wwwroot", "Images", fileName);

                using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                {
                    image1.CopyTo(fileStream);
                }

                spmoi.Anh = fileName;
            }

            var dm = _context.LoaiSps.FirstOrDefault(s => s.TenLoaiSp == LoaiSp);
            if (dm != null)
            {
                spmoi.MaLoaiSp = dm.MaLoaiSp;
            }

            var hang = _context.NhaSanXuats.FirstOrDefault(s => s.TenNsx == Ncc);
            if (hang != null)
            {
                spmoi.MaNsx = hang.MaNsx;
            }
            _context.SanPhams.Add(spmoi);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Admin/SanPham/Edit/5
        public IActionResult Edit(int? id)
        {
            ViewBag.ListLoaiSp = _context.LoaiSps.ToList();
            ViewBag.ListNcc = _context.NhaSanXuats.ToList();
            SanPham? sanpham = _context.SanPhams.SingleOrDefault(n => n.MaSp.Equals(id));
            if (sanpham == null)
            {
                return NotFound();
            }
            return View(sanpham);
        }

        // POST: Admin/SanPham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Edit(SanPham sp, IFormFile image1)
        {
            SanPham? sanpham = _context.SanPhams.SingleOrDefault(n => n.MaSp == sp.MaSp);
            if (sanpham == null)
            {
                return NotFound();
            }
            sanpham.TenSp = sp.TenSp;
            sanpham.Mota = sp.Mota;
            sanpham.GiaBan = sp.GiaBan;
            sanpham.GiaNhap = sp.GiaNhap;
            sanpham.GiaKm = sp.GiaKm;
            sanpham.SoLuong = sp.SoLuong;
            sanpham.Anh = sp.Anh;

            if (image1 != null)
            {
                if (image1.Length > 0)
                {
                    var filename = Path.GetFileName(image1.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        image1.CopyTo(stream);
                    }
                    sanpham.Anh = image1.FileName;
                }
            }

            sanpham.MaLoaiSp = sp.MaLoaiSp;
            sanpham.MaNsx = sp.MaNsx;

            _context.SaveChanges();
            TempData["Edited"] = "Sửa thông tin sản phẩm thành công";
            return RedirectToAction("Index", "SanPham");
        }

        // GET: Admin/SanPham/Delete/5
        public ActionResult Delete(int MaSp)
        {

            SanPham sanPham = _context.SanPhams.Single(ma => ma.MaSp == MaSp);
            if (sanPham == null)
            {
                return NotFound();
            }

            _context.SanPhams.Remove(sanPham);
            _context.SaveChanges();
            return RedirectToAction("Index", "SanPham");
        }

        private bool SanPhamExists(int id)
        {
          return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }
    }
}
