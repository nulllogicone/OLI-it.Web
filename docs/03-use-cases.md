# Use Cases

Last updated: 2026-03-26
Status: draft

## Format

> As a [role], I want [goal], so that [value].  
> Acceptance criteria listed per use case.  
> ID format: UC-NNN

---

## UC-001 — Register

**As a** visitor,  
**I want** to create an account,  
**so that** I can use the platform as author and recipient.

Acceptance criteria:
- Username, email, password, language required
- Username and email must be unique
- Password is hashed on storage
- User is redirected to journal/homepage after registration

---

## UC-002 — Log In / Log Out

**As a** registered user,  
**I want** to log in and log out,  
**so that** my session is secure and my data is private.

Acceptance criteria:
- Login by username + password
- Session expires after inactivity
- Logout clears session
- Incorrect credentials return a non-descriptive error (no username enumeration)

---

## UC-003 — Create Message

**As an** author (logged-in user),  
**I want** to compose a message with a title, content, value, optional time limit, and a wordspace description,  
**so that** matched recipients can discover and receive it.

Acceptance criteria:
- Author fills title, content, Bound-/Flow-KooK value (any signed value), optional time limit
- Negative value means incentive direction for message flow is reversed
- Author navigates the wordspace and marks criteria (nodes/branches) with first-value and second-value
- Description saved as a set of Criteria records
- Message appears in the Journal after creation
- Author can edit a message while no answers exist

---

## UC-004 — Manage Filter Profiles

**As a** recipient (logged-in user),  
**I want** to create and manage filter profiles,  
**so that** I receive only messages matching my interests.

Acceptance criteria:
- User can create multiple named filter profiles
- Each profile is built by navigating the wordspace and marking criteria
- User can activate/deactivate individual profiles
- User can edit or delete profiles

---

## UC-005 — Receive Matched Messages

**As a** recipient,  
**I want** to see messages that match my filter profiles,  
**so that** I find relevant content without noise.

Acceptance criteria:
- Matchmaking algorithm is applied between all active messages and user's active filter profiles
- Only messages satisfying all mutual criteria are shown
- Delivered messages appear in the user's personal inbox/feed
- No message is shown twice for the same delivery match

---

## UC-006 — Answer a Message

**As a** recipient,  
**I want** to reply to a message I received,  
**so that** I can respond to the author's request or question.

Acceptance criteria:
- Any logged-in user who can view a message can answer
- Self-answer is allowed (the message author can also answer)
- Answer text field required
- Answer visible to the message author
- Multiple answers from different recipients allowed
- Author can view all answers to their messages

---

## UC-007 — Rate an Answer

**As a** message author,  
**I want** to rate answers I received,  
**so that** good answers are rewarded and quality is visible.

Acceptance criteria:
- Author can rate each answer once with a numeric score
- Rating triggers a credit transaction for the answerer
- Rating is visible on the answer

---

## UC-008 — View Journal

**As a** visitor or user,  
**I want** to browse the journal of messages chronologically,  
**so that** I can see the latest activity on the platform.

Acceptance criteria:
- All active messages shown in reverse chronological order
- Public access (read-only, no login required)
- Pagination supported

---

## UC-009 — View Charts

**As a** visitor or user,  
**I want** to view a ranking of messages and users by points or money,  
**so that** I can see what is most valued on the platform.

Acceptance criteria:
- Rankings sortable by points and by money
- Message ranking shows title, author, score
- User ranking shows username, balance/points

---

## UC-010 — Search

**As a** visitor or user,  
**I want** to search for users, messages, and answers by keyword,  
**so that** I can find specific content quickly.

Acceptance criteria:
- Search box on main navigation
- Results separated by type: messages, answers, users
- Search is case-insensitive
- Results link to detail pages

---

## UC-011 — View Credit Balance and Transactions

**As a** logged-in user,  
**I want** to see my credit balance and transaction history,  
**so that** I know how much I have earned and spent.

Acceptance criteria:
- Balance displayed in user profile/header
- Transaction list with date, amount, reason, and related entity

---

## UC-012 — Administer Wordspace (Admin)

**As an** administrator,  
**I want** to manage the wordspace (add/edit/remove nets, nodes, trees, branches),  
**so that** the semantic vocabulary grows with platform needs.

Acceptance criteria:
- Admin-only access
- CRUD for nets and their nodes (with hierarchy)
- CRUD for trees and their branches (with hierarchy and ordering)
- Changes reflected immediately in message authoring and filter profile UI

---

## Change Log

- 2026-03-26: Initial 12 use cases from paper + live site analysis.
- 2026-04-08: Updated UC-006 answer rule per OQ-002 (logged-in viewer may answer; self-answer allowed).
- 2026-04-08: Updated UC-003 value semantics per OQ-004 (Bound-/Flow-KooK signed values; negative reverses incentive direction).
