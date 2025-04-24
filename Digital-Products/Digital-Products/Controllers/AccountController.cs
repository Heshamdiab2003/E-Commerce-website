using Digital_Products.Data;
using Digital_Products.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            // Check if email already exists
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Success"] = "Account created successfully! Please log in.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash);

            if (existingUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, existingUser.Username),
                    new Claim(ClaimTypes.Email, existingUser.Email),
                    new Claim(ClaimTypes.Role, existingUser.IsAdmin == true ? "Admin" : "User")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid Email or Password");
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }
    }
}
