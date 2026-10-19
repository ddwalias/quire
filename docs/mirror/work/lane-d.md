# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-mc14-annotation-extensions`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [MC14 annotation checkpoint](lane-d-mc14-annotations.md).
- **Other pushed batches:** AreaType's gate stopped before Release differential execution at the shared Release assembly lookup. OleUtils, header/footer, and ProtectionDataType timestamp wrappers have local Release differential evidence; their gates hit the same lookup blocker.
- **Current scope/evidence:** Tier A single-value and enumerable `AddMC14ConversionLossAnnotations` overloads; see [MC14 extension contract](../evidence/Ptc.PersistentDataObjects.IMC14AnnotationProviderExtensions.md). Original artifact identity and tokens are recorded there.
- **Validation:** focused Release differential run with Mathcad required passed 2, failed 0, skipped 1.
- **Gate:** MC14's corrected gate passed build, Harness (97), MPL (188), and focused differential (392), then stopped before Release differential execution because `tools/validation/validate.py` searched for `Quire.Focused.Differential.Tests.dll`, while the project emits `Quire.Differential.Tests.dll`. The first focused build failure was fixed by renaming the test file to the `*MechanicalDifferentialTests.cs` exclusion suffix. AreaType's gate reached the same shared lookup blocker.
- **Remaining:** `ConvertMC14ConversionLossAnnotationsToStringArray` depends on Lane 5's `EnumerableExtensions.IsNullOrEmpty`. Other Lane D work includes `ProtectableTypesRegistry`, `RegionCollectionPreSerializer.PreSerialize`, and `ProtectionLevelWrapper`.
