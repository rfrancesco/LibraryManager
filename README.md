# LibraryManager

A backend for managing a small book library, built with ASP.NET Core (Minimal APIs), Entity Framework Core, and SQL Server.

Personal project for getting hands-on experience with C#/.NET. The domain is intentionally simple: the parts worth looking at are the service layer, the EF Core queries, and the loan constraint enforced in the database.

## Run the program!
The program can be easily run locally using the Dockerfile. 

## Tech stack

- **ASP.NET Core Minimal APIs** — REST endpoints
- **Entity Framework Core** with SQL Server
- **Swashbuckle / Swagger** — OpenAPI docs at `/swagger` in development
- **EntityFrameworkCore.Exceptions** for provider-agnostic constraint-violation exceptions
- **Docker/Podman + Docker Compose** — containerized build; Compose orchestrates the app alongside a SQL Server container for local development. Dockerfile executes the test suite before deployment.
- **Testcontainers** for integration tests — spins up a real SQL Server container for the test suite.

## Architecture

- **Endpoints** (`*Endpoint.cs`) contain only routing and HTTP translation: status codes, request/response DTOs. 
- **Services** (`BookService`, `UserService`, `LoanService`) own all business logic and EF Core queries. Each sits behind an interface and is registered `Scoped` in DI.
- **DTOs** describe data crossing the HTTP boundary. EF Core entities (`Book`, `User`, `Loan`) are never exposed directly.
- `BookService` handles books, `UserService` handles users. A loan is a relationship between the two, so it sits a layer above both: any query touching loans belongs in `LoanService`, including ones that return `Book` rows, like "books currently on loan to user X". (`/users/{id}/books` is the one place that doesn't follow this yet.)

## Data model

- `Book` — `BookId`, `Title`, `Author`, `Genre`
- `User` — `UserId`, `Name`
- `Loan` — `LoanId`, `BookId`, `UserId`, `LoanDate`, `ExpiryDate`, `ReturnDate` (nullable; `null` means the loan is active)

A book can never be on loan to two people at once. This is enforced at the database level with a filtered unique index:

```csharp
migrationBuilder.CreateIndex(
    name: "IX_Loans_BookId",
    table: "Loans",
    column: "BookId",
    unique: true,
    filter: "ReturnDate IS NULL");
```

The application layer also checks upfront (`HasActiveLoanAsync`) and returns `409 Conflict` if the book is already out. That check exists for a clean error message, not for correctness: if two requests get past it simultaneously, the insert violates the index, and the resulting `UniqueConstraintException` is caught and mapped to the same `409`.

Loans are never deleted. Returning one sets `ReturnDate`, so history stays queryable through `/loans`.

## API overview

All list endpoints accept optional `page` and `pageSize`.

### Books

```
GET   /books           Search books — filters: title, author, genre, available
GET   /books/{id}      Get a single book
POST  /books           Add a book, body { title, author, genre }     → 201
GET   /authors         Distinct author names matching book filters
GET   /genres          Distinct genres matching book filters
```

Book filters are all optional, matched case-insensitively by substring, and combined with AND. `/authors` and `/genres` take the same filter set and return only distinct values.

```
GET /authors?author=ley
["Aldous Huxley", "Mary Shelley"]
```

### Users

```
GET   /users              Search users by name (case-insensitive substring)
GET   /users/{id}         Get a single user
POST  /users              Create a user, body { name }               → 201
GET   /users/{id}/books   Books currently on loan to this user
```

```
GET /users?name=e
[{"userId":1,"name":"Alice"},{"userId":3,"name":"Charlie"},{"userId":5,"name":"Eve"}]
```

### Loans

```
GET   /loans                Loan history — filters: userId, bookId, active
GET   /loans/{id}           Get a single loan
POST  /loans                Create a loan, body { bookId, userId }
                              → 201  loan details
                              → 404  book or user does not exist
                              → 409  book is already on loan
POST  /loans/{id}/return    Mark a loan as returned
                              → 200  updated loan details
                              → 404  not found, or already returned
```

## Assumptions and known simplifications

- No authentication. The API assumes it sits behind a trusted frontend used only by library staff, who have full read/write access.
- One `Book` row is one physical copy, so two copies of the same title are two rows. Handling real duplicates needs a `BookCopy` table keyed on `BookId`.
- `Author` and `Genre` are plain strings for simplicity (a realistic case would need separate tables with many-to-many relationship).
- List endpoints return plain arrays with no total count, so a client can't render "page 3 of 12" without guessing.
- No user quota on loans yet.
- No `DELETE` endpoints yet.

## Roadmap

Rough priority order:

- [ ] Extend integration tests on all services (test suite coverage is not complete)
- [ ] Integration tests over HTTP (`WebApplicationFactory`) for routing and status codes
- [ ] CI: build and test on push
- [ ] Authentication and authorization, splitting access between:
  - library staff (full read/write)
  - public (read-only book search, `available` flag only, no user data)
  - individual users (own profile, own active loans)
- [ ] Soft delete for `Book` and `User` instead of hard `DELETE`, to keep loan history intact. For users that means clearing the identifying fields but keeping the row and its `UserId`, so historical loans stay referentially valid
- [ ] `PUT`/`PATCH`/`DELETE` endpoints for books and users
- [ ] Move `/users/{id}/books` off its direct `AppDbContext` query and into `LoanService`

## Run locally
### Running with Docker Compose
 
```bash
cp .env.example .env   # adjust DB_PASSWORD if you'd like
docker compose up --build
```
 
Listens on `http://localhost:5010`; Swagger UI is at `/swagger`. Migrations are applied automatically at startup (`dbContext.Database.Migrate()`).

Environment variables:
- `SEED_DEMO_DATA=true`: Seeds initial mock data (see below)
- `ASPNETCORE_ENVIRONMENT`: `Development`, `Demo` or `Production`.

Mock data is only seeded if
- `SEED_DEMO_DATA=true`
- __AND__ the environment is either `Development` or `Demo` (only `GET` requests are allowed)
- __AND__ the database is empty.

Swagger UI is activated if
- The environment is `Development`
- __OR__ the environment is `Demo`.
 
By default the SQL Server data directory is **not** persisted — `docker compose down` followed by `up` starts from a clean database each time. 
 
### Running tests
Automatically run by Dockerfile in build process:

```bash
dotnet test
```
