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
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Services ----------------

// Razor & Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();

// Application Layer
builder.Services.AddScoped<IBudgetImportUseCase, BudgetImportUseCase>();
builder.Services.AddScoped<BudgetImportValidator>();
builder.Services.AddScoped<IBudgetCalculationUseCase, BudgetCalculationUseCase>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientInfoProvider, ClientInfoProvider>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IBudgetReportService, BudgetReportService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(BudgetRecordProfile).Assembly);

// Infrastructure Adapters
builder.Services.AddScoped<IBudgetExcelReader, BudgetExcelReader>();

// Repository
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();

// EF Core with Pooling
builder.Services.AddDbContextPool<BudgetManagementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }));

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<BudgetManagementDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserManager", p => p.RequireRole("Admin", "UserManager"));
    options.AddPolicy("User", p => p.RequireRole("Admin", "UserManager", "User"));
});

// Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/AccessDenied";
});

// QuestPDF
QuestPDF.Settings.License = LicenseType.Community;
var fontPath = Path.Combine(builder.Environment.WebRootPath, "fonts", "Vazir.ttf");
if (File.Exists(fontPath))
    FontManager.RegisterFont(File.OpenRead(fontPath));

// Response Compression (Brotli + Gzip)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream", "application/wasm" });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o =>
{
    o.Level = CompressionLevel.Fastest;
});

// Blazor Server SignalR
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        //options.MaximumReceiveMessageSize = 64 * 1024; // 64 KB
        options.MaximumReceiveMessageSize = 2 * 1024 * 1024; // 2 MB

    });

// ---------------- Pipeline ----------------

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    // فقط در Development برای تست Seed
    await IdentitySeed.EnsureSeedAsync(app.Services);
}

app.UseHttpsRedirection();

// Static files with long cache
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
