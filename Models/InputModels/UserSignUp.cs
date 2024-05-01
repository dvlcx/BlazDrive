using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        [MinLength(8, ErrorMessage = "Password must be longer than 6 chars.")]
        [MaxLength(30, ErrorMessage = "Password must be shorter than 30 chars.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Required.")]
        [Compare(nameof(this.Password), ErrorMessage = "Passwords do not match.")]
        public string? Password2 { get; set; }
    }
}