# Audit Artifacts

Audit sessions are written under timestamped folders:

`docs/Audit/<audit_timestamp>/`

Each session folder should contain:

- `sequence.mmd` - Mermaid sequence diagram of workflow activity over time
- `topology.mmd` - Mermaid graph of agent-instance connections with call counts
- `stats.md` - human-readable run statistics
- `events.json` - raw append-only audit events
- `index.html` - local browser report that renders the Mermaid diagrams and shows the statistics

The `AuditorAgent` writes these files only while an explicit user-started audit is active and only when the user ends that audit session.

`docs/Audit/viewer-template.html` is the shared HTML shell that can be copied into a session folder and filled with the generated Mermaid and statistics content.
