namespace intex2.Models.ViewModels;

public class ReviewOrdersViewModel
{
    public IQueryable<Order> Orders { get; set; }
    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
}