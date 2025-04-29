
    using Microsoft.AspNetCore.Mvc;
    using Digital_Products.Models;


     using global::Digital_Products.Data;
    using global::Digital_Products.Models;

    namespace Digital_Products.Controllers
    {
        public class CartController : Controller
        {
            private readonly AppDbcontext _context;

            public CartController(AppDbcontext context)
            {
                _context = context;
            }

            public IActionResult Index()
            {
                var cartItems = _context.CartItems.ToList();
                return View(cartItems);
            }

            [HttpPost]
            public IActionResult AddToCart(int productId)
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return NotFound();
                }

                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1
                };

                _context.CartItems.Add(cartItem);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            public IActionResult RemoveFromCart(int id)
            {
                var item = _context.CartItems.Find(id);
                if (item == null)
                {
                    return NotFound();
                }

                _context.CartItems.Remove(item);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }
    }


