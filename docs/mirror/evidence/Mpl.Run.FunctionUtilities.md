# Mpl.Run.FunctionUtilities

- Original mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`; same-artifact ILSpy output retained outside repository.

FunctionUtilities provides array manipulation and reflection caching for Function evaluation and closure generation:
- `maxargs`: constant maximum fixed arity (16).
- `empty_vec`: shared immutable empty object array for zero-argument invocations.
- Reflection helpers cache constructor and method metadata:
  - `FcnCtor`: `DelegateFunction.make(Delegate, int)` factory method.
  - `CloCtorGeneric`: `Function.closure<CLO>(Delegate, CLO, int)` generic definition.
  - `MethCtor`: `MethFunction(MethodInfo)` constructor.
  - `ObjMethCtor`: `MethFunction(object, MethodInfo)` constructor.
  - `CallMethod`: cached array of 17 `Call_0`..`Call_16` method definitions on `Function`.
  - `CloCtor(Type)`: specializes `CloCtorGeneric` with the given closure type argument.
  - `InstMethCtor(Type)`: reflects the 2-argument constructor `MethFunction(object, MethodInfo)` on the target type.
  - `GetCallMethod(Type r, Type[] args)`: resolves generic `Call_N` on `Function` matching the arity of `args`, with return type `r` and argument types `args`.
- Array helpers:
  - `concat(object[] a, object[] b)`: copies elements from `a` and `b` into a newly allocated array of length `a.Length + b.Length`.
  - `take(int i, T[] a)`: if `i < a.Length`, allocates a new array of length `i` and copies the prefix; otherwise returns `a` directly.
  - `but(int i, T[] a)`: if `i < a.Length`, allocates a new array of length `a.Length - i` copying from offset `i`; otherwise returns a new empty array of `T`.
  - `PushFront(object b, object[] a)`: allocates a new array of length `a.Length + 1` with `b` at index 0 followed by elements of `a`.
  - `PushFront(object b, object c, object[] a)`: allocates a new array of length `a.Length + 2` with `b, c` followed by `a`.
  - `Push(object[] a, object b)`: allocates a new array of length `a.Length + 1` with `a` followed by `b`.
  - `Push(object[] a, object b, object c)`: allocates a new array of length `a.Length + 2` with `a` followed by `b, c`.
  - Constructor `.ctor()` calls base `DimVal` constructor.

| Exact member (REA decoded signature) | Token | Normalized IL prefix | Status |
|---|---|---|---|
| `Mpl.Run.FunctionUtilities..cctor()` | 0x06000275 | 121cb79cff765746 | Analyzed |
| `Mpl.Run.FunctionUtilities.CloCtor(System.Type)` | 0x06000276 | 2a994f9e491e4af1 | Analyzed |
| `Mpl.Run.FunctionUtilities.InstMethCtor(System.Type)` | 0x06000277 | ba69fda565317c84 | Analyzed |
| `Mpl.Run.FunctionUtilities.GetCallMethod(System.Type, System.Type[])` | 0x06000278 | 2360cb84df00c885 | Analyzed |
| `Mpl.Run.FunctionUtilities.concat(object[], object[])` | 0x06000279 | b64f8eda8398971a | Analyzed |
| `Mpl.Run.FunctionUtilities.but(int, !!0[])` | 0x0600027a | 36a5e3861dff5ed8 | Analyzed |
| `Mpl.Run.FunctionUtilities.take(int, !!0[])` | 0x0600027b | daf206f37acde9a6 | Analyzed |
| `Mpl.Run.FunctionUtilities.PushFront(object, object[])` | 0x0600027c | 7046fe6379c09d0e | Analyzed |
| `Mpl.Run.FunctionUtilities.PushFront(object, object, object[])` | 0x0600027d | 321f4778f76df7c0 | Analyzed |
| `Mpl.Run.FunctionUtilities.Push(object[], object)` | 0x0600027e | 07b59257eba29109 | Analyzed |
| `Mpl.Run.FunctionUtilities.Push(object[], object, object)` | 0x0600027f | b1452bf738a05df3 | Analyzed |
| `Mpl.Run.FunctionUtilities..ctor()` | 0x06000280 | c4116854ff76753b | Analyzed |

Fields (Analyzed):
- `Mpl.Run.FunctionUtilities.maxargs`, token `0x0400008b`: int (const 16).
- `Mpl.Run.FunctionUtilities.empty_vec`, token `0x0400008c`: object[].
- `Mpl.Run.FunctionUtilities.FcnCtor`, token `0x0400008d`: System.Reflection.MethodInfo.
- `Mpl.Run.FunctionUtilities.CloCtorGeneric`, token `0x0400008e`: System.Reflection.MethodInfo.
- `Mpl.Run.FunctionUtilities.MethCtor`, token `0x0400008f`: System.Reflection.ConstructorInfo.
- `Mpl.Run.FunctionUtilities.ObjMethCtor`, token `0x04000090`: System.Reflection.ConstructorInfo.
- `Mpl.Run.FunctionUtilities.CallMethod`, token `0x04000091`: System.Reflection.MethodInfo[].
