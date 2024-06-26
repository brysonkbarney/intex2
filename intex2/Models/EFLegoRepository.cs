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
    public IQueryable<UserRecommendations> UserRecommendations => _context.UserRecommendations;

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
        Customer customer = _context.Customers.Where(x => x.NetUserId == id).FirstOrDefault();
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
    public bool UpdateProduct(Product p)
    {
        try
        {
            _context.Products.Update(p);
            return true;
        }
        catch
        {
            return false;
        }
        
    }
    public void CreateProduct(Product p)
    {
        _context.Products.Add(p);
    }
    public void CreateOrder(Order o)
    {
        _context.Orders.Add(o);
    }
    public void CreateLineItems(List<LineItem> items)
    {
        _context.LineItems.AddRange(items);
    }

    public void DeleteProduct(Product p)
    {
        _context.Products.Remove(p);
    }

    public Customer GetCustomerByID(int id)
    {
        var customer = _context.Customers.Find(id);
        return customer;
    }
}