using System.Diagnostics;
using Digital_Products.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Products.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            return View(); // تأكد إن في View اسمه SignIn.cshtml في Views/Home
        }

        public IActionResult SignUp()
        {
            return View(); // تأكد إن في View اسمه SignUp.cshtml في Views/Home
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
