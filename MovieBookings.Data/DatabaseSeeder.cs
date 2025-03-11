using Microsoft.EntityFrameworkCore;

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

            // Shows
            context.Set<Show>().AddRange([
                new Show { Id = 1, MovieId = 1, StartAt = DateTime.Today.AddHours(15), TotalSeat = 7 },
                new Show { Id = 2, MovieId = 1, StartAt = DateTime.Today.AddHours(18), TotalSeat = 7 },
                new Show { Id = 3, MovieId = 2, StartAt = DateTime.Today.AddHours(21), TotalSeat = 7 },
                new Show { Id = 4, MovieId = 2, StartAt = DateTime.Today.AddHours(23), TotalSeat = 7 }
            ]);

            // Seats
            context.Set<Seat>().AddRange([
                new Seat { Id = 1, Row = 'A', Number = 1, Price = 8.00 },
                new Seat { Id = 2, Row = 'A', Number = 2, Price = 12.00 },
                new Seat { Id = 3, Row = 'A', Number = 3, Price = 8.00 },
                new Seat { Id = 4, Row = 'B', Number = 1, Price = 7.00 },
                new Seat { Id = 5, Row = 'B', Number = 2, Price = 7.00 },
                new Seat { Id = 6, Row = 'B', Number = 3, Price = 7.00 },
                new Seat { Id = 7, Row = 'C', Number = 1, Price = 16.00 }
            ]);

            // Users
            context.Set<User>().Add(new User { Id = 1, Email = "andrea@email.com", Name = "Andrea" });

            context.SaveChanges();
        }
    }
}
