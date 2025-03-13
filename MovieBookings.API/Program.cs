using Microsoft.EntityFrameworkCore;
using MovieBookings.API.Endpoints;
using MovieBookings.Core.Interfaces;
using MovieBookings.Core.Services;
using MovieBookings.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options
        .UseSqlite(builder.Configuration.GetConnectionString("Default"))
        .UseSeeding((context, _) => DatabaseSeeder.SeedData(context))
);

// Add my services
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();
app.UseExceptionHandler();

//app.UseAuthorization();
//app.UseAuthentication();

// Add MinimalAPI endpoints
app.MapShowsEndpoints();
app.MapBookingsEndpoints();

// Run!
app.Run();

public partial class Program { }
