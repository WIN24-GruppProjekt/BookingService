# 📅 BookingService

BookingService is a .NET 9 Web API that acts as a **booking ledger** for users registering to events.  
It provides booking creation, deletion, and querying functionality, while keeping track of active participants per event.

---

## ✨ Features

- Manage **bookings**:
  - Create bookings for users against events
  - Delete individual bookings, or delete all bookings for a given user or event
  - Automatically timestamp bookings with `CreatedAt`

- Track **active participants**:
  - Count how many users are registered for an event
  - Enforce consistency by deriving participants count from bookings

- REST API with Swagger documentation
- Data validation using `System.ComponentModel.DataAnnotations`
- Entity Framework Core with SQL Server
- Layered architecture:
  - **Persistence** (Entities, DbContext, Repositories)
  - **Application** (DTOs, Models, Services)
  - **Presentation** (Controllers / API endpoints)

---

## 📂 Project Structure
```
BookingService/
│── Application/
│ ├── DTOs/ # Request models (CreateBookingRequest)
│ ├── Models/ # Domain models & result wrappers
│ └── Services/ # IBookingService interface
│
│── Persistence/
│ ├── Contexts/ # DbContext
│ ├── Entities/ # EF Core entities (BookingEntity)
│ └── Repositories/ # Data access layer
│
│── Presentation/
│ └── Controllers/ # API controller (BookingsController)
│
│── Program.cs # Entry point & dependency injection
``` 

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server (local or cloud)
- A connection string in an `appsettings.json` file such as:
```json
"ConnectionStrings": {
  "localDatabase": "Server=localhost;Database=BookingDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## 📌 API Endpoints

Bookings
```
| Method | Endpoint                                     | Description                                    |
| ------ | -------------------------------------------- | ---------------------------------------------- |
| POST   | `/api/bookings`                              | Create a new booking                           |
| GET    | `/api/bookings/user/{userId}`                | Get all bookings for a specific user           |
| GET    | `/api/bookings/event/{eventId}/participants` | Get number of active participants for an event |
| DELETE | `/api/bookings/{bookingId}`                  | Delete a booking by ID                         |
| DELETE | `/api/bookings/user/{userId}`                | Delete all bookings for a user                 |
| DELETE | `/api/bookings/event/{eventId}`              | Delete all bookings for an event               |
```

## 🧪 Example Requests

### Create Booking
```json
POST /api/bookings
Content-Type: application/json

{
  "userId": "user-123",
  "eventId": "event-456"
}
```

### Get Bookings for a User
```json
GET /api/bookings/user/user-123
```

### GET /api/bookings/event/event-456/participants
```json
GET /api/bookings/event/event-456/participants
```

###
```json
DELETE /api/bookings/booking-guid-here
```

## 🛠️ Tech Stack
- .NET 9
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Dependency Injection



