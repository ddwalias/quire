# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `AssemblyBuilder.EmitAssembly`; current `AssemblyBuilder.LoadAssembly` and its compiler-generated predicate.
- Release IL report: PASS, `.mirror-cache/release-il-aa59e812153c/release-il.json` (595 exact).
- Remaining mismatches: 24 unfinished members; blockers include `TypeBuilder.HasConstructor` lambda-generated `<>c` without a Mathcad counterpart, `DefineTypeInitializer` local/evaluation-stack ordering, and `GetInterfaceMap` unused initialized-local shape; ILCodeConverter exception-block branch destinations; `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`; `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration; `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches and `GetOrAddTypeSpecification` branch shape; `MethodHandleResolver` branch/call shape; and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
