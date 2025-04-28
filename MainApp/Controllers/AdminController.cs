using Microsoft.AspNetCore.Mvc;
using MainApp.Models;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.DTOs;
using Infrastructure.Entities;
using Infrastructure.Hubs;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MainApp.Controllers;

public class AdminController(UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    RoleManager<IdentityRole> roleManager,
    IWebHostEnvironment env,
    IAuthService authService,
    INotificationService notificationService,
    IHubContext<NotificationHub> notificationHub
) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IWebHostEnvironment _env = env;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly IAuthService _authService = authService;

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AdminSignIn()
    {
        return View("AdminSignIn"); 
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminSignIn(AdminSignInFormModel model)
    {
        if (!ModelState.IsValid)
            return View("AdminSignIn", model);

        var success = await _authService.AdminLoginAsync(new AdminLoginDTO
        {
            Email = model.Email,
            Password = model.Password,
            RememberMe = model.RememberMe
        });

        if (!success)
        {
            ModelState.AddModelError("", "Invalid credentials or not an admin.");
            return View("AdminSignIn", model);
        }

        return RedirectToAction("Index", "Dashboard");
    }
    public IActionResult Projects()
    {
        return View();
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Members()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Clients()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AdminExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid provider");
            return View("AdminSignIn"); 
        }

        var redirectUrl = Url.Action("AdminExternalSignInCallback", "Admin", new { returnUrl })!; 
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> AdminExternalSignInCallback(string returnUrl = "/", string remoteError = null!)
    {
        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            return View("AdminSignIn"); 
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("AdminSignIn");

        var result = await _authService.ExternalLoginCallbackAsync(info);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View("AdminSignIn"); 
        }

        // Check if the logged-in user has Admin role here (important for security!)
        var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
        if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _signInManager.SignOutAsync();
            ModelState.AddModelError("", "You are not authorized as an Admin.");
            return View("AdminSignIn");
        }

        return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/admin/dashboard" : returnUrl); 
    }

}
