# Research and implementation references

Updated: 2026-10-03  
Status: draft; paper inventory, not a completed paper review

## Papers located locally

| Candidate | Location | Review status |
|---|---|---|
| ISWC 2011 submission | `OLI-it.Web/wwwroot/iswc2011outrageousid_submission_13.pdf` in this repository | Located; not read in access session |
| Soulmate | `Unternehmung/soulmate.pdf` in the legacy repository | Located; not read |
| 0L1 | `Unternehmung/Dokumentation/ISWC2011Outrageous/0L1.pdf` in the legacy repository | Located; not read |
| 2006 thesis on RDF message mediation | `Anwendung/OLIWeb/2006 Bachelorarbeit kathrin dentler NachrichtenVermittlung RDF OLI-it.pdf` in the legacy repository | Located; not read |

Legacy repository: `C:/Users/luchtfr/source/repos/DevOps/nulllogicone/OLI-it`. Some papers have duplicate copies; identify versions before citing. Do not copy private business documents into this public repository.

For each relevant claim, record paper title/version and page, intended semantic rule, corresponding code/SQL, and a reproducible matching example. Keep historical design intent separate from deployed behavior and proposed changes.

## Existing project explanations

- [Domain entities](../010-domain-entities.md): SAPCT and marking vocabulary.
- [Data model](../020-data-model.md): schema mappings.
- [SQL matching explanation](../065-magic-match-logic.md): `oli.beissen`, `oli.fischen`, mutual constraints and materialized matches.
- [Use cases](../030-use-cases.md) and [UI architecture](../040-ui-ia.md): compare with live behavior.

The matching document leaves exclusion/NOT semantics unresolved. Treat that as a research question; verify papers, procedures and other callers before proposing changes to OgIf.
