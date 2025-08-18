using Microsoft.EntityFrameworkCore;
using RDOXMES.Models;
using RDOXMES.Data;

namespace RDOXMES.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<ViewInventory> ViewInventory { get; set; }

        public DbSet<ViewHistoric> ViewHistoric { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<Historic> Historic { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inventory configuration
            modelBuilder.Entity<Inventory>()
                .HasKey(i => i.PartNumberId);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.PartNumberId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Qty)
                .IsRequired();

            modelBuilder.Entity<Inventory>()
                .Property(i => i.UpdatedAt)
                .IsRequired();

            // Historic configuration
            modelBuilder.Entity<Historic>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<Historic>()
                .Property(h => h.PartNumberId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Historic>()
                .Property(h => h.Movement)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<Historic>()
                .Property(h => h.Qty)
                .IsRequired();

            modelBuilder.Entity<Historic>()
                .Property(h => h.DateTime)
                .IsRequired();

            modelBuilder.Entity<Historic>()
                .Property(h => h.UserNameId)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
