# Validating a mirrored batch

Read the selected members' evidence records first. Test success supports only the recorded
cases; it does not establish method-wide equivalence or authorize status changes by itself.

## Runtime batch: one repeatable command

During iteration and before handoff, use the affected behavior and direct differential tests. For example in PowerShell:

```powershell
$env:QUIRE_REQUIRE_MATHCAD = '1'
dotnet test tests\Quire.Focused.Differential.Tests\Quire.Focused.Differential.Tests.csproj `
  --filter 'FullyQualifiedName~FunctionMatrixDifferentialTests'
```

This executes the selected comparisons without
the full shape, coverage, audit and ratchet pass. Those checks run at the final runtime merge gate, so coverage
or audit gaps can surface there. Local test success alone does not establish audit or mutation readiness.

For a local diagnostic run of those checks, with .NET SDK 10 and Python 3 installed:

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

Scoped validation is optional for ordinary handoffs; the runtime merge gate performs the required full validation
pass. Run scoped local mutation when investigating weak comparisons
or preparing equivalent-mutant/unreachable-line proposals. Omitting local mutation delays that feedback until
the gate; a passing local report without mutation does not establish that the gate's mutation checks will pass.
The focused report records `mutation_requested` and appends `[mutation deferred to gate]` when local mutation
was omitted.

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

**Reviewed bindings.** A type-map row is a reviewed binding when its status is `missing`, `exists`,
`exists-elsewhere`, `shared source (linked into each assembly)` or `duplicate (shared native header)`. One Quire
name can therefore have one binding per Mathcad assembly. For example, `Quire.ProductInfo.Constants` is in 19
assemblies; `Quire.Properties.Settings` is `Spirit.Properties.Settings` in Quire and `Ptc.Properties.Settings` in
Quire.Core.
- A type's `MirrorOf` must equal one of its bindings exactly in Mathcad type and assembly. The assembly comparison
  ignores case, because the type map records install file names (`efisymbolicslexer.dll`), `MirrorOf` names the
  assembly (`EfiSymbolicsLexer`), and Windows treats both case-insensitively. Originals are found in the install
  directory case-insensitively too.
- Two audited projects may not claim the same binding of a name with several: both copies would pair with one
  original and leave another unchecked, so that is an inspection error. A binding no project claims is not:
  Quire keeps one copy of a type that a native header duplicates into several Mathcad assemblies, so the other
  assemblies' bindings stay unclaimed (149 of them in a full phase audit).
- A signature that names a Quire type maps through the binding of the project that declares it. For a project not
  audited in the run, the type map's binding is used when it is the only one. Otherwise that project's own
  `MirrorOf` is used, and only if it is a reviewed binding.
- Keys leave out custom modifiers, because C# cannot write them. When one key matches several originals that
  differ only in modifiers (C++/CLI's `long` is `int modopt(IsLong)`), the original whose modifiers equal the Quire
  method's is chosen. If the modifiers do not single one out, the member is an inspection error.
- msgen's `gen.IlAsm::Push(uint32 modopt(IsLong))` and `Push(int32 modopt(IsLong))` (Tier D, D10/D11) are
  therefore known originals without a counterpart. Quire's `Push(uint)` and `Push(int)` pair with the unmodified
  overloads, and MirrorGen's notes list the two as skipped overloads.

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
  Mathcad binding; locals, nops, stack shuffling, branch encoding and compiler-generated names are ignored, and the
  structure is read from an optimized (Release) build, like the original; the gate also runs the differential test
  suite against Mathcad with the Release Quire build. The recorded comparison, coverage, mutation and body hashes use
  the Debug build so test records, method tokens and PDB lines refer to the same assembly. A difference makes the
  method `structure_mismatch` with the first differing item. Only a
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
  code generation, marshalling, method bodies/IL, assembly or type enumeration, `Type.GetType` or a type looked up by a string literal (`.GetType("`), harness types via
  `typeof`, xUnit exceptions, sleeps or delays. `src/Quire.Mpl` code, which runs in the same process, must not name
  module initializers, xUnit, `MethodComparison`, the `Quire.Differential` namespace, method IL, assembly
  enumeration, environment variables or `QUIRE_*` variables. No `src/` code may depend on the build configuration
  (`#if`/`#elif` naming `DEBUG` or `RELEASE`), and MethodAudit refuses `[Conditional]` code and calls to `Debug` or
  `Contract`, which only Debug builds keep. These token rules are defence in depth, not containment
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
  the repository. `python tools/validation/method_coverage.py unreachable-entry
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
  to local runs, and pastes the same lines into its lane file. Proposed lines are exempted the same way
  but listed under `proposed_exempt_lines`. They follow the same path as proposed mutants: the lane's proof of the
  reason, then the maintainer adds them to `unreachable-lines.txt` and merges with
  `--allow-protected`. Gate runs (`--mutate-since`, `--write-baseline`) refuse the flag, and the merge gate
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
  maintainer in `docs/mirror/accepted-mutants.txt` (`<key> body=<body_sha256> # reason`). The hash is the mutated
  method's `body_sha256` (see What the gate mutates) when the proof was reviewed, so an entry is tied to that body, as
  an unreachable-line entry is tied to its method's source: once the method's IL changes, the entry excuses nothing
  (its survivor fails the run) and `summary.json` `stale_accepted` lists it with its `problem` (`no body hash`,
  `malformed body hash`, `duplicate entry` (a key listed twice), `no such method`, `ambiguous method
  key`, `method body changed`, `no such mutant`), checked against the whole method inventory even in a
  `--method-type` run. A stale entry makes the run
  incomplete; re-review the proof and rehash it, or delete it. `python3 tools/validation/mutate.py annotate-accepted
  <method-inventory.json or method-coverage.json> [--file PATH]` rewrites a file's entries with the body hashes of a
  run's build, prints `rehashed (body changed): <key>` for each entry whose hash it replaced (re-review those proofs)
  and lists entries whose method or mutant it cannot find. An agent proposes one, with its proof, in a
  scratch file, either form (with or without `body=`), passes `--proposed-mutants PATH` to local runs, and pastes the
  same lines into its lane file: those survivors are listed apart under `proposed_survivors` (with reasons), and
  `stale_proposed` lists proposals that matched no surviving mutant (killed, already accepted, not run, or hashed for
  another body). The maintainer adds accepted entries with `body=`. With no reviewer and no user
  approval ([D16](decisions.md)), the maintainer adds the entries on the branch and merges it with
  `--allow-protected`, checking that `gate.json` `protected_changed` lists only `accepted-mutants.txt` and
  `unreachable-lines.txt` ([PLAN](PLAN.md) step 4). Gate runs (`--mutate-since`, `--write-baseline`) refuse the flag, and the
  merge gate blocks any summary that used it. Both `validate.py` and `merge_gate.py` accept `--jobs` to set parallel
  copies. The default is one worker per logical CPU, capped at 12; explicit positive counts override the cap.
  Lower it on hosts with limited memory or CPU contention. The worker count is recorded in the mutation summary
  and in runtime `gate.json` as `mutation_jobs`. It changes scheduling, not the selected mutants or acceptance rules.
- **Exit status and verdict:** `validate.py` exits 0 when the run is complete, 3 when the merge gate's requirements
  hold (`gate_passed`) but gaps remain, and 1 when blocked (2 for an incomplete run with `--allow-differential-skips`).
  A run given `--proposed-mutants` or `--proposed-unreachable` is never complete nor `gate_passed`, so it exits 1; its
  summary records `passes_with_proposals`, whether the gate's requirements would hold with the proposals accepted.
  Its last line, also `summary.json`'s `verdict` (`local_checks`, `gate`, `reasons`, `pending_approval`, `line`),
  separates the run's own checks from what the gate would decide on the same results:

  | Last line | Exit | Meaning | Next |
  |---|---|---|---|
  | `LOCAL CHECKS: pass · GATE: would pass` | 0 | complete | queue the gate |
  | `LOCAL CHECKS: pass · GATE: would pass with gaps (<gaps>)` | 3 | gate requirements hold; methods remain `missing`/`uncovered`/… | queue, or close the gaps first |
  | `LOCAL CHECKS: pass · GATE: would block (N proposed mutants pending approval; …)` | 1 | only proposals block (`pending_approval`: pending the maintainer's addition) | hand the proposals to the maintainer ([PLAN](PLAN.md) step 4) |
  | `LOCAL CHECKS: fail · GATE: would block (<failures>)` | 1/2 | failing or skipped tests, error, ratchet regression, surviving mutant, executed "unreachable" line | fix them |

  `LOCAL CHECKS: fail` names only failures of this run; pending proposals are not failures. A `--method-type` run
  ends with `[scoped to N types; the gate validates all]`: the gate runs unscoped and mutates by its own selection
  (below), so a scoped `would pass` is not a promise. The gate also blocks for reasons outside the validator
  (protected paths, not rebased); see Merge gate.
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
- **Timed-out mutants are retried in parallel:** each worker limit is calibrated from its unmutated test control
  (`max(MUTANT_FLOOR, MUTANT_MARGIN × measured seconds)`, capped by the run timeout). Worker count is capped to the
  CPUs available to the process. After the first pass finishes, fresh controls for
  timed-out test sets run in parallel at that worker count. When a test set has several timed-out mutants, controls
  are sampled up to the retry worker count and the slowest measurement sets its budget. The timed-out mutants then
  retry in parallel on separate copies with those calibrated budgets. A second timeout counts as `killed (timeout)`;
  otherwise the retry outcome stands and `parallel_outcome` records the first. `mutation.json` records retry workers,
  control count and retry wall time; `summary.json` and runtime `gate.json` retain the timeout and retry counts.
- **Mutant scratch space:** the copies of the test output and each mutant run's directory live in a new
  `quire-mutant-scratch-*` directory of the system temporary directory, outside the results (code under test runs
  from a copy, so the results must not be a fixed path away from it). The validator removes it when the run ends and
  names it in the results' `mutation-scratch.txt`; if the validator was killed, the merge gate removes it, but only a
  `quire-mutant-scratch-*` directory directly in the temporary directory that this user owns. Each mutation run first
  removes `quire-mutant-*` directories this user owns in the system temporary directory in which nothing (at any
  depth, links not followed) changed for a day, left by older or killed runs; one too large to examine is kept.
- **What the gate mutates** (`--mutate-since REV`, which the merge gate passes): not every compared method of a
  changed type, but the compared methods whose mutants could now have a different outcome. A mutant's outcome is
  decided by the mutated method's IL, the tests that directly compare that method (only those run it), and whatever
  runs under those tests. MethodAudit records for each method with a body:
  - `body_sha256`: a hash of its IL with every metadata token resolved to the member, type, field (with its type)
    or string it names (with the checkout's absolute path replaced by `<repo>`: `[CallerFilePath]`, used by every
    `MirrorStub.NotImplemented()`, compiles the source path into the IL, and the gate builds in a new clone each time),
    the initial data of fields that have some (`<PrivateImplementationDetails>` arrays), its
    locals' types, exception regions and flags; token renumbering from unrelated edits does not change it. When a
    data field's size cannot be read from its type, only its name is hashed (` data ?`): that relies on the compiler
    naming such fields after a hash of their contents, as Roslyn does;
  - `calls`: the `Quire.Mpl` methods it references (calls, delegate creation, `ldtoken`; through a generic
    instantiation, every same-named method of that type);
  - `virtual_calls`: the dispatch slots of its `callvirt`/`ldvirtftn` targets, `name`arity/parameter count` with the
    name unqualified; each virtual method's own `slot` (parameter types are left out, so `Base<int>.M(!0)` reaches
    overrides `M(int)` and `D<T>.M(T)` alike: this over-selects rather than misses);
  - `field_reads` / `field_writes`: fields it loads / stores, as declaring type (generic definition) and name; taking
    a field's address counts as both; `reference_reads`: the fields it reads whose type can hold a mutable object
    (any reference type but `string`, or a type parameter). Filling an array or collection a field holds
    (`ldsfld Table; stelem`, `Table[k] = v`, `.Add`) stores to no field, so reading such a field counts as writing it.

  For the differential test assembly it records each top-level type's source files and `references`: the other
  top-level types its IL (or its nested types', lambdas included) derives from, implements or names in a call, field,
  type or generic argument, so an extension method `value.Render()` declared in another class counts, and those that
  custom attributes on it, its methods, fields, properties or parameters construct or name in a `System.Type`
  argument (`[MemberData(nameof(A.Cases), MemberType = typeof(A))]`, `[ClassData(typeof(A.Data))]`, a data
  attribute `A` declares). An attribute with an argument of an enum declared in another assembly cannot be decoded;
  every test type whose name its blob spells counts instead. The baseline (schema
  4) stores each method's `body_sha256` (`bodies`) and its `field_writes` and `reference_reads` (`writes`) from the
  last gate run, which tested exactly the tree it merged. A changed method is one whose body hash differs from the
  baseline's or that the baseline lacks, or the hand-written owner of a changed lambda, iterator or async state
  machine. Selected (`mutation.json` `methods`, with reasons; `mutated_methods` lists those actually mutated):
  1. **IL changed** / **new body** / **compiler-generated part changed:** the changed methods;
  2. **comparing tests changed:** methods compared, now or according to the baseline's method → test class map, by a
     changed test class: a top-level type whose source files include an added, changed or deleted file under
     `tests/Quire.Differential.Tests` (or named after it), and, transitively, every type whose IL `references` one.
     Shared helpers (`MatValDifferentialTests.Storage`, `TypeDifferentialTests.Signatures`) are used across classes,
     so a changed helper selects the methods its users compare; a changed harness file (`Values.cs`,
     `MethodComparison.cs`) selects most of them. A textual scan of the test sources read before any test runs is
     united with the IL record, to a fixed point, since each covers what the other misses (reflection by name and the
     like): a class counts when its file names a changed class; the scan alone misses extension-method calls and type
     declarations its pattern (a line starting with modifiers and `class`/`struct`/`interface`/`enum`/`record`) does
     not recognize. Without the IL record (missing or empty `test_types`, or entries without `references`), every
     test class counts as changed. `mutation_notes` says which applied (`IL and text`, or `all`);
  3. **newly compared:** compared now but not in the baseline (its mutants have never run);
  4. **calls a changed method:** direct callers of a changed method by token, or by a virtual call to the slot of a
     changed virtual method or of an instance method the baseline has and the build lacks (a removed override);
  5. **reads a field a changed method writes:** readers of a field that a changed or removed method writes (or reads
     while it holds a mutable reference), now or according to the baseline (so a constructor or `.cctor` that
     stopped initializing a field still counts); when such a reader is a property, indexer or event accessor (or a
     lambda in one), its callers too, and theirs while they are accessors, since a getter passes the value on;
  6. compiler-generated methods selected themselves, or owned by a selected method, since they run under the
     owner's tests.

  A slot with more than 50 callers (`ToString`, `Equals`, enumerator members) keeps them all and adds a line to
  `summary.json` `mutation_notes`: dropping any would be unsound, so the note only explains a large selection. A run
  whose baseline has body hashes fails if the inventory lacks them, and if a method's `slot` disagrees with the one
  derived from its key.

  Not selected: compared methods whose IL, comparing test classes, direct callees and read fields are unchanged.
  Their mutants were all killed (or accepted) by the gate run that last selected them, against the same IL and tests;
  that holds as long as everything those tests execute behaves as before.
  - A changed callee, overriding method or field writer is caught one step away: a mutant killed before can survive
    now only if that change masks it (the helper now normalizes away the value the mutant corrupts).
  - **Residual risk:** masking two or more steps away (a callee's callee; an object reached through a field and
    changed by a callee of the method that read it; a value passed on by an ordinary method rather than an
    accessor), through a delegate or reflection, or through test setup and rendering that
    reach changed code the mutated method never calls. Each needs the changed code to behave differently on inputs
    those tests reach, which its own direct comparisons against Mathcad (mutated under rule 1) would normally catch,
    but nothing proves it. Removing an `accepted-mutants.txt` entry and changing the Mathcad install select nothing.
  - So a maintainer audits with `--mutate-all` (every compared method; `merge_gate.py --mutate-all` or an unscoped
    `validate.py --mutate-all`) after every 10 logical batch merges or weekly, whichever comes first, and after any change to
    the harness, maintainer-owned test helpers, `accepted-mutants.txt` or the Mathcad install. `--mutate-types-wide` (both
    scripts) restores the previous rule: every compared method of types whose `src/Quire.Mpl` source changed (by
    PDB path; files without method bodies select nothing) or whose comparisons live, or lived, in a changed test
    class. The gate also falls back to that rule when the baseline has no body hashes (schema 3); `summary.json`
    `mutation_scope` says which applied. `--mutate-since` fails when PDB paths lie outside the repository.
  - Local `--mutate --method-type` runs mutate every compared method of the named types: for those types, a superset
    of what the gate mutates; the gate may also mutate methods of other types (callers, field readers, users of a
    changed test helper).
  - Keep helpers that several test classes use in maintainer-owned files; a helper in an agent test file widens every
    change to that file to all its users, and maintainer-owned files must not name agent test classes.
  - **Why unchanged methods keep their old kills.** The gate's baseline comes only from gate runs that tested exactly
    the tree they merged, and every one of them killed (or accepted) every mutant it selected. A later merge leaves
    a method unselected only when its IL, the tests that compare it, its direct callees and the fields it reads are
    unchanged. Its mutants then run the same mutated IL under the same tests, against the same Mathcad
    originals, so each kill repeats unless changed code those tests execute now masks it. Masking one step away is
    selected (rules 4 and 5); masking further away is the residual risk above. Test edits are rule 2: a changed test
    class, or a helper it uses, reselects every method it compares, now or in the baseline's map, so a weakened
    assertion cannot keep an old kill it no longer earns. The nightly run below bounds the residual risk to a day.
  - **Nightly audit:** `python3 tools/validation/merge_gate.py --nightly --results <new dir> [--jobs N]` clones the
    tip of `main` (or `--base`) and runs `validate.py --mutate-all --no-mutant-cache` there. It writes no baseline
    and no record, takes no lock and merges nothing. `nightly.json` in the results lists the tip, the mutant count,
    `survivors`, `stale_accepted` and every blocking reason (failed tests, ratchet, executed unreachable lines);
    the command exits 1 when any is present. Schedule it on a host where it does not compete with gates for CPU;
    a survivor is fixed or proposed for acceptance like any other.
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

`ratchet.json` compares a run with `docs/mirror/validation-baseline.json` (schema 4), which records its profile,
per-type counts, each method's stage (emitted by the audit), test classes and IL body hash (`bodies`, used only to
select mutants), the methods whose IL matches Mathcad's exactly (`exact`, optional), and each lint finding by identity (counted). A schema 3 baseline (no `bodies`) is still read. The stages, in order: `no_counterpart`, `missing`, `structure_mismatch`, `untested` (structure matches but no
passing direct comparison), `unchecked_state` (an instance method compared without checking its state), `uncovered`,
`compared`; `untested` and `unchecked_state` are counted as `missing`. Fixing one gap exposes the next, so a method
may move forward but never back (deleting tests never counts as progress), and any other change of state must reach
`compared`. `compared` and each type's hand-written method total may not fall; a type's unfinished methods
(`missing`, `structure_mismatch` and `uncovered` together) and its `verified_unsupported` count may not rise. A new
method without a Mathcad counterpart is rejected, and a new method in an existing type may not arrive as
`structure_mismatch`, `unchecked_state` or `uncovered`, so progress elsewhere cannot pay for it. Unscoped runs also forbid any new lint finding (fixing one does not pay for another) and
disappearing types or methods. New types with counterparts start their own baseline. A baseline from another profile
or schema is rejected; schemas 3 and 4 are both read (4 adds data that selects mutants, not ratchet data; either may carry `exact`). `--write-baseline PATH` writes a run's snapshot (unscoped runs only) once every test and mutation
run has finished and the final seal check has passed. A maintainer creates the first baseline from an
unscoped focused run on `main`; afterwards only the merge gate updates it.

Beside `structure_matches`, MethodAudit reports `exact_match` for each method whose structure it compares (null
otherwise): true when the optimized Quire body and Mathcad's are the same instruction sequence, with short and long
forms folded (`br.s` is `br`, `ldc.i4.2` and `ldc.i4.s 2` are `ldc.i4 2`, `ldarg.1` is `ldarg 1`), local slot
numbers and compiler-generated names (which the structure check spells `<generated>`) renumbered by first use within
the method (so which local or closure an instruction uses still counts), operands otherwise named as the structure
check names them, `calli` with its signature, and branch targets and exception regions as instruction indices.
Unlike the structure check, branch direction and block order count: `if (c) return a; return b;` rewritten as
`c ? a : b` keeps the structure but loses the exact match. The baseline lists the exactly matching methods (`exact`),
and a method listed there that is still in the inventory may not lose its exact match; one that disappeared or was
renamed is reported as such, and one no longer compared (now `excluded`, `declaration_only` or without a counterpart)
says so. A baseline without `exact` holds no exact matches, so the first gate merge that writes it starts the list.
After an SDK update or a change to this normalisation, a maintainer regenerates the baseline from an unscoped run on
`main`. `summary.json` (`il_match`) and the console report how many methods match exactly and how many match the
structure only.

### Merge gate

Work happens on one branch per work unit, based on current `main`. Agents never commit to `main`. From the checkout
that has `main` checked out:

```sh
python3 tools/validation/merge_gate.py <branch>            # check only
python3 tools/validation/merge_gate.py <branch> --apply    # check and fast-forward main
python3 tools/validation/merge_gate.py <branch> --jobs 8   # choose the mutant worker count
python3 tools/validation/merge_gate.py <branch> --full-validation # validate even allowlisted prose edits
```

To share one final validation pass, the maintainer may combine **at most two** queued batches on a
`mirror/integration-<unit>` branch from current `main`. Preserve each batch's squashed commit and its own coherent
scope/checkpoint. Each author reads their complete diff; the integrator reads the combined diff, including conflict
resolutions and accepted-mutant proofs/keys. Record original and integrated commit ids, owners, final base and final
tip. Hold the merge lock through final base checks, gate application and push. Keep the candidate fixed during
validation. Run the same gate on the final candidate against current
main's baseline: its changed-method scope covers both batches. If either batch or the audit schedule needs
`--mutate-all`, the combined gate needs it too. Two combined batches count as two logical merges toward the
10-batch audit interval; weekly audits still apply. Reject the whole candidate on any gate failure. A failed or
interrupted earlier audit is not a passing receipt and must not be hidden by a narrower rerun.

Before every gate merge, fast-forward `main` to `origin/main` first ([PLAN.md](PLAN.md), "Pull first").

Pending maintainer-owned acceptance changes and scopes without required separate runtime evidence cannot be bundled. Batching never
grants permission for additional protected paths or `--allow-unvalidated`; inspect the complete protected-path
list against existing exact authorization. Only the final combined tip is validated: intermediate preserved
commits have not passed independently and are not validated rollback targets. Keep the two constituent
checkpoints separate and link them to the same final receipt. Failure attribution can take more work, so keep
the limit of two and use a single batch when the changes are difficult to understand together.

A lane may continue on the next independent batch while another batch is in the gate. Start from a fresh worktree
and branch, avoid files and shared helpers owned by the active batch, and queue against current `main` after it
lands. Never edit a candidate whose validation is in progress.

An automatic prose route skips cloning, builds, tests, coverage, auditing and mutation only when every changed
path is one of these exact names:

- `README.md`
- `docs/mirror/WORK.md`
- `docs/mirror/STATE.md`
- `docs/mirror/history.md`
- `docs/mirror/work/lane-a.md`
- `docs/mirror/work/lane-b.md`
- `docs/mirror/work/validation-flow.md`

Every changed file must exist as a regular, non-executable UTF-8 file without NUL bytes in both the base and tip.
Additions, deletions, renames between those names, mode changes, other paths and mixed changes use runtime
validation. A protected path never uses the shortcut, even with `--allow-protected`. `--full-validation`,
`--mutate-all` and `--mutate-types-wide` explicitly request the runtime route. The normal ancestry, local-branch,
protected-path and case-collision checks still run. Authors read their complete diffs; the gate is the pre-merge
check ([D14](decisions.md)). `gate.json` records
`validation_mode: prose`, `baseline_reused_from`, and null runtime results/exit status: no tests are claimed.
`--apply` fast-forwards to the authored prose tip while retaining the base's exact baseline, without a baseline
commit. Evidence, manifests, type maps, policy/configuration and tooling are outside this allowlist. Runtime
gates record `validation_mode: runtime` and retain the complete validation process below.

The gate blocks a branch that is not based on current `main`, has no changes, or contains paths that differ only in
case. It also blocks any added, changed, deleted or renamed path matching `tools/validation/protected.txt` (all of
`tools/`, the harness and comparison helper files, test subdirectories, generated sources, every project, props,
targets, SDK/NuGet/runner configuration at any depth, and the maintainer-owned records), unless a maintainer passes
`--allow-protected`. One exception lets any lane implement members: it may delete whole stub members from an
existing generated mirror file matching `src/<Project>/Generated/Mirror/*.g.cs`, each a standalone
`[MirrorStub]` line and the one-line member under it, compared byte for byte (apart from a trailing CR; only `\n`
ends a line), and nothing else. The deletion is automatically recognized only when every changed line in that file
is part of such a deleted stub. A deleted method, property or constructor that is not re-implemented disappears
from the method inventory, which the ratchet rejects. The inventory keys neither fields nor modifiers, so the
gate also checks the matching maintainer-owned `docs/mirror/manifest/<Project>.txt` against the built assembly
(`MirrorGen shape`): a deleted stub must come back with the same kind, access, modifiers and signature. The focused
profile checks `Quire.Mpl`; the phase profile's `MirrorShapeTests` checks every project manifest, including each changed
project. Constant stubs stay,
since a constant has nothing to implement and the shape omits its value. The clone keeps qualifying files as the
branch has them, and `gate.json` lists the deleted members under `stub_deletions`. Headers, `MirrorOf`, type
declarations, stubs with additional attributes, edits, additions, regeneration, deleting the file, or a change that
deletes nothing remain maintainer-only. The focused profile builds and audits only `Quire.Mpl` and runs only its
test projects. For `src/` and `tests/` changes outside them, the gate also runs the phase profile when the Windows
.NET CLI is available. The branch then merges only if both runs pass. Where that CLI is not
available, the gate blocks those changes as before unless a maintainer passes `--allow-unvalidated` after a separate
Windows phase run. Otherwise it validates a separate clone of the branch tip **with `main`'s copies of every
protected file** (with `--allow-protected`, the branch's, except the two baselines,
`docs/mirror/validation-baseline.json` and `docs/mirror/validation-baseline-phase.json`, which always come from
`main`): an unscoped focused run, mutating the changed methods (see What the gate mutates). It merges only when all tests pass, the
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
The author checks test quality and reads the complete diff. The runtime gate is the pre-merge check; there is no
separate independent reviewer ([D14](decisions.md)).

### Validating outside the merge lock

The lock that serialises merges is a process rule (lane and maintainer scripts), not part of the gate. To hold it
only for seconds, a gate run splits in two:

```sh
python3 tools/validation/merge_gate.py <branch> --validate-only         # no lock; records a passed run
flock /tmp/quire-merge.lock python3 tools/validation/merge_gate.py <branch> --apply-validated
```

- **`--validate-only`** runs exactly what the one-shot gate runs, on the branch based on current `main` (V), and
  merges nothing. When the run passes, the gate process, after the validator has exited, writes its record as a
  commit object at `refs/quire-gate/<branch>` in the repository: `gate.json` (base V, tip, changed paths,
  `commits`, `durations`, the maintainer overrides, and `tools`, the blobs of `merge_gate.py` and `validate.py`
  at V), the new baseline and, when the phase ran, the new phase baseline. Code under test runs in a clone that
  has neither this repository's path nor a remote, so it cannot reach the record by accident. The results
  directory is for people; nothing in it is read when applying. A new `--validate-only` run first deletes the
  branch's record, so a failed run never leaves an older pass to apply. The record is self-reported: apply binds it
  only to the branch tip and the gate tools' blobs, and recomputes what the branch changes from git, so it rules out
  accidents, not forgery. Anyone who can write refs could forge a record, just as they could run `git merge`.
- **`--apply-validated`**, from the checkout that has `main` checked out, after the usual pull-first, under the
  lock: blocks (exit 1) without a readable record, or when the record is not a passed run of this branch, or its
  `tools` differ from the blobs at V, or the branch (resolved as `refs/heads/<branch>`) changes protected paths
  without a record of an `--allow-protected` run, or paths only the phase profile validates without a phase run (or
  `--allow-unvalidated`) in the record. The branch's changed, protected and stub-deleted paths are recomputed from
  V..tip, never read from the record, and a record of a prose run must be of a branch that git shows changes only
  allowlisted prose. It also blocks when a path is a file on one side of the replay and a
  directory on the other, or when the landed tree has paths that differ only in case. Otherwise it lands the run
  (exit 0) or reports it stale (exit 4) and deletes the record in both cases. It is stale when:
  - the branch no longer points at the validated tip;
  - current `main` (M) does not contain V;
  - V ≠ M and the branch changes protected paths (stub deletions included), the run used `--allow-protected` or
    `--allow-unvalidated`, or the branch has merge commits;
  - a path changed on V..M is also changed by the branch (renames count by both paths), is protected under V's or
    M's rules, or is an input of the profiles that ran. Inputs are default-deny: everything except the prose
    allowlist and, for a focused-only run, `src/` and `tests/` of projects outside the focused closure
    (`src/Quire.Mpl`, `src/Quire.Mpl.Emit`, `src/Quire.Mirror`, `tests/Quire.Mpl.Tests`,
    `tests/Quire.Differential.Tests`, `tests/Quire.Harness.Tests`, `tests/Quire.Focused.Differential.Tests`). A test
    follows the focused test projects' `ProjectReference`, `Compile` and `Import` items, through imported `.props`
    and `.targets` files and resolved as MSBuild does, and fails on any path outside this list or any MSBuild
    property it cannot resolve; items an SDK adds implicitly and root files (`Directory.Build.*`, `global.json`),
    which are inputs anyway, are not followed. For a run that
    included the phase profile, everything except the prose allowlist is an input, so any concurrent code merge
    makes it stale;
  - M's `validation-baseline.json` differs from V's in any byte, or, for a run that included the phase profile,
    M's phase baseline does; or the recorded baseline no longer holds the ratchet against M's;
  - the source lint, run on the landed tree, finds other findings than the recorded baseline's `lint`: the lint
    reads every `src/` file, not only the focused projects', and `main` can move by a maintainer commit that never
    passed the gate. Findings read from the test assembly are left out of this comparison, since they come from
    focused inputs. A prose record has no lint to compare: a prose run, in the one-shot gate as well, runs no lint and
    keeps M's baseline, so a lint change on `main` cannot make it stale.
- **Baselines.** The branch's baselines are never combined with entries `main` gained meanwhile: a different
  focused baseline on `main` means focused code changed, and the combined tree was never tested, so the run is
  stale. When the run lands, its baseline replaces M's (equal to V's, against which it was checked); without a
  phase run, M's phase baseline is kept, as in the one-shot gate.
- **Landing.** With V = M, as the one-shot gate does. Otherwise each commit of V..tip is replayed onto M from
  objects: its own tree (file modes, additions and deletions as the branch has them) with M's versions of the paths
  V..M changed, keeping author and message; then the baseline commit, then a fast-forward.
- **Why it is never weaker than the serial gate.** A gate run's verdict is a function of what the validator reads:
  the focused projects and their test projects, the tools, the maintainer files under `docs/`, root build files,
  the baselines, and the source lint over all of `src/`. Under these rules the tree that lands and the tree that
  was validated agree byte for byte on all of these except other projects' sources, whose only effect on a focused
  run, the lint, is recomputed on the landed tree and compared. With equal baselines, the one-shot gate on M would
  run the same profiles and reach the same verdict, up to test flakiness, which the serial gate has as well. The
  focused closure is pinned by the test above, not derived at run time: a new reference from the focused projects
  must be added to it.
- **Fallback.** On exit 4, validate again without the lock. After two stale results in a row, run the one-shot
  `--apply` under the lock, which cannot go stale; the stale message says how long that holds the lock, going by
  the run just discarded, and `--apply` prints that the lock stays held for the whole validation.
- `--apply` alone keeps working exactly as before, and `gate.json` of every mode now also lists `commits`, `tools`
  and the overrides; a runtime run lists `durations` (seconds per profile).

**Batching.** One branch may carry several reviewed members, one commit each; the gate validates them as one tree,
and `gate.json` `commits` lists them. See [validation-flow](work/validation-flow.md#merge-gate-throughput) for the
size guideline.

**Why the differential suite runs twice.** The plain run records the method cases; coverage needs instrumented
assemblies. Microsoft.CodeCoverage instruments `Quire.Mpl` statically here, and the recorded cases carry the
assembly's SHA-256 and the compared method's metadata token. Measured on one focused suite (2026-10-18): all
348 tests passed under both runs, but every one of the 20,632 case records carried another `Quire.Mpl` hash, and
every case matched across the runs recorded another method token; apart from those two fields, only 23 records
differed, in values that differ between any two runs (string hash codes, temporary paths). Those records attribute comparisons to methods, so they must come from the uninstrumented build;
the run with coverage only measures lines and branches (plain 109 s, coverage 114 s).

## Windows and phase closure

```sh
dotnet build Quire.slnx
python tools/validation/validate.py --profile phase
```

Run these on Windows with .NET 10 and the Windows Desktop runtime. The phase profile runs all
solution test projects separately with strict reports (including metadata shape/status tests),
without a differential filter, and rejects any solution test project its project selection would omit. The
solution build covers compilation of projects that tests may not reference. A second solution test run is
unnecessary; the phase runner already executes those tests and validates their reports.
Engine and UI tests require their own appropriate runtime environment. Never retarget WPF projects
or omit engine tests to claim phase closure. Focused tests do not substitute for those checks.

### Windows phase profile

Run the phase profile natively on Windows with .NET 10 and the Windows Desktop runtime:

~~~powershell
python tools/validation/validate.py --profile phase
~~~

It builds the solution, runs all solution test projects with strict reports, and rejects any test project omitted by
its selection. A second solution-wide test run is unnecessary. Set QUIRE_REQUIRE_MATHCAD=1 when a test requires the
original Mathcad installation. Keep the checkout, results and temporary files on a Windows drive.

Phase runs ratchet against the protected docs/mirror/validation-baseline-phase.json. Its keys include the project
because shared source can compile into multiple projects. Phase validation does not run mutation testing. A checkout's
bin/ and obj/ belong to the build that last used them; the gate avoids stale outputs by using fresh clones.

When a branch changes src/ or tests/ paths that the focused profile does not cover, the merge gate runs focused
validation and then the phase profile. Both must pass, and both baselines must hold the ratchet against main.
Without a usable Windows .NET CLI, the gate blocks those changes unless a maintainer passes --allow-unvalidated
after a separate Windows phase run.

The gate does not create the first phase baseline. A maintainer bootstraps it from an up-to-date main, and commits
it only when every test project passes and the missing baseline is the only reported regression. Until a phase
baseline exists, branches requiring the phase run are blocked before validation. Phase closure remains open; see
WORK.md for the current blocker.
## Audit in this sandbox (2026-10-14)

Base checkout: `18bec37`, isolated from the implementation agent. The shared `.git` is read-only,
so the worktree was created from a local clone at `/tmp/quire-harness-repo`, with working files at
`/tmp/quire-harness-validation`. No production implementation was changed.

Single-process builds of Mpl behavior, focused differential and harness projects succeeded.
VSTest cannot create its communication socket in this sandbox; the strict Windows CLI check also could not
create its test socket. The strict command was exercised and correctly returned failure with an
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
