
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using myshop.Models;
using System.Data;
using System.Security.Claims;


namespace myshop.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyshopContext _context;
     

        public LoginController (MyshopContext context)
        {
            _context = context;
           
        }
        // GET: LoginController
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("TenDn") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult Login(TaiKhoan user)
        {
            if (HttpContext.Session.GetString("TenDn") == null)
            {
                var u = _context.TaiKhoans
                    .Where(x => x.TenDn.Equals(user.TenDn) && x.MatKhau.Equals(user.MatKhau))
                    .FirstOrDefault();

                if (u != null)
                {
                    HttpContext.Session.SetInt32("MaTk", u.MaTk );
                    HttpContext.Session.SetInt32("MaLoaiTk", u.MaLoaiTk ?? 0);
                    if (u.TenDn != null)
                    {
                        HttpContext.Session.SetString("TenDn", u.TenDn.ToString());
                        ViewData["TenDn"] = u.TenDn.ToString();
                    }

                    if (u.TenKh != null)
                    {
                        HttpContext.Session.SetString("TenKh", u.TenKh.ToString());
                      
                    }  
                    if(u.Sdt != null)
                    {
                        HttpContext.Session.SetString("Sdt", u.Sdt.ToString());
                    }
                    if (u.Diachi != null)
                    {
                        HttpContext.Session.SetString("Diachi", u.Diachi.ToString());
                    }

                    if (u.MaLoaiTk == 4)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }
            }

            return View();
        }
        [HttpGet]
        
        public IActionResult Logout()
        {
            // Xóa dữ liệu từ Session
            HttpContext.Session.Clear();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login");
        }
        

	}
}
