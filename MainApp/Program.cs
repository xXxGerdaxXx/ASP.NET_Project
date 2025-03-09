using Business.Services;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Brodins\\Documents\\ASP_Net.mdf;Integrated Security=True;Connect Timeout=30")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<UserService>();



var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    //.WithStaticAssets();

app.Run();
