using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlazDrive.Models.InputModels
{
    public class UserEditInput
    {
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? NewName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? NewEmail { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? OldPassword { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true)]
        [Unlike("OldPassword", ErrorMessage = "It must differ from old password.")]
        public string? NewPassword { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true)]
        public string? NewPassword2 { get; set; }
    }
}