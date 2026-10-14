# Mpl.Run.Function matrix call dependencies

- Original mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`; same-artifact ILSpy output retained outside repository.

Function constructor stores n, purity=function_purity-n, minargs=purity, no-vectorize=false and function dimension. Full application accepts all counts for varargs; otherwise minargs<=count<=purity. AssertFullApp calls inherited BadCall with minargs when too few, purity otherwise. AsF unary/binary first checks full application and returns delegate calling virtual inv overload, not app (optional arguments are not padded in delegate invocation).

Apply does not check arity; appVec dispatches non-varargs arities 1..16 to app overloads, honoring virtual app1..4 and virtual inv0..16. Varargs dispatch directly invokes invVec preserving original argument-array identity. For other arities equal purity invokes invVec; excess invokes purity-prefix then applies remainder to returned Function; optional counts allocate purity-sized null-padded array; too few build closure chain.

app0 invokes inv when purity=0/varargs or pads when minargs=0, then always raises null_application. app1 invokes when purity=1/varargs, creates closure if minargs>1, otherwise pads with null. app2..15 invoke when purity matches/varargs; excess or insufficient required count applies all but last then invokes returned function with last; otherwise appends null and proceeds to next arity. app16 uses same branches but unsupported optional padding returns default(R). apply generic wrappers return object from typed app calls. inv overloads allocate object arrays and call invVec; inv0 uses shared empty_vec.

clo creates Fn_clo with original function and captured value; clovec repeatedly captures in sequence. Fn_clo function constructor uses base(original.Purity,1), stores function/value; Name combines original name and captured ToString (null throws). Its invVec prepends captured value then calls original appVec; inv0..15 prepend captured typed argument through corresponding original app overload, preserving virtual dispatch. Delegate-based constructor and Fixpoint remain outside this slice.

FunctionUtilities take returns prefix copy only when i<array length, otherwise aliases input; but returns suffix copy when i<length, otherwise empty array; negative lengths/indexes retain Array exceptions. PushFront allocates a new array and shifts existing arguments. empty_vec is a shared empty array; wider reflection initializer remains outside this slice.

| Exact member (REA decoded signature) | Token | Normalized IL prefix | Status |
|---|---|---|---|
| `Mpl.Run.Function..cctor()` | 0x06000151 | db212ccb5d0ec736 | Analyzed |
| `Mpl.Run.Function..ctor(i4, i4)` | 0x06000152 | 2aba5a02b11c19e0 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0)` | 0x06000193 | 43f6acb17902bac5 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1)` | 0x06000194 | da7748f39b0a5379 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2)` | 0x06000195 | 2ca21c83d1ea571e | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3)` | 0x06000196 | 913fee09bff49ae3 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4)` | 0x06000197 | b0dd03658b76f58c | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5)` | 0x06000198 | ef2865f535391ba3 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6)` | 0x06000199 | 45a5886d6b998be0 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7)` | 0x0600019a | 0e10fb7ceed1c043 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8)` | 0x0600019b | 7e6ef21abd532f36 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9)` | 0x0600019c | 57465005b195c8fb | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10)` | 0x0600019d | b207ecf24f271a65 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11)` | 0x0600019e | 609c68cf1365331f | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12)` | 0x0600019f | 68d91b30b6a46c23 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13)` | 0x060001a0 | 159ccf2ec8aa5680 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14)` | 0x060001a1 | cce389652e2ca042 | Analyzed |
| `Mpl.Run.Function.apply(mvar:0, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14, mvar:15)` | 0x060001a2 | c45c58a0bb23965c | Analyzed |
| `Mpl.Run.Function.clo(mvar:0)` | 0x060001a4 | 46d92356de43b922 | Analyzed |
| `Mpl.Run.Function.clovec(mvar:0[])` | 0x060001a5 | 71c98d7108be645e | Analyzed |
| `Mpl.Run.Function.IsFullyApplied(i4)` | 0x060001a6 | 76e4f48d98861daf | Analyzed |
| `Mpl.Run.Function.AssertFullApp(i4)` | 0x060001a7 | 44e8e3a9e581b1b9 | Analyzed |
| `Mpl.Run.Function.AssertFullApp(i4, u4)` | 0x060001a8 | 4951dd3844f54b91 | Analyzed |
| `Mpl.Run.Function.appVec(mvar:0&, object[])` | 0x060001a9 | 9eb9c778d4171eb9 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&)` | 0x060001aa | 49e5bb64da311ff4 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1)` | 0x060001ab | 60b5c8fb68b0810c | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2)` | 0x060001ac | e567f706e0bc648b | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3)` | 0x060001ad | d3079cd103eca81a | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4)` | 0x060001ae | 7b878091b5eedf84 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5)` | 0x060001af | 6111b69cbe307b34 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6)` | 0x060001b0 | 435282bc9513470a | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7)` | 0x060001b1 | ec42fe62bf9d3e3e | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8)` | 0x060001b2 | fba448746daff4f8 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9)` | 0x060001b3 | aaa33e905682a6c9 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10)` | 0x060001b4 | abf295903e4766bb | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11)` | 0x060001b5 | 9a2173b3c367b60c | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12)` | 0x060001b6 | f339368ba7ede053 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13)` | 0x060001b7 | fa1d983d7b0e8170 | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14)` | 0x060001b8 | 552a85fed0cdb72a | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14, mvar:15)` | 0x060001b9 | f1248d493a7aaf3d | Analyzed |
| `Mpl.Run.Function.app(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14, mvar:15, mvar:16)` | 0x060001ba | 423fb342734ec72b | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&)` | 0x060001bc | 9ecaa4723b97e8fd | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1)` | 0x060001bd | abfd58b7a28f089b | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2)` | 0x060001be | 0e93a32654816960 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3)` | 0x060001bf | fdeb62b9e24ab05e | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4)` | 0x060001c0 | 0aa3d2207db33da8 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5)` | 0x060001c1 | 35bb6cb210cc8ff8 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6)` | 0x060001c2 | 90af42775eae3de8 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7)` | 0x060001c3 | 7d1a6493e92c2b51 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8)` | 0x060001c4 | 3ea330f4a4a6a551 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9)` | 0x060001c5 | e57f8a4602057d48 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10)` | 0x060001c6 | 13199c2e90068545 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11)` | 0x060001c7 | 2bdd23cd6ee97065 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12)` | 0x060001c8 | 5d30ce49ed1fa767 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13)` | 0x060001c9 | ffa62ebbc571709d | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14)` | 0x060001ca | b5b991e039feb0fa | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14, mvar:15)` | 0x060001cb | 4341ad7d42a92fb9 | Analyzed |
| `Mpl.Run.Function.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14, mvar:15, mvar:16)` | 0x060001cc | 58aba0b4244838c4 | Analyzed |
| `Mpl.Run.Function.Apply(object[])` | 0x060001d3 | 90a72c6b33b03824 | Analyzed |
| `Mpl.Run.Function.AsDelegate_1()` | 0x060001dd | 8876d96177460141 | Analyzed |
| `Mpl.Run.Function.AsDelegate_2()` | 0x060001de | 87be86511a13b494 | Analyzed |
| `Mpl.Run.Function.AsDelegate_1()` | 0x060001e1 | 690590f626e62db4 | Analyzed |
| `Mpl.Run.Function.AsDelegate_2()` | 0x060001e2 | 6fcea0f4c75f5ef8 | Analyzed |
| `Mpl.Run.Fn_clo`1..ctor(class:196, var:0)` | 0x06000224 | 27f5cc2dc90dae26 | Analyzed |
| `Mpl.Run.Fn_clo`1.get_Name()` | 0x06000226 | a44f0aac959b05a2 | Analyzed |
| `Mpl.Run.Fn_clo`1.invVec(mvar:0&, object[])` | 0x06000227 | c6d448774c11f1af | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&)` | 0x06000228 | 2ad360b05f6a34f5 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1)` | 0x06000229 | f331d8c2dcdf7853 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2)` | 0x0600022a | 7303ae888d5db4aa | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3)` | 0x0600022b | 7b7b51c93d924421 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4)` | 0x0600022c | 4fee33ed132a2ed9 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5)` | 0x0600022d | 4b5fd011210e2a4c | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6)` | 0x0600022e | 8000a1e55b7c8a04 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7)` | 0x0600022f | 61813eb4da1de64f | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8)` | 0x06000230 | 49bb43ae57d8e4d5 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9)` | 0x06000231 | 89fc39f8616c47d1 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10)` | 0x06000232 | f315cf8da9d0673c | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11)` | 0x06000233 | 30c690b26db9bc85 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12)` | 0x06000234 | f5a206ff4f36aaa3 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13)` | 0x06000235 | 0e379de87a886e12 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14)` | 0x06000236 | b7ceca68010176c7 | Analyzed |
| `Mpl.Run.Fn_clo`1.inv(mvar:0&, mvar:1, mvar:2, mvar:3, mvar:4, mvar:5, mvar:6, mvar:7, mvar:8, mvar:9, mvar:10, mvar:11, mvar:12, mvar:13, mvar:14, mvar:15)` | 0x06000237 | 8ba35e43658c57aa | Analyzed |
| `Mpl.Run.FunctionUtilities.but(i4, mvar:0[])` | 0x0600027a | 838233d2a0d3d81a | Analyzed |
| `Mpl.Run.FunctionUtilities.take(i4, mvar:0[])` | 0x0600027b | 7a3d6d25b2aba304 | Analyzed |
| `Mpl.Run.FunctionUtilities.PushFront(object, object[])` | 0x0600027c | f9cb0947492cacc9 | Analyzed |

Confidence: static analysis only; tests await primary validation. No claim for wider Function/FunctionUtilities/Fn_clo members.

Fields (Analyzed):
- `Mpl.Run.Function.OptArg`, token `0x04000073`: object.
- `Mpl.Run.Function.label_delimiter`, token `0x04000074`: char[].
- `Mpl.Run.Fn_clo`1.m_func`, token `0x0400007a`: class:196.
- `Mpl.Run.Fn_clo`1.m_clo`, token `0x0400007b`: var:0.
- `Mpl.Run.FunctionUtilities.empty_vec`, token `0x0400008c`: object[].

Function Name returns stored name or `<anonymous>`; setter assigns name without transformation. ToString returns `Function: ` followed by virtual Name. Static contract, Analyzed.
- `get_Name` token `0x0600014c`, normalized IL `bae3749dbebae8db3c5b5abb2183d8b3bc9eff05c7ef88960de8485d0166d9bc`.
- `set_Name` token `0x0600014d`, normalized IL `518d96cd6f620ce8aff4ff1c8b509b0f803385bed3d03fe69897d11d097480fd`.
- `ToString` token `0x0600014e`, normalized IL `43123ff28345bca6ab54f47c443c74eb08304d1a4f113918932c705c2763e986`.

Confirmed by direct FunctionMatrixDifferentialTests: `AssertFullApp(n, errpos)` and AsF/AsDelegate errpos errors retain inherited DimVal.BadCall dispatch: errpos is `AppException.Argument`, while `AppException.Handle` remains zero. Independent tests assert both fields. This corrects test expectations only; implementation already matched original.
