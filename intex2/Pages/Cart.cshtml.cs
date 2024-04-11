using intex2.Infrastructure;
using intex2.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace intex2.Pages;

public class CartModel : PageModel
{
    private ILegoRepository _repo;
    public CartModel(ILegoRepository temp)
    {
        _repo = temp;
    }
    public string ReturnUrl { get; set; } = "/";
    public Cart? Cart { get; set; }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/";
        Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
    }
    public IActionResult OnPost(int productId, string returnUrl)
    {
        Product prod = _repo.Products
            .FirstOrDefault(x => x.ProductId == productId);

        if (prod != null)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.AddItem(prod,1);
            HttpContext.Session.SetJson("cart", Cart);
        }
        
        return RedirectToPage(new { returnUrl = returnUrl });
        
    }
}