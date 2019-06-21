using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class UA
    {
        [Key]
        public int UAId {get;set;}

        [Required]
        public int UserId {get;set;}

        public User User {get;set;}

        [Required]
        public int ActivityId {get;set;}

        public Activity Activity {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}    