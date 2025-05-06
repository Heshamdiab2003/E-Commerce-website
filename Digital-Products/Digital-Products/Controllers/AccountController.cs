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
        public IActionResult SignUp() => View();

        [HttpPost]
        public IActionResult SignUp(User user)
        {
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
                return RedirectToAction("SignIn");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(User user)
        {
            var existingUser = _context.Users
            .FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash);

            if (existingUser != null)
            {
                var claims = new List<Claim>
{
new Claim(ClaimTypes.Name, existingUser.Username),
new Claim(ClaimTypes.Email, existingUser.Email),
new Claim(ClaimTypes.Role, existingUser.IsAdmin == true ? "Admin" : "User"),
new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()) // إضافة UserId
};

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                HttpContext.Session.SetString("Username", existingUser.Username);

                return RedirectToAction("Index", "Home");
            }

            ViewData["ErrorMessage"] = "Invalid email or password.";
            return View(user);
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("SignIn");
        }
    }
}