# Idea Sparring Partner

A full-stack AI application where users submit ideas and receive adversarial feedback from four independent AI personas.

## Current Implementation Status

This repository is at the **repository and planning setup stage**. Documentation and folder structure are in place; application code has not been started.

The following are **not implemented yet**:

- Authentication
- Database schema and migrations
- Backend API endpoints
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
backend/          # ASP.NET Core Web API (placeholder)
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

Detailed setup steps (prerequisites, local database, backend run, frontend run) will be expanded as the backend and frontend projects are scaffolded.

## Known Limitations

- This is currently **documentation and repository setup only**.
- No runnable application exists yet.
- API contracts and schema are planned, not implemented.

## Submission Notes

The following will be added before final submission:

- Deployment URL
- Screenshots of the working application
- AI usage logs and summary in `ai-logs/`
