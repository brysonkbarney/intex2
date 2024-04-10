namespace intex2.Models;

public class EFLegoRepository : ILegoRepository
{
    private Lego2IntexContext _context;
    
    public EFLegoRepository(Lego2IntexContext temp)
    {
        _context = temp;
    }
    
    public IQueryable<Product> Products => _context.Products;
}