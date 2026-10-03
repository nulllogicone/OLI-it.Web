# Experience comparison

Updated: 2026-10-03  
Status: draft; access baseline only

Use **verified**, **partial**, **blocked**, or **not tested** with evidence. Distinguish a missing feature from a feature that has not been explored.

| User intent | Legacy evidence | Modern evidence | Conclusion |
|---|---|---|---|
| Open public home | Verified | Verified | Both reachable |
| Sign in | Verified on later retry: profile plus log out; initial attempt returned 502 | Verified: profile plus Logout | Both sign-ins established; earlier legacy error cause unknown |
| Understand the product through an example | Random message/answer visible | No equivalent on observed home | Design question; not yet an accepted requirement |
| Create Stamm | Not tested | Not tested | Pending |
| Create PostIt and Code | Verified: message saved and default Code created; semantic marking not tested | Blocked: no creation control observed; local editor only updates existing messages | [Message creation session](sessions/2026-10-03-message-creation.md); modern creation gap |
| Manage Angler and inspect delivery | Not tested | Not tested | Pending |
| Answer using TopLab | Not tested | Not tested | Pending |
| Navigate/maintain NKBZ | Not tested | Home navigation link observed; not tested | Pending |
| Validate OgIf semantics | Not tested | Not tested | Requires paper and implementation analysis |

Baseline source: [access session](sessions/2026-10-03-access.md). Add issue links only after findings have enough evidence and have been checked against existing issues.
