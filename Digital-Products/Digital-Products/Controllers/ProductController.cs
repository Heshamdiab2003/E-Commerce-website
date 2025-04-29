using Digital_Products.Data; 
using Digital_Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Digital_Products.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbcontext _context;

        public ProductController(AppDbcontext context)
        {
            _context = context;
        }

        // عرض كل المنتجات
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // بحث عن منتج
        public async Task<IActionResult> Search(string query)
        {
            var products = await _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToListAsync();

            return View("Index", products);
        }
    }
}
