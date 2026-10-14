# Mpl.Run.Cons

- Original mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`; same-artifact ILSpy output retained outside repository.

Cons stores two object references. head returns the first; tail casts second to Cons (malformed tails become null). Length follows tail until null. Formatting recursively prints comma-space separated items in brackets; null heads/malformed non-null tails throw NullReferenceException. destructive_append returns second for null first; otherwise mutates last non-null tail to second and returns original first. Malformed tails throw; cycles are unbounded.

| Exact member (REA decoded signature) | Token | Normalized IL prefix | Status |
|---|---|---|---|
| `Mpl.Run.Cons..ctor(object, object)` | 0x060003eb | ce7bf576c93074a0 | Analyzed |
| `Mpl.Run.Cons.get_head()` | 0x060003ec | 62cc26295c5dd839 | Analyzed |
| `Mpl.Run.Cons.get_tail()` | 0x060003ed | 85e073b76f43747b | Analyzed |
| `Mpl.Run.Cons.Length(class:476)` | 0x060003ee | 1ad36226ce4e77b1 | Analyzed |
| `Mpl.Run.Cons.FormatItems()` | 0x060003ef | b657978ac6bb114d | Analyzed |
| `Mpl.Run.Cons.ToString()` | 0x060003f0 | 22af0120fc99fa57 | Analyzed |
| `Mpl.Run.Cons.destructive_append(class:476, class:476)` | 0x060003f1 | cca6cb551a8bd7c2 | Analyzed |

Confidence: static analysis only; tests await primary validation. No claim for wider Function/FunctionUtilities/Fn_clo members.

Fields (Analyzed):
- `Mpl.Run.Cons.Field1`, token `0x040000ba`: object.
- `Mpl.Run.Cons.Field2`, token `0x040000bb`: object.
