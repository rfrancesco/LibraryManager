# LibraryManager

A backend for managing a small library, built with ASP.NET Core (Minimal APIs), Entity Framework Core, and SQLite.

This is a learning/portfolio project: the primary goal is to practice idiomatic C#/.NET backend development (service layer design, EF Core querying, REST API design, DB-level integrity constraints), not to build a production-ready system. Some design choices below are deliberately simplified, with the trade-offs made explicit.

## Tech stack

- **ASP.NET Core Minimal APIs** — REST endpoints, no MVC controllers
- **Entity Framework Core** — data access, SQLite provider (dev), designed to be portable to SQL Server/PostgreSQL
- **Swashbuckle / Swagger** — OpenAPI documentation, available at `/swagger` in development
- **EntityFrameworkCore.Exceptions** — provider-agnostic exception types for constraint violations (avoids depending on SQLite-specific error codes)

## Architecture

- **Endpoints** (`*Endpoint.cs`) contain only routing and HTTP-shape translation (status codes, request/response DTOs). No business logic lives here.
- **Services** (`BookService`, `UserService`, `LoanService`) own all business logic and EF Core queries. Each service is exposed through an interface (`IBookService`, etc.) and registered as `Scoped` in DI.
- **DTOs** describe data crossing the HTTP boundary (`BookDetailsDto`, `CreateLoanDto`, ...). They're kept separate from EF Core entities (`Book`, `User`, `Loan`), which are never exposed directly.
- `BookService` only handles books, `UserService` only handles users. `Loan` sits one architectural layer above both, since a loan *is* a relationship between the two. Any query that touches loans — even one returning `Book` or `User` rows, like "books currently on loan to user X" — belongs in `LoanService`, not in `BookService`/`UserService`.

## Data model

- `Book` — `Id`, `Title`, `Author`, `Genre` (all plain strings; see limitations below)
- `User` — `UserId`, `Name`
- `Loan` — `LoanId`, `BookId`, `UserId`, `LoanDate`, `ExpiryDate`, `ReturnDate` (nullable — `null` means the loan is active)

### Enforced invariant: one active loan per book

A **filtered unique index** on `Loans(BookId) WHERE ReturnDate IS NULL` guarantees at the database level that a book can never have two simultaneously active loans, regardless of application-level race conditions. The application layer also performs an upfront check (`HasActiveLoanAsync`) for a fast, readable `409 Conflict` response; the DB constraint is the real safety net and is caught via `UniqueConstraintException` (provider-agnostic, via `EntityFrameworkCore.Exceptions`) in case of a race.

## API overview

All list endpoints support pagination (`page`, `pageSize`) and return plain arrays (no `totalCount`/`totalPages` metadata yet — see Roadmap).

### Books

```
GET  /books                Search books (filters: title, author, genre, available — all optional, case-insensitive substring match)
GET  /books/{id}           Get a single book by id
POST /books                Add a new book
GET  /authors               Distinct author names matching the same book filters
GET  /genres                 Distinct genres matching the same book filters
```

### Users

```
GET  /users                 Search users by name (case-insensitive substring)
GET  /users/{id}            Get a single user by id
POST /users                  Create a new user
GET  /users/{id}/books       Books currently on loan to this user
```

### Loans

```
GET  /loans                          Loan history (filters: userId, bookId, active — all optional)
GET  /loans/{id}                     Get a single loan by id
POST /loans                          Create a loan (body: bookId, userId)
                                      → 201 with loan details
                                      → 404 if book or user doesn't exist
                                      → 409 if the book is already on loan
POST /loans/{id}/return               Mark a loan as returned
                                      → 200 with updated loan details
                                      → 404 if not found or already returned
```

## Assumptions and known simplifications

- **No authentication.** The API is assumed to sit behind a trusted frontend used only by library staff, who have full read/write access to everything. This is the single biggest gap before this could be used by anyone other than staff.
- **One row per physical copy is not modeled.** Each `Book` row is a single, unique item — multiple copies of the same title would need a separate `Copy`/`BookCopy` table with a `BookId` foreign key. Skipped for scope reasons.
- **`Author` and `Genre` are plain strings**, not normalized entities. A book realistically has multiple genres and possibly multiple authors; modeling that properly would need many-to-many relations. Skipped deliberately: the added complexity (junction tables, orphan handling) wasn't worth it for this project's scope.
- **List endpoints return raw arrays**, without total count/page metadata. Fine for now; would need revisiting for a real paginated UI (see Roadmap).
- **Deletion is not implemented at all** (no `DELETE` endpoints for books, users, or loans).

## Roadmap

Rough priority order:

- [ ] **Loan/User service cleanup** — finish migrating any endpoint still querying `AppDbContext` directly instead of going through a service (`/users/{id}/books` at time of writing).
- [ ] **Authentication & authorization** — split access between:
  - Library staff (full read/write)
  - Public (read-only book search, `available` flag only, no user-linked data)
  - Individual library users (view own profile, own active loans)
- [ ] **Soft delete** for `Book` and `User` instead of hard `DELETE`, to preserve loan history integrity, with personal data deletion to simulate a reasonable business case
- [ ] **Unit tests** for service-layer business logic (SQLite in-memory, to exercise real constraints like the filtered unique index).
- [ ] **Integration tests** for HTTP routing/status codes
- [ ] **CI/CD** pipeline (build + test on push).
- [ ] Full `POST`/`PUT`/`DELETE` administration endpoints for books and users.

## Running locally

```bash
dotnet run
```

Swagger UI is available at `/swagger` in the Development environment. The SQLite database (`library.db`) is seeded automatically on first run if empty, via `DbInitializer` — gated behind `IsDevelopment()` in `Program.cs`, so it never runs outside local development. The seed data is illustrative (a few dozen books, users, and loans) and not meant to be exhaustive or realistic; loan records reference books by their expected auto-generated id, so reordering the seed book list will silently point loans at the wrong book.