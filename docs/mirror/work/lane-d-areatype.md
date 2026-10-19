# Lane D — AreaType static initialization

## Active handoff

- **Batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** Tier A `Ptc.PersistentData.AreaType::.cctor()` and its `TypeOfThis` field, a required dependency for `ProtectableTypesRegistry`.
- **Evidence:** [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md); original artifact identity, token, ILSpy C# and IL references are recorded there.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataAreaTypeCctorDifferentialTests'` — passed, 1 passed, 0 failed, 0 skipped.
- **Gate:** the prior Lane D batch reached 392 focused passes but stopped before Release differential validation because `validate.py` looked for a filename that differs from the focused project's configured assembly name. See [Lane D handoff](lane-d.md) on the OleUtils branch; rerun both candidates after the maintainer-owned lookup is fixed.
- **Remaining:** mirror `GetXmlSerializerStatic`, `get_Protect`, `get_ProtectionDataWrapper`, `set_ProtectionDataWrapper`, `get_Regions`, and `set_Regions`; resolve the `worksheetRegionType` serializer factory dependencies before validating the serializer path. `ProtectableTypesRegistry` remains unstarted and unowned.
- **Blockers:** the shared merge gate's Release assembly path mismatch.
