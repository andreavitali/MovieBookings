using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieBookings.Core.Models;
using MovieBookings.Data;
using System.Data.Common;
using System.Net.Http.Json;

namespace MovieBookings.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        public async Task InitializeAsync()
        {
            using var scope = this.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            await Task.CompletedTask;
        }

        public async Task<string> GetTokenForUser(HttpClient client, User user)
        {
            var tokenResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(user.Email, "password"));
            var tokenResult = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
            return tokenResult.Token;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                services.Remove(dbConnectionDescriptor);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("Filename=:memory:");
                    connection.Open();
                    return connection;
                });

                services.AddDbContext<ApplicationDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options
                        .UseSqlite(connection)
                        .UseSeeding((context, _) => DatabaseSeeder.SeedTestData(context as ApplicationDbContext));
                });
            });

            builder.UseEnvironment("Development");
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
