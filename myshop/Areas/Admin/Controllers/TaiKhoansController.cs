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
    public class TaiKhoansController : Controller
    {
        private readonly MyshopContext _context;

        public TaiKhoansController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoans
        public async Task<IActionResult> Index(string tk, int page = 1, int pageSize = 5)
        {
            var query = _context.TaiKhoans.ToList();

            if (!string.IsNullOrEmpty(tk))
                query = query.Where(h => h.TenDn.Contains(tk, StringComparison.OrdinalIgnoreCase)).ToList();

            var totalItemCount = query.Count();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pagedList = new StaticPagedList<TaiKhoan>(model, page, pageSize, totalItemCount);
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.TotalItemCount = totalItemCount;
            ViewBag.Page = page;
            ViewBag.tk = tk;
            return View(pagedList);
        }

        // GET: Admin/TaiKhoans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TaiKhoans == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans

                .Include(t => t.MaLoaiTkNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoans/Create

        public IActionResult Create()
        {
            // Check if _context.Roles is not null before creating SelectList
            if (_context.LoaiTks != null)
            {
                ViewData["MaLoaiTk"] = new SelectList(_context.LoaiTks, "MaLoaiTk", "TenLoaiTk");
            }
            else
            {
                // Handle the case where _context.Roles is null (provide a default behavior or log an error)
                // For example, you can set ViewData["RoleId"] to an empty SelectList or handle it based on your requirements.
                ViewData["MaLoaiTk"] = new SelectList(new List<LoaiTk>(), "MaLoaiTk", "TenLoaiTk");
            }

            return View();
        }
        // POST: Admin/AdAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDn,MatKhau,TenKh,Sdt,Diachi,MaLoaiTk")] TaiKhoan tk)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Use RoleName for display and RoleId for the actual value
            ViewData["MaLoaiTk"] = new SelectList(_context.LoaiTks, "MaLoaiTk", "TenLoaiTk", tk.MaLoaiTk);
            return View(tk);
        }

        // GET: Admin/TaiKhoans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TaiKhoans == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            ViewData["MaLoaiTk"] = new SelectList(_context.LoaiTks, "MaLoaiTk", "MaLoaiTk", taiKhoan.MaLoaiTk);
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTk,TenDn,MatKhau,MaLoaiTk")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTk))
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

            ViewData["MaLoaiTk"] = new SelectList(_context.LoaiTks, "MaLoaiTk", "MaLoaiTk", taiKhoan.MaLoaiTk);
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoans/Delete/5
        public ActionResult Delete(int MaTk)
        {

            TaiKhoan tk = _context.TaiKhoans.Single(ma => ma.MaTk == MaTk);
            if (tk == null)
            {
                return NotFound();
            }

            _context.TaiKhoans.Remove(tk);
            _context.SaveChanges();
            return RedirectToAction("Index", "TaiKhoans");
        }

        private bool TaiKhoanExists(int id)
        {
          return (_context.TaiKhoans?.Any(e => e.MaTk == id)).GetValueOrDefault();
        }
    }
}
