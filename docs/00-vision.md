# Vision

Last updated: 2026-03-26
Status: draft

## Background

OLI-it (0L1) is an open messaging protocol and platform conceived and built by Frederic Luchting.
The core idea: connect any sender with any recipient by matching semantic descriptions against filter profiles in a hierarchical wordspace — without requiring prior acquaintance between parties.

The application has been running since 1994 and is live at https://www.oli-it.com.
The current implementation is ASP.NET WebForms. This project is a full rewrite to modern .NET (ASP.NET Core Razor Pages + Entity Framework Core + SQL Server).

## What is 0L1?

A generic open messaging protocol in which:
- A **sender (author)** describes themselves, the message content, and the desired recipient properties using marked nodes in a hierarchical semantic wordspace.
- A **recipient** specifies one or more filter profiles with complementary descriptions.
- The **matchmaking algorithm** delivers a message only when all mutual requirements of both sides are fulfilled — ensuring a very high hit quality.

Use cases span any domain: Q&A, classified ads, partner search (personal/professional), news, commerce, and more.

## Problem Statement

The legacy WebForms implementation is fully functional but:
- Built on an outdated framework (ASP.NET WebForms)
- Difficult to extend, test, and maintain
- Cannot easily benefit from modern .NET ecosystem improvements
- UX is dated and not mobile-aware

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
- Wordspace (nodes, trees, nets, branches) is modeled and managed via EF Core
- Users can register, log in, create messages, manage filter profiles, answer messages, rate answers, and earn/spend credits
- Multi-language support (EN, DE, ES) preserved
- RSS feed and public journal/chart views reproduced
- Deployment is reproducible and documented

## Scope — Phase 1 (MVP Parity)

- Authentication: register, login, logout
- Wordspace management (admin: nodes, trees, nets, branches)
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

## Key Reference Material

- Paper: "Soulmate" by Frederic Luchting, ISWC 2011 — defines protocol and entity model
- Live system: https://www.oli-it.com (source of behavioral truth)
- Legacy codebase: ASP.NET WebForms (behavioral reference, not code reference)

## Change Log

- 2026-03-26: Initial draft from paper analysis and live site review.
