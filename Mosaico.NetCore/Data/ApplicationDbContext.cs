using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mosaico.NetCore.Models;

namespace Mosaico.NetCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<MosaicoEmail> MosaicoEmails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var builder = modelBuilder.Entity<MosaicoEmail>();
            builder.ToTable("MosaicoEmails");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(128).IsUnicode(true);
            builder.Property(m => m.Template).IsRequired();
            builder.Property(m => m.Metadata).IsRequired().IsUnicode(true);
            builder.Property(m => m.Content).IsRequired().IsUnicode(true);
            builder.Property(m => m.Html).IsRequired().IsUnicode(true);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}