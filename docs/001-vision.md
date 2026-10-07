# OLI-it.Web Application Vision

Last updated: 2026-10-07
Status: draft

## Purpose

This document owns the application's intended outcomes, success criteria, and phased feature goals. OLI-it.Web is an implementation of the protocol, not the protocol itself; its foundational principles live in the [philosophy](philosophy.md).

The phase lists below retain the original draft goals, not verified delivery status. For current migration boundaries and known scope disagreements, see the [transition overview](000-motivation.md).

## Background

OLI-it (0L1) is an open messaging protocol conceived by Frederic Luchting. It connects participants through [mutual relevance](philosophy.md#matching-rather-than-broadcasting), without requiring prior acquaintance.

The application has been running since 1994 and is live at https://www.oli-it.com.
The current implementation is ASP.NET WebForms. This project is a full rewrite to modern .NET (ASP.NET Core Razor Pages + Entity Framework Core + SQL Server).

The [migration rationale](000-motivation.md#development-goals) explains why this application is being modernized. The [use cases](030-use-cases.md) describe its user-facing workflows.

Potential domains include Q&A, classified ads, personal or professional partner search, news, and commerce.

## Product Goal

Deliver a modernized equivalent of the existing live application on ASP.NET Core Razor Pages + EF Core, with:
1. Full feature parity for core workflows
2. Clean, layered domain model reflecting the 0L1 protocol semantics
3. Improved developer experience and maintainability
4. Path to incremental UX improvements

## Success Criteria

- All critical workflows from the live app are reproduced and verified
- Existing data can still be used from existing sql server
- Matchmaking algorithm logic is implemented in sql server and can be used unchanged
- Users can register, log in, create messages, manage filter profiles, answer messages, rate answers, and earn/spend credits
- Multi-language support (EN, DE, ES) preserved
- Public journal/chart views reproduced
- Deployment is reproducible and documented

## Scope — Phase 1 (MVP Parity)

- Authentication: register, login, logout
- Message authoring (description: author self-description, message content, recipient criteria with first/second values)
- Filter profile management (per user, multiple profiles allowed)
- Matchmaking: run matching of descriptions vs. filter profiles
- Message delivery: matched messages visible to recipient
- Answer/reply to messages
- Rating of answers
- Credit/reward transactions
- Journal (chronological message timeline)
- Charts (ranking by points/money)
- Search (users, messages, answers)
- Multi-language (EN, DE, ES)
- RSS feed output

## Scope — Phase 2 (Enhancements)

- Improved UX / mobile-responsive layout
- Advanced wordspace editing UI
- Notification system
- Admin dashboard for moderation
- Analytics / reporting
- API exposure for third-party clients

## Non-Goals (Phase 1)

- Redesigning the core messaging protocol
- Changing the wordspace semantics
- Microservice splitting
- Native mobile apps

## Agent-Driven Development

See [agent operability](000-motivation.md#development-goals) for the migration's human- and agent-facing usability goals.

## Key Reference Material

- Paper: "Soulmate" by Frederic Luchting, ISWC 2011 — defines protocol and entity model
- Live system: https://www.oli-it.com (source of behavioral truth)
- Legacy codebase: ASP.NET WebForms (behavioral reference, not code reference)

## Change Log

- 2026-10-07: Clarified application ownership and linked protocol principles and migration rationale instead of repeating them; retained draft feature goals.
- 2026-03-26: Initial draft from paper analysis and live site review.
