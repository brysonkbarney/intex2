namespace intex2.Models;
using System.ComponentModel.DataAnnotations;


public  class ProductRecommendations
{
    [Key]
    public int ProductId { get; set; }

    public int? Rec1 { get; set; }

    public int? Rec2 { get; set; }

    public int? Rec3 { get; set; }

    public int? Rec4 { get; set; }

    public int? Rec5 { get; set; }
}