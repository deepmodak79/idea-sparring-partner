# AI Usage Summary

Brief record of how AI tools were used during this take-home assignment. **Implementation, schema design, API behavior, and tests were written and verified manually**; AI assisted mainly with deployment, integration, and debugging—not with generating the entire codebase.

## Tools used

| Tool | Primary use |
|------|-------------|
| **ChatGPT** | Clarifying EF Core / Supabase connection options, JWT refresh flow, and Gemini API error messages |
| **Cursor** | Deployment troubleshooting (Render Docker, Netlify publish path, CORS), reading logs, and targeted fixes during debugging sessions |
| **Manual work** | Domain model, migrations, thread isolation (`ContextBuilder`), memory pipeline, synthesis versioning, unit tests, E2E script |

## Where AI helped most

- **Supabase PostgreSQL** — Session pooler vs direct connection; Npgsql connection string format when the password contained special characters
- **Deployment** — Render `PORT` binding, Docker entrypoint, Netlify SPA redirects and build output path, environment variable naming for ASP.NET Core
- **Production debugging** — CORS for `*.netlify.app`, Gemini model name updates (`gemini-2.5-flash`), rate-limit / quota errors on the free tier
- **Documentation** — Drafting and aligning `docs/` with the implemented API and schema

## Engineering decisions (human-led)

- **Thread isolation** — Each persona thread gets its own message history in the AI prompt; other threads are excluded via `ContextBuilder`
- **Memory model** — Separate `memories` table with idea-level vs user-level scope
- **Auth** — Custom JWT + refresh tokens (client-side storage for assignment scope)
- **AI abstraction** — `IAiService` so the provider can be swapped without changing controllers
- **Database** — PascalCase columns in Supabase; EF Core migrations as source of truth

## Edge cases tested manually

- Empty thread → opening challenge generation (with staggered calls to reduce Gemini rate limits)
- Partial AI failure on idea create → threads saved, missing openings retried via API
- Thread isolation — message in Pragmatist must not appear in Skeptic context
- Shared Supabase between local and production — same user, consistent idea list when logged in as demo account
- Render free-tier cold start and keep-alive health pings from the production frontend

## What was not delegated to AI

- Core business logic and controller/service structure
- EF entity relationships and migration authoring
- Frontend component wiring (dashboard, four-panel workspace, auth interceptor)
- Backend test suite (`ContextBuilderTests`, auth and API tests)