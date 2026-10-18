# Ptc.DebugTools

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcDebugToolsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `FailAssertion(string, object[])` | 0x06000136 | MemberGen: `FailAssertion(1, AssertionModifiers.None, message, messageArgs)` | Mechanical; Release IL equals Mathcad's | Verified |
| `FailAssertionDoNotThrow(string, object[])` | 0x06000137 | MemberGen: `FailAssertion(1, AssertionModifiers.DoNotThrow, message, messageArgs)` | Mechanical; Release IL equals Mathcad's | Verified |
| `DevAssert(bool, string, object[])` | 0x06000139 | MemberGen: `DevAssert(condition, 1, AssertionModifiers.None, message, messageArgs)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ReportAssertionFailureOnAnotherThread(string)` | 0x0600013b | MemberGen: `_assertDelegate.Invoke(false, formattedMessageWithStack)` | Mechanical; Release IL equals Mathcad's | Verified |
| `LogError(string)` | 0x0600013e | MemberGen: `Trace.TraceWarning(message)` | Mechanical; Release IL equals Mathcad's | Verified |
| `LogMessage(string)` | 0x0600013f | MemberGen: `LogError(message)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ReplaceAssertionWithException get` | 0x06000131 | MemberGen: `public static bool ReplaceAssertionWithException { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ReplaceAssertionWithException set` | 0x06000132 | MemberGen: `public static bool ReplaceAssertionWithException { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsDebug get` | 0x06000140 | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `DebugVerificationsAllowed get` | 0x06000141 | MemberGen: `public static bool DebugVerificationsAllowed { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DebugVerificationsAllowed set` | 0x06000142 | MemberGen: `public static bool DebugVerificationsAllowed { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcDebugToolsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
