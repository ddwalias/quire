# Mpl.Run.Fn_varargs

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 and Release IL comparison from `mplrun.dll`.
- **Quire:** `src/Quire.Mpl/Run/DelegateFunction.cs`; tests: `Quire.Differential.Tests.FnVarargsDifferentialTests`.

`Fn_varargs` adapts an underlying `Function m_f` to accept variable arguments. Constructed with base purity 1024, minargs 0, and `m_varargs = true`. `Name` delegates to `m_f.Name`. `invVec<R>` delegates to `m_f.inv<R, object[]>`. `Apply` and `DynApply` delegate to `invVec<object>`. Four `DynCall` overloads (taking 1 to 4 arguments) package their parameters into an `object[]` array and invoke `m_f.inv<object, object[]>`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(Function)` | 0x06000263 | Constructs varargs function with purity 1024, minargs 0, varargs true, storing `m_f`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `get_Name()` | 0x06000264 | Delegates to `m_f.Name`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `invVec<R>(out R, object[])` | 0x06000265 | Delegates to `m_f.inv<R, object[]>(out r, args)`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `Apply(object[])` | 0x06000266 | Delegates to `invVec<object>(out r, args)` and returns `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynApply(object[])` | 0x06000267 | Delegates to `invVec<object>(out r, args)` and returns `r`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynCall(object)` | 0x06000268 | Packages `[arg1]` and invokes `m_f.inv<object, object[]>`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynCall(object, object)` | 0x06000269 | Packages `[arg1, arg2]` and invokes `m_f.inv<object, object[]>`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynCall(object, object, object)` | 0x0600026a | Packages `[arg1, arg2, arg3]` and invokes `m_f.inv<object, object[]>`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |
| `DynCall(object, object, object, object)` | 0x0600026b | Packages `[arg1, arg2, arg3, arg4]` and invokes `m_f.inv<object, object[]>`. | Release IL equals Mathcad's; direct differential comparison against `mplrun.dll`. | Verified |

## Test cases

`FnVarargsDifferentialTests` directly compares all 9 members of `Fn_varargs` against Mathcad Prime 12.0.1.0 `mplrun.dll`:
- `.ctor(Function)`, testing encapsulated inner function, default purity=1024, minargs=0, and varargs=true.
- `get_Name()`, testing delegation to `m_f.Name`.
- `invVec<R>(out R, object[])`, testing generic delegation to `m_f.inv<R, object[]>`.
- `Apply(object[])` and `DynApply(object[])`, testing polymorphic invocation and return value extraction.
- Four `DynCall` overloads (arities 1 through 4), testing array packaging and element ordering when invoking `m_f.inv<object, object[]>`.

All 9 methods achieve 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

## Open

None.
