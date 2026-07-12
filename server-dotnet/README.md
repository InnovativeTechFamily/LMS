# LMS Server — .NET 8 (Clean Architecture)

A .NET 8 Web API re-implementation of the original Node/Express LMS backend, built with
Clean Architecture. It is a faithful, drop-in replacement for the Node server: same
`/api/v1` routes, same JSON response shapes, same MongoDB data (reused as-is) and Redis sessions.

## Solution layout

```
server-dotnet/
  LMS.slnx
  src/
    LMS.Domain/          # Entities & enums — no dependencies
    LMS.Application/      # Use-case services, DTOs, interfaces (ports), exceptions
    LMS.Infrastructure/   # MongoDB, Redis, JWT, Cloudinary, Stripe, VdoCipher, SMTP email
    LMS.WebApi/           # Controllers, middleware, auth, SignalR hub, background jobs, Program.cs
```

Dependency rule: `WebApi → Infrastructure → Application → Domain`. Domain has no outward
references; Application defines interfaces that Infrastructure and WebApi implement.

## Requirements

- .NET 8 SDK (the project also builds/runs on the .NET 9/10 SDK — target framework stays `net8.0`)
- MongoDB (defaults to `mongodb://localhost:27017`, database `lms`)
- Redis (defaults to `localhost:6379`)

> If only newer runtimes are installed, run with roll-forward:
> `DOTNET_ROLL_FORWARD=LatestMajor dotnet run --project src/LMS.WebApi`

## Configuration

All settings live in `src/LMS.WebApi/appsettings.json` (sections: `Mongo`, `Redis`, `Jwt`,
`Cloudinary`, `Stripe`, `VdoCipher`, `Email`, `Cors`). **Replace the placeholder `Jwt` secrets**
and supply the integration keys. For real deployments use environment variables or
`dotnet user-secrets` rather than committing secrets. Example (env var overrides use `__`):

```
Jwt__AccessTokenSecret=...   Stripe__SecretKey=...   Cloudinary__ApiSecret=...
```

## Run

```bash
cd server-dotnet
dotnet restore
dotnet run --project src/LMS.WebApi
```

Swagger UI is served at `/swagger` in Development. Liveness check: `GET /test`.
Realtime notifications are delivered over SignalR at the `/hubs/notifications` hub
(the `newNotification` event), replacing the original Socket.IO server.

## How it maps to the original server

| Node concept                     | .NET equivalent                                             |
|----------------------------------|-------------------------------------------------------------|
| Express routers / controllers    | ASP.NET Core controllers under `Controllers/` (`/api/v1`)   |
| Mongoose models                  | `Domain/Entities` POCOs + `Infrastructure/Persistence` maps |
| Mongoose `{ timestamps:true }`   | `createdAt`/`updatedAt` set in repositories                 |
| ioredis sessions & caching       | `ICacheService` → `RedisCacheService` (StackExchange.Redis) |
| JWT in cookies (access/refresh)  | JWT bearer reading `access_token` cookie or header + cookies |
| bcryptjs                         | `BCrypt.Net-Next`                                           |
| ErrorMiddleware / CatchAsyncError| `ExceptionHandlingMiddleware` + typed `AppException`s        |
| Cloudinary / Stripe / VdoCipher  | `IMediaStorage` / `IPaymentService` / `IVideoService`       |
| Nodemailer + EJS templates       | `IEmailService` (MailKit) + `EmailTemplates/*.html`         |
| Socket.IO                        | SignalR `NotificationHub`                                    |
| node-cron notification cleanup   | `NotificationCleanupService` (`BackgroundService`)          |

### Preserved data compatibility

MongoDB documents are reused unchanged: element names are camelCase, string ids are stored
as ObjectId `_id` (roots and embedded sub-documents alike), and the original snake_case fields
`public_id` and `payment_info` are kept.
