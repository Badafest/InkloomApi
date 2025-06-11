# Inkloom API

Inkloom API is a .NET 9 Web API for a blogging platform with user authentication, blog management, tagging, and asset handling. It is designed for extensibility and secure, modern web development.

## Features

- User registration, login (including SSO and magic link), and profile management
- Blog CRUD operations with support for images and rich content
- Tagging system for blogs
- Asset management (upload, serve, and manage images/files)
- Email verification and password reset
- JWT-based authentication and authorization
- Swagger/OpenAPI documentation
- Dockerized for easy deployment

## Project Structure

- `Inkloom.Api` - Main API project (controllers, services, DTOs, middlewares)
- `Inkloom.Api.Data` - Entity Framework Core data models and migrations
- `Inkloom.Api.Assets` - Asset and file management
- `Inkloom.Api.Email` - Email sending and SMTP integration
- `Inkloom.Api.EmailTemplates` - Email template rendering
- `Inkloom.Api.Test` - xUnit test project

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Docker](https://www.docker.com/) (for local development)

### Configuration

Copy `Inkloom.Api/appsettings.Example.json` to `appsettings.Development.json` and fill in the required values (database, JWT, SMTP, etc).

```pwsh
cp Inkloom.Api/appsettings.Example.json Inkloom.Api/appsettings.Development.json
```

### Running with Docker

Start the API and PostgreSQL database using Docker Compose:

```pwsh
docker-compose up --build
```

The API will be available at `http://localhost:8080`.

### Running Locally (without Docker)

1. Ensure PostgreSQL is running and update the connection string in your appsettings.
2. Run database migrations (optional, handled automatically if `MigrateDatabase` is true):
   ```pwsh
   dotnet ef database update --project Inkloom.Api.Data --startup-project Inkloom.Api
   ```
3. Start the API:
   ```pwsh
   dotnet run --project Inkloom.Api
   ```

## API Documentation

Once running, Swagger UI is available at `http://localhost:8080/swagger` (in development mode).

## Main Endpoints

- `POST /auth/register` - Register a new user
- `POST /auth/login` - Login with email/password
- `POST /auth/magic-login` - Magic link login
- `POST /auth/sso-login` - SSO login (Google, etc.)
- `GET /user` - Get current user profile
- `PATCH /user` - Update user profile
- `PATCH /user/change-password` - Change password
- `POST /user/verify-email` - Email verification
- `DELETE /user` - Delete user
- `GET /blog/public` - List public blogs
- `GET /blog/following` - List blogs from followed users
- `GET /blog/{id}` - Get blog by ID
- `POST /blog` - Create blog
- `PATCH /blog/{id}` - Update blog
- `DELETE /blog/{id}` - Delete blog
- `GET /tag` - Search tags
- `GET /asset/{assetId}` - Get asset (image/file)

## Testing

Run tests with:

```pwsh
dotnet test Inkloom.Api.Test
```

## Dependencies

- ASP.NET Core 9
- Entity Framework Core 9
- AutoMapper
- Swashbuckle (Swagger)
- Npgsql (PostgreSQL)
- MailKit (SMTP)
- xUnit (testing)
