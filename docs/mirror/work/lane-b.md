# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `AssemblyBuilder.LoadAssembly`; `MetadataReferenceTracker.GetOrAddTypeSpecification`, `TypeBuilder.GetInterfaceMap`, and `MethodHandleResolver.ResolveConstructor` now match Release IL exactly.
- Latest Release IL inventory: `.mirror-cache/release-il-56208228610a/release-il.json` (3,244 exact; 1,957 unfinished). It reports no `Quire.Mpl.Emit` failures; the project-wide command exits nonzero on concurrent `Quire.Core` changes on main.
- Remaining members: 21 unfinished. Current blockers include `TypeBuilder.HasConstructor` lambda-generated `<>c` without a Mathcad counterpart and `DefineTypeInitializer` local/evaluation-stack ordering (`dup` versus store/reload); ILCodeConverter exception-block branch destinations; `ModuleBuilder.AppendMetadata` branch layout and `AddTargetFrameworkAttribute` lambda-generated `<>c`; `Logger.ProcessLogQueue` branch layout and native P/Invoke declaration; `CustomAttributeBuilder` main-constructor lambda-generated `<>c`, `ValidateType` branch shape, and `EmitValue` object-branch IL layout; `MetadataReferenceTracker` lambda display-class ordinal/no-counterpart mismatches; remaining `MethodHandleResolver` branch/call shapes; and `LoggerAssistant` lambda-generated `<>c` without a Mathcad counterpart.
