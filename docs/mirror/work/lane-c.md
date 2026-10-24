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


