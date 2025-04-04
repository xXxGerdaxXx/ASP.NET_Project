//using Infrastructure.Data;
//using Microsoft.AspNetCore.Mvc;

//namespace MainApp.Controllers;

//public class TagsController(AppDbContext context) : Controller
//{
//    private readonly AppDbContext _context = context;

//    [HttpGet]
//    public async Task<IActionResult> SearchTags(string term)
//    {
//        if (string.IsNullOrEmpty(term))
//            return Json(new List<object>());

//        var tags = await _context.Tags
//            .Where(x => x.TagName.Contains(term))
//            .ToListAsync();

//        return Json(tags);
//    }
//}
