using MainApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers
{
    [Route("members")]
    public class MembersController : Controller
    {
        // GET: /members
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /members/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return PartialView("_Create"); 
        }

        // POST: /members/create
        [HttpPost("create")]
        public IActionResult Create(MemberCreateForm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model); // Return modal with validation errors
            }

            // ✅ TODO: Save the new member to the database

            return Json(new { success = true }); // ✅ Return JSON response (Handled by JS)
        }
    }
}
