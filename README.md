# MovieBookings Exercise

## Features
- NET 8.0
- EntityFramework with SQLite provider

## Missing features or improvements
- TODO

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