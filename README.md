# Mongo Redis Architecture

A blogging platform built with .NET 10 / ASP.NET Core, MongoDB, and Redis — demonstrating Clean Architecture with a document store, caching, and rate limiting.

---

## Quick Start

```bash
# Start MongoDB + Redis
docker compose up -d

# Run the API
dotnet run --project API
```

API runs on `http://localhost:8080` · Scalar docs at `/scalar` (dev only)

---

## Stack

| Layer | Technology |
| --- | --- |
| API | .NET 10 / ASP.NET Core minimal APIs |
| Database | MongoDB (document store) |
| Cache / Rate Limiting | Redis via StackExchange.Redis |
| Logging | Serilog |
| Docs | Scalar / OpenAPI |

---

## Layout

```text
├── API/
│   ├── Api/                         → Minimal API endpoints (BlogApi, PostApi)
│   ├── Core/
│   │   ├── Application/
│   │   │   ├── Domain/
│   │   │   │   ├── Dto/             → Request/response records
│   │   │   │   └── Exceptions/      → AppException, RateLimitExceededException
│   │   │   ├── Exceptions/          → GlobalExceptionHandler, AddExceptionHandling()
│   │   │   ├── Extensions/          → AddApplication(), AddInfrastructure(), AddRedis()
│   │   │   ├── Interfaces/          → IPostService, IBlogService, IPostCacheService, IRateLimitService
│   │   │   └── Services/            → PostService, BlogService
│   │   └── Domain/
│   │       ├── Entities/            → Blog, Post, Comment (MongoDB documents)
│   │       └── Interfaces/          → IBlogRepository, IPostRepository
│   ├── Infrastructure/
│   │   ├── Cache/                   → RedisPostCacheService, RedisRateLimitService
│   │   └── Repositories/            → MongoBlogRepository, MongoPostRepository
│   ├── appsettings.Development.json
│   └── Program.cs
├── tests/
│   ├── IntegrationTests/
│   └── UnitTests/
├── docs/                            → Implementation guides (see below)
├── docker-compose.yml               → MongoDB + Redis
└── MongoRedisArchitecture.sln
```

---

## Domain

User → Blog → Post → Comment

- A `Blog` is owned by a user and contains many `Post` documents
- A `Post` embeds its `Comment` list directly (no separate collection)
- Comments are append-only via MongoDB `$push`

---

## Key Architecture Decisions

**MongoDB over a relational DB**
Comments are embedded inside the Post document. A single read fetches the post and all its comments — no joins, no N+1. Posts are referenced from Blog by ID (not embedded) because posts grow unbounded.

**Cache-aside with Redis**
Individual posts are cached at `post:{id}` and blog post lists at `blog:{blogId}:posts`. The cache is populated on read and invalidated on write. TTL is configurable via `Redis:PostTtlSeconds`.

**Redis for rate limiting**
Comment rate limiting uses Redis `INCR` — atomic, no race condition. The counter key is `rate:comment:{authorId}` and expires after the window (default 1 hour, 10 comments max). Exceeding the limit throws `RateLimitExceededException` → 429.

**Global exception handling**
All errors surface through `GlobalExceptionHandler` as RFC 9457 `ProblemDetails`. Services throw domain exceptions (`AppException` subclasses); endpoints have no try/catch.

**No EF Core for domain entities**
`AppDbContext` is present but unused by the blog domain — it's a template artefact backed by an in-memory DB. All domain data flows through the MongoDB repositories.

---

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/blogs` | Create a blog |
| `GET` | `/api/blogs/{id}` | Get blog with all posts |
| `DELETE` | `/api/blogs/{id}` | Delete blog and its posts |
| `POST` | `/api/blogs/{blogId}/posts` | Create a post |
| `GET` | `/api/posts/{id}` | Get a post |
| `PUT` | `/api/posts/{id}` | Update a post |
| `DELETE` | `/api/posts/{id}` | Delete a post |
| `POST` | `/api/posts/{id}/comments` | Add a comment (rate limited) |
| `GET` | `/health` | Health check |

---
