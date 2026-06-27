# Idea Sparring Partner

A full-stack AI application where users submit ideas and receive adversarial feedback from four independent AI personas.

## Current Implementation Status

This repository has **planning documentation** and an **ASP.NET Core API shell** with a health endpoint. Frontend, database, auth, and feature APIs are not implemented yet.

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

Swagger UI (development): http://localhost:5080/swagger

**Troubleshooting:** If `dotnet run` fails with `address already in use` on port 5080, a previous API instance is still running. Press `Ctrl+C` in that terminal, or on Windows run:

```powershell
Get-NetTCPConnection -LocalPort 5080 | Select-Object -ExpandProperty OwningProcess -Unique | ForEach-Object { Stop-Process -Id $_ -Force }
```

Then run `dotnet run` again.

Frontend setup will be added when the Angular app is scaffolded.

## Known Limitations

- Only the API shell and health endpoint are implemented.
- Database, auth, and feature endpoints are planned, not built.
- Frontend UI does not exist yet.

## Submission Notes

The following will be added before final submission:

- Deployment URL
- Screenshots of the working application
- AI usage logs and summary in `ai-logs/`
