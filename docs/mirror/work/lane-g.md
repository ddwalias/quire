# Lane G — COM Interop Assemblies (D22)

- Owner: Lane G; branch `mirror/g-interop-mathcad-20261107-b`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-g-interop-20261107-b`.
- Assemblies: `Quire.AutomationPrivate` (landed), `Quire.Interop.MathcadOld` (landed), `Quire.Interop.VSInterfacesLib` (landed), `Quire.Interop.Mathcad`.

## Summary Status

| Project | Original Assembly | Types | Claimed / Implemented | Status | Release IL Report |
|---|---|---:|---:|---|---|
| `Quire.Interop.MathcadOld` | `interop.mathcadold.dll` 1.0.0.0 | 7 | 7 / 7 (100%) | **Landed on `main`** (`692955d5`) | **20 / 20 exact pass (100%)** |
| `Quire.Interop.VSInterfacesLib` | `interop.vsinterfaceslib.dll` 1.0.0.0 | 90 | 90 / 90 (100%) | **Landed on `main`** (`692955d5`) | **212 / 212 exact pass (100%)** |
| `Quire.AutomationPrivate` | `interop.mathcadautomationprivate.dll` 1.2 | 18 | 18 / 18 (100%) | **Landed on `main`** (`9805ccbc`) | 51 / 51 declared |
| `Quire.Interop.Mathcad` | `interop.mathcad.dll` 1.0.0.0 | 91 | 91 / 91 (100%) | Implemented, builds cleanly | 545 checked: **459 exact pass**, 86 compiler-level / TlbImp artifacts |

---

## Quire.Interop.MathcadOld (Landed on `main`, commit `692955d5`)

- Original: `interop.mathcadold.dll` 1.0.0.0, SHA-256 `3e879c3f9fbdab74d9204b463a26de51a08e9b04122bf156ac6f213a40f21cf8`, MVID `0e1ddd00-717a-400b-b92f-16f3e6302f96`.
- Project: `Quire.Interop.MathcadOld` (standalone; `net10.0`, references `Quire.Mirror`). Namespace: `Quire.MathcadOld`.
- Members (all 7 types claimed and implemented):
  - 2 enums: `ERegionType`, `__MIDL___MIDL_itf_imathcad_0000_0000_0001` (5 literal fields each)
  - 3 interfaces: `IMathcadConstants` (5 property getters), `IMathcadComponent` (4 methods), `IDebuggingAuto` (5 methods)
  - 1 coclass interface: `DebuggingAuto`
  - 1 coclass: `DebuggingAutoClass` (.ctor + 5 implemented methods)
  - Total: 20 member declarations in the inventory.
- Release IL report: `release_il.py --base main --project Quire.Interop.MathcadOld` → **PASS; 20 checked; 0 failures (100% exact parity)**.
- Structural check: names, signatures, return types, marshalling (`bstr`, `struct`), DispIds (1..5), GUIDs, TypeLibType flags, and enum literal values all match original Release IL.

---

## Quire.Interop.VSInterfacesLib (Landed on `main`, commit `692955d5`)

- Original: `interop.vsinterfaceslib.dll` 1.0.0.0, SHA-256 `749312578dcc94ac37208653c71eba87d8120118d345dacae51afca9cff28179`, MVID `99ac774d-c8b6-42fc-8ef5-919eed4e6ac7`.
- Project: `Quire.Interop.VSInterfacesLib` (standalone; `net10.0`, references `Quire.Mirror`). Namespace: `Quire.VSINTERFACESLib`.
- Members (all 90 types claimed and implemented):
  - 8 enums: `VSAPPLICENSE`, `VSAPPOPTS`, `VSAPPSTATE`, `VSDATATYPE`, `VSOBJMISC`, `VSOLEFLAGS`, `VSSUBOBJCHANGE`, `VSSUBOBJFLAGS`
  - 46 structs: `VSDATABOUNDS`, `tagPOINT`, `_userCLIPFORMAT`, `VSSUBOBJINFO`, `tagSIZE`, `tagRECT`, `_RemotableHandle`, `__MIDL_IWinTypes_0009`, `tagFORMATETC`, `_userSTGMEDIUM`, `_userFLAG_STGMEDIUM`, `tagDVTARGETDEVICE`, `__MIDL_IWinTypes_0001`, `_STGMEDIUM_UNION`, `__MIDL_IAdviseSink_0003`, `_userHMETAFILEPICT`, `__MIDL_IWinTypes_0005`, `_remoteMETAFILEPICT`, `_userHMETAFILE`, `__MIDL_IWinTypes_0004`, `_BYTE_BLOB`, `_userHENHMETAFILE`, `__MIDL_IWinTypes_0006`, `_GDI_OBJECT`, `__MIDL_IAdviseSink_0002`, `_userHBITMAP`, `__MIDL_IWinTypes_0007`, `_userBITMAP`, `_userHPALETTE`, `__MIDL_IWinTypes_0008`, `tagLOGPALETTE`, `tagPALETTEENTRY`, `_userHGLOBAL`, `__MIDL_IWinTypes_0003`, `_FLAGGED_BYTE_BLOB`, `_ULARGE_INTEGER`, `_FILETIME`, `_LARGE_INTEGER`, `tagSTATSTG`, `tagBIND_OPTS2`, `_COSERVERINFO`, `_COAUTHINFO`, `_COAUTHIDENTITY`, `tagSTATDATA`, `VSCMINFO`, `tagRemSNB`
  - 36 interfaces: `IVSDataType`, `IVSUnitData`, `IVSQueueInput`, `IVSQueueOutput`, `IVSObjectExec`, `IVSItemContainer`, `IVSObject`, `IDataObject`, `IVSContainer`, `IVSApp`, `IEnumFORMATETC`, `IAdviseSink`, `IEnumSTATDATA`, `IPersist`, `IPersistStream`, `IMoniker`, `ISequentialStream`, `IStream`, `IBindCtx`, `IEnumMoniker`, `IRunningObjectTable`, `IEnumString`, `IVSObject2`, `IVSObject3`, `IVSItemContainerExec`, `IVSContainer2`, `IVSObjectExec2`, `IVSBuddyObject`, `IVSPropSheet`, `IVSPropSheet2`, `IVSContextMenu`, `IVSWizard`, `IVSUCHelper`, `IStorage`, `IEnumSTATSTG`, `IVSPersistStream`
  - 0 classes (pure declaration library).
  - Total: 212 member declarations in the inventory.
- Release IL report: `release_il.py --base main --project Quire.Interop.VSInterfacesLib` → **PASS; 212 checked; 0 failures (100% exact parity)**.
- Structural check: all 90 types match original Release IL in struct packing, fields, enum values, interface signatures, GUIDs, DispIds, and marshalling.

---

## Quire.Interop.Mathcad

- Original: `interop.mathcad.dll` 1.0.0.0, SHA-256 `0f33c123fada1342814298650e54b5ae088d8bb147808301815964794d9ede7f`, MVID `1577d0a7-ddd8-492e-a29c-463bfa2a3709`.
- Project: `Quire.Interop.Mathcad` (standalone; `net10.0`, references `Quire.Mirror`). Namespace: `Quire`.
- Members (all 91 types claimed and implemented, compiles cleanly with 0 errors/0 warnings):
  - 14 enums: `MCSaveOption`, `MCWorksheetOption`, `MCFileFormat`, `MCWindowState`, `MCRegionType`, `MCCustomMetadataType`, `MCAppOption`, and `__MIDL___MIDL_itf_automation_0001_0066_0001` through `0007`
  - 11 event delegates: `_IWorksheetEvents_ChangedEventHandler`, `_IWorksheetEvents_RecalculatedEventHandler`, `_IApplicationEvents_QuitEventHandler`, `_IApplicationEvents_WorksheetClosingEventHandler`, `_IApplicationEvents_WorksheetOpenedEventHandler`, `_IApplicationEvents_WindowActivatedEventHandler`, `_IApplicationEvents_WindowDeactivatedEventHandler`, `_IApplicationEvents_WorksheetSavingEventHandler`, `_IApplicationEvents_WorksheetSavedEventHandler`, `_IApplicationEvents_WorksheetClosedEventHandler`, `_IApplicationEvents_IdleEventHandler`
  - 32 interfaces & CoClass interfaces: `IValue`, `Value`, `INumericValue`, `NumericValue`, `IStringValue`, `StringValue`, `IMatrixValue`, `MatrixValue`, `IMathcadWorksheet`, `IMathcadWorksheet2`, `Worksheet`, `_IWorksheetEvents`, `_IWorksheetEvents_Event`, `IMathcadApplication`, `IMathcadApplication2`, `Application`, `_IApplicationEvents`, `_IApplicationEvents_Event`, `IMathcadWorksheets`, `IMathcadWorksheets2`, `Worksheets`, `IMathcadWindows`, `Windows`, `IMathcadWindow`, `Window`, `IMathcadRegions`, `Regions`, `IMathcadRegion`, `IMathcadRegion2`, `IMathcadRegion3`, `Region`, `IMathInterface`, `IMathInterface2`, `MathInterface`, `IMathcad`, `MathcadOld`, `ICustomMetadataItem`, `CustomMetadataItem`, `ICustomMetadataCollection`, `CustomMetadataCollection`, `IMetadata`, `IWorksheetMetadata`, `WorksheetMetadata`, `IRegionMetadata`, `RegionMetadata`
  - 17 coclasses: `ValueClass`, `NumericValueClass`, `StringValueClass`, `MatrixValueClass`, `WorksheetClass`, `ApplicationClass`, `WorksheetsClass`, `WindowsClass`, `WindowClass`, `RegionsClass`, `RegionClass`, `MathInterfaceClass`, `MathcadOldClass`, `CustomMetadataItemClass`, `CustomMetadataCollectionClass`, `WorksheetMetadataClass`, `RegionMetadataClass`
  - 4 event provider / sink helper classes: `_IWorksheetEvents_SinkHelper`, `_IWorksheetEvents_EventProvider`, `_IApplicationEvents_SinkHelper`, `_IApplicationEvents_EventProvider`
  - Total: 545 member declarations in the inventory.
- Release IL report: `release_il.py --base main --project Quire.Interop.Mathcad` (`.mirror-cache/release-il-b84db925053c/release-il.json`) → 545 checked.
  - **459 passed exact** under `declaration_only: true`.
  - Fixable mismatches (a): **0** (all 91 types and their member declarations match the original COM interfaces and coclasses).
  - Genuine compiler / TlbImp artifacts (b): **86 differences** across 25 types:
    1. 10 coclass constructors (`CustomMetadataCollectionClass`, `MathInterfaceClass`, `RegionClass`, `RegionMetadataClass`, `RegionsClass`, `ValueClass`, `WindowClass`, `WindowsClass`, `WorksheetMetadataClass`, `WorksheetsClass`):
       - Mathcad IL: `.method assembly specialname rtspecialname instance void .ctor () runtime managed internalcall` (internal visibility for non-creatable COM coclasses).
       - Roslyn limitation: CS0669 ("A class with the ComImport attribute cannot have a user-defined constructor") prevents declaring explicit constructors. Roslyn synthesizes a parameterless constructor with `runtime managed internalcall` but always emits `public` visibility.
    2. 33 delegate members (11 delegates: `_IWorksheetEvents_ChangedEventHandler`, `_IWorksheetEvents_RecalculatedEventHandler`, 9 `_IApplicationEvents_*EventHandler`):
       - Mathcad IL: TlbImp emits `.ctor(object, native uint)` (`UIntPtr`) and `Invoke(...)`. It emits NO `BeginInvoke` or `EndInvoke`.
       - Roslyn limitation: Roslyn synthesizes `.ctor(object, native int)` (`IntPtr`), `Invoke(...)`, `BeginInvoke(...)`, and `EndInvoke(...)`. In C#, `MulticastDelegate` cannot be inherited explicitly (CS0644), so these members cannot be suppressed or altered. The 11 `Invoke` methods match exact (`declaration_only: true`); the other 33 members are compiler artifacts.
    3. 43 methods in `_EventProvider` and `_SinkHelper`:
       - 17 methods with unreadable IL local signatures in Mathcad binary: 10 in `_IApplicationEvents_SinkHelper`, 3 in `_IWorksheetEvents_SinkHelper`, 2 in `_IApplicationEvents_EventProvider` (`.ctor`, `Dispose`), and 2 in `_IWorksheetEvents_EventProvider` (`.ctor`, `Dispose`). TlbImp emitted empty `.locals init ()` StandAloneSig table entries (0 type elements), which violates ECMA-335 §II.23.2.12. MethodAudit / System.Reflection.Metadata / ILSpy reports `IL unreadable: Signature type sequence must have at least one element.` on the Mathcad binary itself.
       - 26 methods in `_EventProvider` (`_IApplicationEvents_EventProvider`, `_IWorksheetEvents_EventProvider`):
         * 11 `add_*` methods: TlbImp emits redundant `castclass System.Object` instructions before `IConnectionPoint.Advise` and `ArrayList.Add`. Roslyn treats reference-to-object conversion as an identity conversion and emits no `castclass`.
         * 11 `remove_*` methods: TlbImp emits `castclass System.Object` and calls `Delegate.Equals(object)`. Roslyn resolves delegate virtual dispatch to `System.Object.Equals(object)`.
         * 2 `Init` methods: TlbImp emits manual 16-element byte array initialization (`newarr`, 16 `stelem.i1`) and redundant `castclass IConnectionPoint` on a variable already typed as `IConnectionPoint`. Roslyn emits `RuntimeHelpers.InitializeArray`.
         * 2 `Finalize` methods: TlbImp passes uninitialized local to `Monitor.Enter(object, ref bool)`. Roslyn requires definite assignment for `ref` parameters (`bool lockTaken = default;`), emitting `ldc.i4.0 / stloc`.
- Structural check: all 91 types match original Release IL in signatures, GUIDs, DispIds, marshalling, enum literal values, and metadata.

---

## Quire.AutomationPrivate (Landed on `main`, commit `9805ccbc`)

- Original: `interop.mathcadautomationprivate.dll` 1.2.0.0, SHA-256 `fc7d6506…188bd`, MVID `9a830ac8-1a16-4f23-935c-eede8e2d2032`.
- Project: `Quire.AutomationPrivate` (standalone; `net10.0`, references `Quire.Mirror`). Namespace: `Quire.AutomationPrivate`.
- Members: 13 interfaces, 4 structs, coclass `SymbolicValueClass` with its constructor. 51 declarations.
- Landed on `main` in commit `9805ccbc`.
