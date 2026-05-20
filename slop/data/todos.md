# Developer Todos

## TODO-001: Review and update EF scaffolding guide
**Status:** in-progress
**Priority:** high
**Created:** 2026-05-13
**Tags:** docs, ef-core
**Notes:** Scaffold command needs updating after DB schema changes in sprint 4

## TODO-002: Write unit tests for PostIt service
**Status:** todo
**Priority:** medium
**Created:** 2026-05-13
**Tags:** testing, services
**Notes:** Cover CRUD operations and pagination logic

## TODO-003: Improve error handling in auth endpoints
**Status:** todo
**Priority:** high
**Created:** 2026-05-13
**Tags:** auth, api
**Notes:** Return proper 401/403 with JSON error bodies instead of HTML redirects

## TODO-004: Add cursor-based pagination to PostIt feed
**Status:** todo
**Priority:** medium
**Created:** 2026-05-13
**Tags:** api, performance
**Notes:** Infinite scroll on front end, cursor pagination in API to avoid OFFSET issues

## TODO-005: Set up structured logging with Serilog
**Status:** done
**Priority:** high
**Created:** 2026-04-20
**Tags:** logging, devops
**Notes:** Using Serilog with seq sink for local dev, App Insights for Azure
