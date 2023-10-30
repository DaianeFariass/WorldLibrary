using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorldLibrary.Web.Data.Entities;

namespace WorldLibrary.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet<PhysicalLibrary> PhysicalLibraries { get; set; }
        public DbSet<ReserveDetail> ReserveDetails { get; set; }
        public DbSet<ReserveDetailTemp> ReserveDetailsTemp { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();
           

            base.OnModelCreating(modelBuilder);
        }
    }
}
