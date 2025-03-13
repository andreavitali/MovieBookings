using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Data;
using System.Net;
using System.Net.Http.Json;

namespace MovieBookings.IntegrationTests
{
    public class BookingsApiTesting : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public BookingsApiTesting(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateBooking_IfSeatsAvailable_ShouldReturnCreatedBooking()
        {
            var userId = 1;
            var showId = 1;
            var showContent = await _httpClient.GetAsync($"/api/shows/{showId}");
            var show = await showContent.Content.ReadFromJsonAsync<ShowResponse>();
            var seatShow = show.Seats[0];
            var request = new List<BookingRequest> { new BookingRequest(show.Id, seatShow.Id) };

            var response = await _httpClient.PostAsJsonAsync($"/api/bookings?userId={userId}", request);

            response.EnsureSuccessStatusCode();
            var booking = await response.Content.ReadFromJsonAsync<BookingResponse>();

            Assert.NotNull(booking);
            Assert.Equal(userId, booking.UserId);
            Assert.Equal(seatShow.Id, booking.BookedSeats[0].Id);
        }

        [Fact(Skip = "Model validation required")]
        public async Task CreateBooking_IfSeatIdIsNull_ShouldReturn400()
        {  
            var showId = 1;
            var userId = 1;
            var request = new List<BookingRequest> { new BookingRequest(showId, null) };

            var response = await _httpClient.PostAsJsonAsync($"/api/bookings?userId={userId}", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateBooking_IfSeatsNotAvailable_ShouldReturn409()
        {   
            var userId = 1;
            var showId = 2;
            var showContent = await _httpClient.GetAsync($"/api/shows/{showId}");
            var show = await showContent.Content.ReadFromJsonAsync<ShowResponse>();
            var seatShow = show.Seats[0];
            var request = new List<BookingRequest> { new BookingRequest(show.Id, seatShow.Id) };

            var response = await _httpClient.PostAsJsonAsync($"/api/bookings?userId={userId}", request);
            response.EnsureSuccessStatusCode();

            response = await _httpClient.PostAsJsonAsync($"/api/bookings?userId={userId}", request);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var bookingError = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal("One ore more seats are already booked", bookingError.Detail);
        }

        [Fact(Skip = "JWT")]
        public async Task GetAllBookings_ForUnauthenticatedUser_ShouldReturn_Forbidden()
        {
            var response = await _httpClient.GetAsync($"/api/bookings");
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
