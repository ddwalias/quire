# Mpl.Run.FnCompose

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/DelegateFunction.cs`; tests: `Quire.Differential.Tests.FnComposeDifferentialTests`.

`FnCompose` represents function composition `f(g(...))`. It stores outer function `m_f` and inner function `m_g`, adopting `m_g.Purity` with minimum arity 0. `Name` formats `"compose({0},{1})"` using the names of `m_f` and `m_g`. `invVec` evaluates `m_g.invVec` on incoming arguments and passes the intermediate result to `m_f.app`. The four generic `inv` overloads (arities 1 through 4) evaluate `m_g.app` and pipe the resulting value into `m_f.app`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Function, Function)` | 0x0600024b | Stores `f` and `g`, setting purity to `g.Purity` and minargs=0. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `get_Name()` | 0x0600024c | Formats `"compose({0},{1})"` with `m_f.Name` and `m_g.Name`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x0600024d | Evaluates `m_g.invVec(out r2, args)` and applies `m_f.app(out r, r2)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A>(out R, A)` | 0x0600024e | Evaluates `m_g.app(out r2, aa)` and applies `m_f.app(out r, r2)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B>(out R, A, B)` | 0x0600024f | Evaluates `m_g.app(out r2, aa, bb)` and applies `m_f.app(out r, r2)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C>(out R, A, B, C)` | 0x06000250 | Evaluates `m_g.app(out r2, aa, bb, cc)` and applies `m_f.app(out r, r2)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D>(out R, A, B, C, D)` | 0x06000251 | Evaluates `m_g.app(out r2, aa, bb, cc, dd)` and applies `m_f.app(out r, r2)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnComposeDifferentialTests` directly compares all 7 members of `FnCompose` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(Function, Function)`, testing outer/inner function references, purity propagation from `g`, and minargs=0.
- `get_Name()`, testing string formatting with distinct inner and outer function names.
- `invVec<R>(out R, object[])`, testing composition execution through argument vector.
- All four `inv` overloads from arity 1 to 4 (`inv<R, A>` through `inv<R, A, B, C, D>`), testing typed application piping intermediate results through `m_f.app`.

All 7 methods achieve 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
