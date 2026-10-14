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
After test execution, every selected method must pass the direct method gate described below.
The existing test suite has not yet been migrated to this requirement; passing tests can therefore
still produce a failing validation report with precise coverage gaps.
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

`StructureTests` check that locals and expression bodies do not change a structure while swapped calls, extra
branches, constants, catch types and throws do.

These check per-method record validation and rejection of false-green reports, reflection ambiguity/missing members, static versus
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

## Direct comparison for every method

The validator now audits compiled method definitions, independently of the reduced shape/status
inventory. It includes **private/public/protected/internal methods, constructors, static constructors,
property getters/setters, event add/remove methods, explicit interface implementations and overloads**.
Methods on non-mirrored helper types are included too, so missing Mathcad counterparts are visible.
Compiler-generated types/names are explicitly reported as excluded under PLAN's scope rule.
Matching abstract methods and CLR delegate declarations without bodies are reported separately as
declaration-only. A Quire-only/deviation/status annotation does not silently exempt a method from
this strict comparison policy; methods without counterparts need reviewed resolution.

Private methods require **direct invocation on both artifacts**. Calling a public method that happens
to reach the private method does not count. The same conservative policy applies to other methods.
The metadata audit checks `MirrorOf` against the reviewed type map, then matches declaring types, assembly-qualified parameter/return
types, generic arity and static/instance signatures, and checks accessibility. It records both tokens,
versions, MVIDs and SHA-256 values. Existing shape/status checks remain necessary for broader shape,
inheritance, constraints, evidence classifications and phase closure requirements.

`method-inventory.json` is the complete compiled inventory for the selected project(s).
`method-coverage.json` identifies each method as compared, declaration-only, excluded or missing,
with reasons, exact identities, case/test/contract references and totals by accessibility.
Validation fails if **any** selected method lacks a direct passing comparison. `Verified` attributes
and existing test-class names alone do not count. Stubs and blocked methods cannot pass this gate.

For a batch, add `--method-type Quire.Mcd.Dim` (repeat for multiple exact declaring types). The report
records that scope; a scoped pass is not a project-wide pass. Unknown types and empty selections fail.
The focused default audits every method in Quire.Mpl; the Windows phase profile audits all manifest
projects and disallows type filters. A differential filter selects tests, not covered methods: selecting
too few tests makes the method gate fail rather than shrinking its method inventory.

### Writing an attributable case

Use `MethodComparison.Compare` inside an xUnit fact/theory. See
`tests/Quire.Differential.Tests/PrivateMethodCoverageTests.cs` for a runnable private-method example.
Select each overload with its exact parameter types and `DeclaredOnly`; choose the actual override
when virtual dispatch could select another body. Close generic types/methods explicitly.

```csharp
MethodComparison.Compare("specific input and edge condition", "Mcd.Dim",
    quireMethod, quireTarget, quireArguments,
    originalMethod, originalTarget, originalArguments,
    quireShow: DescribeQuireResult,
    originalShow: DescribeOriginalResult);
```

The second argument names a contract record in `docs/mirror/evidence` without `.md`. The record must
contain the actual original token, SHA-256 and MVID. The helper invokes each selected body and asserts
matching rendered inputs, outcomes and mutated ref/out arguments. Object/array/constructor results
require explicit renderers. Instance methods also require `quireState` and `originalState` callbacks
that describe relevant target state before and after invocation. Include relevant fields, dimensions,
identity relationships, mutation, events, side effects and error details in observers or further test
assertions as required by that method's contract; a constant renderer is not a useful test.

The normal validator sets a fresh run ID and output directory. After a comparison passes, the helper
writes a case record bound to the actual loaded assemblies and executing xUnit method. The gate
accepts it only when that **same test passes in this run's TRX** and the recorded contract remains
unchanged. Records for another token, overload, artifact, MVID or run do not cover a method. Failed,
skipped, aborted, missing or stale tests cannot supply coverage. Tests using other comparison helpers
must migrate to this API before they count; existing passing behavior tests are not reclassified.
No method status is upgraded automatically.

### Reconstruction integrity

The same run also checks each reconstruction against the original IL:

- **Structure** (`method-inventory.json` → `method-coverage.json`): MethodAudit reads both bodies and compares
  the ordered calls (`call`/`callvirt`/`newobj`/`ldftn`), field reads/writes, type operations (`isinst`,
  `castclass`, `box`, `newarr`, `ldtoken`), constants (sorted; `0`/`1` ignored), exception regions and catch types,
  conditional-branch and switch-target counts, `throw`/`rethrow` counts. Quire types/methods are named after their
  Mathcad binding; locals, nops, stack shuffling, branch encoding and compiler-generated names are ignored, so a
  Debug build can match. A difference makes the method `structure_mismatch` with the first differing item. Only a
  `Deviation` status may differ. A matching structure never substitutes for a direct comparison.
