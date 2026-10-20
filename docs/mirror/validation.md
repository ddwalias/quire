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
