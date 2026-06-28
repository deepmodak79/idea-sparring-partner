# Planning

## Build Strategy

Work in vertical slices with backend-first validation:

```
Schema → Backend API → Backend manual test → Frontend UI → Browser test → Commit
```

## Current Progress

All roadmap tasks through submission documentation are **complete** locally.

| Phase | Status |
|-------|--------|
| 0 Foundation | Done |
| 1 Backend shell + DB | Done |
| 2 Domain + migration | Done |
| 3 Auth backend | Done |
| 4–5 Frontend auth | Done |
| 6 Ideas | Done |
| 7–8 AI + threads | Done |
| 9 Memory | Done |
| 10 Synthesis | Done |
| 11 Tests | Done |
| 12 Submission docs | Done |

## Full Task Roadmap

| Task | Description | Status |
|------|-------------|--------|
| 0.1 | Clean repository structure | Done |
| 0.2 | Product and architecture planning docs | Done |
| 1.1 | ASP.NET Core API shell | Done |
| 1.2 | Supabase PostgreSQL connection | Done |
| 2.1 | Core domain entities and enums | Done |
| 2.2 | EF Core relationships and initial migration | Done |
| 3.1 | Auth DTOs and password hashing | Done |
| 3.2 | JWT and refresh token service | Done |
| 3.3 | Signup/login APIs | Done |
| 3.4 | Refresh/logout/me APIs | Done |
| 4.1 | Angular app shell | Done |
| 4.2 | Frontend API health integration | Done |
| 5.1 | Angular token storage and auth service | Done |
| 5.2 | Login/signup pages | Done |
| 5.3 | Guards, interceptor, logout | Done |
| 6.1 | Idea backend APIs | Done |
| 6.2 | Idea dashboard and creation UI | Done |
| 7.1 | AI provider abstraction | Done |
| 7.2 | Persona opening challenges | Done |
| 8.1 | Thread messaging APIs | Done |
| 8.2 | Isolated context builder | Done |
| 8.3 | Four-panel workspace UI | Done |
| 9.1 | Memory extraction pipeline | Done |
| 9.2 | Memory viewer APIs | Done |
| 9.3 | Memory viewer UI | Done |
| 10.1 | Synthesis backend | Done |
| 10.2 | Synthesis UI | Done |
| 11.1 | Backend tests | Done |
| 11.2 | Frontend tests | Done |
| 12.1 | Final Architecture.md | Done |
| 12.2 | Final API.md and Schema.md | Done |
| 12.3 | Planning.md and AI usage summary | Done |
| 12.4 | Screenshots and final cleanup | Pending (manual) |

## AI Usage Summary

AI coding tools were used during development for scaffolding, documentation, architecture alignment, and implementation assistance. Key decisions retained:

- Thread isolation via dedicated context builder (exclude other thread messages)
- Memory as separate entities with idea/user scope
- Session pooler Supabase connection for local IPv4 compatibility
- Client-side JWT storage for take-home practicality

Detailed session logs: see `ai-logs/README.md`.

## Related Documents

- [PRD.md](./PRD.md)
- [Architecture.md](./Architecture.md)
- [API.md](./API.md)
- [Schema.md](./Schema.md)
