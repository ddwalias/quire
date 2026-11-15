# Controls Core, Whiteboard and Serialization (serialization types)

- Owner: Orca task `task_beb76ff11345`; branch `feature/ws-serial-b-claude-20261116`.
- Types: `Serialization.ResultSerializer`, `TextStylesCollectionManager`, `Serialization.SerializationHelper`, `Serialization.McdxSerializer`. `ContainerItemManager`, `MathFormat`, `ProtectDialog` belong to another worker and are untouched.

## Batch 1 — the four serialization types

- Members: every stub of the four types moved to their hand-written partials in Mathcad order. The generated `.g.cs` files keep only the class headers. Other-worker types are untouched.
- Report: project gate `Quire.Controls.Core` PASS, 2815 exact, 0 failures, 458 unfinished (`release-il-59ac9398d01c`).
- Parked (unmarked throw placeholders, Mathcad order kept):
  - `ResultSerializer.ReadXml(XElement, IResultManager)`: Mathcad builds the `EngineResult` struct with `newobj` and then mutates the local through `ldloca`. Roslyn 10 emits `ldloca` plus `call .ctor` for any mutated struct local, and no source form in this compiler produces the Mathcad sequence.
  - `TextStylesCollectionManager.LoadWorkSheetStylesFromFile`: the Mathcad literal `Ptc.Controls.Core;component/Fonts/#` has no leading slash, so the own-assembly rule in D21 does not apply, and the Quire literal cannot match.
  - `SerializationHelper.InitializeForWrite`: Roslyn counts four members per auto-property where Mathcad counts three, so the lambda ordinal is 38 where Mathcad has 33.
  - `McdxSerializer.PackImage(PackagePart, ...)` (static): Mathcad stores the method-entry display class in a local, while Roslyn 10 emits `newobj` and `dup` for the same captures.
- Remaining generated stubs for these four types: 0 in the `.g.cs` files. Unfinished placeholders: 4 (listed above).
