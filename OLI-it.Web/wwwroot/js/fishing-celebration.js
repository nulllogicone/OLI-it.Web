(() => {
    "use strict";

    const preferenceKey = "oliit.fishing.enabled";
    const motion = window.matchMedia("(prefers-reduced-motion: reduce)");
    // Storage may be unavailable in private browsing. The controls still work for this page.
    const read = (storageName, key) => {
        try { return window[storageName].getItem(key); } catch { return null; }
    };
    const write = (storageName, key, value) => {
        try { window[storageName].setItem(key, value); } catch { /* Optional preference only. */ }
    };

    document.querySelectorAll("[data-fishing-celebration]").forEach(root => {
        const toggle = root.querySelector("[data-fishing-enabled]");
        const replay = root.querySelector("[data-fishing-replay]");
        const status = root.querySelector("[data-fishing-status]");
        const matches = root.dataset.matchKeys.split(",").filter(Boolean);
        const seenKey = `oliit.fishing.seen.${root.dataset.anglerKey}`;
        const seen = new Set((read("sessionStorage", seenKey) || "").split(",").filter(Boolean));
        let timer;

        const stop = () => {
            clearTimeout(timer);
            root.classList.remove("is-celebrating");
        };
        const celebrate = () => {
            if (!toggle.checked || matches.length === 0) return;
            stop();
            status.textContent = "You've got a bite! A PostIt has found its Angler.";
            matches.forEach(key => seen.add(key));
            // Keep only the current result window; avoid growing storage indefinitely.
            write("sessionStorage", seenKey, matches.join(","));
            if (!motion.matches) {
                // Restart the finite animation when replay is clicked.
                void root.offsetWidth;
                root.classList.add("is-celebrating");
                timer = setTimeout(stop, 1800);
            }
        };

        toggle.checked = read("localStorage", preferenceKey) === "true";
        replay.disabled = !toggle.checked || matches.length === 0;
        root.querySelector(".fishing-controls").hidden = false;
        toggle.addEventListener("change", () => {
            write("localStorage", preferenceKey, String(toggle.checked));
            replay.disabled = !toggle.checked || matches.length === 0;
            if (toggle.checked) celebrate();
            else {
                stop();
                status.textContent = "Catch celebrations are off. Your matches are still here.";
            }
        });
        replay.addEventListener("click", celebrate);
        motion.addEventListener("change", stop);
        if (toggle.checked && matches.some(key => !seen.has(key))) celebrate();
    });
})();
