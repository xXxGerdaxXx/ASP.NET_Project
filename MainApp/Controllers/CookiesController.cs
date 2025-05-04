using Microsoft.AspNetCore.Mvc;
using MainApp.Models;
using System.Text.Json;

namespace MainApp.Controllers;

public class CookiesController : Controller
{
    [HttpPost]
    public IActionResult SetCookies([FromBody] CookieConsentFormModel consent)
    {
        if (consent == null)
            return BadRequest();

        if (consent.Functional)
        {
            Response.Cookies.Append("FunctionalCookie", "Non-Essential", new CookieOptions
            {
                IsEssential = false,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                Path = "/",
            });
        }
        else
        {
            Response.Cookies.Delete("FunctionalCookie");
        }

        if (consent.Analytics)
        {
            Response.Cookies.Append("AnalyticsCookie", "Non-Essential", new CookieOptions
            {
                IsEssential = false,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                Path = "/",
            });
        }
        else
        {
            Response.Cookies.Delete("AnalyticsCookie");
        }

        if (consent.Marketing)
        {
            Response.Cookies.Append("MarketingCookie", "Non-Essential", new CookieOptions
            {
                IsEssential = false,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                Path = "/",
            });
        }
        else
        {
            Response.Cookies.Delete("MarketingCookie");
        }

        Response.Cookies.Append("cookieConsent", JsonSerializer.Serialize(consent), new CookieOptions
        {
            IsEssential = false,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            SameSite = SameSiteMode.Lax,
            Path = "/",
        });

        return Ok();
    }

}
