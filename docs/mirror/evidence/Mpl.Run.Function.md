# Mpl.Run.Function

- Original mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`; same-artifact ILSpy output retained outside repository.

Function is the base class for all callable runtime objects in MPL:
- Signature & lifecycle:
  - Constructors establish function purity, minimum argument threshold (`m_minargs`), varargs capability (`m_varargs`), dimension (`function_dim`), and no-vectorization flags.
  - `Name` property caches or formats canonical names; `format_vector` comma-separates object arrays; `canonical_name` strips internal prefixes.
  - `ToString()` returns `"Function: " + Name`.
- Vectorization & Inverse functions:
  - `IgnoreVectorization()` queries or mutates `m_no_vectorize`.
  - `OptFunc(int)` sets `m_minargs` to enable optional trailing parameter padding.
  - `InverseFunction` property stores or retrieves an associated inverse function; `DeclareInverse` sets reciprocal relationships.
  - `IsFunction`, `asFunction`, `IsFunctionN`, and `asFunctionN` perform runtime type assertions with optional error position reporting.
  - `CheckPremium()` and `Premium()` enforce licensing checks for advanced capability functions.
- Execution dispatch:
  - `DynCall` (0 to 16 arguments): calls corresponding `Call_N<object>`.
  - `Call` (0 to 16 arguments): invokes typed `Call_N<object>`.
  - `Call_0<R>()` through `Call_16<R, ...>()`: invokes `app` overloads, unpacking return value from `ref R r`.
  - `Call_vec<R>(object[])`: vector invocation calling `appVec(ref R, args)`.
  - `Apply`, `DynApply`, `VecApply`, and `VecCall`: dynamic application and matrix vectorization overloads.
- Closures and Partial Application:
  - `closure<CLO>` creates `DelegateClosure` instances.
  - `clo(a)` and `clovec(args)` create `Fn_clo<CLO>` chains, capturing values in sequence and tracking purity.
  - `IsFullyApplied` and `AssertFullApp` enforce argument bounds, reporting `AppException` on violation.
  - `CurryMatVal` and `UnCurryMatVal` curry matrix arguments into sequential single-argument applications.
- Composition & Factories:
  - `ConstantFunction`: returns an `FnConstant` returning a constant value.
  - `VectorFunction`: constructs a function over vector arguments.
  - `Fixpoint`: creates recursive fixpoint functions.
  - `Compose`: returns `FnCompose` composing two functions.
  - `AsF` and `AsDelegate_1`..`AsDelegate_4`: project functions to strongly-typed or object-typed delegates with error position handling.
  - `Apply1`: fast-path single-argument invocation for `object`, `int`, and `double`.
- Helper and delegate function types documented under this evidence record:
  - `DelegateFunction`, `UnaryDelegateFunction`, `BinaryDelegateFunction`, `TernaryDelegateFunction`, `QuartnaryDelegateFunction`.
  - `PremiumFunction`, `FnCompose`, `FnConstant`, `Fn_varargs`, `Fn_clo<CLO>`.

| Exact member (REA decoded signature) | Token | Normalized IL prefix | Status |
|---|---|---|---|
| `Mpl.Run.Function.get_Purity()` | 0x06000149 | bc3e6626f215d7b4 | Analyzed |
| `Mpl.Run.Function.get_MinArgs()` | 0x0600014a | 911929a0668e627f | Analyzed |
| `Mpl.Run.Function.get_Varargs()` | 0x0600014b | a90023a5b74bcefc | Analyzed |
| `Mpl.Run.Function.get_Name()` | 0x0600014c | 2593d092d85deae6 | Analyzed |
| `Mpl.Run.Function.set_Name(string)` | 0x0600014d | a75a89e9094c39c8 | Analyzed |
| `Mpl.Run.Function.ToString()` | 0x0600014e | fa58585d1bf253c9 | Analyzed |
| `Mpl.Run.Function.format_vector(object[])` | 0x0600014f | 5a365c57856749bf | Analyzed |
| `Mpl.Run.Function.canonical_name(string)` | 0x06000150 | cc071d0c7ab4fb8f | Analyzed |
| `Mpl.Run.Function..cctor()` | 0x06000151 | 823cbd07557374f5 | Analyzed |
| `Mpl.Run.Function..ctor(int, int)` | 0x06000152 | 8170ba0a0bdffae9 | Analyzed |
| `Mpl.Run.Function..ctor()` | 0x06000153 | 70e3e8e5b988cb14 | Analyzed |
| `Mpl.Run.Function.IgnoreVectorization()` | 0x06000154 | d49ce0f75e2cd069 | Analyzed |
| `Mpl.Run.Function.IgnoreVectorization(bool)` | 0x06000155 | a1955be148f20513 | Analyzed |
| `Mpl.Run.Function.OptFunc(int)` | 0x06000156 | 800f8f2de7ce87c7 | Analyzed |
| `Mpl.Run.Function.get_InverseFunction()` | 0x06000157 | 4e2620dcdf4fba10 | Analyzed |
| `Mpl.Run.Function.set_InverseFunction(Mpl.Run.Function)` | 0x06000158 | 2c4c4f31b826bf0d | Analyzed |
| `Mpl.Run.Function.DeclareInverse(Mpl.Run.Function)` | 0x06000159 | 1597e475abb1e5f6 | Analyzed |
| `Mpl.Run.Function.get_IsFunction()` | 0x0600015a | 2434fdab84f43e54 | Analyzed |
| `Mpl.Run.Function.asFunction(string, uint)` | 0x0600015b | 2434fdab84f43e54 | Analyzed |
| `Mpl.Run.Function.IsFunctionN(int)` | 0x0600015c | 1fa9598fb2238ca1 | Analyzed |
| `Mpl.Run.Function.asFunctionN(int, uint)` | 0x0600015d | b3885e39bc3c737b | Analyzed |
| `Mpl.Run.Function.CheckPremium()` | 0x0600015e | 8adfd1bed97cf55a | Analyzed |
| `Mpl.Run.Function.Premium()` | 0x0600015f | 33a410b1555155f5 | Analyzed |
| `Mpl.Run.Function.DynCall()` | 0x06000160 | c5dde301ed33037b | Analyzed |
| `Mpl.Run.Function.DynCall(object)` | 0x06000161 | 50786d6744a6af8d | Analyzed |
| `Mpl.Run.Function.DynCall(object, object)` | 0x06000162 | ff1c322858ee4a30 | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object)` | 0x06000163 | b6a63a5a1b7dfd0b | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object)` | 0x06000164 | 505fbb207bd0c1b0 | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object)` | 0x06000165 | 73b28e7ea8292f50 | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object)` | 0x06000166 | fcdfde26c555ecc2 | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object)` | 0x06000167 | 6369bae5faad6b9e | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object)` | 0x06000168 | 40f053d528ce5c1c | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object)` | 0x06000169 | 0519ae128400be0c | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object)` | 0x0600016a | ab5b26eed248e0ab | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object, object)` | 0x0600016b | d7cdc962925bbcfe | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600016c | 2bfee750cada751e | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600016d | 9b2e4020299c7ac6 | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600016e | 57cc4d52aa65880b | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600016f | 292fb3162582b4e0 | Analyzed |
| `Mpl.Run.Function.DynCall(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000170 | 4763203ea16db1e7 | Analyzed |
| `Mpl.Run.Function.Call()` | 0x06000171 | c5dde301ed33037b | Analyzed |
| `Mpl.Run.Function.Call(object)` | 0x06000172 | 27a1ca7800b561e1 | Analyzed |
| `Mpl.Run.Function.Call(object, object)` | 0x06000173 | 89805992912c9070 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object)` | 0x06000174 | 102bbdd0a74c9607 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object)` | 0x06000175 | f0a39552e12ae116 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object)` | 0x06000176 | 98ac7b44b9ffb5c0 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object)` | 0x06000177 | f901c3d4660c7561 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object)` | 0x06000178 | 8f0973515f0301a9 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object)` | 0x06000179 | 38a7f55a1d907796 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object)` | 0x0600017a | ff61787f0f12d762 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object)` | 0x0600017b | e069cb31c083f0c8 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object, object)` | 0x0600017c | 5661633a3223753a | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600017d | 6189640fcd2c6ba5 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600017e | 3ea159edf92eda41 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x0600017f | a6978e8aa9941538 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000180 | da0ea098b358b756 | Analyzed |
| `Mpl.Run.Function.Call(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)` | 0x06000181 | a651caa3f71ce1f1 | Analyzed |
| `Mpl.Run.Function.Call_0()` | 0x06000182 | b03332e47670cc7a | Analyzed |
| `Mpl.Run.Function.Call_1(!!1)` | 0x06000183 | f2e7d49a56d49f0c | Analyzed |
| `Mpl.Run.Function.Call_2(!!1, !!2)` | 0x06000184 | 2ed0b55f7f6f11cb | Analyzed |
| `Mpl.Run.Function.Call_3(!!1, !!2, !!3)` | 0x06000185 | d49a2b81f21cae8d | Analyzed |
| `Mpl.Run.Function.Call_4(!!1, !!2, !!3, !!4)` | 0x06000186 | 3bcdb61cf0f6a109 | Analyzed |
| `Mpl.Run.Function.Call_5(!!1, !!2, !!3, !!4, !!5)` | 0x06000187 | b8ed1409c6609291 | Analyzed |
| `Mpl.Run.Function.Call_6(!!1, !!2, !!3, !!4, !!5, !!6)` | 0x06000188 | 4d83f7b256198be7 | Analyzed |
| `Mpl.Run.Function.Call_7(!!1, !!2, !!3, !!4, !!5, !!6, !!7)` | 0x06000189 | e1d1c13297606ed6 | Analyzed |
| `Mpl.Run.Function.Call_8(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8)` | 0x0600018a | aba4bcbbd247034d | Analyzed |
| `Mpl.Run.Function.Call_9(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9)` | 0x0600018b | 5f9bca68cf272370 | Analyzed |
| `Mpl.Run.Function.Call_10(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10)` | 0x0600018c | 67fdedc7e224e1ca | Analyzed |
| `Mpl.Run.Function.Call_11(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11)` | 0x0600018d | d0a7062d812449eb | Analyzed |
| `Mpl.Run.Function.Call_12(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12)` | 0x0600018e | e99a3c7f9acb3569 | Analyzed |
| `Mpl.Run.Function.Call_13(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13)` | 0x0600018f | 4cfc94b8b878c8e6 | Analyzed |
| `Mpl.Run.Function.Call_14(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14)` | 0x06000190 | fd85cc1ff68bcb61 | Analyzed |
| `Mpl.Run.Function.Call_15(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15)` | 0x06000191 | 31f856933936faa9 | Analyzed |
| `Mpl.Run.Function.Call_16(!!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15, !!16)` | 0x06000192 | b85b2aaba886a0f1 | Analyzed |
| `Mpl.Run.Function.apply(!!0)` | 0x06000193 | 18eb0fd9d4b0bfd5 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1)` | 0x06000194 | a0fa3fe3f1504ff8 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2)` | 0x06000195 | 1214f3c36bd33c18 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3)` | 0x06000196 | 706489bddc575dbc | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4)` | 0x06000197 | 32589aca200ab9db | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5)` | 0x06000198 | 23e4bd5952c36323 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6)` | 0x06000199 | ceb56d4bbce3e028 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7)` | 0x0600019a | 80fd4500a87d8ca2 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8)` | 0x0600019b | 0ae371d6f1c47f6e | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9)` | 0x0600019c | b903cd017c157896 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10)` | 0x0600019d | 615e17801bc1d0db | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11)` | 0x0600019e | 32b6c42cace5aeb1 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12)` | 0x0600019f | 5202aeb1dbd7b398 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13)` | 0x060001a0 | 9642012a632aa2ef | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14)` | 0x060001a1 | 9cdaf9cadcbdbc00 | Analyzed |
| `Mpl.Run.Function.apply(!!0, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15)` | 0x060001a2 | ac08e43ea4947528 | Analyzed |
| `Mpl.Run.Function.closure(System.Delegate, !!0, int)` | 0x060001a3 | c1dad7c5cafbceb7 | Analyzed |
| `Mpl.Run.Function.clo(!!0)` | 0x060001a4 | dceba58cdcdfdf9f | Analyzed |
| `Mpl.Run.Function.clovec(!!0[])` | 0x060001a5 | 00989a5a32821149 | Analyzed |
| `Mpl.Run.Function.IsFullyApplied(int)` | 0x060001a6 | 9728e737417e24eb | Analyzed |
| `Mpl.Run.Function.AssertFullApp(int)` | 0x060001a7 | 9432d2690ae82cbd | Analyzed |
| `Mpl.Run.Function.AssertFullApp(int, uint)` | 0x060001a8 | 60e4a4ad48db85ee | Analyzed |
| `Mpl.Run.Function.appVec(!!0&, object[])` | 0x060001a9 | ff4658d0005709bf | Analyzed |
| `Mpl.Run.Function.app(!!0&)` | 0x060001aa | 46bf89c6a8d10cec | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1)` | 0x060001ab | 5168a4333beac18f | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2)` | 0x060001ac | bf71178c34fb9e91 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3)` | 0x060001ad | bf409a84e71a0fda | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4)` | 0x060001ae | cb9878909a68d7a8 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5)` | 0x060001af | 593f7e0744d21f63 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6)` | 0x060001b0 | 8c43463c5ff886e4 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7)` | 0x060001b1 | 25e8ede484b9ce9c | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8)` | 0x060001b2 | 58a67967de049056 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9)` | 0x060001b3 | 856f2798012cdbbe | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10)` | 0x060001b4 | 4c223326fd13ba2e | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11)` | 0x060001b5 | 990dfb56163ba321 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12)` | 0x060001b6 | 7a01ac367c6d7fc5 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13)` | 0x060001b7 | dc349fff16bf00d7 | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14)` | 0x060001b8 | 4ccc8fed03bc222f | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15)` | 0x060001b9 | ce2af153b9b77c9b | Analyzed |
| `Mpl.Run.Function.app(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15, !!16)` | 0x060001ba | e1be8edb07e64e05 | Analyzed |
| `Mpl.Run.Function.invVec(!!0&, object[])` | 0x060001bb |                  | Analyzed |
| `Mpl.Run.Function.inv(!!0&)` | 0x060001bc | be73e290be19d805 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1)` | 0x060001bd | ac28abafd0290b86 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2)` | 0x060001be | 80d112a2fb6a9a7a | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3)` | 0x060001bf | 15301ee4be3f5977 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4)` | 0x060001c0 | 70b27140986000ae | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5)` | 0x060001c1 | 11f11c83b61e3f6c | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6)` | 0x060001c2 | d1654d98f6345e5e | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7)` | 0x060001c3 | 034be8dcb67cc2c3 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8)` | 0x060001c4 | 1723d8bc210aab92 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9)` | 0x060001c5 | 188e0748b3aacf64 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10)` | 0x060001c6 | d3497e48fc7fb235 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11)` | 0x060001c7 | 220c80acd305312f | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12)` | 0x060001c8 | b72b998e8435670e | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13)` | 0x060001c9 | 1d588dd1e47b1e0e | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14)` | 0x060001ca | 3087822c542f1396 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15)` | 0x060001cb | 6057eb4ce28b3674 | Analyzed |
| `Mpl.Run.Function.inv(!!0&, !!1, !!2, !!3, !!4, !!5, !!6, !!7, !!8, !!9, !!10, !!11, !!12, !!13, !!14, !!15, !!16)` | 0x060001cc | cb97dcad42311d3b | Analyzed |
| `Mpl.Run.Function.VecCall(object)` | 0x060001cd | 367952eebbf30931 | Analyzed |
| `Mpl.Run.Function.VecCall(object, object)` | 0x060001ce | 5e828323c631c4f1 | Analyzed |
| `Mpl.Run.Function.VecCall(object, object, object)` | 0x060001cf | eda1be544eed8b8b | Analyzed |
| `Mpl.Run.Function.VecApply(object[])` | 0x060001d0 | 99aac91516a48303 | Analyzed |
| `Mpl.Run.Function.DynApply(object[])` | 0x060001d1 | 8c8e3d1c58307a92 | Analyzed |
| `Mpl.Run.Function.Call_vec(object[])` | 0x060001d2 | 2c4d07c6291a20cb | Analyzed |
| `Mpl.Run.Function.Apply(object[])` | 0x060001d3 | 99c4c609e921b2a9 | Analyzed |
| `Mpl.Run.Function.InvokeVec(object[])` | 0x060001d4 | ef3bdd460c2dd1c9 | Analyzed |
| `Mpl.Run.Function.CurryMatVal()` | 0x060001d5 | f5f9e56dc3f05e8a | Analyzed |
| `Mpl.Run.Function.UnCurryMatVal()` | 0x060001d6 | 8b7d27983c0cae77 | Analyzed |
| `Mpl.Run.Function.ConstantFunction(object)` | 0x060001d7 | 8086a925fe9b0af3 | Analyzed |
| `Mpl.Run.Function.VectorFunction(object)` | 0x060001d8 | 5f2fa30d10023bda | Analyzed |
| `Mpl.Run.Function.AsF(uint)` | 0x060001d9 | 9634c11c2a527298 | Analyzed |
| `Mpl.Run.Function.AsF(uint)` | 0x060001da | bb72cec677de0bd5 | Analyzed |
| `Mpl.Run.Function.AsF(uint)` | 0x060001db | e979fcd5e22d5911 | Analyzed |
| `Mpl.Run.Function.AsF(uint)` | 0x060001dc | bfce3a6b3dbbde93 | Analyzed |
| `Mpl.Run.Function.AsDelegate_1()` | 0x060001dd | a890b93e8d6f1b95 | Analyzed |
| `Mpl.Run.Function.AsDelegate_2()` | 0x060001de | 4c957cb5e8744b1d | Analyzed |
| `Mpl.Run.Function.AsDelegate_3()` | 0x060001df | 4772a33be95031c6 | Analyzed |
| `Mpl.Run.Function.AsDelegate_4()` | 0x060001e0 | 4172e5bc339de385 | Analyzed |
| `Mpl.Run.Function.AsDelegate_1(uint)` | 0x060001e1 | 09537e7f65f3cbe9 | Analyzed |
| `Mpl.Run.Function.AsDelegate_2(uint)` | 0x060001e2 | d52aa1242f20dc70 | Analyzed |
| `Mpl.Run.Function.AsDelegate_3(uint)` | 0x060001e3 | 364d1ae89535ad2f | Analyzed |
| `Mpl.Run.Function.AsDelegate_4(uint)` | 0x060001e4 | c38c360862355a16 | Analyzed |
| `Mpl.Run.Function.Fixpoint(Mpl.Run.Function)` | 0x060001e5 | b7ed4a39a048b300 | Analyzed |
| `Mpl.Run.Function.Compose(Mpl.Run.Function, Mpl.Run.Function)` | 0x060001e6 | 9945dd736aa48713 | Analyzed |
| `Mpl.Run.Function.Apply1(object)` | 0x060001e7 | 6c97f51470a8606b | Analyzed |
| `Mpl.Run.Function.Apply1(int)` | 0x060001e8 | 17680ec7d1f991ce | Analyzed |
| `Mpl.Run.Function.Apply1(double)` | 0x060001e9 | 678c782af4fefd2d | Analyzed |

Fields (Analyzed):
- `Mpl.Run.Function.m_purity`, token `0x0400006c`: int.
- `Mpl.Run.Function.m_n`, token `0x0400006d`: int.
- `Mpl.Run.Function.m_no_vectorize`, token `0x0400006e`: bool.
- `Mpl.Run.Function.m_minargs`, token `0x0400006f`: int.
- `Mpl.Run.Function.m_varargs`, token `0x04000070`: bool.
- `Mpl.Run.Function.m_name`, token `0x04000071`: string.
- `Mpl.Run.Function.m_inverse_function`, token `0x04000072`: Mpl.Run.Function.
- `Mpl.Run.Function.OptArg`, token `0x04000073`: object.
- `Mpl.Run.Function.label_delimiter`, token `0x04000074`: char[].
