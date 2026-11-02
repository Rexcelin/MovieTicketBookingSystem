# Movie Ticket Booking System

A RESTful Web API built using **C#, ASP.NET Core Web API (.NET 10), Entity Framework Core 10, and SQL Server**. This application manages movies, theatres, screens, seats, show schedules, seat availability, and movie ticket bookings.

## Features

- **Movie Management:** Create, view, update, delete, and search movies.
- **Theatre Management:** Manage theatre details.
- **Screen Management:** Manage screens belonging to theatres.
- **Seat Management:** Create and manage physical seats, including bulk seat creation.
- **Show Management:** Schedule movie shows and validate overlapping schedules.
- **Show Seat Availability:** Automatically generate seat availability records whenever a show is created.
- **Booking Management:** Book multiple available seats and calculate the total amount automatically.
- **Booking Cancellation:** Cancel bookings and release the associated seats while preserving booking history.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Programming language |
| .NET 10 | Application framework |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core 10 | ORM and database operations |
| SQL Server Express | Database |
| Scalar / OpenAPI | API documentation |
| Postman | API testing |
| Git and GitHub | Version control |

## Architecture

The project follows a layered architecture:

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
ApplicationDbContext
    ↓
SQL Server Database
```

- **Controllers:** Handle HTTP requests and responses.
- **Services:** Implement business logic and validations.
- **Repositories:** Handle database operations.
- **Models:** Define database entities.
- **DTOs:** Define API request and response structures.
- **Data:** Contains the Entity Framework Core DbContext.

## Project Structure

```text
MovieTicketBookingSystem/
│
├── Controllers/
│   ├── MoviesController.cs
│   ├── TheatresController.cs
│   ├── ScreensController.cs
│   ├── SeatsController.cs
│   ├── ShowsController.cs
│   └── BookingsController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── DTOs/
│   ├── Movies/
│   ├── Theatres/
│   ├── Screens/
│   ├── Seats/
│   ├── Shows/
│   └── Bookings/
│
├── Models/
│   ├── Movie.cs
│   ├── Theatre.cs
│   ├── Screen.cs
│   ├── Seat.cs
│   ├── Show.cs
│   ├── ShowSeat.cs
│   ├── Booking.cs
│   └── BookingSeat.cs
│
├── Repositories/
│   ├── Interfaces/
│   ├── MovieRepository.cs
│   ├── TheatreRepository.cs
│   ├── ScreenRepository.cs
│   ├── SeatRepository.cs
│   ├── ShowRepository.cs
│   ├── ShowSeatRepository.cs
│   └── BookingRepository.cs
│
├── Services/
│   ├── Interfaces/
│   ├── MovieService.cs
│   ├── TheatreService.cs
│   ├── ScreenService.cs
│   ├── SeatService.cs
│   ├── ShowService.cs
│   └── BookingService.cs
│
├── Migrations/
├── appsettings.json
├── Program.cs
├── .gitignore
└── README.md
```

## Database Design

The application contains eight main tables:

| Table | Description |
|---|---|
| Movies | Stores movie information |
| Theatres | Stores theatre information |
| Screens | Stores screens belonging to theatres |
| Seats | Stores physical seats belonging to screens |
| Shows | Stores movie show schedules |
| ShowSeats | Tracks seat availability for individual shows |
| Bookings | Stores customer booking information |
| BookingSeats | Stores seats included in each booking |

### Booking Workflow

1. Create a movie and theatre.
2. Create a screen and its physical seats.
3. Schedule a movie show.
4. The system automatically creates ShowSeats for that show.
5. Retrieve available seats.
6. Select available seats and create a booking.
7. The system calculates the total ticket price on the server.
8. The booking, booking seats, and seat status changes are saved in a database transaction.
9. When a booking is cancelled, its seats become available again while the booking history is preserved.

## Installation and Setup

### Prerequisites

- Visual Studio 2026
- .NET 10 SDK
- SQL Server Express
- SQL Server Management Studio (SSMS)
- Postman

### 1. Clone the Repository

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
```

### 2. Open the Project

Open `MovieTicketBookingSystem.sln` in Visual Studio.

### 3. Configure the Database

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Adjust the SQL Server instance name if necessary.

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Apply Database Migrations

```bash
dotnet ef database update
```

### 6. Build the Application

```bash
dotnet build
```

### 7. Run the Application

```bash
dotnet run
```

Open the Scalar API documentation URL displayed by the application.

## API Endpoints

### Movies

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/movies` | Create movie |
| GET | `/api/movies` | Get all movies |
| GET | `/api/movies/{id}` | Get movie by ID |
| PUT | `/api/movies/{id}` | Update movie |
| DELETE | `/api/movies/{id}` | Delete movie |
| GET | `/api/movies/search?title=leo` | Search movies |

### Theatres

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/theatres` | Create theatre |
| GET | `/api/theatres` | Get all theatres |
| GET | `/api/theatres/{id}` | Get theatre by ID |
| PUT | `/api/theatres/{id}` | Update theatre |
| DELETE | `/api/theatres/{id}` | Delete theatre |

### Screens

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/screens` | Create screen |
| GET | `/api/screens` | Get all screens |
| GET | `/api/screens/{id}` | Get screen by ID |
| GET | `/api/screens/theatre/{theatreId}` | Get screens by theatre |
| PUT | `/api/screens/{id}` | Update screen |
| DELETE | `/api/screens/{id}` | Delete screen |

### Seats

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/seats` | Create seat |
| POST | `/api/seats/bulk` | Create multiple seats |
| GET | `/api/seats` | Get all seats |
| GET | `/api/seats/{id}` | Get seat by ID |
| GET | `/api/seats/screen/{screenId}` | Get seats by screen |
| PUT | `/api/seats/{id}` | Update seat |
| DELETE | `/api/seats/{id}` | Delete seat |

### Shows and Show Seats

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/shows` | Create show |
| GET | `/api/shows` | Get all shows |
| GET | `/api/shows/{id}` | Get show by ID |
| GET | `/api/shows/movie/{movieId}` | Get shows by movie |
| GET | `/api/shows/screen/{screenId}` | Get shows by screen |
| GET | `/api/shows/{showId}/seats` | Get seat availability |
| PUT | `/api/shows/{id}` | Update show |
| DELETE | `/api/shows/{id}` | Delete show |

### Bookings

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/bookings` | Create booking |
| GET | `/api/bookings` | Get all bookings |
| GET | `/api/bookings/{id}` | Get booking by ID |
| PUT | `/api/bookings/{id}/cancel` | Cancel booking |

## Sample Booking Request

**POST** `/api/bookings`

```json
{
  "showId": 2,
  "customerName": "Rex",
  "customerEmail": "rex@example.com",
  "customerPhone": "9876543210",
  "seatIds": [8]
}
```

The application validates seat availability and calculates the total amount automatically using the show's ticket price.

## Transaction Management

Database transactions are used during:

- Booking creation.
- Booking cancellation.

This ensures that the booking records and associated seat availability updates are committed together or rolled back if an operation fails.

## Current Version Limitations

- No user authentication or authorization.
- No online payment integration.
- No temporary seat-holding mechanism.
- Version 1 does not guarantee concurrency safety when multiple customers attempt to book the same seat simultaneously.

## Future Enhancements

- User registration and login.
- JWT authentication and role-based authorization.
- Online payment gateway integration.
- Temporary seat reservation with expiry.
- Concurrency-safe seat booking.
- Email and SMS booking confirmations.
- Admin dashboard and booking reports.

## Author

**Rexcelin J.**

Computer Science and Engineering

GitHub: https://github.com/Rexcelin
