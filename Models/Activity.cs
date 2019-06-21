using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId {get;set;}

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "Title")] 
        public string Title {get;set;}

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        [Display(Name = "Description")] 
        public string Description {get;set;}

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime ActivityDate {get;set;}

        [Required]
        [Display(Name = "Time")]
        [DataType(DataType.Time)] 
        public DateTime ActivityTime {get;set;}

        [Required]
        [Display(Name = "Duration")] 
        public int Duration {get;set;}

        [Required]
        public string DurationUnit {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;


        // Navigation property for related Message objects
        public List<UA> UserList {get;set;}

        public int UserId {get;set;}
        // Navigation property for related User object
        public User Creator {get;set;}
    }
}    