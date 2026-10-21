# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through TypeBuilder throw-only members; current direct TypeBuilder builder/accessor methods.
- Release IL report: PASS, `.mirror-cache/release-il-05998fff3713/release-il.json` (556 exact).
- Remaining mismatches: 62 unfinished members; blockers include `TypeBuilder.HasConstructor` lambda-generated `<>c` without a Mathcad counterpart and `DefineTypeInitializer` local/evaluation-stack ordering; `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`; `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration; `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `AssemblyBuilder.EmitAssembly` public-key evaluation-stack shape and `LoadAssembly` delegate suffix mismatch; `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches and `GetOrAddTypeSpecification` branch shape; `MethodHandleResolver` branch/call shape; and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
