# Idea Sparring Partner

A full-stack AI application where users submit ideas and receive adversarial feedback from four independent AI personas (Pragmatist, Skeptic, Realist, Contrarian).

## Live Demo

| Service | URL |
|---------|-----|
| Frontend (Netlify) | https://idea-sparring-partner.netlify.app |
| Backend API (Render) | https://idea-sparring-partner-api.onrender.com/api |
| Health check | https://idea-sparring-partner-api.onrender.com/api/health |

### Demo login

A demo account is available for reviewers:

| Field | Value |
|-------|-------|
| Email | `dmm@gmail.com` |
| Password | `12345678` |

Use **Log in** on the live site (or sign up with your own account locally).

## Design & scope note

This project prioritizes **structural flow, database design, API contracts, thread isolation, memory extraction, and synthesis** over visual polish. Due to time constraints, effort went into workflow and feature implementation rather than theming or a production-grade UI. The interface is functional and readable, not a finished design system.

## AI provider (Gemini free tier)

The app uses the **Google Gemini API** with a **free-tier API key**. Free quotas are limited (requests per minute/day vary by Google’s current policy). When the limit is reached, AI actions may fail with an error such as **“limit exceeded”** or a rate-limit / quota message in the UI or API response. Opening challenges, chat replies, memory extraction, and synthesis all depend on Gemini.

If you hit the limit during review:

- Wait a few minutes and retry, or
- Use an idea that already has thread messages in the database, or
- Run locally with your own Gemini key in user secrets (see Setup below).

## Current Implementation Status

The full application is implemented:

- ASP.NET Core Web API — auth, ideas, threads, messages, memories, synthesis
- Angular frontend — login, dashboard, four-panel workspace, memory viewer, synthesis
- Supabase PostgreSQL via EF Core
- Gemini AI integration (`IAiService` abstraction)

## Stack

| Layer | Technology |
|-------|------------|
| Frontend | Angular (port 4300) |
| Backend | ASP.NET Core Web API (port 5080) |
| Database | Supabase PostgreSQL |
| ORM | Entity Framework Core |
| AI | Gemini API (free tier) |
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

Configure secrets (do not commit these). Copy `.env.example` to `.env` at the repo root, or use user secrets:

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
- Log in → create idea → spar in four panels → view memories → generate synthesis

Optional end-to-end script (local API):

```powershell
.\local\e2e-test.ps1
```

### Troubleshooting

**Port 5080 in use:** Stop the previous `dotnet run` with `Ctrl+C`, or on Windows:

```powershell
Get-NetTCPConnection -LocalPort 5080 | Select-Object -ExpandProperty OwningProcess -Unique | ForEach-Object { Stop-Process -Id $_ -Force }
```

**Gemini quota / rate limit:** See [AI provider (Gemini free tier)](#ai-provider-gemini-free-tier) above.

**Render cold start:** The free backend may sleep after ~15 minutes of inactivity; the first request can take 30–60 seconds.

## Tests

```bash
cd backend/IdeaSparringPartner.Api.Tests
dotnet test

cd frontend
npm test
```

## Documentation

| Document | Description |
|----------|-------------|
| [docs/PRD.md](docs/PRD.md) | Product requirements |
| [docs/Architecture.md](docs/Architecture.md) | System design |
| [docs/API.md](docs/API.md) | REST API reference |
| [docs/Schema.md](docs/Schema.md) | Database schema |
| [docs/Planning.md](docs/Planning.md) | Build roadmap and progress |
| [ai-logs/README.md](ai-logs/README.md) | AI tool usage summary |

## Deployment

Production: **Netlify** (frontend) + **Render** (Dockerized API) + **Supabase** (PostgreSQL). See `local/DEPLOYMENT-GUIDE.md` (gitignored) for step-by-step instructions.

## Submission artifacts

| Item | Location / status |
|------|-------------------|
| Live app | https://idea-sparring-partner.netlify.app |
| Demo credentials | See [Demo login](#demo-login) |
| Architecture & API docs | `docs/` |
| AI usage summary | `ai-logs/README.md` |
| Screenshots | Add PNG/JPG files under `docs/screenshots/` (see below) |

### Screenshots

Create folder `docs/screenshots/` and add images such as:

| File (suggested name) | What to capture |
|-----------------------|-----------------|
| `01-login.png` | Login page |
| `02-dashboard.png` | Dashboard with idea list |
| `03-workspace.png` | Four-panel workspace with thread messages |