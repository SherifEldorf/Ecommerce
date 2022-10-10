using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTutorial.Models;

namespace WebCoreTutorial.Data
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options) :base(options)
        {
            if (!Database.CanConnect())
                Database.EnsureCreated();
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppConfirm> AppConfirms { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<VideoFile> VideoFiles { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<BillingAddress> BillingAddresses { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
