using ASP.NETCoreLoyaltyRewardsSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ASP.NETCoreLoyaltyRewardsSystem.Models;

namespace ASP.NETCoreLoyaltyRewardsSystem.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Transaction> Transactions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.DateOfTransaction)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.AmountBeforeDiscount)
               .IsRequired()
               .HasMaxLength(50);
            entity.Property(e => e.PointsApplied)
               .HasMaxLength(50);
        });

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
        builder.Property(u => u.DateOfBirth).HasMaxLength(255);
        builder.Property(u => u.AvailablePoints).HasDefaultValue(500); 

    }
}





