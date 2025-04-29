using Microsoft.AspNetCore.Mvc;
using Digital_Products.Models;
using Digital_Products.Data;

namespace Digital_Products.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbcontext _context;

        public CategoryController(AppDbcontext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }
    }
}
