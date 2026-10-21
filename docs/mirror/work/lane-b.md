# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `ILGenerator`; current `TypeBuilder` unsupported methods and flag helpers.
- Release IL report: PASS, `.mirror-cache/release-il-5c950fc03ccb/release-il.json` (513 exact).
- Remaining mismatches: 105 unfinished members; blockers include `TypeBuilder.HasConstructor` lambda-generated `<>c` without a Mathcad counterpart; `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`; `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration; `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `AssemblyBuilder.EmitAssembly` public-key evaluation-stack shape and `LoadAssembly` delegate suffix mismatch; `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches and `GetOrAddTypeSpecification` branch shape; `MethodHandleResolver` branch/call shape; and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
