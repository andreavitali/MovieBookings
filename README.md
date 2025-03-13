# MovieBookings Exercise

## Features
- NET 8.0
- MinimalAPI
- EntityFramework with SQLite provider

## Database schema
This is a diagram of the database schema:

![Database schema](database_schema.png)

For the sake of simplicity there is only one theatre with an hardcoded seats configuration.

## Missing features or improvements
- TODO

## Usage
The database file is already in the repository with seeded data
If you want to create a new database file delete the MovieBookings.db and, from the project root folder, run:
```
dotnet ef database update --startup-project .\MovieBookings.API\
```

Start MovieBookings.API project.  

If the browser is showing error about HTTPS certificates you need to trust the self-signed ASP.NET Core HTTPS on the local machine:
```
dotnet dev-certs https --check --trust
```