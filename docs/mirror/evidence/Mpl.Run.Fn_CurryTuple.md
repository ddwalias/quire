# Mpl.Run.Fn_CurryTuple

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/Fn_CurryTuple.cs`; tests: `Quire.Differential.Tests.FnCurryTupleDifferentialTests`.

`Fn_CurryTuple` adapts an underlying `Function m_f` with specified purity and a string representation name, asserting full application on invocation. Constructed with `base(purity, 0)`, storing `m_name = name.ToString()`, `m_varargs = false`, and `m_f = f`. `Name` returns `m_name`. `invVec<R>` asserts full application with `args.Length` and delegates to `m_f.inv<R, object[]>`. `Apply` and `DynApply` delegate to `invVec<object>`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Function, int, object)` | 0x0600026c | Stores `m_name`, sets `m_varargs = false`, and stores `m_f`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `get_Name()` | 0x0600026d | Evaluates `m_name`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x0600026e | Asserts full application of `args.Length` and delegates to `m_f.inv<R, object[]>`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `Apply(object[])` | 0x0600026f | Invokes `invVec<object>` and returns `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynApply(object[])` | 0x06000270 | Invokes `invVec<object>` and returns `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnCurryTupleDifferentialTests` directly compares all 5 members of `Fn_CurryTuple` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(Function, int, object)`, testing inner function reference, purity, minargs=0, varargs=false, and name evaluation.
- `get_Name()`, testing returned name property.
- `invVec<R>(out R, object[])`, testing full application assertion and vector delegation to `m_f.inv<R, object[]>`.
- `Apply(object[])` and `DynApply(object[])`, testing polymorphic invocation and result extraction.

All 5 methods achieve 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
