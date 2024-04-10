using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using intex2.Models;
using intex2.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace intex2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private UserManager<AppUser> userManager;
    private ILegoRepository _repo;
    public HomeController(UserManager<AppUser> userMgr,ILogger<HomeController> logger, ILegoRepository temp)
    {
        _logger = logger;
        userManager = userMgr;
        _repo = temp;
    }
    [Authorize] 
    public IActionResult Secured()
    {
        return View((object)"Hello");
    }
    
    public IActionResult Index()
    {
        var productIds = new List<int> { 34, 9, 24, 37 }; // specify the product IDs you want to display
        var bestProductIds = new List<int> { 23, 19, 21, 22 }; // specify the other product IDs you want to display

        var model = new ProductsListViewModel
        {
            Products = _repo.Products.Where(p => productIds.Contains(p.ProductId)),
            BestProducts = _repo.Products.Where(p => bestProductIds.Contains(p.ProductId))
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Shop(int pageNum, string? productType)
    {
        if (pageNum <= 0)
        {
            pageNum = 1;
        }
            
        int pageSize = 5;

        var blah = new ProductsListViewModel
        {
            Products = _repo.Products
                .OrderBy(x => x.Name)
                .Skip((pageNum - 1) * (pageSize))
                .Take(pageSize),

            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = productType == null ? _repo.Products.Count() : _repo.Products.Count()
            },
                
            CurrentProductType = productType
        };

        return View(blah);
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

    public IActionResult ProductDetails(int id)
    {
        var product = _repo.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }

        var recommendations = _repo.ProductRecommendations.FirstOrDefault(r => r.ProductId == id);
        if (recommendations != null)
        {
            var recommendedProductIds = new List<int>();

            // Add the recommendation IDs to the list if they are not null
            if (recommendations.Rec1.HasValue) recommendedProductIds.Add(recommendations.Rec1.Value);
            if (recommendations.Rec2.HasValue) recommendedProductIds.Add(recommendations.Rec2.Value);
            if (recommendations.Rec3.HasValue) recommendedProductIds.Add(recommendations.Rec3.Value);
            if (recommendations.Rec4.HasValue) recommendedProductIds.Add(recommendations.Rec4.Value);
            if (recommendations.Rec5.HasValue) recommendedProductIds.Add(recommendations.Rec5.Value);

            var recommendedProducts = _repo.Products.Where(p => recommendedProductIds.Contains(p.ProductId)).ToList();

            // Pass the recommended products to the view
            ViewBag.RecommendedProducts = recommendedProducts;
        }

        return View(product);
    }
    
}