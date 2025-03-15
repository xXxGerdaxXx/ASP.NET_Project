using Business.Services;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ? Register AppDbContext with LocalDB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Brodins\\Documents\\ASP_Net.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True"));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IClientService, ClientService>();



builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<MemberService>();




var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
//app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//.WithStaticAssets();

app.Run();




//using Business.Services;
//using Infrastructure.Data;
//using Infrastructure.Interfaces;
//using Infrastructure.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Identity;
//using Infrastructure.Models;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Brodins\\Documents\\ASP_Net.mdf;Integrated Security=True;Connect Timeout=30")));

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/admin/login"; // Redirect to login if unauthorized
//        options.AccessDeniedPath = "/admin/access-denied"; // Redirect if access is denied
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Session expires after 60 min
//        options.SlidingExpiration = true; // Reset expiration time on activity
//    });

//// ? Add Authorization Services
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireRole("Admin"));
//});

//// ? Register Identity Services
//builder.Services.AddIdentity<UserEntity, RoleEntity>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

//// ? Register Application Services
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IClientRepository, ClientRepository>();
//builder.Services.AddScoped<IMemberRepository, MemberRepository>();

//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<ClientService>();
//builder.Services.AddScoped<MemberService>();

//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// ? Ensure Roles & Admin User Exist (Inside Async Scope)
//using (var scope = app.Services.CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    await EnsureRolesAndAdminUserAsync(serviceProvider);
//}

//// ? Middleware
//app.UseHsts();
//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

//// ? Ensure Roles and Admin User Exist
//async Task EnsureRolesAndAdminUserAsync(IServiceProvider serviceProvider)
//{
//    var roleManager = serviceProvider.GetRequiredService<RoleManager<RoleEntity>>();
//    var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

//    // ? Ensure "Admin" Role Exists
//    if (!await roleManager.RoleExistsAsync("Admin"))
//    {
//        await roleManager.CreateAsync(new RoleEntity { RoleName = "Admin" });
//    }

//    // ? Ensure Admin User Exists
//    string adminEmail = "admin@example.com";
//    string adminPassword = "SecureAdmin123!";

//    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
//    if (existingAdmin == null)
//    {
//        var adminUser = new UserEntity
//        {
//            Username = "admin",
//            Email = adminEmail,

//        };
//        await userManager.CreateAsync(adminUser, adminPassword);
//        await userManager.AddToRoleAsync(adminUser, "Admin");
//        Console.WriteLine("? Admin User Created Successfully!");
//    }
//}