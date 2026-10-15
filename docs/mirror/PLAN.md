# Mathcad Prime 12 method-by-method mirroring plan

Mirror each in-scope type/member's name, kind, inheritance, visibility, signature, and behavior;
only rename under [mapping.md](mapping.md). Applies to all phases and existing hand-written code.

**Original:** `C:\Program Files\PTC\Mathcad Prime 12.0.1.0`
(WSL: `/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0`); `MATHCAD_DIR` overrides defaults.
Bind contracts to assembly/version/SHA-256/MVID and exact signature/token; invalidate on artifact changes.

## Execute: load → mirror → checkpoint → merge

1. **Load:** Read PLAN + [WORK.md](WORK.md). Take WORK's next batch; extract only its source,
   manifest/status rows, evidence, dependencies/callers, and tests. Confirm artifact identity; reuse valid
   contracts. Fetch missing original evidence only. Record exact overloads before implementation.
2. **Mirror:** Apply rules below. Analyze shared dependencies once, then implement and test the batch.
   Run affected behavior/regression and feasible differential tests; inspect failures, retain concise results.
   Refresh coverage/status once after member changes. Implement a stub by deleting its whole `[MirrorStub]` line
   pair from `Generated/Mirror` (the gate allows only that there; constant stubs stay) and declaring the member with the same shape; never
   commit `docs/mirror/manifest/*` (maintainer-owned: `git checkout main -- docs/mirror/manifest`). Regeneration and
   shape changes are maintainer work; never auto-accept removals. Broaden/repeat checks only for changes or unresolved failures.
3. **Checkpoint:** Update member evidence/status, then replace WORK with remaining exact members, next action,
   changed/evidence paths, blockers, and validation command + passed/failed/skipped counts/case limits.
   Reference durable records rather than repeating findings or old logs. Stop at the evidence-supported boundary.
4. **Merge:** Work on a branch per batch, based on current `main`; only the [merge gate](validation.md#merge-gate)
   merges into `main`.

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
4. **Deviation:** Before relying on a departure, require an explicitly accepted, tested
   [ledger entry](deviations.md) with evidence, behavior, rationale, impact, revisit plan, decision.
   Report separately from matches. Notes authorize nothing; unknowns stay open, bugs need fixing.
5. **Blocked:** Private/unreachable members still need analysis. Missing/unreadable/ambiguous original:
   record exact member/reason, request needed access/clarification, continue independent work; never guess.
6. **Status:** One supported status/member ([definitions](coverage.md)): Stub/Unclassified → Analyzed
   → Tested (independent tests pass) → Verified (direct cases). Blocked/Documented incomplete; Deviation follows
   rule 4; Abstract checks original declaration; Quire-only proves no counterpart. Include fields/accessors,
   constructors/events/lifecycle; separately check coverage-excluded explicit interfaces, enum values, delegates.
7. **Done:** Every in-scope member needs evidenced implementation/tests, accepted tested deviation, or justified
   declaration-only/Quire-only status. Stub/Unclassified/Blocked/Documented/Analyzed prevent behavioral completion.
   Separate matches, case-limited verification, deviations; disclose unresolved work at administrative closure.

**Phase closure only:** `dotnet test Quire.slnx`, manifest-shape/status-record checks, planned stub reduction,
coverage-excluded checks, `Quire.exe --render` screenshot validation, a passing direct-method gate for the
phase's projects (existing differential tests migrated to `MethodComparison.Compare`), and reviewer pass
separate from author before phase commit. Passing tests/type presence alone never establish equivalence.

## References: open only on trigger

| Need | Read |
|---|---|
| Scope/exclusions/native/scripts/API | [scope.md](scope.md), selected CSV rows, `Rules.cs` |
| Mapping/migration | [mapping.md](mapping.md), `Rules.cs`/`Mapper.cs`/`Identifier.cs` |
| Generator/merge/shape | [tooling.md](tooling.md), relevant `notes.txt` entries |
| Engine result/Phase 2 consumer | [result-contract.md](result-contract.md): gates dependent work |
| Another phase | [roadmap.md](roadmap.md): 11 phases; prioritize Phase 1 value/runtime dependencies |
| Decision/substitute | [decisions.md](decisions.md): D4–D7 open; no inferred approval |
| Audit/history | [STATE.md](STATE.md), [history.md](history.md): dated, not current evidence |

Coverage/status own live classifications; CSV `missing`/`exists` are inventory labels. Licensing/COM assemblies
remain deferred; native/scripts/Axiom/user-function C API remain in scope. Substitutes require recorded decisions.
MirrorGen reads structural metadata; it neither executes methods nor analyzes bodies. Keep original code, raw
analysis and logs outside checkpoint/repository; never reload whole inventories/generated trees/history by default.
