# Quire.EquationEditor checkpoint (haiku-equationeditor-20261123)

- Owner: EquationEditor worker (task_a5b258a05c92)
- Branch: `feature/haiku-equationeditor-20261123`
- Assembly: `Quire.EquationEditor`

## Batch 1 — Tree.MatrixNodeInfo viewport and format accessors

- Members: `ViewportSize`, `ViewportOffset`, `ShowIndices`, `CollapseNestedMatrices`, `Origin` (get and set), in `Mirrored/EquationEditor.Tree.MatrixNodeInfo.FormatProperties.cs`. Stubs removed from the `.g.cs` by hand, because MirrorGen `emit` could not run (needs a Release build of `Quire.Componentsmgd`).
- Release IL: `--project Quire.EquationEditor` PASS, 1074 checked, 0 failures.
- Remaining mismatches: none in changed members.

## Parked (blockers, unchanged)

- Field/method name clashes (CS0102): obfuscated fields such as `a`/`b`/`c` that share names with Mathcad methods. MirrorGen suffixes these fields, and the gate compares field names, so members touching them cannot match. Needs a maintainer decision.
- Obfuscated property accessors (e.g. `RedefinitionWarning` setters named `a`/`b`/`c`, `MathNodeSettings`): C# property accessors compile to `get_`/`set_`, so they cannot match Mathcad's accessor names.
- `MatrixNodeInfo.MatrixFormat` getter/setter: field `a` clashes with method `a(string, object)`.

## Batch 2 — Tree.MatrixNodeInfo viewport change dispatcher

- Member: `MatrixNodeInfo.a(string, object)` (private property-change dispatcher for "Size" and "Offset"), in `Mirrored/EquationEditor.Tree.MatrixNodeInfo.ViewportChange.cs`. Stub removed from the `.g.cs` by hand.
- Release IL: `--project Quire.EquationEditor` PASS, 1075 checked, 0 failures.
- Still blocked: `MatrixNodeInfo.MatrixFormat` (field `a` clashes with method `a`); `ICloneable.Clone` explicit implementation (Mathcad name `b` clashes with field `b`).

## Batch 3 — Scoping.NavigatorSegment constructor, right state and operator parts

- Members: `NavigatorSegment(NavigatorState, NavigatorState)` constructor, `RightNavigatorState` get/set (field `b` moved to `Mirrored/EquationEditor.Tree.Scoping.NavigatorSegment.cs`), `LeftOperatorPart`, `RightOperatorPart`. Stubs removed from the `.g.cs` by hand.
- Release IL: `--project Quire.EquationEditor` PASS, 1080 checked, 0 failures.
- Still blocked: `LeftNavigatorState` (field `a` clashes with method `a`), `NavigatorSegment.a(...)` overloads, and members that read `a`.
