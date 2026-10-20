# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Next batch:** `ProtectableTypesRegistry` (`_map`, `Register`, `GetXmlSerializer`), based on current `origin/main`; create a fresh `mirror/d-persistentdata-protectable-types-registry` worktree. Its `AreaType.TypeOfThis` dependency merged with AreaType at `aaca91d1`.
- **Implementation:** mirror the three declarations from `src/Quire.Core/Generated/Mirror/PersistentData.ProtectableTypesRegistry.g.cs` in a separate partial. Inspect exact IL and compare the registry behavior with Mathcad, including root-name lookup, `useOverrides`, and missing-name behavior. Add focused behavioral and Release differential cases.
- **Gate:** use `python C:\Projects\Quire\tools\validation\lane_merge.py mirror/d-persistentdata-protectable-types-registry --jobs 6` from the lane worktree. The Release assembly lookup and linked-worktree rebase issues are fixed in current main; the gate validates outside the shared lock and retries stale receipts.
- **AreaType completed:** `mirror/d-persistentdata-areatype` merged at `aaca91d1`; see [completed AreaType receipt](lane-d-areatype.md).
- **Other pushed branches:** annotation extensions, header/footer, OleUtils, protection timestamp, and worksheet serializer remain on origin. Their previous gate receipts predate the validation fixes; check current ownership and diff against main before retrying or taking overlapping scope.
- **Later dependencies:** non-null protection wrappers depend on Lane 5's `ProtectionData` wrapper; `RegionCollectionPreSerializer.PreSerialize` depends on `EnumerableExtensions.Each`; `ProtectionLevelWrapper` uses `EnumHelper`. Keep these separate until the dependencies land.
