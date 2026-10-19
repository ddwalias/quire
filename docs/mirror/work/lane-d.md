# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-mc14-annotation-extensions`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [MC14 annotation checkpoint](lane-d-mc14-annotations.md).
- **Other pushed batches:** AreaType is in the runtime merge gate. OleUtils, header/footer, and ProtectionDataType timestamp wrappers have local Release differential evidence; previous gates stopped before Release differential execution at the shared assembly path lookup.
- **Current scope/evidence:** Tier A single-value and enumerable `AddMC14ConversionLossAnnotations` overloads; see [MC14 extension contract](../evidence/Ptc.PersistentDataObjects.IMC14AnnotationProviderExtensions.md). Original artifact identity and tokens are recorded there.
- **Validation:** focused Release differential run with Mathcad required passed 2, failed 0, skipped 1.
- **Gate:** AreaType's full runtime gate is in progress. Previous Lane D tips stopped because `tools/validation/validate.py` looked for `Quire.Focused.Differential.Tests.dll`, while the focused project outputs `Quire.Differential.Tests.dll`.
- **Remaining:** `ConvertMC14ConversionLossAnnotationsToStringArray` depends on Lane 5's `EnumerableExtensions.IsNullOrEmpty`. Other Lane D work includes `ProtectableTypesRegistry`, `RegionCollectionPreSerializer.PreSerialize`, and `ProtectionLevelWrapper`.
