//using Business.Services;
//using Infrastructure.DTOs;
//using Infrastructure.Models;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace MainApp.Controllers;

//[Route("admin")]
////[Authorize(Roles = "Admin")] 
//public class AdminController(UserService userService, MemberService memberService, ClientService clientService) : Controller
//{
//    private readonly UserService _userService = userService;
//    private readonly MemberService _memberService = memberService;
//    private readonly ClientService _clientService = clientService;

//    // ✅ Admin Login
//    [AllowAnonymous]
//    [HttpGet("login")]
//    public IActionResult Login()
//    {
//        return View(); // Create a separate admin login view (Admin/Login.cshtml)
//    }

//    [AllowAnonymous]
//    [HttpPost("login")]
//    public async Task<IActionResult> Login(AdminLoginDTO loginDTO)
//    {
//        if (!ModelState.IsValid)
//            return View(loginDTO);

//        var user = await _userService.AuthenticateUserAsync(loginDTO.Email, loginDTO.Password);
//        if (user == null || user.Role.RoleName != "Admin") // Ensure user is an admin
//        {
//            ModelState.AddModelError("", "Invalid admin credentials.");
//            return View(loginDTO);
//        }

//        if (loginDTO.AdminCode != "YOUR_SECRET_ADMIN_CODE") // Extra admin security check
//        {
//            ModelState.AddModelError("", "Invalid admin access code.");
//            return View(loginDTO);
//        }

//        // ✅ Create Admin Claims
//        var claims = new List<Claim>
//        {
//            new Claim(ClaimTypes.Name, user.Username),
//            new Claim(ClaimTypes.Email, user.Email),
//            new Claim(ClaimTypes.Role, "Admin") // Explicitly assign "Admin" role
//        };

//        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//        var authProperties = new AuthenticationProperties { IsPersistent = loginDTO.RememberMe };

//        // ✅ Sign in the Admin
//        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
//                                      new ClaimsPrincipal(claimsIdentity),
//                                      authProperties);

//        return RedirectToAction("Dashboard", "Admin"); // ✅ Redirect to admin dashboard
//    }

//    [HttpPost("logout")]
//    public async Task<IActionResult> Logout()
//    {
//        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//        return RedirectToAction("Login");
//    }

//    // ✅ Admin Dashboard
//    [HttpGet("dashboard")]
//    public IActionResult Dashboard()
//    {
//        return View(); // Admin dashboard view
//    }

//    // ✅ CRUD for Members

//    // ✅ READ - Get All Members
//    [HttpGet("members")]
//    public async Task<IActionResult> GetMembers()
//    {
//        var members = await _memberService.GetAllMembersAsync();
//        return View(members);
//    }

//    // ✅ CREATE - Show Member Form
//    [HttpGet("members/create")]
//    public IActionResult CreateMember()
//    {
//        return View();
//    }

//    // ✅ CREATE - Add New Member
//    [HttpPost("members/create")]
//    public async Task<IActionResult> CreateMember(MemberEntity member)
//    {
//        if (!ModelState.IsValid) return View(member);
//        await _memberService.CreateMemberAsync(member);
//        return RedirectToAction("GetMembers");
//    }

//    // ✅ UPDATE - Show Edit Form
//    [HttpGet("members/edit/{id}")]
//    public async Task<IActionResult> EditMember(int id)
//    {
//        var member = await _memberService.GetMemberByIdAsync(id);
//        if (member == null) return NotFound();
//        return View(member);
//    }

//    // ✅ UPDATE - Save Changes
//    [HttpPost("members/edit")]
//    public async Task<IActionResult> EditMember(MemberEntity updatedMember)
//    {
//        if (!ModelState.IsValid) return View(updatedMember);
//        var success = await _memberService.UpdateMemberAsync(updatedMember);
//        if (!success) return NotFound();
//        return RedirectToAction("GetMembers");
//    }

//    // ✅ DELETE - Remove Member
//    [HttpPost("members/delete/{id}")]
//    public async Task<IActionResult> DeleteMember(int id)
//    {
//        await _memberService.DeleteMemberAsync(id);
//        return RedirectToAction("GetMembers");
//    }

//    // ✅ CRUD for Clients

//    // ✅ READ - Get All Clients
//    [HttpGet("clients")]
//    public async Task<IActionResult> GetClients()
//    {
//        var clients = await _clientService.GetAllClientsAsync();
//        return View(clients);
//    }

//    // ✅ CREATE - Show Client Form
//    [HttpGet("clients/create")]
//    public IActionResult CreateClient()
//    {
//        return View();
//    }

//    // ✅ CREATE - Add New Client
//    [HttpPost("clients/create")]
//    public async Task<IActionResult> CreateClient(ClientEntity client)
//    {
//        if (!ModelState.IsValid) return View(client);
//        await _clientService.CreateClientAsync(client);
//        return RedirectToAction("GetClients");
//    }

//    // ✅ UPDATE - Show Edit Form
//    [HttpGet("clients/edit/{id}")]
//    public async Task<IActionResult> EditClient(int id)
//    {
//        var client = await _clientService.GetClientByIdAsync(id);
//        if (client == null) return NotFound();
//        return View(client);
//    }

//    // ✅ UPDATE - Save Changes
//    [HttpPost("clients/edit")]
//    public async Task<IActionResult> EditClient(ClientEntity updatedClient)
//    {
//        if (!ModelState.IsValid) return View(updatedClient);
//        var success = await _clientService.UpdateClientAsync(updatedClient);
//        if (!success) return NotFound();
//        return RedirectToAction("GetClients");
//    }

//    // ✅ DELETE - Remove Client
//    [HttpPost("clients/delete/{id}")]
//    public async Task<IActionResult> DeleteClient(int id)
//    {
//        await _clientService.DeleteClientAsync(id);
//        return RedirectToAction("GetClients");
//    }
//}
using Microsoft.AspNetCore.Mvc;
using MainApp.Models;

namespace MainApp.Controllers;
[Route("admin")]

public class AdminController : Controller
{
    [Route("members")]
    public IActionResult Members()
    {
        return View();
    }

    [Route("clients")]

    public IActionResult Clients()
    {
        return View();
    }

    [HttpPost]

    public IActionResult CreateClient(ClientCreateFormModel form)
    {
        if (!ModelState.IsValid)

            return RedirectToAction("Clients");

        return View();
    }
    [HttpPost]

    public IActionResult EditClient(ClientCreateFormModel form)
    {
        if (!ModelState.IsValid)

            return RedirectToAction("Clients");

        return View();
    }
}