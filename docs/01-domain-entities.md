# Domain Entities

Last updated: 2026-03-26
Status: draft

## Source

Derived from:
- Paper: "Soulmate" by Frederic Luchting, ISWC 2011
- Live application: https://www.oli-it.com
- Existing database with German table names

---

## Database Entity Mapping

The actual database uses German naming from the original implementation. See `02-data-model.md` for the complete mapping table. Key references:

- **Stamm** = User
- **Angler** = Filter Profile  
- **PostIt** = Message
- **Code** = Description
- **TopLab** = Response/Answer
- **Netz** = Net, **Knoten** = Node
- **Baum** = Tree, **Zweig** = Branch
- **Olis** = Message Marking, **Ilos** = Filter Marking
- **get** = Receiver Threshold, **fit** = Sender Threshold

---

## Glossary

| Term | Definition |
|------|-----------|
| 0L1 / OLI | The open messaging protocol this application implements |
| Author | User who creates and sends a message (also called Sender) |
| Recipient | User who receives matched messages via filter profiles |
| Wordspace | Hierarchical semantic space used to describe author, message, and recipient properties |
| Net | A wordspace structure with a collection of nodes (non-reusable within wordspace) |
| Tree | A wordspace structure with disjoint branches (reusable within wordspace) |
| Node | An element inside a net |
| Branch | An element inside a tree (mutually exclusive alternatives) |
| Description | A sender's full marking in the wordspace: self, message content, desired recipient |
| Filter Profile | A recipient's marking in the wordspace describing what messages they want |
| Criterion / Marking | A wordspace element marked with first-value and second-value by author or recipient |
| First Value | Controls strictness: 3 = strict (once per structure), 2 = flexible (multiple), 1 = exclude |
| Second Value | Controls what the other side must have: 3 = must match, 2 = at least one in structure, 0 = voluntary/discoverable |
| Matchmaking | The algorithm that evaluates all sender descriptions against all filter profiles and delivers a message when all mutual requirements are met |
| Answer / Reply | A response submitted by a logged-in user who can view the message |
| Rating | Evaluation score given to an answer |
| Credit / Transaction | Monetary/point value exchange resulting from messages, answers, and ratings |
| Journal | Chronological view of all messages |
| Chart | Ranking view sorted by points or money |

---

## Entity Catalog

### ENT-User (Stamm)

A registered person using the platform. Can act as author (sender) or recipient — both roles use the same account.

**Database table:** `Stamm`

| Attribute | Type | Notes |
|-----------|------|-------|
| UserId | GUID/int | PK |
| Username | string | unique login name |
| DisplayName | string | public display |
| Email | string | unique |
| PasswordHash | string | hashed, never plaintext |
| Language | enum | EN, DE, ES |
| CreditBalance | decimal | current earned/spent balance |
| IsActive | bool | soft-disable |
| CreatedAt | datetime | |
| UpdatedAt | datetime | |

Relationships:
- Creates many Messages (as author)
- Has many FilterProfiles
- Submits many Answers
- Gives many Ratings
- Has many Transactions

---

### ENT-WordspaceNode (Knoten)

A node inside a **Net**. Represents a concept in the wordspace (e.g., `author`, `message`, `recipient` at top-level).

**Database table:** `Knoten`

| Attribute | Type | Notes |
|-----------|------|-------|
| NodeId | int | PK |
| NetId | int | FK → ENT-WordspaceNet |
| Name | string | |
| DisplayOrder | int | ordering within net |
| ParentNodeId | int? | supports hierarchy within a net |

---

### ENT-WordspaceNet (Netz)

A net groups related nodes. Top-level structure. Non-reusable (nodes belong to one net).

**Database table:** `Netz`

| Attribute | Type | Notes |
|-----------|------|-------|
| NetId | int | PK |
| Name | string | e.g. "root", "message-topics" |
| Description | string | |
| IsRoot | bool | top-level net (author/message/recipient) |

---

### ENT-WordspaceTree (Baum)

A tree is reusable within the wordspace and has disjoint branches (e.g., sex tree with branches male/female).

**Database table:** `Baum`

| Attribute | Type | Notes |
|-----------|------|-------|
| TreeId | int | PK |
| Name | string | e.g. "sex", "location", "course of studies" |
| Description | string | |

---

### ENT-WordspaceBranch (Zweig)

A branch inside a tree. Branches within the same tree are mutually exclusive alternatives.

**Database table:** `Zweig`

| Attribute | Type | Notes |
|-----------|------|-------|
| BranchId | int | PK |
| TreeId | int | FK → ENT-WordspaceTree |
| Name | string | e.g. "male", "female", "physics", "statistics" |
| DisplayOrder | int | |
| ParentBranchId | int? | supports hierarchical sub-branches |

---

### ENT-Message (PostIt)

The core business object. Created by an author with a full description (self + content + recipient criteria).

**Database table:** `PostIt`

| Attribute | Type | Notes |
|-----------|------|-------|
| MessageId | GUID/int | PK |
| AuthorId | int | FK → ENT-User |
| Title | string | short summary |
| Content | string | message body |
| Value | decimal | positive (recipient paid to receive) or negative (author pays) |
| TimeLimit | datetime? | optional deadline for answers |
| IsActive | bool | soft-delete |
| CreatedAt | datetime | |
| UpdatedAt | datetime | |

