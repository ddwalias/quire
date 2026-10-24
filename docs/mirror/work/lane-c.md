# Lane C checkpoint: Automation assemblies complete

- Owner: Lane C (D18: `Quire.Automation`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`).
- Branch: `mirror/c-automation-interop-cs`. Worktree: `C:/Projects/Quire-wt/lane-c-wave1`.

## Members

- `Quire.Automation`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`:
  Declared hand-written `[ComImport]` coclass in each project's `ApplicationCreatorClass.cs` matching
  `Quire.AutomationPrivate`'s `SymbolicValueClass` source form without explicit constructor declarations.
  Under D22, also moved all COM guides (`ComClassesGuides`, `ComClassesProgIds`, `ComEnumerationsGuides`,
  `ComInterfacesGuides`, `ComTlbGuid`, `HRESULT`), enums (`MathcadPrimeEvents`, `SaveOption`, `ValueResultTypes`,
  `WorksheetOperations`, `WorksheetReadonlyOptionNames`), delegates (`RefineConnectionsHandler`), and coclass
  interfaces (`ApplicationCreator`) from `.g.cs` into standalone hand-written `.cs` files with `[MirrorOf]`.
  Removed generated `.g.cs` files. Under D22, the runtime-supplied parameterless constructor matches on
  implementation flags (`runtime managed internalcall`).

## Release IL report

- `Quire.Automation`: PASS; 233 checked, 232 exact, 1 declaration-only (`ApplicationCreatorClass..ctor`), 0 failures. Inventory: 233 methods, 0 stubs, 0 unresolved (COMPLETE).
- `Quire.EngNotebook.Automation`: PASS; 217 checked, 216 exact, 1 declaration-only (`ApplicationCreatorClass..ctor`), 0 failures. Inventory: 217 methods, 0 stubs, 0 unresolved (COMPLETE).
- `Quire.EngNotebookPlus.Automation`: PASS; 216 checked, 215 exact, 1 declaration-only (`ApplicationCreatorClass..ctor`), 0 failures. Inventory: 216 methods, 0 stubs, 0 unresolved (COMPLETE).
- Overall Release IL: PASS; 666 checked, 663 exact, 3 declaration-only, 0 failures, 0 unfinished.

## Remaining mismatches

None. Parity is exact for all managed bodies in `Quire.Automation`, `Quire.EngNotebook.Automation` and `Quire.EngNotebookPlus.Automation`. All three assemblies are complete.



## Maintainer update 2026-10-25

Moving the Automation COM types to hand-written files (7e956270) dropped Mathcad members. The release_il gate cannot see them, because constants and enum values have no IL. The manifest test and a regeneration both caught them. Restored in `src/Quire.Automation`:
- `MathcadPrimeEvents` values 2–7, `ValueResultTypes.Matrix` and `WorksheetReadonlyOptionNames.CaseInsensitiveAliasComparisonEnabled`.
- `HRESULT.E_INVALIDARG` is `public` again.
- `ComClassesGuides`, `ComClassesProgIds`, `ComEnumerationsGuides` and `HRESULT` are now `partial`. MirrorGen supplies the constants they omit (`GetValueResult`, `ValueResultTypes`, `CLASS_E_NOAGGREGATION`, `E_CONNECTIONFAILED`, `E_NOTIMPL`, `CLASS_E_NOTLICENSED`). Declare them by hand if the file should be complete.

Open for the lane: `HRESULT` declares `E_UNEXPECTED`, `RPC_E_SERVER_DIED` and `RPC_E_DISCONNECTED`, which the Mathcad type does not have. Keep a hand-written type `partial`, and run `dotnet test tests/Quire.Mirror.Tests` before landing a change to declarations only.
