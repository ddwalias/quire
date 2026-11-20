# Automation.Native lane checkpoint

- Owner: Automation.Native lane; branch `feature/automation-native-agy`.
- Target: `src/Quire.Automation.Native` (C# net10.0-windows, hosting `automationmgd.dll` and `componentsmgd.dll`).

## Batch 1 — Value and Proxy types

- Members:
  - `Mathcad.Automation.ScalarValueProxy`: `.ctor`, `Core`, `AsScalar`, `RealPart`, `ImagPart`, and field `m_value`. Hand-written in `Mirrored/ScalarValueProxy.cs`.
  - `Mathcad.Automation.StringValueProxy`: `.ctor(StrVal, bool, RuntimeOptions)`, `.ctor(string, bool, RuntimeOptions)`, `Core`, `AsString`, `String`, and field `m_value`. Hand-written in `Mirrored/StringValueProxy.cs`.
  - `Mathcad.Automation.FunctionValueProxy`: `.ctor(Function, bool, RuntimeOptions)`, `Core`, `AsFunction`, `FunctionInfo`, and field `m_value`. Hand-written in `Mirrored/FunctionValueProxy.cs`.
  - `Mathcad.Automation.StringValue`: `.ctor()`, `.ctor(string)`, `.ctor(string, RuntimeOptions)`, `String`, `Value` (get/set), `Type`, `AsString`, `WriteValueXml`, `WriteValueRL`, and `Escaped(string)`. Hand-written in `Mirrored/StringValue.cs`.
  - `Mathcad.Automation.FunctionValue`: `.ctor()`, `.ctor(Function, RuntimeOptions)`, `Function`, `Type`, `AsString`, `Arity`, `WriteValueXml`, and `WriteValueRL`. Hand-written in `Mirrored/FunctionValue.cs`.
- Hand-written: `src/Quire.Automation.Native/Mirrored/{ScalarValueProxy,StringValueProxy,FunctionValueProxy,StringValue,FunctionValue}.cs`.
- Stubs removed: `Generated/Mirror/Automation.{ScalarValueProxy,StringValueProxy,FunctionValueProxy,StringValue,FunctionValue}.g.cs`.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Automation.Native` — PASS, 49 checked (was 17 on base, +32 exact), 0 failures. Report: `.mirror-cache/release-il-5d20270f7f31/release-il.json`.
- Remaining: 445 unfinished members reported by the gate. `Mathcad.Automation.UnitPower` is absent from `automationmgd.dll`'s type list.

## Prior Batches

- Members: `Mathcad.Automation.PlainDataConversionOptions` field and constructor. Tried an idiomatic `double.NaN` field initializer; the Release IL gate matched the constructor exactly.
- Release IL reports: PASS, 10 checked (`.mirror-cache/release-il-39c04defc71a/release-il.json`); PASS, 12 checked (`.mirror-cache/release-il-e99701c32c75/release-il.json`); PASS, 13 checked (`.mirror-cache/release-il-7e4e8026efb2/release-il.json`); PASS, 17 checked (`.mirror-cache/release-il-b3da47a804e5/release-il.json`). Main-checkout landing gates passed after refreshed-base revalidation (`C:/Projects/Quire/.mirror-cache/gate-il-32fc6cf0d948/gate.json`, `gate-il-cf691e3e076f/gate.json`).
- Members: `Mathcad.Automation.SerializerCOMStreamAdaptor` constructor and `Commit`; passed the original stream to the base constructor and preserved the IL's handled `NotImplementedException` region around `base.Commit()`.
- Members: `Mathcad.Automation.MPLRunSerializationBinder` constructor and `BindToType`; matched the framework base call, assembly-name comparison, and `MatVal` assembly type lookup.
- Members: `Mathcad.Automation.SymbolicValueProxy` field, properties, and constructor; returned its field for `Core`/`Symbolic`, `this` for `AsSymbolic`, and delegated constructor arguments to the base before storing the field.
