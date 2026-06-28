# Database Schema (Planned)

> **Status:** Schema implemented via EF Core initial migration against Supabase PostgreSQL.

## Entity Relationship Diagram

```mermaid
erDiagram
    users ||--o{ ideas : owns
    users ||--o{ memories : has
    users ||--o{ refresh_tokens : has
    ideas ||--|{ threads : has
    ideas ||--o{ memories : has
    ideas ||--o{ syntheses : has
    threads ||--o{ messages : contains
    threads ||--o{ memories : sources
    messages ||--o| memories : sources

    users {
        uuid id PK
        string email UK
        string password_hash
        string display_name
        datetime created_at
        datetime updated_at
    }

    refresh_tokens {
        uuid id PK
        uuid user_id FK
        string token_hash
        datetime expires_at
        datetime created_at
        datetime revoked_at
    }

    ideas {
        uuid id PK
        uuid user_id FK
        string title
        text description
        datetime created_at
        datetime updated_at
    }

    threads {
        uuid id PK
        uuid idea_id FK
        enum persona UK_with_idea
        enum status
        datetime created_at
        datetime updated_at
    }

    messages {
        uuid id PK
        uuid thread_id FK
        enum role
        text content
        datetime created_at
    }

    memories {
        uuid id PK
        uuid user_id FK
        uuid idea_id FK_nullable
        enum scope
        enum type
        text content
        uuid source_thread_id FK_nullable
        uuid source_message_id FK_nullable
        bool is_deleted
        datetime created_at
        datetime deleted_at_nullable
    }

    syntheses {
        uuid id PK
        uuid idea_id FK
        int version UK_with_idea
        jsonb strongest_challenges
        jsonb weakest_reasoning
        jsonb unresolved_tensions
        datetime created_at
    }
```

## Enums

### PersonaType

| Value | Description |
|-------|-------------|
| `Pragmatist` | Financial and practical viability |
| `Skeptic` | Emotional reasoning and motivations |
| `Realist` | Market and competitive assumptions |
| `Contrarian` | Opposite position entirely |

### ThreadStatus

| Value | Description |
|-------|-------------|
| `Active` | Thread open for messaging |
| `Archived` | Thread closed (future use) |

### MessageRole

| Value | Description |
|-------|-------------|
| `User` | Message from the human user |
| `Assistant` | Message from the AI persona |
| `System` | Internal/system prompt artifacts if persisted |

### MemoryScope

| Value | Description |
|-------|-------------|
| `Idea` | Memory specific to one idea (`idea_id` required) |
| `User` | Memory about user patterns across ideas (`idea_id` null) |

### MemoryType

| Value | Description |
|-------|-------------|
| `Fact` | Verified or stated fact |
| `Assumption` | Unverified belief |
| `Constraint` | Limitation (budget, time, skill) |
| `Pattern` | Recurring user behavior (typically user-scoped) |
| `Concession` | Point user yielded on |
| `Deflection` | Topic user avoided |
| `Risk` | Identified risk |
| `OpenQuestion` | Unresolved question |

---

## Tables

### users

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | Generated on insert |
| `email` | VARCHAR(255) | UNIQUE, NOT NULL | Login identifier |
| `password_hash` | VARCHAR(255) | NOT NULL | BCrypt or PBKDF2 |
| `display_name` | VARCHAR(100) | NOT NULL | Shown in UI |
| `created_at` | TIMESTAMPTZ | NOT NULL | |
| `updated_at` | TIMESTAMPTZ | NOT NULL | |

---

### refresh_tokens

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | |
| `user_id` | UUID | FK → users, NOT NULL | |
| `token_hash` | VARCHAR(255) | NOT NULL | **Hash only**, never store raw token |
| `expires_at` | TIMESTAMPTZ | NOT NULL | Default 7 days from creation |
| `created_at` | TIMESTAMPTZ | NOT NULL | |
| `revoked_at` | TIMESTAMPTZ | NULL | Set on logout or rotation |

Index: `(user_id)`, `(token_hash)` for lookup.

---

