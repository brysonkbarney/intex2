namespace intex2.Models.ViewModels;

public class ProductsListViewModel
{
    public IQueryable<Product> Products { get; set; }
    public IEnumerable<Product> BestProducts { get; set; }

    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
    
    public string? CurrentProductType { get; set; }
}