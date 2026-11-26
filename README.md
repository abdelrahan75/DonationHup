# DonationHub

A donation platform API: people browse charity campaigns and donate, campaign managers run campaigns, and admins manage everything.
Built with **ASP.NET Core Web API**, **EF Core + SQL Server** and **Clean Architecture** as the .NET Level 2 (Round 2) project.

**Current status: Sprint 1** — solution structure, domain model, identity (JWT + refresh tokens), categories, soft delete, options, and interactive API docs.

---

## Prerequisites

| Tool | Notes |
|---|---|
| [.NET 9 SDK](https://dotnet.microsoft.com/download) | The projects target `net9.0` (the SRS asks for .NET 10; the code does not depend on any .NET 10 feature and will be moved when the SDK is upgraded) |
| SQL Server | LocalDB, SQL Server Express or full SQL Server. The default connection string uses Windows authentication on `.` (local default instance) |
| `dotnet-ef` tool | `dotnet tool install --global dotnet-ef` |
| Git | to clone the repository |

---

## Setup

### 1. Clone and restore

```bash
git clone https://github.com/abdelrahan75/DonationHup.git
cd DonationHup
dotnet restore
```

### 2. Set the secrets (never commit real values)

`appsettings.json` deliberately contains **empty** values for every secret. The app validates its settings on startup and **will refuse to start** until they are provided. Use [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), which are stored outside the repository:

```bash
cd src/DonationHup.API

# JWT signing key: at least 32 characters
dotnet user-secrets set "Jwt:SecretKey" "<a-random-string-of-32-or-more-characters>"

# Initial Admin (the email comes from appsettings.json: admin@donationhub.local)
dotnet user-secrets set "SeedAdmin:Password" "<strong-password>"

# Development-only test users (optional: skipped if not set)
dotnet user-secrets set "DevSeed:ManagerEmail"    "manager@donationhub.local"
dotnet user-secrets set "DevSeed:ManagerPassword" "<strong-password>"
dotnet user-secrets set "DevSeed:DonorEmail"      "donor@donationhub.local"
dotnet user-secrets set "DevSeed:DonorPassword"   "<strong-password>"
```

Password policy: at least 8 characters with an upper-case letter, a lower-case letter and a digit.

Using a different SQL Server? Override the connection string the same way (the connection is named `DN`):

```bash
dotnet user-secrets set "ConnectionStrings:DN" "Server=...;Database=DonationHup;..."
```

### 3. Create the database

From the repository root:

```bash
dotnet ef database update --project src/DonationHub.Infrastructure --startup-project src/DonationHup.API
```

This applies all migrations in `src/DonationHub.Infrastructure/Migrations`. Run it before the first start: the app seeds data at startup and needs the tables to exist.

To add a new migration later:

```bash
dotnet ef migrations add <Name> --project src/DonationHub.Infrastructure --startup-project src/DonationHup.API
```

(In Visual Studio's Package Manager Console: set the **Default project** to `DonationHub.Infrastructure`, the **Startup project** to `DonationHup.API`, then `Add-Migration <Name>` / `Update-Database`.)

---

## Run

```bash
dotnet run --project src/DonationHup.API --launch-profile https
```

The API listens on `https://localhost:7096` (and `http://localhost:5073`).
If the browser or a client complains about the development certificate: `dotnet dev-certs https --trust`.

On startup (Development) the app seeds:
- the roles `Admin`, `CampaignManager`, `Donor`;
- the Admin from `SeedAdminOptions` (only if it does not already exist);
- the Campaign Manager and Donor test users, if their secrets are set;
- five categories (Health, Education, Relief, Orphans, Environment) with Arabic and English names.

In non-Development environments only the roles and the Admin are seeded. Passwords are never logged.

---

## Try it (Scalar)

1. Open **https://localhost:7096/scalar/v1** (it also opens automatically when you start with Visual Studio).
2. Call `POST /api/v1/auth/login` with the Admin credentials:
   ```json
   { "email": "admin@donationhub.local", "password": "<the SeedAdmin:Password you set>" }
   ```
3. Copy the `accessToken` from the response.
4. Click **Authorize** (the Bearer scheme), paste the token and confirm.
5. Call a protected endpoint, e.g. `POST /api/v1/categories`. Without the token it returns **401**, with an Admin token **201**, and with a Donor token **403**.

The access token lives 15 minutes. When it expires call `POST /api/v1/auth/refresh` with the `refreshToken` to get a new pair (each refresh token can be used **once**; using it twice returns `401 Auth.RefreshTokenInvalid`).

A ready-made request file is also included: `src/DonationHup.API/DonationHup.http` (works in Visual Studio and VS Code with the REST Client extension).

### Endpoints (all under `/api/v1`)

| Method | Path | Access | Purpose |
|---|---|---|---|
| POST | `/auth/register` | Anonymous | Register a donor |
| POST | `/auth/login` | Anonymous | Login, returns access + refresh token |
| POST | `/auth/refresh` | Anonymous | Rotate the refresh token |
| POST | `/auth/logout` | Authenticated | Revoke a refresh token |
| GET | `/auth/me` | Authenticated | Current user profile |
| POST | `/admin/staff` | Admin | Create an Admin or Campaign Manager |
| GET | `/categories` | Anonymous | List categories |
| GET | `/categories/{id}` | Anonymous | Get one category |
| POST | `/categories` | Admin | Create a category |
| PUT | `/categories/{id}` | Admin | Update a category |
| DELETE | `/categories/{id}` | Admin | Delete a category (409 `Category.HasCampaigns` if it has campaigns) |

Errors are returned as ProblemDetails with a stable `code` field (for example `Auth.InvalidCredentials`, `Category.NotFound`) and a `traceId`.

---

## Project structure

```
DonationHub.slnx
src/
  DonationHup.Domain/           Entities, value objects, enums, domain rules. Depends on nothing.
  DonationHup.Application/      Use cases, DTOs, interfaces (repository, unit of work, services), options classes.
                                Depends on Domain only. Organized by feature (Auth, Features/Categories).
  DonationHub.Infrastructure/   EF Core (AppDbContext, configurations, migrations, soft-delete interceptor),
                                repository + unit of work, Identity, service implementations, seeding.
  DonationHup.API/              Controllers, JWT setup, error handling, OpenAPI/Scalar, Program.cs (composition).
```

Dependency direction: `Api → Infrastructure → Application → Domain` (the API references Infrastructure only to register services).

---

## Design decisions

Each decision lists the **reason** and the **alternative** that was considered.

### 1. Clean Architecture with four projects
- **Why:** business rules stay free of EF Core, ASP.NET and SQL Server, so they can be unit-tested without a database and the persistence or web layer can change without touching them. The compiler enforces the dependency direction because a project cannot reference a layer above it.
- **Alternative:** a single project with folders, or a classic N-tier. Faster to start, but nothing stops a controller from using the `DbContext` directly, and rules tend to leak into controllers.

### 2. Business rules live in the domain entities
- **Why:** entities have private setters and change state only through methods (for example `Category.EnsureCanBeDeleted`, `RefreshToken.Revoke`). An entity cannot be put in an invalid state from outside, and each rule exists in exactly one place. Time is passed in as a `nowUtc` parameter, so the domain is deterministic and testable.
- **Alternative:** "anemic" entities with public setters and the rules in services. Simpler to write, but the same rule gets copied into several services and can be bypassed by any code that holds the entity.

### 3. Writes go through the Generic Repository + Unit of Work; reads use the `DbContext` directly (`AsNoTracking` + DTO projection) — SRS §3.3 rule 5
- **Why for writes:** a write must load the entity, run its domain methods, and save changes as one atomic unit. `IGenericRepository<T>` (`GetById`, `Add`, `Update`, `Remove`) hides EF Core from the Application layer, and `IUnitOfWork.CompleteAsync()` is the single place that commits, so several changes are saved in **one transaction** (for example, a donation succeeding must update the donation and the campaign together). Both are registered as `Scoped`, so they share one `DbContext` per request.
- **Why for reads:** a read does not change anything, so it does not need the domain rules or change tracking. `AsNoTracking()` skips the change tracker (less memory and CPU), and `Select(...)` into a DTO fetches only the columns the response needs instead of loading whole entities and mapping them afterwards. A generic repository cannot express projections, filters, sorting and paging without becoming either a leaky abstraction or a copy of LINQ.
- **Alternative:** route everything through the repository. One consistent path, but the reads become slower, and the repository grows a method for every query shape (or exposes `IQueryable`, which defeats the abstraction). Another alternative is the Specification pattern, which is a possible bonus later.
- **Today (Sprint 2):** the read queries are MediatR query handlers that use `IAppDbContext` (no-tracking `IQueryable` sets) and `Select` straight into DTOs inside the query. The Application layer has no EF Core reference.
- **Never** return an EF entity from the API; controllers return DTOs.

### 4. Soft delete with a `SaveChangesInterceptor` and a global query filter
- **Why:** campaigns that received donations must not be hard-deleted (domain rule 9). `ISoftDeletable` marks the entities that support it, and `SoftDeleteInterceptor` turns any `Deleted` entry into a `Modified` one (`IsDeleted = true`, `DeletedAtUtc` set) right before saving, so a plain `Remove()` is always safe and no one can forget to write the soft-delete code. `HasQueryFilter(e => !e.IsDeleted)` hides deleted rows from every normal query automatically; `IgnoreQueryFilters()` is used on purpose when deleted rows matter. Both `SavingChanges` and `SavingChangesAsync` are overridden, otherwise `SaveChangesAsync()` would hard-delete.
- **Alternative:** write the `IsDeleted` update and the `Where(!IsDeleted)` in every repository method (easy to forget somewhere), or database triggers (logic hidden from the code).
- **Accepted exception:** `IsDeleted` and `DeletedAtUtc` have public setters so the interceptor can write them. This is the one deliberate exception to the "private setters" rule.
- **Category deletion** counts campaigns with `IgnoreQueryFilters()`: a soft-deleted campaign still holds a foreign key to its category, so it must still block the deletion.

### 4b. Campaign and donation relations use `DeleteBehavior.Restrict`
- **Why:** money-related history (donations, campaigns) must never disappear through a cascade. **Alternative:** cascade delete, which is convenient but can silently delete financial records.

### 5. ASP.NET Core Identity, short-lived JWT, hashed rotating refresh tokens
- **Why:** Identity provides password hashing, the password policy and role management, so we do not write security code ourselves. The access token is a 15-minute JWT (claims: user id, email, one role claim per role), so a stolen token expires quickly. The refresh token is 64 random bytes, shown to the client **once**, and only its **SHA-256 hash** is stored: a database leak does not expose usable tokens. On every refresh the old token is revoked (`RevokedAt`) and linked to the new one (`ReplacedByTokenId`) in a single `CompleteAsync`, so a token can be used only once. Deactivated users cannot log in or refresh.
- **Alternative:** a long-lived JWT only (cannot be revoked), or storing refresh tokens in plain text (a leak equals account takeover). Reuse detection (revoking the whole token family) is a planned bonus.
- **Known limitation:** two simultaneous refreshes with the same token are not serialized yet.

### 6. The same error for a wrong email and a wrong password
- **Why:** returning `Auth.InvalidCredentials` (401) for both prevents attackers from discovering which emails are registered. `Auth.AccountDeactivated` (403) is returned only after a **correct** password, so it cannot be used to probe accounts either.
- **Alternative:** specific messages ("email not found" / "wrong password"), friendlier but it enables user enumeration.

### 7. Options pattern with validation on startup; secrets in user-secrets
- **Why:** each setting group (`JwtOptians`, `SeedAdminOptions`, `DonationOptions`) is a typed class with data annotations and `ValidateOnStart()`. A missing or invalid value stops the app at startup with a clear message instead of failing later in the middle of a request. No secret is stored in the repository: `appsettings.json` has empty placeholders and real values come from user-secrets (and from environment variables in Docker, Sprint 6).
- **Alternative:** reading `IConfiguration["..."]` where needed (typos and missing values only show up at runtime) or committing the secrets (a security incident).

### 8. Expected errors are `Result` values; a global handler catches only unexpected crashes (Sprint 2)
- **Why:** handlers return `Result` / `Result<T>` with a static `Error` from the catalogue (23 codes: `AuthErrors`, `CategoryErrors`, `CampaignErrors`, `DonationErrors`, `CommonErrors`, ...). Each `ErrorType` maps to one HTTP status in `ProblemFactory` (the only mapping; `ApiController` uses it). Business rules stay in the entity, and the entity methods **return a `DomainResult`** (a failure value) instead of throwing; the handler maps it to a catalogue error with `CampaignErrors.FromDomain`. A thrown exception is kept only for real crashes.
- **Validation:** `ValidationBehavior` (MediatR pipeline) runs the FluentValidation validators first and returns `Validation.Failed` (400 + `errors`) without calling the handler.
- **Unexpected errors:** `GlobalExceptionHandler` (`IExceptionHandler`) is the safety net: it logs the full exception and returns a generic 500 ProblemDetails with `code` and `traceId`. No message, stack trace or SQL leaks.
- **Alternative:** exceptions for business failures. Simpler to write, but slower and hidden from the method signature.

### 8b. Optimistic concurrency on campaigns (Sprint 2)
- **Why:** two admins editing one campaign must not silently overwrite each other. `Campaign.RowVersion` is an EF row-version token. The client sends the last `rowVersion` it read and the repository sets it as the original value. `UnitOfWork.CompleteAsync` catches `DbUpdateConcurrencyException` and throws the Application-level `ConcurrencyException` (so EF Core stays out of Application); the update handler catches it and returns `Campaign.ConcurrencyConflict` (409). `GlobalExceptionHandler` maps the same exception for any handler that does not catch it.

### 8c. Campaign language and DTOs (Sprint 2)
- Public endpoints return one language chosen from `Accept-Language` (`ar` or `ar-*`, anything else falls back to `en`). Admin endpoints return both languages plus `rowVersion` (`GET /campaigns/{id}/admin` was added so an admin can read the `rowVersion` needed for `PUT`).
- `progressPercent` and `daysLeft` are computed in the read model with `TimeProvider` (never above 100 / never negative).

### 9. Unique category names: a database index and an early check
- **Why:** the unique indexes on `ArName` and `EnName` are the real guarantee (they hold even under concurrent requests). The service checks first so a duplicate returns a clear `409` (`Category.NameArAlreadyExists` / `Category.NameEnAlreadyExists`) instead of a database error.
- **Alternative:** only the check (race condition between two requests) or only the index (an unfriendly 500).

### 10. OpenAPI + Scalar with a Bearer security scheme
- **Why:** .NET 9 no longer ships Swagger UI by default; the built-in `AddOpenApi()` generates the document and Scalar renders it. A small document transformer declares the Bearer scheme, so protected endpoints can be tested from the **Authorize** button.
- **Alternative:** Swashbuckle (no longer included in the templates). Note: the transformer is written against `Microsoft.OpenApi` 1.x, which is what .NET 9 uses; it must be adapted if the project moves to .NET 10.

### 11. A `.slnx` solution and a `src/` folder
- **Why:** `.slnx` is the new, short, merge-friendly XML solution format. All projects live under `src/`, with the solution at the root.
- **Alternative:** the classic `.sln`, which is longer and conflicts more often in git.

### Known deviations
- Campaign Manager access to campaigns comes in Sprint 3 (Sprint 2 is Admin only).
- Unit tests cover domain rules, validators, handlers and the pipeline; the Infrastructure services (for example `AuthService`) are not unit-tested yet.

---

## What is included in Sprint 1

- [x] Four-layer solution (`.slnx`), Clean Architecture dependency rules
- [x] Domain model with business rules (Category, Campaign, Donation, PaymentAttempt, RefreshToken, …)
- [x] EF Core mapping, code-first migrations committed, unique indexes, concurrency token
- [x] Generic Repository + Unit of Work
- [x] Soft delete (interceptor + global query filter)
- [x] Options classes validated on startup, secrets via user-secrets
- [x] Identity: register, login, refresh (rotation), logout, me, staff creation, seeding (IDN-01 … IDN-12)
- [x] Categories: CRUD with Admin-only writes and public reads (CAT-01 … CAT-04)
- [x] Scalar API documentation with Bearer support
- [x] README

## What is included in Sprint 2

- [x] CQRS with MediatR pinned to `[12.5.0]` (the last free version; 13+ needs a commercial license) + feature folders
- [x] Optimistic concurrency (`RowVersion`) -> `Campaign.ConcurrencyConflict`
- [x] Result pattern + full error catalogue (20 codes)
- [x] FluentValidation + `ValidationBehavior` (the campaign validators also reject a past end date, using `TimeProvider`)
- [x] Global exception handler + RFC 9457 ProblemDetails
- [x] Campaigns: create, update, delete (soft), list, success stories, details (`progressPercent`, `daysLeft`), `Accept-Language`
- [x] `TimeProvider` instead of `DateTime.UtcNow`
- [x] Unit tests (xUnit, Moq, FluentAssertions): `dotnet test`

**Next (Sprint 3):** donations, pagination/filtering, localized messages, Campaign Manager role.
