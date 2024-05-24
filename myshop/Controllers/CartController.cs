using Microsoft.AspNetCore.Mvc;
using myshop.Models;
using myshop.Models.ViewModels;
using myshop.Repository;

namespace myshop.Controllers
{
    public class CartController : Controller
    {
        private readonly MyshopContext _context;

        public CartController(MyshopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            int? tongSoLuong = cartItems.Sum(item => item.SoLuong);
            ViewBag.TongSoLuong = tongSoLuong;
            CartItemViewModel? cartItemVM = new()
            {
                CartItems = cartItems,
               
                GrandTotal = (double)cartItems.Sum(x => x.SoLuong * x.GiaKm),

            };
            return View(cartItemVM);
        }
        public async Task<IActionResult> Add(int maSP)
        {
            SanPham? sanpham = await _context.SanPhams.FindAsync(maSP);
            List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel? cartItems = cartItem.Where(c => c.MaSp == maSP).FirstOrDefault();

            if (cartItems == null)
            {
                cartItem.Add(new CartItemModel(sanpham));
            }
            else
            {
                cartItems.SoLuong += 1;
            }
            HttpContext.Session.SetJson("Cart", cartItem);
            TempData["success"] = "Thêm vào giỏ hàng thành công";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> UpdateCart(int maSP, int newQuantity)
        {
            // Kiểm tra sản phẩm có tồn tại trong cơ sở dữ liệu hay không. Nếu không, xử lý lỗi.
            SanPham? product = await _context.SanPhams.FindAsync(maSP);
            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại";
                return RedirectToAction("Error"); // Chuyển hướng đến trang thông báo lỗi
            }

            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel? cartItem = cartItems.FirstOrDefault(c => c.MaSp == maSP);

            if (cartItem != null)
            {
                if (newQuantity > 0)
                {
                    if (newQuantity <= product.SoLuong)
                    {
                        cartItem.SoLuong = newQuantity;
                        TempData["success"] = "Cập nhật giỏ hàng thành công";
                    }
                    else
                    {
                        cartItem.SoLuong = 1;
                        TempData["ErrorMessage"] = $"Sản phẩm {cartItem.TenSp} chỉ còn {product.SoLuong} sản phẩm";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Số lượng không hợp lệ.";
                }
            }
            HttpContext.Session.SetJson("Cart", cartItems);

            return RedirectToAction("");
        }
        public IActionResult Decrease(int maSP)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel? cartItem = cart.Where(c => c.MaSp == maSP).FirstOrDefault();

            if (cartItem != null)
            {
                if (cartItem.SoLuong > 1)
                {
                    cartItem.SoLuong--;
                }
                else
                {
                    cart.RemoveAll(p => p.MaSp == maSP);
                }

                if (cart.Count == 0)
                {
                    HttpContext.Session.Remove("Cart");
                }
                else
                {
                    HttpContext.Session.SetJson("Cart", cart);
                }
            }
            TempData["success"] = "Bớt  một giỏ hàng thành công";
            return RedirectToAction("Index"); // Sửa từ "Redirect" thành "RedirectToAction"
        }
        public IActionResult Increase(int maSP)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel? cartItem = cart.Where(c => c.MaSp == maSP).FirstOrDefault();

            if (cartItem != null)
            {
                if (cartItem.SoLuong >= 1)
                {
                    cartItem.SoLuong++; // Tăng số lượng
                }
                else
                {
                    cart.RemoveAll(p => p.MaSp == maSP); // Xóa sản phẩm khỏi giỏ hàng nếu số lượng = 1
                }

                if (cart.Count == 0)
                {
                    HttpContext.Session.Remove("Cart");
                }
                else
                {
                    HttpContext.Session.SetJson("Cart", cart);
                }
            }
            TempData["success"] = "Thêm một giỏ hàng thành công";
            return RedirectToAction("Index"); // Redirect về action "Index"
        }
        public IActionResult Remove(int maSP)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(c => c.MaSp == maSP);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "xóa một giỏ hàng thành công";
            return RedirectToAction("Index"); // Redirect về action "Index"
        }
        public IActionResult Clear(int maSP)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "xóa tất cảt giỏ hàng thành công";
            return RedirectToAction("Index");
        }
    }
}
