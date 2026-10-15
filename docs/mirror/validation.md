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
  `Deviation` status whose accepted ledger row names the member may differ (see Status below). A matching structure
  never substitutes for a direct comparison.
- **Lint** (`integrity.json`): hand-written `src/` code (any `.cs` file, whatever its case) and agent-writable
  differential test files (every `.cs` under `tests/Quire.Differential.Tests` that `protected.txt` leaves writable,
  whatever its case) must not contain
  `goto`, compiler-generated names, IL labels, decompiler comments, decompiler-style locals (`num2`, `type2`,
  `obj1`), `#line` or `#pragma checksum`. Differential test files compile into the harness assembly, so each must
  declare exactly `namespace Quire.Differential.Tests;` and one top-level type, `class <file name>` (checked outside
  braces, comments and strings, so no `Assert` or harness name can be shadowed and every test maps to its file),
  avoid `global using`, `file`-local types and `[assembly:]`/`[module:]` attributes, and not name the environment, files, processes, assembly locations, `QUIRE_*` variables,
  code generation, marshalling, method bodies/IL, assembly or type enumeration, `Type.GetType`, harness types via
  `typeof`, xUnit exceptions, sleeps or delays. `src/Quire.Mpl` code, which runs in the same process, must not name
  module initializers, xUnit, `MethodComparison`, the `Quire.Differential` namespace, method IL, assembly
  enumeration, environment variables or `QUIRE_*` variables. These token rules are defence in depth, not containment
  (string building and reflection can evade them), so test diffs still need review. `file`-local types are matched
  on each raw line and on the line with comments and strings blanked, skipping only `//` comments and simple one-line
  strings; after anything a simple lexer cannot follow (verbatim, raw or nested-quote strings, block comments) nothing
  later in the file is skipped, so such constructs cause findings rather than hide them. Exceptions are
  maintainer-reviewed entries in `docs/mirror/lint-allowed.txt` (`<path> | <rule> | <exact trimmed line>`), keyed on
  the whole line and counted: each entry covers one occurrence, so an edited, new or copied line needs its own entry.
- **Test assembly types** (`integrity.json`): MethodAudit lists every top-level type compiled into
  `Quire.Differential.Tests.dll` with the PDB source files of its methods. Each must come only from maintainer-owned
  files, or be the single class an agent test file is named after; types without source lines or with sources
  outside the repository (other than restored NuGet packages, outside `Quire.*`) are rejected, as are
  compiler-named types other than anonymous types and constant data (so a file-local class cannot hide). This reads
  the binary, so source-level tricks that fool the text checks still cannot add a top-level type.

`python3 tools/validation/integrity.py --inventory <run>/method-inventory.json` repeats the lint and test-type checks.
No original code is committed; the structure check reads the original IL from the artifact itself.
The structure comparison is a fingerprint, not a proof: identical structure with different arithmetic or operand
order passes it, which the direct comparisons must catch.

### Branch coverage, mutation and status

- **Branch coverage:** the differential tests run a second time, unrecorded, under the Microsoft
  code-coverage collector (cobertura). Instrumentation rewrites the module and corrupts the method tokens that
  comparison records carry, so it never runs in the recorded pass. A compared method whose PDB statement lines (in
  every source document) are not all executed, or whose branch conditions are partly covered, becomes `uncovered`,
  with the lines named. Coverage counts any execution during the differential run, including test setup; mutation
  is the attributable check.
