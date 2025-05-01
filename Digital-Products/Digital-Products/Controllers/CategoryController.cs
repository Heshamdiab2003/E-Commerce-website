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

        public IActionResult LoadCategories()
        {
            var categories = _context.Categories.Take(6).ToList();


            if (categories == null || !categories.Any())
            {
                Console.WriteLine("No categories found in the database.");  
            }
            else
            {
                Console.WriteLine($"Found {categories.Count} categories.");
            }

            return View(categories); 
        }

        public IActionResult Index()
        {
            var categories= _context.Categories.ToList();
            return View(categories);
        }

    }
}
