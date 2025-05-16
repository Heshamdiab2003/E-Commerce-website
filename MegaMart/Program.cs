using MegaMart.Data;
using MegaMart.Models;
using MegaMart.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ تفعيل تسجيل السجلات في الكونسول
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ✅ إعداد DbContext باستخدام SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ إعداد ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true; // لا يمكن تسجيل الدخول إلا بعد التفعيل
    options.User.RequireUniqueEmail = true;

    // تعديل متطلبات كلمة المرور لتكون أكثر أماناً
    options.Password.RequireDigit = true; // تتطلب رقم واحد على الأقل
    options.Password.RequireLowercase = true; // تتطلب حرف صغير على الأقل
    options.Password.RequireUppercase = true; // تتطلب حرف كبير على الأقل
    options.Password.RequireNonAlphanumeric = true; // تتطلب رمز خاص على الأقل
    options.Password.RequiredLength = 8; // الحد الأدنى للطول 8 أحرف
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ✅ إعدادات البريد الإلكتروني
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

// ✅ إعداد Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

// ✅ إضافة دعم MVC (Controllers with Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ استخدام Middleware
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
