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

## Batch 4 — Scoping.NavigatorSegment.SegmentIsSingleNode

- Member: `SegmentIsSingleNode` getter, in `Mirrored/EquationEditor.Tree.Scoping.NavigatorSegment.cs`, `[SkipLocalsInit]` on the getter because Mathcad has `.initlocals False`. Stub removed from the `.g.cs` by hand.
- Release IL: `--project Quire.EquationEditor` PASS, 1081 checked, 0 failures (first run failed only on `.initlocals`).
- Still blocked: `LeftNode`, `RightNode`, `Linearizable` (obfuscated `bq` and `aw` helpers and field `a`), `LeftNavigatorState`.

## Batch 5 — MemberGen `accept` over Quire.EquationEditor (mechanical members)

- Owner: EquationEditor worker (haiku-equationeditor-20261123). Branch `feature/haiku-equationeditor-20261123`.
- Method: `python tools/Quire.Tools.MemberGen/accept.py --project Quire.EquationEditor --rounds 3`
  (run once; 3 rounds; each kept member passed the Release IL check inside the tool).
- Kept: 434 members (52 fields moved out of `Generated/Mirror`). Rejected: 147 (140 IL mismatch,
  7 compile errors). Rejected members keep their stubs.
- Files: 86 modified or removed stubs in `Generated/Mirror/*.g.cs`, plus new hand-written partials
  under `src/Quire.EquationEditor/{Equation,EquationEditor,Native/PtcEquationEditor,SpecTable/Model,Tree,Tree/Scoping,Tree/Transform}`.
  Duplicate `using` lines introduced by the tool were removed from 8 files (no IL effect).
- Release IL: `--project Quire.EquationEditor` PASS, 1392 checked, 0 failures (was 1081 before this batch).
  Stub-not-implemented skips: 2666 -> 2355.
- Remaining mismatches: none in checked members.
- Quality debt (not fixed in this batch): MemberGen emits ILSpy-style parameter names (`A_0`, `A_1`, ~1250
  uses in ~56 files) and some `MirrorVerified` placeholder partials (`b_`, `c_`, ...). These are IL-neutral
  and need a manual idiomatic pass (rename parameters from their bodies) under AGENTS.md before the names are final.

## Still blocked (unchanged)

- Field/method name clashes, `.initlocals`, ctor-stores-before-base-call, and closure ordinals that need
  placeholder stubs. The 147 members MemberGen rejected are in this group.

## Batch 6 — parameter names of batch 5 (no IL change)

- Renamed every `A_n` parameter that batch 5 (3512543ee) added in the hand-written partials and new
  files: 111 declarations, 276 uses in 41 files. The pre-batch `Generated` stubs carried the original
  metadata names where present (e.g. `ColumnMemento(column)`), so only the metadata-less `A_n` names
  needed descriptive names: from the parameter type (`mathNode`, `text`, `index`, ...), with
  ordinal words for obfuscated types. Names avoid every identifier used in the file.
- No named-argument call sites of these methods in `Quire.EquationEditor`. The `A_0:` named calls in
  `Quire.Engine.Utl` target `Dispose(bool)` of another assembly and are unchanged.
- Release IL: `--project Quire.EquationEditor` PASS, 1392 checked, 0 failures.
- Remaining `A_n` names: none in batch-5 lines. Pre-batch hand-written code and `Generated` stubs still use them.

## Rejected members (batch 5 gate); not yet mirrored

- 147 rejected: 140 field-name clashes (`a_`/`b_`/... fields vs Mathcad `a`/`b`) and 7 compile errors.
  The field-name clash group stays parked. The compile-error group is being worked next.

## Batch 7 — IdentifierNodeInfo Value, ValueSExpression, Clone and IsEquivalentTo (hand-written)

- Members, in `Mirrored/EquationEditor.Tree.IdentifierNodeInfo.cs`: `Value` (get/set, `(FormattedString)base.Value`),
  `ValueSExpression`, `Clone()`, `IsEquivalentTo(INodeInfo)`, `IsEquivalentTo(IdentifierNodeInfo)`. Their stubs are
  removed from `Generated/Mirror/EquationEditor.Tree.IdentifierNodeInfo.g.cs` by hand.
- Two IL-shape notes, each with a comment in the source: the nullable comparison uses a non-short-circuit `&`
  (Release IL evaluates both operands), and the `IsEquivalentTo` guard is written as `if (!(...)) return false;` so the
  branch is `brtrue` as in Mathcad.
- Still stubs (not in this batch): the constructors (Mathcad stores `algorithm` before the base call, which C# cannot
  express), and `IdLabel` get/set (closure over `CommandBuffer`, needs compile-order placeholders).
- Release IL: `--project Quire.EquationEditor` PASS, 1398 checked, 0 failures (was 1392).
