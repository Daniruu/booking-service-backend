using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data
{
    public class BookingServiceDbContext : DbContext
    {
        public BookingServiceDbContext(DbContextOptions<BookingServiceDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceGroup> ServiceGroups { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BusinessCategory> BusinessCategories { get; set; }
        public DbSet<BusinessImage> BusinessImages { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<FavoriteBusiness> FavoriteBusinesses { get; set; }
        public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
        public DbSet<DaySchedule> DaySchedules { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account 
            modelBuilder.Entity<Account>()
                .ToTable("Accounts");

            // User
            modelBuilder.Entity<User>()
                .ToTable("Users");

            // Business
            modelBuilder.Entity<Business>()
                .ToTable("Businesses");

            // User -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            // User -> Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            // FavoriteBusiness
            modelBuilder.Entity<FavoriteBusiness>()
                .HasKey(fb => new { fb.UserId, fb.BusinessId });
            modelBuilder.Entity<FavoriteBusiness>()
                .HasOne(fb => fb.User)
                .WithMany(u => u.FavoriteBusinesses)
                .HasForeignKey(fb => fb.UserId);
            modelBuilder.Entity<FavoriteBusiness>()
                .HasOne(fb => fb.Business)
                .WithMany()
                .HasForeignKey(fb => fb.BusinessId);

            // Business -> Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Business)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BusinessId);

            // Business -> ServiceGroup
            modelBuilder.Entity<ServiceGroup>()
                .HasOne(sg => sg.Business)
                .WithMany(b => b.ServiceGroups)
                .HasForeignKey(sg => sg.BusinessId);

            // Business -> Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Business)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BusinessId);

            // Business -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Business)
                .WithMany(b => b.Bookings)
                .HasForeignKey(b => b.BusinessId);

            // Business -> RegistrationData
            modelBuilder.Entity<Business>()
                .OwnsOne(b => b.RegistrationData);

            // Business -> Address
            modelBuilder.Entity<Business>()
                .OwnsOne(b => b.Address);

            // Business -> BusinessCategory
            modelBuilder.Entity<Business>()
                .OwnsOne(b => b.Settings);

            // ServiceGroup -> Service
            modelBuilder.Entity<Service>()
                .HasOne(s => s.ServiceGroup)
                .WithMany(sg => sg.Services)
                .HasForeignKey(s => s.ServiceGroupId);

            // Service -> Booking
            modelBuilder.Entity<Booking>()
                 .HasOne(b => b.Service)
                 .WithMany()
                 .HasForeignKey(b => b.ServiceId);

            // Employee -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany()
                .HasForeignKey(b => b.EmployeeId);

            // Review -> ReviewImage
            modelBuilder.Entity<ReviewImage>()
                .HasOne(ri => ri.Review)
                .WithMany(r => r.Images)
                .HasForeignKey(ri => ri.ReviewId);

            // Business -> BusinessImage
            modelBuilder.Entity<BusinessImage>()
                .HasOne(bi => bi.Business)
                .WithMany(b => b.Images)
                .HasForeignKey(bi => bi.BusinessId);

            // Business -> Schedule
            modelBuilder.Entity<DaySchedule>()
                .HasOne(d => d.Business)
                .WithMany(b => b.Schedule)
                .HasForeignKey(d => d.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee -> Schedule
            modelBuilder.Entity<DaySchedule>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Schedule)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            //DaySchedule -> TimeSlots
            modelBuilder.Entity<TimeSlot>()
                .HasOne(ts => ts.DaySchedule)
                .WithMany(d => d.TimeSlots)
                .HasForeignKey(ts => ts.DayScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Email for users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_User_Email");

            // Unique Email for businesses
            modelBuilder.Entity<Business>()
                .HasIndex(b => b.Email)
                .IsUnique()
                .HasDatabaseName("IX_Business_Email");
        }
    }
}
