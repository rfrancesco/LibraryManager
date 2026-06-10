# LibraryManager

Simple library manager backend using ASP.NET Core, implementing 
- REST API for listing books
- REST API for listing users
- Users and Books managed with the Entity Framework, stored in SQLite

The project is still barebones and in implementation.

### Assumptions
It is for now assumed that the backend is protected by external access, and only accessed by the library management frontend by employees, who have total admin rights (!!!).

One could also implement authentication and role-based authorization in the future, to split access between
- Library employees (admin rights, check in/out, view and edit users, view and edit books)
- Public interface (list books, view books without user information, only available=yes/no)
- Library user (view own user, list borrowed books)
This could be more or less fine-grained.

For now, there is no log of loans and loan data is stored directly in the book record.
This is not ideal (separation of concerns!). Todo: add a Loan table, which acts as a junction table between users and loans.
Then, add endpoints to query active and expired loans.

### Implemented
```
BookQuery: 
    title, author, genre, available (bool), pagination (page, pageSize)
    all parameters are optional and matched case-insensitively by substring

GET /books
    Search through all books (in: BookQuery, out: List<Book>)
GET /authors, /genres
    Search through the list of authors or genres (in: BookQuery, out: List<Author>, List<Genre>). Only unique values are returned.

(Some queries are admittedly a bit ridiculous, but the requirements are just an excuse to gain experience)

Examples:
GET /authors?author=ley
Output:
[{"author":"Aldous Huxley"},{"author":"Mary Shelley"}]

UserQuery:
    name (matched case-insensitively by substring)

GET /users 
    Get list of all users
GET /users/{id}
    Get user data
GET /users/{id}/books(?page=...&pageSize=...)
    Get borrowed books by user {id}

Examples:
GET /users?name=e
Output:
[{"userId":1,"name":"Alice"},{"userId":3,"name":"Charlie"},{"userId":5,"name":"Eve"}]
```
### To be implemented
```
Proper error checking:
- [ ] Check for bookId, userId and return NotFound() if not found
- [ ] Return Ok() in all other cases

Administration:
POST, DELETE, PUT to /books, /books/{id}
POST, DELETE, PUT to /users, /users/{id}
```
