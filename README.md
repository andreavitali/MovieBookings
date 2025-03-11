# MovieBookings Exercise

## Features
- NET 8.0
- EntityFramework with SQLite provider

## Missing features or improvements
- Authentication via JWT
- Add an admin role able to see and edit the bookings of all users
- Improve database models to handle multiple theathres each one with multiple screens
- Improve database models to keep track of available/booked seats without the need of runtime calculation
- Use Result pattern instead of throwing exceptions for domain errors

## Usage

First migration:
dotnet ef migrations add InitialCreate --project .\MovieBookings.Data\ --startup-project .\MovieBookings.API\

Install dotnet ef CLI:
```
dotnet tool install --global dotnet-ef
```

The database file is already in the repository with seeded data.
If you want to create a new database file delete the MovieBookings.db and, from the project root folder, run:
```
dotnet ef database update --startup-project .\MovieBookings.API\
```

Start MovieBookings.API project.  

If the browser is showing error about HTTPS certificates you need to trust the self-signed ASP.NET Core HTTPS on the local machine:
```
dotnet dev-certs https --check --trust
```