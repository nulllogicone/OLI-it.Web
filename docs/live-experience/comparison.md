# Experience comparison

Updated: 2026-10-21  
Status: draft; creation verified historically, end-to-end flow pending

Use **verified**, **partial**, **blocked**, or **not tested** with evidence. Distinguish a missing feature from a feature that has not been explored.

| User intent | Legacy evidence | Modern evidence | Conclusion |
|---|---|---|---|
| Open public home | Verified | Verified | Both reachable |
| Sign in | Verified on later retry: profile plus log out; initial attempt returned 502 | Verified: profile plus Logout | Both sign-ins established; earlier legacy error cause unknown |
| Understand the product through an example | Random message/answer visible | No equivalent on observed home | Design question; not yet an accepted requirement |
| Create Stamm | Not tested | Not tested | Pending |
| Create PostIt and initial Code | Verified on 2026-10-03: message saved and default Code created; semantic marking not tested | Verified in Azure test on 2026-10-04: message saved, initial Code and 1.01 message credit displayed | Creation gap addressed by PR #49; see [journey continuation](sessions/2026-10-21-message-flow.md) |
| Update PostIt content | Not compared side by side | Verified locally with text round trips; SQL/handler suite (11 tests) passed locally and in CI | Live update comparison remains pending |
| Describe and send | Existing UI observed; semantics not validated | No new description/send implementation included in PR #49 | Deferred pending shared understanding of NKBZ/OgIf |
| Manage Angler and inspect delivery | Not tested | Not tested | Pending |
| Answer using TopLab | Not tested | Not tested | Pending |
| Navigate/maintain NKBZ | Not tested | Home navigation link observed; not tested | Pending |
| Validate OgIf semantics | Not tested | Not tested | Requires paper and implementation analysis |

Baseline source: [access session](sessions/2026-10-03-access.md). Add issue links only after findings have enough evidence and have been checked against existing issues.
