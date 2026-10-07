# OLI-it Philosophy

## What OLI-it is

**OLI-it is an open protocol for exchanging meaning, not just data.**

It is intended as fundamental communication infrastructure: simple, public, decentralized and freely implementable.

OLI-it is not a product, a company, an AI platform, or a centralized service. There should be no privileged implementation and no central authority controlling the semantic space.

The ambition is closer to protocols such as HTTP than to an application:

> Define enough common structure that independent participants can understand each other, then get out of the way.

## The three pillars

At the heart of OLI-it are three questions:

**from whom — about what — to whom**

These form the stable semantic structure of the protocol.

They describe communication independently of the technology, application or type of participant involved.

- **FROM** describes the origin or sender.
- **ABOUT** describes what the communication concerns.
- **TO** describes the intended receiver or audience.

These pillars are deliberately fundamental. New concepts should normally be expressed **below them**, rather than by changing the root structure.

This also protects the meaning of existing OLI-it data as the protocol evolves.

## Meaning before addressing

Traditional communication systems are usually concerned with addresses:

> Send this message to this endpoint.

OLI-it is concerned with meaning:

> This comes from someone of this kind, is about these things, and is relevant to someone interested in these things.

The recipient does not necessarily have to be known when a message is created.

Instead, sender, subject and receiver can be described semantically and matched through a shared **Wortraum** — an open semantic space.

Communication therefore becomes a matching problem rather than merely an addressing problem.

## The Wortraum

The Wortraum should not be understood as one centrally maintained ontology.

It is an open and extensible semantic space.

Concepts can form trees, nets and relationships. Participants can use increasingly precise concepts without requiring every implementation to understand the entire world.

The vocabulary should be able to grow organically.

OLI-it should therefore resist the temptation to define a huge universal ontology upfront.

The protocol provides the structure through which meaning can be expressed and discovered. The community of participants provides the meaning that grows inside it.

## Participants are peers

A participant can be a human, an organisation, software, an AI agent, a device, or something we have not invented yet.

The protocol should not fundamentally care.

In particular, **AI does not need its own OLI-it universe.**

An AI agent should participate in the same semantic space as a human or any other participant.

Generic concepts such as:

- Type
- Skill
- Capability
- Purpose
- Intent

can describe participants and communication without coupling the protocol to today's AI technology.

FROM and TO can therefore share much of the same semantic vocabulary.

## OLI-it and AI

AI makes the original OLI-it idea more interesting, but it does not change its foundation.

The goal is not to bolt AI onto OLI-it.

The goal is to allow intelligent agents to become first-class participants in the existing protocol.

An agent may describe what it knows, what it can do, what it wants, what it is looking for, and what it can contribute.

Humans can do the same.

OLI-it can then provide the semantic infrastructure through which these participants discover and communicate with each other.

AI may be very good at interpreting, translating and navigating the Wortraum. But the semantic infrastructure itself should remain open and independent of any particular AI model or provider.

## Matching rather than broadcasting

An important idea in OLI-it is that communication does not have to mean broadcasting information to everyone and hoping that somebody cares.

Both sides can express conditions.

A sender can describe the intended semantic destination of a message.

A receiver can describe what kinds of messages are relevant.

Communication happens where those descriptions match.

This creates a communication model based on **mutual relevance** rather than simply reach.

## Evolution without breaking the past

OLI-it has existed conceptually for a long time. Its vocabulary, implementations and participants will continue to change.

The fundamental protocol should therefore evolve conservatively.

A useful principle is:

> **Extend downward rather than restructure upward.**

If more detail is needed — for example to distinguish humans, organisations and AI agents — introduce that detail beneath existing concepts rather than inserting new structural layers that invalidate existing data.

Old OLI-it information should remain meaningful even when newer participants have much richer descriptions.

## Minimal infrastructure

OLI-it should contain as little policy as possible.

It should define enough structure to make semantic communication possible, while leaving applications free to decide how they:

- represent themselves,
- construct their Wortraum,
- publish information,
- discover information,
- perform matching,
- establish trust,
- communicate after a match.

The protocol should enable ecosystems rather than attempt to become the ecosystem.

## Historical continuity

The early OLI-it concepts already contained this basic idea.

Models involving concepts such as **Stamm**, **Angler**, **PostIt**, **Code** and **TopLab** described a loop in which information was semantically represented, matched and returned to an interested participant.

Later implementations and terminology may look very different.

The underlying idea remains:

**Describe meaning independently of sender, receiver and transport so that relevant participants can find one another.**

## Design compass

When extending OLI-it, prefer designs that are:

**Open** — anyone can implement and participate.

**Decentralized** — no central company, server, AI provider or ontology owns the protocol.

**Semantic** — communicate meaning rather than merely addresses and payloads.

**Symmetric** — humans, organisations, software and agents participate through the same fundamental concepts.

**Extensible** — new concepts grow beneath stable foundations.

**Backward compatible** — old information remains meaningful.

**Minimal** — put only what must be common into the protocol.

**Organic** — allow the Wortraum and its vocabulary to evolve through use.

And above all, preserve the simplest expression of OLI-it:

# from whom — about what — to whom