### ideas

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | |
| `user_id` | UUID | FK → users, NOT NULL | Owner |
| `title` | VARCHAR(200) | NOT NULL | |
| `description` | TEXT | NOT NULL | |
| `created_at` | TIMESTAMPTZ | NOT NULL | |
| `updated_at` | TIMESTAMPTZ | NOT NULL | |

Index: `(user_id, created_at DESC)` for dashboard listing.

---

### threads

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | |
| `idea_id` | UUID | FK → ideas, NOT NULL | |
| `persona` | PersonaType | NOT NULL | |
| `status` | ThreadStatus | NOT NULL, default Active | |
| `created_at` | TIMESTAMPTZ | NOT NULL | |
| `updated_at` | TIMESTAMPTZ | NOT NULL | |

**Unique constraint:** `(idea_id, persona)` — exactly four threads per idea, one per persona.

---

### messages

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | |
| `thread_id` | UUID | FK → threads, NOT NULL | |
| `role` | MessageRole | NOT NULL | |
| `content` | TEXT | NOT NULL | |
| `created_at` | TIMESTAMPTZ | NOT NULL | |

Index: `(thread_id, created_at ASC)` for conversation order.

---

### memories

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | |
| `user_id` | UUID | FK → users, NOT NULL | Owner |
| `idea_id` | UUID | FK → ideas, NULL | Required when `scope = Idea` |
| `scope` | MemoryScope | NOT NULL | |
| `type` | MemoryType | NOT NULL | |
| `content` | TEXT | NOT NULL | Extracted insight text |
| `source_thread_id` | UUID | FK → threads, NULL | Traceability |
| `source_message_id` | UUID | FK → messages, NULL | Traceability |
| `is_deleted` | BOOLEAN | NOT NULL, default false | Soft delete flag |
| `created_at` | TIMESTAMPTZ | NOT NULL | |
| `deleted_at` | TIMESTAMPTZ | NULL | Set when soft-deleted |

Index: `(user_id, scope, is_deleted)`, `(idea_id, is_deleted)`.

---

### syntheses

| Column | Type | Constraints | Notes |
|--------|------|-------------|-------|
| `id` | UUID | PK | |
| `idea_id` | UUID | FK → ideas, NOT NULL | |
| `version` | INT | NOT NULL | Starts at 1, increments per idea |
| `strongest_challenges` | JSONB | NOT NULL | Array of strings |
| `weakest_reasoning` | JSONB | NOT NULL | Array of strings |
| `unresolved_tensions` | JSONB | NOT NULL | Array of strings |
| `created_at` | TIMESTAMPTZ | NOT NULL | |

**Unique constraint:** `(idea_id, version)` — versioned syntheses per idea.

---

## Relationships

| Relationship | Cardinality | Notes |
|--------------|-------------|-------|
| User → Ideas | 1:N | User owns many ideas |
| Idea → Threads | 1:4 | Exactly four threads (one per persona) |
| Thread → Messages | 1:N | Ordered by `created_at` |
| User → Memories | 1:N | All memories belong to a user |
| Idea → Memories | 1:N | Idea-scoped memories only |
| Memory → Thread/Message | N:1 optional | Source traceability |
| Idea → Syntheses | 1:N | Versioned history |
| User → Refresh tokens | 1:N | Multiple devices/sessions possible |

---

## Important Schema Decisions

### Memory is separate from Message

Memories are first-class entities, not appended to message content. This allows:

- Independent listing and deletion
- Scoped retrieval for context assembly
- Clear distinction between conversation text and extracted knowledge

### Thread uses unique (idea_id + persona)

Enforced at database level so an idea cannot accidentally get two Pragmatist threads.

### Synthesis uses unique (idea_id + version)

Every synthesis generation increments version; history is preserved.

### Refresh token stores token_hash, not raw token

Raw refresh tokens are returned once to the client; only a hash is persisted. Compromise of DB does not expose usable refresh tokens.

### Soft delete memories using is_deleted

Deleted memories remain for audit but are excluded from AI context via `is_deleted = false` filter. `deleted_at` records when removal occurred.

---

## Not in Initial Schema

- Full-text search indexes
- Embedding vectors for memories
- Audit log table
- Email verification tokens

These are intentional scope cuts for the take-home assignment.
