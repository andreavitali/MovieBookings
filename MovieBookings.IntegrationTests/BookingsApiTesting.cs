using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieBookings.Core;
using MovieBookings.Data;
using System.Net;
using System.Net.Http.Json;

namespace MovieBookings.IntegrationTests
{
    public class BookingsApiTesting : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public BookingsApiTesting(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
            this._factory = factory;
        }

        [Fact]
        public async Task CreateBooking_IfSeatsAvailable_ShouldReturnCreatedBooking()
        {
            var showId = 1;
            var showContent = await _httpClient.GetAsync($"/api/shows/{showId}");
            var show = await showContent.Content.ReadFromJsonAsync<ShowResponse>();
            var seatShow = show.Seats[0];
            var request = new List<BookingRequest> { new BookingRequest(null, seatShow.Id) };

            var user = DatabaseSeeder.GetTestUsers().First();
            var token = await _factory.GetTokenForUser(_httpClient, user);

            var response = await _httpClient.WithBearerToken(token).PostAsJsonAsync($"/api/bookings", request);

            response.EnsureSuccessStatusCode();
            var booking = await response.Content.ReadFromJsonAsync<BookingResponse>();

            Assert.NotNull(booking);
            Assert.Equal(user.Id, booking.UserId);
            Assert.Equal(seatShow.Id, booking.BookedSeats[0].Id);
        }

        [Fact]
        public async Task CreateBooking_IfSeatsNotAvailable_ShouldReturn409()
        {   
            var showId = 2;
            var showContent = await _httpClient.GetAsync($"/api/shows/{showId}");
            var show = await showContent.Content.ReadFromJsonAsync<ShowResponse>();
            var seatShow = show.Seats[0];
            var request = new List<BookingRequest> { new BookingRequest(null, seatShow.Id) };

            var user = DatabaseSeeder.GetTestUsers().First();
            var token = await _factory.GetTokenForUser(_httpClient, user);

            var response = await _httpClient.WithBearerToken(token).PostAsJsonAsync($"/api/bookings", request);
            response.EnsureSuccessStatusCode();

            response = await _httpClient.WithBearerToken(token).PostAsJsonAsync($"/api/bookings", request);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var bookingError = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal("One ore more seats are already booked", bookingError.Detail);
        }

        [Fact]
        public async Task GetAllBookings_ForUnauthenticatedUser_ShouldReturn_Unauthorized()
        {
            var response = await _httpClient.GetAsync($"/api/bookings");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateBooking_ForUnauthenticatedUser_ShouldReturn_Unauthorized()
        {
            var request = new List<BookingRequest> { new BookingRequest(1, null) };
            var response = await _httpClient.PostAsJsonAsync($"/api/bookings", request);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBooking_OfAnotherUser_ShouldReturn_Forbidden()
        {
            var showId = 1;
            var request = new List<BookingRequest> { new BookingRequest(showId, null) };

            var user1 = DatabaseSeeder.GetTestUsers().First();
            var token1 = await _factory.GetTokenForUser(_httpClient, user1);

            var response = await _httpClient.WithBearerToken(token1).PostAsJsonAsync($"/api/bookings", request);
            response.EnsureSuccessStatusCode();

            var booking = await response.Content.ReadFromJsonAsync<BookingResponse>();

            var user2 = DatabaseSeeder.GetTestUsers().Skip(1).Single();
            var token2 = await _factory.GetTokenForUser(_httpClient, user2);

            response = await _httpClient.WithBearerToken(token2).DeleteAsync($"/api/bookings/{booking.Id}");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
