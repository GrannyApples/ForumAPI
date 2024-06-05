using ForumAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ForumAPI.Data
{
        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Models.Post> Posts { get; set; }
        public DbSet<Models.Comment> Comments { get; set; }
        public DbSet<Models.ReportedItem> ReportedItems { get; set; }
        public DbSet<Models.Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
