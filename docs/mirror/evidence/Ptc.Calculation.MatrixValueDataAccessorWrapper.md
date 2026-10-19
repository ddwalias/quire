# Ptc.Calculation.MatrixValueDataAccessorWrapper

- **Original:** `Ptc.Calculation.dll` 12.0.1.0, SHA-256 `fe8cf4f1b2791b7a223d203785600667bb00a3e2b30e975a949fe412cbf07e4b`, MVID `82391b36-0d84-45c3-b21a-f7dccf288c68`
- **Reference:** ILSpy C# and IL from `python tools\reference\reference.py --assembly Ptc.Calculation Ptc.Calculation.MatrixValueDataAccessorWrapper --il`
- **Quire:** `src/Quire.Calculation`; direct tests: `Quire.Differential.Tests.PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `AccessorProxy get` | 0x06000078 | Returns the auto-property backing field | Direct Mathcad comparison on constructed wrappers, including replacement through the setter | Verified |
| `AccessorProxy set` | 0x06000079 | Stores the accessor argument in the auto-property backing field | Direct Mathcad comparison verifies the assigned proxy through target state | Verified |
| `.ctor(IMatrixValueDataAccessor)` | 0x0600007a | Stores the supplied accessor through `AccessorProxy` | Direct Mathcad constructor comparison with matching test proxy inputs | Verified |
| `GetColumnsPlainDataValues(int, int)` | 0x0600007e | Forwards both bounds to `AccessorProxy.GetColumnsPlainDataValues` and returns the result | Direct Mathcad comparison with rows and bounds supplied by an automation-interface test proxy | Verified |
| `GetFirstNonPlainDataColumnIndex()` | 0x0600007f | Returns `AccessorProxy.GetFirstNonPlainDataColumnIndex()` | Direct Mathcad comparison with a test proxy | Verified |
| `GetFirstNonPlainDataRowIndex()` | 0x06000080 | Returns `AccessorProxy.GetFirstNonPlainDataRowIndex()` | Direct Mathcad comparison with a test proxy | Verified |
| `GetRowsPlainDataValues(int, int)` | 0x06000082 | Forwards both bounds to `AccessorProxy.GetRowsPlainDataValues` and returns the result | Direct Mathcad comparison with rows and bounds supplied by an automation-interface test proxy | Verified |
| `GetEntireMatrixDataTypes()` | 0x0600007b | Aggregates the accessor's column flags and parses the result as Core `PlainDataTypes` | Calls `Ptc.Core.EnumHelper.TryParse`, outside Lane C's assigned assemblies | Blocked |
| `GetColumnsPlainDataTypes()` | 0x0600007c | Converts the accessor's column flags via `ConvertPlainDataTypeList` | Its helper calls `Ptc.Core.EnumHelper.TryParse`, outside Lane C's assigned assemblies | Blocked |
| `ConvertPlainDataTypeList(IList<Mathcad.Automation.PlainDataTypes>)` | 0x0600007d | Converts each value through `EnumHelper.TryParse`; returns null on the first parse failure | Calls `Ptc.Core.EnumHelper.TryParse`, outside Lane C's assigned assemblies | Blocked |
| `GetRowsPlainDataTypes()` | 0x06000081 | Converts the accessor's row flags via `ConvertPlainDataTypeList` | Its helper calls `Ptc.Core.EnumHelper.TryParse`, outside Lane C's assigned assemblies | Blocked |

**Observed** (`PtcCalculationMatrixValueDataAccessorWrapper_MechanicalDifferentialTests`): constructor, property getter/setter, column/row data forwarding and both first non-plain indices are compared directly to Mathcad with interface test proxies. The proxy supplies only automation interface responses; product code does not implement automation or `Ptc.Core` members. The flag conversion methods stay as generated stubs until their out-of-scope `EnumHelper.TryParse` dependency is resolved by its owning lane.
