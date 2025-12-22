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

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename default AspNet* tables to remove AspNet prefix
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
            });

            builder.Entity<IdentityRole<int>>(b =>
            {
                b.ToTable("Roles");
            });

            builder.Entity<IdentityUserRole<int>>(b =>
            {
                b.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<int>>(b =>
            {
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<int>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserToken<int>>(b =>
            {
                b.ToTable("UserTokens");
            });

            builder.Entity<RefreshToken>(b =>
            {
                b.ToTable("RefreshTokens");
                b.HasKey(x => x.Id);
                b.Property(x => x.Token).IsRequired().HasMaxLength(450);
                b.Property(x => x.IsRevoked).HasDefaultValue(false);
                b.HasIndex(x => x.UserId);
            });
        }
    }
}
