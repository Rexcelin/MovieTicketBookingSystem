
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Existing database tables
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Screen> Screens => Set<Screen>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Show> Shows => Set<Show>();
    public DbSet<ShowSeat> ShowSeats => Set<ShowSeat>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Identity tables first
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(b => b.UserId);
        });

        // Movie configuration
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(m => m.MovieId);

            entity.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(m => m.Description)
                .HasMaxLength(500);

            entity.Property(m => m.ReleaseDate)
                .HasColumnType("date");

            entity.Property(m => m.Language)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(m => m.Genre)
                .IsRequired()
                .HasMaxLength(50);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Movies_DurationMinutes",
                    "[DurationMinutes] > 0"));
        });

        // Theatre configuration
        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(t => t.TheatreId);

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(200);
        });

        // Screen configuration
        modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(s => s.ScreenId);

            entity.Property(s => s.ScreenName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(s => s.Theatre)
                .WithMany(t => t.Screens)
                .HasForeignKey(s => s.TheatreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seat configuration
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(s => s.SeatId);

            entity.Property(s => s.RowName)
                .IsRequired()
                .HasMaxLength(5);

            entity.Property(s => s.SeatType)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasIndex(s => new
            {
                s.ScreenId,
                s.RowName,
                s.SeatNumber
            }).IsUnique();

            entity.HasOne(s => s.Screen)
                .WithMany(sc => sc.Seats)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Seats_SeatNumber",
                    "[SeatNumber] > 0"));
        });

        // Show configuration
        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(s => s.ShowId);

            entity.Property(s => s.TicketPrice)
                .HasPrecision(10, 2);

            entity.HasOne(s => s.Movie)
                .WithMany(m => m.Shows)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Screen)
                .WithMany(sc => sc.Shows)
                .HasForeignKey(s => s.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Shows_EndTime",
                    "[EndTime] > [StartTime]");

                t.HasCheckConstraint(
                    "CK_Shows_TicketPrice",
                    "[TicketPrice] >= 0");
            });
        });

        // ShowSeat configuration
        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.HasKey(ss => ss.ShowSeatId);

            entity.Property(ss => ss.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(ss => new
            {
                ss.ShowId,
                ss.SeatId
            }).IsUnique();

            entity.HasOne(ss => ss.Show)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Seat)
                .WithMany(s => s.ShowSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_ShowSeats_Status",
                    "[Status] IN ('Available', 'Booked')"));
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.BookingId);

            entity.Property(b => b.BookingReference)
                .IsRequired()
                .HasMaxLength(30);

            entity.HasIndex(b => b.BookingReference)
                .IsUnique();

            entity.Property(b => b.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(b => b.CustomerEmail)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(b => b.CustomerPhone)
                .IsRequired()
                .HasMaxLength(15);

            entity.Property(b => b.TotalAmount)
                .HasPrecision(10, 2);

            entity.Property(b => b.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(b => b.Show)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Bookings_TotalAmount",
                    "[TotalAmount] >= 0");

                t.HasCheckConstraint(
                    "CK_Bookings_Status",
                    "[Status] IN ('Confirmed', 'Cancelled')");
            });
        });

        // BookingSeat configuration
        modelBuilder.Entity<BookingSeat>(entity =>
        {
            entity.HasKey(bs => bs.BookingSeatId);

            entity.Property(bs => bs.Price)
                .HasPrecision(10, 2);

            entity.HasIndex(bs => new
            {
                bs.BookingId,
                bs.SeatId
            }).IsUnique();

            entity.HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_BookingSeats_Price",
                    "[Price] >= 0"));
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.RefreshTokenId);

            entity.Property(rt => rt.UserId)
                .IsRequired();

            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasIndex(rt => rt.Token)
                .IsUnique();

            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
