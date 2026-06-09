# LibraryManager

Simple library manager backend using ASP.NET Core, implementing 
- REST API for listing books
- REST API for listing users
- Users and Books managed with the Entity Framework, stored in SQLite

The project is still barebones and in implementation.

### Assumptions
It is for now assumed that the backend is protected by external access, and only accessed by the library management frontend by employees.

One could also implement authentication in the future, to split access between
- Library employees (admin rights, check in/out, view and edit users, view and edit books)
- Public interface (list books, view books without user information, only available=yes/no)
- Library user (view own user, list borrowed books)

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

/users
/users/{id}
```
### To be implemented
```
Administration:
POST, DELETE, PUT to /books, /books/{id}
POST, DELETE, PUT to /users, /users/{id}
```
