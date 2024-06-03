using System.ComponentModel.DataAnnotations;

namespace BlazDrive.Models
{
    public class UserSignUp
    {
        [Required(ErrorMessage = "Required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [MinLength(5, ErrorMessage = "Name must be longer than 5 chars.")]
        [MaxLength(20, ErrorMessage = "Name must be shorter than 20 chars.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [MinLength(6, ErrorMessage = "Password must be longer than 6 chars.")]
        [MaxLength(30, ErrorMessage = "Password must be shorter than 30 chars.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*#?&])[!-~]{4,}$", ErrorMessage = "Password must contain at least 1 special char, 1 number and 1 uppercase letter")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Required.")]
        [Compare(nameof(this.Password), ErrorMessage = "Passwords do not match.")]
        public string? Password2 { get; set; }
    }
}