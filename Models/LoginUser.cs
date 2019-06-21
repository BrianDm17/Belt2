using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class LoginUser
    {
        [EmailAddress]
        [Required]
        [Display(Name = "Email")] 
        public string LoginUserEmail {get;set;}

        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Password")] 
        public string LoginUserPassword {get;set;}
    }
}    