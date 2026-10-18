# Ptc.PersistentDataObjects.presentationTextRendering

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000e03 | MemberGen: `proofingLanguageField = "en-US";` | Mechanical; Release IL equals Mathcad's | Verified |
| `textStyles get` | 0x06000e04 | MemberGen: `textStylesField; set => textStylesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `textStyles set` | 0x06000e05 | MemberGen: `textStylesField; set => textStylesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProofingLanguage get` | 0x06000e06 | MemberGen: `proofingLanguageField; set => proofingLanguageField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProofingLanguage set` | 0x06000e07 | MemberGen: `proofingLanguageField; set => proofingLanguageField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspresentationTextRenderingMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
