using MailKit.Net.Smtp;
using MailKit.Security;
using MegaMart.Data;
using MegaMart.Models;
using MegaMart.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MegaMart.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace MegaMart.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<EmailSettings> emailSettings,
            IConfiguration configuration,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSettings = emailSettings.Value;
            _configuration = configuration;
            _emailSender = emailSender;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                try
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    // إنشاء رابط التأكيد مع اسم المضيف الصحيح
                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: Request.Scheme,
                        host: Request.Host.Value);

                    _logger.LogInformation($"Generated confirmation link: {confirmationLink}");

                    await _emailSender.SendConfirmationEmailAsync(user.Email, confirmationLink);
                    _logger.LogInformation($"Registration successful for {user.Email}, confirmation email sent");

                    return RedirectToAction("EmailVerificationSent");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending confirmation email to {user.Email}");
                    // Delete the user since we couldn't send the confirmation email
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "حدث خطأ أثناء إرسال بريد التأكيد. يرجى المحاولة مرة أخرى.");
                    return View(model);
                }
            }

            foreach (var error in result.Errors)
            {
                _logger.LogWarning($"Registration failed for {model.Email}: {error.Description}");
                string errorMessage = error.Code switch
                {
                    "PasswordRequiresNonAlphanumeric" => "يجب أن تحتوي كلمة المرور على رمز خاص واحد على الأقل (!#$%^&*)",
                    "PasswordRequiresDigit" => "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل",
                    "PasswordRequiresLower" => "يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل",
                    "PasswordRequiresUpper" => "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل",
                    "PasswordTooShort" => "يجب أن تكون كلمة المرور 8 أحرف على الأقل",
                    "DuplicateUserName" => "هذا البريد الإلكتروني مستخدم بالفعل",
                    "InvalidUserName" => "البريد الإلكتروني غير صالح",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EmailVerificationSent() => View();

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Invalid email confirmation attempt: missing userId or token");
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Email confirmation failed: user not found with ID {userId}");
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Email confirmed successfully for user {user.Email}");
                return View("ConfirmEmailSuccess");
            }

            _logger.LogWarning($"Email confirmation failed for user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return View("Error");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        _logger.LogWarning($"Login attempt for unconfirmed email: {model.Email}");
                        ModelState.AddModelError(string.Empty, "يرجى تأكيد بريدك الإلكتروني قبل تسجيل الدخول.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {user.Email} logged in successfully");
                        
                        // Check if user is admin
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed login attempt for {model.Email}: Invalid password");
                        ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Failed login attempt: User not found with email {model.Email}");
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }
    }
}
