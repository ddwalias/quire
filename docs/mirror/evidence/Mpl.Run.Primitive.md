# Mpl.Run.Primitive

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection:** original decompilation of `Mpl.Run.Primitive.IsForeignPrimitive()`; broader scratch decompilation is `/tmp/primitive.ilspy.cs` and is not committed.
- **Quire:** `src/Quire.Mpl/Run/Primitive.cs`; differential test: `PrimitiveDifferentialTests.Primitive_kind_predicates`.

| Member | Token | Contract | Status |
|---|---|---|---|
| `Primitive` | 0x02000070 | Classifies instance records for four primitive kinds. | — |
| `m_opcodes` | 0x040000a2 | Opcode sequence stored by opcode primitives. | Stub |
| `m_code` | 0x040000a3 | Emitted code stored by code primitives. | Stub |
| `m_method` | 0x040000a4 | Reflected method stored by method primitives. | Stub |
| `m_foreign` | 0x040000a5 | String field storing the foreign primitive marker. | Stub |
| `IsOpCodePrimitive()` | 0x06000359 | True only when `m_code == null` and `m_opcodes != null`. | Verified |
| `IsCodePrimitive()` | 0x0600035a | Returns whether `m_code != null`. | Verified |
| `IsMethodPrimitive()` | 0x0600035b | True only when `m_method != null`, `m_code == null`, and `m_opcodes == null`. | Verified |
| `IsForeignPrimitive()` | 0x0600035c | Returns whether `m_foreign != null`; an empty string is still a foreign marker. | Verified |

Direct comparisons use fresh uninitialized Quire and Mathcad targets with varied `m_code`, `m_opcodes`, `m_method`, and `m_foreign` states. They cover the false and true branches of each classification predicate, including an empty foreign string. This avoids invoking the large `Primitive` type initializer, which constructs `Code` and `Instruction` entries and reaches Lane B runtime types. `GetUninitializedObject` is used only in the test harness; the methods under comparison read instance fields and do not touch static state.
