
using myshop.Models;

using Microsoft.AspNetCore.Mvc;
using myshop.Repository;
using Microsoft.EntityFrameworkCore;

namespace myshop.Areas.Components
{
    public class ChiTiet : ViewComponent
    {
        private readonly MyshopContext _context;

        public ChiTiet(MyshopContext context)
        {
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync() => View(await _context.ChiTietHds.ToListAsync());
    }
}