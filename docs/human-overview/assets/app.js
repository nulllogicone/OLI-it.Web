const MARKDOWN_VIEWER_PAGE = "markdown-viewer.html";

function escapeHtml(value) {
    return value
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll("\"", "&quot;")
        .replaceAll("'", "&#39;");
}

function slugify(value) {
    return value
        .toLowerCase()
        .trim()
        .replace(/[^\w\s-]/g, "")
        .replace(/\s+/g, "-")
        .replace(/-+/g, "-");
}

function parseInline(text, currentMarkdownPath) {
    let inline = escapeHtml(text);
    inline = inline.replace(/`([^`]+)`/g, "<code>$1</code>");
    inline = inline.replace(/\*\*([^*]+)\*\*/g, "<strong>$1</strong>");
    inline = inline.replace(/\*([^*]+)\*/g, "<em>$1</em>");
    inline = inline.replace(/\[([^\]]+)\]\(([^)]+)\)/g, (_, label, href) => {
        const trimmedHref = href.trim();
        if (isMarkdownHref(trimmedHref)) {
            const resolvedHref = resolveMarkdownPath(trimmedHref, currentMarkdownPath);
            return `<a href="${MARKDOWN_VIEWER_PAGE}?src=${encodeURIComponent(resolvedHref)}">${label}</a>`;
        }

        return `<a href="${trimmedHref}">${label}</a>`;
    });

    return inline;
}

function isMarkdownHref(href) {
    return /\.md($|[#?])/i.test(href) && !/^(https?:|mailto:|javascript:)/i.test(href);
}

function resolveMarkdownPath(target, currentMarkdownPath) {
    const base = new URL(currentMarkdownPath || ".", window.location.href);
    const resolved = new URL(target, base);
    return resolved.pathname + (resolved.search || "") + (resolved.hash || "");
}

function rewriteMarkdownLinksToViewer() {
    for (const link of document.querySelectorAll("a[href]")) {
        const href = link.getAttribute("href");
        if (!href || !isMarkdownHref(href)) {
            continue;
        }

        link.setAttribute("href", `${MARKDOWN_VIEWER_PAGE}?src=${encodeURIComponent(href)}`);
    }
}

function renderMarkdown(markdown, currentMarkdownPath) {
    const lines = markdown.replace(/\r\n/g, "\n").split("\n");
    const output = [];

    let inCode = false;
    let codeLang = "";
    let inUnorderedList = false;
    let inOrderedList = false;

    function closeLists() {
        if (inUnorderedList) {
            output.push("</ul>");
            inUnorderedList = false;
        }
        if (inOrderedList) {
            output.push("</ol>");
            inOrderedList = false;
        }
    }

    for (const line of lines) {
        const trimmed = line.trim();

        if (trimmed.startsWith("```")) {
            closeLists();
            if (inCode) {
                output.push("</code></pre>");
                inCode = false;
                codeLang = "";
            } else {
                codeLang = trimmed.slice(3).trim();
                const languageClass = codeLang ? ` class="language-${escapeHtml(codeLang)}"` : "";
                output.push(`<pre><code${languageClass}>`);
                inCode = true;
            }
            continue;
        }

        if (inCode) {
            output.push(`${escapeHtml(line)}\n`);
            continue;
        }

        if (!trimmed) {
            closeLists();
            continue;
        }

        if (/^#{1,6}\s+/.test(trimmed)) {
            closeLists();
            const level = trimmed.match(/^#+/)[0].length;
            const text = trimmed.slice(level).trim();
            const id = slugify(text);
            output.push(`<h${level} id="${id}">${parseInline(text, currentMarkdownPath)}</h${level}>`);
            continue;
        }

        if (/^\d+\.\s+/.test(trimmed)) {
            if (!inOrderedList) {
                closeLists();
                output.push("<ol>");
                inOrderedList = true;
            }
            const itemText = trimmed.replace(/^\d+\.\s+/, "");
            output.push(`<li>${parseInline(itemText, currentMarkdownPath)}</li>`);
            continue;
        }

        if (/^[-*]\s+/.test(trimmed)) {
            if (!inUnorderedList) {
                closeLists();
                output.push("<ul>");
                inUnorderedList = true;
            }
            const itemText = trimmed.replace(/^[-*]\s+/, "");
            output.push(`<li>${parseInline(itemText, currentMarkdownPath)}</li>`);
            continue;
        }

        if (/^>\s?/.test(trimmed)) {
            closeLists();
            output.push(`<blockquote>${parseInline(trimmed.replace(/^>\s?/, ""), currentMarkdownPath)}</blockquote>`);
            continue;
        }

        if (/^---+$/.test(trimmed)) {
            closeLists();
            output.push("<hr>");
            continue;
        }

        closeLists();
        output.push(`<p>${parseInline(trimmed, currentMarkdownPath)}</p>`);
    }

    closeLists();

    if (inCode) {
        output.push("</code></pre>");
    }

    return output.join("\n");
}

async function loadMarkdownViewer() {
    const root = document.getElementById("markdown-content");
    if (!root) {
        return;
    }

    const params = new URLSearchParams(window.location.search);
    const sourceSpec = params.get("src");
    if (!sourceSpec) {
        root.innerHTML = "<p class=\"error-text\">Missing markdown source. Use ?src=../README.md</p>";
        return;
    }

    const [sourcePath, sourceHash] = sourceSpec.split("#", 2);
    const sourceUrl = new URL(sourcePath, window.location.href);
    const sourceName = sourcePath.split("/").pop() || sourcePath;
    const pathLabel = document.getElementById("markdown-source-path");
    if (pathLabel) {
        pathLabel.textContent = sourcePath;
    }
    document.title = `${sourceName} - Markdown Viewer`;

    try {
        if (window.location.protocol === "file:") {
            root.innerHTML = "<p class=\"error-text\">This viewer needs a local web server (http://localhost) to fetch markdown files. Open docs via a local server instead of file://.</p>";
            return;
        }

        const response = await fetch(sourceUrl.href);
        if (!response.ok) {
            throw new Error(`Failed to load ${sourcePath} (${response.status})`);
        }

        const markdown = await response.text();
        root.innerHTML = renderMarkdown(markdown, sourceUrl.pathname);

        if (sourceHash) {
            const decodedHash = decodeURIComponent(sourceHash);
            const target = document.getElementById(decodedHash) || document.getElementById(slugify(decodedHash));
            if (target) {
                target.scrollIntoView();
            }
        }
    } catch (error) {
        root.innerHTML = `<p class="error-text">${escapeHtml(error.message)}</p>`;
    }
}

function wireTogglesAndFilters() {
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
    if (filterButtons.length === 0) {
        return;
    }

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

document.addEventListener("DOMContentLoaded", async () => {
    rewriteMarkdownLinksToViewer();
    wireTogglesAndFilters();
    await loadMarkdownViewer();
});
