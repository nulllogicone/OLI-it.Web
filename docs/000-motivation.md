# OLI-it.Web — Motivation

The new OLI-it.Web was created because **open-claw** (Copilot agent) had trouble operating the old ASP.NET WebForms UI — ViewState, postbacks, and classic server-rendered controls made it unreliable for agent-driven interaction.

## Goals

- Replace legacy WebForms with modern **ASP.NET Core Razor Pages** so Copilot agents can reliably read and manipulate the UI. And also propose changes (feature requests) within this repository. Most simple is to add a new thing to the backlog.md. 

- Support the two core domains:
  - **SAPCT** — message flow (PostIt / TopLab)
  - **NKBZ Wortraum** — wordspace network (Netz / Knoten / Baum / Zweig)

## Key Principle

> The UI must be agent-friendly: clean HTML, no hidden ViewState, predictable DOM structure.
