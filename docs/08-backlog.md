# Backlog

Last updated: 2026-03-26
Status: draft

## Phase 1 — MVP Parity (priority order)

| ID | Title | Use Case | Entity Impact | Status |
|----|-------|----------|---------------|--------|
| BL-001 | Project setup: .NET 10, EF Core, SQL Server, Identity | — | All | not started |
| BL-002 | EF Core models + DbContext + initial migration | — | All entities | not started |
| BL-003 | User registration + login/logout | UC-001, UC-002 | ENT-User | not started |
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

| ID | Title | Notes |
|----|-------|-------|
| BL-101 | Mobile-responsive layout | Bootstrap responsive pass |
| BL-102 | Notifications for matched messages | Email or in-app |
| BL-103 | Admin moderation dashboard | Flag/review messages |
| BL-104 | Advanced wordspace editing (drag-drop ordering) | |
| BL-105 | Analytics/reporting for authors | Views, delivery count |
| BL-106 | API exposure | REST API for external clients |
| BL-107 | Implement rate limiting for login endpoint | Prevent brute force attacks; consider AspNetCoreRateLimit package |
| BL-108 | Add anti-forgery token handling to login API | Re-enable CSRF protection for login/logout endpoints |
| BL-109 | Hash passwords instead of plaintext storage | Use BCrypt or ASP.NET Core Identity password hasher |
| BL-110 | Implement account lockout after failed login attempts | Lock account for X minutes after Y failed attempts |

## Change Log

- 2026-03-26: Initial backlog from use case and entity analysis.
- 2026-03-26: Added authentication security enhancements (BL-107 to BL-110).