- **Lint** (`integrity.json`): hand-written `src/` code must not contain `goto`, compiler-generated names, IL
  labels, decompiler comments or decompiler-style locals (`num2`, `type2`, `obj1`). A reviewed exception carries
  `// quire-lint: allow <reason>` on that line.
- **IL listings** (`integrity.json`): every type with a selected non-stub, classified method needs its listing in
  [il/](il/), whose header must name the SHA-256 and MVID of the artifact the audit loaded. Listings for assemblies
  the run did not load are reported as unchecked.

`python3 tools/validation/integrity.py --inventory <run>/method-inventory.json` repeats the lint/listing checks.
The structure comparison is a fingerprint, not a proof: identical structure with different arithmetic or operand
order passes it, which the direct comparisons must catch.

### Branch coverage, mutation and status

- **Branch coverage:** the differential tests run a second time, unrecorded and `--no-build`, under the Microsoft
  code-coverage collector (cobertura). Instrumentation rewrites the module and corrupts the method tokens that
  comparison records carry, so it never runs in the recorded pass. A compared method whose PDB statement lines are not all executed, or whose branch conditions are partly covered,
  becomes `uncovered`, with the lines named. Coverage counts any execution during the differential run, including
  test setup, so mutation is the attributable check.
- **Mutation** (`--mutate` with `--method-type`, or `--mutate-since REV`): MethodAudit lists same-size, verifiable
  byte changes in each compared method (negated branches, swapped `add`/`sub`, `mul`/`div`, `and`/`or`, `shl`/`shr`,
  `cgt`/`clt`, `0`/`1` swapped, other constants moved by one). Each mutant is applied to a copy of the test output and **only the tests
  whose direct comparisons covered that method** are rerun; one must fail. `mutation.json` lists survivors. A
  survivor means a missing case or a lossy renderer. Equivalent mutants are accepted only by a maintainer in
  `docs/mirror/accepted-mutants.txt` (`<key> # reason`). `--jobs` sets parallel copies.
- **Status:** a `Verified` method that this run does not show as compared (or declaration-only) counts as
  `verified_unsupported`.

### Ratchet

`ratchet.json` compares per-type counts with `docs/mirror/validation-baseline.json`: `compared` may not fall;
`missing`, `structure_mismatch`, `uncovered` and `verified_unsupported` may not rise; unscoped runs also forbid more
lint findings or listing errors and disappearing types. New types start their own baseline. `--write-baseline PATH`
writes a run's counts (unscoped runs only). Only the merge gate or a maintainer updates the committed baseline.

### Merge gate

Work happens on one branch per work unit, based on current `main`. Agents never commit to `main`. From the checkout
that has `main` checked out:

```sh
python3 tools/validation/merge_gate.py <branch>            # check only
python3 tools/validation/merge_gate.py <branch> --apply    # check, fast-forward main, commit the new baseline
```

The gate blocks a branch that is not based on current `main`, has no changes, or changes a maintainer-owned path in
`tools/validation/protected.txt` (harness, MethodAudit, MirrorGen, comparison helper, pinned identity, mirror
attributes, build props, PLAN/validation/deviations/decisions, baseline, accepted mutants). A maintainer may pass
`--allow-protected`. Otherwise it validates a detached worktree of the branch **with `main`'s copies of every
protected file**, unscoped, mutating types whose source changed. It merges only when all tests pass, the ratchet
holds and no mutant survives. `gate.json` beside the results records the decision.

This enforces attributable, case-limited direct validation. It cannot automatically prove equivalence
for every possible input, or determine whether a case and its observers fully express the contract.
Review test quality and retain the analysis and independent-review requirements of PLAN.

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

### Direct-method gate audit (2026-10-14)

The metadata audit, focused differential and harness projects build successfully. Supplementary
direct invocation passed **18 harness facts** and the new private `Dim.intern(int[])` comparison fact
(**5 recorded cases**, including repeated identities, retained input storage and an exception case). Python gate checks: **13 passed**. The gate tests
cover private/public-caller distinction, missing constructors/accessors/initializers/explicit methods,
stale runs/binaries, wrong overloads, failed tests, changed contracts, stubs, unmatched methods,
empty scopes and the separate bodyless/compiler categories. The original VSTest socket restriction
still applies; no normal test-run or phase closure success is claimed.

A supplementary audit of the isolated baseline inventoried **2,047 method declarations**, including
**199 private declarations** and **276 constructors/type initializers**. With only the new attributable
private comparison, it reported **1 compared / 165 declaration-only / 67 compiler exclusions /
1,814 missing**. Private results were **1 compared / 43 compiler exclusions / 155 missing**. This is
an intentionally failing project-wide coverage report; existing tests must be attributed before their
methods count. These counts describe the isolated baseline, not the concurrently changing implementation.
