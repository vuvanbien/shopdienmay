using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using myshop.Models;
using X.PagedList;

namespace myshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
        private readonly MyshopContext _context;

        public HoaDonController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDon
        public async Task<IActionResult> Index(string email, string diachi, string sdt, DateTime beginDate = default, DateTime endDate = default, int page1 = 1, int page2 = 1, int page3 = 1, int page4 = 1, int pageSize = 5, int pageId = 1)
        {
            //var query = await _context.HoaDons.ToListAsync();
            ////var model = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            //var totalItemCount = query.Count();
            ////var pagedList = new StaticPagedList<HoaDon>(model, page, pageSize, totalItemCount);
            //var pagedList = query.ToPagedList();
            //ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            //ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            //ViewBag.TotalItemCount = totalItemCount;
            //ViewBag.Page = page;
            //return View(pagedList);
            if (beginDate == default)
            {
                beginDate = DateTime.MinValue.Date;
            }

            if (endDate == default)
            {
                endDate = DateTime.MaxValue.Date;
            }

            // Tổng
            var query = await _context.HoaDons.ToListAsync();
            //var pagedList = query.ToPagedList(page, pageSize);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(email))
                query = query.Where(o => o.TenKh.Contains(email, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(diachi))
                query = query.Where(h => h.DiaChi.Contains(diachi, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(sdt))
                query = query.Where(h => h.Sdt.Contains(sdt, StringComparison.OrdinalIgnoreCase)).ToList();

            if (beginDate < endDate)
                query = query.Where(h => h.Ngay > beginDate && h.Ngay < endDate).ToList();

            var totalItemCount = query.Count();
            ViewBag.TotalItemCount = totalItemCount;
            //ViewBag.PagedList = pagedList;


            // Chờ xác nhận
            var query1 = query.Where(o => o.TrangThai == 1);
            var pagedList1 = query1.ToPagedList(page1, pageSize);
            var totalItemCount1 = query1.Count();
            ViewBag.PageStartItem1 = (page1 - 1) * pageSize + 1;
            ViewBag.PageEndItem1 = Math.Min(page1 * pageSize, totalItemCount1);
            ViewBag.TotalItemCount1 = totalItemCount1;
            ViewBag.PagedList1 = pagedList1;

            // Đang giao hàng
            var query2 = query.Where(o => o.TrangThai == 2);
            var pagedList2 = query2.ToPagedList(page2, pageSize);
            var totalItemCount2 = query2.Count();
            ViewBag.PageStartItem2 = (page2 - 1) * pageSize + 1;
            ViewBag.PageEndItem2 = Math.Min(page2 * pageSize, totalItemCount2);
            ViewBag.TotalItemCount2 = totalItemCount2;
            ViewBag.PagedList2 = pagedList2;

            // Đã giao hàng
            var query3 = query.Where(o => o.TrangThai == 3);
            var pagedList3 = query3.ToPagedList(page3, pageSize);
            var totalItemCount3 = query3.Count();
            ViewBag.PageStartItem3 = (page3 - 1) * pageSize + 1;
            ViewBag.PageEndItem3 = Math.Min(page3 * pageSize, totalItemCount3);
            ViewBag.TotalItemCount3 = totalItemCount3;
            ViewBag.PagedList3 = pagedList3;

            // Đã hủy
            var query4 = query.Where(o => o.TrangThai == 4);
            var pagedList4 = query4.ToPagedList(page4, pageSize);
            var totalItemCount4 = query4.Count();
            ViewBag.PageStartItem4 = (page4 - 1) * pageSize + 1;
            ViewBag.PageEndItem4 = Math.Min(page4 * pageSize, totalItemCount4);
            ViewBag.TotalItemCount4 = totalItemCount4;
            ViewBag.PagedList4 = pagedList4;

            ViewBag.email = email;
            ViewBag.diachi = diachi;
            ViewBag.sdt = sdt;
            ViewBag.beginDate = beginDate;
            ViewBag.endDate = endDate;
            ViewBag.Page1 = page1;
            ViewBag.Page2 = page2;
            ViewBag.Page3 = page3;
            ViewBag.Page4 = page4;
            ViewBag.PageId = pageId;

            return View();
        }
        public IActionResult Xacnhan(int id, int page)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.MaHd == id);
            hoadon.TrangThai = 2;
            _context.Update(hoadon);
            _context.SaveChanges();
            return RedirectToAction("Index", page);
        }

        public IActionResult Vanchuyen(int id, int page)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.MaHd == id);
            hoadon.TrangThai = 3;
            _context.Update(hoadon);
            _context.SaveChanges();
            return RedirectToAction("Index", page);
        }
        public IActionResult Huy(int id, int page)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.MaHd == id);
            hoadon.TrangThai = 4;
            _context.Update(hoadon);
            _context.SaveChanges();
            return RedirectToAction("Index", page);
        }
        // GET: Admin/HoaDon/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = _context.HoaDons
                .Include(hd => hd.ChiTietHds)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefault(m => m.MaHd == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: Admin/HoaDon/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/HoaDon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,Ngay,Note,TrangThai,ThanhToan,TenKh,Sdt,DiaChi,TotalPrice")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hoaDon);
        }

        // GET: Admin/HoaDon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            return View(hoaDon);
        }

        // POST: Admin/HoaDon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,Ngay,Note,TrangThai,ThanhToan,TenKh,Sdt,DiaChi,TotalPrice")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hoaDon);
        }

        // GET: Admin/HoaDon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: Admin/HoaDon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.HoaDons == null)
            {
                return Problem("Entity set 'MyshopContext.HoaDons'  is null.");
            }
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
          return (_context.HoaDons?.Any(e => e.MaHd == id)).GetValueOrDefault();
        }
    }
}
