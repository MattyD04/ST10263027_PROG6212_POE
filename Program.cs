using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework and connect to SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register FluentValidation services
builder.Services.AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ClaimViewModelValidator>()); // Register validators

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Enable session middleware before authorization

app.UseAuthentication(); // Required if using ASP.NET Identity
app.UseAuthorization();

// Set up the default route to always open Index.cshtml in HomeController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