- **Unreachable lines:** some lines can never execute, in Quire or in Mathcad's original, such as `return null;` and
  the closing brace after a helper that always throws (`OnError(...)`, `AppException.Raise(...)`); rewriting them
  would break IL-structure matching, and joining them onto one line to game the line metric is forbidden. A maintainer
  lists them in `docs/mirror/unreachable-lines.txt` (protected), one per line:
  `<Quire type>::<signature> <path>:<line> sha256=<hash> # reason`. The method key is the one `accepted-mutants.txt`
  and the ratchet use; the path is repository-relative, under `src/`, and must exist; the reason explains why the
  line is unreachable in the original too. The hash pins the reviewed source and the line's place in it: SHA-256 of
  the listed line's offset from the method's first statement line in that file, a newline, then the method's
  statement lines there (all its PDB lines, each stripped of trailing whitespace, joined by `\n`), recomputed from
  the file as read before the build. MethodAudit exports, for each PDB document, the checksum the compiler recorded
  (`source[].checksum`: algorithm and hash); it must be SHA-256 and equal the hash of those bytes, so the file
  compiled is the file hashed. Two things this relies on: the source lint's `line directive` rule, which forbids
  `#line` and `#pragma checksum` (either would let a file's sequence points or recorded checksum name another file);
  and the validator and `dotnet` sharing one path namespace, so that PDB paths are paths the validator can relate to
  the repository (never a Windows `dotnet.exe` driven from WSL). `python3 tools/validation/method_coverage.py unreachable-entry
  <method-coverage.json> <key> <path>:<line> "<reason>"` prints a correctly hashed entry from a run's report.
  Malformed or duplicate entries fail the run. A PDB document matches
  the entry only when its path relative to the repository (the clone, in the gate) equals the entry's path,
  case-insensitively for Windows-style paths. A listed line that the run leaves unexecuted no longer makes its
  method `uncovered` (`method-coverage.json` names it under the method's `exempt_lines`). Only lines are exempt: a
  partly covered branch, even one whose missing arm is a listed line, still leaves the method `uncovered`.
  MethodAudit attributes to a method only its own sequence points, so a lambda's body is not part of its owner's
  lines, but a lambda written on the owner's line puts statements of both methods on it; a line any other inventory
  method also has statements on is never exempt. `stale_unreachable` lists entries that exempt nothing, by
  `problem`: `no such method`, `ambiguous method key` (the key names methods in several assemblies, as the phase
  profile's mirrored `Quire.ProductInfo` constants do), `document path mismatch` (a document of the method only ends in the entry's path, such
  as a nested look-alike `src/X/Sub/src/X/T.cs`), `not a source line of the method`, `executed`, `line shared with
  <methods>`, `inventory lacks checksums; rerun` (an inventory from an older MethodAudit), `document checksum
  mismatch` and `method source changed` (the method stays `uncovered` until the entry
  is re-reviewed and rehashed; moving the line within the method changes the hash too). Any stale accepted entry,
  including one whose line has become shared, makes the run incomplete (the summary counts them under
  `stale_unreachable`); an executed line is reachable,
  so its entry is wrong and the gate fails too (`executed_unreachable` in the summary). A method compared through an
  exemption is mutated like any other, but mutants on an exempt line can never be killed: each needs a matching
  `accepted-mutants.txt` entry with the same reason. Removing an exemption that moves a method back to `uncovered` is
  a ratchet regression. An agent proposes entries in a scratch file, same format, passes `--proposed-unreachable PATH`
  to local runs, and pastes the same lines into its lane file for review. Proposed lines are exempted the same way
  but listed under `proposed_exempt_lines`. The user approves a proposal; a maintainer moves it into
  `unreachable-lines.txt`. Gate runs (`--mutate-since`, `--write-baseline`) refuse the flag, and the merge gate
  blocks any summary that used it.
- **Mutation** (`--mutate` with `--method-type`, or `--mutate-since REV`): MethodAudit lists same-size, verifiable
  byte changes in each compared method: negated branches; swapped `add`/`sub`, `mul`/`div`, `and`/`or`, `shl`/`shr`,
  `cgt`/`clt`; `ceq` → `cgt.un`; `0`/`1` swapped; other integer constants plus one; floating constants plus one
  (doubled where rounding absorbs it; NaN and infinities skipped). An integer constant load in `[0, 1<<20]` immediately followed
  by `newobj` of a framework capacity constructor taking a single `int32` is not mutated: the capacity is not
  observable unless the method reads it, so it is exempt by policy. The types are `System.Text.StringBuilder`,
  `System.Collections` `ArrayList`/`Stack`/`Queue`, and `System.Collections.Generic`
  ``List`1``/``Stack`1``/``Queue`1``/``HashSet`1``/``Dictionary`2``, referenced from `System.Runtime`,
  `System.Collections`, `System.Collections.NonGeneric`, `System.Private.CoreLib`, `mscorlib` or `netstandard`
  (not `Hashtable`, whose bucket count changes enumeration order). A method that calls `get_Capacity`,
  `set_Capacity`, `EnsureCapacity`, `GetChunks` or `TrimExcess` keeps all such mutants. No other
  mutant is presumed equivalent. Unmutated control runs, of all selected tests and of each distinct per-method
  test subset, must pass and record no mismatch (a recorded mismatch in a passing run means a test swallowed it; a
  subset that fails alone would kill every mutant). Each mutant is applied to a copy of
  the test output and only the tests whose direct comparisons covered that method are rerun; the copy is
  fingerprinted and checked after every run.
  It counts as killed only when `MethodComparison` records an outcome or state mismatch on a method of the mutated
  assembly **in a test that the TRX report shows failed**; the record and report live in a scratch directory outside
  the test output. The mismatch is usually the mutated method's own, but the first mismatch ends a test, so it may
  belong to another method compared earlier (`killed (via another method compared first)`). A mutant that crashes
  the test host (a fatal CLR error such as a stack overflow or access violation, which VSTest reports in the TRX
  `ResultSummary` as "The active test run was aborted. Reason: Test host process crashed") under tests whose control
  run passed is `killed (crash)`, counted in `crashes`, so a crashing mutant never belongs in
  `accepted-mutants.txt`. A run that leaves no TRX report proves nothing. Other failures that record no mismatch,
  such as exceptions in a type initializer or setup errors, do not count; a maintainer may accept such mutants. A run
  that exceeds four times its test subset's control duration (at least 30 s and at most `--timeout`; the control runs
  in a worker under the same parallel load) is killed with its process tree and counts as killed (`timeouts`), so a test
  that stalls deliberately would also kill mutants; the lint forbids sleeps and delays. Crash and timeout kills need
  no comparison against Mathcad, so a test that recurses or loops on a hard-coded expectation could kill mutants
  without one; reviewers must look for such tests.
  `mutation.json` lists survivors: a missing case or a lossy renderer. Equivalent mutants are accepted only by a
  maintainer in `docs/mirror/accepted-mutants.txt` (`<key> # reason`). An agent proposes one in a scratch file or
  its lane file, same format, and passes `--proposed-mutants PATH` to local runs: those survivors are listed apart
  under `proposed_survivors` (with reasons), and `stale_proposed` lists proposals that
  matched no surviving mutant (killed, already accepted or not run). The user approves a proposal; a maintainer
  moves it into `accepted-mutants.txt`. Gate runs (`--mutate-since`, `--write-baseline`) refuse the flag, and the
  merge gate blocks any summary that used it. `--jobs` sets parallel copies.
- **Exit status:** `validate.py` exits 0 when the run is complete, 3 when the merge gate's requirements hold
  (`gate_passed`) but gaps remain, and 1 when blocked. A run given `--proposed-mutants` or `--proposed-unreachable`
  is never complete nor `gate_passed`, so it exits 1; its summary records `passes_with_proposals`, whether the gate's
  requirements would hold with the proposals accepted.
  Local runs cache outcomes outside the repository (`~/.cache/quire-mutants/`, or `QUIRE_MUTANT_CACHE`, which no
  test run sees) by SHA-256 of the whole test output directory, `mutate.py` and `validate.py`, SDK choice and
  installed runtimes, the Mathcad install (`MATHCAD_DIR` and its DLLs' names, sizes and modification times), the
  mutant's key, offset and bytes, and its test set; not its timeout, since only outcomes reached within one are
  cached. Only the validator writes entries, after a run; timeouts and crashes (load- or memory-dependent) are never
  cached, every control still runs, and `cached` in `mutation.json` counts reused outcomes. The cache is local-only:
  code under test runs as the same user and could forge an entry, so gate runs (`--mutate-since`,
  `--write-baseline`, or `--no-mutant-cache`, which the merge gate passes) neither read nor write it. Builds pass
  `EnableSourceControlManagerQueries=false` so the commit is not stamped into assemblies; a docs-only commit leaves
  the output in the same directory byte-identical (built assemblies embed their absolute path).
  `--mutate-since` selects types whose `src/Quire.Mpl` source changed (files without method bodies select
  nothing) or whose comparisons live, or lived according to the baseline's method → test class map, in an added,
  changed or deleted differential test file (by its single top-level class, nested classes included), and fails when PDB paths lie
  outside the repository.
- **Status:** a `Verified` method that this run does not show as compared (or declaration-only) counts as
  `verified_unsupported`. A `Deviation` status skips the structure check only when the `deviations.md` index lists
  its id with `accepted-temporary` or `accepted-permanent` and names the method as `Quire.Type::Member` in the scope
  column (every overload of that name).
- **Compiler-generated methods:** lambdas, local functions, iterator and async bodies compile into methods with
  names only a compiler can produce (containing `<`). They are `excluded` from structure and coverage, but MethodAudit
  records the hand-written method that owns each one, and mutation mutates it against the owner's tests whenever the
  owner is compared and selected, so idiomatic code keeps its logic under test. They may appear and disappear
  freely in the ratchet. An iterator or async kickoff method has no statement lines and stays `uncovered`; shared
  closure plumbing (constructors of `<>c`) has no owner and is not mutated.

### Ratchet

`ratchet.json` compares a run with `docs/mirror/validation-baseline.json` (schema 3), which records its profile,
per-type counts, each method's stage (emitted by the audit) and test classes, and each lint finding by identity
(counted). The stages, in order: `no_counterpart`, `missing`, `structure_mismatch`, `untested` (structure matches but no
passing direct comparison), `unchecked_state` (an instance method compared without checking its state), `uncovered`,
`compared`; `untested` and `unchecked_state` are counted as `missing`. Fixing one gap exposes the next, so a method
may move forward but never back (deleting tests never counts as progress), and any other change of state must reach
`compared`. `compared` and each type's hand-written method total may not fall; a type's unfinished methods
(`missing`, `structure_mismatch` and `uncovered` together) and its `verified_unsupported` count may not rise. A new
method without a Mathcad counterpart is rejected, and a new method in an existing type may not arrive as
`structure_mismatch`, `unchecked_state` or `uncovered`, so progress elsewhere cannot pay for it. Unscoped runs also forbid any new lint finding (fixing one does not pay for another) and
disappearing types or methods. New types with counterparts start their own baseline. A baseline from another profile
or schema is rejected; schemas 3 and 4 are both read (4 adds data that selects mutants, not ratchet data). `--write-baseline PATH` writes a run's snapshot (unscoped runs only) once every test and mutation
run has finished and the final seal check has passed. A maintainer creates the first baseline from an
unscoped focused run on `main`; afterwards only the merge gate updates it.

### Merge gate

Work happens on one branch per work unit, based on current `main`. Agents never commit to `main`. From the checkout
that has `main` checked out:

```sh
python3 tools/validation/merge_gate.py <branch>            # check only
python3 tools/validation/merge_gate.py <branch> --apply    # check, fast-forward main, commit the new baseline
```

The gate blocks a branch that is not based on current `main`, has no changes, or contains paths that differ only in
case. It also blocks any added, changed, deleted or renamed path matching `tools/validation/protected.txt` (all of
`tools/`, the harness and comparison helper files, test subdirectories, generated sources, every project, props,
targets, SDK/NuGet/runner configuration at any depth, and the maintainer-owned records), unless a maintainer passes
`--allow-protected`. One exception lets a branch implement members: it may delete whole stub members from an
existing Quire.Mpl generated mirror file (`src/Quire.Mpl/Generated/Mirror/*.g.cs`), each a `[MirrorStub]` line and
the one-line member under it, compared byte for byte (apart from a trailing CR; only `\n` ends a line), and nothing
else. A deleted method, property or constructor that is not re-implemented disappears from the method inventory,
which the ratchet rejects. The inventory keys neither fields nor modifiers, so before any test runs the focused
profile also checks every line of the maintainer-owned `docs/mirror/manifest/Quire.Mpl.txt` against the build
(`MirrorGen shape`): a deleted stub must come back with the same kind, access, modifiers and signature. Constant stubs
stay, since a constant has nothing to implement and the shape omits its value. The clone keeps those files as the
branch has them, and `gate.json` lists the deleted members under `stub_deletions`. Headers, `MirrorOf`, type declarations, stubs under another
attribute, edits, additions, regeneration, deleting the file, or a change that deletes nothing stay maintainer-only. Because the gate runs the focused profile, which builds and audits only `Quire.Mpl` and runs
only its test projects, it blocks `src/` and `tests/` changes outside them unless a maintainer passes
`--allow-unvalidated` after a Windows phase run. Otherwise it validates a separate clone of the branch tip **with `main`'s copies of every
protected file** (with `--allow-protected`, the branch's, except `docs/mirror/validation-baseline.json`, which always
comes from `main`): an unscoped focused run, mutating the changed types. It merges only when all tests pass, the
ratchet holds, no mutant survives, and the new baseline parses and holds the ratchet against `main`'s committed one.
`gate.json` beside the results records the decision, including a failure to prepare the clone. A branch whose tip is
not on a local branch is blocked, since the clone copies only local branches.

Code under test runs inside every `dotnet test` with the validator's own user rights, so the run is arranged to
limit what it can reach:

- `validate.py` reruns itself from a private temporary copy with `python -I -B` and imports every module up front;
  it builds MethodAudit into a temporary directory and all test projects, then reads every verdict input (baseline,
  accepted mutants, unreachable lines, lint allowlist, deviations, protected rules, the `--mutate-since` diff) and runs the source lint
  before any test starts.
- Tests run from their built assemblies, not their projects, so no MSBuild evaluation follows code under test.
  Coverage results go to a fresh temporary directory.
- Every `dotnet` command runs from its own new temporary directory, removed afterwards, holding the repository's
  `global.json` as read before any test runs (or an empty `{}` one, the CLI's defaults, when the repository has
  none), with absolute paths to projects and assemblies. The nearest `global.json` wins, so one planted in the
  checkout or above cannot change the SDK or test runner a later command picks. A process left running would have to
  find and rewrite a brand-new directory between its creation and the `dotnet` muxer reading it; that race remains.
  `--dotnet` is resolved once on `PATH` before any test runs (on Windows a bare name would otherwise be looked up in
  the repository first) and refused if it lies inside the repository.
- The few files read afterwards (type map, every file at any depth under `docs/mirror/evidence` and
  `docs/mirror/manifest`, built test outputs, the MethodAudit build) are fingerprinted and checked after every test,
  coverage and mutation run and at the end, so a record added, nested, changed or deleted mid-run is caught.
- The merge gate validates in a separate clone (`git clone --no-local`, with the `origin` remote and the reflogs
  removed so it does not record this repository's path), never sharing this repository's `.git`, hooks or config; blocks unless the
  validator exits normally (0, or 3 when only the completeness goal is unmet); reads the new baseline once, which
  the validator writes only after its last test run, and requires those bytes to parse and hold the ratchet against
  the base's committed baseline; and builds the merge commit from the tip's committed tree plus exactly those bytes
  with hooks disabled, so nothing done to the checkout or the baseline file during validation can reach `main`.

Not covered: test processes are not sandboxed. Code under test can forge records it writes during its own run
(case, mismatch and coverage records), and a process it leaves running could interfere with a later stage of the
same run. `src/Quire.Mpl` and `tests/Quire.Mpl.Tests` are not held to the differential tests' API rules. Deliberate
tampering of this kind is out of scope for the gate; review agent diffs for it.

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
