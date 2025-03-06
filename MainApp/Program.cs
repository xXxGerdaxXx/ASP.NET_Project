using Business.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ? Register AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Brodins\\Documents\\ASP_Net.mdf;Integrated Security=True;Connect Timeout=30")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<UserService>(); // ? Registers UserService correctly


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
