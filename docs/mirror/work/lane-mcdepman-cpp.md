# mcdepman C++/CLI spike

- Owner: Codex, Orca task `task_c5fad519dd24`.
- Branch: `feature/mcdepman-cpp-spike` (new Orca worktree from `main` at `9527b6b9c151c7838e8f7a800c6c9faecf98f569`).
- Rebased base for the repeated candidate gate: `1e8d06128e7b5cfd179bacab647ec50440299943`.
- Members: `depman.IDepman.SetAutoCalc(bool)` and `depman.IDepman.Remove(long)` from `mcdepman`; declarations only, as in Mathcad.
- Release IL report: `.mirror-cache/release-il-dbee358fcf96/release-il.json`.
- Exact gate stdout (exit 0):

```text
Building candidate Release assemblies
Release IL: PASS; 83 checked; C:\Users\DuyDang\orca\workspaces\Quire\mcdepman-cpp-spike\.mirror-cache\release-il-dbee358fcf96\release-il.json
```

Both selected rows have `declaration_only: true`, `shape_matches: true`, and no structure differences. Original tokens: Remove `0x060002cd`, SetAutoCalc `0x060002da`. `long h` emits `int32 modopt([System.Runtime.CompilerServices.VisualC]System.Runtime.CompilerServices.IsLong)`; `bool b` emits `marshal(unsigned int8)`. Candidate IL from the initial production Release build stays in `.mirror-cache/release-il-66345c2f60a4/candidate.il`. The production Release project also builds successfully with zero warnings/errors.

Remaining mismatches: none in this two-member spike. Remaining migration: the rest of mcdepman and existing C# depman mirrors in Quire.Mcd; this spike does not remove that host's declarations.
