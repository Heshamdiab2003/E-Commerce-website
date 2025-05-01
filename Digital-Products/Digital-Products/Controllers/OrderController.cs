using Microsoft.AspNetCore.Mvc;
using Digital_Products.Models;
using Digital_Products.Data;

namespace Digital_Products.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbcontext _context;

        public OrderController(AppDbcontext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }



        public IActionResult Details(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }
    }
}
