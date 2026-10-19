# Mpl.Run.MethFunction

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/DelegateFunction.cs`; tests: `Quire.Differential.Tests.MethFunctionDifferentialTests`.

`MethFunction` wraps a reflection `MethodInfo` into a callable `Function`. It stores `m_method`, the optional closure target `m_clo`, and any prepended arguments `m_args`. It calculates `m_minargs` taking into account whether the method is an unbound instance method (`!method.IsStatic && clo == null`, requiring 1 extra argument for the receiver `this`) and the length of any prepended arguments. When invoked via `invVec<R>`, it dispatches to `m_method.Invoke(m_clo, FunctionUtilities.concat(m_args, args))` for static/bound methods, or uses `args[0]` as the receiver target and passes `FunctionUtilities.concat(m_args, FunctionUtilities.but(1, args))` for unbound instance methods.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(MethodInfo, object, object[])` | 0x06000271 | Stores method, clo target, and prepended args; computes minargs based on method parameters, staticness, clo presence, and prepended args length. Sets purity to impure. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `.ctor(MethodInfo, object)` | 0x06000272 | Chains to `.ctor(method, clo, null)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `.ctor(MethodInfo)` | 0x06000273 | Chains to `.ctor(method, null, null)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x06000274 | Dispatches invocation to `m_method.Invoke`, resolving receiver for unbound instance methods and concatenating prepended args. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`MethFunctionDifferentialTests` directly compares all members of `MethFunction` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(MethodInfo, object, object[])`, `.ctor(MethodInfo, object)`, `.ctor(MethodInfo)`: verifies method, closure target, prepended args, minargs calculation for both static and instance methods, and impure purity flag.
- `invVec<R>(out R, object[])`: verifies dynamic dispatch for static methods, bound instance methods, and unbound instance methods (where `args[0]` is the target instance), including prepended argument forwarding.

Release IL opcodes match Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
