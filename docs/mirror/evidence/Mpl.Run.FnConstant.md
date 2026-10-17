# Mpl.Run.FnConstant

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/DelegateFunction.cs`; tests: `Quire.Differential.Tests.FnConstantDifferentialTests`.

`FnConstant` represents a constant function returning an encapsulated value `m_val` regardless of its invocation arguments. Constructed with base purity 1024, minargs 0, and `m_varargs = true`. `Name` evaluates `m_val.ToString()`. `invVec<R>` and generic `inv<R, ...>` unbox `m_val` directly into the `out R` return slot. `Apply` and `DynApply` delegate through `invVec<object>` and return the unboxed object.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(object)` | 0x06000252 | Constructs constant function with purity 1024, minargs 0, varargs true, storing `m_val`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `get_Name()` | 0x06000253 | Evaluates `m_val.ToString()`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x06000254 | Unboxes `m_val` to `R` into out slot `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `Apply(object[])` | 0x06000255 | Invokes `invVec<object>` and returns `m_val`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynApply(object[])` | 0x06000256 | Invokes `invVec<object>` and returns `m_val`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A>(out R, A)` | 0x06000257 | Unboxes `m_val` to `R` into out slot `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B>(out R, A, B)` | 0x06000258 | Unboxes `m_val` to `R` into out slot `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C>(out R, A, B, C)` | 0x06000259 | Unboxes `m_val` to `R` into out slot `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D>(out R, A, B, C, D)` | 0x0600025a | Unboxes `m_val` to `R` into out slot `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnConstantDifferentialTests` directly compares all 9 members of `FnConstant` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(object)`, testing encapsulated value, default purity=1024, minargs=0, and varargs=true.
- `get_Name()`, testing `ToString()` delegation across diverse value types (strings, integers, custom representations).
- `invVec<R>(out R, object[])`, testing generic unboxing across empty and non-empty argument vectors.
- `Apply(object[])` and `DynApply(object[])`, testing polymorphic invocation and return value extraction.
- Generic `inv` overloads 1 through 4 (`inv<R, A>` through `inv<R, A, B, C, D>`), testing argument discarding and direct out parameter assignment.

All 9 methods achieve 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
