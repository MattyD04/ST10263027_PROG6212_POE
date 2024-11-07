using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Data;
using FluentValidation.AspNetCore; // Ensure you have this namespace for FluentValidation

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework and connect to SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register FluentValidation services
builder.Services.AddControllersWithViews()
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssemblyContaining<ClaimViewModelValidator>();
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

app.UseAuthorization();

// Set up the default route to always open Index.cshtml in HomeController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Redirect the root URL to Home/Index
app.MapGet("/", context =>
{
    context.Response.Redirect("/Home/Index");
    return Task.CompletedTask;
});

app.Run();
