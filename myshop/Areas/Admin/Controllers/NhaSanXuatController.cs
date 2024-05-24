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
    public class NhaSanXuatController : Controller
    {
        private readonly MyshopContext _context;

        public NhaSanXuatController(MyshopContext context)
        {
            _context = context;
        }

        // GET: Admin/NhaSanXuat
        public async Task<IActionResult> Index(string nsx, int page = 1, int pageSize = 5)
        {
            var query = _context.NhaSanXuats.ToList();

            if (!string.IsNullOrEmpty(nsx))
                query = query.Where(h => h.TenNsx.Contains(nsx, StringComparison.OrdinalIgnoreCase)).ToList();

            var totalItemCount = query.Count();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pagedList = new StaticPagedList<NhaSanXuat>(model, page, pageSize, totalItemCount);
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.TotalItemCount = totalItemCount;
            ViewBag.Page = page;
            ViewBag.nsx = nsx;
            return View(pagedList);
        }

        // GET: Admin/NhaSanXuat/Details/5


        // GET: Admin/NhaSanXuat/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaSanXuat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNsx,TenNsx")] NhaSanXuat nhaSanXuat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhaSanXuat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhaSanXuat);
        }

        // GET: Admin/NhaSanXuat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NhaSanXuats == null)
            {
                return NotFound();
            }

            var nhaSanXuat = await _context.NhaSanXuats.FindAsync(id);
            if (nhaSanXuat == null)
            {
                return NotFound();
            }
            return View(nhaSanXuat);
        }

        // POST: Admin/NhaSanXuat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNsx,TenNsx")] NhaSanXuat nhaSanXuat)
        {
            if (id != nhaSanXuat.MaNsx)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhaSanXuat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaSanXuatExists(nhaSanXuat.MaNsx))
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
            return View(nhaSanXuat);
        }

        // GET: Admin/NhaSanXuat/Delete/5
        public ActionResult Delete(int MaNsx)
        {

            NhaSanXuat nhaSanXuat = _context.NhaSanXuats.Single(ma => ma.MaNsx == MaNsx);
            if (nhaSanXuat == null)
            {
                return NotFound();
            }

            _context.NhaSanXuats.Remove(nhaSanXuat);
            _context.SaveChanges();
            return RedirectToAction("Index", "NhaSanXuat");
        }

        private bool NhaSanXuatExists(int id)
        {
          return (_context.NhaSanXuats?.Any(e => e.MaNsx == id)).GetValueOrDefault();
        }
    }
}
