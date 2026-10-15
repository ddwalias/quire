# Reconciliation snapshot — 2026-10-14

Read only for baseline audits or validation provenance. Resume execution from [WORK.md](WORK.md);
current generated coverage/status supersedes this dated snapshot.

- **Target / phase:** Mathcad Prime 12.0.1.0 at PLAN's install path. Phase 0/type migration complete;
  Phase 1 method fidelity open. [Result contract](result-contract.md) gates dependent Phase 2 consumers.
- **Scope:** 5,372 Mirror CSV entries across 112 assemblies; earlier 5,377/5,378 counts superseded.
  Scope entries differ from generated Quire types. `Rules.ProjectFor` routes projects; `Quire.Efi.Common` unnecessary.
- **Checked-in coverage:** 5,384 types / 56,869 counted members: 55,544 Stub, 1,231 Unclassified,
  23 Verified, 71 Abstract; other counts zero. Source of truth: member attributes, reported by generated `coverage.md` ([status/README.md](status/README.md)).
- **Recorded behavior:** [Mcd.Rational](evidence/Mcd.Rational.md), 23 case-limited Verified members;
  `RationalTests` and `RationalDifferentialTests`. Non-ASCII negative-sign culture comparison remains open.
- **Open:** Remaining Phase 1 methods and hand-written classifications; coverage-excluded surface;
  generator-note reconciliation; D4–D7 and licensing/legal confirmation. No accepted deviations recorded.
- **Access checkpoint:** Installed binaries accessible; REA MCP was not exposed in the reconciliation session.
  Recheck availability on resume. If still unavailable, record blocked analysis; do independent work under PLAN rule 5.

## Recorded validation

Recorded by the earlier 2026-10-14 reconciliation; these are not new test runs from the restructuring edit:

- `MirrorGen scope-diff`: 0 unmatched / 0 unlisted non-generated types.
- Installed `mplrun.dll` SHA-256 matches Rational evidence.
- Windows .NET `dotnet test tests/Quire.Differential.Tests/Quire.Differential.Tests.csproj --no-restore --verbosity quiet`:
  3 passed / 0 failed / 0 skipped against installed original; two existing nullable warnings in `Interpreter.Plot.cs`.
- Windows .NET `dotnet test tests/Quire.Mirror.Tests/Quire.Mirror.Tests.csproj --no-restore --verbosity quiet`:
  32 passed / 0 failed / 0 skipped.
- Earlier local Markdown links and `git diff --check` passed. Full solution tests/rendering were not rerun.

Documentation restructuring rechecked CSV totals (5,372 Mirror entries / 112 assemblies), Rational source
attributes (23 Verified), installed `mplrun.dll` SHA-256, local Markdown paths, and `git diff --check`.
No implementation tests or rendering rerun for this documentation-only edit.
