# client2 — LumeLMS Premium UI

A brand-new, premium front-end for the LMS, built against the **.NET 8 backend** (`server-dotnet`).
Dark-first, sleek design with gradient accents and glassmorphism.

## Stack

- **Next.js 14** (App Router) + **TypeScript**
- **Tailwind CSS** with a custom HSL design-token theme (light + dark)
- shadcn-style UI primitives (`Button`, `Input`, `Card`, `Badge`, `Skeleton`)
- `next-themes` (theme toggle), `sonner` (toasts), `lucide-react` (icons)

## What's included (first pass)

- **Landing** — hero with animated gradient/grid backdrop, stats, feature grid, featured courses, CTA
- **Courses** (`/courses`) — live search, category chips, sort (popular / rating / newest / price)
- **Course detail** (`/course/[id]`) — curriculum, benefits, prerequisites, reviews, sticky enroll card
- **Auth** — `/login` and `/signup` (two-step: register → 4-digit email activation), wired to `/api/v1`
- Auth state via React context; JWT access token stored client-side and sent as `Authorization: Bearer`
  (the .NET server also accepts the `access_token` cookie)

## Backend wiring

All calls go to the .NET server via `src/lib/api.ts` → `src/lib/services.ts`, using the endpoints:
`/registration`, `/activate-user`, `/login`, `/me`, `/logout`, `/get-courses`, `/get-course/{id}`,
`/create-order`. The API base URL is configured in `.env.local`:

```
NEXT_PUBLIC_API_URL=http://localhost:8000/api/v1
```

The .NET server's CORS already allows `http://localhost:3000` with credentials, so run this app on port 3000.

## Run it

1. Start the backend (from `../server-dotnet`):
   ```bash
   dotnet run --project src/LMS.WebApi         # serves http://localhost:8000
   ```
2. Start this app:
   ```bash
   npm install
   npm run dev                                  # http://localhost:3000
   ```

> If you run the app on a different port, add that origin to `Cors:AllowedOrigins` in the server's
> `appsettings.json` (or `appsettings.Local.json`).

## Notes

- Course thumbnails/avatars are rendered with plain `<img>` (Cloudinary URLs), so no `next/image`
  domain config is required.
- The catalog shows graceful empty/skeleton/error states, so the UI works even before any courses exist.
