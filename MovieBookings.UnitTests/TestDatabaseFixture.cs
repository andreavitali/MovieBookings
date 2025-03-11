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
                DatabaseSeeder.SeedUnitTestData(context);
            }
        }

        public ApplicationDbContext CreateContext() => new ApplicationDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }
}
