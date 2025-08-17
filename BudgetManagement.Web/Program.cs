using BudgetManagement.Application.Interfaces;
using BudgetManagement.Application.UseCases;
using BudgetManagement.Application.Validators;
using BudgetManagement.Infrastructure.Persistence;
using BudgetManagement.Infrastructure.Repositories;
using BudgetManagement.Infrastructure.Services.Excel;
using BudgetManagement.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Application
builder.Services.AddScoped<IBudgetImportUseCase, BudgetImportUseCase>();
builder.Services.AddScoped<BudgetImportValidator>();
builder.Services.AddScoped<IBudgetCalculationUseCase, BudgetCalculationUseCase>();


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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


