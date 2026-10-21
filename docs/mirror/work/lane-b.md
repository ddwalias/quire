# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `ILGeneratorStore`; current all managed `ILGenerator` members.
- Release IL report: PASS, `.mirror-cache/release-il-f74def4d997c/release-il.json` (505 exact).
- Remaining mismatches: 113 unfinished members; blockers include `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`, `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration, `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `AssemblyBuilder.EmitAssembly` public-key evaluation-stack shape, `AssemblyBuilder.LoadAssembly` delegate suffix mismatch, `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches, `GetOrAddTypeSpecification` branch shape, `MethodHandleResolver` branch/call shape, and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
