# Design Overview Page

This folder documents the narrative and visual flow behind the `/design-overview` page.

## Full Story

In OLI-it, a **Stamm** (author) creates a **PostIt** (message).  
The message is scanned and matched by **Angler** (filter profiles).  
A receiver answers with **TopLab** (response).  
This answer can trigger new communication and continues the **Kreislauf** (circuit).

The semantic context around matching and interpretation is anchored in **Wortraum** structures
(`Netz`, `Knoten`, `Zweig`) and design class groups (`SAPCT`, `NKPZ`) used for conceptual framing.

## Kreislauf + Wortraum (Mermaid)

```mermaid
flowchart LR
    Stamm["👤 Stamm<br/>Author"] --> PostIt["📝 PostIt<br/>Message"]
    PostIt --> Angler["🎣 Angler<br/>Filter Profile"]
    Angler --> TopLab["💬 TopLab<br/>Answer"]
    TopLab --> Stamm

    subgraph Wortraum["🌐 Wortraum Context (NKBZ)"]
        Netz["N: Netz"]
        Knoten["K: Knoten"]
        Baum["B: Baum"]
        Zweig["Z: Zweig"]
        Netz --> Knoten --> Baum --> Zweig
    end

    PostIt -. classified by .-> Wortraum
    Angler -. matched in .-> Wortraum
```

## SAPCT roles (concept view)

```mermaid
flowchart TB
    S["S = Stamm"] --> P["P = PostIt"]
    P --> A["A = Angler"]
    A --> T["T = TopLab"]
    P --> C["C = Code / classification cues"]
    C --> A
```

## Assets in this folder

- `design-overview-diagrams.jpg` - exported visual (charts/comics/diagram snapshot)
- `architecture.drawio` - editable architecture source for future updates
