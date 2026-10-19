# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-persistentdata-areatype`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [AreaType checkpoint](lane-d-areatype.md).
- **Previous batch:** `mirror/d-persistentdata-oleutils` is pushed at `606af2f0` and remains unmerged. Its Release Mathcad differential test passed; the lane gate stopped before Release differential validation due the focused assembly path mismatch below. Its test, reference, and full scope are in that branch's earlier checkpoint/evidence.
- **Current scope/evidence:** AreaType Tier A `.cctor()` initializes `TypeOfThis`; see [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md). Original artifact identity and member token are recorded there.
- **Validation:** AreaType `.cctor()` test passed in Release with Mathcad required, 1 passed, 0 failed, 0 skipped.
- **Gate:** both `mirror/d-persistentdata-oleutils` and `mirror/d-persistentdata-areatype` were pushed and gated. Each passed build, Harness (97), MPL (188), and focused differential (392); each stopped before Release differential execution because `tools/validation/validate.py` looks for `Quire.Focused.Differential.Tests.dll`, while the focused project outputs `Quire.Differential.Tests.dll`.
- **Remaining:** AreaType's other Tier A members are `GetXmlSerializerStatic`, `get_Protect`, `get_ProtectionDataWrapper`, `set_ProtectionDataWrapper`, `get_Regions`, and `set_Regions`. Serializer behavior depends on `worksheetRegionType` factory methods. `ProtectableTypesRegistry` remains unstarted.
- **Blocker:** maintainer-owned fix required for the merge gate's Release differential assembly path lookup; retry both Lane D branches after it is fixed.
