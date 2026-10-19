# Lane D — AreaType static initialization

## Active handoff

- **Batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** Tier A `AreaType::.cctor()`, `get_Protect`, `get_Regions`, `set_Regions`, `get_ProtectionDataWrapper`, and `set_ProtectionDataWrapper`; `.cctor()` initializes `TypeOfThis` for `ProtectableTypesRegistry`.
- **Evidence:** [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md); original artifact identity, token, ILSpy C# and IL references are recorded there.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataAreaTypeCctorDifferentialTests'` — passed, 2 passed, 0 failed, 0 skipped.
- **Gate:** the expanded tip was gated from this branch's worktree. Build, Harness (97), MPL (188), and focused differential (392) passed; validation stopped before Release differential execution because it looked for `Quire.Focused.Differential.Tests.dll`, while the focused project emits `Quire.Differential.Tests.dll`. The cctor-only tip hit the same blocker. See [Lane D handoff](lane-d.md).
- **Remaining:** mirror `GetXmlSerializerStatic`; its path depends on `worksheetRegionType` serializer factories. Non-null protection wrappers depend on Lane 5's `ProtectionData` wrapper.
- **Blockers:** the shared merge gate's maintainer-owned Release assembly path mismatch; retry both Lane D branches after correction.
