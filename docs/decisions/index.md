# ADR Index

<!-- MAINTAINED BY: Human, after each review session.
     Codex reads this file to select which ADRs to load for a given feature.
     Do not load individual ADR files without first checking this index.

     Tag convention — use these categories consistently:
     Layer tags:   domain | application | infrastructure | api | all-layers
     Topic tags:   persistence | async | validation | events | auth | testing
     Add project-specific tags as needed — keep them lowercase and hyphenated. -->

| ADR | Title | Status | Tags |
|-----|-------|--------|------|
| [001](_adr-template.md) | _(rename this when writing your first real ADR)_ | Draft | |

---

## How Codex uses this index

When starting `$new-feature`, read this index and identify ADRs whose tags overlap
with the feature's affected layers or topics.
Load only those ADR files — not all of them.

Example: a feature touching `domain` and `persistence` → load ADRs tagged with either.
A feature touching `api` only → skip `domain`-only ADRs.

If an ADR tag is `all-layers`, always load it regardless of feature scope.
