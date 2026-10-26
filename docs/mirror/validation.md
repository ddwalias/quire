# Release IL validation

The only mirroring acceptance criterion is exact Release IL parity ([D17](decisions.md)).
All inspection and comparison commands use Release. Debug IL is never an input.

## Compare a batch

```powershell
python tools/validation/release_il.py --base main --project Quire.Mpl
python tools/validation/release_il.py --base main --project Quire.Mpl --type Quire.Mcd.Dim
```

Production projects affected by the diff are included automatically, together with every project
whose `ProjectReference` closure (its `.csproj` and imported `.props`, such as `Generated/Mirror.props`)
reaches one: a changed `const`, enum value or default argument changes its dependents' IL. `--project`
adds scope; it cannot remove an affected project. Root build configuration changes and
`docs/mirror/type-map.csv` (MethodAudit's pairing input) select all production projects. Each
invocation builds the base and current working tree with `-c Release` and `--no-incremental` into
separate temporary directories, all selected projects in one build. It builds MethodAudit in Release
**from the base commit**, and `lane_merge.py` always runs the main checkout's gate scripts (a lane's
own copy re-runs main's), so a branch cannot change the tool or the rules that judge it; a change to
MethodAudit, MirrorGen or `tools/validation` takes effect for the runs after it lands. It reads production assemblies directly, without a
test host, and never runs tests.

Base inventories are cached in `.mirror-cache/inventory-cache/`, keyed by the base commit, the
project list, the judge's MethodAudit/MirrorGen trees and the install's DLL listing. The merge gate
stores each committed candidate it passes (in the main checkout's cache, shared by every lane), so the
next lane's base scan is usually a cache hit. Uncommitted candidates are never cached; a corrupt or
locked entry is rebuilt. Delete the folder to force a rebuild. Scope diffs list both sides of a
renamed file, so moving a file out of a project affects that project too.

The check requires changed/new methods and every previously exact method in affected projects
to match Mathcad. Removed methods fail. Unchanged unfinished methods are reported but do not
block. Existing methods without a Mathcad counterpart may change or be removed; new ones fail
([D21](decisions.md)). Use `--type` or exact `--method 'Type::signature'` selections when checking members
whose IL was already unchanged; selections must exist. Selecting a type includes its stubs.

The report is `.mirror-cache/release-il-<id>/release-il.json`, with base/candidate inventories
and a build log alongside it. `--results <new directory>` selects another local location.
Exit 0 means pass; exit 1 means mismatch or inspection/build failure. When MethodAudit reports
inspection errors they are printed (first 20) and its inventory is kept in the results folder. A
method that cannot be paired or read (an ambiguous custom-modifier tie, an unreadable body) fails its
own row with the reason in `structure_differences`; it no longer aborts the scan. Reports contain artifact
identities, signatures and verdicts, not original IL. No committed validation baseline is used.

## C++/CLI candidate

```powershell
python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

The base is still the project's C# Release build. The candidate is built from the `.vcxproj` with
MSBuild (`-t:Rebuild -p:Configuration=Release -p:Platform=x64`, OutDir/IntDir in the temporary
directory) and must produce `<project>.dll` (its `TargetName`). It needs Windows, the Windows SDK
desktop x64 libraries, the .NET 10 SDK, and the exact compiler Mathcad used: **MSVC 19.51.36246**,
from Visual Studio 2026 **18.6.2** Build Tools with `Microsoft.VisualStudio.Component.VC.CLI.Support`.
Install it side by side from the 18.6.2 fixed-version Build Tools bootstrapper on the Visual Studio
2026 release history page, never update that instance, and check that its `cl.exe` reports
`19.51.36246`. Its toolset folder is still named `14.51.36231`, so the folder name does not identify
the build. The gate finds the instance through `vswhere` by compiler build, not `-latest`. It uses
that instance's MSBuild, gives builds of projects that contain a `.vcxproj` or reference a project
that does its path as the `QuireMsvcInstallDir` environment property, and fails a DLL whose Rich
header shows another linker build or a newer tool build. Gates for Utl's dependents (Engine, Mcd,
Efi, Engine.Native) therefore need the pinned instance too. MethodAudit reads the Mathcad install, and builds
may read it too ([D27](decisions.md)). A missing prerequisite fails the run. The path must be inside the
repository. Release is selected by configuration name; the DebuggableAttribute Debug check is not
verified for MSVC output, so exact IL matching is what rules out a Debug-shaped candidate.

The production `src/Quire.Engine.Utl` `.csproj` builds the mixed-mode DLL in Release itself, so the
plain `--project Quire.Engine.Utl` command (and `lane_merge.py`) checks that candidate. It must build
its `.vcxproj` with the MSBuild under `$(QuireMsvcInstallDir)` and fail when that property is empty.

MethodAudit reads the mixed image's metadata and IL bodies with the same exactness rules. For a
mixed-mode Quire assembly it pairs what the C++/CLI compiler emits as follows:

- C++ types the compiler emits under their C++ names (`_RTL_CRITICAL_SECTION`, `std.exception`,
  `std.basic_string<…>`, `<CrtImplementationDetails>.*`) cannot carry `[MirrorOf]`. Such a type is
  the same-named type of a Mathcad assembly the Quire assembly already binds, when the type map lists
  that original as `c++ artefact` or not at all (template instances and compiler-generated types,
  which the map leaves out), and its kind and native size match. A type the map mirrors still needs
  its reviewed `[MirrorOf]`; a type with no such original has no counterpart and fails.
- global `<Module>` functions pair with the `<Module>` of the one mixed-mode Mathcad assembly the
  Quire assembly binds (`utl-mgd`), by signature with those types mapped, and their IL is compared
  like any other method's. Unpaired ones have no counterpart and fail; their row ids are the
  signature, with custom modifiers added only where overloads share one.
- unpaired MSVC support members are reported as `toolset_support` and do not fail ([D19](decisions.md)):
  globals named `std.*`, `__std_*`, `_CRT*`, `__security_*`, `___Cxx*`, `__eh*`, `__Exception*`,
  `_atexit_*`, or exactly `_onexit`, `_getFiberPtrId`, `PtrToStringChars`, `new`, `delete`, `delete[]`,
  and types under `std.`, `<CrtImplementationDetails>` or `vc.cppcli.`. The allowance needs
  MethodAudit's `original_name_taken: false`: when the bound Mathcad assembly has a member of the
  same owner, name and arity, the row is a pairing miss and fails. Other unpaired members, including
  instantiations of Mathcad's own templates, fail; so does a member that was exact or that lost its
  counterpart.
- methods whose RVA holds native code have no IL. Against a native Mathcad counterpart with a matching
  declaration they are reported as `native_pairs`; otherwise they are `unsupported` and fail.
- removing a C# stand-in with no Mathcad counterpart (such as `_tildeSafeMutex`) passes once the
  project is mixed-mode.
- the D10 `engine_native` exemption applies only to IL-only Quire assemblies; a mixed candidate
  can emit those bodies, so it must match them.

Quire-authored types under `std` or `vc::cppcli` also receive the toolset allowance; that is an
accepted risk.

Names the compiler derives from the source path (`?A0x<hash>` anonymous namespaces, such as
`vc.cppcli.attributes.?A0x….CppInlineNamespaceAttribute` and `__clr_placement_new_t`) differ between
checkouts and never match Mathcad's. Members the original has and the candidate lacks are not
reported; only the candidate's rows are checked.

## Inspect a mismatch

```powershell
python tools/validation/il_diff.py Quire.Mcd.Dim::Div --project Quire.Mpl
python tools/reference/reference.py Mcd.Dim --il
```

`il_diff.py` builds Release production assemblies and prints the first differing instructions.
It is a diagnostic; use `release_il.py` for acceptance. The reference command reads the original
installed Mathcad artifact and caches its IL locally.

MethodAudit defaults to the strict Release comparison and rejects assemblies marked with
disabled optimization. It preserves opcode forms and local slots, resolves token operands
through the existing original-name mappings, and compares branch destinations, locals and
exception regions. A structural fingerprint alone cannot pass. Raw DLL/token bytes differ
because Quire has its own assembly identities and metadata layout. Framework assembly identity
is normalized through framework type names. Unresolved compiler-generated members fail when
changed; the workflow does not claim they have been compared.

Strict operands and locals keep what C# and C++/CLI can make differ: custom modifiers
(`modopt(IsLong)`, `modopt(IsConst)`, `modreq(IsVolatile)`, `modreq(InAttribute)`), `pinned` locals,
array shapes, signature headers (`instance`, `explicit`, unmanaged calling convention, `vararg`) and
full function-pointer signatures. Non-framework types render with their mapped assembly, so two
same-named types from different assemblies do not match; a module's own `<…>` types render as
`[<self>]`. A P/Invoke additionally compares each parameter's and the return value's attributes
(`In`/`Out`/`Optional`) and `[MarshalAs]` descriptor. RVA data typed by a modifier-qualified native
type (vtables, `ThrowInfo`) holds layout-dependent addresses and is not hashed.

Generated nested types inherit the reviewed enclosing type's mapping only when the exact
nested name exists as a compiler-generated type in the same original artifact. Unmapped
or differently numbered helpers still fail; no generated body is exempted.
Generated ordinals depend on member declaration order, including field declarations;
align source order when the compiler emits different numeric suffixes. Every member counts,
accessors and nested types included, across partial files in compile order (`Generated/`
first), so generated stubs precede hand-written members. Placeholder stubs declared in
the hand-written file keep Mathcad's order while members are unfinished.

A body that is only a `MirrorStub` throw before and after is unchanged even when the
source file it records moved; it stays unfinished and is checked once implemented. A new
generated stub (regeneration mapped another member) is unfinished too when its declaration
matches its Mathcad counterpart; one without a counterpart still fails.

Explicit implementation names come from their MethodImpl interface identities. Inventory
IDs/signatures use that canonical identity, while `declared_signature` retains the compiled
spelling. Changing `global::` or namespace qualification therefore does not remove a method.
Selectors accept both canonical and declared signatures.
Field `ldtoken` operands include their field type and available RVA data hash in strict IL.
A string literal starting with a pack URI that names the assembly holding it
(`/Ptc.Calculation.Core;component/…` in Mathcad, `/Quire.Calculation.Core;component/…` in Quire)
compares with that name as `<self>`; the rest must match ([D21](decisions.md)).

## Merge

```powershell
# In the clean agent branch worktree:
python C:/Projects/Quire/tools/validation/lane_merge.py mirror/<lane>-<unit>

# Check only, from a clean main checkout:
python tools/validation/merge_gate.py mirror/<lane>-<unit>
```

The lane wrapper rebases onto a fresh `origin/main` and validates the committed candidate in a
temporary clone **outside** the merge lock, so lanes validate in parallel. It then takes the lock,
fetches and fast-forwards `main`, and lands one of three ways, recorded under `landing.path` in the
receipt:

- `validated`: `main` has not moved; the validated tip lands.
- `rebased-unaffected`: `main` moved, the rebase is clean, the rebased branch affects no more
  projects than were validated, and the new `main` commits touch none of the candidate's inputs
  (compared projects and their forward reference closure, `src/` files outside project folders,
  root files other than `*.md`, the type map, MethodAudit, MirrorGen, `tools/validation/`, and for
  native projects their folders, `docs/mirror/native/` and the assembly map). It lands without building.
- `revalidated`: anything else is validated again under the lock, as before.

A rebase conflict is aborted and its files listed. There are no stored validation refs, baseline
commits, protected-list approvals or two-stage validation records. Changes without production
inputs skip compilation. The merge receipt is `.mirror-cache/gate-il-<id>/gate.json`.

Native projects (`src/<P>/` with `<P>.vcxproj` and no `<P>.csproj`) the branch affects are checked by
`native_gate.py` after the IL check; both must pass. A change to `docs/mirror/native/internal-targets.json`,
the assembly map, `native_bytes.py` or `native_gate.py` selects every native project. Native work no
longer lands unchecked.

Pass the same `--project`, `--type` or `--method` selectors to `lane_merge.py` when needed.
Keep the candidate fixed during validation. Never force-push `main`.

## Entry points

`release_il.py` checks the batch, `il_diff.py` inspects a mismatch, and `lane_merge.py` lands
it through `il_merge.py`. For native projects, `native_gate.py --base main --project <P>` is the batch
check and `native_bytes.py` the per-function harness. `native_bytes.py` is strict by default:
unverified call/address targets fail unless `--allow-unverified` is given (`--strict` is accepted and
does nothing). Export, import-slot and import-thunk targets include the relocation's addend; data
exports are byte-compared; manifest names match regardless of the `?A0x<hash>` anonymous-namespace
tag, which depends on the checkout path.

For `/GL` projects (D30), `native_linked.py --score` adds each failing export's similarity to the original (0–1),
and `native_permute.py` searches source shapes for a near miss, in the manner of decomp-permuter:

```
python tools/validation/native_permute.py --project Quire.Native.EfistatsciImpl --source mutils_mat.cpp \
    --function matdbl_sum --inline validate --symbol matdbl_sum --minutes 20
```

Write the export first, in the shape you would land, so the project builds. `--function` names the bodies to rewrite
(`name` or `file:name`, repeatable). `--symbol` names the exports to match. `--inline` expands calls to a shared
helper inside those bodies, so the search can vary a private copy without breaking the helper's other users. An
attempt that breaks an export that already matched is never kept. On a match, the change is reduced to what the match
needs, then checked strictly against every export. The tool writes `match.patch` (apply it with `git apply` from the
repository root, then run `native_gate.py` as usual) and `report.txt` under `.mirror-cache/permute/`. The working
tree is never written. When a search stalls, the improvement diffs and `native_linked.py --score` show where the
remaining difference lies. A structural difference, such as a loop shape or a helper boundary, may need a hand-written
shape, which `--minutes 0` scores without searching.

The old names `validate.py`, `quick_check.py` and `merge_gate.py`
are small compatibility aliases for these Release IL commands. They contain no runtime pipeline.

The retired validators, mutation tools, acceptance lists and baselines have been removed.
Previous versions are recoverable from Git history. Product regression/differential tests
remain optional engineering tools and are not invoked by the five-step workflow.
