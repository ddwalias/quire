# Mpl.Run.Fn_papp

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/DelegateFunction.cs`; tests: `Quire.Differential.Tests.FnPappDifferentialTests`.

`Fn_papp` represents a partial function application. It stores the base function `m_f` and the prepended argument vector `m_args`, setting `m_minargs` to `args.Length`, `m_purity` to `f.Purity`, and inheriting `m_varargs` from `f.Varargs`. `Name` formats `"{0}{1}"` using `m_f.Name` and `Function.format_vector(m_args)`. `invVec` concatenates `m_args` with the incoming arguments via `FunctionUtilities.concat(m_args, args)` and invokes `m_f.invVec<R>`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Function, object[])` | 0x0600025e | Stores `f` and `args`, sets purity to `f.Purity`, minargs to `args.Length`, and inherits `f.Varargs`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `get_Name()` | 0x06000260 | Formats `"{0}{1}"` with `m_f.Name` and `Function.format_vector(m_args)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x0600025f | Concatenates `m_args` and `args` via `FunctionUtilities.concat` and evaluates `m_f.invVec(out r, ...)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnPappDifferentialTests` directly compares all members of `Fn_papp` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(Function, object[])`: verifies base function, argument array, minargs, purity, and varargs matching.
- `get_Name()`: verifies string formatting with base function name and argument vector rendering.
- `invVec<R>(out R, object[])`: verifies vector argument concatenation and invocation dispatch.

Release IL opcodes match Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
