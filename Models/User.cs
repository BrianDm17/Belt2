using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [MinLength(2)]
        [MaxLength(45)]
        [Display(Name = "Name")] 
        public string Name {get;set;}

        [EmailAddress]
        [Required]
        public string Email {get;set;}

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // Will not be mapped to your users table!
        [NotMapped]
        [Display(Name = "Confirm PW")] 
        [Compare("Password", ErrorMessage="Passwords do not match")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        // Navigation property for related Message objects
        public List<Activity> CreatedActivities {get;set;}
        public List<UA> ActivityList {get;set;}
    }
}    