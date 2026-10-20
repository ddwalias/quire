# Exact Release IL workflow

**Goal:** the produced Release IL matches Mathcad Prime 12.0.1.0 for each selected member.
This workflow replaces the former behavior, differential, coverage, mutation and phase
gates for all managed mirroring batches, including Tiers A, A2 and B (decision D17).
Existing scope and name mappings remain in [scope.md](scope.md), [tiers/](tiers/)
and [mapping.md](mapping.md). The four assembly assignments in [WORK](WORK.md) supersede
historical tier deferrals (D18). Native work does not acquire a managed IL counterpart.

## Five steps

1. **Claim.** Read [WORK](WORK.md) and the selected lane's active ownership. Use a separate
   branch/worktree from current `main`; avoid another agent's files. Choose a coherent set
   of members. There is no minimum batch size or separate approval step.
2. **Read the reference.** Use `python tools/reference/reference.py <Mathcad type> --il`
   (`--assembly <name>` outside mplrun). Reuse the artifact-bound local cache. Read only
   the selected members and helpers needed to reconstruct their IL.
3. **Implement.** Preserve the original declarations, instructions, operands, branches,
   locals and exception regions. Replace corresponding stubs and use the existing name
   mappings. Work only from Release IL; never inspect Debug IL.
4. **Compare.** Run `python tools/validation/release_il.py --base main --project <project>`.
   It builds base and candidate production projects in Release, compares changed methods
   with Mathcad and retains every previously exact match in the affected projects.
   Add `--type <Quire type>` or `--method '<Type>::<signature>'` to check selected members
   even when their bodies were already unchanged. Fix mismatches and repeat this step.
5. **Land.** Read the diff, record owner/branch/member scope/report/remaining mismatches
   in the lane checkpoint, commit, then run `python tools/validation/lane_merge.py <branch>`.
   It refreshes `main`, rebases, validates the committed Release IL in a temporary clone,
   fast-forwards and pushes under one merge lock. Reuse an active lane's worktree for
   subsequent batches; remove it when that lane finishes.

## Acceptance

All changed methods with bodies must match their original Release IL. Every previously
exact method in affected projects must stay exact; removed methods fail. Matching abstract
or delegate declarations need no body. New/changed methods without a counterpart, unreadable
IL and structure-only matches fail. Compiler-generated bodies are not silently exempted;
an unresolved generated counterpart blocks the affected change.

Comparison resolves metadata tokens to original member/type names using the existing map;
Quire's renamed assemblies are not expected to be byte-identical DLLs. Opcode forms, operand
values, instruction order, branch destinations, local slots/types, initialization, max stack
and exception regions count. Standard framework type names are compared across the framework
versions. See [validation](validation.md) for commands and practical limits.

Previously unfinished methods outside the changed/selected scope do not block an independent
batch. Calling an unfinished callee does not prevent matching the caller's IL; it does not
mark the callee complete.

## Keep the loop small

No batch requirement for behavior/differential tests, coverage, mutation, baseline ratchets,
status promotion, per-member contract documents, reviewer/maintainer approvals or phase closure.
Existing product tests remain available as optional diagnostics. Their results do not determine
Release IL acceptance. A short checkpoint and the generated IL report are sufficient evidence.

Original install: `C:\Program Files\PTC\Mathcad Prime 12.0.1.0`; override with `MATHCAD_DIR`.
Reports bind comparisons to the original SHA-256, MVID and member tokens. Original binaries,
decompiled C# and IL stay in `.mirror-cache` and are never committed.
