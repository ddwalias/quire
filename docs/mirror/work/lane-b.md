# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `AssemblyBuilder.LoadAssembly`; `MetadataReferenceTracker.GetOrAddTypeSpecification` now matches Release IL exactly.
- Latest Release IL inventory: `.mirror-cache/release-il-17d5fd681b7a/release-il.json` (3,024 exact; 1,683 unfinished). It reports no `Quire.Mpl.Emit` failures; the project-wide command exits nonzero on 59 concurrent `Quire.Core` main changes.
- Remaining mismatches: 23 unfinished members; blockers include `TypeBuilder.HasConstructor` lambda-generated `<>c` without a Mathcad counterpart, `DefineTypeInitializer` local/evaluation-stack ordering, and `GetInterfaceMap` unused initialized-local shape; ILCodeConverter exception-block branch destinations; `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`; `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration; `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches; `MethodHandleResolver` branch/call shape; and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
