# Validation workflow checkpoint

Branch: `mirror/workflow-trim`, based on `main` `fff8ec0`.

## Scope

- Normal handoff uses targeted behavior/direct differential tests; the runtime gate owns full validation.
- Seven explicitly named prose files can reuse the base's validation only for edits to existing UTF-8 regular
  non-executable files. Evidence, configuration, tooling, new files and mixed changes use the runtime gate.
- Phase closure builds the solution, then runs the strict phase validator; the extra solution test invocation
  is removed. The phase runner checks that its selection includes every solution test project.
- Squash once before queueing, rebase only when needed, and keep batch results once in the lane checkpoint.

Changed code: `tools/validation/merge_gate.py`, `validate.py`, and their Python unit tests. Workflow documentation:
PLAN, validation.md and the short WORK index. The existing mutation selection, survivor approvals, runtime
acceptance rules, independent review, and separate differential/coverage passes remain in force.

## Validation and next action

`python3 -m unittest discover -s tools/validation -p 'test_*.py'`: 156 passed, no failures or skips. This includes
prose check/apply routing, unchanged baseline bytes, protected/mixed/non-text changes, explicit audit overrides,
stale branches and phase project selection. CLI help and `git diff --check` passed.

Independent review passed with no blockers. A real CLI check/apply smoke run in a scratch repository completed
the prose route without invoking .NET and preserved the baseline bytes (0.08 s on this workstation; not a full
workflow benchmark).

This tooling branch passed the full protected runtime gate and was merged as `c011f35`; receipt:
`/tmp/quire-workflow-trim-runtime-gate/gate.json`. The branch and main were pushed. Resume the implementation
lanes linked from WORK. This change does
not establish Windows phase completion.

## Two-batch integration amendment

The user requested following PLAN and improving any demonstrably wasteful execution step. Two independently
reviewed queued batches may now share one unchanged final merge gate, with at most two batches per integration
candidate. Each retains its own coherent scope, squashed commit, ownership and checkpoint. Full combined-diff
review and all required approvals precede validation; audit triggers use the union of both batches and count
logical batches so frequency is retained. No gate implementation, test/mutation selection, acceptance rule or
evidence requirement changes. Only the final candidate is validated, and the entire candidate fails together.

Motivation: the pending runtime test work shares the same build, regression, coverage and audit stages; an
accepted-mutant change also requires a full mutation audit (the current inventory contains 2,112 mutants).
Serial gates for two such queued batches would repeat that complete pass. This amendment permits one final
pass after both are ready, without using an interrupted/failed run as evidence. It does not claim a measured
end-to-end speedup. Protected changes are only PLAN and validation.md. Independent review passed for `8ab21a1`; the existing
protected runtime gate passed Harness 36, Mpl 159 and Differential 79, with no failures/skips, lint 0 and ratchet
ok. No methods were selected for mutation. The gate merged the amendment as `7c5d9aa`, and main was pushed.
Receipt: `/tmp/quire-gate-batching-runtime-gate/gate.json`; protected paths match exactly those two policy files.

The earlier `02ecce1` full audit at `/tmp/quire-matval-final-runtime-gate` was interrupted after
1,180/2,112 mutants when unmerged DimVal comparison fixes and their pending approvals were identified.
Its gate receipt is blocked (validator exit 241, no sealed summary); it is not a passing audit or merge
authorization. The corrected candidate still requires a fresh complete full audit.

## Phase profile from WSL and Windows-project gating

