using MainApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Controllers
{
    [Route("clients")]
    public class ClientsController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return PartialView("_Create");  // ✅ Render as a partial view
        }

        [HttpPost("create")]
        public IActionResult Create(ClientCreateFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", model); // ✅ Return Partial if validation fails
            }

            // TODO: Save client to database

            return RedirectToAction("Index");
        }
    }
}