Relationships:
- Has one Description (set of wordspace markings)
- Has many Answers

---

### ENT-Description (Code)

The wordspace markings made by the author when composing a message. Encodes: who the author is, what the message is about, and what recipient is desired.

**Database table:** `Code`

| Attribute | Type | Notes |
|-----------|------|-------|
| DescriptionId | int | PK |
| MessageId | int | FK → ENT-Message |

Resolved via ENT-Criterion (the individual marked elements).

---

### ENT-FilterProfile (Angler)

A recipient's filter specification. A user can have multiple filter profiles. A message is delivered to a recipient if at least one filter profile matches the message's description.

**Database table:** `Angler`

| Attribute | Type | Notes |
|-----------|------|-------|
| FilterProfileId | int | PK |
| UserId | int | FK → ENT-User |
| Name | string | user-defined label for the profile |
| IsActive | bool | |
| CreatedAt | datetime | |

Resolved via ENT-Criterion (the individual marked filter elements).

---

### ENT-Criterion (Olis, Ilos, get, fit)

A single marking in either a Description or a FilterProfile. Marks a WordspaceNode or WordspaceBranch with a first-value and second-value.

**Database tables:**
- `Olis` = Message markings (sender criteria)
- `Ilos` = Filter markings (receiver criteria)
- `get` = Receiver threshold values
- `fit` = Sender threshold values

| Attribute | Type | Notes |
|-----------|------|-------|
| CriterionId | int | PK |
| ContextType | enum | Description or FilterProfile |
| ContextId | int | FK → DescriptionId or FilterProfileId |
| ElementType | enum | Node or Branch |
| ElementId | int | FK → NodeId or BranchId |
| FirstValue | int | 1, 2, or 3 (see rules) |
| SecondValue | int | 0, 2, or 3 (see rules) |

---

### ENT-Answer (TopLab)

A reply submitted by a logged-in user who can view the message (including self-answer by the message author).

**Database table:** `TopLab`

| Attribute | Type | Notes |
|-----------|------|-------|
| AnswerId | int | PK |
| MessageId | int | FK → ENT-Message |
| AuthorId | int | FK → ENT-User (the answerer) |
| Content | string | answer text |
| CreatedAt | datetime | |
| IsActive | bool | soft-delete |

---

### ENT-Rating

An evaluation given by the message author (or recipient) for an answer.

| Attribute | Type | Notes |
|-----------|------|-------|
| RatingId | int | PK |
| AnswerId | int | FK → ENT-Answer |
| GivenByUserId | int | FK → ENT-User |
| Score | int | numeric rating |
| CreatedAt | datetime | |

---

### ENT-Transaction

A credit/money movement triggered by message delivery, answer submission, or rating.

| Attribute | Type | Notes |
|-----------|------|-------|
| TransactionId | int | PK |
| UserId | int | FK → ENT-User |
| RelatedMessageId | int? | |
| RelatedAnswerId | int? | |
| Amount | decimal | positive = credit, negative = debit |
| Reason | enum | MessageDelivery, AnswerReward, Rating, etc. |
| CreatedAt | datetime | |

---

## Relationships Summary

```
User ──< Message (authoring)
User ──< FilterProfile
User ──< Answer (answering)
User ──< Rating (giving)
User ──< Transaction

Message ──┤ Description ──< Criterion → Node/Branch
FilterProfile ──< Criterion → Node/Branch
Message ──< Answer ──< Rating

WordspaceNet ──< WordspaceNode
WordspaceTree ──< WordspaceBranch (hierarchical)
```

---

## Matchmaking Rules (from paper)

| First Value | Second Value | Behavior |
|-------------|-------------|---------|
| 3 | 3 | Strict: choose only one element in the structure (e.g. only one course); other side must exactly match |
| 3 | 2 | Strict send, flexible receive: other side must have at least one in same structure |
| 2 | 2 | Flexible both sides: multiple criteria allowed, at least one must match |
| 2 | 0 | Voluntary: marks discoverability, no requirement imposed on other side |
| 1 | — | Exclusion: if other side marked this criterion, no match |

Algorithm: for each (Description, FilterProfile) pair, both sides check their criteria. Message is delivered only if all constraints on both sides are satisfied.

---

## Business Rules

- A message is only delivered when **all** mutual criteria are satisfied by both sides.
- First value 3 can be used **only once** per net or tree structure.
- First value 1 means **exclusion**: if the other side marked this criterion, the pair does not match.
- A user can have **multiple filter profiles**; delivery occurs if **any one** profile matches.
- Bound- and Flow-KooK can take any value (positive or negative); negative values indicate reversed incentive direction for message flow.
- Soft-delete preferred over hard-delete for messages, answers, and users.
- Matchmaking should run efficiently; consider async/background processing for large sets.

---

## Open Validation Items

- Confirm DB schema from legacy system vs. paper model
- Clarify how wordspace trees are attached/referenced within nets
- Clarify exact credit/reward computation rules
- Determine if matchmaking runs on-write (eager) or on-request (lazy)
- Confirm multi-language support scope for wordspace node labels

## Change Log

- 2026-03-26: Initial full entity catalog derived from ISWC 2011 paper and live site review.
- 2026-04-08: Updated answer/toplab actor rule per OQ-002.
- 2026-04-08: Updated credit semantics per OQ-004 (Bound-/Flow-KooK signed values, negative = reversed incentive direction).
