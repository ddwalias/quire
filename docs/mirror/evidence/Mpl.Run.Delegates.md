# Mpl.Run.Delegates

- Original mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- inspection record `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`; same-artifact ilspycmd output retained outside repository.

Delegates provides reflection caches, delegate factories, currying, and projections for MPL function evaluation:
- Static type caches:
  - `gtypes`: open generic delegate types `F<R, A>`, `F<R, A, B>`, `F<R, A, B, C>`, `F<R, A, B, C, D>`.
  - `vtypes`: open generic action types `V<A>`, `V<A, B>`, `V<A, B, C>`, `V<A, B, C, D>`.
  - `imethods`: method handles for dynamic invocation wrappers `Invoke1` through `Invoke4`.
  - `clo_method`: method handles for typed currying helpers `Closure_1` through `Closure_4`.
  - `PI_1`, `PI_2`: static projection functions `pi_21()` and `pi_22()`.
- Delegate closure generation:
  - `Closure_1`..`Closure_4`: curry a typed `F<...>` delegate with a captured `clo` argument, producing a delegate of arity `N-1`.
  - `DelegateClosure_1`..`DelegateClosure_4`: wrap an untyped `Delegate` into an `F<object, ...>` delegate capturing `clo`.
  - `FunctionClosure_1`..`FunctionClosure_4`: create a `DelegateFunction` wrapping the corresponding `DelegateClosure_N`.
  - `DelegateClosure(Delegate d, CLO clo, int purity)`: dynamic dispatch selecting `DelegateClosure_1`..`4` by arity.
- Signature and type conversion:
  - `clo_params`: computes remaining parameter types when the first argument (index 1 of `ftys`) is captured in a closure.
  - `fparams`: extracts return type followed by parameter types from `(Type, Type[])`, `MethodInfo`, or `Delegate`.
  - `FType` / `VType`: constructs closed `F<...>` or `V<...>` types from argument arrays.
  - `toFDelegate`: converts an existing delegate to the matching `F<...>` delegate type using constructor reflection or cast.
  - `MethodToDelegate`, `MethodToDelegateType`, `MethodToDelegateCtor`: creates delegates and constructor info from `MethodInfo`.
  - `ParameterTypesToDelegateType`, `ParameterTypesToDelegateCtor`: computes delegate types and constructors for signature components.
- Invocation and wrapping:
  - `Invoke1`..`Invoke4`: invokes generic delegates with arguments unpacked from `object`.
  - `Premium_1`..`Premium_4`: attaches premium license verification to delegates.
  - `Wrap1`..`Wrap3`: wraps strongly-typed delegates into untyped `object` delegates.
  - `project21`, `project22`: binary projection delegates returning first or second argument.
  - `pi_21`, `pi_22`: `DelegateFunction` instances wrapping `project21` and `project22`.

