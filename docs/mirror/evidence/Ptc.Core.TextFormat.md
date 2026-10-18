# Ptc.Core.TextFormat

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcCoreTextFormatMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Clone()` | 0x0600103a | MemberGen: `new TextFormat(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextStyleData get` | 0x06001052 | MemberGen: `public TextStyleData TextStyleData { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextStyleData set` | 0x06001053 | MemberGen: `public TextStyleData TextStyleData { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BasedOnKey get` | 0x06001056 | MemberGen: `public string BasedOnKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BasedOnKey set` | 0x06001057 | MemberGen: `public string BasedOnKey { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedFontFamily get` | 0x06001066 | MemberGen: `public bool MixedFontFamily { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedFontFamily set` | 0x06001067 | MemberGen: `public bool MixedFontFamily { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedFontWeight get` | 0x0600106a | MemberGen: `public bool MixedFontWeight { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedFontWeight set` | 0x0600106b | MemberGen: `public bool MixedFontWeight { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedFontStyle get` | 0x0600106c | MemberGen: `public bool MixedFontStyle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedFontStyle set` | 0x0600106d | MemberGen: `public bool MixedFontStyle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedTextDecorations get` | 0x0600106e | MemberGen: `public bool MixedTextDecorations { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedTextDecorations set` | 0x0600106f | MemberGen: `public bool MixedTextDecorations { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedForeground get` | 0x06001070 | MemberGen: `public bool MixedForeground { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedForeground set` | 0x06001071 | MemberGen: `public bool MixedForeground { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedBackground get` | 0x06001072 | MemberGen: `public bool MixedBackground { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedBackground set` | 0x06001073 | MemberGen: `public bool MixedBackground { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedMarkerStyle get` | 0x06001074 | MemberGen: `public bool MixedMarkerStyle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedMarkerStyle set` | 0x06001075 | MemberGen: `public bool MixedMarkerStyle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedStyle get` | 0x06001076 | MemberGen: `public bool MixedStyle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedStyle set` | 0x06001077 | MemberGen: `public bool MixedStyle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedTextAlignment get` | 0x06001078 | MemberGen: `public bool MixedTextAlignment { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MixedTextAlignment set` | 0x06001079 | MemberGen: `public bool MixedTextAlignment { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatChanged add` | 0x0600103d | MemberGen: `public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatChanged remove` | 0x0600103e | MemberGen: `public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x0600107d | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x0600107e | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCoreTextFormatMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
