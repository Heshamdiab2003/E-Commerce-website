using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MegaMart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IConfiguration configuration, ILogger<SettingsController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var settings = new StoreSettings
            {
                StoreName = _configuration["StoreSettings:Name"],
                StoreDescription = _configuration["StoreSettings:Description"],
                StoreEmail = _configuration["StoreSettings:Email"],
                StorePhone = _configuration["StoreSettings:Phone"],
                StoreAddress = _configuration["StoreSettings:Address"],
                Currency = _configuration["StoreSettings:Currency"],
                TaxRate = decimal.Parse(_configuration["StoreSettings:TaxRate"] ?? "0"),
                ShippingCost = decimal.Parse(_configuration["StoreSettings:ShippingCost"] ?? "0"),
                MinimumOrderAmount = decimal.Parse(_configuration["StoreSettings:MinimumOrderAmount"] ?? "0")
            };

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSettings(StoreSettings settings)
        {
            if (ModelState.IsValid)
            {
                // Update configuration
                _configuration["StoreSettings:Name"] = settings.StoreName;
                _configuration["StoreSettings:Description"] = settings.StoreDescription;
                _configuration["StoreSettings:Email"] = settings.StoreEmail;
                _configuration["StoreSettings:Phone"] = settings.StorePhone;
                _configuration["StoreSettings:Address"] = settings.StoreAddress;
                _configuration["StoreSettings:Currency"] = settings.Currency;
                _configuration["StoreSettings:TaxRate"] = settings.TaxRate.ToString();
                _configuration["StoreSettings:ShippingCost"] = settings.ShippingCost.ToString();
                _configuration["StoreSettings:MinimumOrderAmount"] = settings.MinimumOrderAmount.ToString();

                _logger.LogInformation("Store settings updated by {User}", User.Identity.Name);
                TempData["SuccessMessage"] = "تم تحديث الإعدادات بنجاح";
            }

            return View("Index", settings);
        }
    }

    public class StoreSettings
    {
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string StoreEmail { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
        public string Currency { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal MinimumOrderAmount { get; set; }
    }
} 