using Microsoft.EntityFrameworkCore;
using Digital_Products.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Digital_Products
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           


            // إضافة خدمات MVC
            builder.Services.AddControllersWithViews();

            // إعداد AppDbContext لقاعدة البيانات
            builder.Services.AddDbContext<AppDbcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // إعداد الكوكيز للتوثيق
            builder.Services.AddAuthentication("MyCookieAuth")
                .AddCookie("MyCookieAuth", options =>
                {
                    options.Cookie.Name = "MyCookieAuth";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            // إعداد الجلسات
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // مدة الجلسة
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;  // لتعيين الكوكيز كأمر أساسي في التطبيق
            });

    
        var app = builder.Build();

            app.UseStaticFiles();


            // إعدادات الـ Middleware (Pipeline) الخاصة بالتطبيق
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();  // صفحة الأخطاء في بيئة التطوير
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");  // عرض صفحة الأخطاء في بيئة الإنتاج
                app.UseHsts();  // تأمين الاتصال عبر HTTPS
            }

            app.UseHttpsRedirection();  // إعادة توجيه جميع الطلبات إلى HTTPS
            app.UseStaticFiles();  // تمكين ملفات الاستاتيكية (مثل الصور، CSS، JS)

            app.UseRouting();  // تمكين التوجيه للمسارات

            // إضافة الجلسات بعد التوجيه
            app.UseSession();  // تفعيل دعم الجلسات

            app.UseAuthentication();  // استخدام التوثيق باستخدام الكوكيز
            app.UseAuthorization();   // استخدام التفويض (التحقق من الأذونات)

            // إعداد التوجيه للمسارات الافتراضية
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // تشغيل التطبيق
            app.Run();
        }
    }
}
