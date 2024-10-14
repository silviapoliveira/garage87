using garage87.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace garage87.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<VehicleAssignment> VehicleAssignment { get; set; }

        public DbSet<Repair> Repair { get; set; }

        public DbSet<RepairDetail> RepairDetail { get; set; }

        public DbSet<Message> Message { get; set; }

        public DbSet<Notifications> Notifications { get; set; }

        public DbSet<Brand> Brand { get; set; }

        public DbSet<Model> Model { get; set; }

        public DbSet<Specialities> Specialities { get; set; }

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
                .HasIndex(c => c.VatNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .Property(p => p.Salary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
