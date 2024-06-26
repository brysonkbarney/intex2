using intex2.Infrastructure;
using intex2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace intex2.Pages;

[Authorize]
public class CartModel : PageModel
{
    private ILegoRepository _repo;
    public Cart? Cart { get; set; }
    public CartModel(ILegoRepository temp, Cart cartService)
    {
        _repo = temp;
        Cart = cartService;
    }
    public string ReturnUrl { get; set; } = "/";
    
    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/";
    }
    public IActionResult OnPost(int productId, string returnUrl)
    {
        Product prod = _repo.Products
            .FirstOrDefault(x => x.ProductId == productId);

        if (prod != null)
        {
            Cart.AddItem(prod,1);
        }
        
        return RedirectToPage(new { returnUrl = returnUrl });
        
    }
    public IActionResult OnPostRemove(int productId, string returnUrl)
    {
        Cart.RemoveLine(Cart.Lines.First(x=>x.Product.ProductId==productId).Product);

        return RedirectToPage(new { returnUrl = returnUrl });
    }
}