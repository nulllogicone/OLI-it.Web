document.addEventListener("DOMContentLoaded", () => {
    for (const toggle of document.querySelectorAll("[data-toggle-target]")) {
        toggle.addEventListener("click", () => {
            const targetId = toggle.getAttribute("data-toggle-target");
            const target = document.getElementById(targetId);
            if (!target) {
                return;
            }

            target.classList.toggle("open");
        });
    }

    const filterButtons = document.querySelectorAll("[data-filter]");
    if (filterButtons.length > 0) {
        filterButtons.forEach((button) => {
            button.addEventListener("click", () => {
                const filter = button.getAttribute("data-filter");
                document.querySelectorAll("[data-card]").forEach((card) => {
                    const type = card.getAttribute("data-card");
                    const shouldShow = filter === "all" || type === filter;
                    card.style.display = shouldShow ? "" : "none";
                });
            });
        });
    }
});
