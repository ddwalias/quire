# Release IL validation

The only mirroring acceptance criterion is exact Release IL parity ([D17](decisions.md)).
All inspection and comparison commands use Release. Debug IL is never an input.

## Compare a batch

```powershell
python tools/validation/release_il.py --base main --project Quire.Mpl
python tools/validation/release_il.py --base main --project Quire.Mpl --type Quire.Mcd.Dim
```

Production projects affected by the diff are included automatically. `--project` adds scope;
it cannot remove an affected project. Root build configuration changes select all production
projects. Each invocation builds the base and current working tree with `-c Release` and
`--no-incremental` into separate temporary directories. It builds MethodAudit in Release and
reads production assemblies directly, without a test host. It never runs tests.

The check requires changed/new methods and every previously exact method in affected projects
to match Mathcad. Removed methods fail. Unchanged unfinished methods are reported but do not
block. Use `--type` or exact `--method 'Type::signature'` selections when checking members
whose IL was already unchanged; selections must exist. Selecting a type includes its stubs.

The report is `.mirror-cache/release-il-<id>/release-il.json`, with base/candidate inventories
and a build log alongside it. `--results <new directory>` selects another local location.
Exit 0 means pass; exit 1 means mismatch or inspection/build failure. Reports contain artifact
identities, signatures and verdicts, not original IL. No committed validation baseline is used.

## C++/CLI candidate

```powershell
python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

The base is still the project's C# Release build. The candidate is built from the `.vcxproj` with
MSBuild (`-t:Rebuild -p:Configuration=Release -p:Platform=x64`, OutDir/IntDir in the temporary
directory) and must produce `<project>.dll` (its `TargetName`). It needs Windows, Visual Studio with
the C++/CLI component (`Microsoft.VisualStudio.Component.VC.CLI.Support`, found through `vswhere`),
the v145 MSVC x64 toolset, the Windows SDK desktop x64 libraries, and the .NET 10 SDK. MethodAudit
reads the Mathcad install; builds never do: every build runs without `MATHCAD_DIR` and with
`MathcadInstallDir` pointing at an empty path ([D19](decisions.md)). A missing prerequisite fails the run. The path must be inside the repository. Release is
selected by configuration name; the DebuggableAttribute Debug check is not verified for MSVC
output, so exact IL matching is what rules out a Debug-shaped candidate.

The production `src/Quire.Engine.Utl` `.csproj` builds the mixed-mode DLL in Release itself, so the
plain `--project Quire.Engine.Utl` command (and `lane_merge.py`) checks that candidate.

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
  `_onexit`, `_atexit_*`, `_getFiberPtrId`, `PtrToStringChars`, `new`/`delete`, and types under
  `std.`, `<CrtImplementationDetails>` or `vc.cppcli.`. Other unpaired members, including
  instantiations of Mathcad's own templates, still fail.
- methods whose RVA holds native code have no IL. Against a native Mathcad counterpart they are
  reported as `native_pairs`; otherwise they are `unsupported` and fail.
- removing a C# stand-in with no Mathcad counterpart (such as `_tildeSafeMutex`) passes once the
  project is mixed-mode.
- the D10 `engine_native` exemption applies only to IL-only Quire assemblies; a mixed candidate
  can emit those bodies, so it must match them.

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

Generated nested types inherit the reviewed enclosing type's mapping only when the exact
nested name exists as a compiler-generated type in the same original artifact. Unmapped
or differently numbered helpers still fail; no generated body is exempted.
Generated ordinals depend on member declaration order, including field declarations;
align source order when the compiler emits different numeric suffixes. Every member counts,
accessors and nested types included, across partial files in compile order (`Generated/`
first), so generated stubs precede hand-written members. Placeholder stubs declared in
the hand-written file keep Mathcad's order while members are unfinished.

A body that is only a `MirrorStub` throw before and after is unchanged even when the
source file it records moved; it stays unfinished and is checked once implemented.

Explicit implementation names come from their MethodImpl interface identities. Inventory
IDs/signatures use that canonical identity, while `declared_signature` retains the compiled
spelling. Changing `global::` or namespace qualification therefore does not remove a method.
Selectors accept both canonical and declared signatures.
Field `ldtoken` operands include their field type and available RVA data hash in strict IL.

## Merge

```powershell
# In the clean agent branch worktree:
python C:/Projects/Quire/tools/validation/lane_merge.py mirror/<lane>-<unit>

# Check only, from a clean main checkout:
python tools/validation/merge_gate.py mirror/<lane>-<unit>
```

The lane wrapper refreshes and rebases under the merge lock, then validates an immutable
committed candidate in a temporary clone and fast-forwards/pushes `main` after a pass. It
rechecks the base and tip before applying. There are no stored validation refs, baseline
commits, protected-list approvals or two-stage validation records. Changes without production
inputs skip compilation. The merge receipt is `.mirror-cache/gate-il-<id>/gate.json`.

Pass the same `--project`, `--type` or `--method` selectors to `lane_merge.py` when needed.
Keep the candidate fixed during validation. Never force-push `main`.

## Entry points

`release_il.py` checks the batch, `il_diff.py` inspects a mismatch, and `lane_merge.py` lands
it through `il_merge.py`. The old names `validate.py`, `quick_check.py` and `merge_gate.py`
are small compatibility aliases for these Release IL commands. They contain no runtime pipeline.

The retired validators, mutation tools, acceptance lists and baselines have been removed.
Previous versions are recoverable from Git history. Product regression/differential tests
remain optional engineering tools and are not invoked by the five-step workflow.
