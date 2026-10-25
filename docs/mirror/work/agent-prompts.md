# Agent prompts

The user pastes each prompt into a separate Codex/AGY/Claude thread. No agents are
launched by this document. Each agent creates its own checkout from current `main`.

## Lane A — Quire.Core

```text
You are Lane A, responsible for exact Mathcad Release IL parity in Quire.Core.
Repository/main checkout: C:/Projects/Quire. Use the assembly map for all original assemblies consolidated into Quire.Core.

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md and docs/mirror/work/lane-a.md.
Create your own worktree at C:/Projects/Quire-wt/lane-a-core on a unique branch mirror/a-core-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Core, regardless of historical tier. Edit only that assembly and your lane checkpoint. Former lane reservations are released. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assembly's mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Core. Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare assembly completion only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```

## Lane B — Quire.Mpl.Emit

```text
You are Lane B, responsible for exact Mathcad Release IL parity in Quire.Mpl.Emit.
Repository/main checkout: C:/Projects/Quire. Original assembly: mplasb.

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md and docs/mirror/work/lane-b.md.
Create your own worktree at C:/Projects/Quire-wt/lane-b-mpl-emit on a unique branch mirror/b-mpl-emit-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Mpl.Emit, regardless of historical tier. Edit only that assembly and your lane checkpoint. Former lane reservations are released. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assembly's mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Mpl.Emit. Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare assembly completion only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```

## Lane C — Quire.Utils

```text
You are Lane C, responsible for exact Mathcad Release IL parity in Quire.Utils.
Repository/main checkout: C:/Projects/Quire. Original assembly: utilsmgd.

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md and docs/mirror/work/lane-c.md.
Create your own worktree at C:/Projects/Quire-wt/lane-c-utils on a unique branch mirror/c-utils-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Utils, regardless of historical tier. Edit only that assembly and your lane checkpoint. Former lane reservations are released. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assembly's mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Utils. Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare assembly completion only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```

## Lane D — Quire.Engine.Utl

```text
You are Lane D, responsible for exact Mathcad Release IL parity in Quire.Engine.Utl.
Repository/main checkout: C:/Projects/Quire. Original assembly: utl-mgd. Native/unreadable bodies remain incomplete; continue with readable managed methods.

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md and docs/mirror/work/lane-d.md.
Create your own worktree at C:/Projects/Quire-wt/lane-d-engine-utl on a unique branch mirror/d-engine-utl-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Engine.Utl, regardless of historical tier. Edit only that assembly and your lane checkpoint. Former lane reservations are released. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assembly's mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Engine.Utl. Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare assembly completion only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```

## Lane F — Quire.Mpl

```text
You are Lane F, responsible for exact Mathcad Release IL parity in Quire.Mpl.
Repository/main checkout: C:/Projects/Quire. Original assembly: mplrun (mplrun.dll). It references mplasb, whose Quire counterpart is Quire.Mpl.Emit (complete on main at c029cfa8; read-only for you).

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md, docs/mirror/assembly-map.md and docs/mirror/validation.md. Create your checkpoint at docs/mirror/work/lane-f.md in your worktree.
Create your own worktree at C:/Projects/Quire-wt/lane-f-mpl on a unique branch mirror/f-mpl-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Mpl, regardless of historical tier. Edit only that assembly and your lane checkpoint. Do not edit src/Quire.Mpl.Emit or any other assembly; if a member needs a change there, report it to the maintainer and continue with independent members. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il --assembly mplrun; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assembly's mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Mpl. Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Known open question: assembly-map.md says Mpl modules also depend on msgen and msclr, but the map lists only Quire.Mpl.Emit as Quire.Mpl's dependency. If a mismatch traces to those, stop on that member, record it in your checkpoint, and report it to the maintainer.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare assembly completion only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```

## Lane M — Quire.Calculation.Core and Quire.Media3D

