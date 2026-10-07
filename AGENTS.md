# Project documentation and work planning

- Keep internal project knowledge, architecture decisions, domain explanations, and the long-term overview in version-controlled Markdown files in this repository.
- Start with `docs/000-motivation.md` and `docs/README.md` for project context. Record architecture decisions in `docs/070-decisions/`.
- Use GitHub issues for public feedback and discussion: https://github.com/nulllogicone/OLI-it.Web/issues. Keep internal specifications, ideas, and todos in docs/; link relevant public issues rather than duplicate them.
- Link documentation and issues to each other when relevant. Avoid duplicating the issue backlog in Markdown; documentation should explain the system and durable decisions.
- Carry durable conclusions from chats into the appropriate documentation. Keep documented status consistent with verified implementation and validation results.
- Before publishing documentation or issue content, check that it contains no secrets, credentials, or private information. Version-controlled documentation in this public repository is publicly visible when pushed.

## Local notes and human review

- Publish project knowledge by default. Use local-notes/ beside docs/ only for deliberately private notes or temporary experiments, such as machine-specific access and permission tests. Move useful, shareable conclusions into docs/.
- local-notes/ is excluded in this checkout through .git/info/exclude. Verify the exclusion before writing private notes in a new clone or worktree; never force-add these files. Do not store passwords, API keys, or tokens there.
- Never commit or push unless the user explicitly requests that action. Preparing edits or switching branches does not authorize committing or pushing. Leave changes available for the user's review and commit/push checkpoint.