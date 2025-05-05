using Digital_Products.Data;
using Digital_Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Digital_Products.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbcontext _context;

        public ProductController(AppDbcontext context)
        {
            _context = context;
        }

        // عرض كل المنتجات أو البحث عنها
        public async Task<IActionResult> Search(string query)
        {
            var products = string.IsNullOrWhiteSpace(query)
                ? await _context.Products.Include(p => p.Category).ToListAsync()
                : await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                    .ToListAsync();

            return View("Index", products); // تأكد إن عندك View اسمه Index للعرض
        }

        // عرض المنتجات حسب التصنيف
        public async Task<IActionResult> ByCategory(int id)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return View("Index", products); // نفس View لعرض النتائج
        }

        // عرض كل المنتجات بشكل افتراضي
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }
    }
}
