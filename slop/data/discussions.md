# Discussions

## DIS-001: Cache Stamm profiles separately from Netz hierarchy?
**Status:** open
**Created:** 2026-05-08
**Tags:** caching, architecture
**Notes:** WortraumCacheService currently caches the whole Netz/Baum hierarchy together. Should Stamm profiles get their own TTL since they change more frequently?

## DIS-002: API versioning strategy
**Status:** in-progress
**Created:** 2026-05-10
**Tags:** api, architecture
**Notes:** URL-based (/api/v1/) vs header-based (Accept-Version). Leaning URL prefix since it's simpler for Razor Pages fetch calls and easier to test in browser

## DIS-003: Image CDN vs direct Blob Storage serving
**Status:** open
**Created:** 2026-05-12
**Tags:** images, performance, azure
**Notes:** Azure CDN in front of Blob Storage would help thumbnail load times globally. Worth the extra infra cost and complexity at current scale?
