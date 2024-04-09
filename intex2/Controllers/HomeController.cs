using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using intex2.Models;
using Microsoft.AspNetCore.Identity;

namespace intex2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private UserManager<AppUser> userManager;
    public HomeController(UserManager<AppUser> userMgr,ILogger<HomeController> logger)
    {
        _logger = logger;
        userManager = userMgr;
    }
    [Authorize] 
    public IActionResult Secured()
    {
        return View((object)"Hello");
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        AppUser user = await userManager.GetUserAsync(HttpContext.User);
        string message = "Hello " + user.UserName;
        return View((object)message);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Shop()
    {
        return View();
    }
    public IActionResult AboutUs()
    {
        return View();
    }

    public IActionResult Cookies()
    {
        if (HttpContext.Request.Cookies.ContainsKey("CookieConsent") &&
            HttpContext.Request.Cookies["CookieConsent"] == "true")
        {
            // User has consented to the use of cookies.
            // It's safe to set non-essential cookies here.
            HttpContext.Response.Cookies.Append("YourNonEssentialCookie", "YourValue",
                new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        }

        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}