using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieBookings.Data;

public class BookedSeatConfiguration : IEntityTypeConfiguration<BookedSeat>
{
    public void Configure(EntityTypeBuilder<BookedSeat> builder)
    {
        builder.HasIndex(bs => new { bs.SeatId, bs.ShowId }).IsUnique();
    }
}