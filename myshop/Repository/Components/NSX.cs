using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myshop.Models;
using SQLitePCL;

namespace myshop.Repository.Components
{
    public class NSX : ViewComponent
    {
        private readonly MyshopContext _context;

        public NSX(MyshopContext context)
        {
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync() => View(await _context.NhaSanXuats.ToListAsync());
    }
}
