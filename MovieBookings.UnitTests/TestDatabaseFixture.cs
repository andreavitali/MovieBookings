using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MovieBookings.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookings.UnitTests
{
    public class TestDatabaseFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        public TestDatabaseFixture()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new ApplicationDbContext(_contextOptions);

            if (context.Database.EnsureCreated())
            {
                context.Movies.Add(new Movie { Id = 1, Title = "Parasite", Duration = 132 });

                context.Shows.AddRange([
                    new Show { Id = 1, MovieId = 1, StartAt = DateTime.Today.AddHours(15), TotalSeat = 7 },
                    new Show { Id = 2, MovieId = 1, StartAt = DateTime.Today.AddHours(18), TotalSeat = 7 },
                ]);

                context.Seats.Add(new Seat { Id = 1, Row = 'A', Number = 1, Price = 8.00 });

                context.Users.Add(new User { Id = 1, Email = "andrea.test@email.com", Name = "Andrea Test" });

                context.SaveChanges();
            }
        }

        public ApplicationDbContext CreateContext() => new ApplicationDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }
}
