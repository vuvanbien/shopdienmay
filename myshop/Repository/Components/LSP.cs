using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myshop.Models;
using SQLitePCL;

namespace myshop.Repository.Components
{
    public class LSP : ViewComponent
    {
        private readonly MyshopContext _context;

        public LSP(MyshopContext context)
        {
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync() => View(await _context.LoaiSps.ToListAsync());
    }
}
