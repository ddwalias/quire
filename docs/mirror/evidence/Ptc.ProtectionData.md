# Ptc.ProtectionData

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcProtectionDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `HasPassword get` | 0x0600024c | MemberGen: `EnumerableExtensions.IsNotEmpty<byte>(_EncryptedPassowrd)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Timestamp get` | 0x0600024d | MemberGen: `_timestamp` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShowTimestamp get` | 0x0600024e | MemberGen: `public bool ShowTimestamp { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShowTimestamp set` | 0x0600024f | MemberGen: `public bool ShowTimestamp { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProtectionLevel get` | 0x06000250 | MemberGen: `_protectionLevel` | Mechanical; Release IL equals Mathcad's | Verified |
| `EncryptedPassword get` | 0x06000255 | MemberGen: `_EncryptedPassowrd` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x06000256 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x06000257 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcProtectionDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
