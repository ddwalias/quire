# Ptc.Controls.MruFileItem

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsMruFileItemMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600018d | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HasAccessKey get` | 0x0600019a | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |
| `MruFileType get` | 0x0600019b | MemberGen: `public MruFileTypes MruFileType { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MruFileType set` | 0x0600019c | MemberGen: `public MruFileTypes MruFileType { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `onPinChangedEvent add` | 0x0600018b | MemberGen: `public event Delegate2<MruFileItem, bool> onPinChangedEvent;` | Mechanical; Release IL equals Mathcad's | Verified |
| `onPinChangedEvent remove` | 0x0600018c | MemberGen: `public event Delegate2<MruFileItem, bool> onPinChangedEvent;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsMruFileItemMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
