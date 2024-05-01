using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazDrive.Models
{
    public class UserLogIn
    {
        [Required(ErrorMessage = "Required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? Password { get; set; }
    }
}