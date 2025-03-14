# MovieBookings Exercise

## Features
- NET 8.0
- MinimalAPI
- OpenAPI documentation with Swagger (`POST /api/bookings` and `POST /api/auth/` endpoints are well documented)
- EntityFramework with SQLite provider

## Database schema
This is a diagram of the database schema:

![Database schema](database_schema.png)

For the sake of simplicity there is only one theatre with an hardcoded seats configuration.

## Usage
The database file is already in the repository with some seeded data.  
The default user to use for login is:
```
{
  "email": "andrea@email.com",
  "password": "password"
}
```

Only the /api/bookings routes are protected with JWT authentication.

If you want to create a new database file delete the MovieBookings.db and, from the project root folder, run:
```
dotnet ef database update --startup-project .\MovieBookings.API\
```

If the browser is showing error about HTTPS certificates starting the application 
you need to trust the self-signed ASP.NET Core HTTPS on the local machine:
```
dotnet dev-certs https --check --trust
```

## Missing features or improvements
- TODO

### Microservices architecture