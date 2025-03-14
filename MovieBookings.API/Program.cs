using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieBookings.API.Endpoints;
using MovieBookings.API.Extensions;
using MovieBookings.Data;
using System.Text;

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

// Add JWT Authentication
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

// Add from extensions
builder.Services.AddDomainServices();
builder.Services.AddSwaggerGenWithAuth();

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

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Add MinimalAPI endpoints
app.MapAuthEndpoints();
app.MapShowsEndpoints();
app.MapBookingsEndpoints();

// Run!
app.Run();

public partial class Program { }
