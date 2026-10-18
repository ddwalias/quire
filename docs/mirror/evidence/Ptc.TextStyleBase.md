# Ptc.TextStyleBase

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcTextStyleBaseMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Weight get` | 0x060002e5 | MemberGen: `public double Weight { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x060002ea | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x060002eb | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleKey get` | 0x060002ec | MemberGen: `public string StyleKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StyleKey set` | 0x060002ed | MemberGen: `public string StyleKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Style get` | 0x060002ee | MemberGen: `public Style Style { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Style set` | 0x060002ef | MemberGen: `public Style Style { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BasedOnKey get` | 0x060002f0 | MemberGen: `_baseOnKey; set => _baseOnKey = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BasedOnKey set` | 0x060002f1 | MemberGen: `_baseOnKey; set => _baseOnKey = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Stretch get` | 0x060002f8 | MemberGen: `public double Stretch { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontFamily get` | 0x060002f9 | MemberGen: `_fontFamily; set => Update("FontFamily", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FontFamily set` | 0x060002fa | MemberGen: `_fontFamily; set => Update("FontFamily", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Bold get` | 0x060002fb | MemberGen: `public bool Bold { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Italic get` | 0x060002fc | MemberGen: `public bool Italic { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Normal get` | 0x060002fd | MemberGen: `public bool Normal { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Oblique get` | 0x060002fe | MemberGen: `public bool Oblique { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Background get` | 0x060002ff | MemberGen: `_background; set => Update("Background", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Background set` | 0x06000300 | MemberGen: `_background; set => Update("Background", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Foreground get` | 0x06000301 | MemberGen: `_foreground; set => Update("Foreground", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Foreground set` | 0x06000302 | MemberGen: `_foreground; set => Update("Foreground", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextDecorations get` | 0x06000305 | MemberGen: `_textDecorationCollection; set => Update("TextDecorations", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextDecorations set` | 0x06000306 | MemberGen: `_textDecorationCollection; set => Update("TextDecorations", value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextEffects get` | 0x06000309 | MemberGen: `_textEffectCollection` | Mechanical; Release IL equals Mathcad's | Verified |
| `Condenst get` | 0x0600030f | MemberGen: `public bool Condenst { get; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcTextStyleBaseMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
