# BookHive

BookHive is an ASP.NET Core MVC online bookstore backed by SQL Server LocalDB and Entity Framework Core. It includes identity-based authentication, profile management, admin book inventory workflows, search and filtering, cart and checkout flows, profile and cover image uploads, JSON APIs for storefront interactions, and an admin dashboard.

## Tech stack

- ASP.NET Core MVC on `.NET 10`
- Entity Framework Core 10 with SQL Server
- ASP.NET Core Identity for authentication and password hashing
- Razor views with Bootstrap and custom CSS
- xUnit with EF Core InMemory for automated service tests

## Project structure

- `BookHive/` - main MVC application
- `BookHive/Data` - EF Core context and migrations
- `BookHive/Areas/Admin` - admin dashboard and catalog management
- `BookHive/Controllers/Api` - JSON endpoints used by enhanced frontend interactions
- `BookHive.Tests/` - automated tests
- `frontend/` and `backend/` - reserved organizational folders requested during setup

## Implemented features

- User registration, login, logout, and role seeding
- User profile view, edit, delete, and profile image upload
- Admin book create, edit, delete, and cover image upload
- Public catalog listing, details, search, and filtering
- Shopping cart add, update, and remove flows
- Checkout, order persistence, and order summaries
- Admin dashboard for users, orders, inventory, and order status updates
- API-backed cart interactions with client-side success and error feedback
- Deterministic startup seeding for a 20-book demo catalog

## Configuration

Default configuration points to a LocalDB database named `BookHiveDB`.

Use the following environment variables to override local settings:

```powershell
ConnectionStrings__BookHiveDB=Server=(localdb)\MSSQLLocalDB;Database=BookHiveDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
Admin__Email=admin@bookhive.local
Admin__Password=ChangeMe123
```

The application automatically applies migrations and seeds `Admin` and `Customer` roles on startup. If the configured admin account does not exist, it is created automatically. When the `Books` table is empty, the app also imports a demo catalog from `BookHive/Data/Seed/books.json`.

## Run locally

```powershell
dotnet build BookHive.slnx
dotnet run --project BookHive\BookHive.csproj
```

Open the application URL printed by ASP.NET Core after startup.

## Test

```powershell
dotnet test BookHive.slnx
```

## Notes

- Uploaded files are stored under `BookHive/wwwroot/uploads/`.
- The initial migration is committed in `BookHive/Data/Migrations/`.
- Cart validation prevents quantities from exceeding available inventory before checkout.
- The demo catalog seeder only runs when the `Books` table is empty, so existing admin-managed inventory is left untouched.
