# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `TypeNameBuilder`; `AssemblyBuilder` state fields, four properties, constructor, and `GetName` now pass.
- Release IL report: PASS, `.mirror-cache/release-il-4c87e6edfca0/release-il.json` (216 exact).
- Remaining mismatches: 402 unfinished members; blockers also include `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches and `GetOrAddTypeSpecification` branch shape.
