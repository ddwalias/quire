# Lane C checkpoint: wave 1

- Owner: Lane C (D18: `Quire.Utils`). Also this batch: `Quire.Automation`, `Quire.EngNotebook.Automation`,
  `Quire.EngNotebookPlus.Automation`, `Quire.Engine.Misc`.
- Branch: `mirror/c-wave1-20261023165040`. Worktree: `C:/Projects/Quire-wt/lane-c-wave1`, from main `f909e58e`.
- Earlier Calc.Core and Media3D records are in Git history (before this commit).

## Members

- `Quire.Utils`: `BMPFormatter..ctor`, `PNGFormatter..ctor`. Generated `[MirrorStub]` ctors replaced by
  hand-written `public ...() { }` in the type's own file; stubs removed from the `.g.cs`.
- `Quire.Engine.Misc`: `FileUtils.ReadExpanded` throw uses `string.Format("End of file within comment.")`
  instead of the explicit `default(ReadOnlySpan<object>)` argument. Release IL is unchanged.
- `Quire.Automation`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`:
  `ApplicationCreatorClass..ctor` not changed (blocked, see below). Stubs remain.

## Release IL report

- `Quire.Utils`: PASS; 37 checked, 37 exact, 0 failures. Inventory: 42 methods, 0 stubs, 0 unresolved
  (5 interface declarations).
- `Quire.Engine.Misc`: PASS; 129 checked, 129 exact, 0 failures. Inventory: 129 methods, 0 stubs, 0 unresolved.
- `Quire.Automation` / `Quire.EngNotebook.Automation` / `Quire.EngNotebookPlus.Automation`: unchanged from
  main (232 / 216 / 215 exact; 1 unfinished `ApplicationCreatorClass..ctor` each).

## Remaining mismatches and blockers (maintainer)

1. `ApplicationCreatorClass..ctor` (three Automation projects). The original is a ComImport coclass
   (`import` type flag) whose ctor has no body, impl flags `runtime managed internalcall` (0x1003).
   - C# cannot express it: CS0669 rejects any user-declared ctor in a `[ComImport]` class, `extern` included.
     Without `[ComImport]` the ctor would compile, but the Import flag is lost.
   - The gate cannot pass it either: MethodAudit compares only members with IL bodies. A non-abstract,
     non-delegate, non-P/Invoke member with no body reports "not compared: no IL body", so it is never exact
     or declaration-only.
   - Needs a decision on the ComImport-class source form and on how the gate matches no-body runtime members.
   - Origin: the coclass wrapper was most likely produced by the type-library importer (reflection emit), not
     compiled from C# source. Evidence: `interop.mathcadautomationprivate.dll` (`ImportedFromTypeLibAttribute`)
     has a coclass with the same `import` flag and no-body `.ctor() runtime managed internalcall`; the Automation
     DLL's coclass has the same shape but no importer marker. Not yet checked against the `.tlb` GUID.
   - Candidate route: Quire-authored type library for the coclass, imported at build time. Needs a maintainer
     decision and a toolchain check; a build must not read the Mathcad install (D19).
2. `tools/reference/reference.py` cannot read ilspycmd output containing non-UTF-8 bytes (`run()` returns
   None on Windows; `TypeError` in `write_text`). Reproduced for `Engine.Misc.FileUtils` (mcmisc). Workaround
   used: run `ilspycmd -il` into the ignored cache and extract the type block.
3. Not a blocker: the `InlineArrayAsReadOnlySpan`/`InlineArrayElementRef` helpers in `Quire.Engine.Misc` have
   Mathcad counterparts. The original mcmisc `<PrivateImplementationDetails>` defines the same two methods, and
   `LispUtils.badSyntax` / `XmlUtils.TestCheck` call them the same way. No call-site change is needed.
