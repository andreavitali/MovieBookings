using Microsoft.EntityFrameworkCore;

namespace MovieBookings.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<ShowSeat> ShowSeats { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // These should be moved into classes extending IEntityTypeConfiguration<TEntity>
        modelBuilder.Entity<ShowSeat>()
            .Property(ss => ss.Status)
            .HasConversion<int>();

        modelBuilder.Entity<ShowSeat>()
            .HasIndex(ss => new { ss.ShowId, ss.SeatNumber})
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasMany(b => b.BookedSeats)
            .WithOne()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
