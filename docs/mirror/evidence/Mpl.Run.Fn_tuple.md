# Mpl.Run.Fn_tuple

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/Fn_tuple.cs`; tests: `Quire.Differential.Tests.FnTupleDifferentialTests`.

`Fn_tuple` represents a tuple-building function that prepends its stored elements `m_args` to any argument vector passed to `invVec<R>`. Constructed with purity and minimum arity equal to `ptup.Length`. `invVec<R>` evaluates `FunctionUtilities.concat(m_args, args)` and unboxes the concatenated array into `out R r`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(object[], int)` | 0x06000261 | Chains `base(purity, ptup.Length)` and stores `m_args = ptup`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x06000262 | Concatenates `m_args` with incoming `args` via `FunctionUtilities.concat` and unboxes to `R`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnTupleDifferentialTests` directly compares both members of `Fn_tuple` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(object[], int)`, testing encapsulated array storage, configured purity, and arity matching array length.
- `invVec<R>(out R, object[])`, testing tuple concatenation across varying array lengths, empty vectors, and element ordering.

Both methods achieve 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
