using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using System.Net;
using System.Net.Http.Json;

namespace MovieBookings.IntegrationTests
{
    public class ShowsApiTesting : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ShowsApiTesting(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAllShows_ShouldReturn_AListOfShows()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/shows");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<ShowResponse>>();

            Assert.IsType<List<ShowResponse>>(content);
            Assert.Equal(2, content.Count);
        }

        [Fact]
        public async Task GetShowById_IfExists_ShouldReturn_TheShow()
        {
            var client = _factory.CreateClient();
            var id = 1;

            // Act
            var response = await client.GetAsync($"/api/shows/{id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<ShowResponse>();

            Assert.Equal(1, content.Id);
            Assert.Equal(4, content.Seats.Count);
        }

        [Fact]
        public async Task GetShowById_IfNotExists_ShouldReturn_NotFound()
        {
            var client = _factory.CreateClient();
            var id = int.MaxValue;

            var response = await client.GetAsync($"/api/shows/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(content);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        [InlineData("-1")]
        [InlineData("fake")]
        public async Task GetShowById_IfIdIsNotInt_ShouldReturn_NotFound(string input)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/api/shows/{input}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
