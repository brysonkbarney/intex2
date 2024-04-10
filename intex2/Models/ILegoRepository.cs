namespace intex2.Models;

public interface ILegoRepository 
{
    public IQueryable<Product> Products { get; }
    public IQueryable<Order> Orders { get; }
    IQueryable<ProductRecommendations> ProductRecommendations { get; }

    public void CreateCustomer(Customer customer);

    public void Save();

    public Customer GetCustomerByNetUserId(string id);
    public bool UpdateCustomer(Customer customer);
    public bool UpdateProduct(Product p);
    public void CreateProduct(Product p);
    public void DeleteProduct(Product p);
}