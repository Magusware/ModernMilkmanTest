namespace CustomerServiceNS.Database
{
    using Microsoft.EntityFrameworkCore;

    public class CustomerServiceDbContext : DbContext
    {
        public CustomerServiceDbContext(DbContextOptions options)
            : base(options)
        { }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CustomerInfo");

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Addresses)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Title)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Customer>()
                .Property(e => e.Forename)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Customer>()
                .Property(e => e.Surname)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Customer>()
                .Property(e => e.EmailAddress)
                .HasMaxLength(75)
                .IsRequired();

            // Add a unique constraing on the email address
            // as this is the only thing which could not 
            // possibly be shared with another customer at 
            // any point in time. However, this constraint
            // will be ignored by the in-memory database
            modelBuilder.Entity<Customer>()
                .HasIndex(e => e.EmailAddress)
                .IsUnique();
       
            modelBuilder.Entity<Customer>()
                .Property(e => e.MobileNumber)
                .HasMaxLength(15)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(e => e.AddressLine1)
                .HasMaxLength(80)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(e => e.AddressLine2)
                .HasMaxLength(80);

            modelBuilder.Entity<Address>()
                .Property(e => e.Town)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(e => e.County)
                .HasMaxLength(50);

            modelBuilder.Entity<Address>()
                .Property(e => e.Postcode)
                .HasMaxLength(10)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(e => e.Country)
                .HasMaxLength(20)
                .HasDefaultValue("UK")
                .IsRequired();
        }
    }
}
