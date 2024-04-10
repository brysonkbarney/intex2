namespace intex2.Models;

public class EFLegoRepository : ILegoRepository
{
    private Lego2IntexContext _context;
    
    public EFLegoRepository(Lego2IntexContext temp)
    {
        _context = temp;
    }
    
    public IQueryable<Product> Products => _context.Products;
    public IQueryable<ProductRecommendations> ProductRecommendations => _context.ProductRecommendations;

    public IQueryable<Order> Orders => _context.Orders;
    public void CreateCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
    }
    public void Save()
    {
        _context.SaveChanges();
    }

    public Customer GetCustomerByNetUserId(string id)
    {
        Customer customer = _context.Customers.Where(x => x.NetUserId == id).SingleOrDefault();
        return customer;
    }

    public bool UpdateCustomer(Customer customer)
    {
        try
        {
            _context.Customers.Update(customer);
            return true;
        }
        catch
        {
            return false;
        }
        
    }
}