using garage87.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace garage87.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Service> Services { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.VatNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(c => c.Registration)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .Property(p => p.Salary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .HasIndex(c => c.Name)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
