# Mathcad Prime 12 method-by-method mirroring plan

Mirror each in-scope type/member's name, kind, inheritance, visibility, signature, and behavior;
only rename under [mapping.md](mapping.md). Applies to all phases and existing hand-written code.

**Original:** `C:\Program Files\PTC\Mathcad Prime 12.0.1.0`; `MATHCAD_DIR` overrides defaults.
Bind contracts to assembly/version/SHA-256/MVID and exact signature/token; invalidate on artifact changes.

## Execute: load → mirror → checkpoint → merge

**Pipeline** (user decision, 2026-10-18; it supersedes conflicting text in the steps below):
- **Where:** lanes run as native Windows sessions in separate worktrees of the shared clone `C:\Projects\Quire`,
  at `C:\Projects\Quire-wt\<branch>`. Remove a worktree after its branch merges. Mathcad stays read-only at its
  install path.
- **Batch:** a whole class or a functional cluster of about 15–30 methods (the remaining Function methods; whole
  MatVal or MatDim operations).
- **Local checks:** run only the affected behavior and differential test classes. Do not run the focused validator or
  advisory `quick_check.py` on every batch; the merge gate performs the authoritative shape, coverage, ratchet and
  mutation checks. Use scoped validation or `quick_check.py` to diagnose a specific issue, and local `--mutate` only
  to prepare proposals or chase a gate survivor.
