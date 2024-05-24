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
    public class LoaiSpController : Controller
    {
        private readonly MyshopContext _context;

        public LoaiSpController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Admin/LoaiSp
        // GET: Admin/LoaiSp
        public async Task<IActionResult> Index(string lsp, int page = 1, int pageSize = 5)
        {
            var query = _context.LoaiSps.ToList();

            if (!string.IsNullOrEmpty(lsp))
                query = query.Where(h => h.TenLoaiSp.Contains(lsp, StringComparison.OrdinalIgnoreCase)).ToList();

            var totalItemCount = query.Count();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pagedList = new StaticPagedList<LoaiSp>(model, page, pageSize, totalItemCount);
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.TotalItemCount = totalItemCount;
            ViewBag.Page = page;
            ViewBag.lsp = lsp;
            return View(pagedList);
        }



        // GET: Admin/LoaiSp/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiSp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoaiSp,TenLoaiSp")] LoaiSp loaiSp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiSp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiSp);
        }

        // GET: Admin/LoaiSp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LoaiSps == null)
            {
                return NotFound();
            }

            var loaiSp = await _context.LoaiSps.FindAsync(id);
            if (loaiSp == null)
            {
                return NotFound();
            }
            return View(loaiSp);
        }

        // POST: Admin/LoaiSp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiSp,TenLoaiSp")] LoaiSp loaiSp)
        {
            if (id != loaiSp.MaLoaiSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiSp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiSpExists(loaiSp.MaLoaiSp))
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
            return View(loaiSp);
        }

        // GET: Admin/LoaiSp/Delete/5
        public ActionResult Delete(int MaLoaiSp)
        {

            LoaiSp loaisp = _context.LoaiSps.Single(ma => ma.MaLoaiSp == MaLoaiSp);
            if (loaisp == null)
            {
                return NotFound();
            }

            _context.LoaiSps.Remove(loaisp);
            _context.SaveChanges();
            return RedirectToAction("Index", "LoaiSp");
        }
        private bool LoaiSpExists(int id)
        {
            return (_context.LoaiSps?.Any(e => e.MaLoaiSp == id)).GetValueOrDefault();
        }
    }
}
