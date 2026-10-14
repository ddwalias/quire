# Validating a mirrored batch

Read the selected members' evidence records first. Test success supports only the recorded
cases; it does not establish method-wide equivalence or authorize status changes by itself.

## Runtime batch: one repeatable command

From the repository root, with .NET SDK 10 and Python 3 installed:

```sh
python3 tools/validation/validate.py --diff-filter 'FullyQualifiedName~FunctionMatrixDifferentialTests'
```

On Windows use `python` if that is the installed Python command. Set `MATHCAD_DIR` to the
**directory** of the original install when it differs from the default. The default focused
profile builds/runs `Quire.Harness.Tests`, all `Quire.Mpl.Tests`, and the maintained differential
sources through `Quire.Focused.Differential.Tests`. Omit `--diff-filter` for all runtime
comparisons; use a VSTest expression such as
`FullyQualifiedName~MatVal|FullyQualifiedName~MatDim|FullyQualifiedName~FunctionMatrix`
for the matrix slice. The filter applies only to differential tests. It does not filter regressions.
This profile covers mplrun, not engine/UI/other-project behavior; also run affected tests there.

The focused project links the differential sources, targets `net10.0`, and references only
Quire.Mpl. Its assembly name stays `Quire.Differential.Tests` to retain the existing internal
access grant. It has its own output directory. Do not copy tests into it or add it to the solution:
the solution already runs the same sources through the original Windows project.

The command writes project logs, TRX files and `summary.json` in a new temporary directory and
prints its path. Each project has a 600-second timeout; `--timeout SECONDS` overrides it.
`--results PATH` selects a **new** directory; existing directories are rejected
so stale results cannot be accepted. `--dotnet PATH` selects another .NET CLI. The report records
HEAD, working tree changes, original mplrun path/hash, exact commands, filter and counts. Inspect
failures in the named project log and TRX. A nonzero process exit, aborted run, no executed tests,
any failure or skip fails validation. No Mathcad install is a failure in this mode. A changed
mplrun SHA-256 or loaded MVID fails the differential harness; revalidate contracts before changing
the pinned identity in `Mathcad.cs`.

`--allow-differential-skips` permits some skips for exploratory work, records them, and exits 2
for an incomplete run. An all-skipped suite still fails. Such results cannot support Verified.
Unwrapped `dotnet test` without `QUIRE_REQUIRE_MATHCAD=1` retains the original optional-install
skip behavior; a green console result alone is therefore insufficient.

## Harness checks

```sh
python3 -m unittest discover -s tools/validation -p 'test_*.py'
dotnet test tests/Quire.Harness.Tests/Quire.Harness.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false
```

These check rejection of false-green reports, reflection ambiguity/missing members, static versus
instance calls, ref parameter selection/mutation, propagation of formatter failures, comparison
of actual target and deferred iterator exceptions, and detection of differing outcomes.
Differential tests run serially because they change runtime defaults/caches on both sides.

When reflection arguments are ambiguous, use `CallSig` and the exact parameter type names.
`CallSig` currently matches **simple** type names; verify full parameter identity from the member
contract and reflection metadata when types share names. The helper fails when multiple declarations
match. Missing members or ambiguous selection are harness errors, never comparable runtime results.
Use an explicit result renderer when fields, type, dimensions, identity or mutation matter:
`Side.Show` falls back to type names for unrecognized mirrored objects, so equal text alone may
omit relevant state. Do not use a vacuous scenario; `Scenario.Compare` requires outcomes on both sides.

## Windows and phase closure

```sh
python tools/validation/validate.py --profile phase
dotnet test Quire.slnx
```

Run these on Windows with .NET 10 and the Windows Desktop runtime. The phase profile runs all
solution test projects separately with strict reports (including metadata shape/status tests),
without a differential filter. The solution command remains PLAN's explicit closure check.
Engine and UI tests require their own appropriate runtime environment. Never retarget WPF projects
or omit engine tests to claim phase closure. Linux focused tests do not substitute for those checks.

The status gate checks that references resolve; it does **not** prove that a differential class
covers each attributed member or that its comparisons ran. The shape gate checks missing expected
manifest lines, not behavioral fidelity. Neither gate alone validates coverage-excluded interfaces,
enums, delegates, artifact-bound evidence, or an accepted deviation's tested behavior. Inspect the
selected records, refresh/review shape/status inventories when required, check planned stub reduction,
perform `Quire.exe --render` screenshot validation, and obtain the independent reviewer pass required
by PLAN. Do not auto-accept manifest removals or rewrite statuses to make a gate green.

## Audit in this sandbox (2026-10-14)

Base checkout: `18bec37`, isolated from the implementation agent. The shared `.git` is read-only,
so the worktree was created from a local clone at `/tmp/quire-harness-repo`, with working files at
`/tmp/quire-harness-validation`. No production implementation was changed.

Single-process builds of Mpl behavior, focused differential and harness projects succeeded.
VSTest cannot create its communication socket in this sandbox; Windows CLI interop also cannot
create its WSL socket. The strict command was exercised and correctly returned failure with an
incomplete JSON report for that aborted run. Ordinary test execution and Windows phase gates
remain to be run in an environment permitting those operations. Direct fact invocation was used
only as a supplementary smoke check, not as a substitute VSTest result or a phase closure claim.

Final supplementary results: **123 Mpl facts / 57 differential facts / 11 harness facts passed,
0 failed, 0 skipped** through direct invocation. The differential checks used the installed
artifact matching the pinned SHA-256 and MVID. Missing-install and changed-artifact negative
smoke checks both failed as intended. Python result-parser checks: **3 passed** (including
multiple empty/skipped/failed/aborted/unknown-outcome fixtures). `git diff --check` passed.
These counts are supplementary checks, not VSTest results; no member statuses were upgraded.

The audit exposed and corrected shared-helper failures that previously could compare as equal
exceptions: override enumeration counted inherited declarations twice; static and instance
methods competed; `Function.Apply` needed an exact array signature; the `DimVal.nearly_equal`
scenario omitted `Double` from its explicit signature. Fixing these selected the intended bodies,
and the resulting full runtime comparisons passed. Result-rendering errors now escape comparison,
while deferred iterator execution remains part of the comparable runtime operation.
