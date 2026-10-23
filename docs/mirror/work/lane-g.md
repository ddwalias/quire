# Lane G — interop.mathcadautomationprivate

- Owner: Lane G; branch `mirror/g-interop-20261024-0238`; worktree `C:/Projects/Quire-wt/lane-g-interop`.
- Original: `interop.mathcadautomationprivate.dll` 1.2.0.0, SHA-256 `fc7d6506…188bd`, MVID `9a830ac8-1a16-4f23-935c-eede8e2d2032`.
- Project: `Quire.AutomationPrivate` (new, C#, standalone; `net10.0`, references `Quire.Mirror`). Namespace per the type-name rule (`MathcadAutomationPrivate.X` → `Quire.AutomationPrivate.X`).
- Members (all claimed and implemented): 13 interfaces (`ISymbolicValue`, `SymbolicValue`, `IValuePrivate`, `ISequentialStream`, `IStream`, `IMatrixValuePrivate`, `INumericValuePrivate`, `IFunctionValue`, `IValueFactory`, `IWorksheetPrivate`, `IMASImageCache`, `IWindowPrivate`, `IApplicationPrivate`), 4 structs (`_LARGE_INTEGER`, `_ULARGE_INTEGER`, `tagSTATSTG`, `_FILETIME`), coclass `SymbolicValueClass` with its constructor. 51 declarations (methods, accessors, constructor), the gate's inventory count.
- Release IL report: `release_il.py --base main --project Quire.AutomationPrivate` → **FAIL, 0 checked**. All 18 `[MirrorOf]` types fail inspection with "MirrorOf does not match the reviewed type-map binding": `docs/mirror/type-map.csv` has no `interop.mathcadautomationprivate.dll` rows. Nothing was compared.
- Local structural check (Release build vs original Release IL, not the gate): names, signatures, `in`/`out`/`ref` flags, marshalling, default values, GUIDs, DispIds, interface/TypeLib flags, struct fields, `pack` and `size` all match. Remaining differences below.

## Remaining mismatches (compiler-level; C# cannot express them)

1. Method implementation flags: original `runtime managed internalcall` (ImplAttributes 0x1003) on the 50 interface methods and accessors. Roslyn emits `cil managed` for them. `MethodImplOptions` rejects the CodeType bits (CS0591). The coclass constructor already matches. The gate's `declaration_only` path does not compare implementation flags.
2. `hidebysig` on the coclass constructor (Roslyn always emits it).
3. Property signatures: the original has no `instance` calling-convention bit (`Size`, `IsRange`, `Arity`, `IsModified`); Roslyn always sets it.
4. `beforefieldinit` on the 13 interfaces and `SymbolicValueClass`; Roslyn adds it, the original does not.
5. Custom attribute order on 8 types (metadata order only; the gate does not compare type attributes).
6. `IWorksheetPrivate.IsModified` setter parameter named `value`; the original is `pIsModified` (metadata only).
7. `SymbolicValue` `[CoClass]` names `Quire.AutomationPrivate.SymbolicValueClass`, which is the namespace rule itself.

## Blockers for the maintainer (shared tooling and mappings; not edited here)

- `docs/mirror/type-map.csv`: rows binding each type to `interop.mathcadautomationprivate.dll` are required before MethodAudit can pair them (blocks the gate).
- `tools/Quire.Tools.MirrorGen/Rules.cs`: `IsDeferredAssembly` defers every `interop.*` assembly; `LanguageOf`, `ProjectFor` and the D20 table have no entry for this assembly.
- `docs/mirror/assembly-map.json` and `WORK.md` have no entry for this lane or assembly.
- The 8 embedded copies in `src/Quire.Automation.Native/Generated/Mirror/AutomationPrivate.*.g.cs` already define `Quire.AutomationPrivate` interfaces. Routing needs a decision (D20 embeds per consumer; this project is standalone). A project that references both would get duplicate-type errors.
- `reference.py` cannot read per-type IL for the 6 types without methods (ISymbolicValue, SymbolicValue, and the 4 structs): MethodAudit `members` exits 1. Their IL was read from the cached whole-assembly listing.
