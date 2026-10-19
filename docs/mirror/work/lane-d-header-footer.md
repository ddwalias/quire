# Lane D — header/footer region adapters

## Active handoff

- **Batch:** `mirror/d-persistentdata-header-footer`, created from `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; gate rebases it onto current main.
- **Scope:** all three mapped Tier A methods each on `Ptc.PersistentData.footer` and `Ptc.PersistentData.header`: Regions get/set and the MC14 conversion flag getter.
- **Evidence:** [footer](../evidence/Ptc.PersistentData.footer.md) and [header](../evidence/Ptc.PersistentData.header.md); both bind to `Ptc.Core` 12.0.1.0 by SHA-256/MVID and record exact tokens.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataHeaderFooterMechanicalDifferentialTests'` — 2 passed, 0 failed, 0 skipped.
- **Gate:** after the class/file lint fix, the latest gate passed build, Harness (97), MPL (188), focused (393), Release-versus-Mathcad (393), coverage, IL audit (1544 exact / 80 structure-only), and ratchet. The phase profile then failed because Differential.Tests did not all pass and generated `bin/Debug/net10.0-windows/s1000001` and `s2000001` files that the validator flags. No merge occurred.
- **Remaining:** after the phase blocker is resolved, retry this branch and continue with the next unowned Lane 4 Tier A class. `AreaType` and `OleUtils` remain pushed and unmerged.
- **Blocker:** the phase Differential.Tests suite has unrelated failures, including existing mirror stubs and abstract-constructor cases, and writes files under `bin` that fail validation.
