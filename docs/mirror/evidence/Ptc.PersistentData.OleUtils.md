# Ptc.PersistentData.OleUtils

- **Original:** `Ptc.Core` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`.
- **Quire:** `src/Quire.Core`; direct tests: `PtcPersistentDataOleUtilsDifferentialTests.OleUtils_methods_match_PtcCore`.
- **Reference:** `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.PersistentData.OleUtils.cs` and `.il`, generated with ilspycmd 11.1.0.9782. Cache artifacts are local and ignored.

The method bodies follow the decompiled control flow and IL. The COM registry lookups and `ole32.dll` call are machine-dependent; the differential cases compare original and Quire in the same Windows process and do not assume Excel or any other OLE server is installed. Tested both an arbitrary class ID and the Excel 8 class ID, so `IsVersionInstalled` exercises the registered and missing-registration outcome according to the current machine. ProgIDs include known, unknown, empty, and null inputs. These cases establish equivalence only for the installed Mathcad artifact and this machine's COM registry.

The requested methods depend on static GUID values, and Tier A also marks `OleUtils::.cctor()` as unmapped. The batch therefore includes that initializer plus its Tier B static GUID and conversion-map field data. The test directly invokes the original and Quire type initializers, then compares every initialized GUID and the conversion dictionary.

| Member | Token | Signature | Evidence/status |
|---|---:|---|---|
| `ProgIDFromCLSID` | `0x060008f4` | `private static string(ref Guid)` | Directly compared for arbitrary and Excel 8 class IDs; COM registration outcome is machine-dependent. Verified for those cases. |
| `IsMicrosoftEquationGuid` | `0x060008f5` | `public static bool(Guid)` | Compared for empty, unknown, and each of the three declared equation IDs. Verified for those cases. |
| `GetExcelSheetGuid` | `0x060008f6` | `public static Guid(string)` | Compared for Excel ProgIDs, unknown ProgID, empty, and null. Registration outcome is machine-dependent. Verified for those cases. |
| `IsSupportedExcelGuid` | `0x060008f7` | `public static bool(Guid)` | Compared for empty, unknown, and all four declared Excel IDs. Verified for those cases. |
| `CanCreateExcelContainer` | `0x060008f8` | `public static bool()` | Compared against the local `Excel.Sheet` registration. Verified for the current machine. |
| `get_IsExcel8Installed` | `0x060008f9` | `public static bool()` | Compared against the local `Excel.Sheet` registration. Verified for the current machine. |
| `IsOleAppSupportZoom` | `0x060008fa` | `public static bool(string)` | Compared across known prefixes, versioned ProgIDs, prefix substring collision, malformed input, empty, and null. Verified for those cases. |
| `IsVersionInstalled` | `0x060008fb` | `public static bool?(object)` | Compared for non-`IPersist`, arbitrary and Excel 8 `IPersist` IDs; the COM registration result is machine-dependent. Verified for those cases. |
| `.cctor` | `0x060008fc` | `private static void()` | Directly invoked and compared; all initialized GUIDs and the chart conversion dictionary are then compared. Verified for the initialized state. |

The implementation deliberately retains the original list construction and membership calls, explicit null check and return structure, and `IsVersionInstalled` exception boundary. `IsOleAppSupportZoom` uses the literal embedded in the original method body. Remaining constants, methods, and members stay outside this batch.
