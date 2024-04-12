using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace intex2.Models;

public partial class Product
{
    public int ProductId { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public short? Year { get; set; }
    [Required]
    public int? NumParts { get; set; }
    [Required]
    public int Price { get; set; }
    [Required]
    public string? ImgLink { get; set; }
    [Required]
    public string? PrimaryColor { get; set; }
    [Required]
    public string? SecondaryColor { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public string? Category { get; set; }
    public int? Rating { get; set; }
    public int? ReviewCount { get; set; }
}
