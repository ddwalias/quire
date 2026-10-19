# Ptc.StringExtension

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** Tier B bodies from MemberGen (tools/Quire.Tools.MemberGen); Tier A bodies from ilspycmd 11.1.0.9782, `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.StringExtension.cs` and `.il`
- **Quire:** `src/Quire.Core`; Tier B tests: `Quire.Differential.Tests.PtcStringExtensionMechanicalDifferentialTests`; Tier A tests: `Quire.Differential.Tests.PtcStringExtensionTierAMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor()` | 0x0600027b | Initialize `_invalidXmlChars`, `_validXmlString` and `AsciiEndCode = 127` | Decompiled C# corroborated by IL; direct comparison also checks the initialized fields | Verified |
| `IsWrappedWithQuotes(string, QuotesType)` | 0x06000277 | Apply the quote-pair rules selected by `QuotesType` | Decompiled C# corroborated by IL; direct cases cover straight, matching Unicode, mixed, short and invalid-enum inputs | Verified |
| `IsStraightQuoteChar(char)` | 0x06000278 | True only for `"` | Decompiled C# and exact Release IL match | Verified |
| `IsAnyQuoteChar(char)` | 0x06000279 | True for `"` or a Unicode initial/final quote punctuation character | Decompiled C# and exact Release IL match | Verified |
| `IsOpenCloseUnicodeQuoteChar(char)` | 0x0600027a | True for Unicode initial or final quote punctuation | Decompiled C# and exact Release IL match | Verified |
| `StripInvalidXmlChars(string)` | 0x06000273 | MemberGen: `_invalidXmlChars.Replace(str, "")` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsValidXmlString(string)` | 0x06000274 | MemberGen: `_validXmlString.IsMatch(str)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcStringExtensionMechanicalDifferentialTests`): Tier B methods are directly compared on generated inputs. **Observed** (`PtcStringExtensionTierAMechanicalDifferentialTests`): the Tier A initializer and four quote methods are directly compared against the recorded artifact; the test also checks both initialized regex patterns and `AsciiEndCode`.

`ContainsUnicodeCharOrSlash` and its compiler-generated predicate are Tier C and remain outside this batch.
