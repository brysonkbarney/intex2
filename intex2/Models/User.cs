using System.ComponentModel.DataAnnotations;

namespace intex2.Models
{
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool TwoFactor { get; set; } = false;
        
        public int CustomerId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateOnly? BirthDate { get; set; }
        [Required]
        public string? CountryOfResidence { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public double? Age { get; set; }
    
        public string? NetUserId { get; set; }
    }
}