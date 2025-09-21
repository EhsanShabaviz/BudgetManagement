using BudgetManagement.Application.Interfaces;
using BudgetManagement.Application.Mappings;
using BudgetManagement.Application.UseCases;
using BudgetManagement.Application.Validators;
using BudgetManagement.Infrastructure.Identity;
using BudgetManagement.Infrastructure.Persistence;
using BudgetManagement.Infrastructure.Repositories;
using BudgetManagement.Infrastructure.Services;
using BudgetManagement.Infrastructure.Services.Excel;
using BudgetManagement.Web.Common.Models;
using BudgetManagement.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

// Application
builder.Services.AddScoped<IBudgetImportUseCase, BudgetImportUseCase>();
builder.Services.AddScoped<BudgetImportValidator>();
builder.Services.AddScoped<IBudgetCalculationUseCase, BudgetCalculationUseCase>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IClientInfoProvider, ClientInfoProvider>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(BudgetRecordProfile).Assembly);

// Infrastructure Adapters
builder.Services.AddScoped<IBudgetExcelReader, BudgetExcelReader>();


// Repository
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();

builder.Services.AddDbContext<BudgetManagementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // فعال‌کردن ریتری خودکار روی خطاهای موقت
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // تعداد تلاش مجدد
                maxRetryDelay: TimeSpan.FromSeconds(10), // فاصله بین تلاش‌ها
                errorNumbersToAdd: null // همه خطاهای موقت
            );
        }));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<BudgetManagementDbContext>()
.AddDefaultTokenProviders();


// تعریف نقش‌ها و سیاست‌ها
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserManager", p => p.RequireRole("Admin", "UserManager"));
    options.AddPolicy("User", p => p.RequireRole("Admin", "UserManager", "User"));
});

// پیکربندی کوکی‌ها
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";// مسیر صفحه ورود
    options.LogoutPath = "/Identity/Account/Logout";// مسیر صفحه خروج
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";// مسیر صفحه دسترسی غیرمجاز
    //options.ExpireTimeSpan = TimeSpan.FromHours(8);// مدت اعتبار کوکی
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;// تمدید خودکار اعتبار کوکی در صورت فعالیت کاربر
});

// رجیستر فونت فارسی برای QuestPDF
/*var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Vazir.ttf");
if (File.Exists(fontPath))
{
    FontManager.RegisterFont(File.OpenRead(fontPath));
}*/

// 1) Select QuestPDF license
QuestPDF.Settings.License = LicenseType.Community;
// اگر سازمان درآمد > $1M دارد، باید Professional استفاده شود.

var fontPath = Path.Combine(builder.Environment.WebRootPath, "fonts", "Vazir.ttf");
if (File.Exists(fontPath))
    FontManager.RegisterFont(File.OpenRead(fontPath));


var app = builder.Build();

//هر بار برنامه اجرا میشه، اگر نقش‌ها یا ادمین وجود نداشتند، ساخته میشه
await IdentitySeed.EnsureSeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapRazorPages();   // Login.cshtml این خط برای 
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


