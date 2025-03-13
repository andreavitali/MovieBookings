using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MovieBookings.Core;
using MovieBookings.Core.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace MovieBookings.IntegrationTests
{
    public class ShowsApiTesting : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public ShowsApiTesting(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAllShows_ShouldReturn_AListOfShows()
        {
            var response = await _httpClient.GetAsync("/api/shows");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<ShowResponse>>();

            Assert.IsType<List<ShowResponse>>(content);
            Assert.Equal(2, content.Count);
        }

        [Fact]
        public async Task GetShowById_IfExists_ShouldReturn_TheShow()
        {
            var id = 1;

            var response = await _httpClient.GetAsync($"/api/shows/{id}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<ShowResponse>();

            Assert.Equal(1, content.Id);
            Assert.Equal(4, content.Seats.Count);
        }

        [Fact]
        public async Task GetShowById_IfNotExists_ShouldReturn_NotFound()
        {
            var id = int.MaxValue;

            var response = await _httpClient.GetAsync($"/api/shows/{id}");

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
            var response = await _httpClient.GetAsync($"/api/shows/{input}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
