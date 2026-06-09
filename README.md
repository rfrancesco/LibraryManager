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
/books
    query parameters: title, author, genre (substring case-insensitive match), available (bool)

/users
/users/{id}
```
### To be implemented
```
Search query:
/books/search?author=...&title=...&available=...

Administration:
POST, DELETE, PUT to /books, /books/{id}
POST, DELETE, PUT to /users, /users/{id}

Pagination for GET requests (imagine receiving a JSON with 1000 books...)
```
