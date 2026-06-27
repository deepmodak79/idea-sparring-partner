# Product Requirements Document

## Problem Statement

People often validate ideas in echo chambers — talking to friends who agree, or reasoning alone without structured pushback. Important risks, blind spots, and weak assumptions go unexamined until it is too late.

Idea Sparring Partner gives a single user a structured space to stress-test an idea through four adversarial AI personas, each challenging a different dimension of the idea without collapsing into one generic chat.

## Target User

A solo builder, founder, or product thinker who wants honest, structured critique of a business or product idea before investing significant time or money.

For this take-home assignment, the user is a single authenticated account exercising the full flow locally or on a deployed demo.

## Product Goal

Help users sharpen ideas by:

1. Creating four isolated sparring threads with distinct personas
2. Preserving thread isolation while sharing only extracted, scoped memory
3. Letting the user synthesize cross-thread insights on demand

## User Journey

1. **Sign up / log in** — Create an account and authenticate.
2. **Create an idea** — Submit a title and description.
3. **Receive opening challenges** — The system creates four threads (Pragmatist, Skeptic, Realist, Contrarian), each with an initial AI challenge.
4. **Spar in threads** — The user replies in one thread at a time; each persona responds in context of that thread only plus shared memory.
5. **Review memory** — Extracted facts, assumptions, patterns, and risks appear in a memory viewer; user can delete incorrect memories.
6. **Generate synthesis** — User requests a synthesis that reads all four threads and surfaces strongest challenges, weakest reasoning, and unresolved tensions.
7. **Return later** — User opens past ideas, continues threads, generates new synthesis versions.

## Core Features

| Feature | Description |
|---------|-------------|
| Auth | Signup, login, refresh, logout, current user |
| Ideas | Create and list ideas with title and description |
| Four threads | One thread per persona per idea, created automatically |
| Messaging | User sends messages; AI replies per thread with isolated context |
| Memory extraction | After AI replies, system extracts one candidate memory (idea- or user-scoped) |
| Memory viewer | List and soft-delete memories |
| Synthesis | Versioned cross-thread summary on demand |

## Four Personas

### Pragmatist

Challenges **financial and practical viability**.

- Questions: Can this be built affordably? What is the realistic timeline? Where does revenue come from?
- Tone: Grounded, cost-aware, execution-focused.

### Skeptic

Challenges **emotional reasoning and hidden motivations**.

- Questions: Why do you really want this? What are you avoiding? Is this identity-driven rather than market-driven?
- Tone: Probing, reflective, uncomfortable but fair.

### Realist

Stress-tests **market and competitive assumptions**.

- Questions: Who else solves this? Why would customers switch? What evidence supports demand?
- Tone: Analytical, market-oriented, evidence-seeking.

### Contrarian

Argues the **opposite position entirely**.

- Questions: What if the opposite strategy is better? What if this idea should not be pursued at all?
- Tone: Deliberately oppositional, creative, devil's advocate.

Each persona operates in its own thread. Personas do not see each other's message history unless an insight is promoted to shared memory.

## Idea-Level Memory

Facts, constraints, assumptions, risks, or unresolved points **specific to one idea**.

Examples:

- "User has limited capital for this idea."
- "Target market is small business owners in healthcare."
- "Assumption: customers will pay monthly without a free tier."

Scope: tied to a single `idea_id`. Retrieved when generating AI replies for any thread on that idea.

## User-Level Memory

Patterns in **how the user thinks across all ideas**.

Examples:

- "User tends to avoid financial viability questions."
- "User frequently cites personal experience as primary validation."
- "User concedes quickly when challenged on competition."

Scope: tied to `user_id`, not a specific idea. Retrieved when generating AI replies for any thread owned by that user.

## Cross-Thread Synthesis

The user can generate a synthesis for an idea at any time.

**Input:** All four thread conversations for that idea (not other ideas).

**Output (saved, versioned):**

- Strongest challenges across personas
- Weakest reasoning identified in the user's arguments
- Unresolved tensions between threads

Each synthesis is a new version; previous versions remain available for comparison.

## Non-Goals

- Multi-user collaboration on the same idea
- Real-time streaming responses (initial implementation)
- Vector search / semantic memory retrieval
- Mobile-native apps
- Billing or subscriptions
- Admin panel
- Email verification or password reset (for take-home scope)

## Success Criteria

For this assignment, the product succeeds if:

1. A user can sign up, log in, and stay authenticated via refresh tokens.
2. Creating an idea spawns four threads with distinct opening AI challenges.
3. Messages in one thread do not leak into another thread's AI context.
4. Memories are extracted, scoped correctly, and visible in a viewer.
5. Synthesis reads all four threads and produces a saved, versioned result.
6. The app runs locally and can be deployed (frontend + backend + database).

## Scope Boundaries (Take-Home Assignment)

**In scope:**

- Custom JWT auth with refresh tokens
- CRUD for ideas; read/write for threads and messages
- Gemini-powered AI with provider abstraction
- EF Core + Supabase PostgreSQL
- Angular UI: auth, dashboard, four-panel workspace, memory viewer, synthesis
- Basic tests and submission artifacts (docs, screenshots, AI logs)

**Out of scope for initial delivery:**

- Production-grade cookie-based auth
- Streaming SSE responses
- Advanced memory ranking or embeddings
- Polished design system beyond clean, functional UI
