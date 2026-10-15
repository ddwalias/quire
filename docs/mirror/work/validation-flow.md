# Validation workflow checkpoint

Branch: `mirror/workflow-trim`, based on `main` `fff8ec0`.

## Scope

- Normal handoff uses targeted behavior/direct differential tests; the runtime gate owns full validation.
- Seven explicitly named prose files can reuse the base's validation only for edits to existing UTF-8 regular
  non-executable files. Evidence, configuration, tooling, new files and mixed changes use the runtime gate.
- Phase closure builds the solution, then runs the strict phase validator; the extra solution test invocation
  is removed. The phase runner checks that its selection includes every solution test project.
- Squash once before queueing, rebase only when needed, and keep batch results once in the lane checkpoint.

Changed code: `tools/validation/merge_gate.py`, `validate.py`, and their Python unit tests. Workflow documentation:
PLAN, validation.md and the short WORK index. The existing mutation selection, survivor approvals, runtime
acceptance rules, independent review, and separate differential/coverage passes remain in force.

## Validation and next action

`python3 -m unittest discover -s tools/validation -p 'test_*.py'`: 156 passed, no failures or skips. This includes
prose check/apply routing, unchanged baseline bytes, protected/mixed/non-text changes, explicit audit overrides,
stale branches and phase project selection. CLI help and `git diff --check` passed.

Independent review passed with no blockers. A real CLI check/apply smoke run in a scratch repository completed
the prose route without invoking .NET and preserved the baseline bytes (0.08 s on this workstation; not a full
workflow benchmark).

Merge this tooling branch through the full protected runtime gate; its receipt and any baseline update are
recorded by the merge gate. After merging, resume the implementation lanes linked from WORK. This change does
not establish Windows phase completion.

## Two-batch integration amendment

The user requested following PLAN and improving any demonstrably wasteful execution step. Two independently
reviewed queued batches may now share one unchanged final merge gate, with at most two batches per integration
candidate. Each retains its own coherent scope, squashed commit, ownership and checkpoint. Full combined-diff
review and all required approvals precede validation; audit triggers use the union of both batches and count
logical batches so frequency is retained. No gate implementation, test/mutation selection, acceptance rule or
evidence requirement changes. Only the final candidate is validated, and the entire candidate fails together.

Motivation: the pending runtime test work shares the same build, regression, coverage and audit stages; an
accepted-mutant change also requires a full mutation audit (the current inventory contains 2,112 mutants).
Serial gates for two such queued batches would repeat that complete pass. This amendment permits one final
pass after both are ready, without using an interrupted/failed run as evidence. It does not claim a measured
end-to-end speedup. Protected changes are only PLAN and validation.md; run the existing protected runtime gate
for this amendment before using it. Independent review and the gate receipt must be recorded after execution.

The earlier `02ecce1` full audit at `/tmp/quire-matval-final-runtime-gate` was interrupted after
1,180/2,112 mutants when unmerged DimVal comparison fixes and their pending approvals were identified.
Its gate receipt is blocked (validator exit 241, no sealed summary); it is not a passing audit or merge
authorization. The corrected candidate still requires a fresh complete full audit.
