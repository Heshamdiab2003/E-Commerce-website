using Microsoft.AspNetCore.Mvc;
using Digital_Products.Data;
using Digital_Products.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Digital_Products.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbcontext _context;

        public OrderController(AppDbcontext context)
        {
            _context = context;
        }

        // Display list of orders for the current user
        public IActionResult Index()
        {
            // الحصول على معرف المستخدم الحالي
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "يجب تسجيل الدخول لعرض الطلبات.";
                return RedirectToAction("Login", "Account");
            }

            // جلب الطلبات الخاصة بالمستخدم الحالي
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.CartItems)
                    .ThenInclude(ci => ci.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // Display details of a specific order
        public IActionResult Details(int id)
        {
            // الحصول على معرف المستخدم الحالي
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "يجب تسجيل الدخول لعرض تفاصيل الطلب.";
                return RedirectToAction("Login", "Account");
            }

            // جلب الطلب مع التأكد من أنه يخص المستخدم الحالي
            var order = _context.Orders
                .Include(o => o.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "الطلب غير موجود أو لا يخصك.";
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}