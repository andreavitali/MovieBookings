using MovieBookings.Core;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MovieBookings.IntegrationTests;

public static class HttpClientExtensions
{
    public static HttpClient WithBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
