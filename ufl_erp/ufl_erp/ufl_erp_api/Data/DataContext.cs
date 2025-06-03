using Microsoft.EntityFrameworkCore;
using ufl_erp_api.Models;

namespace ufl_erp_api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Detail>()
                .HasOne(s => s.Sale)
                .WithMany(s => s.Details)
                .HasForeignKey(s => s.SaleId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Detail>()
                .HasOne(s => s.Product)
                .WithMany(s => s.Details)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Branch)
                .WithMany(s => s.Stocks)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(s => s.Stocks)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<Person>(p => p.UserId);


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
