using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MovieBookings.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(DbContext context)
        {
            // Movies
            context.Set<Movie>().AddRange([
                new Movie { Id = 1, Title = "Parasite", Duration = 132 },
                new Movie { Id = 2, Title = "1917", Duration = 119 }
            ]);

            // Seats
            List<Seat> availableSeats = [
                new Seat { Id = 1, Row = 'A', Number = 1, Price = 8.00 },
                new Seat { Id = 2, Row = 'A', Number = 2, Price = 12.00 },
                new Seat { Id = 3, Row = 'A', Number = 3, Price = 8.00 },
                new Seat { Id = 4, Row = 'B', Number = 1, Price = 7.00 },
                new Seat { Id = 5, Row = 'B', Number = 2, Price = 7.00 },
                new Seat { Id = 6, Row = 'B', Number = 3, Price = 7.00 }
            ];

            context.Set<Seat>().AddRange(availableSeats);

            // Shows
            context.Set<Show>().AddRange([
                new Show { Id = 1, MovieId = 1, StartAt = DateTime.Today.AddHours(15), TotalSeat = availableSeats.Count },
                new Show { Id = 2, MovieId = 1, StartAt = DateTime.Today.AddHours(18), TotalSeat = availableSeats.Count },
                new Show { Id = 3, MovieId = 2, StartAt = DateTime.Today.AddHours(21), TotalSeat = availableSeats.Count },
            ]);

            // ShowSeats
            var showSeats = context.Set<Show>().Local
                    .SelectMany(show => availableSeats.Select(seat => (Show: show, Seat: seat)))
                    .Select(t => new ShowSeat { ShowId = t.Show.Id, SeatId = t.Seat.Id, Status = ShowSeatStatus.Available })
                    .ToList();

            context.Set<ShowSeat>().AddRange(showSeats);

            // Users
            context.Set<User>().Add(new User { Id = 1, Email = "andrea@email.com", Name = "Andrea" });

            context.SaveChanges();
        }

        public static void SeedUnitTestData(ApplicationDbContext context)
        {
            var availableSeats = new List<Seat>
                {
                    new() { Id = 1, Row = 'A', Number = 1, Price = 8.00 },
                    new() { Id = 2, Row = 'A', Number = 2, Price = 8.00 },
                    new() { Id = 3, Row = 'B', Number = 1, Price = 8.00 },
                    new() { Id = 4, Row = 'B', Number = 2, Price = 8.00 }
                };

            context.Set<Seat>().AddRange(availableSeats);

            context.Movies.Add(new Movie { Id = 1, Title = "Parasite", Duration = 132 });

            context.Shows.AddRange([
                new Show { Id = 1, MovieId = 1, StartAt = DateTime.Today.AddHours(15), TotalSeat = availableSeats.Count },
                new Show { Id = 2, MovieId = 1, StartAt = DateTime.Today.AddHours(18), TotalSeat = availableSeats.Count },
            ]);

            var showSeats = context.Shows.Local.ToList()
                .SelectMany(show => availableSeats.Select(seat => (Show: show, Seat: seat)))
                .Select(t => new ShowSeat { ShowId = t.Show.Id, SeatId = t.Seat.Id, Status = ShowSeatStatus.Available })
                .ToList();

            context.ShowSeats.AddRange(showSeats);

            context.Users.Add(new User { Id = 1, Email = "andrea.test@email.com", Name = "Andrea Test" });

            context.SaveChanges();
        }
    }
}