| Exact member (decoded signature) | Token | Normalized IL prefix | Status |
|---|---|---|---|
| `Mpl.Run.Delegates..cctor()` | 0x06000319 | 290077c082ce36bd | Analyzed |
| `Mpl.Run.Delegates.clo_params(System.Type[])` | 0x0600031a | 36c46086f40c398e | Verified |
| `Mpl.Run.Delegates.fparams(System.Type, System.Type[])` | 0x0600031b | 84648a5567f5fcc3 | Verified |
| `Mpl.Run.Delegates.fparams(System.Reflection.MethodInfo)` | 0x0600031c | c54f23b5b6d4713d | Verified |
| `Mpl.Run.Delegates.fparams(System.Delegate)` | 0x0600031d | a792102a9a2f7b34 | Verified |
| `Mpl.Run.Delegates.Invoke1(Mpl.Run.F<!!0, !!1>, object)` | 0x0600031e | e02982f795346a97 | Verified |
| `Mpl.Run.Delegates.Invoke2(Mpl.Run.F<!!0, !!1, !!2>, object, object)` | 0x0600031f | 75632cc3aa6d5015 | Verified |
| `Mpl.Run.Delegates.Invoke3(Mpl.Run.F<!!0, !!1, !!2, !!3>, object, object, object)` | 0x06000320 | 3fd486ff9f205d86 | Verified |
| `Mpl.Run.Delegates.Invoke4(Mpl.Run.F<!!0, !!1, !!2, !!3, !!4>, object, object, object, object)` | 0x06000321 | 51873d154e67d66c | Verified |
| `Mpl.Run.Delegates.Premium_1(Mpl.Run.F<!!0, !!1>)` | 0x06000322 | 2f44a88c304e31a2 | Verified |
| `Mpl.Run.Delegates.Premium_2(Mpl.Run.F<!!0, !!1, !!2>)` | 0x06000323 | 2afc83a930a96bba | Verified |
| `Mpl.Run.Delegates.Premium_3(Mpl.Run.F<!!0, !!1, !!2, !!3>)` | 0x06000324 | 1a750c321a171519 | Verified |
| `Mpl.Run.Delegates.Premium_4(Mpl.Run.F<!!0, !!1, !!2, !!3, !!4>)` | 0x06000325 | 42993fdf5587f41a | Verified |
| `Mpl.Run.Delegates.Closure_1(Mpl.Run.F<!!0, !!1, !!2>, !!1)` | 0x06000326 | 2c4d42c6bdee7f14 | Analyzed |
| `Mpl.Run.Delegates.Closure_2(Mpl.Run.F<!!0, !!1, !!2, !!3>, !!1)` | 0x06000327 | f6123a16a5f579b5 | Analyzed |
| `Mpl.Run.Delegates.Closure_3(Mpl.Run.F<!!0, !!1, !!2, !!3, !!4>, !!1)` | 0x06000328 | ee1bf3c8c508a689 | Analyzed |
| `Mpl.Run.Delegates.Closure_4(Mpl.Run.F<!!0, !!1, !!2, !!3, !!4, !!5>, !!1)` | 0x06000329 | 55bb5dc0195d3a89 | Analyzed |
| `Mpl.Run.Delegates.DelegateClosure_1(System.Delegate, !!0)` | 0x0600032a | 3f190b14dd945f2d | Analyzed |
| `Mpl.Run.Delegates.FunctionClosure_1(System.Delegate, !!0)` | 0x0600032b | 9571fc5235cfbb8e | Analyzed |
| `Mpl.Run.Delegates.DelegateClosure_2(System.Delegate, !!0)` | 0x0600032c | c6b65f7a966e5918 | Analyzed |
| `Mpl.Run.Delegates.FunctionClosure_2(System.Delegate, !!0)` | 0x0600032d | b4e083ca8e429ad7 | Analyzed |
| `Mpl.Run.Delegates.DelegateClosure_3(System.Delegate, !!0)` | 0x0600032e | 90911dba7f0dd92f | Analyzed |
| `Mpl.Run.Delegates.FunctionClosure_3(System.Delegate, !!0)` | 0x0600032f | 8ec712da1d108434 | Analyzed |
| `Mpl.Run.Delegates.DelegateClosure_4(System.Delegate, !!0)` | 0x06000330 | a81f21e4e3eea2d7 | Analyzed |
| `Mpl.Run.Delegates.FunctionClosure_4(System.Delegate, !!0)` | 0x06000331 | e4170e249b856b52 | Analyzed |
| `Mpl.Run.Delegates.DelegateClosure(System.Delegate, !!0, int)` | 0x06000332 | 3a461b9e69ad8a8b | Analyzed |
| `Mpl.Run.Delegates.toFDelegate(System.Type, System.Delegate)` | 0x06000333 | e13474987f3975aa | Analyzed |
| `Mpl.Run.Delegates.FType(System.Type[])` | 0x06000334 | d19cff4d39aa1c8a | Verified |
| `Mpl.Run.Delegates.VType(System.Type[])` | 0x06000335 | 510d352d79f8ebca | Verified |
| `Mpl.Run.Delegates.MethodToDelegate(System.Reflection.MethodInfo)` | 0x06000336 | e91e93052449e07d | Verified |
| `Mpl.Run.Delegates.MethodToDelegateType(System.Reflection.MethodInfo)` | 0x06000337 | 13d79c922d51a5df | Verified |
| `Mpl.Run.Delegates.MethodToDelegateCtor(System.Reflection.MethodInfo)` | 0x06000338 | ea7cbbb097b816dc | Verified |
| `Mpl.Run.Delegates.ParameterTypesToDelegateType(System.Type, System.Type[])` | 0x06000339 | ad3e210d7864fe56 | Verified |
| `Mpl.Run.Delegates.ParameterTypesToDelegateCtor(System.Type, System.Type[])` | 0x0600033a | 3834242e412d13b9 | Verified |
| `Mpl.Run.Delegates.Wrap1(Mpl.Run.F<!!0, !!1>)` | 0x0600033b | bea96280494991b1 | Verified |
| `Mpl.Run.Delegates.Wrap2(Mpl.Run.F<!!0, !!1, !!2>)` | 0x0600033c | 3761ca81c7f817a8 | Verified |
| `Mpl.Run.Delegates.Wrap3(Mpl.Run.F<!!0, !!1, !!2, !!3>)` | 0x0600033d | 39f933799b638a74 | Verified |
| `Mpl.Run.Delegates.project21()` | 0x0600033e | 15d96cab27096a76 | Verified |
| `Mpl.Run.Delegates.project22()` | 0x0600033f | 58293e0f346cb70d | Verified |
| `Mpl.Run.Delegates.pi_21()` | 0x06000340 | 95db1e3d063f5648 | Verified |
| `Mpl.Run.Delegates.pi_22()` | 0x06000341 | 8cd1005d0a7a7e0f | Verified |
| `Mpl.Run.Delegates..ctor()` | 0x06000342 | 5e4aeaf3c57ac765 | Verified |

Fields (Analyzed):
- `Mpl.Run.Delegates.gtypes`, token `0x04000092`: System.Type[].
- `Mpl.Run.Delegates.vtypes`, token `0x04000093`: System.Type[].
- `Mpl.Run.Delegates.imethods`, token `0x04000094`: System.Reflection.MethodInfo[].
- `Mpl.Run.Delegates.PI_1`, token `0x04000095`: Mpl.Run.Function.
- `Mpl.Run.Delegates.PI_2`, token `0x04000096`: Mpl.Run.Function.
- `Mpl.Run.Delegates.clo_method`, token `0x04000097`: System.Reflection.MethodInfo[].
