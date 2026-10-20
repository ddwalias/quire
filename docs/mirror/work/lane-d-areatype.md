# Lane D — AreaType and default serializer helper

## Completed batch

- **Branch:** `mirror/d-persistentdata-areatype`; merged to `main` at `aaca91d1` on top of `71932051`.
- **Scope:** AreaType Tier A `.cctor()`, `get_Protect`, Regions accessors, null `ProtectionDataWrapper` accessors, and `worksheetRegionType.CreateDefaultSerializer(Type, string)`.
- **Focused gate:** Harness 97/97, MPL 188/188, Debug and Release focused differential 396/396 each, coverage 396/396; method audit, integrity, and focused ratchet passed.
- **Phase gate:** Calculation 8/8, Controls 16/16, Core 7/7, Engine 97/97, EquationEditor 88/88, Harness 97/97, Mirror 32/32, and MPL 188/188 passed. Scoped Debug and Release differential each passed 13 with 1 skip; Phase ratchet passed. Two differential cases and one coverage case were skipped under existing phase skip policy.
- **Known limit:** AreaType serializer behavior remains partially unsupported because dependent worksheet-region metadata is incomplete. `ProtectableTypesRegistry` is now the next independent batch and can use `AreaType.TypeOfThis`.
- **Evidence:** [AreaType contract](../evidence/Ptc.PersistentData.AreaType.md) and [worksheetRegionType serializer contract](../evidence/Ptc.PersistentDataObjects.worksheetRegionType.md).
