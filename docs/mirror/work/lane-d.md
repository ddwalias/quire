# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [AreaType checkpoint](lane-d-areatype.md).
- **Other pushed batches:** OleUtils, header/footer, and ProtectionDataType timestamp wrappers remain unmerged. Their local Release differential tests passed; their gates stopped before Release differential execution at the same assembly path mismatch.
- **Current scope/evidence:** AreaType Tier A initializer/accessors and `worksheetRegionType.CreateDefaultSerializer`; see [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md) and [serializer helper contract](../evidence/Ptc.PersistentDataObjects.worksheetRegionType.md).
- **Validation:** scoped Release differential run with Mathcad required passed 11, failed 0, skipped 1. The skip is AreaType `GetXmlSerializer(bool)`, blocked by missing serialized-model metadata in dependent types.
- **Gate:** both the cctor-only and expanded AreaType tips passed build and focused suites (392 differential tests) then stopped before Release differential validation because `tools/validation/validate.py` looks for `Quire.Focused.Differential.Tests.dll`, while the focused project outputs `Quire.Differential.Tests.dll`.
- **Remaining:** AreaType `GetXmlSerializerStatic` needs more XML metadata and modeled types in the dependent graph; non-null protection wrappers depend on Lane 5's `ProtectionData` wrapper. `ProtectableTypesRegistry` remains unstarted. `RegionCollectionPreSerializer.PreSerialize` depends on Lane 5's `EnumerableExtensions.Each`; `ProtectionLevelWrapper` uses Lane 5's `EnumHelper`.
- **Blocker:** maintainer-owned fix required for the merge gate's Release differential assembly lookup; retry pushed Lane D branches after correction.
