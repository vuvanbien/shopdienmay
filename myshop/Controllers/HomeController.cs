using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using myshop.Models;
using myshop.Models.ViewModels;
using myshop.Repository;
using PayPal.v1.Orders;
using System.Diagnostics;
using X.PagedList;

namespace myshop.Controllers
{
    public class HomeController : Controller
    {

        private readonly MyshopContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyshopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = 9)
        {
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			int? tongSoLuong = cartItems.Sum(item => item.SoLuong);
			ViewBag.TongSoLuong = tongSoLuong;
			var query = _context.SanPhams.OrderByDescending(sp => sp.MaSp);//sắp xếp giảm dần
            var model = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalItemCount = query.Count();
            var pagedList = new StaticPagedList<SanPham>(model, page, pageSize, totalItemCount);
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.Page = page;
            ViewBag.TotalItemCount = totalItemCount;
            /*ViewBag.lsp = lsp;*/
            return View(pagedList);
        }
        public IActionResult Details(int MaSP)
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            int? tongSoLuong = cartItems.Sum(item => item.SoLuong);
            ViewBag.TongSoLuong = tongSoLuong;
            if (MaSP == null) return RedirectToAction("Index");
            var productById = _context.SanPhams.Where(p => p.MaSp == MaSP).FirstOrDefault();
            return View(productById);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
		public async Task<IActionResult> Search(string searchTen, List<string> lsp, List<string> ncc,  decimal maxban, decimal minban, int page = 1, int pageSize = 6)
		{
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			int? tongSoLuong = cartItems.Sum(item => item.SoLuong);
			ViewBag.TongSoLuong = tongSoLuong;
			ViewBag.ListLoaiSp = await _context.LoaiSps.ToListAsync();
			ViewBag.ListNcc = await _context.NhaSanXuats.ToListAsync();

			var query = from sp in _context.SanPhams
						join nc in _context.NhaSanXuats on sp.MaNsx equals nc.MaNsx
						join ls in _context.LoaiSps on sp.MaLoaiSp equals ls.MaLoaiSp
						select new ProductViewModel()
						{
							MaSp = sp.MaSp,
							TenSp = sp.TenSp,
							Mota = sp.Mota,
							GiaKm = sp.GiaKm,
							GiaBan = sp.GiaBan,
							Anh = sp.Anh,
							SoLuong = sp.SoLuong,
							MaLoaiSp = ls.MaLoaiSp,
							LoaiSanPham = ls.TenLoaiSp,
							NhaCungCap = nc.TenNsx,
							MaNsx = nc.MaNsx,
							
						};

			if (!string.IsNullOrEmpty(searchTen))
			{
				query = query.Where(s => s.TenSp.Contains(searchTen));
			}

			if (lsp != null && lsp.Any())
			{
				query = query.Where(s => lsp.Contains(s.LoaiSanPham));
			}

			if (ncc != null && ncc.Any())
			{
				query = query.Where(s => ncc.Contains(s.NhaCungCap));
			}


			if (maxban != 0 || minban != 0)
			{
				query = query.Where(item => item.GiaBan >= minban && item.GiaBan <= maxban);
			}

			var totalItemCount = await query.CountAsync();
			var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
			var pagedList = new StaticPagedList<ProductViewModel>(model, page, pageSize, totalItemCount);

			ViewBag.PageStartItem = (page - 1) * pageSize + 1;
			ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
			ViewBag.Page = page;
			ViewBag.TotalItemCount = totalItemCount;
			ViewBag.searchTen = searchTen;
			ViewBag.lsp = lsp;
			ViewBag.ncc = ncc;

			ViewBag.minban = minban;
			ViewBag.maxban = maxban;

			return View(pagedList);
		}
	}
}
