using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEShop.Models;
using MyEShop.Models.DatabaseContext;
using MyEShop.Models.DTO;
using MyEShop.Models.Entities;
using MyEShop.Models.ViewModel.Cart;
using MyEShop.Models.ViewModel.ListProduct;
using MyEShop.Services;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ZarinPal.Class;
using Dto.Payment;           
using Dto.Response.Payment;

namespace MyEShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyEshopContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;



        public HomeController(ILogger<HomeController> logger, MyEshopContext context, IMapper mapper, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var productVM = await _context.products
                .Where(p => p.Id == id)
                .AsNoTracking()
                .ProjectTo<DetailsViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return View(productVM);
        }

        [Authorize]
        public async Task<IActionResult> AddToCard(int itemId)
        {
            if (itemId == 0) return NotFound();

            var product = await _context.products
                .Include(p => p.Item)
                .FirstOrDefaultAsync(p => p.ItemId == itemId);

            if (product == null)
            {
                TempData["Error"] = "محصول مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            var order = await _context.orders
                .FirstOrDefaultAsync(o => o.UserId == userId && !o.Finaly);

            if (order != null)
            {
                var orderDetail = await _context.orderDetails
                    .FirstOrDefaultAsync(d => d.OrderId == order.OrderId && d.ProductId == product.Id);

                if (orderDetail != null)
                {
                    orderDetail.Count += 1;
                    _context.orderDetails.Update(orderDetail);
                }
                else
                {
                    var newOrderDetail = new OrderDetails()
                    {
                        OrderId = order.OrderId,
                        ProductId = product.Id,
                        Price = product.Item?.Price ?? 0,
                        Count = 1
                    };
                    await _context.orderDetails.AddAsync(newOrderDetail);
                }
            }
            else
            {
                var newOrder = new Order()
                {
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    Finaly = false
                };

                await _context.orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                var orderDetail = new OrderDetails()
                {
                    OrderId = newOrder.OrderId,
                    ProductId = product.Id,
                    Price = product.Item?.Price ?? 0,
                    Count = 1
                };
                await _context.orderDetails.AddAsync(orderDetail);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "محصول به سبد خرید اضافه شد";

            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> RemoveFromCart(int detailId)
        {
            var orderDetail = await _context.orderDetails.FindAsync(detailId);

            if (orderDetail != null)
            {
                _context.orderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();

                var order = await _context.orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == orderDetail.OrderId);

                if (order != null && (order.OrderDetails == null || !order.OrderDetails.Any()))
                {
                    _context.orders.Remove(order);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "محصول از سبد خرید حذف شد";
            }

            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> IncreaseFromCart(int detailId)
        {
            var orderDetail = await _context.orderDetails.FindAsync(detailId);

            if (orderDetail != null)
            {
                var product = await _context.products
                    .Include(p => p.Item)
                    .FirstOrDefaultAsync(p => p.Id == orderDetail.ProductId);

                if (product != null && product.Item != null && orderDetail.Count < product.Item.QuantityInStock)
                {
                    orderDetail.Count += 1;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تعداد محصول افزایش یافت";
                }
                else
                {
                    TempData["Error"] = "موجودی محصول کافی نیست";
                }
            }

            return RedirectToAction(nameof(ShowCart));
        }

        public async Task<IActionResult> DecreaseFromCart(int detailId)
        {
            var orderDetail = await _context.orderDetails.FindAsync(detailId);

            if (orderDetail != null)
            {
                if (orderDetail.Count > 1)
                {
                    orderDetail.Count -= 1;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تعداد محصول کاهش یافت";
                }
                else
                {
                    _context.orderDetails.Remove(orderDetail);
                    await _context.SaveChangesAsync();

                    var order = await _context.orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefaultAsync(o => o.OrderId == orderDetail.OrderId);

                    if (order != null && (order.OrderDetails == null || !order.OrderDetails.Any()))
                    {
                        _context.orders.Remove(order);
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "محصول از سبد خرید حذف شد";
                }
            }

            return RedirectToAction(nameof(ShowCart));
        }

        [Authorize]
        public async Task<IActionResult> ShowCart()
        {
            

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            var order = await _context.orders
                .Where(o => o.UserId == userId && !o.Finaly)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync();

            if (order == null || order.OrderDetails == null || !order.OrderDetails.Any())
            {
                ViewBag.EmptyCart = true;
                return View();
            }

            ViewBag.TotalPrice = order.OrderDetails.Sum(od => od.Count * od.Price);
            return View(order);
        }

        [Route("ContactUs")]
        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public async Task<IActionResult> Pay()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(userIdClaim);

            var order = await _context.orders
                .Where(o => o.UserId == userId && !o.Finaly)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();

            if (order == null || !order.OrderDetails.Any())
            {
                TempData["Error"] = "سبد خرید خالی است";
                return RedirectToAction(nameof(ShowCart));
            }

            long amount = (long)order.OrderDetails.Sum(od => od.Count * od.Price);

            var payment = new Payment();

            var dto = new DtoRequest
            {
                MerchantId = _configuration["ZarinPal:MerchantId"],
                Amount = (int)amount,
                Description = $"پرداخت سفارش شماره {order.OrderId}",
                CallbackUrl = _configuration["ZarinPal:CallbackUrl"] ?? Url.Action("VerifyPayment", "Home", null, Request.Scheme)
            };

            // لاگ برای دیباگ
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            _logger.LogInformation("Sending to ZarinPal: {Json}", json);

            var result = await payment.Request(dto, Payment.Mode.sandbox);

            // اگر به اینجا رسید، لاگ کن
            _logger.LogInformation("ZarinPal Response - Status: {Status}, Authority: {Authority}", result.Status, result.Authority);

            if (result.Status == 100)
            {
                order.Authority = result.Authority;
                await _context.SaveChangesAsync();
                return Redirect($"https://sandbox.zarinpal.com/pg/StartPay/{result.Authority}");
            }

            TempData["Error"] = $"خطا: {result.Status}";
            return RedirectToAction(nameof(ShowCart));
        }

    }
}