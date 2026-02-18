# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build
dotnet build SunsetBooking.sln

# Run API (dev mode with Swagger)
dotnet run --project SunsetBooking.API/SunsetBooking.API.csproj
# HTTP: http://localhost:5072 | HTTPS: https://localhost:7149

# Run all tests
dotnet test SunsetBooking.sln

# Run a single test
dotnet test SunsetBooking.Tests/SunsetBooking.Tests.csproj --filter "FullyQualifiedName~TestMethodName"

# EF Core migrations (requires ASPNETCORE_ENVIRONMENT=Local or Development)
ASPNETCORE_ENVIRONMENT=Local dotnet dotnet-ef migrations add MigrationName \
  --project SunsetBooking.Domain/SunsetBooking.Domain.csproj \
  --startup-project SunsetBooking.API/SunsetBooking.API.csproj \
  --context HotelRolodexDbContext \
  --output-dir HotelsRolodexFeature/Migrations

# Docker
docker compose up
```

## Architecture

**.NET 8** solution (SDK pinned via `global.json`) with three projects: `SunsetBooking.API` → `SunsetBooking.Domain` ← `SunsetBooking.Tests`

### CQRS + Feature Slices

Domain logic is organized into **feature folders** (e.g., `HotelsRolodexFeature/`), each containing:
- `Bootstrap/` — `DependencyRegistry` with `AddFeatureName(this IServiceCollection)` extension, called from `Program.cs`
- `Commands/` — Command records inheriting `CommandBase`, with co-located `FluentValidation` validators in the same file. Handlers extend `CommandHandlerBase<TCommand, TResult>` and are registered via `ICommandHandler<TCommand, TResult>`.
- `Entities/` — EF Core entities inheriting `EntityBase` (provides `long Id`)
- `Repositories/` — Feature-specific `DbContext` classes inheriting `CustomDbContext<T>`
- `Migrations/` — EF Core migrations scoped per feature DbContext

### Key Patterns

- **Command handler base:** `CommandHandlerBase<TCommand, TResult>` implements `ICommandHandler<TCommand, TResult>`. Handlers are registered as scoped services in each feature's `DependencyRegistry`.
- **FluentValidation auto-validation:** Validators are auto-discovered from the Domain assembly and run automatically via `FluentValidation.AspNetCore` before controller actions execute.
- **Soft delete:** Entities implement `ISoftDeletable`. `CustomDbContext` intercepts deletes and sets `IsDeleted = true`. Global query filters auto-exclude deleted records.
- **Audit fields:** Entities implement `IAuditable` (`CreatedById`, `CreatedAt`, `ModifiedById`, `ModifiedAt`). Populated automatically by `CustomDbContext` via `IUserContext`.
- **Mapping:** `Mapster` (`command.Adapt<Entity>()`) for command-to-entity mapping.
- **Error handling:** `ExceptionMiddleware` catches `ValidationException`, `InvalidOperationException`, `RecordNotFoundException` and returns RFC 7231 problem details JSON.

### Database

- **PostgreSQL** via Npgsql + EF Core
- **Snake_case naming** convention on all tables/columns (`EFCore.NamingConventions`)
- Connection string: per-feature named connection in appsettings (e.g., `ConnectionStrings:HotelRolodexConnection`) for non-prod, or `SUNSET_BOOKING_HOTEL_ROLODEX_DB_CONNECTION` env var for prod
- Migrations run automatically on startup (`Database.Migrate()` for relational, `EnsureCreated()` for InMemory in tests)
- Local `dotnet-ef` tool at version 8.0.0 (use `dotnet dotnet-ef` not `dotnet ef`)

### Testing

- **Alba** integration tests with **InMemory EF Core** provider
- `IntegrationTestBase` (in `SunsetBooking.Tests/Base/`) provides shared Alba host setup: swaps Npgsql for InMemory DB and replaces `IUserContext` with a test stub
- Test classes extend `IntegrationTestBase` and use `Host` property for scenarios

### Logging

Serilog with console sink. Swagger enabled only in `Development` and `Local` environments.


