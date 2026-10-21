# Semantic API documentation consistency

Date: 2026-10-19
Status: reviewed source snapshot; runtime unverified

## Conclusion and scope

The adjacent [nulllogicone.net repository](https://github.com/nulllogicone/nulllogicone.net), inspected at local commit `37dba67` with a clean working tree, fits the [transition overview](../000-motivation.md): it modernizes the semantic API alongside the separate OliWeb end-user UI migration. The owner reports it deployed at [nulllogicone.net](https://nulllogicone.net) and sufficient for current needs. This review does not propose API expansion or change the UI migration scope.

## Source evidence

Paths below are relative to the API repository unless stated otherwise.

| Documentation statement | Source observation |
|---|---|
| Modern ASP.NET Core API and HTML views | `NulllogiconeCore/NulllogiconeCore/NulllogiconeCore.csproj` targets .NET 10; `Program.cs` maps Minimal APIs and Razor Pages. |
| JSON and RDF exposure | `Endpoints/` registers Stamm, Angler, PostIt and TopLab GET routes, each with `.json` and `.rdf` variants and content negotiation. HTML requests redirect to `/ui/...`. RDF mappers produce RDF/XML; endpoints return it as `text/xml`. XML is this RDF serialization, not a separate generic XML format. |
| Existing OLI-it database | `Data/ApplicationDbContext.cs` maps existing German-named tables in schema `oli`; `Program.cs` uses EF Core SQL Server. This supports schema compatibility, but does not prove the deployed API and UI use the same database instance. |
| Ontop SPARQL over SQL Server | `Endpoints/SparqlEndpoints.cs` exposes GET/POST `/sparql` and proxies queries to a configured Ontop endpoint. `ontop/nulllogicone.obda.ttl` supplies SQL-to-RDF mappings. |
| Broader semantic entity coverage | Twelve mapped classes: Stamm, Angler, PostIt, Wurzeln, TopLab, Code, Ring, Loch, Netz, Knoten, Baum and Zweig. Mapping coverage is broader than the four individual entity routes; it does not imply HTML/JSON/RDF routes for every class. |
| Parallel service rather than UI dependency | OLI-it.Web's `OLI-it.Web/Program.cs` configures its own SQL Server DbContext; the inspected C# source contains no nulllogicone.net API integration. |

## Documentation reconciliation

- The overview and OQ-014 correctly identify an existing separate JSON/RDF API. The mission statement in the API repository also agrees with OLI-it as an open protocol for exchanging meaning.
- BL-106 previously described JSON, XML and RDF as separate formats and left the service simply “in progress.” It now records the owner's current sufficiency assessment and separates optional API expansion from UI parity work.
- The API application's README says “All Entities” have endpoints, which exceeds the four registered entity route groups. The root README's remaining entity/ontology expansion items are more accurate.
- `ontop/README.md` lists eleven entities but the checked-in mapping includes Wurzeln as a twelfth. Its example using `nlo:stammGuid` as an Angler-to-Stamm relationship differs from the mapping's `nlo:belongsToStamm`; the mapping also defines the reverse `nlo:hasAngler` relationship.
- The API application's README describes the internal Ontop endpoint; the source additionally exposes a public-facing application proxy at `/sparql`. These are two layers of the same design, not competing services.

The API repository was left unchanged. Its README differences are minor documentation follow-up, not evidence that the service needs more implementation for the owner's current use.

## Verification boundary

Reviewed root/application/Ontop READMEs, mission, project target, startup, entity routes, representative RDF mapping, EF table mappings, content negotiation and Ontop mappings. No build, database access, writes, exhaustive ontology audit or legacy endpoint parity test was performed.

The web reader could not access the deployed URL; shell HTTP requests to the root and OpenAPI document failed hostname resolution in this environment. Live availability, query execution and the deployed commit remain unverified. These access failures do not establish that the site is down.