`validate.py --profile phase` now runs from WSL through Windows' `dotnet.exe`: `--dotnet`, else
`QUIRE_WINDOWS_DOTNET`, else `/mnt/c/Program Files/dotnet/dotnet.exe`. The checkout, results and `QUIRE_WINDOWS_TEMP`
must be on `/mnt/<drive>`; paths are converted with `wslpath -w`, test variables are forwarded through `WSLENV`, and
the build's PDB/coverage paths are read back through `/mnt`. MethodAudit also runs under `dotnet.exe`. Phase runs
ratchet against `docs/mirror/validation-baseline-phase.json`. For a branch whose `src/`/`tests/` changes the focused
profile does not cover, the merge gate runs the phase profile after the focused run where that CLI is available, and
passes only when both pass; `--apply` commits both baselines. Without the CLI it blocks as before. See
[validation.md](../validation.md#phase-profile-from-wsl).

First phase run of main from WSL: all nine phase test projects passed, 0 skipped
(Calculation 8, Controls 16, Core 7, Differential 272 plus its coverage run, Engine 94, EquationEditor 88, Mirror 32,
Mpl 188, Harness 86). Harness needed this commit's fix to `OptimizedBuildTests`: on Windows an assembly loaded by
path stays locked, so the test's cleanup failed; it now loads from bytes. The method audit then failed for reasons that are independent of
WSL. MethodAudit crashes on `Quire.Engine.Native` (`Program.cs:135`, two Mathcad methods match one key), and
several projects report `MirrorOf` and type-map inspection errors (Efi 396, Mpl.Modules 82, Mcd 9, Quire 2, and a
shared `Quire.ProductInfo.Constants` error that appears once in each of 17 more projects). So there is no phase
baseline yet, and until a maintainer fixes these, the gate's phase run blocks every Windows-project branch with
these specific reasons. Phase mutation is not implemented.

Review follow-up: the gate commits `main`'s phase baseline unchanged on every merge without a phase run, so no
branch can change either baseline, even with `--allow-protected`/`--allow-unvalidated`. A branch needing the
phase run is blocked before any run while `main` has no phase baseline (a maintainer bootstraps it on `main`;
see validation.md) or the Windows temporary directory is unusable. The gate keeps the 3 newest phase run
directories.

## Merge gate throughput

Lanes follow PLAN's pipeline: worktrees of the ext4 clone `/root/src/quire-main`, batches of a whole class or
functional cluster, local checks without mutation, and one mutation pass in the gate. A batch goes through this
template (Windows-project work stays under `/mnt/c` instead):

```sh
U=b-<unit>; B=mirror/$U; W=/root/src/worktrees/$U
find /tmp -maxdepth 1 \( -name 'quire-validation-*' -o -name 'quire-coverage-*' -o -name 'quire-audit-*' \
  -o -name 'quire-gate-*' \) -mmin +720 -exec rm -rf {} +             # stale scratch only, never a running gate's
git -C /root/src/quire-main fetch -q origin
git -C /root/src/quire-main worktree add "$W" -b "$B" origin/main
cd "$W"
# implement the batch, then the fast local checks (no --mutate):
QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj \
  --filter 'FullyQualifiedName~<TestClass>'
python3 tools/validation/validate.py --profile focused --method-type <Type> --jobs 14
# fast, advisory check of only the changed methods in this worktree; also accepts --method-type or --method:
python3 tools/validation/quick_check.py
git add <paths> && git commit -m '<summary>' -m 'Co-Authored-By: <model> <noreply@anthropic.com>'
sh /root/src/lane-merge.sh "$B"                                       # validate, then land and push
cd /root/src && git -C /root/src/quire-main worktree remove --force "$W"
```

When a method is not exact, `python3 tools/validation/il_diff.py <Type | Type::Name | Type::signature> ...` prints its
first Release-IL instruction that differs from Mathcad's, with context on both sides and the Quire source line (stdout only).

`lane-merge.sh` (keep a copy at `/root/src/lane-merge.sh`) validates without the merge lock and lands under it.
Run it in the worktree that has the branch checked out (it rebases there); it exits with the gate's code. `MAIN`
defaults to the worktree that has `main` checked out, where the gate must run:

```sh
#!/bin/sh
B="$1"                                    # run in the worktree that has $B checked out
MAIN=${MAIN:-$(git worktree list --porcelain | awk '/^worktree /{w=substr($0,10)} /^branch refs\/heads\/main$/{print w}')}
[ -n "$MAIN" ] || { echo "no worktree has main checked out; set MAIN" >&2; exit 1; }
GATE="python3 $MAIN/tools/validation/merge_gate.py"
LOCK=/tmp/quire-merge.lock
PULL="git fetch -q origin && git merge -q --ff-only origin/main"
PUSH="git push -q origin main"
stale=0
while :; do
  flock "$LOCK" sh -c "cd '$MAIN' && $PULL" || exit $?                    # fresh main, briefly under the lock
  out=$(git rebase -q main "$B" 2>&1) || { echo "git rebase main $B failed: $out" >&2; exit 1; }
  (cd "$MAIN" && $GATE "$B" --validate-only --jobs 14) || exit $?         # BLOCKED: fix the branch
  flock "$LOCK" sh -c "cd '$MAIN' && $PULL && $GATE '$B' --apply-validated && $PUSH"
  code=$?
  [ $code -eq 0 ] && exit 0                                              # merged and pushed
  [ $code -ne 4 ] && exit $code                                          # blocked, or the push failed
  stale=$((stale + 1))
  [ $stale -lt 2 ] && continue                                           # stale: validate again
  # Two stale results in a row: the one-shot gate, holding the lock for the whole validation.
  flock "$LOCK" sh -c "cd '$MAIN' && $PULL && git -C '$PWD' rebase -q main '$B' && $GATE '$B' --apply --jobs 14 && $PUSH"
  exit $?
done
```

Between `--validate-only` and `--apply-validated` do not rebase: the record is bound to the tip, and apply itself
replays the branch onto a moved `main` or exits 4. Inside `sh -c` everything is chained with `&&`, so the push
runs only after a merge and the gate's exit code (1 blocked, 4 stale) comes back out of `flock`. Exit 4 means
`main` gained something that can change the verdict; the script validates again, and after two stale results in
a row falls back to the one-shot `--apply`, which cannot go stale but holds the lock for the whole validation
(the stale message says for how long). Rules: validation.md, "Validating outside the merge lock".

- **Batch members.** Every gate run pays about 3 minutes before any mutant runs (build, the differential suite
  twice, the audit), and local checks no longer mutate, so a batch is a whole class or a functional cluster of
  about 15–30 methods (the remaining Function methods; whole MatVal or MatDim operations), one commit per batch.
  Its single mutation pass is the gate's.
  Pick it with `python3 tools/validation/batch_picker.py <method-coverage.json> --type-prefix Quire.Mpl. --type-prefix
  Quire.Mcd.` (or `--method-type <Type>`): remaining Tier A members ranked by gain, members with exact IL that only
  need a comparison first, then leaf callees first, plus totals per type.
- **Phase branches.** A run that includes the phase profile goes stale on any concurrent code merge and takes much
  longer, so batch Windows-project work more aggressively, and expect the one-shot fallback more often.
- **Mutation scope** is unchanged: changed methods, comparing tests, direct callers and field readers. A broad
  rewrite such as an idiom cleanup changes IL in hundreds of methods and is legitimately long (gate37: 1,326
  mutants, 68 minutes); with the lock covering only landing, it no longer blocks other lanes. The nightly
  `merge_gate.py --nightly` covers the residual risk of the narrow selection.
- **The differential suite runs twice** on purpose: coverage instrumentation changes the assembly hash and method
  tokens that the case records need (measured; see validation.md).
