# Mpl.Run.Fn_clo

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/Function.Matrix.cs`; tests: `Quire.Differential.Tests.FnCloDifferentialTests`.

`Fn_clo<CLO>` represents a partially applied closure wrapping an underlying `Function` (or `DelegateFunction`) and a single captured argument of type `CLO`. It overrides `Name` to display `"{0}[{1}]"` with the underlying function name and the string representation of the captured argument. `Fixpoint` creates a self-referential closure whose captured argument is itself. `invVec` prepends the captured argument to the incoming argument vector and dispatches through `m_func.appVec`. The 16 generic `inv` overloads route calls directly through `m_func.app`, supplying `m_clo` as the first argument followed by the remaining arguments.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Delegate, CLO, int)` | 0x06000223 | Creates `DelegateFunction.make(f, function_purity)` and stores `clo`, setting purity and minargs=1. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `.ctor(Function, CLO)` | 0x06000224 | Stores `f` and `clo`, setting purity to `f.Purity` and minargs=1. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `Fixpoint(Function)` | 0x06000225 | Creates an `Fn_clo<object>` wrapping `f` and null, then sets its captured argument to itself. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `get_Name()` | 0x06000226 | Formats `"{0}[{1}]"` with `m_func.Name` and `m_clo.ToString()`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x06000227 | Prepends `m_clo` using `FunctionUtilities.PushFront` and dispatches via `m_func.appVec`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R>(out R)` | 0x06000228 | Dispatches `m_func.app(out r, m_clo)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A>(out R, A)` | 0x06000229 | Dispatches `m_func.app(out r, m_clo, aa)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B>(out R, A, B)` | 0x0600022a | Dispatches `m_func.app(out r, m_clo, aa, bb)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C>(out R, A, B, C)` | 0x0600022b | Dispatches `m_func.app(out r, m_clo, aa, bb, cc)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D>(out R, A, B, C, D)` | 0x0600022c | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E>(out R, A, B, C, D, E)` | 0x0600022d | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F>(out R, A, B, C, D, E, F)` | 0x0600022e | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G>(out R, A, B, C, D, E, F, G)` | 0x0600022f | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H>(out R, A, B, C, D, E, F, G, H)` | 0x06000230 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I>(out R, A, B, C, D, E, F, G, H, I)` | 0x06000231 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I, J>(out R, A, B, C, D, E, F, G, H, I, J)` | 0x06000232 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I, J, K>(out R, A, B, C, D, E, F, G, H, I, J, K)` | 0x06000233 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I, J, K, L>(out R, A, B, C, D, E, F, G, H, I, J, K, L)` | 0x06000234 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M)` | 0x06000235 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M, N)` | 0x06000236 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `inv<R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(out R, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O)` | 0x06000237 | Dispatches `m_func.app(out r, m_clo, aa, bb, cc, dd, ee, ff, gg, hh, ii, jj, kk, ll, mm, nn, oo)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnCloDifferentialTests` directly compares all 21 members of `Fn_clo<CLO>` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- Both constructors (`.ctor(Function, CLO)` and `.ctor(Delegate, CLO, int)`), testing receiver reference, captured argument, purity, and minargs=1.
- `Fixpoint(Function)`, testing creation of self-referential closure (`m_clo == self`).
- `get_Name()`, testing formatted representation with different captured types.
- `invVec<R>(out R, object[])`, testing prepending captured argument and invoking underlying `appVec`.
- All 16 `inv` overloads from arity 1 up to arity 16, testing direct delegation to underlying `m_func.app`.

All 21 methods achieve 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
