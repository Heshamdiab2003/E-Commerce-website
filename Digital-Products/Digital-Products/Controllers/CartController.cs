using Microsoft.AspNetCore.Mvc;
using Digital_Products.Data;
using Digital_Products.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Digital_Products.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbcontext _context;

        public CartController(AppDbcontext context)
        {
            _context = context;
        }

        // Display cart items
        public IActionResult Index()
        {
            var cartItems = _context.CartItems
            .Include(c => c.Product)
            .ToList();
            return View(cartItems);
        }

        // Add item to cart via AJAX
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            Console.WriteLine($"Received ProductId: {productId}");
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                Console.WriteLine($"Product with ID {productId} not found.");
                return Json(new { success = false, message = "Product not found." });
            }

            var cartItem = _context.CartItems
            .FirstOrDefault(c => c.ProductId == productId && c.OrderId == null);
            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                _context.CartItems.Update(cartItem);
                Console.WriteLine($"Updated CartItem ID: {cartItem.Id}, Quantity: {cartItem.Quantity}");
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1
                };
                _context.CartItems.Add(cartItem);
                Console.WriteLine($"Added new CartItem for ProductId: {productId}");
            }

            try
            {
                _context.SaveChanges();
                Console.WriteLine("CartItem saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveChanges Error: {ex.Message}");
                return Json(new { success = false, message = $"Error saving to database: {ex.Message}" });
            }

            var cartCount = _context.CartItems
            .Where(c => c.OrderId == null)
            .Sum(c => c.Quantity);
            Console.WriteLine($"Cart Count: {cartCount}");

            return Json(new { success = true, cartCount = cartCount, message = "Product added to cart successfully!" });
        }

        // Remove item from cart
        public IActionResult RemoveFromCart(int id)
        {
            var item = _context.CartItems.Find(id);
            if (item == null)
            {
                TempData["Error"] = "Item not found.";
                return RedirectToAction("Index");
            }

            _context.CartItems.Remove(item);
            _context.SaveChanges();

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        // Update cart item quantity
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, string action)
        {
            Console.WriteLine($"Updating CartItem ID: {cartItemId}, Action: {action}");
            var cartItem = _context.CartItems
            .FirstOrDefault(c => c.Id == cartItemId && c.OrderId == null);
            if (cartItem == null)
            {
                Console.WriteLine($"CartItem with ID {cartItemId} not found.");
                return Json(new { success = false, message = "Cart item not found." });
            }

            if (action == "increment")
            {
                cartItem.Quantity += 1;
            }
            else if (action == "decrement")
            {
                cartItem.Quantity -= 1;
                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
            }
            else
            {
                return Json(new { success = false, message = "Invalid action." });
            }

            try
            {
                _context.SaveChanges();
                Console.WriteLine("Quantity updated successfully.");
                return Json(new { success = true, message = "Quantity updated successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveChanges Error: {ex.Message}");
                return Json(new { success = false, message = $"Error updating quantity: {ex.Message}" });
            }
        }

        // Redirect to ShippingInfo page on Proceed to Checkout
        [HttpPost]
        public IActionResult Checkout()
        {
            var cartItems = _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.OrderId == null)
            .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "السلة فارغة!";
                return RedirectToAction("Index");
            }

            if (cartItems.Any(item => item.Product == null))
            {
                TempData["Error"] = "بعض المنتجات في السلة غير صالحة.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("ShippingInfo");
        }

        // Display ShippingInfo form
        [HttpGet]
        public IActionResult ShippingInfo()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "يجب تسجيل الدخول لإتمام الطلب.";
                return RedirectToAction("SignIn", "Account"); // تغيير من Login إلى SignIn
            }

            var model = new ShippingInfo();
            return View(model);
        }

        // Process ShippingInfo and complete checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShippingInfo(ShippingInfo shippingInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(shippingInfo);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "يجب تسجيل الدخول لإتمام الطلب.";
                return RedirectToAction("SignIn", "Account"); // تغيير من Login إلى SignIn
            }

            var userExists = _context.Users.Any(u => u.Id == userId);
            if (!userExists)
            {
                TempData["Error"] = "المستخدم غير موجود.";
                return RedirectToAction("SignIn", "Account");
            }

            var cartItems = _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.OrderId == null)
            .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "السلة فارغة!";
                return RedirectToAction("Index");
            }

            if (cartItems.Any(item => item.Product == null))
            {
                TempData["Error"] = "بعض المنتجات في السلة غير صالحة.";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = cartItems.Sum(item => item.Quantity * item.Product.Price)
            };

            _context.Orders.Add(order);
            try
            {
                _context.SaveChanges();
                Console.WriteLine($"Order created successfully with ID: {order.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveChanges Error: {ex.Message}");
                TempData["Error"] = "حدث خطأ أثناء إنشاء الطلب.";
                return View(shippingInfo);
            }

            shippingInfo.OrderId = order.Id;
            _context.ShippingInfos.Add(shippingInfo);
            try
            {
                _context.SaveChanges();
                Console.WriteLine("ShippingInfo saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveChanges Error: {ex.Message}");
                TempData["Error"] = "حدث خطأ أثناء حفظ معلومات الشحن.";
                return View(shippingInfo);
            }

            foreach (var item in cartItems)
            {
                item.OrderId = order.Id;
                _context.CartItems.Update(item);
            }

            try
            {
                _context.SaveChanges();
                Console.WriteLine("Cart items associated with order successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveChanges Error: {ex.Message}");
                TempData["Error"] = "حدث خطأ أثناء ربط العناصر بالطلب.";
                return View(shippingInfo);
            }

            TempData["Success"] = "تم تنفيذ الطلب بنجاح!";
            return RedirectToAction("Index", "Order");
        }
    }
}