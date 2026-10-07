# Project documentation and work planning

- Keep internal project knowledge, architecture decisions, domain explanations, and the long-term overview in version-controlled Markdown files in this repository.
- Start with `docs/000-motivation.md` and `docs/README.md` for project context. Record architecture decisions in `docs/070-decisions/`.
- Use GitHub issues for public feedback and discussion: https://github.com/nulllogicone/OLI-it.Web/issues. Keep internal specifications, ideas, and todos in docs/; link relevant public issues rather than duplicate them.
- Link documentation and issues to each other when relevant. Avoid duplicating the issue backlog in Markdown; documentation should explain the system and durable decisions.
- Carry durable conclusions from chats into the appropriate documentation. Keep documented status consistent with verified implementation and validation results.
- Before publishing documentation or issue content, check that it contains no secrets, credentials, or private information. Version-controlled documentation in this public repository is publicly visible when pushed.

## Documentation structure and linking

- Give each document a clear purpose and abstraction level; record its role in `docs/README.md`. Keep complementary documents rather than merging them solely because their subjects overlap.
- Maintain one authoritative home for each definition or detailed explanation. Higher-level documents should use a short summary and link a meaningful word or phrase to that home, preferably to the relevant heading, instead of repeating the explanation.
- Keep enough context for each document to be readable on its own. Brief reminders and local compatibility constraints are useful; duplicated definitions, procedures, and status lists are not. Use relative Markdown links and verify their targets after editing.
- Use `docs/philosophy.md` for protocol principles, `docs/000-motivation.md` for migration rationale and current transition context, `docs/001-vision.md` for application goals and success criteria, `docs/010-domain-entities.md` for conceptual entities and rules, and `docs/020-data-model.md` for SQL/EF naming and compatibility. Link to more specific documents for implementation details.
- Distinguish protocol aspirations, application plans, accepted decisions, and verified implementation. Do not turn a documentation cleanup into a scope or behavior change; flag disagreements for reconciliation.
- When adding knowledge, update its owning document and link from related documents. Do not create another parallel explanation or a specialized agent merely to enforce this rule.

## Local notes and human review

- Publish project knowledge by default. Use local-notes/ beside docs/ only for deliberately private notes or temporary experiments, such as machine-specific access and permission tests. Move useful, shareable conclusions into docs/.
- local-notes/ is excluded in this checkout through .git/info/exclude. Verify the exclusion before writing private notes in a new clone or worktree; never force-add these files. Do not store passwords, API keys, or tokens there.
- Never commit or push unless the user explicitly requests that action. Preparing edits or switching branches does not authorize committing or pushing. Leave changes available for the user's review and commit/push checkpoint.