# Lane D — header/footer region adapters

## Active handoff

- **Batch:** `mirror/d-persistentdata-header-footer`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** all three mapped Tier A methods each on `Ptc.PersistentData.footer` and `Ptc.PersistentData.header`: Regions get/set and the MC14 conversion flag getter.
- **Evidence:** [footer](../evidence/Ptc.PersistentData.footer.md) and [header](../evidence/Ptc.PersistentData.header.md); both bind to `Ptc.Core` 12.0.1.0 by SHA-256/MVID and record exact tokens.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataHeaderFooterDifferentialTests'` — 2 passed, 0 failed, 0 skipped.
- **Gate:** pending. Two earlier Lane D candidates passed local Release differential tests and focused gate tests, then stopped before Release differential execution at the same assembly path mismatch; see [Lane D handoff](lane-d.md).
- **Remaining:** run the gate for this batch, then continue with the next unowned Lane 4 Tier A class. `AreaType` and `OleUtils` remain pushed and unmerged.
- **Blocker:** maintainer-owned Release differential assembly lookup in `tools/validation/validate.py`.
