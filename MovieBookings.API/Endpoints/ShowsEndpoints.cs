namespace MovieBookings.API.Endpoints;

public static class ShowsEndpoints
{
    public static void MapShowsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/shows");
    }
}
