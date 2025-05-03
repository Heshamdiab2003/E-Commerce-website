using Microsoft.AspNetCore.Mvc;
using Digital_Products.Models;
using Digital_Products.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;

namespace Digital_Products.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbcontext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(AppDbcontext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. عرض 6 فئات فقط
        public IActionResult LoadCategories()
        {
            var categories = _context.Categories.Take(6).ToList();
            return View(categories);
        }

        // 2. عرض كل الفئات
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // 3. عرض Top Brands
        public IActionResult TopBrands()
        {
            var brands = _context.Categories
                .Where(c => c.Name.Contains("Brand")) // عدل الشرط حسب التصنيف
                .ToList();
            return View(brands);
        }

        // 4. عرض Top Smartphones
        public IActionResult TopSmartPhones()
        {
            var phones = _context.Categories
                .Where(c => c.Name.Contains("Phone")) // عدل حسب التسمية
                .ToList();
            return View(phones);
        }

        // 5. GET: إنشاء كاتجوري - فقط للإدمن
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // تحقق من Claim يدويًا هنا إذا كنت تريد تخصيص الوصول
            if (!User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        // 6. POST: إنشاء كاتجوري
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Category category, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // مسار حفظ الصورة
                    var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "category");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fileName = Path.GetFileName(imageFile.FileName);
                    var fullPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // مسار يتم تخزينه في الداتابيز
                    category.ImagePath = $"/images/category/{fileName}";
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(category);
        }
    }
}
