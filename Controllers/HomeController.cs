using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private ActivityCenterDBContext dbContext;
        
        // here we can "inject" our context service into the constructor
        public HomeController(ActivityCenterDBContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("processregister")]
        [HttpPost]
        public IActionResult ProcessRegister(User user)
        { 
            System.Console.WriteLine("In ProcessRegister - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine(user.Email + "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine(user.Password + "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine(HttpContext.Request.Form.ToList());
            
            // Check initial ModelState
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                // other code

                var input = user.Password;

                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasSpecial = new Regex(@"[!@#$%^&*(),.?"":{}|<>]+");
                var hasMinimum8Chars = new Regex(@".{8,}");

                var isValidated = hasNumber.IsMatch(input) && hasUpperChar.IsMatch(input) && hasMinimum8Chars.IsMatch(input) && hasSpecial.IsMatch(input);
                Console.WriteLine($"password regex check - {hasNumber.IsMatch(input)} - {hasUpperChar.IsMatch(input)} - {hasMinimum8Chars.IsMatch(input)} - {isValidated}");

                if(!isValidated)
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Password", "Password is not strong");
                    
                    // You may consider returning to the View at this point
                    return View("Index");
                    //RedirectToAction("Index");
                }

                // Initializing a PasswordHasher object, providing our User class as its
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);

                // We can take the User object created from a form submission
                // And pass this object to the .Add() method
                // user.CreatedAt = DateTime.Now;
                // user.UpdatedAt = user.CreatedAt;
                dbContext.Add(user);
                // OR dbContext.Users.Add(newUser);
                dbContext.SaveChanges();

                int? userid = HttpContext.Session.GetInt32("UserId");

                if (userid == null)
                    HttpContext.Session.SetInt32("UserId", user.UserId);

                ViewBag.UserId = (int) user.UserId;
                ViewBag.User = user;

                return RedirectToAction("DashBoard");
            }           
            else
            {
                return View("Index");
            }
        }

        [Route("processlogin")]
        [HttpPost]
        public IActionResult ProcessLogin(LoginViewUser user)
        {
            System.Console.WriteLine("In ProcessLogin - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine(user.LoginUser.LoginUserEmail + "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine(user.LoginUser.LoginUserPassword + "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            // foreach(string k in HttpContext.Request.Form.Keys)
            // {
            //     System.Console.WriteLine(k);
            //     System.Console.WriteLine(HttpContext.Request.Form[k].ToString());
            // }

            var userInDb = (User) null;

            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginUser.LoginUserEmail);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginUser.Email", "Invalid Email/Password");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(user.LoginUser, userInDb.Password, user.LoginUser.LoginUserPassword);

                System.Console.WriteLine(userInDb.Password);
                System.Console.WriteLine(user.LoginUser.LoginUserPassword);
                System.Console.WriteLine(result);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("LoginUser.Email", "Invalid Email/Password");
                    return View("Index");
                }
            }
            else 
            {
                return View("Index");
            }

            int? userid = HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);

            System.Console.WriteLine($"Inside ProcessLogin just berfore return {userInDb.UserId}");

            ViewBag.UserId = (int) userInDb.UserId;
            ViewBag.User = userInDb;

            //return RedirectToAction("Account", new {userid = userInDb.UserId});
            //return Redirect($"/account/{userInDb.UserId}");
            return RedirectToAction("DashBoard");
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        [Route("dashboard")]
        [HttpGet]
        public IActionResult DashBoard()
        {
            System.Console.WriteLine("In DashBoard - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            User userInDb = dbContext.Users.FirstOrDefault(u => u.UserId == userid);
            
            DateTime testdate =  DateTime.Now;
            List<ActivityCenter.Models.Activity> AllActivities = dbContext.Activities
                .Where(a => a.ActivityDate.Date >= testdate.Date && a.ActivityTime.TimeOfDay >=  testdate.TimeOfDay )
                .Include(a => a.Creator) ///?????
                .Include(ua => ua.UserList)
                .ThenInclude(u => u.User) ///??????
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            ViewBag.UserId = (int) userid;
            ViewBag.User = userInDb;

            System.Console.WriteLine($"In DashBoard - {userid.GetValueOrDefault()}");

            return View(AllActivities);
            //return View();
        }


        [Route("newactivity")]
        [HttpGet]
        public IActionResult NewActivity()
        {
            System.Console.WriteLine("In NewActivity - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            ViewBag.UserId = (int) userid;

            return View();
        }

        [Route("createactivity")]
        [HttpPost]
        public IActionResult CreateActivity(ActivityCenter.Models.Activity activity)
        {
            foreach(string k in HttpContext.Request.Form.Keys)
            {
                System.Console.WriteLine(k);
                System.Console.WriteLine(HttpContext.Request.Form[k].ToString());
            }
            System.Console.WriteLine("In NewWedding - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"{activity.Title} - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"{activity.Description} - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"{activity.ActivityDate}- @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"{activity.ActivityTime} - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"{activity.Duration} - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"{activity.DurationUnit} - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            System.Console.WriteLine($"End Time - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(activity.ActivityDate.Date <= DateTime.Now.Date && activity.ActivityTime.TimeOfDay <= DateTime.Now.TimeOfDay)
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("ActivityDate", "Date has to be in the future!");
                    
                    ViewBag.UserId = (int) userid;

                    // You may consider returning to the View at this point
                    return View("NewActivity");
                }

                dbContext.Activities.Add(activity);
                // OR dbContext.Users.Add(newUser);
                dbContext.SaveChanges();

                ViewBag.UserId = (int) userid;

                //return RedirectToAction("DashBoard");
                return Redirect($"activitydetails/{activity.ActivityId}");
            }

            ViewBag.UserId = (int) userid;
            
            return View("NewActivity");
        }

        [Route("activitydetails/{activityid}")]
        [HttpGet]
        public IActionResult ActivityDetails(int activityid)
        {
            System.Console.WriteLine($"In ActivityDetails - {activityid} -@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            ActivityCenter.Models.Activity va = dbContext.Activities
                .Include(u => u.Creator)
                .Include(ua => ua.UserList)
                .ThenInclude(u => u.User)
                .FirstOrDefault(a => a.ActivityId == activityid);

            // Wedding vw = dbContext.Weddings
            //     //.Include(u => u.UserId)
            //     .Include(r => r.RSVPs)
            //     .ThenInclude(u => u.User)
            //     .FirstOrDefault(w => w.WeddingId == weddingid);

            ViewBag.UserId = (int) userid;

            return View(va);
        }

        [Route("deleteactivity/{activityid}")]
        [HttpGet]
        public IActionResult DeleteActivity(int activityid)
        {
            System.Console.WriteLine($"In DeleteActivity - {activityid} -@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            ActivityCenter.Models.Activity dactivity = dbContext.Activities.FirstOrDefault(a => a.ActivityId == activityid);
            dbContext.Activities.Remove(dactivity);
            dbContext.SaveChanges();

            ViewBag.UserId = (int) userid;

            return RedirectToAction("DashBoard");
        }

        [Route("joinactivity/{activityid}")]
        [HttpGet]
        public IActionResult JoinActivity(int activityid)
        {
            System.Console.WriteLine($"In JoinActivity - {activityid} -@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            UA ua = new UA();

            ua.UserId = (int) userid;
            ua.ActivityId = activityid;

            dbContext.UserActivity.Add(ua);
            dbContext.SaveChanges();

            ViewBag.UserId = (int) userid;

            return RedirectToAction("DashBoard");
        }

        [Route("leaveactivity/{activityid}")]
        [HttpGet]
        public IActionResult LeaveActivity(int activityid)
        {
            System.Console.WriteLine($"In LeaveActivity - {activityid} -@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            int? userid= HttpContext.Session.GetInt32("UserId");

            if (userid == null)
                return RedirectToAction("Index");

            UA ua = dbContext.UserActivity.FirstOrDefault(x => x.ActivityId == activityid && x.UserId == (int) userid);

            dbContext.UserActivity.Remove(ua);
            dbContext.SaveChanges();

            ViewBag.UserId = (int) userid;

            return RedirectToAction("DashBoard");
        }

    }
}
