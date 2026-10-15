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
