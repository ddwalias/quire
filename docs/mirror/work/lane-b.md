# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `DynamicMethod` reflection members; current `DynamicMethod_ILGenerator` constructor, `Id`, and managed forwarding members.
- Release IL report: PASS, `.mirror-cache/release-il-806c96d293bc/release-il.json` (378 exact).
- Remaining mismatches: 240 unfinished members; blockers include `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`, `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration, `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `AssemblyBuilder.EmitAssembly` public-key evaluation-stack shape, `AssemblyBuilder.LoadAssembly` delegate suffix mismatch, `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches, and `GetOrAddTypeSpecification` branch shape.
