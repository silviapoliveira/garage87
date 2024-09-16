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

            //// Habilitar a regra de apagar em cascata(Cascade Delete Rule)
            //var cascadeFKs = modelBuilder.Model
            //    .GetEntityTypes()
            //    .SelectMany(t => t.GetForeignKeys())
            //    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            //foreach (var fk in cascadeFKs)
            //{
            //    fk.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            base.OnModelCreating(modelBuilder);
        }
    }
}
