# Idea Sparring Partner

A full-stack AI application where users submit ideas and receive adversarial feedback from four independent AI personas.

## Current Implementation Status

This repository has **planning documentation**, an **ASP.NET Core API shell**, and **Supabase PostgreSQL connectivity** via EF Core. Domain schema, auth, feature APIs, and frontend are not implemented yet.

The following are **not implemented yet**:

- Authentication
- Database schema and migrations
- Feature APIs (ideas, threads, messages, memories, synthesis)
- AI integration
- Frontend UI

## Planned Stack

- **Frontend:** Angular
- **Backend:** ASP.NET Core Web API
- **Database:** Supabase PostgreSQL
- **ORM:** Entity Framework Core
- **AI Provider:** Gemini API
- **Auth:** Custom JWT access token + refresh token

## Planned Local URLs

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4300 |
| Backend API | http://localhost:5080/api |

## Repository Structure

```
README.md
.gitignore
.env.example
backend/IdeaSparringPartner.Api/  # ASP.NET Core Web API
frontend/         # Angular app (placeholder)
ai-logs/          # AI usage logs for submission
docs/
  PRD.md          # Product requirements
  Architecture.md # System design
  API.md          # Planned API contracts
  Schema.md       # Planned database schema
  Planning.md     # Build roadmap and progress
```

## Environment Variables

Copy `.env.example` to `.env` and fill in values when backend and database are implemented. See `.env.example` for the full list of placeholders:

- Database connection string
- JWT configuration (secret, issuer, audience, token lifetimes)
- Gemini API key and AI provider
- Frontend and backend URLs

**Do not commit `.env` or real secrets.**

## Setup

### Backend (local)

```bash
cd backend/IdeaSparringPartner.Api
dotnet restore
dotnet build
dotnet run
```

Health check: http://localhost:5080/api/health

Database health check: http://localhost:5080/api/health/database

Swagger UI (development): http://localhost:5080/swagger

### Database (Supabase PostgreSQL)

The backend uses **Supabase PostgreSQL** as the hosted database with **Entity Framework Core** and the Npgsql provider. No domain tables or migrations exist yet — this task only establishes connectivity.

Configure the connection string locally with **.NET user secrets** (recommended) or environment variables. Do not commit real credentials to `appsettings.json`.

```bash
cd backend/IdeaSparringPartner.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-supabase-connection-string>"
```

In Supabase: **Project Settings → Database → Connection string**. Choose **Session pooler** (not Direct connection) and copy the **.NET** or **URI** format. For EF Core, Session pooler on port **5432** is recommended — the Direct connection string often fails on IPv4-only networks.

On Render or other hosts, set `ConnectionStrings__DefaultConnection` as an environment variable.

**Troubleshooting:** If `dotnet run` fails with `address already in use` on port 5080, a previous API instance is still running. Press `Ctrl+C` in that terminal, or on Windows run:

```powershell
Get-NetTCPConnection -LocalPort 5080 | Select-Object -ExpandProperty OwningProcess -Unique | ForEach-Object { Stop-Process -Id $_ -Force }
```

Then run `dotnet run` again.

Frontend setup will be added when the Angular app is scaffolded.

## Known Limitations

- API shell and health endpoints are implemented; EF Core is wired but no domain schema exists yet.
- Auth and feature endpoints are planned, not built.
- Frontend UI does not exist yet.

## Submission Notes

The following will be added before final submission:

- Deployment URL
- Screenshots of the working application
- AI usage logs and summary in `ai-logs/`
