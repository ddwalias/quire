# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `AssemblyBuilder.LoadAssembly`; `MetadataReferenceTracker.GetOrAddTypeSpecification` and `TypeBuilder.GetInterfaceMap` now match Release IL exactly.
- Latest Release IL inventory: `.mirror-cache/release-il-663bfda3c5ed/release-il.json` (3,025 exact; 1,604 unfinished). It reports no `Quire.Mpl.Emit` failures; the project-wide command exits nonzero on 15 concurrent `Quire.Core` main changes.
- Remaining members: 22 unfinished. Current blockers include `TypeBuilder.HasConstructor` lambda-generated `<>c` without a Mathcad counterpart and `DefineTypeInitializer` local/evaluation-stack ordering (`dup` versus store/reload); ILCodeConverter exception-block branch destinations; `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`; `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration; `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches; `MethodHandleResolver` branch/call shape; and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
