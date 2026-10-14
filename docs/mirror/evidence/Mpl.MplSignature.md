# Mpl.MplSignature

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/MplSignature.cs`; tests: `Quire.Mpl.Tests.Run.MplRunHelpersTests`, `Quire.Differential.Tests.MplRunHelpersDifferentialTests`

A return type and parameter types, used as a key.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `return_type, parameter_types` | 0x04000001, 0x04000002 | — | Private. | High | Verified |
| `.ctor(Type, Type[])` | 0x06000004 | `b4a398e8` | Stores both; the array isn't copied. | High | Verified |
| `RetType, ArgTypes` | 0x06000001, 0x06000002 | `1c391ab2`, `5a889236` | The fields. | High | Verified |
| `GetHashCode()` | 0x06000003 | `640df087` | `(hash(return) + Σ hash(parameter)) · 64 + count`, wrapping; null types throw. | High | Verified |
| `Equals(object)` | 0x06000005 | `4ac0348b` | False for null or another runtime class. Then this signature's parameter types, position by position (the other's extras ignored, fewer throw `IndexOutOfRangeException`), then the return type. Null types in this signature throw. | High | Verified |

## Test cases

**Observed** (`MplRunHelpersDifferentialTests.MplSignature_members`): 10 signatures (incl. null return, null array, null element) pairwise and against copies, null and a string; hashes compared in the same process.
**Inferred** (`MplRunHelpersTests`).
