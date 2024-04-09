using System.ComponentModel.DataAnnotations;
namespace intex2.Models
{
    public class TwoFactor
    {
        [Required]
        public string TwoFactorCode { get; set; }
    }
}