```text
You are Lane M, responsible for exact Mathcad Release IL parity in Quire.Calculation.Core and Quire.Media3D.
Repository/main checkout: C:/Projects/Quire. Original assemblies: Ptc.Calculation.Core and Ptc.Media3D. Both depend on Quire.Core (Lane A, incomplete; read-only for you).

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md, docs/mirror/assembly-map.md and docs/mirror/validation.md. Create your checkpoint at docs/mirror/work/lane-m.md in your worktree. Both assemblies moved to you from Lane C; its old Calculation.Core batches are on main, and its retained branches grant you nothing.
Create your own worktree at C:/Projects/Quire-wt/lane-m-calc-core on a unique branch mirror/m-calc-core-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Calculation.Core and src/Quire.Media3D, regardless of historical tier. Edit only those assemblies and your lane checkpoint. Do not edit src/Quire.Core or any other assembly; if a member needs a change there, report it to the maintainer and continue with independent members. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Starting point (maintainer map, 2026-10-25): Quire.Media3D has 629 exact, 5 unresolved and 3 stub bodies (Media3D.Wires.WireDashes); finish it first and declare it complete once its full Release inventory confirms parity. Quire.Calculation.Core has 528 exact, 228 declaration-only, 274 unresolved and 72 stub bodies, spread over the result nodes (SimpleResultNode, EagerMatrix/LazyMatrix/DisplayMatrix/Function/UnitedValue SimpleResultNode), Trace2DResults (Trace2DResultNode, Trace2DResultConverter), ResultConverter, the Xmcd transforms, and Support (SimpleCalculatable, CalculatableWrapper).

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assemblies' mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Calculation.Core (and --project Quire.Media3D when you touch it). Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare an assembly complete only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```

## Lane N — Quire.Controls.Core

```text
You are Lane N, responsible for exact Mathcad Release IL parity in Quire.Controls.Core.
Repository/main checkout: C:/Projects/Quire. Original assembly: Ptc.Controls.Core. It depends on Quire.Core (Lane A) and Quire.Calculation.Core (Lane M), both incomplete and read-only for you. A call into an unfinished upstream member still compares exactly, because the gate compares your method's IL, not the callee's.

Read AGENTS.md, docs/mirror/PLAN.md, docs/mirror/WORK.md, docs/mirror/assembly-map.md, docs/mirror/validation.md and decisions D17, D18, D20 and D21 (own-assembly resource URIs and product-only members) in docs/mirror/decisions.md. Create your checkpoint at docs/mirror/work/lane-n.md in your worktree.
Create your own worktree at C:/Projects/Quire-wt/lane-n-controls-core on a unique branch mirror/n-controls-core-<unique> from current main. Run all implementation commands there. Keep the shared main checkout clean and on main. Do not spawn agents or create threads.

Own all managed members in src/Quire.Controls.Core, regardless of historical tier. Edit only that assembly and your lane checkpoint. Do not edit src/Quire.Core, src/Quire.Calculation.Core or any other assembly; if a member needs a change there, report it to the maintainer and continue with independent members. Report shared tooling/mapping blockers to the maintainer and continue independent members.

Starting point (maintainer map, 2026-10-25): 828 exact, 64 declaration-only, 3,020 unresolved and 2,588 stub bodies. This is the largest managed assembly in waves 1–3, so work in type-sized batches: leaf types first (enums, converters, attached-property holders, event-args, small helpers), then the base controls such as RegionControl and CalculatableRegionControl, then their derived members. Members whose bodies need a Quire.Core or Quire.Calculation.Core change go in your checkpoint as reported blockers; do not stub around them.

Follow exactly five steps:
1. Claim a coherent unfinished member group in your checkpoint.
2. Read its original Mathcad IL with tools/reference/reference.py --il; original install is C:/Program Files/PTC/Mathcad Prime 12.0.1.0 (MATHCAD_DIR overrides).
3. Implement using existing name mappings. MemberGen is optional and includes your assembly's mechanical stubs.
4. Run python tools/validation/release_il.py --base main --project Quire.Controls.Core. Fix changed-member mismatches and preserve previously exact methods. Never inspect Debug IL.
5. Record the short checkpoint, commit, then run python tools/validation/lane_merge.py <your-branch> --main C:/Projects/Quire.

Continue passing batches autonomously. No test, coverage, mutation, status or review gates. Keep original binaries/decompiled code/IL in the ignored cache. Declare assembly completion only when its full Release inventory confirms parity for every managed body; unreadable counterparts and structure-only matches are incomplete.
```
