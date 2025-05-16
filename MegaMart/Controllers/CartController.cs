using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MegaMart.Models;

namespace MegaMart.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            // عرض محتوى السلة
            return View();
        }

        public IActionResult Checkout()
        {
            // ينقل إلى صفحة الشحن
            return RedirectToAction("ShippingInfo");
        }

        [HttpGet]
        public IActionResult ShippingInfo()
        {
            // نموذج الشحن
            return View();
        }

        [HttpPost]
        public IActionResult ShippingInfo(ShippingInfoModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // حفظ بيانات الشحن مؤقتاً
            return RedirectToAction("Payment");
        }

        [HttpGet]
        public IActionResult Payment()
        {
            // نموذج الدفع
            return View();
        }

        [HttpPost]
        public IActionResult Payment(PaymentModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // معالجة الدفع
            return RedirectToAction("Index", "Orders");
        }
    }
    // يمكنك إنشاء Models/ShippingInfoModel.cs و Models/PaymentModel.cs
} 