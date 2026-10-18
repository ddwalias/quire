# Windows batch validation and handoff

## Workspace

All new agent sessions use native Windows Git in the shared clone `C:\Projects\Quire`. Create one branch and
worktree per agent under `C:\Projects\Quire-wt\<unit>`. Never share a checkout between agents. The repository
remote is `ddwalias/quire`; GCM is configured to prefer the `ddwalias` account.

The existing `MplForeignMacro` batch is owned and has uncommitted changes. Do not edit or duplicate it. Select
another unowned batch from [WORK.md](../WORK.md).

## Batch loop

Use one coherent class or functional cluster of about 15–30 methods. Run only the affected behavior and differential
tests locally; the runtime gate performs full validation and mutation.

```powershell
$U = '<lane>-<unit>'; $B = "mirror/$U"; $W = "C:\Projects\Quire-wt\$U"
python C:\Projects\Quire\tools\validation\sweep.py
git -C C:\Projects\Quire fetch -q origin
git -C C:\Projects\Quire worktree add $W -b $B origin/main
Set-Location $W
$env:QUIRE_REQUIRE_MATHCAD = '1'
dotnet test tests\Quire.Focused.Differential.Tests\Quire.Focused.Differential.Tests.csproj --filter 'FullyQualifiedName~<TestClass>'
git add <paths>
git commit -m '<batch summary>'
git push -u origin $B
python C:\Projects\Quire\tools\validation\lane_merge.py $B --jobs 14
Set-Location C:\Projects
git -C C:\Projects\Quire worktree remove $W
```

Read the full diff and update the lane checkpoint with scope, evidence, commands, results, remaining work and blockers.
Do not commit directly to `main`; `lane_merge.py` rebases, runs the gate, and pushes `main` only after a passing
gate. If `main` advances during validation, the wrapper retries against the new base. Do not edit a branch while its
gate is running.

Agents may continue on a fresh worktree during a gate only when changes do not overlap the gated batch's files or
shared helpers. The maintainer may combine at most two ready batches into one final gate; keep their checkpoints
separate.

Do not run the full Python harness suite for ordinary batches. It is a maintainer check for changes to
`tools/validation` and took about 17 minutes for 263 tests on this machine.

## Windows phase gate

Phase closure is still open; check [WORK.md](../WORK.md) for its current status. Changes outside the focused
`Quire.Mpl` profile may also need the Windows phase profile. Check project routing before choosing a batch. If the
gate encounters the documented phase-baseline or method-audit blocker, record it and continue only with independent
work that can still be merged.

## Mutation results

The gate owns the mutation pass. Fix all live survivors it reports before retrying. A provably equivalent survivor or
unreachable line needs a written proof in the checkpoint; the maintainer records accepted entries and runs the
protected gate.
