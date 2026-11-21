# Automation.Native lane checkpoint

- Owner: Automation.Native lane; branch `feature/automation-native-agy`.
- Target: `src/Quire.Automation.Native` (C# net10.0-windows, hosting `automationmgd.dll` and `componentsmgd.dll`).

## Batch 5 — MatrixValue

- Members:
  - `Mathcad.Automation.MatrixValue`: `size` field, `.ctor()`, `.ctor(MatVal)`, `.ctor(MatVal, RuntimeOptions)`, `.ctor(double[], int, int)`, `.ctor(double[], double[], int, int)`, `.ctor(int, int)`, `Matrix`, `Size`, `IsComplex` (get/set), `Rows`, `Cols`, `IsUniform`, `IsScalarMatrix`, `IsUniformScalar`, `IsRealMatrix`, `IsRealMatrixMixed`, `Type`, `AsString`, `IsRange`, `WriteValueXml`, `WriteValueRL`, `CountElements`, `SetElement`, `GetElement`. Hand-written in `Mirrored/MatrixValue.cs`.
- Stubs removed: `Generated/Mirror/Automation.MatrixValue.g.cs` (25 stubs removed).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Automation.Native` — PASS, 186 checked (was 161 on base, +25 exact), 0 failures. Report: `.mirror-cache/release-il-c2d6d81ffc5d/release-il.json`.
- Remaining: 179 unfinished stubs reported by the gate.

## Batch 4 — ValueFactory, McxIO, McxInput, and McxOutput

- Members:
  - `Mathcad.Automation.ValueFactory`: `.ctor()`, `Instance`, `RangeToVector`, `TransposeArray`, `GetDimensions`, `FirstValue`, `VariantToEngine`, `ToAutomation(object, string)`, `ToAutomation(object, string, RuntimeOptions)`, `FromAutomation`, `EngineToAutomation(object)`, `EngineToAutomation(object, RuntimeOptions)`, `AutomationToEngine`, `VariantToAutomation`, `AutomationToVariant`, `WrapperToAutomation`, `AutomationToWrapper`, `VSDataToAutomation`, `AutomationToVSData`. Hand-written in `Mirrored/ValueFactory.cs`.
  - `Mathcad.Components.McxIO`: `.ctor(UnitAction)`, `data`, `action`, `Data` (get/set), `IsValid`, `CopyOf`. Hand-written in `Mirrored/McxIO.cs`.
  - `Mathcad.Components.McxInput`: `.ctor(UnitAction)`, `Set`, `IsReady`, `Recv`. Hand-written in `Mirrored/McxInput.cs`.
  - `Mathcad.Components.McxOutput`: `.ctor(UnitAction)`, `Set`, `Send`, `IsReady`. Hand-written in `Mirrored/McxOutput.cs`.
- Stubs removed: `Generated/Mirror/Automation.ValueFactory.g.cs`, `Generated/Mirror/Components.McxIO.g.cs`, `Generated/Mirror/Components.McxInput.g.cs`, `Generated/Mirror/Components.McxOutput.g.cs`.
- Blocked: `VSValueFactory` deferred: `VSValueFactory.VSDataToAutomation` in original `componentsmgd.dll` calls `[interop.mathcad]Mathcad.IMatrixValue::GetElement`, while `Quire.Automation.Native` maps `IMatrixValue` to `[automationmgd]Mathcad.IMatrixValue`, creating a cross-assembly interop call reference mismatch.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Automation.Native` — PASS, 161 checked (was 128 checked, +33 exact), 0 failures. Report: `.mirror-cache/release-il-7cae7307e729/release-il.json`.
- Remaining: 203 unfinished stubs reported by the gate.

## Batch 3 — NumericValue, Utilities, BitStream, Efi, UnitPower, and Interfaces

- Members:
  - `Mathcad.Automation.NumericValue`: `.ctor()`, `.ctor(NumVal)`, `.ctor(NumVal, RuntimeOptions)`, `.ctor(int)`, `.ctor(double)`, `.ctor(double, double)`, `Number`, `Size`, `Imag` (get/set), `Real` (get/set), `Integer` (get/set), `Type`, `AsString`, `FormatDoubleLegacy`, `WriteValueXml`, `WriteValueRL`, and `UnitsXML`. Hand-written in `Mirrored/NumericValue.cs`.
  - `Mathcad.Components.Utilities`: `Unwrap`, `Release`, `ReleaseAll`, `SetProperty`, `GetProperty`, `SendMessage`, `ProgIDFromCLSID`, `GetUnits`, `ScaleToSI`, `ScaleFromSI`, `CALLBACK_INFO` (.ctor, hwnd, handle), `CALLBACK_MSG` (.ctor, msg, handle). Hand-written in `Mirrored/Utilities.cs`.
  - `Mathcad.Components.BitStream`: `.ctor(IStream)`, `SerializeException`, `stream`, `ReadSome`, `WriteSome`, `WriteStringLength`, `ReadStringLength`, `ReadDouble`, `WriteDouble`, `ReadUInt`, `WriteUInt`, `BytesToString`, `StringToBytes`, `ReadString`, `WriteString`, `.cctor`. Hand-written in `Mirrored/BitStream.cs`.
  - `Mathcad.Components.Efi`: `SendMessage`, `EvalComponent`. Hand-written in `Mirrored/Efi.cs`.
  - `Mathcad.Automation.UnitPower`: `.ctor(string, Rational)`, `Name`, `Power`. Hand-written in `Mirrored/UnitPower.cs`.
  - Interface declarations: `IAutomationValue`, `IMatrixValueDataAccessor`, `IMatrixValueProxy`, `IValueProxy`, `IValueFactory`, `IOleClientSite`, `IOleObject`, `IPersistStream`. Hand-written in `Mirrored/AutomationInterfaces.cs`.
- Stubs removed: `Generated/Mirror/` matching the 13 types above.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Automation.Native` — PASS, 212 checked (was 133 on base, +79 exact), 0 failures. Report: `.mirror-cache/release-il-1b27de507e0b/release-il.json`.
- Remaining: 234 unfinished stubs reported by the gate.

## Batch 2 — Wrappers, HRESULT, and Interface declarations

- Members:
  - `Mathcad.Components.HRESULT`: `.cctor`, `S_FALSE` field. Hand-written in `Mirrored/HRESULT.cs`.
  - `Automation.Wrappers.StringValue`: `.ctor()`, `.ctor(StringValue)`, `.ctor(SerializationInfo, StreamingContext)`, `Type`, `AsString`, `Value` (get/set), `GetObjectData`, and field `_innerObject`. Hand-written in `Mirrored/Wrappers.StringValue.cs`.
  - `Automation.Wrappers.NumericValue`: `.ctor()`, `.ctor(NumericValue)`, `.ctor(SerializationInfo, StreamingContext)`, `Type`, `AsString`, `Real` (get/set), `Imag` (get/set), `Integer` (get/set), `GetObjectData`, and field `_innerObject`. Hand-written in `Mirrored/Wrappers.NumericValue.cs`.
  - `Automation.Wrappers.MatrixValue`: `.ctor()`, `.ctor(MatrixValue)`, `.ctor(SerializationInfo, StreamingContext)`, `Type`, `AsString`, `Rows`, `Cols`, `GetElement`, `SetElement`, `GetObjectData`, and field `_innerObject`. Hand-written in `Mirrored/Wrappers.MatrixValue.cs`.
  - `Automation.Wrappers.WrapperFactory`: `.ctor()`, `Instance`, `AutomationToWrapper`, `WrapperToAutomation`. Hand-written in `Mirrored/Wrappers.WrapperFactory.cs`.
  - Interface declarations (abstract without bodies): `IScalarValueProxy`, `IStringValueProxy`, `IFunctionValueProxy`, `ISymbolicValueProxy`, `IFunctionValue`, `INumericValuePrivate`, `IMatrixValuePrivate`, `IValuePrivate`, `IValue`, `IStringValue`, `INumericValue`, `IMatrixValue`, `IMathcadComponent`, `IMathcadComponent2`, `IMcxComponent`, `IPersist`, `IVSPersistStream`. Hand-written in `Mirrored/AutomationInterfaces.cs`.
- Stubs removed: `Generated/Mirror/` matching the 22 types above.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Automation.Native` — PASS, 133 checked (was 49 on base, +84 exact), 0 failures. Report: `.mirror-cache/release-il-6341a47271d8/release-il.json`.
- Remaining: 362 unfinished members reported by the gate. `Mathcad.Automation.UnitPower` is absent from `automationmgd.dll`'s type list.

## Batch 1 — Value and Proxy types

- Members:
  - `Mathcad.Automation.ScalarValueProxy`: `.ctor`, `Core`, `AsScalar`, `RealPart`, `ImagPart`, and field `m_value`. Hand-written in `Mirrored/ScalarValueProxy.cs`.
  - `Mathcad.Automation.StringValueProxy`: `.ctor(StrVal, bool, RuntimeOptions)`, `.ctor(string, bool, RuntimeOptions)`, `Core`, `AsString`, `String`, and field `m_value`. Hand-written in `Mirrored/StringValueProxy.cs`.
  - `Mathcad.Automation.FunctionValueProxy`: `.ctor(Function, bool, RuntimeOptions)`, `Core`, `AsFunction`, `FunctionInfo`, and field `m_value`. Hand-written in `Mirrored/FunctionValueProxy.cs`.
  - `Mathcad.Automation.StringValue`: `.ctor()`, `.ctor(string)`, `.ctor(string, RuntimeOptions)`, `String`, `Value` (get/set), `Type`, `AsString`, `WriteValueXml`, `WriteValueRL`, and `Escaped(string)`. Hand-written in `Mirrored/StringValue.cs`.
  - `Mathcad.Automation.FunctionValue`: `.ctor()`, `.ctor(Function, RuntimeOptions)`, `Function`, `Type`, `AsString`, `Arity`, `WriteValueXml`, and `WriteValueRL`. Hand-written in `Mirrored/FunctionValue.cs`.
- Hand-written: `src/Quire.Automation.Native/Mirrored/{ScalarValueProxy,StringValueProxy,FunctionValueProxy,StringValue,FunctionValue}.cs`.
- Stubs removed: `Generated/Mirror/Automation.{ScalarValueProxy,StringValueProxy,FunctionValueProxy,StringValue,FunctionValue}.g.cs`.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Automation.Native` — PASS, 49 checked (was 17 on base, +32 exact), 0 failures. Landed on main: `8aef933af`.
- Remaining: 445 unfinished members reported by the gate. `Mathcad.Automation.UnitPower` is absent from `automationmgd.dll`'s type list.

## Prior Batches

- Members: `Mathcad.Automation.PlainDataConversionOptions` field and constructor. Tried an idiomatic `double.NaN` field initializer; the Release IL gate matched the constructor exactly.
- Release IL reports: PASS, 10 checked (`.mirror-cache/release-il-39c04defc71a/release-il.json`); PASS, 12 checked (`.mirror-cache/release-il-e99701c32c75/release-il.json`); PASS, 13 checked (`.mirror-cache/release-il-7e4e8026efb2/release-il.json`); PASS, 17 checked (`.mirror-cache/release-il-b3da47a804e5/release-il.json`). Main-checkout landing gates passed after refreshed-base revalidation (`C:/Projects/Quire/.mirror-cache/gate-il-32fc6cf0d948/gate.json`, `gate-il-cf691e3e076f/gate.json`).
- Members: `Mathcad.Automation.SerializerCOMStreamAdaptor` constructor and `Commit`; passed the original stream to the base constructor and preserved the IL's handled `NotImplementedException` region around `base.Commit()`.
- Members: `Mathcad.Automation.MPLRunSerializationBinder` constructor and `BindToType`; matched the framework base call, assembly-name comparison, and `MatVal` assembly type lookup.
- Members: `Mathcad.Automation.SymbolicValueProxy` field, properties, and constructor; returned its field for `Core`/`Symbolic`, `this` for `AsSymbolic`, and delegated constructor arguments to the base before storing the field.
