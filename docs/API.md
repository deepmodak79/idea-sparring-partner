# API Reference (Planned)

> **Status:** Most endpoints are **planned**, not implemented. `GET /api/health` and `GET /api/health/database` are implemented. Request/response shapes are drafts and may change during implementation.

**Base URL (local):** `http://localhost:5080/api`

**Authentication:** Endpoints marked "Auth required" expect `Authorization: Bearer {accessToken}`.

**Common error shape:**

```json
{
  "error": "Human-readable message",
  "code": "OPTIONAL_ERROR_CODE"
}
```

---

## 1. Health

### GET /api/health

| | |
|---|---|
| **Auth required** | No |
| **Status** | Implemented |

**Response 200:**

```json
{
  "status": "ok",
  "service": "Idea Sparring Partner API",
  "timestamp": "2026-06-28T12:00:00Z"
}
```

---

### GET /api/health/database

| | |
|---|---|
| **Auth required** | No |
| **Status** | Implemented |

**Response 200 (database reachable):**

```json
{
  "status": "ok",
  "database": "reachable",
  "timestamp": "2026-06-28T12:00:00Z"
}
```

**Response 503 (connection string not configured):**

```json
{
  "status": "unavailable",
  "database": "not_configured",
  "timestamp": "2026-06-28T12:00:00Z"
}
```

**Response 503 (database unreachable):**

```json
{
  "status": "unavailable",
  "database": "unreachable",
  "timestamp": "2026-06-28T12:00:00Z"
}
```

Connection strings and detailed database errors are never returned in the response.

---

## 2. Auth

### POST /api/auth/signup

| | |
|---|---|
| **Auth required** | No |
| **Status** | Planned |

**Request body:**

```json
{
  "email": "user@example.com",
  "password": "securePassword123",
  "displayName": "Jane Builder"
}
```

**Response 201:**

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "opaque-refresh-token",
  "expiresIn": 900,
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "displayName": "Jane Builder"
  }
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 400 | Validation failed (weak password, invalid email) |
| 409 | Email already registered |

---

### POST /api/auth/login

| | |
|---|---|
| **Auth required** | No |
| **Status** | Planned |

**Request body:**

