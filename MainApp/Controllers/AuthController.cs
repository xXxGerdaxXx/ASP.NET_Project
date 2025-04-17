using Infrastructure.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using MainApp.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MainApp.Controllers;

public class AuthController(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    RoleManager<IdentityRole> roleManager,
    IWebHostEnvironment env,
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


    [HttpGet]
    public IActionResult SignIn(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInFormModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            ModelState.AddModelError("Invalid", "Invalid email or password.");
            return View(model);
        }

        // ✅ Fetch user and send notification
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            await _notificationService.AddNotificationAsync(
                notificationTypeId: 1, // login type or generic
                message: $"{user.FirstName} {user.LastName} just signed in",
                image: "fa-solid fa-right-to-bracket"
            );
        }

        return RedirectToAction("Index", "Dashboard");
    }

    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);

        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        if (user != null)
        {
            await _notificationService.AddNotificationAsync(
                notificationTypeId: 2,
                message: $"{user.FirstName} {user.LastName} logged out",
                image: "fa-solid fa-right-from-bracket"
            );
        }

        return RedirectToAction("SignIn");
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpFormModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new UserEntity
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            AvatarUrl = "/images/default-avatar.png"
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        await _userManager.AddToRoleAsync(user, "User");

        return RedirectToAction("SignIn");
    }

    [HttpGet]
    public IActionResult AdminSignIn()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminSignIn(SignInFormModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            ModelState.AddModelError("", "Invalid credentials or not an admin.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Login failed.");
            return View(model);
        }

        // ✅ Now that login succeeded, send notification
        await _notificationService.AddNotificationAsync(
            notificationTypeId: 1,
            message: $"🛡️ Admin {user.FirstName} {user.LastName} signed in",
            image: "fa-solid fa-shield-halved"
        );

        return RedirectToAction("Dashboard", "Admin");
    }


    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new EditProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            CurrentAvatarUrl = user.AvatarUrl
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;

        if (model.Avatar != null && model.Avatar.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
            var extension = Path.GetExtension(model.Avatar.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("Avatar", "Only image files are allowed.");
                return View(model);
            }

            // Delete old avatar if it's not the default one
            if (!string.IsNullOrWhiteSpace(user.AvatarUrl) && user.AvatarUrl != "/images/default-avatar.png")
            {
                var oldAvatarPath = Path.Combine(_env.WebRootPath, user.AvatarUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldAvatarPath))
                {
                    System.IO.File.Delete(oldAvatarPath);
                }
            }

            // Save new avatar
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Avatar.CopyToAsync(fileStream);
            }

            user.AvatarUrl = "/uploads/avatars/" + uniqueFileName;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("EditProfile");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    public IActionResult Denied()
    {
        return View();
    }

    #region External Logins
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid provider");
            return View("SignIn");
        }
        var redirectUrl = Url.Action("ExternalSignInCallback", "Auth", new { returnUrl })!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        if (!string.IsNullOrEmpty(returnUrl) || returnUrl == "/") 
        {
            returnUrl = Url.Content("/dashboard");
        }

       
        

        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            return View("SignIn");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("SignIn");


        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );

        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            string firstName = string.Empty;
            string lastName = string.Empty;
            try
            {
                firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!;
                lastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!;

            } catch { }
            string email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            string username = $"ext_{info.LoginProvider.ToLower()}_{email}";
            var user = new UserEntity
            {
                Email = email,
                UserName = username, 
                FirstName = firstName,
                LastName = lastName
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("SignIn");
            
        }
    }

    #endregion

}