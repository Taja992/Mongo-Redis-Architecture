# MongoRedisArchitecture

A .NET 10 / ASP.NET Core mongoredisarchitecture using Clean Architecture.

---

## Quick Start

```bash
# Clone and rename
git clone <repo> MyProject
cd MyProject
bash rename-project.sh MyProject

# Run
dotnet build
dotnet run --project API
```

API runs on `http://localhost:8080` · Scalar docs at `/scalar/v1` (dev only)

---

## Layout

```text
├── API/
│   ├── Api/                         → Minimal API endpoints
│   ├── Core/
│   │   ├── Application/
│   │   │   ├── Domain/Dto/          → DTOs
│   │   │   ├── Extensions/          → Dependency injection
│   │   │   ├── Interfaces/          → Service interfaces
│   │   │   └── Services/            → Service implementations
│   │   ├── Configuration/           → AppOptions
│   │   └── Domain/
│   │       ├── Entities/            → EF Core entities
│   │       └── Interfaces/          → Repository interfaces
│   ├── Infrastructure/
│   │   ├── Configuration/           → IEntityTypeConfiguration
│   │   ├── Repositories/            → Repository implementations
│   │   └── AppDbContext.cs
│   ├── appsettings.json
│   └── Program.cs
├── tests/
│   ├── IntegrationTests/            → Testcontainers + WebApplicationFactory
│   └── UnitTests/                   → xUnit + NSubstitute
├── .github/workflows/               → CI (backend)
├── docker-compose.yml               → Postgres + MailHog
├── rename-project.sh                → Rename the mongoredisarchitecture
└── MongoRedisArchitecture.sln
```

---

## Before You Go Live

**Swap the database** — `Program.cs` uses InMemory by default. Replace with Npgsql:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);
```

Store the connection string in user secrets for local dev:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=myapp;Username=postgres;Password=postgres"
```

**Run migrations** (required before integration tests will pass against Postgres):

```bash
dotnet ef migrations add Initial --project API
dotnet ef database update --project API
```

**Enable JWT auth** — uncomment `AddJwtAuthentication` in `Program.cs` and `AddIdentityServices`.

---

## Local Dev with Docker

`docker-compose up` starts Postgres on port `5432` and MailHog on `8025` (email UI).

---

## Running Tests

**Unit tests:**

```bash
dotnet test tests/UnitTests
```

**Integration tests** — Docker must be running (Testcontainers spins up a `postgres:16` container):

```bash
dotnet test tests/IntegrationTests
```

---

## Renaming This MongoRedisArchitecture

```bash
bash rename-project.sh MyNewProject
```

Replaces `MongoRedisArchitecture`/`mongoredisarchitecture` across all file contents, file names, and folder names. Skips `.git/`, `bin/`, `obj/`, `node_modules/`.

Verify after running:

```bash
dotnet build
```

---

## Dev Container (VS Code / Cursor)

This repo includes `.devcontainer/devcontainer.json` for a reproducible Docker-based dev environment.

- .NET 10 dev image
- Runs `dotnet restore` on container creation
- Forwards port `8080`

**VS Code** — `Dev Containers: Reopen in Container`

**Cursor** — same command via command palette. If Dev Container commands are missing in your Cursor build, open in VS Code first then switch back to Cursor.
