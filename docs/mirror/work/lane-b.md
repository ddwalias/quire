# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `DynamicAssemblyModel.MethodIndex`, `FieldIndex`, and `MethodOffset`; `DynamicAssemblyModel.AddTypeDefinition` and `AddFieldDefinition` now pass.
- Release IL report: PASS, `.mirror-cache/release-il-ad3d1853c33d/release-il.json` (185 checked).
- Remaining mismatches: 433 unfinished members; blockers: `SignatureHelper` `ParameterInfo[]` lambda generated members, `MethodHandleResolver` overload branch shapes, and `GetOrAddTypeSpecification` branch shape.
