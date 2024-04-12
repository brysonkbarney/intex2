using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace intex2.Models;

public partial class LineItem
{
    [Key]
    public int LineId { get; set; }
    public int? TransactionId { get; set; }

    public int? ProductId { get; set; }

    public int? Qty { get; set; }

    public int? Rating { get; set; }
}
