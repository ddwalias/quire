# Ptc.VCard.VCard

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcVCardVCardMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Version get` | 0x0600068a | MemberGen: `public string Version { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Version set` | 0x0600068b | MemberGen: `public string Version { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormattedName get` | 0x0600068c | MemberGen: `public string FormattedName { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormattedName set` | 0x0600068d | MemberGen: `public string FormattedName { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x0600068e | MemberGen: `public Name Name { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x0600068f | MemberGen: `public Name Name { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Title get` | 0x06000690 | MemberGen: `public string Title { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Title set` | 0x06000691 | MemberGen: `public string Title { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Organization get` | 0x06000692 | MemberGen: `public string Organization { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Organization set` | 0x06000693 | MemberGen: `public string Organization { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeliveryAddress get` | 0x06000694 | MemberGen: `public Address DeliveryAddress { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeliveryAddress set` | 0x06000695 | MemberGen: `public Address DeliveryAddress { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddressLabel get` | 0x06000696 | MemberGen: `public string AddressLabel { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AddressLabel set` | 0x06000697 | MemberGen: `public string AddressLabel { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TelephoneList get` | 0x06000698 | MemberGen: `public Telephone[] TelephoneList { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TelephoneList set` | 0x06000699 | MemberGen: `public Telephone[] TelephoneList { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Email get` | 0x0600069c | MemberGen: `public string Email { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Email set` | 0x0600069d | MemberGen: `public string Email { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Url get` | 0x0600069e | MemberGen: `public string Url { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Url set` | 0x0600069f | MemberGen: `public string Url { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Photo get` | 0x060006a1 | MemberGen: `public byte[] Photo { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Photo set` | 0x060006a2 | MemberGen: `public byte[] Photo { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcVCardVCardMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
