using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class LoginViewUser
    {
        public User User {get;set;}

        public LoginUser LoginUser {get;set;}
    }
}    