```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Response 200:**

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "opaque-refresh-token",
  "expiresIn": 900,
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "displayName": "Jane Builder"
  }
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 400 | Missing fields |
| 401 | Invalid email or password |

---

### POST /api/auth/refresh

| | |
|---|---|
| **Auth required** | No (uses refresh token in body) |
| **Status** | Planned |

**Request body:**

```json
{
  "refreshToken": "opaque-refresh-token"
}
```

**Response 200:**

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "new-opaque-refresh-token",
  "expiresIn": 900
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Invalid or expired refresh token |
| 401 | Token revoked (logout) |

---

### POST /api/auth/logout

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Request body:**

```json
{
  "refreshToken": "opaque-refresh-token"
}
```

**Response 204:** No content.

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |

---

### GET /api/auth/me

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Response 200:**

```json
{
  "id": "uuid",
  "email": "user@example.com",
  "displayName": "Jane Builder",
  "createdAt": "2026-06-28T12:00:00Z"
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Invalid or expired access token |

---

## 3. Ideas

### GET /api/ideas

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Response 200:**

```json
{
  "items": [
    {
      "id": "uuid",
      "title": "AI meal planner for busy parents",
      "description": "Weekly meal plans based on pantry inventory...",
      "createdAt": "2026-06-28T12:00:00Z",
      "updatedAt": "2026-06-28T12:00:00Z"
    }
  ]
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |

---

### POST /api/ideas

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Request body:**

```json
{
  "title": "AI meal planner for busy parents",
  "description": "Weekly meal plans based on pantry inventory and dietary restrictions."
}
```

**Response 201:**

```json
{
  "id": "uuid",
  "title": "AI meal planner for busy parents",
  "description": "Weekly meal plans based on pantry inventory...",
  "createdAt": "2026-06-28T12:00:00Z",
  "updatedAt": "2026-06-28T12:00:00Z",
  "threads": [
    { "id": "uuid", "persona": "Pragmatist", "status": "Active" },
    { "id": "uuid", "persona": "Skeptic", "status": "Active" },
    { "id": "uuid", "persona": "Realist", "status": "Active" },
    { "id": "uuid", "persona": "Contrarian", "status": "Active" }
  ]
}
```

Creating an idea also creates four threads and triggers opening AI challenges (async or inline — implementation detail).

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 400 | Title or description missing/too long |
| 401 | Not authenticated |

---

### GET /api/ideas/{ideaId}

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Response 200:**

```json
{
  "id": "uuid",
  "title": "AI meal planner for busy parents",
  "description": "Weekly meal plans based on pantry inventory...",
  "createdAt": "2026-06-28T12:00:00Z",
  "updatedAt": "2026-06-28T12:00:00Z",
  "threads": [
    { "id": "uuid", "persona": "Pragmatist", "status": "Active", "messageCount": 5 },
    { "id": "uuid", "persona": "Skeptic", "status": "Active", "messageCount": 3 },
    { "id": "uuid", "persona": "Realist", "status": "Active", "messageCount": 4 },
    { "id": "uuid", "persona": "Contrarian", "status": "Active", "messageCount": 2 }
  ]
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |
| 404 | Idea not found or not owned by user |

---

## 4. Threads and Messages

### GET /api/ideas/{ideaId}/threads

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Response 200:**

```json
{
  "items": [
    {
      "id": "uuid",
      "ideaId": "uuid",
      "persona": "Pragmatist",
      "status": "Active",
      "createdAt": "2026-06-28T12:00:00Z"
    }
  ]
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |
| 404 | Idea not found |

---

### GET /api/threads/{threadId}/messages

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Response 200:**

```json
{
  "items": [
    {
      "id": "uuid",
      "threadId": "uuid",
      "role": "Assistant",
      "content": "What's your realistic budget for the first six months?",
      "createdAt": "2026-06-28T12:01:00Z"
    },
    {
      "id": "uuid",
      "threadId": "uuid",
      "role": "User",
      "content": "Around $5,000.",
      "createdAt": "2026-06-28T12:02:00Z"
    }
  ]
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |
| 404 | Thread not found |

---

### POST /api/threads/{threadId}/messages

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Request body:**

```json
{
  "content": "Around $5,000 for MVP development."
}
```

**Response 201:**

```json
{
  "userMessage": {
    "id": "uuid",
    "threadId": "uuid",
    "role": "User",
    "content": "Around $5,000 for MVP development.",
    "createdAt": "2026-06-28T12:02:00Z"
  },
  "assistantMessage": {
    "id": "uuid",
    "threadId": "uuid",
    "role": "Assistant",
    "content": "With $5,000, you'll need to prioritize ruthlessly...",
    "createdAt": "2026-06-28T12:02:05Z"
  },
  "extractedMemory": {
    "id": "uuid",
    "scope": "Idea",
    "type": "Constraint",
    "content": "User has approximately $5,000 budget for MVP.",
    "createdAt": "2026-06-28T12:02:05Z"
  }
}
```

`extractedMemory` may be `null` if nothing worth storing.

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 400 | Empty content |
| 401 | Not authenticated |
| 404 | Thread not found |
| 502 | AI provider failure |
| 503 | AI provider timeout |

---

## 5. Memories

### GET /api/memories

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Query parameters:**

| Param | Required | Description |
|-------|----------|-------------|
| `ideaId` | No | Filter to memories for a specific idea (idea-scoped + user-scoped for owner) |
| `scope` | No | `Idea` or `User` |

**Response 200:**

```json
{
  "items": [
    {
      "id": "uuid",
      "scope": "Idea",
      "type": "Constraint",
      "content": "User has approximately $5,000 budget for MVP.",
      "ideaId": "uuid",
      "sourceThreadId": "uuid",
      "sourceMessageId": "uuid",
      "createdAt": "2026-06-28T12:02:05Z"
    },
    {
      "id": "uuid",
      "scope": "User",
      "type": "Pattern",
      "content": "User tends to underestimate competitive landscape.",
      "ideaId": null,
      "sourceThreadId": "uuid",
      "sourceMessageId": "uuid",
      "createdAt": "2026-06-28T11:00:00Z"
    }
  ]
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |

---

### DELETE /api/memories/{memoryId}

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

Soft-deletes the memory (`is_deleted = true`). Deleted memories are excluded from AI context.

**Response 204:** No content.

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |
| 404 | Memory not found or not owned by user |

---

## 6. Syntheses

### POST /api/ideas/{ideaId}/syntheses

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Request body:** Empty `{}` or omitted.

**Response 201:**

```json
{
  "id": "uuid",
  "ideaId": "uuid",
  "version": 2,
  "strongestChallenges": [
    "Pragmatist: $5,000 budget is insufficient for stated feature scope.",
    "Realist: No evidence of customer interviews cited."
  ],
  "weakestReasoning": [
    "Assumption that parents will pay $15/month without trial data."
  ],
  "unresolvedTensions": [
    "Contrarian argues for B2B school partnerships; user focused on direct consumer."
  ],
  "createdAt": "2026-06-28T13:00:00Z"
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |
| 404 | Idea not found |
| 502 | AI provider failure |

---

### GET /api/ideas/{ideaId}/syntheses

| | |
|---|---|
| **Auth required** | Yes |
| **Status** | Planned |

**Response 200:**

```json
{
  "items": [
    {
      "id": "uuid",
      "version": 1,
      "strongestChallenges": ["..."],
      "weakestReasoning": ["..."],
      "unresolvedTensions": ["..."],
      "createdAt": "2026-06-28T12:30:00Z"
    },
    {
      "id": "uuid",
      "version": 2,
      "strongestChallenges": ["..."],
      "weakestReasoning": ["..."],
      "unresolvedTensions": ["..."],
      "createdAt": "2026-06-28T13:00:00Z"
    }
  ]
}
```

**Possible errors:**

| Status | Condition |
|--------|-----------|
| 401 | Not authenticated |
| 404 | Idea not found |
