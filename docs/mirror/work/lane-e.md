# Lane E — Lane 5 Tier A

## Active handoff

- **Active batch:** `Ptc.IO.EmbeddedResourceCopier.Copy` (`0x06000f88`), whole class. The requested `Ptc.Core.Serialization.ApplicationPropertiesXmlSerializable` Tier A members were already implemented and directly verified on `main` by the generated-member batch, so they remain owned and were not changed.
- **Ownership check:** fetched `origin`; based the branch on `origin/main` at `f863245e`. No Lane E branch/worktree, evidence, or source implementation owned `EmbeddedResourceCopier`; its only Tier A member was a generated stub. Lane D's `Ptc.PersistentData*` scope is excluded.
- **Reference:** Ptc.Core 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`; ilspycmd 11.1.0.9782. Exact signature/token and member contract are in [evidence](../evidence/Ptc.IO.EmbeddedResourceCopier.md).
- **Validation:** `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Differential.Tests/Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~EmbeddedResourceCopierMechanicalDifferentialTests'` — passed, 1 test, 0 skipped. The test checks expected output and directly compares Mathcad Prime to Quire for `System.Private.CoreLib.Strings.resources` and a missing-resource/read-only-destination case. The `MechanicalDifferentialTests.cs` suffix keeps this Quire.Core-only source out of the portable focused build.
- **Remaining Lane 5 scope:** unowned Tier A types in `Ptc.Core.dll` outside `Ptc.PersistentData*`, especially remaining `Ptc.IO`, `Ptc.Serialization`, `Ptc.Xml`, `Ptc.Wpf`, and other Lane 5 types. Before every next batch, fetch and inspect updated `main`, branches/worktrees, lane checkpoints, member status, and source ownership; omit Tier B members covered by generated-member batches.
- **Blockers:** none for this batch. Other remaining classes have not yet been re-audited for ownership or original-artifact accessibility.
