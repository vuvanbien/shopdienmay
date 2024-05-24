using Microsoft.AspNetCore.Mvc;

namespace myshop.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
