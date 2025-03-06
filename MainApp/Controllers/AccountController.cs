using Business.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers;

public class AccountController(UserService userService) : Controller
{
    private readonly UserService _userService = userService;

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Authentication logic goes here...

        return RedirectToAction("Index", "Home"); // Redirect to homepage after login
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(UserDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            _userService.RegisterUser(model);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}
