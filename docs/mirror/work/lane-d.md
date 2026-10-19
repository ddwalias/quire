# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [AreaType checkpoint](lane-d-areatype.md).
- **Other pushed batches:** OleUtils, header/footer, and ProtectionDataType timestamp wrappers remain unmerged. Their local Release differential tests passed; their gates stopped before Release differential execution at the same assembly path mismatch.
- **Current scope/evidence:** AreaType Tier A `.cctor()`, Regions accessors, protection getter, and null wrapper accessors; see [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md). Original artifact identity and tokens are recorded there.
- **Validation:** AreaType tests passed in Release with Mathcad required, 2 passed, 0 failed, 0 skipped.
- **Gate:** both the cctor-only and expanded AreaType tips passed build and focused suites (392 differential tests) then stopped before Release differential validation because `tools/validation/validate.py` looks for `Quire.Focused.Differential.Tests.dll`, while the focused project outputs `Quire.Differential.Tests.dll`.
- **Remaining:** AreaType `GetXmlSerializerStatic` depends on `worksheetRegionType` serializer factories; non-null protection wrappers depend on Lane 5's `ProtectionData` wrapper. `ProtectableTypesRegistry` remains unstarted. `RegionCollectionPreSerializer.PreSerialize` depends on Lane 5's `EnumerableExtensions.Each`; `ProtectionLevelWrapper` uses Lane 5's `EnumHelper`.
- **Blocker:** maintainer-owned fix required for the merge gate's Release differential assembly lookup; retry pushed Lane D branches after correction.
