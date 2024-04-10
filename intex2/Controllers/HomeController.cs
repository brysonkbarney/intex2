using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using intex2.Models;
using intex2.Models.ViewModels;
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
    public HomeController(UserManager<AppUser> userMgr,ILogger<HomeController> logger, ILegoRepository temp)
    {
        _logger = logger;
        userManager = userMgr;
        _repo = temp;
        
        //initialize InferenceSession, ensure the path is correct
        try
        {
            _session = new InferenceSession(
                "/Users/landongraham/Documents/GitHub/intex2/intex2/decision_tree_model.onnx");
            _logger.LogInformation("ONNX model loaded successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro loading ONNX model: {e.Message}");
        }
        
    }
    [Authorize] 
    public IActionResult Secured()
    {
        return View((object)"Hello");
    }
    
    [HttpPost]
    public IActionResult Predict(int time, int amount, int age, int transaction_shipping_match, int residence_transaction_match, 
        int day_of_week_Fri, int day_of_week_Mon, int day_of_week_Sat, int day_of_week_Sun, int day_of_week_Thu, int day_of_week_Tue, 
        int day_of_week_Wed, int entry_mode_CVC, int entry_mode_PIN, int type_of_transaction_POS, int country_of_transaction_China, 
        int country_of_transaction_India, int country_of_transaction_Russia, int country_of_transaction_USA, int shipping_address_China, 
        int shipping_address_India, int shipping_address_Russia, int shipping_address_USA, int bank_Barclays, int bank_HSBC, 
        int bank_Halifax, int bank_Lloyds, int bank_Metro, int bank_Monzo, int bank_RBS, int type_of_card_MasterCard, int type_of_card_Visa, 
        int gender_F)
    {
        // Dictionary mapping the numeric prediction to an animal type
        var class_type_dict = new Dictionary<int, string>
        {
            { 0, "non-fraudulent" },
            { 1, "fraudulent" }
        };

        try
        {
            var input = new List<float> { time, amount, age, transaction_shipping_match, residence_transaction_match, day_of_week_Fri, day_of_week_Mon, day_of_week_Sat, day_of_week_Sun, day_of_week_Thu, day_of_week_Tue, day_of_week_Wed, entry_mode_CVC, entry_mode_PIN, type_of_transaction_POS, country_of_transaction_China, country_of_transaction_India, country_of_transaction_Russia, country_of_transaction_USA, shipping_address_China, shipping_address_India, shipping_address_Russia, shipping_address_USA, bank_Barclays, bank_HSBC, bank_Halifax, bank_Lloyds, bank_Metro, bank_Monzo, bank_RBS, type_of_card_MasterCard, type_of_card_Visa, gender_F
            };
            var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
            };

            using (var results = _session.Run(inputs)) // makes the prediction with the inputs from the form
            {
                var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                if (prediction != null && prediction.Length > 0)
                {
                    // Use the prediction to get the animal type from the dictionary
                    var fraudPrediction = class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown");
                    ViewBag.Prediction = fraudPrediction;
                }
                else
                {
                    ViewBag.Prediction = "Error: Unable to make a prediction.";
                }
            }

            _logger.LogInformation("Prediction executed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during prediction: {ex.Message}");
            ViewBag.Prediction = "Error during prediction.";
        }

        return View("Index");
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