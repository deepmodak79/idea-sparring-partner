# Idea Sparring Partner

A full-stack AI application where users submit ideas and receive adversarial feedback from four independent AI personas.

## Current Implementation Status

The full application is implemented locally:

- ASP.NET Core Web API with auth, ideas, threads, messages, memories, and synthesis
- Angular frontend with login, dashboard, four-panel workspace, memory viewer, and synthesis
- Supabase PostgreSQL via EF Core
- Gemini AI integration (requires API key in user secrets)

## Stack

| Layer | Technology |
|-------|------------|
| Frontend | Angular (port 4300) |
| Backend | ASP.NET Core Web API (port 5080) |
| Database | Supabase PostgreSQL |
| ORM | Entity Framework Core |
| AI | Gemini API |
| Auth | JWT access token + refresh token |

## Local URLs

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4300 |
| Backend API | http://localhost:5080/api |
| Swagger | http://localhost:5080/swagger |

## Setup

### 1. Backend

```bash
cd backend/IdeaSparringPartner.Api
dotnet restore
dotnet build
dotnet run
```

Configure secrets (do not commit these):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<supabase-session-pooler-connection-string>"
dotnet user-secrets set "Jwt:Secret" "<long-random-secret-at-least-32-chars>"
dotnet user-secrets set "Ai:GeminiApiKey" "<your-gemini-api-key>"
```

Use **Supabase Session pooler** (port 5432), not Direct connection, for local development.

Apply migrations if needed:

```bash
dotnet ef database update
```

### 2. Frontend

```bash
cd frontend
npm install
npm start
```

Open http://localhost:4300

### 3. Verify

- http://localhost:5080/api/health
- http://localhost:5080/api/health/database
- Sign up → create idea → spar in four panels → view memories → generate synthesis

### Troubleshooting

**Port 5080 in use:** Stop the previous `dotnet run` with `Ctrl+C`, or on Windows:

```powershell
Get-NetTCPConnection -LocalPort 5080 | Select-Object -ExpandProperty OwningProcess -Unique | ForEach-Object { Stop-Process -Id $_ -Force }
```

## Tests

```bash
cd backend/IdeaSparringPartner.Api.Tests
dotnet test

cd frontend
npm test
```

## Deployment

See `local/DEPLOYMENT-GUIDE.md` (gitignored) for Render + Netlify instructions.

## Submission Notes

- Deployment URL: _to be added_
- Screenshots: _to be added_
- AI usage summary: `ai-logs/README.md`
