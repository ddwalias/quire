# Lane B — Quire.Mpl.Emit

- Owner: Lane B (Codex).
- Branch: `mirror/b-mpl-emit-20261021-a7`.
- Selected members: prior passing groups through `AssemblyBuilder.LoadAssembly`; `MetadataReferenceTracker.GetOrAddTypeSpecification`, `TypeBuilder.GetInterfaceMap`, all three `MethodHandleResolver` overloads, `CustomAttributeBuilder.EmitValue`, and `ILCodeConverter.ConvertEndExceptionBlock` now match Release IL exactly.
- Latest full Release IL inventory: `.mirror-cache/release-il-5bf190154552/release-il.json` (3417 exact; 1504 unfinished across the repository). No `Quire.Mpl.Emit` members fail; unrelated `Quire.Core` and `Quire.Media3D` changes make the project-wide comparator exit nonzero.
- Remaining managed members: 16 — `CustomAttributeBuilder.ValidateType` and its main constructor; `DynamicMethod` constructor; two `ILCodeConverter` exception-block/emit methods; `Logger.ProcessLogQueue`; two `LoggerAssistant` formatting methods; two `MetadataReferenceTracker` reference methods; `ModuleBuilder.AppendMetadata` and `AddTargetFrameworkAttribute`; `SignatureHelper.MethodSignatureEnconder`; `TypeBuilder.HasConstructor` and `DefineTypeInitializer`.
