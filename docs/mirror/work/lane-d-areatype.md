# Lane D — AreaType and default serializer helper

## Active handoff

- **Batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** AreaType Tier A `.cctor()`, `get_Protect`, Regions accessors, and null ProtectionDataWrapper accessors; plus `worksheetRegionType.CreateDefaultSerializer(Type, string)` used by AreaType's serializer path. Exact XML-ignore metadata observed for AreaType's ignored members is mirrored.
- **Evidence:** [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md) and [worksheetRegionType serializer contract](../evidence/Ptc.PersistentDataObjects.worksheetRegionType.md); original artifact identity, tokens, ILSpy C# and IL are recorded there.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataObjectsWorksheetRegionTypeSerializerDifferentialTests|FullyQualifiedName~PtcPersistentDataAreaTypeMechanicalDifferentialTests'` — 11 passed, 0 failed, 1 skipped. The serializer helper directly compares string and integer XML roots. The AreaType serializer wrapper is skipped because its dependency graph still cannot serialize mirrored worksheet regions.
- **Gate:** the expanded tip was gated from this branch's worktree. Build, Harness (97), MPL (188), and focused differential (392) passed; validation stopped before Release differential execution because it looked for `Quire.Focused.Differential.Tests.dll`, while the focused project emits `Quire.Differential.Tests.dll`. The cctor-only tip hit the same blocker. See [Lane D handoff](lane-d.md).
- **Remaining:** resolve dependent XML metadata/types needed by AreaType `GetXmlSerializerStatic`; expand default factory direct cases after graph serialization is supported. Non-null protection wrappers depend on Lane 5's `ProtectionData` wrapper.
- **Blockers:** the shared merge gate's maintainer-owned Release assembly path mismatch; retry both Lane D branches after correction.
