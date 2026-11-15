# Controls Core, Whiteboard and Serialization (serialization types)

- Owner: Orca task `task_beb76ff11345`; branch `feature/ws-serial-b-claude-20261116`.
- Types: `Serialization.ResultSerializer`, `TextStylesCollectionManager`, `Serialization.SerializationHelper`, `Serialization.McdxSerializer`. `ContainerItemManager`, `MathFormat`, `ProtectDialog` belong to another worker and are untouched.

## Batch 1 — the four serialization types

- Members: every stub of the four types moved to their hand-written partials in Mathcad order. The generated `.g.cs` files keep only the class headers. Other-worker types are untouched.
- Report: project gate `Quire.Controls.Core` PASS, 2815 exact, 0 failures, 458 unfinished (`release-il-59ac9398d01c`).
## Batch 2 — Retrying the four parked members

- Landed:
  - `TextStylesCollectionManager.LoadWorkSheetStylesFromFile` (commit `421a17988`): Mathcad's literal `Ptc.Controls.Core;component/Fonts/#` has no leading slash and is compared as-is by MethodAudit (D21 pack URI rule requires leading slash). Emitting the exact literal via `string.Concat` in Quire matches Release IL.
  - `SerializationHelper.InitializeForWrite` (commit `ff4452f1b`): Closure ordinal alignment achieved via compile-order device (placed before `GetNextRegionId`), matching Mathcad's `<>c.<InitializeForWrite>b__33_0`.
- Parked (unmarked throw placeholders, Mathcad order kept):
  - `ResultSerializer.ReadXml(XElement, IResultManager)`: Mathcad builds the `EngineResult` struct with `newobj` and then mutates the local through `ldloca`. Roslyn 10 emits `ldloca` plus `call .ctor` for any mutated struct local, and no source form produces the Mathcad sequence. In addition, `Quire.Calculation.Core.SynchronizationStatus.Synchronized` is defined with value 1 in `Quire.Calculation.Core` instead of Mathcad's value 3.
  - `McdxSerializer.PackImage(PackagePart, ...)` (static): Mathcad stores the method-entry display class immediately in a local (`newobj; stloc.0; ldloc.0`), while Roslyn 10 emits `newobj` and chains `dup` for the captures across all source forms (lambdas, delegate syntax).
- Remaining generated stubs for these four types: 0 in the `.g.cs` files. Unfinished placeholders: 2 (listed above).
