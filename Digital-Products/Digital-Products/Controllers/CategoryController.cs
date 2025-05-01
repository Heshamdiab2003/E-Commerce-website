using Microsoft.AspNetCore.Mvc;
using Digital_Products.Models;
using Digital_Products.Data;
using System.Linq;

namespace Digital_Products.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbcontext _context;

        public CategoryController(AppDbcontext context)
        {
            _context = context;
        }

        // عرض كل الكاتيجورز
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList(); // استرجاع كل الكاتيجوريات
            return View(categories); // عرضها في الفيو
        }
    }
}
