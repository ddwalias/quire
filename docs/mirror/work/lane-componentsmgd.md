# Componentsmgd lane checkpoint

## Batch 2 — McxComponent accessors (2026-11-25)

- Owner: AGY; branch `feature/componentsmgd-yield-2`.
- Exact members: `McxComponent.ThreadsPerDocument`, `Origin`, `CWD`, `Executive`, `LastError`, and `ShouldScale`; the `PROGIDS`/`MultiThreaded` initialization also pairs exactly.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Componentsmgd` PASS; 90 checked, 0 failures (`.mirror-cache/release-il-1f7f755e2e5d/release-il.json`).
- Remaining: 19 stubs (10 `McxComponent`, 8 `VSDataValue`, 1 `VSValueFactory`). The 9 `VSDataValue`/`VSValueFactory` methods use `Mathcad.IMatrixValue` across the `interop.mathcad` boundary noted below; the 10 `McxComponent` methods remain to triage.

- Owner: Componentsmgd lane; branch `feature/componentsmgd-agy`.
- Target: `src/Quire.Componentsmgd` (C# net10.0-windows, original assembly `componentsmgd.dll`).

## Batch 1 — Project Split & Feasible Members

- Project Split (D20 / D22):
  - Created `src/Quire.Componentsmgd/Quire.Componentsmgd.csproj` (`net10.0-windows`, `UseWindowsForms=true`, `CompileUsingReferenceAssemblies=false`).
  - Created `src/Quire.Componentsmgd/Generated/Mirror.props`.
  - Moved all 18 `componentsmgd`-owned types out of `Quire.Automation.Native` into `Quire.Componentsmgd`:
    - Hand-written implementations: `BitStream.cs`, `Efi.cs`, `HRESULT.cs`, `McxIO.cs`, `McxInput.cs`, `McxOutput.cs`, `Utilities.cs`.
    - Component interfaces moved to `Mirrored/ComponentInterfaces.cs`: `IMcxComponent`, `IPersist`, `IVSPersistStream`, `IOleClientSite`, `IOleObject`, `IPersistStream`.
    - Moved generated `.g.cs` files: `Components.{BitStream,Efi,HRESULT,IMcxComponent,IOleClientSite,IOleObject,IPersist,IPersistStream,IVSPersistStream,McxComponent,McxIO,McxInput,McxOutput,Utilities,VSDataValue,VSValueFactory}.g.cs`.
  - Cleaned up `Quire.Automation.Native` so that it contains only `automationmgd.dll` types, leaving all 320 members 100% exact.
- Feasible Members Implemented:
  - `Mathcad.Components.VSValueFactory`: `Instance`, `AutomationToVSData`, `.ctor`.
  - `Mathcad.Components.VSDataValue`: `MatrixMarker`, `handleR`, `handleI`, `IsScalar`, `DataType`, `Dimensions`, `Bounds`, `IsDimensionless`, `Clsid`, `IsDirty`, `MaxSize`, `InitNew`, `Clone`, `Lock`, `LockI`, `Unlock`, `UnlockI`, `IsString`, `IsMatrix`, `IsObject`, `GetStringValue`, `SetStringValue`, `SetUnitSystem`, `GetUnitSystem`, `GetNumBaseUnits`, `SetUnitValue`, `GetUnitValue`, `GetUnitName`, `SetPowers`, `GetPowers`, `.ctor(MatVal)`, `.ctor()`.
- Release IL Report:
  - `python tools/validation/release_il.py --base main --project Quire.Automation.Native --project Quire.Componentsmgd`
  - Result: `Release IL: PASS; 354 checked; 0 failures` (320 exact in `Quire.Automation.Native`, 34 exact in `Quire.Componentsmgd`).
- Remaining / Concrete Blocker:
  - Remaining: 27 stubs (8 in `VSDataValue`, 1 in `VSValueFactory`, 18 in `McxComponent`).
  - Blocker: Cross-assembly COM interop mapping for `[interop.mathcad]Mathcad.IMatrixValue`.
    - In original Mathcad Prime, `componentsmgd.dll` directly references `interop.mathcad.dll` for `Mathcad.IMatrixValue`.
    - `Quire.Automation.Native` defines `Quire.IMatrixValue` with `[MirrorOf("Mathcad.IMatrixValue, automationmgd")]` without `EmbedInteropTypes=true` (D22).
    - `MethodAudit` currently does not map external referenced generated assemblies (`Quire.Interop.Mathcad`) unless audited directly, causing calls to emit `[quire.interop.mathcad]` instead of `[interop.mathcad]`, or `[automationmgd]` instead of `[interop.mathcad]`.
    - Cross-assembly integration changes are escalated to the maintainer.
