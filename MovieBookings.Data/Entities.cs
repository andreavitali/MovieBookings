namespace MovieBookings.Data;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }

    public ICollection<Show> Shows { get; set; } = new List<Show>();
}

public class Show
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public DateTime StartAt { get; set; }
    public int TotalSeat { get; set; }

    public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
}

public class Seat
{
    public int Id { get; set; }
    public char Row { get; set; }
    public int Number { get; set; }
    public double Price { get; set; }

    public override string ToString() => $"{Row}-{Number}";
}

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public double TotalPrice { get; set; }

    public ICollection<BookedSeat> BookedSeats { get; set; } = new List<BookedSeat>();
}

public class BookedSeat
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public Seat Seat { get; set; }
    public int ShowId { get; set; }
    public Show Show { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}