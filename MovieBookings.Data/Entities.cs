using System.ComponentModel;
using System.Text.Json.Serialization;

namespace MovieBookings.Data;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    [Description("Duration in minutes")]
    public int Duration { get; set; }

    //public ICollection<Show> Shows { get; set; } = new List<Show>();
}

public class Show
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public DateTime StartAt { get; set; }
    public int TotalSeat { get; set; }

    public ICollection<ShowSeat> Seats { get; set; } = new List<ShowSeat>();
}

[JsonConverter(typeof(JsonStringEnumConverter<ShowSeatStatus>))]
public enum ShowSeatStatus
{
    Available = 0,
    Booked = 1
}

public class ShowSeat
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public Show Show { get; set;} = null!;
    public string SeatNumber { get; set; }
    public double Price { get; set; }
    public int? BookingId { get; set; }
    public ShowSeatStatus Status { get; set; }
}

// Not mapped to the database, just a tool to represent seat data for seeding
public class SeatData
{
    public char Row { get; set; }
    public int Number { get; set; }
    public double Price { get; set; }
    public string Description => $"{Row}-{Number}";
}

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public double TotalPrice { get; set; }

    public ICollection<ShowSeat> BookedSeats { get; set; } = new List<ShowSeat>();
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}