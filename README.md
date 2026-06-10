# OLI-it

> *Connect any sender with any recipient — without prior acquaintance.*

**OLI-it** is an open messaging platform built around a semantic matchmaking protocol called **0L1** (Null-Logic-One). Instead of routing messages through social graphs or keyword searches, 0L1 matches messages based on precise mutual criteria expressed in a semantic wordspace.

The platform has been running since 1994 and is live at **[oli-it.com](https://www.oli-it.com)**.  
This repository is a full rewrite of the original ASP.NET WebForms application to modern .NET.

---

## How It Works

Every message on OLI-it carries three kinds of metadata set by the **author**:

1. **A self-description** — who the author is, expressed as marked nodes in the wordspace.
2. **Content and intend** - of the messag, why was it written, what answers are expected. expressed as marked nodes in the wordspace.
3. **Recipient criteria** — what kind of person the author wants to reach, expressed the same way.

Every **recipient** maintains one or more **filter profiles** — their own description of themselves and what content they want to receive.

The **matchmaking algorithm** delivers a message to a recipient only when *all mutual criteria are satisfied on both sides*. No message is ever shown to someone who does not match — and no match is left unseen by someone who asked for it.

```
Author (S)          Wordspace (NKBZ)          Recipient (A)
  │   describes self ──► nodes ◄── describes self   │
  │   sets recipient  ──► nodes ◄── sets content     │
  │            criteria (C) | logic (OgIf) | filter profile (L)    │
  │                                                  │
  └───────── match? ────────────────────────────────►│
                   only when both sides agree (spiegle-x) --> Response (T), reward (kook)
```

---

## Use Cases

Because the wordspace is generic, the same protocol covers very different domains:

- **Q&A** — post a question and reach only people qualified to answer it
- **Classified ads** — sellers and buyers describe themselves and what they want
- **Partner search** — personal or professional, with precise mutual criteria
- **News & announcements** — reach exactly the audience that asked to be reached
- **Commerce** — offers and requests matched by product, price range, location

Anyone can **read** all messages. You must be **logged in** to post, answer, or manage filter profiles.  
Authors can **rate answers**, and good answers are **rewarded with credits**.

---

## This Repository

A modern rewrite of the live application:

| Layer | Technology |
|-------|-----------|
| Web framework | ASP.NET Core Razor Pages (.NET 10) |
| ORM | Entity Framework Core (database-first) |
| Database | SQL Server (existing schema, preserved as-is) |
| Front-end | Bootstrap + vanilla JS (no build step) |
| Hosting | Azure App Service + Azure Blob Storage |

The database schema and matchmaking stored procedures are the source of truth and are not modified by this project. The schema uses German table names (`Stamm`, `PostIt`, `TopLab`, `Angler`, …) reflecting the original 1994 design.

### Current delivery focus

Localization work (EN/DE/ES UI content rollout) is intentionally paused while core feature parity is completed first. The existing language URL switching remains available, but broad page-content translations will resume once MVP workflows are complete.

---

## Getting Started

**Prerequisites:** .NET 10 SDK, SQL Server

```powershell
git clone https://github.com/nulllogicone/OLI-it.Web
cd OLI-it.Web

# copy and fill in your local connection string and image storage URL
# edit OLI-it.Web/appsettings.Development.json

dotnet run --project OLI-it.Web
# → https://localhost:7119
```

Full setup instructions, configuration reference, and infrastructure deployment details are in **[docs/developer-guide.md](docs/developer-guide.md)**.

---

## Documentation

| Document | Description |
|----------|-------------|
| [Vision](docs/00-vision.md) | Background, goals, and success criteria for this rewrite |
| [Motivation](docs/000-motivation.md) | Why the technology choices were made |
| [Domain Entities](docs/01-domain-entities.md) | Core entities and their relationships |
| [Use Cases](docs/03-use-cases.md) | Detailed user stories and acceptance criteria |
| [Architecture](docs/06-architecture.md) | Stack, project structure, and key decisions |
| [Architecture Decisions](docs/07-decisions/) | ADRs (database-first, stored-proc matching, German names) |
| [Developer Guide](docs/developer-guide.md) | Setup, configuration, infra deployment, component docs |
| [Backlog](docs/08-backlog.md) | Upcoming features and known gaps |

---

## Author

Conceived and built by **Frederic Luchting**.  
Protocol described in: [*"Soulmate"* (ISWC 2011, 4 pages)](http://iswc2011.semanticweb.org/fileadmin/iswc/Papers/outrageous/iswc2011outrageousid_submission_13.pdf)

