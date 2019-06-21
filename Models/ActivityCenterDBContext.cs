using Microsoft.EntityFrameworkCore;
 
namespace ActivityCenter.Models
{
    public class ActivityCenterDBContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ActivityCenterDBContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}

        public DbSet<Activity> Activities {get;set;}

        public DbSet<UA> UserActivity {get;set;}

    }
}