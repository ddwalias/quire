# Lane F — Quire.Mpl

- Owner: Lane F. Branch `mirror/f-mpl-20261024-mplrun` (from `main` e4c77df0); worktree `C:/Projects/Quire-wt/lane-f-mpl`.
- Original: `mplrun` (mplrun.dll), split project; depends on `Quire.Mpl.Emit` (read-only for this lane).

## Baseline (Release IL, `release_il.py --base main --project Quire.Mpl`, base e4c77df0)

- PASS: 1867 bodies exact, 0 failures, 0 unfinished, 0 engine_native, 0 native_pairs.
- Inventory: 2032 candidate methods; 165 declaration-only, all body-less in the original (abstract and delegate members).
- Coverage: the mplrun IL listing has 2032 `.method` declarations, and the candidate maps to 2032 distinct original tokens. No original method lacks a counterpart.
- Stubs outside the gate: 26 `[MirrorStub]` field and const declarations in six types. The gate checks bodies only.

## Claimed group (step 1) and change (step 3)

Stub-marked fields and consts that were the only declarations of members the hand-written code already uses. Each declaration moved into the type's hand-written partial with the same name, type and value; the stub was removed from the `.g.cs`.

| Type | Members | Hand-written file |
|---|---|---|
| Mpl.Run.Keyword | `KeyDefun`…`Nkeys` (16 consts) | `Run/Atom.cs` |
| Mpl.Run.BindingInfo | `Value`, `Macro` (2 consts) | `Run/Env.cs` |
| Mpl.Run.Primitive | `m_name`, `m_opcodes`, `m_code`, `m_method`, `m_foreign` | `Run/Primitive.cs` |
| Mpl.Run.CloDblFunction | `m_clo` (double) | `Run/CloDblFunction.cs` |
| Mpl.Run.CloIntFunction | `m_clo` (int) | `Run/CloIntFunction.cs` |
| Mpl.Run.CloObjFunction | `m_clo` (object) | `Run/CloObjFunction.cs` |

Original declarations (mplrun IL `.field` lines) match names, types and literal values. Reference IL cached via `tools/reference/reference.py <type> --il`.

## Result (step 4)

- Release IL: PASS. 1867 of 1867 bodies exact, 165 declaration-only, 0 failures (report `.mirror-cache/release-il-7c4bc33b2704/release-il.json`, base e30c79ab).
- `[MirrorStub]` markers left in `src/Quire.Mpl`: 0.
- Remaining mismatches: none.
- The known msgen/msclr question did not arise: no mismatch traced to those dependencies.

## Status

- Assembly `Quire.Mpl`: complete for the full Release inventory (bodies, declarations, coverage and stub markers). Declared on this evidence, pending the `lane_merge.py` re-validation on current `main`.
