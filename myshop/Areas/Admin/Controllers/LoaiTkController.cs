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
    public class LoaiTkController : Controller
    {
        private readonly MyshopContext _context;

        public LoaiTkController(MyshopContext context)
        {
            _context = context ;
        }

        // GET: Admin/LoaiTk
        public async Task<IActionResult> Index(string ltk, int page = 1, int pageSize = 5)
        {
            if (_context != null)
            {
                var query = _context.LoaiTks.ToList();

                if (!string.IsNullOrEmpty(ltk))
                    query = query.Where(h => h.TenLoaiTk.Contains(ltk, StringComparison.OrdinalIgnoreCase)).ToList();

                var totalItemCount = query.Count();
                var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                var pagedList = new StaticPagedList<LoaiTk>(model, page, pageSize, totalItemCount);
                ViewBag.PageStartItem = (page - 1) * pageSize + 1;
                ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
                ViewBag.TotalItemCount = totalItemCount;
                ViewBag.Page = page;
                ViewBag.lsp = ltk;
                return View(pagedList);
            }
            else
            {
                // Xử lý trường hợp _context là null
                // Ví dụ: trả về một trang lỗi hoặc thông báo lỗi
                return NotFound(); // hoặc BadRequest() hoặc Redirect() hoặc một ActionResult khác tùy theo yêu cầu của bạn
            }
        }

        // GET: Admin/LoaiTk/Details/5


        // GET: Admin/LoaiTk/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LoaiTk/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoaiTk,TenLoaiTk")] LoaiTk loaiTk)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiTk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiTk);
        }

        // GET: Admin/LoaiTk/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LoaiTks == null)
            {
                return NotFound();
            }

            var loaiTk = await _context.LoaiTks.FindAsync(id);
            if (loaiTk == null)
            {
                return NotFound();
            }
            return View(loaiTk);
        }

        // POST: Admin/LoaiTk/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiTk,TenLoaiTk")] LoaiTk loaiTk)
        {
            if (id != loaiTk.MaLoaiTk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiTk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiTkExists(loaiTk.MaLoaiTk))
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
            return View(loaiTk);
        }

        // GET: Admin/LoaiTk/Delete/5

        // POST: Admin/LoaiTk/Delete/5

        public ActionResult Delete(int MaLoaiTk)
        {

            LoaiTk loaitk = _context.LoaiTks.Single(ma => ma.MaLoaiTk == MaLoaiTk);
            if (loaitk == null)
            {
                return NotFound();
            }

            _context.LoaiTks.Remove(loaitk);
            _context.SaveChanges();
            return RedirectToAction("Index", "LoaiTk");
        }

        private bool LoaiTkExists(int id)
        {
          return (_context.LoaiTks?.Any(e => e.MaLoaiTk == id)).GetValueOrDefault();
        }
    }
}
