# Ptc.Encryptor

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcEncryptorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Encrypt(string)` | 0x060001c4 | MemberGen: `Encrypt(_cryptoTransformer, data)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConnected get` | 0x060001cb | MemberGen: `public bool IsConnected { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConnected set` | 0x060001cc | MemberGen: `public bool IsConnected { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcEncryptorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
