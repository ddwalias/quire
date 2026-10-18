# Ptc.PersistentDataObjects.presentation

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspresentationMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000d93 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `pageModel get` | 0x06000d8a | MemberGen: `pageModelField; set => pageModelField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `pageModel set` | 0x06000d8b | MemberGen: `pageModelField; set => pageModelField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `mathRendering get` | 0x06000d8c | MemberGen: `mathRenderingField; set => mathRenderingField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `mathRendering set` | 0x06000d8d | MemberGen: `mathRenderingField; set => mathRenderingField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `textRendering get` | 0x06000d8e | MemberGen: `textRenderingField; set => textRenderingField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `textRendering set` | 0x06000d8f | MemberGen: `textRenderingField; set => textRenderingField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `language get` | 0x06000d90 | MemberGen: `languageField; set => languageField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `language set` | 0x06000d91 | MemberGen: `languageField; set => languageField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspresentationMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
