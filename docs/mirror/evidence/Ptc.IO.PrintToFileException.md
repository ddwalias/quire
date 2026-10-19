# Ptc.IO.PrintToFileException

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** ILSpy 11.1.0.9782 C# and IL from `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.IO.PrintToFileException.{cs,il}`
- **Quire:** `src/Quire.Core/IO/PrintToFileException.cs`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(string, Exception, PrintFileErrorCode, object[])` | `0x06000fb9` | Call `ApplicationException(message, innerException)`, assign `ErrorCode`, and assign `MessageArgs` to the supplied array or a new empty `object[]` when null. | Direct Mathcad constructor comparison on generated null/default and distinct inputs; result comparison includes both fields and base exception state. Release IL matches Mathcad. | Verified for recorded cases |

## Test cases

`PtcIOPrintToFileExceptionTierAMechanicalDifferentialTests.ctor_06000fb9` invokes the constructor directly on Mathcad Ptc.Core for the mechanical null/default and distinct input rounds.
