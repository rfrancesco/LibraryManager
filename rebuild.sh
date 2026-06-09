rm -r Migrations
rm library.db
dotnet ef migrations add InitialCreate
dotnet ef database update