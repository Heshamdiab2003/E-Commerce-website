using Digital_Products.Data;
using Digital_Products.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Digital_Products.Controllers
{
    public class AccountController : Controller
    {
         
        private readonly AppDbcontext _context;

        public AccountController(AppDbcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine("Validation error: " + error.ErrorMessage);
            }
            return View(user);
            //return View(user);
        }

    }
}
