using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MovieBookings.Data
{
    public static class DatabaseSeeder
    {
        public static List<SeatData> GenerateSeatData(int rows, int columns)
        {
            var seats = new List<SeatData>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    seats.Add(new SeatData { Row = (char)(i + 65), Number = j + 1, Price = 8.00 + (1*i) });
                }
            }
            return seats;
        }

        public static void SeedData(DbContext context)
        {
            // Movies
            context.Set<Movie>().AddRange([
                new Movie { Id = 1, Title = "Parasite", Duration = 132 },
                new Movie { Id = 2, Title = "1917", Duration = 119 }
            ]);

            // Seats
            List<SeatData> availableSeats = GenerateSeatData(3, 3);

            // Shows
            context.Set<Show>().AddRange([
                new Show { Id = 1, MovieId = 1, StartAt = DateTime.Today.AddHours(15), TotalSeat = availableSeats.Count },
                new Show { Id = 2, MovieId = 1, StartAt = DateTime.Today.AddHours(18), TotalSeat = availableSeats.Count },
                new Show { Id = 3, MovieId = 2, StartAt = DateTime.Today.AddHours(21), TotalSeat = availableSeats.Count },
            ]);

            // ShowSeats
            var showSeats = context.Set<Show>().Local
                    .SelectMany(show => availableSeats.Select(seat => (Show: show, Seat: seat)))
                    .Select(t => new ShowSeat { ShowId = t.Show.Id, SeatNumber = t.Seat.Description, Price = t.Seat.Price, Status = ShowSeatStatus.Available })
                    .ToList();

            context.Set<ShowSeat>().AddRange(showSeats);

            // Users
            var defaultUser = new User { Id = 1, Email = "andrea@email.com", Name = "Andrea" };
            defaultUser.Password = BCrypt.Net.BCrypt.HashPassword("password");
            context.Set<User>().Add(defaultUser);

            context.SaveChanges();
        }

        public static void SeedTestData(ApplicationDbContext context)
        {
            List<SeatData> availableSeats = GenerateSeatData(2, 2);

            context.Movies.Add(new Movie { Id = 1, Title = "Parasite", Duration = 132 });

            context.Shows.AddRange([
                new Show { Id = 1, MovieId = 1, StartAt = DateTime.Today.AddHours(15), TotalSeat = availableSeats.Count },
                new Show { Id = 2, MovieId = 1, StartAt = DateTime.Today.AddHours(18), TotalSeat = availableSeats.Count },
            ]);

            var showSeats = context.Shows.Local.ToList()
                .SelectMany(show => availableSeats.Select(seat => (Show: show, Seat: seat)))
                .Select(t => new ShowSeat { ShowId = t.Show.Id, SeatNumber = t.Seat.Description, Price = t.Seat.Price, Status = ShowSeatStatus.Available })
                .ToList();

            context.ShowSeats.AddRange(showSeats);

            context.Users.AddRange(GetTestUsers());

            context.SaveChanges();
        }

        public static IEnumerable<User> GetTestUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Email = "andrea.test@email.com", Name = "Andrea Test", Password = BCrypt.Net.BCrypt.HashPassword("password") },
                new User { Id = 2, Email = "pippo@email.com", Name = "Pippo", Password = BCrypt.Net.BCrypt.HashPassword("password") }
            };
        }
    }
}
