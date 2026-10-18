# Ptc.TextStyleChangedHandler

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcTextStyleChangedHandlerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000360 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleChanged add` | 0x0600035a | MemberGen: `public event Delegate1<ICollection<TextStyleData>> StyleChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleChanged remove` | 0x0600035b | MemberGen: `public event Delegate1<ICollection<TextStyleData>> StyleChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleCreateRequest add` | 0x0600035c | MemberGen: `public event Delegate1<TextFormat> StyleCreateRequest;` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleCreateRequest remove` | 0x0600035d | MemberGen: `public event Delegate1<TextFormat> StyleCreateRequest;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcTextStyleChangedHandlerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
