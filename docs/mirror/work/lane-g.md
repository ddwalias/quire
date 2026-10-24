# Lane G — COM Interop Assemblies (D22)

- Owner: Lane G; branch `mirror/g-interop-batch1-20261024`; worktree `C:/Projects/Quire-wt/lane-g-interop`.
- Assemblies: `Quire.AutomationPrivate` (landed), `Quire.Interop.MathcadOld`, `Quire.Interop.VSInterfacesLib`, `Quire.Interop.Mathcad`.

## Summary Status

| Project | Original Assembly | Types | Claimed / Implemented | Release IL Report |
|---|---|---:|---:|---|
| `Quire.Interop.MathcadOld` | `interop.mathcadold.dll` 1.0.0.0 | 7 | 7 / 7 (100%) | **20 / 20 exact pass (100%)** |
| `Quire.Interop.VSInterfacesLib` | `interop.vsinterfaceslib.dll` 1.0.0.0 | 90 | 90 / 90 (100%) | **212 / 212 exact pass (100%)** |
| `Quire.AutomationPrivate` | `interop.mathcadautomationprivate.dll` 1.2 | 18 | 18 / 18 (100%) | Landed on `main` (commit `9805ccbc`) |
| `Quire.Interop.Mathcad` | `interop.mathcad.dll` 1.0.0.0 | 91 | 91 / 91 (100%) | In progress (next batch) |

---

## Quire.Interop.MathcadOld (Complete, 100% exact parity)

- Original: `interop.mathcadold.dll` 1.0.0.0, SHA-256 `3e879c3f9fbdab74d9204b463a26de51a08e9b04122bf156ac6f213a40f21cf8`, MVID `0e1ddd00-717a-400b-b92f-16f3e6302f96`.
- Project: `Quire.Interop.MathcadOld` (new, C#, standalone; `net10.0`, references `Quire.Mirror`). Namespace per D22 / Rules.cs: `Quire.MathcadOld`.
- Members (all 7 types claimed and implemented):
  - 2 enums: `ERegionType`, `__MIDL___MIDL_itf_imathcad_0000_0000_0001` (5 literal fields each)
  - 3 interfaces: `IMathcadConstants` (5 property getters), `IMathcadComponent` (4 methods), `IDebuggingAuto` (5 methods)
  - 1 coclass interface: `DebuggingAuto`
  - 1 coclass: `DebuggingAutoClass` (.ctor + 5 implemented methods)
  - Total: 20 member declarations in the inventory.
- Release IL report: `release_il.py --base main --project Quire.Interop.MathcadOld` → **PASS; 20 checked; 0 failures (100% exact parity)**.
- Structural check: names, signatures, return types, marshalling (`bstr`, `struct`), DispIds (1..5), GUIDs, TypeLibType flags, and enum literal values all match original Release IL.

---

## Quire.Interop.VSInterfacesLib (Complete, 100% exact parity)

- Original: `interop.vsinterfaceslib.dll` 1.0.0.0, SHA-256 `749312578dcc94ac37208653c71eba87d8120118d345dacae51afca9cff28179`, MVID `99ac774d-c8b6-42fc-8ef5-919eed4e6ac7`.
- Project: `Quire.Interop.VSInterfacesLib` (new, C#, standalone; `net10.0`, references `Quire.Mirror`). Namespace per D22 / Rules.cs: `Quire.VSINTERFACESLib`.
- Members (all 90 types claimed and implemented):
  - 8 enums: `VSAPPLICENSE`, `VSAPPOPTS`, `VSAPPSTATE`, `VSDATATYPE`, `VSOBJMISC`, `VSOLEFLAGS`, `VSSUBOBJCHANGE`, `VSSUBOBJFLAGS`
  - 46 structs: `VSDATABOUNDS`, `tagPOINT`, `_userCLIPFORMAT`, `VSSUBOBJINFO`, `tagSIZE`, `tagRECT`, `_RemotableHandle`, `__MIDL_IWinTypes_0009`, `tagFORMATETC`, `_userSTGMEDIUM`, `_userFLAG_STGMEDIUM`, `tagDVTARGETDEVICE`, `__MIDL_IWinTypes_0001`, `_STGMEDIUM_UNION`, `__MIDL_IAdviseSink_0003`, `_userHMETAFILEPICT`, `__MIDL_IWinTypes_0005`, `_remoteMETAFILEPICT`, `_userHMETAFILE`, `__MIDL_IWinTypes_0004`, `_BYTE_BLOB`, `_userHENHMETAFILE`, `__MIDL_IWinTypes_0006`, `_GDI_OBJECT`, `__MIDL_IAdviseSink_0002`, `_userHBITMAP`, `__MIDL_IWinTypes_0007`, `_userBITMAP`, `_userHPALETTE`, `__MIDL_IWinTypes_0008`, `tagLOGPALETTE`, `tagPALETTEENTRY`, `_userHGLOBAL`, `__MIDL_IWinTypes_0003`, `_FLAGGED_BYTE_BLOB`, `_ULARGE_INTEGER`, `_FILETIME`, `_LARGE_INTEGER`, `tagSTATSTG`, `tagBIND_OPTS2`, `_COSERVERINFO`, `_COAUTHINFO`, `_COAUTHIDENTITY`, `tagSTATDATA`, `VSCMINFO`, `tagRemSNB`
  - 36 interfaces: `IVSDataType`, `IVSUnitData`, `IVSQueueInput`, `IVSQueueOutput`, `IVSObjectExec`, `IVSItemContainer`, `IVSObject`, `IDataObject`, `IVSContainer`, `IVSApp`, `IEnumFORMATETC`, `IAdviseSink`, `IEnumSTATDATA`, `IPersist`, `IPersistStream`, `IMoniker`, `ISequentialStream`, `IStream`, `IBindCtx`, `IEnumMoniker`, `IRunningObjectTable`, `IEnumString`, `IVSObject2`, `IVSObject3`, `IVSItemContainerExec`, `IVSContainer2`, `IVSObjectExec2`, `IVSBuddyObject`, `IVSPropSheet`, `IVSPropSheet2`, `IVSContextMenu`, `IVSWizard`, `IVSUCHelper`, `IStorage`, `IEnumSTATSTG`, `IVSPersistStream`
  - 0 classes (pure declaration library).
  - Total: 212 member declarations in the inventory.
- Release IL report: `release_il.py --base main --project Quire.Interop.VSInterfacesLib` → **PASS; 212 checked; 0 failures (100% exact parity)**.
- Structural check: all 90 types match original Release IL in struct packing, fields, enum values, interface signatures, GUIDs, DispIds, and marshalling.

---

## Quire.AutomationPrivate (Landed)

- Original: `interop.mathcadautomationprivate.dll` 1.2.0.0, SHA-256 `fc7d6506…188bd`, MVID `9a830ac8-1a16-4f23-935c-eede8e2d2032`.
- Project: `Quire.AutomationPrivate` (C#, standalone; `net10.0`, references `Quire.Mirror`). Namespace per the type-name rule (`MathcadAutomationPrivate.X` → `Quire.AutomationPrivate.X`).
- Members: 13 interfaces, 4 structs, coclass `SymbolicValueClass` with its constructor. 51 declarations.
- Landed on `main` in commit `9805ccbc`.
