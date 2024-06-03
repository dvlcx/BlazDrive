using System.ComponentModel.DataAnnotations;

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