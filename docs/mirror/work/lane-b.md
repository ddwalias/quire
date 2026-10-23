# Lane B — Quire.Mpl.Emit

- Owner: Lane B (idiom cleanup).
- Branch: `mirror/b-mpl-emit-idiom-202610231735`.
- Worktree: `C:/Projects/Quire-wt/lane-b-mpl-emit`.
- Acceptance: `python tools/validation/release_il.py --base main --project Quire.Mpl.Emit` must PASS with 682 methods exact.

## Batch 1 — goto/label removal

- Scope: `HandleResolvers.cs`, `MethodHandleResolver.ResolveMethodBase.cs`, `MethodHandleResolver.ResolveMethodInfo.cs`, `MethodHandleResolver.ResolveConstructor.cs`, `Logger.ProcessLogQueue.cs`. 23 gotos removed; switch type patterns and `when` guards replace the labels; `ProcessLogQueue` is two nested `while` loops.
- Report: `.mirror-cache/release-il-83e6d40fb882/release-il.json` (PASS, 682 checked).
- Failed idiomatic forms: none.

## Batch 2 — decompiler-style local and parameter names

- Scope: digit-suffixed locals/parameters/lambda parameters in 19 hand-written files (`value2`, `item4`, `type6`, `methodBase5`, `handler3`, `num2`, `arg1`, and others). 274 renames, applied by line.
- Excluded: `Probe0`/`Probe1`/`Probe_Int32_Object` (probe names, not locals); the `v10` and `net10.0` strings; `kernel32`.
- Report: `.mirror-cache/release-il-9526d9e04f7b/release-il.json` (PASS, 682 checked).
- Failed idiomatic forms: none.

## Remaining mismatches

None reported at batch 2.
