using ahis.template.identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ahis.template.identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename default AspNet* tables to remove AspNet prefix
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("IdentityUser");
            });

            builder.Entity<IdentityRole<int>>(b =>
            {
                b.ToTable("IdentityRole");
            });

            builder.Entity<IdentityUserRole<int>>(b =>
            {
                b.ToTable("IdentityUserRole");
            });

            builder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.ToTable("IdentityUserClaim");
            });

            builder.Entity<IdentityUserLogin<int>>(b =>
            {
                b.ToTable("IdentityUserLogin");
            });

            builder.Entity<IdentityRoleClaim<int>>(b =>
            {
                b.ToTable("IdentityRoleClaim");
            });

            builder.Entity<IdentityUserToken<int>>(b =>
            {
                b.ToTable("IdentityUserToken");
            });
        }
    }
}
