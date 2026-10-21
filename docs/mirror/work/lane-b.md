# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `AssemblyBuilder.LoadAssembly`; `MetadataReferenceTracker.GetOrAddTypeSpecification`, `TypeBuilder.GetInterfaceMap`, all three `MethodHandleResolver` overloads, and `CustomAttributeBuilder.EmitValue` now match Release IL exactly.
- Latest full Release IL inventory: `.mirror-cache/release-il-328855f303e9/release-il.json` (601 exact; 18 unfinished). Seventeen unfinished members have managed IL; `Logger.GetCurrentThreadId` is an unmanaged P/Invoke. There are no `Quire.Mpl.Emit` failures; unrelated `Quire.Core` changes make the project-wide comparator exit nonzero.
- Remaining managed members: 17 — `CustomAttributeBuilder.ValidateType` and its main constructor; `DynamicMethod` constructor; three `ILCodeConverter` exception-block/emit methods; `Logger.ProcessLogQueue`; two `LoggerAssistant` formatting methods; two `MetadataReferenceTracker` reference methods; `ModuleBuilder.AppendMetadata` and `AddTargetFrameworkAttribute`; `SignatureHelper.MethodSignatureEnconder`; `TypeBuilder.HasConstructor` and `DefineTypeInitializer`.
