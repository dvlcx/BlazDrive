using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FoolProof.Core;

namespace BlazDrive.Models.InputModels
{
    public class UserEditInput
    {   
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [MinLength(5, ErrorMessage = "Name must be longer than 5 chars.")]
        [MaxLength(20, ErrorMessage = "Name must be shorter than 20 chars.")]
        public string? NewName { get; set; }
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [EmailAddress]
        public string? NewEmail { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? OldPassword { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [NotEqualTo(nameof(OldPassword))]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*#?&])[!-~]{4,}$", ErrorMessage = "Password must contain at least 1 special char, 1 number and 1 uppercase letter")]
        public string? NewPassword { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Compare(nameof(this.NewPassword), ErrorMessage = "Passwords do not match.")]
        public string? NewPassword2 { get; set; }
    }
}