using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace intex2.Models;

public partial class Order
{
    [Key]
    public int TransactionId { get; set; }

    public int CustomerId { get; set; }

    public DateOnly? Date { get; set; }

    public string? DayOfWeek { get; set; }

    public int? Time { get; set; }

    public string? EntryMode { get; set; }

    public int? Amount { get; set; }

    public string? TypeOfTransaction { get; set; }
    [Required]
    public string? CountryOfTransaction { get; set; }
    [Required]
    public string? ShippingAddress { get; set; }
    [Required]
    public string? Bank { get; set; }
    [Required]
    public string? TypeOfCard { get; set; }

    public int? Fraud { get; set; }
}
