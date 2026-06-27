# Planning

## Build Strategy

Work in vertical slices with backend-first validation:

```
Schema → Backend API → Backend manual test → Frontend UI → Browser test → Commit
```

For each feature:

1. Define or update schema/docs if needed
2. Implement backend endpoint(s)
3. Test with curl, Swagger, or REST client
4. Implement matching frontend UI
5. Verify end-to-end in browser
6. Commit with a focused message

Do not batch unrelated features into one commit.

## Commit Strategy

- **Small commits** per verified feature
- **Minimum 15 meaningful commits** across the project
- Commit message format: `type: short description` (e.g., `feat: add idea creation API`, `docs: add product and architecture planning`)
- Do not commit secrets, `node_modules`, or build artifacts

## Current Progress

| Task | Status |
|------|--------|
| 0.1 Clean repository structure | Complete |
| 0.2 Product and architecture planning docs | Complete |
| 1.1 ASP.NET Core API shell | Complete |
| 1.2 Supabase PostgreSQL connection | Complete |

Backend API shell and EF Core database connectivity are implemented. Domain schema, auth, and feature APIs are pending.

## Full Task Roadmap

### Phase 0 — Foundation

| Task | Description | Status |
|------|-------------|--------|
| 0.1 | Clean repository structure | Done |
| 0.2 | Product and architecture planning docs | Done |

### Phase 1 — Backend Shell

| Task | Description | Status |
|------|-------------|--------|
| 1.1 | ASP.NET Core API shell | Done |
| 1.2 | Supabase PostgreSQL connection | Done |

### Phase 2 — Domain and Database

| Task | Description | Status |
|------|-------------|--------|
| 2.1 | Core domain entities and enums | Pending |
| 2.2 | EF Core relationships and initial migration | Pending |

### Phase 3 — Authentication (Backend)

| Task | Description | Status |
|------|-------------|--------|
| 3.1 | Auth DTOs and password hashing | Pending |
| 3.2 | JWT and refresh token service | Pending |
| 3.3 | Signup/login APIs | Pending |
| 3.4 | Refresh/logout/me APIs | Pending |

### Phase 4 — Frontend Shell

| Task | Description | Status |
|------|-------------|--------|
| 4.1 | Angular app shell | Pending |
| 4.2 | Frontend API health integration | Pending |

### Phase 5 — Authentication (Frontend)

| Task | Description | Status |
|------|-------------|--------|
| 5.1 | Angular token storage and auth service | Pending |
| 5.2 | Login/signup pages | Pending |
| 5.3 | Guards, interceptor, logout | Pending |

### Phase 6 — Ideas

| Task | Description | Status |
|------|-------------|--------|
| 6.1 | Idea backend APIs | Pending |
| 6.2 | Idea dashboard and creation UI | Pending |

### Phase 7 — AI Foundation

| Task | Description | Status |
|------|-------------|--------|
| 7.1 | AI provider abstraction | Pending |
| 7.2 | Persona opening challenges | Pending |

### Phase 8 — Thread Messaging

| Task | Description | Status |
|------|-------------|--------|
| 8.1 | Thread messaging APIs | Pending |
| 8.2 | Isolated context builder | Pending |
| 8.3 | Four-panel workspace UI | Pending |

### Phase 9 — Memory

| Task | Description | Status |
|------|-------------|--------|
| 9.1 | Memory extraction pipeline | Pending |
| 9.2 | Memory viewer APIs | Pending |
| 9.3 | Memory viewer UI | Pending |

### Phase 10 — Synthesis

| Task | Description | Status |
|------|-------------|--------|
| 10.1 | Synthesis backend | Pending |
| 10.2 | Synthesis UI | Pending |

### Phase 11 — Testing

| Task | Description | Status |
|------|-------------|--------|
| 11.1 | Backend tests | Pending |
| 11.2 | Frontend tests | Pending |

### Phase 12 — Submission

| Task | Description | Status |
|------|-------------|--------|
| 12.1 | Final Architecture.md | Pending |
| 12.2 | Final API.md and Schema.md | Pending |
| 12.3 | Planning.md and AI usage summary | Pending |
| 12.4 | Screenshots and final cleanup | Pending |

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Auth token bugs | Users logged out unexpectedly or tokens leak | Test refresh rotation; document storage tradeoff |
| Context leakage between threads | Personas blend; core product value lost | Unit test context builder; explicit exclusion list |
| Memory scope confusion | Wrong memories in wrong threads | Enforce scope in extraction prompt and DB constraints |
| AI API failures | Broken messaging flow | Graceful errors; retry once; don't persist partial state |
| Deployment env var issues | App fails in production | Mirror `.env.example` on Render/Supabase; health checks |
| Overbuilding UI before backend stable | Rework and wasted effort | Backend manual test before each UI slice |

## Scope Cuts

Explicitly deferred to keep delivery focused:

- **No streaming** — Full AI responses returned synchronously initially
- **No vector search** — All active memories included in context (deterministic retrieval)
- **Simple memory retrieval** — No embedding-based relevance ranking
- **Basic UI styling** — Clean and functional, not a design showcase

## AI Usage Summary

> **Placeholder — to be completed before final submission.**

This section will document:

- AI tools used during development (prompts, sessions)
- Helpful outputs (architecture decisions, code snippets adopted)
- Rejected suggestions (and why)
- Final engineering decisions influenced by AI assistance

See also `ai-logs/` for detailed session logs.

## Related Documents

- [PRD.md](./PRD.md)
- [Architecture.md](./Architecture.md)
- [API.md](./API.md)
- [Schema.md](./Schema.md)
