# Optional fishing celebration

Status: experiment; online review pending
Updated: 2026-10-21
Review tracking: [issue #52](https://github.com/nulllogicone/OLI-it.Web/issues/52). Original idea: [issue #47](https://github.com/nulllogicone/OLI-it.Web/issues/47).

## Behavior

The Angler detail page offers a small catch celebration when its existing result list contains matches. It is off by default. Enabling the checkbox celebrates the current catches and saves the preference in browser local storage. A replay button lets the user repeat it or try it with reduced motion enabled.

On later page visits with celebrations enabled, the script celebrates if the currently loaded results include a PostIt not previously celebrated in this tab for that Angler. Distinct PostIt identifiers avoid treating multiple matching Codes for one PostIt as separate catches. The remembered identifiers are bounded to the current result window (the page already loads at most 50 Spiegel rows). This is page-load feedback, not a live notification or proof that a match was just created. Returning matches that left the remembered window can celebrate again. Blocked browser storage degrades to page-local controls.

The animation lasts 1.6 seconds, has no sound or flashing, and is contained in the card. Both CSS and JavaScript respect `prefers-reduced-motion`: the catch message remains available without animation. Disabling celebrations stops the animation. Decorative elements are hidden from assistive technology; feedback uses a polite status region. Controls remain hidden when JavaScript is unavailable.

No database, schema, matching procedure, authentication, or message-writing behavior changes. The same partial and script power the Angler integration and sample page.

## Review without creating messages

Run the app and open `/experiments/fishing-celebration`. The page uses a fixed sample catch and makes no database queries. A link is also available on `/design-overview`.

For local review: <http://localhost:5187/experiments/fishing-celebration> while the development server is running.

After a successful feature-branch deployment, the expected shared preview URL is <https://oliitrazorweb-test.azurewebsites.net/experiments/fishing-celebration>. This URL is not evidence that the feature has been deployed.

## Branch deployment and review

Feature branch: `codex/47-fishing-celebration`, based on `origin/main` at `8d9ff93`.

The application workflow builds and deploys qualifying branch pushes to the shared `test` slot. Pull-request events build and test without deployment. All branches share that slot, so another deployment can replace this experiment. Production deployment requires a non-pull-request run on `main`; the feature branch does not meet that condition.

Review the finished changes before committing and pushing, as required by the repository contributor instructions. After deployment, record the actual commit and workflow run before inviting online review. Keep review issue #52 open while evaluating; this experiment does not establish acceptance of a permanent feature.

## Validation

- Release build succeeded, including Razor compilation.
- All 11 `PostItCreationTests` passed (the current deployment workflow's test selection).
- `node --check` passed for the new script.
- Browser review of the sample page verified the default off state, opt-in feedback, replay, disabling, and persistence across reloads. Reloading an already celebrated sample did not repeat the feedback.
- Reduced-motion CSS/JavaScript and responsive CSS were inspected; operating-system reduced-motion emulation and a narrow viewport were not exercised.
- Live Angler data integration compiled but has not been exercised against hosted data.
- Build output includes existing package vulnerability warnings for `System.Security.Cryptography.Pkcs` and existing Razor Page/compiler warnings; dependency changes are outside this UI experiment.
