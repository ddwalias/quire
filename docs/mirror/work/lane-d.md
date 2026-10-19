# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-persistentdata-protection-timestamp`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [timestamp checkpoint](lane-d-protection-timestamp.md).
- **Other pushed batches:** OleUtils, AreaType initializer, and header/footer all remain unmerged. Their local Release differential tests passed; all three gates passed the build and focused suites, then stopped before Release differential execution at the same assembly path mismatch. The timestamp batch just hit the same gate failure.
- **Current evidence:** [ProtectionDataType contract](../evidence/Ptc.PersistentData.ProtectionDataType.md), bound to `Ptc.Core` 12.0.1.0 by the recorded SHA-256/MVID.
- **Current validation:** timestamp wrapper direct differential test passed, 1 passed, 0 failed, 0 skipped.
- **Remaining:** `ProtectionLevelWrapper` methods depend on Lane 5 `Ptc.EnumHelper`; `RegionCollectionPreSerializer.PreSerialize` depends on Lane 5 `EnumerableExtensions.Each`; AreaType serializer work depends on worksheetRegionType serializer helpers; ProtectableTypesRegistry depends on AreaType and the serializer path.
- **Blocker:** maintainer-owned `tools/validation/validate.py` looks for `Quire.Focused.Differential.Tests.dll`, but the focused project emits `Quire.Differential.Tests.dll`. All pushed Lane D candidates need passing gates after that lookup is corrected. `RegionCollectionPreSerializer.PreSerialize` is additionally dependent on the Lane 5 `EnumerableExtensions.Each` stub; `ProtectionLevelWrapper` uses the Lane 5 `EnumHelper`.
