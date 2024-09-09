using garage87.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace garage87.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