- **One mutation pass:** the gate's, in its own clone with `--no-mutant-cache`. There is no reviewer ([D14](decisions.md)).
- **Jobs:** `--jobs 14` for `validate.py` and `merge_gate.py` (16 cores, 31 GiB).
- **Scratch:** before a new unit, `python tools\validation\sweep.py` removes stale validation, coverage, audit and
  gate folders from the temporary directory (unchanged for 12 hours, so never a running gate's).
- **Merge:** `python C:\Projects\Quire\tools\validation\lane_merge.py <branch>`
  ([validation-flow](work/validation-flow.md#merge-gate-throughput)): `--validate-only` outside the merge lock,
  then `--apply-validated` and the push of `main` under it; not the one-shot `--apply`. The per-batch command
  template is there too.

1. **Load:** Read PLAN + [WORK.md](WORK.md). Take WORK's next batch; extract only its source,
   manifest/status rows, evidence, dependencies/callers, and tests. Confirm artifact identity; reuse valid
   contracts. Fetch missing original evidence only. Record exact overloads before implementation.
2. **Mirror:** Apply rules below. A batch is a whole class or a functional cluster of about 15–30 methods within
   the assembly; split unrelated type groups into separate checkpoints. Analyze shared dependencies once, then
   implement the batch. During iteration and before handoff, run the affected behavior tests and direct
   differential tests, for example in PowerShell:

   ```powershell
   $env:QUIRE_REQUIRE_MATHCAD = '1'
   dotnet test tests\Quire.Focused.Differential.Tests\Quire.Focused.Differential.Tests.csproj `
     --filter 'FullyQualifiedName~<related test class>'
   ```

   Keep local checks scoped to the change;
   the final runtime merge gate owns the full regression, shape, method audit, coverage, ratchet and mutation
   pass. Run scoped `validate.py --method-type <type> --diff-filter <filter>` locally when diagnosing those
   checks, rather than as a mandatory second gate. Local mutation is optional unless the batch needs survivor
   or unreachable-line proposals (step 4). Missing coverage may therefore surface at the gate. A targeted pass
   supports only its executed cases; it does not establish audit readiness, project-wide or phase completion.
   Refresh coverage/status locally after member changes if needed (`MirrorGen manifest`; `coverage.md` and
   `status/*.txt` are generated and gitignored, see [status/README.md](status/README.md)). Implement a stub by deleting its whole `[MirrorStub]` line
   pair from `Generated/Mirror` (the gate allows only that there; constant stubs stay) and declaring the member with the same shape; never
   commit `docs/mirror/manifest/*` (maintainer-owned: `git checkout main -- docs/mirror/manifest`). Regeneration and
   shape changes are maintainer work; never auto-accept removals. Broaden/repeat checks only for changes or unresolved failures.
3. **Checkpoint:** Update member contracts/status in their evidence records. Keep remaining exact members, next
   action, changed/evidence paths, blockers and validation commands/counts/case limits once in the lane checkpoint.
   WORK is a short index linking active lane checkpoints; update it only when that index or next action changes.
   Evidence links to the checkpoint's run results instead of repeating them. Stop at the evidence-supported boundary.
4. **Merge:** only the [merge gate](validation.md#merge-gate) merges into `main`.
   - **Branches:** one `mirror/<lane>-<unit>` branch per batch, based on current `main`. Never commit to `main`, not
     even lane notes or evidence: they reach `main` through the gate like code. Push only your own `mirror/*` branches
     to `origin`, for backup, and use `--force-with-lease` only on those; never force-push `main`. The gate pushes
     `main` after it applies a passing batch. Read your complete diff; squash once before queueing the gate.
     Rebase only when `main` has moved past the branch's base. `merge_gate.py` blocks a branch not based on current
     `main`; `lane_merge.py` pulls `main`, rebases the branch onto it and the gate then validates the rebased
     tip. So after a clean rebase do not re-validate locally;
     rerun affected tests and review the resolved hunks when the rebase had conflicts in `src/` or `tests/`.
   - **Pull first:** other checkouts also merge into `origin/main`. `lane_merge.py` fast-forwards local `main` to
     `origin/main` under the merge lock (`merge_lock.py`) before rebasing the branch and again before applying the
     gate's record. If the fast-forward fails because local `main` has merges not yet on `origin`, stop: replay
     those commits onto `origin/main` in an integration branch, leaving out the generated "Update validation
     baseline after merging ..." commits (the gate rewrites the baseline), and gate it. The lock covers only this
     machine's sessions: another machine can push in between, and the push is
     then rejected as non-fast-forward, so integrate the same way. Never force-push `main`.
   - **Pushing `main`** (user decision, 2026-10-16; it replaces "never push `main`" above): whoever merges a branch
     through the gate, lane agent or maintainer, pushes `main` to `origin` right after the merge (with the gate's baseline commit), still
     under the lock, so the other checkouts pull it. Only a gate-applied fast-forward is pushed; never force-push `main`.
  - **Local validation:** affected behavior/direct differential tests are the normal handoff check. Full local shape,
    coverage, ratchet and mutation validation is optional for ordinary batches; the runtime gate requires all of it.
    Use scoped `validate.py --method-type <changed types> --mutate` when investigating test sensitivity
     or preparing survivor/unreachable-line proposals. It mutates every compared method of those types, a superset
     of what the gate mutates in them; the gate may also mutate other types' methods (callers, field readers, users
     of a changed test helper). A local run
     without mutation does not establish mutation readiness. The last line separates the run's own checks from the
     gate's likely decision (`LOCAL CHECKS: … · GATE: …`; outcomes in
     [validation.md](validation.md#branch-coverage-mutation-and-status)). Both commands accept `--jobs`; the default
     is one worker per logical CPU, capped at 12. Lower it when memory or CPU contention slows the host.
   - **Prose edits:** the gate has an explicit allowlist for existing UTF-8, non-executable prose files, including
     WORK and the lane checkpoints. Eligible edits retain the existing baseline and skip runtime validation;
     normal branch, protected-path and case-collision checks still apply. Evidence, policy,
     configuration, tooling, additions/deletions and mixed changes use the full gate. `--full-validation` forces
     the runtime gate; explicit mutation audit flags also force it. See [validation.md](validation.md#merge-gate).
   - **Survivors and uncovered lines:** a surviving mutant needs a stronger case; a mutant that crashes the test host
     is killed, never proposed. Only a provably equivalent survivor may be proposed. A line left `uncovered` needs a
     case; only a line that cannot execute in Mathcad's original either (e.g. `return null;` after a helper that
     always throws) may be proposed, with an entry generated by `method_coverage.py unreachable-entry`
     ([validation.md](validation.md#branch-coverage-mutation-and-status)) whose reason says why it is unreachable in
     the original too; mutants on that line cannot be killed, so propose each for `accepted-mutants.txt` with the
     same reason. Never restructure or join source lines to avoid one. The path, for both:
     1. the lane agent runs scoped local mutation, writes each exact key or entry with its proof in a scratch file,
        passes it to local runs
        (`--proposed-mutants` / `--proposed-unreachable`), and pastes the same lines into the lane file;
     2. the lane hands them to the maintainer; there is no reviewer and no user approval ([D16](decisions.md));
     3. the maintainer (the Claude maintainer session) adds them to `accepted-mutants.txt` /
        `unreachable-lines.txt` on the branch and merges it through the gate with `--allow-protected`, checking that
        `gate.json` `protected_changed` lists only those two files.

     A run whose only blockers are pending proposals (`LOCAL CHECKS: pass · GATE: would block (N proposed … pending
     approval)`, meaning pending the maintainer's addition) is the expected state, not a failure: record the run in
     the lane file, hand the proposals to the maintainer, and continue with other independent work on a new branch
     from `main`.
   - **Review:** there is no independent review step ([D14](decisions.md)); the merge gate is the check before
     `main`. The author reads the complete diff before merging. `src/` changes in types another lane owns still
     need that lane owner's OK; protected-file changes need the maintainer. D14 supersedes the review requirements
     that remain in the prose-shortcut and two-batch sections.
   - **Two-batch integration:** the maintainer may combine at most two queued batches on a
     `mirror/integration-<unit>` branch from current `main`, preserving each batch's squashed commit. Each batch
     keeps its own branch, owner, coherent scope and checkpoint. Each author reads their complete diff; the
     integrator reads the combined diff and conflict resolutions. Record original and integrated commit ids,
     owners, and final base/tip in the lane checkpoints. Keep the candidate fixed during validation. Under the merge
     lock, run the unchanged gate once against
     current `main`'s baseline and apply atomically only on a pass. Its changed-method scope includes both batches;
     use `--mutate-all` if either batch or the audit schedule requires it. Count this as two logical batch merges
     for the periodic audit schedule. Unresolved maintainer-owned acceptance changes and scopes lacking required runtime evidence stay out;
     batching grants no extra protected-path or unvalidated-scope authority. Inspect the entire protected-path
     list. Only the final combined tip is gate-validated; intermediate commits are not validated rollback targets.
     A failed or interrupted earlier run never authorizes this candidate. See [validation.md](validation.md#merge-gate).

     A lane may continue on the next independent batch while another batch is in the gate: use a fresh worktree and
     branch, avoid owned files and shared helpers, and queue it against current `main` after the earlier merge. Never
     edit a candidate whose validation is in progress.

## 3c. Method-by-method fidelity

Stable rule numbers/anchor for D9 and existing reports.

1. **Ownership:** Confirm original owning member before moving behavior. Preserve private/nested members;
   evidence remaining Quire-only code. No name-based or bulk helper migration.
2. **Evidence:** Before behavior changes, read the original with the tool its artifact needs, per
   [evidence/README.md](evidence/README.md): for IL-only assemblies (`mplrun`) the local reference
   (`tools/reference/reference.py`: ILSpy C#, member tokens, IL on request); for native and mixed-mode C++/CLI
   assemblies (`iengine`) REA, including its native decompiler. Use REA cross-references and call paths to settle
   ownership (rule 1) and trace callers. Record per-member contract, confidence/limits, observed/inferred
   cases; trace relevant helpers, calls, errors, mutation, edge cases, initialization/events/lifecycle.
   Documentation/black-box results corroborate; documentation alone is incomplete. Original code (IL or
   decompiled) stays in the gitignored `.mirror-cache/` and is never committed. Analysis already authorized;
   applicable licensing/legal constraints still need confirming.
3. **Tests:** Reconstruct each body from the original as its source most likely was: same control
   flow, evaluation order, calls, locals, exceptions and mutation, written as idiomatic C# (no decompiler
   artifacts). Raw decompiler output is not the implementation. Behavior/regression tests
   mandatory. Verified means direct Mathcad comparison for recorded cases only; skips, analysis, or inferred cases
   do not prove equivalence. Run the [direct-method validation gate](validation.md#direct-comparison-for-every-method)
   for the selected scope: every method with a body, including private methods, needs an artifact-bound direct
   Mathcad comparison from a passing test in the current run. Public-caller coverage does not satisfy private
   method validation. Report all remaining gaps; a scoped pass is not project-wide completion.
   **Release compiler shapes** (structure is compared on a Release build; each verified against `mplrun`):
   `c ? 1 : 0` compiles to a branchless `ceq`/`cgt`/`clt`, so no unsafe byte reads; `if (c) return A; return B;`
   compiles like `!c ? B : A`, so `c ? A : B` flips the branch and arm order of `if (c) return A; return B;`: keep
   the `if` form rather than writing an inverted ternary; `a ?? b` compiles to `dup; brtrue; pop`, unlike an
   if-null-return through a local; merging early returns into `&&`/`||`, or inlining a local, also changes the IL; write
   `string.Format`, not interpolation, where Mathcad calls `Format`; write `new T[0]`, not `[]`, for an empty array
   (`[]` emits `Array.Empty`); `?.` and `??=` on a type with `op_Equality` (`Type`, `FieldInfo`) differ from
   `== null`; `x is T t` drops the `castclass` of `x is T` then `(T)x`; keep Mathcad's own dead reads
   (`_ = head;`) and explicit default-value field writes. MethodAudit's structure match ignores branch direction
   and block order, so it is weaker than an exact match: compare the Release opcode sequence with Mathcad's too.
4. **Deviation:** Before relying on a departure, require an explicitly accepted, tested
   [ledger entry](deviations.md) with evidence, behavior, rationale, impact, revisit plan, decision.
   Report separately from matches. Notes authorize nothing; unknowns stay open, bugs need fixing.
5. **Blocked:** Private/unreachable members still need analysis. Missing/unreadable/ambiguous original:
   record exact member/reason, request needed access/clarification, continue independent work; never guess.
6. **Status:** One supported status/member ([definitions](statuses.md)): Stub/Unclassified → Analyzed
   → Tested (independent tests pass) → Verified (direct cases). Blocked/Documented incomplete; Deviation follows
   rule 4; Abstract checks original declaration; Quire-only proves no counterpart. Include fields/accessors,
   constructors/events/lifecycle; separately check coverage-excluded explicit interfaces, enum values, delegates.
7. **Done:** Every in-scope member needs evidenced implementation/tests, accepted tested deviation, or justified
   declaration-only/Quire-only status. Stub/Unclassified/Blocked/Documented/Analyzed prevent behavioral completion.
   Separate matches, case-limited verification, deviations; disclose unresolved work at administrative closure.
   In-scope and done are per tier ([D11](decisions.md)): Tiers A/A2 as above; Tier B by exact Release-IL match with
   settled callees, which replaces rule 3's per-method direct comparison once the gate supports it; C/D stay stubs.

**Phase closure only (Windows):** `dotnet build Quire.slnx` for full solution compilation, then
`python tools/validation/validate.py --profile phase` for all test projects and strict reports. The phase runner
rejects solution test projects it would omit. Keep manifest-shape/status-record checks, planned stub reduction,
coverage-excluded checks, `Quire.exe --render` screenshots, a passing direct-method gate for the phase's projects
(existing differential tests migrated to `MethodComparison.Compare`), and the phase gate before merge.
The phase runner supplies the required test execution; a second `dotnet test Quire.slnx` is not required.
Passing tests/type presence alone never establish equivalence.

## References: open only on trigger

| Need | Read |
|---|---|
| Scope/exclusions/native/scripts/API | [scope.md](scope.md), selected CSV rows, `Rules.cs` |
| Mapping/migration | [mapping.md](mapping.md), `Rules.cs`/`Mapper.cs`/`Identifier.cs` |
| Generator/merge/shape | [tooling.md](tooling.md), relevant `notes.txt` entries |
| Engine result/Phase 2 consumer | [result-contract.md](result-contract.md): gates dependent work |
| Another phase | [roadmap.md](roadmap.md): 11 phases; prioritize Phase 1 value/runtime dependencies |
| Decision/substitute | [decisions.md](decisions.md): D6–D7 open; no inferred approval |
| Tier of a member/type | [tiers/](tiers/) per [D11](decisions.md) |
| Audit/history | [STATE.md](STATE.md), [history.md](history.md): dated, not current evidence |

Member attributes own live classifications (reported by generated `coverage.md`/`status/`); CSV `missing`/`exists` are inventory labels. Licensing/COM assemblies
remain deferred; native internals, scripts, Axiom and the user-function C API are tiered by D10/D11. Substitutes require recorded decisions.
MirrorGen reads structural metadata; it neither executes methods nor analyzes bodies. Keep original code, raw
analysis and logs outside checkpoint/repository; never reload whole inventories/generated trees/history by default.
