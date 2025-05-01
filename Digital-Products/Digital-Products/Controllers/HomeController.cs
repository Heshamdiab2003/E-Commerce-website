using System.Diagnostics;
using Digital_Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Digital_Products.Data;

namespace Digital_Products.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbcontext _context;

        public HomeController(ILogger<HomeController> logger, AppDbcontext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View(); 
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }






        public IActionResult LoadCategories()
        {
            var categories = _context.Categories.Take(4).ToList();

         
            _logger.LogInformation($"عدد الفئات المسترجعة: {categories.Count}");

          
            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("لا توجد فئات لعرضها");
                return PartialView("_ListCategories", new List<Category>());
            }

            return PartialView("_ListCategories", categories);
        }
    }
}
