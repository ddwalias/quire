# Mpl.Run.Tuple

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Tuple.cs`; tests: `Quire.Mpl.Tests.Run.MplRunHelpersTests`, `Quire.Differential.Tests.MplRunHelpersDifferentialTests`

How the runtime represents tuples, and the generic classes `Tuple`2` … `Tuple`5` (public fields `m_1` … `m_n`, default constructors, same normalized IL `41ad57d1`). Quire's `typeof(Tuple).GetMethod(...)` and `tup_types` refer to Quire's own classes, the counterparts of Mathcad's. The nested enum `Rep` (Singleton 0, ObjVector 1, HeteroVector 2, TupleType 3) stays generated: MirrorGen re-emits nested types even when written by hand.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `tup_types` | 0x040000d2 | — | Private static: null, null, then the generic classes of arity 2–5. | High | Verified |
| `gf, sf` | 0x040000d3, 0x040000d4 | — | Private static `GetField` and `SetField` method definitions. | High | Verified |
| `.ctor()` | 0x0600042a | `41ad57d1` | No state. | High | Verified |
| `tuple_length(object)` | 0x0600041f | `aca948a1` | 0 for null, the length of an `object[]`, else 1 (other arrays included). | High | Verified |
| `IType(Type[])` | 0x06000421 | `c353de18` | Private: `tup_types[n].MakeGenericType(tys)`; fewer than 2 types throw `NullReferenceException`, more than 5 `IndexOutOfRangeException`. | High | Verified |
| `AllObjects(Type[])` | 0x06000422 | `6c18fae9` | No value type among them (true for none). | High | Verified |
| `TupleType(Type[])` | 0x06000423 | `fe376473` | null for 6 or more types, else `IType`. | High | Verified |
| `TupleRep(out Rep, Type[])` | 0x06000424 | `90076d79` | One type: itself, `Singleton`. All reference types (none included): `object[]`, `ObjVector`. Up to 5 with a value type: the generic class, `TupleType`. More: `object[]`, `HeteroVector`. | High | Verified |
| `GetFieldInfo(Type, int)` | 0x06000425 | `d39af0a2` | null type gives null, else `GetField("m_" + n)` (public instance; null when absent). | High | Verified |
| `Getter(Type)` | 0x06000426 | `7eab34fc` | `GetField<T>` closed over the type. | High | Verified |
| `Setter(Type)` | 0x06000427 | `7eab34fc` | `SetField<T>` closed over the type. | High | Verified |
| `GetField<T>(object[], int)` | 0x06000428 | `6b1d14d8` | Unboxing cast of the slot (throws on mismatch, null for a value type). | High | Verified |
| `SetField<T>(object[], int, T)` | 0x06000429 | `f879e602` | Stores into the slot. | High | Verified |
| `Tuple`2…`5 m_1 … m_5` | 0x040000c4–0x040000d1 | — | Public mutable fields. | High | Verified |
| `Tuple`2 .ctor()` | 0x0600041b | `41ad57d1` | No state. | High | Verified |
| `Tuple`3 .ctor()` | 0x0600041c | `41ad57d1` | No state. | High | Verified |
| `Tuple`4 .ctor()` | 0x0600041d | `41ad57d1` | No state. | High | Verified |
| `Tuple`5 .ctor()` | 0x0600041e | `41ad57d1` | No state. | High | Verified |

## Test cases

**Observed** (`MplRunHelpersDifferentialTests.Tuple_members`, `Generic_tuples`): 12 type lists (empty to 7, value and reference types, nullable), `GetFieldInfo` on 0–3, `Getter`/`Setter` invoked on three element types and two arrays, construction and field writes of each generic class. Types are compared by mplrun name and generic arguments.
**Inferred** (`MplRunHelpersTests`).
