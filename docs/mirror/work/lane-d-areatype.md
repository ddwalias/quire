# Lane D — AreaType static initialization

## Active handoff

- **Batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** Tier A `Ptc.PersistentData.AreaType::.cctor()` and its `TypeOfThis` field, a required dependency for `ProtectableTypesRegistry`.
- **Evidence:** [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md); original artifact identity, token, ILSpy C# and IL references are recorded there.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataAreaTypeCctorDifferentialTests'` — passed, 1 passed, 0 failed, 0 skipped.
- **Gate:** `python C:\Projects\Quire\tools\validation\lane_merge.py mirror/d-persistentdata-areatype --jobs 14` ran from this branch's worktree. Build, Harness (97), MPL (188), and focused differential (392) tests passed. Validation stopped before Release differential execution because it looked for `Quire.Focused.Differential.Tests.dll`, but the focused project emits `Quire.Differential.Tests.dll`. See [Lane D handoff](lane-d.md); OleUtils hit the same blocker.
- **Remaining:** mirror `GetXmlSerializerStatic`, `get_Protect`, `get_ProtectionDataWrapper`, `set_ProtectionDataWrapper`, `get_Regions`, and `set_Regions`; resolve the `worksheetRegionType` serializer factory dependencies before validating the serializer path. `ProtectableTypesRegistry` remains unstarted and unowned.
- **Blockers:** the shared merge gate's maintainer-owned Release assembly path mismatch; retry both Lane D branches after correction.
