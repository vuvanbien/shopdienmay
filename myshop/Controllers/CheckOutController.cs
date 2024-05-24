using BraintreeHttp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myshop.Models;
using myshop.Models.ViewModels;
using myshop.Repository;
using PayPal.Core;
using PayPal.v1.Payments;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace myshop.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly MyshopContext _context;
        private readonly string? _clientId;
        private readonly string? _secretKey;
        private readonly double TyGia = 10000;
      

        public CheckOutController(MyshopContext context, IConfiguration  config )
        {
            _context = context;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
            
        }

           
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("TenKh");

            if (user == null)
            {

                return RedirectToAction("Login", "Login");
            }
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

        public IActionResult Dangky()
        {
            ViewBag.Message = TempData["Message"] as string; // Lấy thông báo từ TempData
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Dangky(string TenDn, string matkhau, string tenkh, string sdt, string diachi)
        {
            var ten = _context.TaiKhoans.FirstOrDefault(t => t.TenDn == TenDn);
            if (ten == null)
            {
                var user = new TaiKhoan
                {
                    TenDn = TenDn,
                    MatKhau = matkhau,
                    TenKh = tenkh,
                    Sdt = sdt,
                    Diachi = diachi,
                    MaLoaiTk = 4
                };

                _context.TaiKhoans.Add(user);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Đăng ký thành công"; // Lưu thông báo vào TempData
                return RedirectToAction("Dangky", "CheckOut");
            }

            ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
            return View();
        }
        [HttpPost]
        public  IActionResult Checkout(string TenKh, string Sdt, string Diachi, String Note)
        {
            
            
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                if (cartItems.Count == 0)
                {
                    TempData["error"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }
                CartItemViewModel? cartItemVM = new()
                {
                    CartItems = cartItems,
                    
                    GrandTotal = (double)cartItems.Sum(x => x.SoLuong * x.GiaKm)
                
                };
                var hoaDon = new HoaDon
                    {
                        Ngay = DateTime.Now,
                        TenKh = TenKh,
                        Sdt = Sdt,
                        Note = Note,
                        TrangThai = 1,
                        ThanhToan = 0,
                        DiaChi = Diachi,
                        TotalPrice = (decimal?)cartItemVM.GrandTotal
                   
                    };
                _context.Add(hoaDon);
                _context.SaveChanges();


                foreach (var cart in cartItems)
                {
                    var orderDetail = new ChiTietHd();
                    orderDetail.MaHd = hoaDon.MaHd;
                    orderDetail.MaSp = cart.MaSp;
                    if(cart.GiaKm != null)
                    {
                        orderDetail.Price = (decimal?)cart.GiaKm;
                    }
                    else
                    {
                    orderDetail.Price = (decimal?)cart.GiaBan;
                    }
                    
                    orderDetail.SoLuong = cart.SoLuong;
                    _context.Add(orderDetail);
                    _context.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Thanh toán thành công, vui lòng chờ duyệt đơn hàng";
                return RedirectToAction("Index", "Cart");
            
          
        }

        public async Task<IActionResult> PaypalCheckout()
        {
            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

          

            
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                if (cartItems.Count == 0)
                {
                    TempData["error"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                    return RedirectToAction("Index", "Cart");
                }

                var environment = new SandboxEnvironment(_clientId, _secretKey);
                var client = new PayPalHttpClient(environment);

                #region Create Paypal Order
                var itemList = new ItemList()
                {
                    Items = new List<Item>()
                };
                var total = Math.Round((decimal)(cartItems.Sum(p => p.thanhTien) / TyGia), 2);
                foreach (var item in cartItems)
                {
                    itemList.Items.Add(new Item()
                    {
                        Name = item.TenSp,
                        Currency = "USD",
                        Price = Math.Round((item.GiaKm ?? 0) / (double)TyGia, 2).ToString(),
                        Quantity = item.SoLuong.ToString(),
                        Sku = "sku",
                        Tax = "0"
                    });
                }
                #endregion

                var payment = new Payment()
                {
                    Intent = "sale",
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = new Amount()
                            {
                                Total = total.ToString(),
                                Currency = "USD",
                                Details = new AmountDetails
                                {
                                    Tax = "0",
                                    Shipping = "0",
                                    Subtotal = total.ToString()
                                }
                            },
                            ItemList = itemList,
                            Description = $"Invoice #{paypalOrderId}",
                            InvoiceNumber = paypalOrderId.ToString()
                        }
                    },
                    RedirectUrls = new RedirectUrls()
                    {
                        CancelUrl = $"{hostname}/Checkout/CheckoutFail",
                        ReturnUrl = $"{hostname}/Checkout/CheckoutSuccess"
                    },
                    Payer = new Payer()
                    {
                        PaymentMethod = "paypal"
                    }
                };

                PaymentCreateRequest request = new PaymentCreateRequest();
                request.RequestBody(payment);

                try
                {
                    var response = await client.Execute(request);
                    var statusCode = response.StatusCode;
                    Payment result = response.Result<Payment>();

                    var links = result.Links.GetEnumerator();
                    string? paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        LinkDescriptionObject lnk = links.Current;
                        if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.Href;
                        }
                    }

                    return Redirect(paypalRedirectUrl);
                }
                catch (HttpException httpException)
                {
                    var statusCode = httpException.StatusCode;
                    var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                    //Process when Checkout with Paypal fails
                    return Redirect("/Checkout/CheckoutFail");
                }
            
        }

        public IActionResult CheckoutFail()
        {
            TempData["success"] = "Chưa thanh toán thành công, vui lòng thanh toán lại";
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult CheckoutSuccess()
        {

            var tenKh = HttpContext.Session.GetString("TenKh");
            var sdt = HttpContext.Session.GetString("Sdt");
            var diachi = HttpContext.Session.GetString("Diachi");
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel? cartItemVM = new()
            {
                CartItems = cartItems,

                GrandTotal = (double)cartItems.Sum(x => x.SoLuong * x.GiaKm),

            };
            var hoaDon = new HoaDon
            {
                TenKh = tenKh,
                Sdt = sdt,
                Note = "",
                TrangThai = 1,
                ThanhToan = 1,
                DiaChi = diachi,
                TotalPrice = (decimal)cartItemVM.GrandTotal
            };
            _context.Add(hoaDon);
            _context.SaveChanges();

            foreach (var cart in cartItems)
            {
                var orderDetail = new ChiTietHd();
                orderDetail.MaHd = hoaDon.MaHd;
                orderDetail.MaSp = cart.MaSp;
                if (cart.GiaKm != null)
                {
                    orderDetail.Price = (decimal)cart.GiaKm;
                }
                else
                {
                    orderDetail.Price = (decimal?)cart.GiaBan;
                }
                orderDetail.SoLuong = cart.SoLuong;
                _context.Add(orderDetail);
                _context.SaveChanges();
            }

            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Thanh toán thành công, vui lòng chờ duyệt đơn hàng";
            return RedirectToAction("Index", "Cart");
        }
        public async Task<IActionResult> Lichsu(int page = 1, int pageSize = 10)
        {


           var user = _context.TaiKhoans.FirstOrDefault();

            // Kiểm tra nếu user không null và có thuộc tính Email
            if ( user != null && !string.IsNullOrEmpty(user.TenKh))
            {
                // Sử dụng Email trong truy vấn
                var query = _context.HoaDons.Where(hd => hd.TenKh == user.TenKh).OrderByDescending(hd => hd.Ngay);
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                int? tongSoLuong = (int?)cartItems.Sum(item => item.SoLuong);
                ViewBag.TongSoLuong = tongSoLuong;
                var model = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var totalItemCount = query.Count();
                var pagedList = new StaticPagedList<HoaDon>(model, page, pageSize, totalItemCount);
                ViewBag.PageStartItem = (page - 1) * pageSize + 1;
                ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
                ViewBag.Page = page;
                ViewBag.TotalItemCount = totalItemCount;
                /*ViewBag.lsp = lsp;*/
                return View(pagedList);
            }
            else
            {
                // Xử lý khi không có thông tin về người dùng
                return RedirectToAction("Login", "Login"); // Hoặc chuyển hướng đến trang login
            }
        }
        public IActionResult Details(int id)
        {
            var orderDetailList = _context.ChiTietHds
                .Where(o => o.MaHd == id)
                .Join(
                    _context.SanPhams,
                    orderDetail => orderDetail.MaSp,
                    product => product.MaSp,
                    (orderDetail, product) => new ChiTietHd
                    {
                        SanPham = product,
                        SoLuong = orderDetail.SoLuong,
                        Price = orderDetail.Price,
                        // Add other properties as needed
                    })
                .ToList();

            return PartialView("_OrderDetailPartialView", orderDetailList);
        }
        public IActionResult Huy(int id, int page)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.MaHd == id);

            if (hoadon == null)
            {
                TempData["error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Lichsu", page);
            }

            hoadon.TrangThai = 4;
            _context.Update(hoadon);
            _context.SaveChanges();

            var chiTietHoaDons = _context.ChiTietHds.Where(ct => ct.MaHd == id).ToList();
            foreach (var chiTiet in chiTietHoaDons)
            {
                var product = _context.SanPhams.Find(chiTiet.MaSp);

                if (product != null)
                {
                    product.SoLuong += chiTiet.SoLuong;
                    _context.Update(product);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Lichsu", page);
        }
    }
}
