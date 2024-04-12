using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using intex2.Models;
using intex2.Models.ViewModels;
using intex2.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace intex2.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private UserManager<AppUser> userManager;
    private ILegoRepository _repo;
    private readonly InferenceSession _session;
    private readonly PredictionService _prediction;
    private readonly Cart _cart;
    public HomeController(UserManager<AppUser> userMgr,ILogger<HomeController> logger, ILegoRepository temp, PredictionService prediction, Cart cart)
    {
        _logger = logger;
        userManager = userMgr;
        _repo = temp;
        _prediction = prediction;
        _cart = cart;
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
        
        var userId = userManager.GetUserId(User);
        var customerId = _repo.GetCustomerByNetUserId(userId).CustomerId;
        
        var recommendations = _repo.UserRecommendations.FirstOrDefault(r => r.ProductId == customerId);
        if (recommendations != null)
        {
            var recommendedProductIds = new List<int>();

            // Add the recommendation IDs to the list if they are not null
            if (recommendations.Rec1.HasValue) recommendedProductIds.Add(recommendations.Rec1.Value);
            if (recommendations.Rec2.HasValue) recommendedProductIds.Add(recommendations.Rec2.Value);
            if (recommendations.Rec3.HasValue) recommendedProductIds.Add(recommendations.Rec3.Value);
            if (recommendations.Rec4.HasValue) recommendedProductIds.Add(recommendations.Rec4.Value);

            var recommendedProducts = _repo.Products.Where(p => recommendedProductIds.Contains(p.ProductId)).ToList();

            // Pass the recommended products to the view
            ViewBag.RecommendedProducts = recommendedProducts;
        }

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Shop(int pageNum, string? productType, List<string> productTypes, List<string> colors, int pageSize = 20)
    {
        if (pageNum <= 0)
        {
            pageNum = 1;
        }

        // Get all products if no filters are applied
        IQueryable<Product> products = _repo.Products;
        if (!string.IsNullOrEmpty(productType))
        {
            // Filter products based on product type
            products = products.Where(p => p.Category.ToLower().Contains(productType.ToLower()));
        }
        else if (productTypes != null && productTypes.Count > 0)
        {
            // Filter products based on product types
            products = products.Where(p => productTypes.Any(pt => p.Category.ToLower().Contains(pt.ToLower())));
        }

        // If colors are specified, filter products based on colors
        if (colors != null && colors.Count > 0)
        {
            products = products.Where(p => colors.Any(c => p.PrimaryColor.ToLower().Contains(c.ToLower())));
        }

        // If colors are specified, filter products based on colors
        if (colors != null && colors.Count > 0)
        {
            products = products.Where(p => colors.Any(c => p.PrimaryColor.ToLower().Contains(c.ToLower())));
        }

        var model = new ProductsListViewModel
        {
            Products = products
                .OrderBy(x => x.Name)
                .Skip((pageNum - 1) * (pageSize))
                .Take(pageSize),

            PaginationInfo = new PaginationInfo
            {
                CurrentPage = pageNum,
                ItemsPerPage = pageSize,
                TotalItems = productType == null ? products.Count() : products.Count()
            },

            CurrentProductType = productType
        };
   
        var filteredProducts = model.Products.ToList();
        Console.WriteLine($"Filtered products: {filteredProducts.Count}");

        // If the request is an AJAX request, return the partial view with the filtered products
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_Products", filteredProducts);
        }
    
        return View(model);
    }
    
    [HttpPost]
    public IActionResult Shop([FromBody] ProductTypesViewModel filters)
    {
        // Get all products if no filters are applied
        IQueryable<Product> products = _repo.Products;

        // Filter products based on product types
        if (filters.ProductTypes != null && filters.ProductTypes.Count > 0)
        {
            products = products.Where(p => filters.ProductTypes.Any(pt => p.Category.ToLower().Contains(pt.ToLower())));
        }

        // Filter products based on colors
        if (filters.Colors != null && filters.Colors.Count > 0)
        {
            products = products.Where(p => filters.Colors.Any(c => p.PrimaryColor.ToLower().Contains(c.ToLower())));
        }

        var filteredProducts = products.ToList();

        // Return the partial view with the filtered products
        return PartialView("_Products", filteredProducts);
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
    
    [HttpPost]
    [Authorize]
    public IActionResult CheckoutConfirmationStart()
    {
        // Assuming the user is logged in and their ID is stored in User.Identity.Name
        var appUser = userManager.FindByNameAsync(User.Identity.Name).Result;
        var customer = _repo.GetCustomerByNetUserId(appUser.Id);
        if (customer == null)
        {
            return NotFound();
        }
        var order = _repo.Orders.FirstOrDefault(o => o.CustomerId == customer.CustomerId);
        if (order == null)
        {
            Order newOrder = new Order()
            {
                Amount = (int)_cart.CalculateTotal(),
            };
            return View("CheckoutConfirmation", newOrder);
        }
        Order newOrder1 = new Order()
        {
            ShippingAddress = order.ShippingAddress,
            Amount = (int)_cart.CalculateTotal(),
            CountryOfTransaction = order.CountryOfTransaction,
            Bank = order.Bank,
            TypeOfCard = order.TypeOfCard
        };
        return View("CheckoutConfirmation", newOrder1);
    }

    [HttpPost]
    [Authorize]
    public IActionResult CheckoutConfirmation(Order order)
    {
        if (ModelState.IsValid)
        {
            int fraud = _prediction.PredictFraud(order);
            order.Fraud = fraud;
            _repo.CreateOrder(order);
            _repo.Save();
            
            int orderId = order.TransactionId;
            List <LineItem> items = new List<LineItem>();
            foreach (var item in _cart.Lines)
            {
                LineItem newItem = new LineItem()
                {
                    TransactionId = orderId,
                    ProductId = item.Product.ProductId,
                    Qty = item.Quantity,
                };
                items.Add(newItem);
            }
            _repo.CreateLineItems(items);
            _repo.Save();

            if (fraud == 0)
            {
                return View("CheckoutSuccess", order);
            }
            else
            {
                return View("OrderPending", order);
            }
        }
        else
        {
            return View(order);
        }


    }
    
}