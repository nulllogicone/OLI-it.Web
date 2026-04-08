# Backlog

Last updated: 2026-04-08
Status: draft

## Phase 1 — MVP Parity (priority order)

| ID | Title | Use Case | Entity Impact | Status |
|----|-------|----------|---------------|--------|
| BL-001 | Project setup: .NET 10, EF Core, SQL Server, Identity | — | All | ✅ completed |
| BL-002 | EF Core models + DbContext + initial migration | — | All entities | ✅ completed |
| BL-003 | User registration + login/logout | UC-001, UC-002 | ENT-User | in progress |
| BL-004 | Wordspace admin: nets, nodes, trees, branches CRUD | UC-012 | ENT-Wordspace* | not started |
| BL-005 | Wordspace Navigator component (create message / filter profile) | UC-003, UC-004 | ENT-Criterion | not started |
| BL-006 | Create / edit message with description | UC-003 | ENT-Message, ENT-Description, ENT-Criterion | not started |
| BL-007 | Create / edit filter profiles | UC-004 | ENT-FilterProfile, ENT-Criterion | not started |
| BL-008 | Matchmaking algorithm implementation | UC-005 | ENT-MessageDelivery | not started |
| BL-009 | Inbox — matched messages per user | UC-005 | ENT-MessageDelivery | not started |
| BL-010 | Message detail page + answer submission | UC-006 | ENT-Answer | not started |
| BL-011 | Rate answers + credit transactions | UC-007, UC-011 | ENT-Rating, ENT-Transaction | not started |
| BL-012 | Journal page (public chronological) | UC-008 | ENT-Message | not started |
| BL-013 | Charts page (rankings) | UC-009 | ENT-Message, ENT-User | not started |
| BL-014 | Search (users, messages, answers) | UC-010 | All searchable | not started |
| BL-015 | User profile + transaction history | UC-011 | ENT-User, ENT-Transaction | not started |
| BL-016 | Multi-language support (EN/DE/ES) | — | All pages | not started |
| BL-017 | RSS feed | — | ENT-Message | not started |

## Phase 2 — Enhancements

| ID | Title | Notes | Status |
|----|-------|-------|--------|
| BL-101 | Mobile-responsive layout | Bootstrap responsive pass | not started |
| BL-102 | Notifications for matched messages | Email or in-app | not started |
| BL-103 | Admin moderation dashboard | Flag/review messages | not started |
| BL-104 | Advanced wordspace editing (drag-drop ordering) | | not started |
| BL-105 | Analytics/reporting for authors | Views, delivery count | not started |
| BL-106 | API exposure | REST API for external clients | not started |
| BL-107 | Implement rate limiting for login endpoint | Fixed window: 5 attempts/minute per IP using built-in RateLimiter | ✅ completed |
| BL-108 | Add anti-forgery token handling to login API | Re-enable CSRF protection for login/logout endpoints | ✅ completed |
| BL-109 | Hash passwords instead of plaintext storage | Use BCrypt or ASP.NET Core Identity password hasher | not started |
| BL-110 | Implement account lockout after failed login attempts | Lock account for X minutes after Y failed attempts | not started |
| BL-111 | StammCard → StammStatisticsOverview link | Add a link in the meta-stamm-info box (top-right of StammCard) to a new `StammStatisticsOverview` page that renders charts from recent Stamm activity (journal volume, answer rates, credit flow, etc.) | not started |
| BL-112 | PostItViewer — bare URI content page | A minimal page that fetches and renders a URI's content in its native format (plain text, HTML, Markdown, etc.) with zero chrome: no menu, header, nav, or border. Intended for embedding Gists, blog snippets, and standalone statements as PostIt content. | not started |
| BL-113 | Stakeholder dashboard overview (progress + issues) | A concise dashboard for business stakeholders, product owners, and scrum master to track delivery progress, current issues/blockers, and overall project health at a glance. Proposed implementation: leverage a Copilot agent workflow (similar to spec-kit style) to scrape and interpret project markdown artifacts (issues, feature requests, requirements, acceptance criteria, backlog IDs like BL-112) and synthesize a dependency-aware overview. | ✅ completed |
| BL-114 | DocFx docs site + GitHub Pages publishing | Set up DocFx to build project docs and publish them to GitHub Pages as a public documentation portal (dashboard can be integrated later). | not started |

## Change Log

- 2026-03-26: Initial backlog from use case and entity analysis.
- 2026-03-26: Added authentication security enhancements (BL-107 to BL-110).
- 2026-03-26: Completed BL-107 - Rate limiting implemented with fixed window (5 attempts/min).
- 2026-03-26: Completed BL-108 - Anti-forgery token protection enabled for login/logout endpoints.
- 2026-04-08: Added BL-111 - StammStatisticsOverview page linked from StammCard meta-info box (spontaneous idea).
- 2026-04-08: Added BL-112 - PostItViewer bare URI content page (spontaneous idea).
- 2026-04-08: Added BL-113 - Stakeholder dashboard overview for progress and issues visibility.
- 2026-04-08: Refined BL-113 with implementation detail: Copilot/spec-kit style markdown scraping and dependency-aware synthesis.
- 2026-04-08: Completed BL-113 - Static HTML dashboard generated by docs/generate-dashboard.ps1; prompt at .github/prompts/generate-dashboard.prompt.md.
- 2026-04-08: Added BL-114 - DocFx + GitHub Pages follow-up (deferred).
