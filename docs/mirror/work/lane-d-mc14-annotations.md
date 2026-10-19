# Lane D — MC14 annotation extensions

## Active handoff

- **Batch:** `mirror/d-mc14-annotation-extensions`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`.
- **Scope:** Tier A single-value and enumerable overloads of `IMC14AnnotationProviderExtensions.AddMC14ConversionLossAnnotations`.
- **Evidence:** [IMC14 extension contract](../evidence/Ptc.PersistentDataObjects.IMC14AnnotationProviderExtensions.md); artifact identity, tokens, signatures, ILSpy C# and IL are recorded there.
- **Validation:** `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcPersistentDataObjectsIMC14AnnotationProviderExtensionsDifferentialTests'` — 2 passed, 0 failed, 1 skipped.
- **Remaining:** `ConvertMC14ConversionLossAnnotationsToStringArray` calls `Ptc.EnumerableExtensions.IsNullOrEmpty<T>`, a Lane 5 owned helper. Implement after that dependency is available.
- **Gate:** first gate attempt failed while building `Quire.Focused.Differential.Tests`: it includes test sources that do not use the `*MechanicalDifferentialTests.cs` exclusion convention and lacks a Quire.Core reference. Renamed the test file to the excluded convention and reran the affected Release tests successfully. Retry the gate with the fixed tip. The prior AreaType gate completed but stopped at the shared Release assembly lookup blocker.
