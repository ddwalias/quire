# Lane A work checkpoint

## DimVal, Dim, NumVal and Iterator comparisons (`mirror/a-dimval`)

Branch `mirror/a-dimval`, rebased onto `c022010`: one work commit, then the maintainer's acceptance commit on top.
It supersedes `mirror/a-dimval-sensitivity` (`b4867dc`). That branch's stronger `MatVal` renderer, the `NumVal` renderer,
`extend_2(0, 0)` and the 1- and 3-element `Rfinal` ranges are ported; its other cases were already covered.
Cases are listed in the [DimVal](../evidence/Mcd.DimVal.md), [Dim](../evidence/Mcd.Dim.md),
[NumVal](../evidence/Mcd.NumVal.md) and [RngVal](../evidence/Mcd.RngVal.md) evidence.

Accepted on this branch (reviewed, user-approved, in `accepted-mutants.txt` via the acceptance commit): the 19
`DynCall`/`VecCall` actual-arity mutants formerly pending here, the `Dim` type initializer's `Hashtable(64)`
capacity and the two `NumVal.ToString` simplify flags. No earlier proposal remains pending.

Local run: `validate.py --profile focused --method-type` `Quire.Mcd.DimVal`, `NumVal`, `Dim`, `Iterator`, `RngVal`,
`RngValInterpolate`, `Dim+Hash` `--mutate` with the proposals below: Harness 36, Mpl 159 and Differential 83
passed, no failures or skips; lint 0; ratchet ok; 258 compared, 0 uncovered, 3 missing (the abstract
`DimVal` and `Iterator` constructors, the `ApplyMatVal` stub); mutation 616/643 killed, 22 accepted, 5 proposed,
0 other survivors. Verdict: `LOCAL CHECKS: pass · GATE: would block (5 proposed mutants pending approval; 47
proposed unreachable lines pending approval)`, `passes_with_proposals` true. Scoped; the gate validates all.

Next: independent review of the proposals below, user approval, maintainer acceptance, then the runtime gate.

### Pending unreachable-line proposals (47)

Each line follows a call that always throws (`AppException.Raise`, `OnError`, `BadCall(-1, …)`,
`WrongNumberOfArguments`); the original's IL ends the same way (the throwing call, then `pop` where it returns
a value, the default value and `ret`), checked in the reference IL for each method.

```text
Quire.Mcd.Dim::Id`0([Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.Dim|True src/Quire.Mpl/Mcd/Dim.cs:193 sha256=67b0c8134efc8019102087dea2ab8742e0e99d1fdc9fcb3a2e0352ec9570fe10 # after Dim.OnError(string, Dim, uint), which always throws; the original's Id ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.Dim::Eq`0([Quire.Mpl]Quire.Mcd.Dim,[Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.Dim|True src/Quire.Mpl/Mcd/Dim.cs:212 sha256=cd10ec1024714f1f3fffb9e211afb305a25dbf5d91bbd5fef37a28442479feca # after Dim.OnError(string, Dim, Dim, uint), which always throws; the original's Eq ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.String|False src/Quire.Mpl/Mcd/DimVal.cs:147 sha256=89362eef95a3fc84cd4a9e2b3383bf137f72b2476fe00cfdce3eda128229456c # after AppException.Raise, which always throws; the original's asString ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.String|False src/Quire.Mpl/Mcd/DimVal.cs:148 sha256=9c4c523196cf610e21896c0fd194ed4dd829850b1844817418ac03e970e98cb1 # after AppException.Raise, which always throws; the original's asString ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.Double|False src/Quire.Mpl/Mcd/DimVal.cs:160 sha256=9fd06e7d074418b6ad8827a36c6e17a9bda575229384571841f8881648138b8d # after AppException.Raise, which always throws; the original's asReal ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.Double|False src/Quire.Mpl/Mcd/DimVal.cs:161 sha256=2059778228989aaf58b4fc19e6340e5a7036404929e35f43b792a160b3ecee69 # after AppException.Raise, which always throws; the original's asReal ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.Int32|False src/Quire.Mpl/Mcd/DimVal.cs:180 sha256=6c43e72a50a78a07fa04f2a4f29fcb1db2efec77262fea7d5abf6306b7cbc999 # after AppException.Raise, which always throws; the original's asInt ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:232 sha256=89362eef95a3fc84cd4a9e2b3383bf137f72b2476fe00cfdce3eda128229456c # after AppException.Raise, which always throws; the original's asVector ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:233 sha256=9c4c523196cf610e21896c0fd194ed4dd829850b1844817418ac03e970e98cb1 # after AppException.Raise, which always throws; the original's asVector ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asRealVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:245 sha256=89362eef95a3fc84cd4a9e2b3383bf137f72b2476fe00cfdce3eda128229456c # after AppException.Raise, which always throws; the original's asRealVector ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asRealVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:246 sha256=9c4c523196cf610e21896c0fd194ed4dd829850b1844817418ac03e970e98cb1 # after AppException.Raise, which always throws; the original's asRealVector ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asRealMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:258 sha256=89362eef95a3fc84cd4a9e2b3383bf137f72b2476fe00cfdce3eda128229456c # after AppException.Raise, which always throws; the original's asRealMatrix ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asRealMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:259 sha256=9c4c523196cf610e21896c0fd194ed4dd829850b1844817418ac03e970e98cb1 # after AppException.Raise, which always throws; the original's asRealMatrix ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:271 sha256=89362eef95a3fc84cd4a9e2b3383bf137f72b2476fe00cfdce3eda128229456c # after AppException.Raise, which always throws; the original's asMatrix ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.MatVal|False src/Quire.Mpl/Mcd/DimVal.cs:272 sha256=9c4c523196cf610e21896c0fd194ed4dd829850b1844817418ac03e970e98cb1 # after AppException.Raise, which always throws; the original's asMatrix ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asTuple`0([System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object[]|False src/Quire.Mpl/Mcd/DimVal.cs:279 sha256=7426e74054557ea2cdef4e5dfe60334ebc55dfcad5cd68ab62038bea77dbe349 # after AppException.Raise, which always throws; the original's asTuple ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asTuple`0([System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object[]|False src/Quire.Mpl/Mcd/DimVal.cs:280 sha256=f8a4cff47a6c709494b4f11a4c6411ec79455a9a7d91e411133b2bdf8ab043ec # after AppException.Raise, which always throws; the original's asTuple ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asRange`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.RngVal|False src/Quire.Mpl/Mcd/DimVal.cs:292 sha256=89362eef95a3fc84cd4a9e2b3383bf137f72b2476fe00cfdce3eda128229456c # after AppException.Raise, which always throws; the original's asRange ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asRange`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mcd.RngVal|False src/Quire.Mpl/Mcd/DimVal.cs:293 sha256=9c4c523196cf610e21896c0fd194ed4dd829850b1844817418ac03e970e98cb1 # after AppException.Raise, which always throws; the original's asRange ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asFunction`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mpl.Run.Function|False src/Quire.Mpl/Mcd/DimVal.cs:306 sha256=5fb3202abd1c1447bbbfe33a891049fa543ed64d836cea8290843798cbde3206 # after OnError(uint, string, object), which always throws; the original's asFunction ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asFunction`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mpl.Run.Function|False src/Quire.Mpl/Mcd/DimVal.cs:307 sha256=6e42aa0024b3a56aa2bdef34af3ea6e2ad293c5de6ca38d165fa91f0d25d9102 # after OnError(uint, string, object), which always throws; the original's asFunction ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asFunctionN`0([System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mpl.Run.Function|False src/Quire.Mpl/Mcd/DimVal.cs:319 sha256=c7a2d60c3690aefb81a7c01013e457e1601e742190829f95e44e8ed7a0e1bf6c # after BadCall(-1, argc, errpos), which raises bad_function, which always throws; the original's asFunctionN ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::asFunctionN`0([System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mpl.Run.Function|False src/Quire.Mpl/Mcd/DimVal.cs:320 sha256=86efc2d3cc8f51b7a29facf61d576046160abff2d31cec88f4b42b80bfde51ea # after BadCall(-1, argc, errpos), which raises bad_function, which always throws; the original's asFunctionN ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::matrix_get_1`0([System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:335 sha256=e2e2f329f10213c3c016d065708c58d0fa7ddfaff86217f399e4b9c36fd62192 # after AppException.Raise, which always throws; the original's matrix_get_1 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::matrix_get_1`0([System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:336 sha256=24bca2166021e64912ca05b2814b6b4168ce36a051f51aaead25fbaed1d81538 # after AppException.Raise, which always throws; the original's matrix_get_1 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::sym_matrix_get_1`0([System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:342 sha256=e2e2f329f10213c3c016d065708c58d0fa7ddfaff86217f399e4b9c36fd62192 # after AppException.Raise, which always throws; the original's sym_matrix_get_1 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::sym_matrix_get_1`0([System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:343 sha256=24bca2166021e64912ca05b2814b6b4168ce36a051f51aaead25fbaed1d81538 # after AppException.Raise, which always throws; the original's sym_matrix_get_1 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:349 sha256=0a0e7929bf69cdf5532472acb91786c5613e564102bd74f037ab4f32f36d8a3e # after AppException.Raise, which always throws; the original's matrix_get_2 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:350 sha256=ad40a4e55f187089f645ecfbc5a0aeaac0f572de68829d229039161cd2e32be3 # after AppException.Raise, which always throws; the original's matrix_get_2 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::sym_matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:356 sha256=0a0e7929bf69cdf5532472acb91786c5613e564102bd74f037ab4f32f36d8a3e # after AppException.Raise, which always throws; the original's sym_matrix_get_2 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::sym_matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:357 sha256=ad40a4e55f187089f645ecfbc5a0aeaac0f572de68829d229039161cd2e32be3 # after AppException.Raise, which always throws; the original's sym_matrix_get_2 ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::Apply`0([System.Runtime]System.Object[])->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:373 sha256=f6ea1725bd25e9bb33853640d0b05d82f6f6f4720e15888c063995656494c534 # after OnError(uint, string, object), which always throws; the original's Apply ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::Apply`0([System.Runtime]System.Object[])->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:374 sha256=8da63dd78ff7869dc003af48ef5f2a9bc442ce43ef1fc2becb66133e593de37c # after OnError(uint, string, object), which always throws; the original's Apply ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|True src/Quire.Mpl/Mcd/DimVal.cs:401 sha256=c9e20262074957b0ab71bf56656acc6c5e93ef5df996be39e0ee6cd8581df77d # after OnError(errpos, ...) on every branch, which always throws; the original's WrongNumberOfArguments ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|True src/Quire.Mpl/Mcd/DimVal.cs:405 sha256=7a3b79e9aa1f05814a0932237e5e297ce3dee5a937d66de1d86f97fd323f78ef # after OnError(errpos, ...) on every branch, which always throws; the original's WrongNumberOfArguments ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|True src/Quire.Mpl/Mcd/DimVal.cs:406 sha256=7d2c670765c33a02c5ccab33d68d106480368691da3dbbb62142f53480dc387b # after OnError(errpos, ...) on every branch, which always throws; the original's WrongNumberOfArguments ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|True src/Quire.Mpl/Mcd/DimVal.cs:410 sha256=d99bf72a5d9605fa21e13262360ed46a2a3584507ffb5f868fdfff780946ccfe # after OnError(errpos, ...) on every branch, which always throws; the original's WrongNumberOfArguments ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|True src/Quire.Mpl/Mcd/DimVal.cs:411 sha256=ed80f404840f1546935b5aec7c4994fb41c799ddbe5305738762ffbb9b914dff # after OnError(errpos, ...) on every branch, which always throws; the original's WrongNumberOfArguments ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|True src/Quire.Mpl/Mcd/DimVal.cs:412 sha256=1eef6b6db9c037606294898f683c84140824eaf915fb95d79598f25246df023d # after OnError(errpos, ...) on every branch, which always throws; the original's WrongNumberOfArguments ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:424 sha256=929faa1cc85c5a2f4ce165d0fbedaf4802bcced0f827c63bc3c40724cef5b88a # after WrongNumberOfArguments(int, int, uint), which raises on every branch, which always throws; the original's BadCall ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:425 sha256=f5b3a74de7ea866912eb1f69e2893700279f6791f38149e8cf7e461f1b461a70 # after WrongNumberOfArguments(int, int, uint), which raises on every branch, which always throws; the original's BadCall ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::DynCall`0()->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:434 sha256=94003c636b5806a1002156ecb2727265babad66f37bccf9b203bc987a96104e7 # after AppException.Raise(string), which always throws; the original's DynCall ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.DimVal::DynCall`0()->[System.Runtime]System.Object|False src/Quire.Mpl/Mcd/DimVal.cs:435 sha256=7b988a9c6441425282a0285df0fd7df407e7ce67cc7ba4a8f9352c242376dcbf # after AppException.Raise(string), which always throws; the original's DynCall ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.NumVal::LessThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True src/Quire.Mpl/Mcd/NumVal.cs:397 sha256=c9c1ee16ae175f8020358eea2ec2909d392b0ed69f96ec569f601b5afe05d401 # after AppException.Raise, which always throws; the original's LessThan ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.NumVal::GreaterThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True src/Quire.Mpl/Mcd/NumVal.cs:413 sha256=bec4c7832a53eb1f0118b22032e7f8819a2421c97b5cc2acd6345e6590cb24e6 # after AppException.Raise, which always throws; the original's GreaterThan ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.NumVal::LessEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True src/Quire.Mpl/Mcd/NumVal.cs:429 sha256=6be31b42bb4aab1ae9726d0ef7284cefb136ed79941032660a64852f8cace527 # after AppException.Raise, which always throws; the original's LessEqThan ends the same way (throwing call, then the default return), so the line never executes there either
Quire.Mcd.NumVal::GreaterEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True src/Quire.Mpl/Mcd/NumVal.cs:445 sha256=89250bad30b18e2a476ac8751a27004aaaef40dc9650b349e6f99c96fa6124bb # after AppException.Raise, which always throws; the original's GreaterEqThan ends the same way (throwing call, then the default return), so the line never executes there either
```

### Pending mutant proposals (5)

The mutated constant is the default return on one of the unreachable lines above.

```text
Quire.Mcd.DimVal::asInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.Int32|False@IL_001e ldc.i4.0 -> ldc.i4.1 # the default return 0 after AppException.Raise("must be integer", a), which always throws; the original ends the same way (Raise; pop; ldc.i4.0; ret), so the constant is never returned
Quire.Mcd.NumVal::LessThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True@IL_00c2 ldc.i4.0 -> ldc.i4.1 # the default return false after AppException.Raise("incomparable"), which always throws; the original ends the same way (Raise; pop; ldc.i4.0; ret), so the constant is never returned
Quire.Mcd.NumVal::GreaterThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True@IL_00c2 ldc.i4.0 -> ldc.i4.1 # the default return false after AppException.Raise("incomparable"), which always throws; the original ends the same way (Raise; pop; ldc.i4.0; ret), so the constant is never returned
Quire.Mcd.NumVal::LessEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True@IL_00c2 ldc.i4.0 -> ldc.i4.1 # the default return false after AppException.Raise("incomparable"), which always throws; the original ends the same way (Raise; pop; ldc.i4.0; ret), so the constant is never returned
Quire.Mcd.NumVal::GreaterEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True@IL_00c2 ldc.i4.0 -> ldc.i4.1 # the default return false after AppException.Raise("incomparable"), which always throws; the original ends the same way (Raise; pop; ldc.i4.0; ret), so the constant is never returned
```

Updated: 2026-10-15. Phase 1 remains open; Windows phase closure has not run.
Read [PLAN](../PLAN.md) before starting a batch.

## Ownership

Lane A owns the S-expression family and runtime helpers (`Cons`, `VecBuilder`, `True`, `Tuple`,
`MplSignature`), `Rational`, `Utilities`, `AppException`, `Type`, `UnitSystem`, `RuntimeOptions`,
and remaining runtime batch members outside Lane B. Lane B owns `MatVal*`, `MatDim`, `Function*`,
`FunctionUtilities`, `DimVal.ApplyMatVal`, and `Quire.Engine`.

## Tok.walk_pattern batch

Merged to `main` at commit `71c16c5dc2fb` after passing independent review and the runtime merge gate (Harness 36, Mpl 160, Differential 88, Mutation 110/111 killed, 0 survivors).

## SCons.walk_pattern batch

Merged to `main` at commit `92e763bf581a` after passing independent review and the runtime merge gate.

## Environment helpers batch (mirror/a-env-helpers)

- Branch: `mirror/a-env-helpers`
- Worktree: `/root/src/quire-env-helpers`
- Scope: Implementation of `MplEnv`, `BindingInfo`, `MacroBindingInfo`, `LocalEnv` (leaving `AddFunctionMacro` and `AddForeignMacro` stubs for subsequent batches), `MplMacro`, and `MplSymbolMacro` in `src/Quire.Mpl/Run/Env.cs`.
- Contracts & Behavior:
  - `Mpl.Run.MplEnv`: `Level => 0`; base mutation methods (`AddLocal`, `AddLocalEquation`, `AddSymbolMacro`, `AddFunctionMacro`, `AddForeignMacro`) reject modifications by calling `MayNotModify()`; `LoadMacro(Sym)` returns `null` for `Keyword` symbols and otherwise delegates to `lookup(s)?.macro`; `LoadForeignMacro` calls `CanNotResolve(sig)`; `CanNotResolve` throws `AppException("Can not resolve signature")`; `MayNotModify` throws `AppException("May not modify import environment")`; protected parameterless constructor.
  - `Mpl.Run.BindingInfo`: `m_handle`, `m_zone`, `macro => null`, constructors `BindingInfo()`, `BindingInfo(uint)`, `BindingInfo(uint, int)`. Constants `Value = 0` and `Macro = 1` preserved in `.g.cs`. Subclass `MacroBindingInfo`: `m_macro`, `macro => m_macro`, constructor `(handle, macro)` calling `base(handle, 1)`.
  - `Mpl.Run.LocalEnv`: `m_level`, `m_parent`, `m_table` (`SortedDictionary<string, BindingInfo>`); `Level => m_level`; constructors `LocalEnv(parent)` and `LocalEnv()`; `lookup_local` searches `m_table`; `lookup` queries `lookup_local` then delegates to `m_parent.lookup` (null parent throws `NullReferenceException` on miss); `LoadForeignMacro` delegates to `m_parent.LoadForeignMacro`; `AddLocal` adds new binding and throws `ArgumentException` on duplicates; `AddLocalEquation` sets/overwrites binding; `AddSymbolMacro` registers `MacroBindingInfo` wrapping `MplSymbolMacro`.
  - `Mpl.Run.MplMacro`: `m_name`, constructor `(sym)`, default `ExpandRef` / `ExpandCall` calling `IllegalMacroRef` / `IllegalMacroCall` throwing `AppException("illegal macro reference" / "illegal macro call", 0u, handle)`; static `NotAtTopLevel` and `CanNotExpand` throwing `AppException`; `ToString() => m_name.ToString()`.
  - `Mpl.Run.MplSymbolMacro`: `m_replacement`, constructor `(s, rep)` calling `base(s)`, `ExpandRef => m_replacement`, `ExpandCall` replaces head with `m_replacement` while preserving tail, static `make(Atom s, int n)` wrapping `int_constant.NewTok`, `ToString() => "MplSymbolMacro: " + m_name + " " + m_replacement`.
- Evidence files:
  - [Mpl.Run.MplEnv](../evidence/Mpl.Run.MplEnv.md)
  - [Mpl.Run.BindingInfo](../evidence/Mpl.Run.BindingInfo.md)
  - [Mpl.Run.LocalEnv](../evidence/Mpl.Run.LocalEnv.md)
  - [Mpl.Run.MplMacro](../evidence/Mpl.Run.MplMacro.md)
  - [Mpl.Run.MplSymbolMacro](../evidence/Mpl.Run.MplSymbolMacro.md)
- Validation receipts:
  - `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 166 passed, 0 failed, 0 skipped.
  - `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 94 passed, 0 failed, 0 skipped.
  - `git diff --check` — passed cleanly.
  - `python3 tools/validation/merge_gate.py mirror/a-env-helpers` — passed (90/90 mutants evaluated, 0 unaccepted survivors, ratchet ok, 0 lint findings).
- Unblocks: direct comparisons and full coverage of lowercase symbol handling in all AST walk methods (`Tok.walk_expression`, `Tok.walk_pattern`, `SCons.walk_pattern`, `SCons.walk_expression`, `Sym.walk_*`).

## Tok.walk_expression batch (mirror/a-tok-expression)

- Branch: `mirror/a-tok-expression`
- Worktree: `/root/src/quire-tok-expression`
- Scope: Verification of `Tok.walk_expression(MplEnv)` in `src/Quire.Mpl/Run/Sexpr.cs`.
- Contracts & Behavior:
  - `Mpl.Run.Tok.walk_expression(MplEnv env)`:
    - If `Key == null || !Key.LowId`: returns `this` immediately without environment lookup.
    - If `Key.LowId`: loads macro via `env.LoadMacro(Key)`.
    - If macro is null: returns `this`.
    - If macro is present: returns `macro.ExpandRef(env, this).walk_expression(env)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600006e.
  - Exception and handle propagation:
    - When `ExpandRef` is called on an `MplMacro` that does not override `ExpandRef` (function macro), it invokes base `MplMacro.ExpandRef`, throwing `AppException("illegal macro reference", 0u, handle)`.
    - Unbound lowercase symbols in empty `LocalEnv()` throw `NullReferenceException` due to null parent dereference on miss, matching Mathcad.
    - Empty or null symbol names throw `ArgumentOutOfRangeException` / `ArgumentNullException` from `LowId` (`char.IsLower`), matching Mathcad.
- Evidence files:
  - [Mpl.Run.Tok](../evidence/Mpl.Run.Tok.md): `walk_expression` updated to `Verified`.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 172 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 93 passed, 0 failed, 0 skipped.
  - Direct comparisons added: bound symbol macro, chained symbol macros, bound function macro (`AppException` "illegal macro reference"), scoped `LocalEnv(parent)` resolution and shadowing, bound local variable returning `this`, and enabled lowercase symbol comparison in empty `LocalEnv()`.
  - Format checks: `git diff --check` passed cleanly.

## SCons.walk_expression and Sym.walk_expression_as batch (mirror/a-scons-expression)

- Branch: `mirror/a-scons-expression`
- Worktree: `/root/src/quire-scons-expression`
- Scope: Verification of `SCons.walk_expression(MplEnv)`, `SCons.walk_expressions(MplEnv, SCons)` in `src/Quire.Mpl/Run/Sexpr.cs`, and `Sym.walk_expression_as(MplEnv, SCons)` in `src/Quire.Mpl/Run/Atom.cs`.
- Contracts & Behavior:
  - `Mpl.Run.Sym.walk_expression_as(MplEnv env, SCons form)`:
    - If `LowId`: loads macro via `env.LoadMacro(this)`.
    - If macro is present: invokes `macro.ExpandCall(env, form).walk_expression(env)`.
    - If macro is null (or !LowId): walks arguments with `SCons.walk_expressions(env, form.tail)`. If arguments changed, returns `new SCons(form.head, expr)`; if unchanged, returns `form`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000097. Attribute updated to `[MirrorVerified("SexprDifferentialTests")]`.
  - `Mpl.Run.SCons.walk_expression(MplEnv env)`:
    - If `head.is_sym`: delegates to `head.sym.walk_expression_as(env, this)`.
    - If `head` is not a symbol (e.g. integer constant, nested list): wraps as `(apply e1 e2 ...)` via `new SCons(new Tok(apply_sym, head.handle), this).walk_expression(env)`.
    - Null head throws `NullReferenceException` when checking `head.is_sym`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600004b. Already `[MirrorVerified]`.
  - `Mpl.Run.SCons.walk_expressions(MplEnv env, SCons exps)`:
    - If `exps == null`: returns `Nul.singleton`.
    - Walks `exps.head.walk_expression(env)` and `walk_expressions(env, exps.tail)`. If neither changed (by reference equality), returns `exps` unchanged; otherwise constructs and returns `new SCons(expr, expr2)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600004a. Already `[MirrorVerified]`.
  - Exception and handle propagation:
    - `ExpandCall` on base `MplMacro` (or function macro not overriding `ExpandCall`) invokes `IllegalMacroCall`, throwing `AppException("illegal macro call", 0u, form.handle)`.
    - Unbound lowercase symbols in empty `LocalEnv()` throw `NullReferenceException` due to null parent dereference on miss, matching Mathcad.
    - Non-symbol heads preserve source handle in generated `apply` token (`head.handle`).
- Evidence files:
  - [Mpl.Run.Sym](../evidence/Mpl.Run.Sym.md): `walk_expression_as` updated to `Verified`.
  - [Mpl.Run.SCons](../evidence/Mpl.Run.SCons.md): `walk_expression` and `walk_expressions` verified.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 181 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 100 passed (focused on SexprDifferentialTests: 9 passed, 0 failed, 0 skipped).
  - Direct comparisons added:
    - `Sym_members`: bound local (unchanged vs changed args), bound symbol macro, chained symbol macros, function macro throwing `AppException("illegal macro call", 0u, form.handle)`, nested `LocalEnv(parent)` scoping and shadowing.
    - `SCons_members`: symbol macro call, function macro call throwing `AppException("illegal macro call")`, non-symbol heads `(1 2 3)` and `((a b) c)` wrapping in `apply` token, `walk_expressions` returning new chain when macro args change.
  - Format checks: `git diff --check` passed cleanly.

## SCons.walk_pattern, SCons.walk_declaration_lhs, Sym.walk_declaration_as, and Tok.walk_pattern batch (mirror/a-scons-pattern-macro)

- Branch: `mirror/a-scons-pattern-macro`
- Worktree: `/root/src/quire-scons-pattern-macro`
- Scope: Verification of `Tok.walk_pattern(MplEnv)`, `SCons.walk_declaration_lhs(MplEnv)`, and `SCons.walk_pattern(MplEnv)` in `src/Quire.Mpl/Run/Sexpr.cs`, and `Sym.walk_declaration_as(MplEnv, SCons)` in `src/Quire.Mpl/Run/Atom.cs`.
- Contracts & Behavior:
  - `Mpl.Run.Tok.walk_pattern(MplEnv env)`:
    - If `Key == null`: returns `this` immediately without environment lookup.
    - If `Key.UpId`: loads macro via `env.LoadMacro(Key)`. If macro is null, returns `this`; if present, returns `macro.ExpandRef(env, this).walk_pattern(env)`.
    - If `!Key.UpId`: registers local via `env.AddLocal(Key, handle)` and returns `this`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x0600006d. Attribute updated to `[MirrorVerified("SexprDifferentialTests")]`.
  - `Mpl.Run.SCons.walk_pattern(MplEnv env)`:
    - If `Key == null`: calls `BadPattern(this)` throwing `AppException("bad pattern", 0u, handle)`.
    - If `Key.LowId`: loads macro via `env.LoadMacro(Key)`. If macro is null and `Length == 1`, calls `env.AddLocal(Key, handle)` and returns `this`; if `Length != 1`, calls `BadPattern(this)` throwing `AppException("bad pattern", 0u, handle)`. If macro is present, returns `macro.ExpandCall(env, this).walk_pattern(env)`.
    - If `!Key.LowId`: returns `new SCons(head, walk_patterns(env, tail))`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000049. Attribute set to `[MirrorTested("Mpl.Run.SCons")]` (uncovered sequence points follow the `BadPattern` throwing helper).
  - `Mpl.Run.SCons.walk_declaration_lhs(MplEnv env)`:
    - If `Key == null`: calls `BadDeclaration(this)` throwing `AppException("illegal declaration", 0u, handle)`.
    - If `Key != null`: delegates to `Key.walk_declaration_as(env, this)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000047. Attribute set to `[MirrorTested("Mpl.Run.SCons")]`.
  - `Mpl.Run.Sym.walk_declaration_as(MplEnv env, SCons form)`:
    - Loads macro via `env.LoadMacro(this)`.
    - If macro is null: calls `Sexpr.BadDeclaration(form)` throwing `AppException("illegal declaration", 0u, form.handle)`.
    - If macro is present: returns `macro.ExpandCall(env, form).walk_declaration_lhs(env)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000095. Attribute retained as `[MirrorAnalyzed("Mpl.Run.Sym")]` pending full implementation of `MplQuoteMacro` in the Quote macro / Keyword batch.
  - Exception and handle propagation:
    - `ExpandCall` on base `MplMacro` (or function macro without `ExpandCall` override) invokes `IllegalMacroCall`, throwing `AppException("illegal macro call", 0u, form.handle)`.
    - `ExpandRef` on base `MplMacro` (or function macro without `ExpandRef` override) invokes `IllegalMacroRef`, throwing `AppException("illegal macro reference", 0u, handle)`.
    - `BadPattern` and `BadDeclaration` propagate the form/token handle accurately.
    - Unbound lowercase symbols in empty `LocalEnv()` throw `NullReferenceException` due to null parent dereference on miss, matching Mathcad.
- Evidence files:
  - [Mpl.Run.Tok](../evidence/Mpl.Run.Tok.md): `walk_pattern` updated to `Verified`.
  - [Mpl.Run.SCons](../evidence/Mpl.Run.SCons.md): `walk_pattern` updated to `Tested`, `walk_declaration_lhs` updated to `Tested`.
  - [Mpl.Run.Sym](../evidence/Mpl.Run.Sym.md): `walk_declaration_as` updated to `Analyzed`.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 185 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 104 passed (focused on SexprDifferentialTests: 9 passed, 0 failed, 0 skipped).
  - Direct comparisons added:
    - `Sym_members`: `Sym.walk_declaration_as` with no macro (`AppException("illegal declaration", 0u, form.handle)`), bound function macro (`AppException("illegal macro call", 0u, form.handle)`), and symbol macros expanding calls to non-symbol and un-macro'd symbol heads (`AppException("illegal declaration", 0u, rep.handle)`).
    - `SCons_members`: `SCons.walk_declaration_lhs` across no-macro symbol heads (`AppException("illegal declaration")`), function macro heads (`AppException("illegal macro call")`), and symbol macro expanding to integer heads; `SCons.walk_pattern` across single-element lowercase symbols in `LocalEnv()` (registers local and returns `this`), multi-element lowercase symbols with no macro (`AppException("bad pattern")`), lowercase symbol macros expanding to uppercase and integer heads, and function macro heads (`AppException("illegal macro call")`).
    - `Tok_members`: `Tok.walk_pattern` across uppercase symbols with macro bound (macro replacement walked), uppercase symbols without macro (returns `this`), lowercase symbols in `LocalEnv()` (registers local and returns `this`), and non-symbol tokens (int, str).
  - Format checks: `git diff --check` passed cleanly.

## DimVal, NumVal and RngVal caller sensitivity batch (mirror/a-caller-sensitivity)

- Branch: `mirror/a-caller-sensitivity`
- Worktree: `/root/src/quire-caller-sensitivity`
- Scope: `tests/Quire.Differential.Tests/CallerSensitivityDifferentialTests.cs` (isolated test suite).
- Objective: Eliminate all 15 latent caller survivors in `DimVal` (`get_IsDimlessReal`, `asDimlessReal`, `matrix_set0_1`, `matrix_set0_2`), `NumVal` (`Format`, `asResult`), and `RngVal` (`asRealVector`, `GetHashCode`) that blocked Lane B integration of `mirror/b-matval-dimension-arrays` and other pointer/array batches.
- Design:
  - Placed in a dedicated test class (`CallerSensitivityDifferentialTests`) rather than modifying `DimValDifferentialTests.cs`, ensuring `validate.py --mutate-since` does not select `DimValDifferentialTests` or unapproved `DimVal.DynCall`/`VecCall` mutants.
  - Detailed matrix element formatting: `ShowVal` delegates matrix formatting to `MatValDifferentialTests.Storage(side, v)`, making differential assertions sensitive to matrix dimensions, strides, flags, and individual real/imag/pointer array elements (killing `matrix_set0_1` and `matrix_set0_2` `ldc.r8 0 -> 1` mutants).
  - Value classifier sensitivity: Added direct comparisons on `NumVal` (dimensionless real, dimensioned real, complex) and `StrVal` for `get_IsDimlessReal` and `asDimlessReal` overloads (killing all 6 `IsDimlessReal` and `asDimlessReal` mutants).
  - Loop-start sensitivity: Added non-zero initial range test case (`MakeRange(2.5, 1.0, 3, "L")`) for `RngVal.asRealVector` to kill `i = 0 -> 1` loop-start mutations and `clt`/`brtrue` mutants.
  - Range hash sensitivity: Added multi-range `GetHashCode` comparisons on non-zero, negative, and dimensioned ranges to kill `RngVal.GetHashCode` `xor -> or` mutant.
  - Unit magnitude sensitivity: Applied a thread-local US unit system (`ft` magnitude 0.3048) in direct comparisons of `NumVal.Format(IFormatProvider, string, bool, bool)` to kill `re /= u.magnitude` and `im /= u.magnitude` `div -> mul` mutants.
  - Result identity sensitivity: Added reference-identity sensitive comparison for `NumVal.asResult` to kill `m_dim != null` `cgt.un -> clt.un` mutant.
- Validation receipts:
  - Scoped mutation on `RngVal` (`/tmp/quire-validation-w9545cf9/summary.json`): 88/88 killed (0 survivors; all 3 `asRealVector` and `GetHashCode` mutants killed).
  - Scoped mutation on `NumVal` (`/tmp/quire-validation-76eozpn9/summary.json`): 182/195 killed (0 survivors in `Format` and `asResult`; all 3 target mutants killed).
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 108 passed, 0 failed, 0 skipped (all 4 new facts passed).
  - Merge gate dry-run (`validate.py --mutate-since e9d973e61e9a`): Ratchet ok, 0 unaccepted survivors, gate would pass.
  - Format checks: `git diff --check` passed cleanly.

## MplQuoteMacro mirror and SCons.walk_declaration_lhs / Sym.walk_declaration_as verification batch (mirror/a-quote-macro-decl)

- Branch: `mirror/a-quote-macro-decl`
- Worktree: `/root/src/quire-quote-macro-decl`
- Scope: Implementation of `MplQuoteMacro` (type token `0x0200002b`, fields `singleton` `0x0400005f`, ctor `()` `0x06000133`, cctor `()` `0x06000134`, `ExpandCall(MplEnv, Sexpr)` `0x06000135`) in `src/Quire.Mpl/Run/Env.cs` and deletion of stubs in `src/Quire.Mpl/Generated/Mirror/Mpl.Run.MplQuoteMacro.g.cs`, and verification of `SCons.walk_declaration_lhs(MplEnv)` (token `0x06000047`) in `src/Quire.Mpl/Run/Sexpr.cs` and `Sym.walk_declaration_as(MplEnv, SCons)` (token `0x06000095`) in `src/Quire.Mpl/Run/Atom.cs`.
- Contracts & Behavior:
  - `Mpl.Run.MplQuoteMacro`:
    - Inherits `MplMacro(new Sym("quote", null))`.
    - `public static MplQuoteMacro singleton;` initialized in static constructor.
    - `private MplQuoteMacro() : base(new Sym("quote", null)) { }`.
    - `ExpandCall(MplEnv e, Sexpr f)`:
      - Casts `f as SCons`. If `scons.Length != 2`, invokes `IllegalMacroCall(sCons)`, throwing `AppException("illegal macro call", 0u, sCons.handle)`. Non-SCons or null input dereferences null on `scons.Length`, throwing `NullReferenceException`, matching Mathcad's single conditional branch.
      - If valid length 2, returns `new Qt(sCons.second)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000135 (normalized IL `bac33f95`). Attributes set to `[MirrorVerified("EnvDifferentialTests")]`.
  - `Mpl.Run.Sym.walk_declaration_as(MplEnv env, SCons form)`:
    - Loads macro via `env.LoadMacro(this)`.
    - If macro is null, calls `Sexpr.BadDeclaration(form)`, throwing `AppException("illegal declaration", 0u, form.handle)`.
    - If macro is present, returns `macro.ExpandCall(env, form).walk_declaration_lhs(env)`.
    - When macro is `MplQuoteMacro`, `ExpandCall` returns `new Qt(form.second)`. `Qt.walk_declaration_lhs(env)` returns `null`, so `Sym.walk_declaration_as` returns `null`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000095. Attribute updated to `[MirrorVerified("SexprDifferentialTests")]`.
  - `Mpl.Run.SCons.walk_declaration_lhs(MplEnv env)`:
    - If `Key == null`, calls `BadDeclaration(this)`, throwing `AppException("illegal declaration", 0u, handle)`.
    - If `Key != null`, delegates to `Key.walk_declaration_as(env, this)`.
    - When `Key.walk_declaration_as` returns `null` (such as with `MplQuoteMacro`), `SCons.walk_declaration_lhs` returns `null`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000047. Attribute updated to `[MirrorVerified("SexprDifferentialTests")]`.
- Evidence files:
  - [Mpl.Run.MplQuoteMacro](../evidence/Mpl.Run.MplQuoteMacro.md): Created with `Verified` status across all members.
  - [Mpl.Run.SCons](../evidence/Mpl.Run.SCons.md): `walk_declaration_lhs` updated from `Tested` to `Verified`.
  - [Mpl.Run.Sym](../evidence/Mpl.Run.Sym.md): `walk_declaration_as` updated from `Analyzed` to `Verified`.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 186 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 111 passed, 0 failed, 0 skipped (`EnvDifferentialTests`: 6 passed; `SexprDifferentialTests`: 9 passed).
  - Direct comparisons added:
    - `EnvDifferentialTests.MplQuoteMacro_members`: `.ctor()`, `.cctor()`, `singleton` identity, `ExpandCall` on valid length 2 list `(quote x)` returning `Qt(x)`, invalid length 3 list `(quote x y)` throwing `AppException("illegal macro call", 0u, handle)`, invalid length 1 list `(quote)` throwing `AppException("illegal macro call", 0u, handle)`, non-SCons token throwing `NullReferenceException`, and null form throwing `NullReferenceException`.
    - `SexprDifferentialTests.Sym_members`: `Sym.walk_declaration_as` with bound `MplQuoteMacro.singleton` returning `null` on length 2 `(quote x)`, throwing `AppException("illegal macro call")` on length 3 and length 1 forms.
    - `SexprDifferentialTests.SCons_members`: `SCons.walk_declaration_lhs` with bound `MplQuoteMacro.singleton` returning `null` on length 2 `(quote x)`, throwing `AppException("illegal macro call")` on length 3 and length 1 forms.
  - Type-level audit: `python3 tools/validation/validate.py --method-type Quire.Mpl.Run.MplQuoteMacro`: compared 3, missing 0, structure_mismatch 0, uncovered 0, verified_unsupported 0; Ratchet ok; gate would pass.
  - Format checks: `git diff --check` passed cleanly.
  - Protected paths: `merge_gate.stub_deletions` verified stub removals from `src/Quire.Mpl/Generated/Mirror/Mpl.Run.MplQuoteMacro.g.cs` cleanly without modifying any other protected files.

## Keyword and block_keyword mirror and Sexpr.WalkModule verification batch (mirror/a-keyword-block)

- Branch: `mirror/a-keyword-block`
- Worktree: `/root/src/quire-keyword-block`
- Scope: Implementation of `Keyword` base members (type token `0x02000011`, fields `keyword` `0x04000030`, `key_tok` `0x0400003f`, `m_keynum` `0x0400003e`, ctor `(string, int)` `0x0600009a`, cctor `()` `0x06000099`, `get_KeyNum()` `0x06000098`, `get_quote_atom()` `0x0600009d`, `walk_declaration_as(MplEnv, SCons)` `0x0600009e`, `walk_expression_as(MplEnv, SCons)` `0x0600009f`) and `block_keyword` (type token `0x02000014`, field `singleton` `0x04000040`, cctor `()` `0x060000aa`, ctor `()` `0x060000ab`, `NewTok(uint)` `0x060000ac`, `MakeBlock(uint, Sexpr)` `0x060000ae`, `walk_declaration_as(MplEnv, SCons)` `0x060000ad`) in `src/Quire.Mpl/Run/Atom.cs` with stub deletions in `src/Quire.Mpl/Generated/Mirror/Mpl.Run.Keyword.g.cs` and `src/Quire.Mpl/Generated/Mirror/Mpl.Run.block_keyword.g.cs`, and promoting `Sexpr.WalkModule(MplEnv, Sexpr)` (token `0x0600004e`) in `src/Quire.Mpl/Run/Sexpr.cs` from `[MirrorAnalyzed]` to `[MirrorVerified("SexprDifferentialTests")]`.
- Contracts & Behavior:
  - `Mpl.Run.Keyword`:
    - Inherits `Sym(nm, keyword, 5)`.
    - `public static string keyword;` initialized to `string.Intern("Keyword")` in `.cctor()`.
    - `public static Tok key_tok;` initialized to `Sym.MakeSymQ("Key", "MplMacro")` in `.cctor()`.
    - `m_keynum`: stores integer keyword index.
    - `KeyNum`: returns `m_keynum`.
    - `quote_atom`: returns `SCons.list2(key_tok, int_constant.NewTok(m_keynum, 0u))`.
    - `walk_declaration_as(MplEnv env, SCons form)`: calls `Sexpr.BadDeclaration(form)` which throws `AppException("illegal declaration", 0u, form.handle)`, returns `null`. Marked `[MirrorTested("Mpl.Run.Keyword")]` because `return null;` is unreachable after the throwing call.
    - `walk_expression_as(MplEnv env, SCons form)`: walks expressions via `SCons.walk_expressions(env, form.tail)`. If unchanged returns `form`, otherwise returns `new SCons(form.head, walkedTail)`.
    - `key(int)` and `keytok(int)` remain stubs in `.g.cs` until all 15 keyword singletons are implemented.
  - `Mpl.Run.block_keyword`:
    - Inherits `Keyword("block", 4)`.
    - `public static block_keyword singleton;` initialized in `.cctor()`.
    - `NewTok(uint handle)`: returns `new Tok(singleton, handle)`.
    - `MakeBlock(uint handle, Sexpr forms)`: returns `new SCons(new Tok(singleton, handle), forms)`.
    - `walk_declaration_as(MplEnv env, SCons form)`: if `form.tail != null`, appends `tail.head.walk_declaration_lhs(env)` to recursive `walk_declaration_as(env, tail)`; otherwise returns `form.Field2.walk_declaration_lhs(env)`.
  - `Mpl.Run.Sexpr.WalkModule(MplEnv env, Sexpr block)`:
    - Calls `Cons defs = block.walk(env, out _)`.
    - Returns `new SCons(block_keyword.NewTok(0u), SCons.as_Sexpr(defs))`.
    - Attribute updated from `[MirrorAnalyzed("Mpl.Run.Sexpr")]` to `[MirrorVerified("SexprDifferentialTests")]`.
- Evidence files:
  - [Mpl.Run.Keyword](../evidence/Mpl.Run.Keyword.md): Created with `Verified` status across implemented members, `Tested` on `walk_declaration_as` (unreachable return after throw), and `Stub` on `key`/`keytok`.
  - [Mpl.Run.block_keyword](../evidence/Mpl.Run.block_keyword.md): Created with `Verified` status across all members.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 187 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 116 passed, 0 failed, 0 skipped (`SexprDifferentialTests`: 10 passed, 0 failed).
  - Validation audit: `python3 tools/validation/validate.py --method-type Quire.Mpl.Run.Keyword --method-type Quire.Mpl.Run.block_keyword` — Harness 42, Mpl 187, Differential 116 passed; Method coverage: 10 compared, 0 structure mismatch, 2 missing stubs (`key`/`keytok`), 1 uncovered (`walk_declaration_as`), 0 verified_unsupported; Ratchet: ok; LOCAL CHECKS: pass.
  - Direct comparisons added:
    - `SexprDifferentialTests.Keyword_and_block_keyword_members`: Direct comparisons for `Keyword.cctor`, `Keyword.keyword`, `Keyword.key_tok`, `Keyword(string, int)` constructor across multiple names and keynums, `KeyNum`, `quote_atom`, `walk_declaration_as` throwing `AppException("illegal declaration", 0u, handle)`, `walk_expression_as` across unchanged arguments returning identity and changed arguments returning new SCons; `block_keyword.cctor`, `block_keyword()`, `singleton` identity, `NewTok(uint)` across multiple handles, `MakeBlock(uint, Sexpr)` on empty and populated lists, `walk_declaration_as` on empty block `(block)` and block with quote declaration; `Sexpr.WalkModule(env, Nul)` and `Sexpr.WalkModule(env, (block))`.
  - Format checks: `git diff --check` passed cleanly with 0 findings.
  - Protected paths: `merge_gate.stub_deletions` verified stub removals from `Mpl.Run.Keyword.g.cs` (8 member deletions) and `Mpl.Run.block_keyword.g.cs` (5 member deletions) cleanly without modifying any other protected files.

## Rationalization of MirrorTested attributes for abstract base constructors and unreachable throwing tails (mirror/a-tested-attributes)

- Branch: `mirror/a-tested-attributes`
- Worktree: `/root/src/quire-tested-attributes`
- Scope: Correct status attributes from `[MirrorVerified]` to `[MirrorTested]` across 10 methods in 5 Lane A runtime types (`Atom`, `Sym`, `Sexpr`, `SCons`, `AppException`):
  1. `src/Quire.Mpl/Run/Atom.cs`:
     - `Atom(int tag)`: `[MirrorTested("Mpl.Run.Atom")]` (abstract base constructor cannot be instantiated directly via reflection in `MethodComparison.Compare`).
     - `Sym.walk_definition_as(MplEnv env, SCons form)`: `[MirrorTested("Mpl.Run.Sym")]` (unreachable `return null;` following unconditionally throwing `Sexpr.BadDeclaration(form)`).
  2. `src/Quire.Mpl/Run/Sexpr.cs`:
     - `Sexpr(int tag)`: `[MirrorTested("Mpl.Run.Sexpr")]` (abstract base constructor cannot be instantiated directly in `MethodComparison.Compare`).
     - `Sexpr.quotation`: `[MirrorTested("Mpl.Run.Sexpr")]` (unreachable `return null;` after throwing helper `NotAQuotation(this)`).
     - `Sexpr.quote`: `[MirrorTested("Mpl.Run.Sexpr")]` (unreachable `return null;` after throwing helper `QuotationNotImplemented(this)`).
     - `Sexpr.scons`: `[MirrorTested("Mpl.Run.Sexpr")]` (unreachable `return null;` after throwing helper `ExpectedAList(this)`).
     - `Sexpr.walk_pattern(MplEnv env)`: `[MirrorTested("Mpl.Run.Sexpr")]` (unreachable `return null;` after throwing helper `BadPattern(this)`).
     - `Sexpr.walk_expression(MplEnv env)`: `[MirrorTested("Mpl.Run.Sexpr")]` (unreachable `return null;` after throwing helper `BadExpression(this)`).
     - `SCons.walk_expression(MplEnv env)`: `[MirrorTested("Mpl.Run.SCons")]` (unreachable `BadApplication(this); return null;` fallback because `Key` property dereferences `head` first).
  3. `src/Quire.Mpl/Run/AppException.cs`:
     - `AppException.Raise(object[] msgs, uint handle)`: `[MirrorTested("Mpl.Run.AppException")]` (unreachable method closing brace `}` because every execution path throws via `Raise(text, handle)`).
- Contracts & Behavior:
  - All 10 methods have 100% behavioral test coverage in `Quire.Mpl.Tests` and direct differential test comparisons in `Quire.Differential.Tests`.
  - In decompiled PTC Mathcad Prime 12.0.1.0 `mplrun.dll`, the identical `ldnull; ret` sequence points exist due to C# compiler constraints following void-returning throwing methods, but are physically unreachable at runtime.
  - Per Quire mirror policy and precedents (`Keyword.walk_declaration_as`, `SCons.walk_pattern`, and `Mcd.Type`), methods with full behavioral test coverage whose physical sequence points cannot be reached without maintainer unreachable-line exemptions are properly designated `[MirrorTested]`.
  - Abstract base constructors cannot be invoked directly by `MethodComparison.Compare` via `ConstructorInfo.Invoke(args)` because reflection throws `MemberAccessException: Cannot create an instance of an abstract class`. They are properly designated `[MirrorTested]`.
- Evidence files updated:
  - [Mpl.Run.Atom](../evidence/Mpl.Run.Atom.md): `Atom(int)` status confirmed as `Tested`.
  - [Mpl.Run.Sym](../evidence/Mpl.Run.Sym.md): `walk_definition_as` status updated to `Tested`.
  - [Mpl.Run.Sexpr](../evidence/Mpl.Run.Sexpr.md): `quotation`, `quote`, `scons`, `walk_pattern`, `walk_expression` updated to `Tested`; `WalkModule` confirmed as `Verified`.
  - [Mpl.Run.SCons](../evidence/Mpl.Run.SCons.md): `walk_expression` status updated to `Tested`.
  - [Mpl.Run.AppException](../evidence/Mpl.Run.AppException.md): `Raise(object[], uint)` status updated to `Tested`.

## MplBackquoteMacro mirror batch (mirror/a-backquote-macro)

- Branch: `mirror/a-backquote-macro`
- Worktree: `/root/src/quire-backquote-macro`
- Scope: Implementation of `MplBackquoteMacro` (type token `0x0200002e`, fields `backquote` `0x04000064`, `singleton` `0x04000065`, `m_macro` `0x04000066`, cctor `()` `0x0600013f`, ctor `()` `0x06000140`, `ExpandCall(MplEnv, Sexpr)` `0x06000141`) in `src/Quire.Mpl/Run/Env.cs` and deletion of all 5 matching stubs in `src/Quire.Mpl/Generated/Mirror/Mpl.Run.MplBackquoteMacro.g.cs`.
- Contracts & Behavior:
  - `Mpl.Run.MplBackquoteMacro`:
    - Inherits `MplMacro(backquote)`.
    - `public static readonly Sym backquote;` initialized in `.cctor()` to `new Sym("backquote", "MplMacro")` (qualifier dropped per `Sym.ctor(string, string)` contract).
    - `public static readonly MplBackquoteMacro singleton;` initialized in `.cctor()`.
    - `private MplBackquoteMacro() : base(backquote) { }`.
    - `ExpandCall(MplEnv e, Sexpr form)`:
      - Checks `m_macro == null`. If null, resolves `m_macro = e.LoadForeignMacro("MplMacro.backquote")`.
      - When `e` is null, throws `NullReferenceException` on `e.LoadForeignMacro`.
      - When `e` is `LocalEnv()` with null parent, throws `NullReferenceException` from `m_parent.LoadForeignMacro`.
      - When `e` is `MplEnv`, throws `AppException("Can not resolve signature")` via `CanNotResolve`.
      - When `m_macro` is populated, delegates directly to `m_macro.ExpandCall(e, form)`.
    - Emits IL matching Mathcad `mplrun.dll` token 0x06000141 byte-for-byte. Attribute on `ExpandCall` set to `[MirrorTested("Mpl.Run.MplBackquoteMacro")]` because `MplEnv.LoadForeignMacro` unconditionally throws `AppException("Can not resolve signature")` in `mplrun.dll`, preventing unmocked foreign macro assignment in differential runs without an external host.
- Evidence files:
  - [Mpl.Run.MplBackquoteMacro](../evidence/Mpl.Run.MplBackquoteMacro.md): Created with `Verified` status across static fields/constructors and `Tested` on `ExpandCall`.
- Test counts & Validation receipts:
  - Unit tests: `dotnet test tests/Quire.Mpl.Tests/Quire.Mpl.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 188 passed, 0 failed, 0 skipped.
  - Differential tests: `QUIRE_REQUIRE_MATHCAD=1 dotnet test tests/Quire.Focused.Differential.Tests/Quire.Focused.Differential.Tests.csproj -m:1 -nr:false -p:UseSharedCompilation=false` — 117 passed, 0 failed, 0 skipped (`EnvDifferentialTests.MplBackquoteMacro_members`: passed).
  - Direct comparisons added:
    - `EnvDifferentialTests.MplBackquoteMacro_members`: `.ctor()`, `.cctor()`, `singleton` identity, `backquote` identity/qualifier-drop, `ExpandCall(null, form)` throwing `NullReferenceException` on `m_macro == null`, `ExpandCall(LocalEnv, form)` throwing `NullReferenceException` from parent lookup, and `ExpandCall` with populated `m_macro` across valid macro expansion and invalid argument forms throwing `AppException`.
  - Format checks: `git diff --check` passed cleanly.
  - Protected paths: `merge_gate.stub_deletions` verified stub removals from `src/Quire.Mpl/Generated/Mirror/Mpl.Run.MplBackquoteMacro.g.cs` (5 member deletions) cleanly without modifying any other protected files.

## Current method audit, separate from historical merges

Source: `/tmp/quire-workflow-trim-runtime-gate/run/method-coverage.json`, run
`e4ecd053-57b6-4787-a8ac-e284bebae831`, carried into baseline commit `c011f35`.
This is a previously executed runtime gate report, not a new validation run for this documentation change.
Its project-wide counts are 719 compared, 925 missing, 137 structure mismatches, 42 uncovered and
81 unsupported Verified statuses; `complete` is false. A passing ratchet permits existing gaps and
never establishes phase completion.

| Lane A type | Missing direct comparisons | Uncovered methods | Unsupported Verified statuses |
|---|---:|---:|---:|
| `Atom` | 1 | 0 | 1 |
| `Sexpr` | 1 | 6 | 6 |
| `SCons` | 0 | 3 | 1 |
| `Sym` | 0 | 3 | 1 |
| `Tok` | 0 | 2 | 0 |
| `AppException` | 0 | 1 | 1 |
| `Mcd.Type` | 1 | 0 | 0 |

Exact remaining members in those types:

- Missing direct comparisons: `Atom..ctor(int)`, `Sexpr..ctor(int)`, `Mcd.Type..ctor()`.
  Abstract base construction needs a direct comparison approach; caller coverage does not satisfy it.
- `Sexpr`: `get_quotation`, `get_quote`, `get_scons`, `WalkModule(MplEnv,Sexpr)`,
  `walk_pattern(MplEnv)`, `walk_expression(MplEnv)`.
- `SCons`: `walk_declaration_lhs(MplEnv)`, `walk_pattern(MplEnv)`, `walk_expression(MplEnv)`.
- `Sym`: `walk_declaration_as(MplEnv,SCons)`, `walk_definition_as(MplEnv,SCons)`,
  `walk_expression_as(MplEnv,SCons)`.
- `Tok`: `walk_pattern(MplEnv)`, `walk_expression(MplEnv)`.
- `AppException.Raise(object[],uint)`.

Some uncovered locations follow helpers that throw. Verify original reachability before proposing exact
unreachable-line entries; never reshape source to hide them. No new acceptance proposal is made here.
This is a queue for the listed scopes, not an exhaustive list of all remaining Lane A runtime types.
`Rational`, `Utilities`, `UnitSystem` and `RuntimeOptions` have no missing/structure/uncovered methods in
this report; their passing recorded cases still do not prove every possible input.

## NumVal.Compare scope correction

Prepared `mirror/a-numval-compare` on `c011f35`, rebased onto `7c5d9aa`, and merged the reviewed
`57872bf` through the runtime gate to main `b107792`. WORK retains its short index. The requested `Mcd.NumVal.Compare` has no source/generated declaration, current manifest
row or member/token in the installed original. No implementation or test should be invented for it.
Artifact identity and the original inspection command are in [NumVal evidence](../evidence/Mcd.NumVal.md).
The obsolete Lane B blocker is corrected in [Lane B](lane-b.md).

Changes: this checkpoint, the NumVal evidence scope note and Lane B's NumVal note. No runtime behavior,
tests, manifest or protected policy changed. Source/manifest inspection and `git diff --check` passed;
the final evidence-change runtime gate passed Harness 36, Mpl 159 and Differential 79, with no failures
or skips, lint 0 and ratchet ok. No methods were selected for mutation. Receipt:
`/tmp/quire-numval-final-runtime-gate/gate.json`; validator exit 3 records existing project-wide gaps,
not phase completion. The reviewed branch and gate-approved main were pushed to origin.

## List_keyword and Tuple_keyword singleton batch (`mirror/a-keyword-singletons`)

- Branch: `mirror/a-keyword-singletons`, rebased onto `2930f1a`; implementation commit `a602f5f`.
- Scope: `List_keyword` and `Tuple_keyword` static singletons, constructors and `NewTok(uint)` factories. The constructors preserve the original keyword strings and key numbers 13 and 14.
- Differential tests: `SimpleKeywordDifferentialTests.Singleton_constructors_and_tokens` directly compares both type initializers and constructors and compares each factory at handles 0, 1, 37 and `uint.MaxValue`.
- Evidence: [List_keyword](../evidence/Mpl.Run.List_keyword.md), [Tuple_keyword](../evidence/Mpl.Run.Tuple_keyword.md). Original IL was inspected with ilspycmd before implementation.
- Checks: filtered direct test passed 1/1. Post-rebase scoped focused validation passed Harness 52, Mpl 188 and Differential 122; 6 methods compared, 0 missing or uncovered, 6 exact, 0 structure-only, lint 0, ratchet ok, mutations 2/2 killed with no survivors. Run `8009ac32-79a2-422d-b6b5-791cf7c37deb`; receipt `/tmp/quire-validation-whc2ts20/summary.json`.
- Exact IL before/after: 0/6 stubbed bodies -> 6/6 exact. No exact method regressed.
- Pending proposals: none. Independent review is outstanding.
- Next: once this batch is reviewed and merged, proceed to an unowned Lane A runtime member. `Rts.GetTypeInitializer(TypeBuilder)` remains blocked by the protected Reflection.Emit linter allowlist; do not edit that policy file.

## User-approved proposals awaiting maintainer acceptance

On 2026-10-16, the user approved the independently reviewed proposal sets on `mirror/a-walk-tail-audit` (one equivalent mutant and 16 unreachable-line entries) and `mirror/a-case-keyword` (two unreachable-line entries). On 2026-10-17, the user also approved the four unreachable-line proposals on `mirror/a-keyword-do-try` and the one unreachable-line proposal on `mirror/a-define-keyword`. These approvals are recorded here while the proposals await maintainer updates to the protected acceptance lists. The affected batches remain unmerged until that acceptance is recorded; this Lane A agent does not edit the protected lists.

## locally_keyword mirror batch (`mirror/a-locally-keyword`)

- Branch: `mirror/a-locally-keyword`, rebased onto `c4529f9`; tip `ea6d03d` (reviewed candidate; merged as `44af299`).
- Scope: `locally_keyword` singleton, `NewTok`, `walk_expression_as`, and the three `MakeLocally` overloads in `src/Quire.Mpl/Run/locally_keyword.cs`; removed their generated stub pairs.
- Original: inspected the installed `mplrun.dll` with ilspycmd; tokens `.cctor` `0x060000d0`, ctor `0x060000d1`, `NewTok` `0x060000d2`, walk `0x060000d3`, factories `0x060000d4`–`0x060000d6`.
- Direct comparisons: type initializer, ctor, `NewTok` at 0/1/42/`uint.MaxValue`, all factory overloads (including null and `Cons` inputs), valid empty-block walk and invalid-arity exception.
- Validation: `validate.py --profile focused --method-type Quire.Mpl.Run.locally_keyword --mutate --jobs 2`. Harness 79/79, Mpl 188/188, Differential 186/186; 6 compared, 0 missing, 0 structure mismatch, 1 uncovered branch, 0 unsupported Verified statuses; 7/7 exact, lint 0, ratchet ok, mutation 1/1 killed. Local checks pass; gate would pass with the known gap. Run `7fb7a302-0da9-4f64-8cef-25018532ca31`; receipt `/tmp/quire-validation-lslp8664/summary.json`.
- Coverage blocker: `locally_keyword.walk_expression_as`'s `cons != null` branch at line 23 is reachable, but needs a declaration form. This base still has the generated `define_keyword.MakeDefine` stub, so the differential case cannot produce declarations. Keep the method `[MirrorTested]`; add the branch comparison after that dependency reaches main. No unreachable-line proposal is appropriate.
- Evidence: [Mpl.Run.locally_keyword](../evidence/Mpl.Run.locally_keyword.md).
- Pending proposals: none. Exact IL before/after: 0/6 implemented method bodies -> 6/6 exact; the original type initializer is also exact. Independent review returned MERGE. Runtime gate passed and merged to main at `44af299` (gate candidate `ea6d03df756e`); the full inventory retained its existing ratcheted gaps. Main was pushed to origin.
- Next: continue with another unowned Lane A member while the declaration dependency is pending.

## let_keyword dependency blocker

Original `mplrun.dll` inspected with ilspycmd on 2026-10-17; identity is the one recorded in [let_keyword evidence](../evidence/Mpl.Run.let_keyword.md). Its `ExpandDefs` branches call `define_keyword.MakeDefine` and `lambda_keyword.MakeLambda`, while `ExpandLet` calls `locally_keyword.MakeLocally`. The definition and lambda batches are still unmerged, so direct comparisons of these reachable branches cannot pass against the generated stubs on main. Do not mark these lines unreachable. Resume this member after those dependencies merge; `locally_keyword` is now on main.

## MplFunctionMacro mirror batch (`mirror/a-mpl-function-macro`)

- Branch: `mirror/a-mpl-function-macro`, rebased onto `7ee2d00`.
- Scope: implement the string constructor, `make`, and `ExpandCall` in `src/Quire.Mpl/Run/MplFunctionMacro.cs`; remove only their generated stub pairs. The existing `Sym` constructor and `ExpandRef` remain exact.
- Original: inspected `mplrun.dll` C# and IL with ilspycmd. New method tokens: ctor `0x06000137`, `make` `0x06000138`, `ExpandCall` `0x06000139`.
- Direct cases: ordinary/empty/null string names for ctor and `make`; a successful identity expansion; a directly thrown `AppException` wrapped at the form handle.
- Evidence: [Mpl.Run.MplFunctionMacro](../evidence/Mpl.Run.MplFunctionMacro.md).
- Validation: filtered `MplFunctionMacroDifferentialTests` passed 2/2. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.MplFunctionMacro --mutate --jobs 2`: Harness 79, Mpl 188, Differential 192 passed; 5 compared, 0 missing, 0 structure mismatch, 0 uncovered, 0 unsupported Verified statuses; 5 exact, 0 structure-only, lint 0, ratchet ok, mutation 0/0 (no eligible mutants). Run `ffa1d80c-b1ae-4102-b535-6092b30d5fbb`; receipt `/tmp/quire-validation-_aqlz2mq/summary.json`.
- Exact IL before/after: 2/5 implemented method bodies -> 5/5 exact; no baseline exact method regressed. Independent review returned MERGE. Pending proposals: none.
- Final current branch tip: this single squashed batch commit on `mirror/a-mpl-function-macro`, including this checkpoint update.
- Next: after this batch, resume blocked `let_keyword` once its `define_keyword` and `lambda_keyword` dependencies reach main; otherwise select the next unowned Lane A member.

## CodeArg constructors (`mirror/a-codearg`)

- Rebasing after `mirror/gen-mpl-2` merged to main followed the user's direction: this batch kept only the two `CodeArg` constructors. The fields and argument walks are supplied by MemberGen; no overlapping `Code.cs` changes remain.
- Scope: `.ctor(Code, Code)` and `.ctor(Code)` in `CodeArg`; `Code.NullCode` is now initialized by the separately reviewed `mirror/a-code-nullcode` batch below.
- Original tokens: `.ctor(Code, Code)` `0x0600034c`; `.ctor(Code)` `0x0600034d`. Contracts: [Mpl.Run.CodeArg](../evidence/Mpl.Run.CodeArg.md).
- Post-rebase differential tests: 3 passed. Scoped validation: Harness 86, Mpl 188 and Differential 3 passed; 4 methods compared, 4 exact, 0 missing/structure mismatch/uncovered/unsupported, lint 0, ratchet ok, mutation 0/0 (no eligible mutants). Run `58162f55-2d80-4251-b25d-0d854f0e2568`; receipt `/tmp/quire-validation-_64jgbb3/summary.json`.
- Exact IL before/after: the two implemented `CodeArg` members remained exact; constructors raised the type from 2/4 to 4/4 exact. Runtime gate passed and merged to main at `cb6911a`, then pushed. No exact method regressed; no proposals pending.


## Code.NullCode type initializer (`mirror/a-code-nullcode`)

- Scope: the `Code` type initializer and its public static `NullCode` field; original initializer token `0x06000343`.
- Original: `tools/reference/reference.py Mpl.Run.Code --il`; `new Code()`, store into `NullCode`, return. The Release body matches exactly.
- Direct tests: `CodeNullCodeDifferentialTests`, `CodeArgDifferentialTests` and `MplRunCodeMechanicalDifferentialTests` passed 6/6 on Windows .NET 10. `Quire.Mpl` Release build succeeded.
- Scoped validation: hybrid Linux/Windows run `/mnt/c/q/quire-validation-code-nullcode-hybrid-with-cases/summary.json`, run `b5407258-e7fc-48b5-8c7c-07ed5af2048c`; Harness 86, Mpl 188 and Differential 6 passed, 0 failures/skips. All 5 selected Code methods compared, 5 exact, 0 missing/structure mismatch/uncovered/unsupported, lint 0, ratchet ok, mutation 0/0 (no eligible mutants). The earlier Windows Harness cleanup failure and two wrapper setup failures are incomplete attempts, not validation results.
- Exact IL before/after: `.cctor` matches `newobj Code::.ctor; stsfld NullCode; ret`; all five selected Code Release bodies are exact, with no exact method regressed. Independent review returned MERGE. Pending proposals: none.
- The reviewed `mirror/a-primitive-constructors` branch (`2d03eea`) remains unmerged. Its targeted tests and independent review passed; the earlier validator/gate attempts did not complete. Consider it with this batch for a single reviewed two-batch gate once both are rebased to current main.
- Next: pass this prerequisite and the private `Primitive` constructors through the runtime gate, then implement and directly compare the `Primitive` type initializer and its static fields.

## CodeFunction constructor and argument walks (`mirror/a-codefunction`)

- Scope: `CodeFunction(Code, Code[])`, `BegArg(ILGenerator, int)`, and `EndArg(ILGenerator, int)`; `EmitCode` was already implemented by MemberGen on main.
- Original tokens: ctor `0x0600034b`, `BegArg` `0x06000348`, `EndArg` `0x0600034a`. Evidence: [Mpl.Run.CodeFunction](../evidence/Mpl.Run.CodeFunction.md).
- Differential tests: constructor with empty and populated arrays, with explicit reference identity checks for both fields; `BegArg`/`EndArg` at indices 0 and 1 use an asymmetric pair of argument slots to distinguish selection and forwarded index. The existing mechanical `EmitCode` comparison also runs.
- Tests: filtered focused run passed 3/3. Scoped Release validation passed Harness 86, Mpl 188 and Differential 3; 4 compared, 0 missing/structure mismatch/uncovered/unsupported, 4 exact, lint 0, ratchet ok, mutation 0/0. Run `b5cc1abc-fd31-47d5-939c-c45b5e5fd7e7`; receipt `/tmp/quire-validation-yf10d0_k/summary.json`.
- Exact IL before/after: `EmitCode` was exact; the three new methods raised the type from 1/4 to 4/4 exact. No exact method regressed. Independent review returned MERGE. Runtime gate passed and merged to main at `244324a4deae`; receipt `/tmp/quire-a-codefunction-gate/run/summary.json`. Gate: Harness 86, Mpl 188, Differential 212; 934 compared, 1435 exact, 97 structure-only, 0 lint, ratchet ok, mutation 0/0; existing inventory gaps remain (777 missing, 106 structure mismatches, 13 uncovered, 43 verified unsupported). Main was pushed.
- Pending proposals: none.

## Primitive cast helpers (`mirror/a-primitive-casts`)

- Scope: `Primitive.as_matrix`, `as_number`, `as_string`, `as_function`, and `as_range`; each returns its input when it has the target runtime type and null otherwise.
- Original: inspected `mplrun.dll` C# with ilspycmd and CIL metadata with REA. Tokens 0x06000395–0x06000399; each body is `ldarg.0; isinst <target>; ret`.
- Tests: five token-bound mechanical comparisons retain null/wrong-type inputs; direct differential cases now also use matching target instances and verify the returned reference for each helper.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered focused tests passed 20/20; scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 20; 29 compared, 0 structure mismatches/uncovered/unsupported, 29 exact, lint 0, ratchet ok, mutations 37/37 killed. Run `6214f1be-d9a8-4eb9-91d5-cf9c5a6d3545`; receipt `/tmp/quire-validation-2o8xpa5h/summary.json`. Scoped status notes 57 pre-existing Primitive stubs; the full gate validates all.
- Exact IL before/after: 24 existing exact methods -> 29 exact; no existing exact method regressed. Independent review returned MERGE. Runtime gate passed after rebase at `08527c9` and merged to main at `168be17`; gate receipt `/tmp/quire-a-primitive-casts-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-casts-gate/run/summary.json`. Gate: Harness 86, Mpl 188, Differential 220; 941 compared, 1440 exact, 97 structure-only, lint 0, ratchet ok, mutation 37/37 killed. Existing inventory gaps remain (770 missing, 106 structure mismatches, 13 uncovered, 43 verified unsupported). Main was pushed. Pending proposals: none.
- Next: implement and directly compare `Primitive.IsOpCodePrimitive`, `IsCodePrimitive`, `IsMethodPrimitive`, and `IsForeignPrimitive`.

## Primitive classification predicates (`mirror/a-primitive-predicates`)

- Scope: `Primitive.IsOpCodePrimitive`, `IsCodePrimitive`, `IsMethodPrimitive`, and `IsForeignPrimitive`; they classify by nullness of `m_code`, `m_opcodes`, `m_method`, and `m_foreign`.
- Original: inspected `mplrun.dll` with ilspycmd before implementation. Tokens 0x06000359–0x0600035c. The method predicate uses `MethodInfo.op_Inequality`; the foreign predicate accepts an empty string.
- Tests: direct differential cases vary each relevant field independently, including the code-suppresses-opcode/method branches and empty foreign name.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered focused tests passed 4/4. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 24; 33 compared, 0 structure mismatches/uncovered/unsupported, 33 exact, lint 0, ratchet ok and mutations 49/49 killed. Run `2564a8b9-35df-463e-afc8-728b18bce55f`; receipt `/tmp/quire-validation-tawm_vq8/summary.json`. Scoped status notes 53 pre-existing Primitive stubs; the full gate validates all.
- Exact IL before/after: the four predicate bodies are exact; Primitive increased from 29/86 to 33/86 exact, with no exact method regressed. Independent review returned MERGE. Runtime gate passed after rebase at `d57aadbf` and merged to main at `ca3f68b8`; gate receipt `/tmp/quire-a-primitive-predicates-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-predicates-gate/run/summary.json`. Gate: Harness 86, Mpl 188, Differential 229; 950 compared, 1446 exact, 98 structure-only, lint 0, ratchet ok, mutation 12/12 killed. Existing inventory gaps remain (764 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). Main was pushed. Pending proposals: none.
- Next: implement and directly compare the integer and double arithmetic helpers on `Primitive`.

## Primitive arithmetic helpers (`mirror/a-primitive-arithmetic`)

- Scope: `neg_i4`, `add_i4`, `sub_i4`, `mul_i4`, `div_i4`, `rem_i4`, `neg_f8`, `add_f8`, `sub_f8`, `mul_f8`, and `div_f8`.
- Original: inspected `mplrun.dll` before implementation. Tokens 0x06000369–0x0600036e and 0x06000375–0x06000379; each is a direct arithmetic opcode with no exception region. Integer division and remainder throw on zero and on `int.MinValue / -1`; floating division follows IEEE results.
- Tests: direct comparisons include wraparound, signed division/remainder, integer exceptional cases, NaN, infinities, signed zero and floating overflow.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered Primitive differential suite passed 26/26. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 26; 44 compared, 0 structure mismatch/uncovered/unsupported, 44 exact, lint 0, ratchet ok, mutation 58/58 killed. Run `b4133d07-318c-49cb-a2d6-0ebf5cacf7a7`; final receipt after signed-zero bit-rendering change `/tmp/quire-validation-gv812y1o/summary.json`. The 42 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: the 11 arithmetic helpers compile to the original direct operators; Primitive rose from 33/86 to 44/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE for the complete diff, including the double bit-pattern renderer. Pending proposals: none.
- Final branch tip `cca6d021679648f0d3ea3abc5dff2be8605c32f4` passed the runtime gate and merged to main at `faf91ae3b8c0`; main was pushed. Gate receipt `/tmp/quire-a-primitive-arithmetic-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-arithmetic-gate/run/summary.json`: Harness 86, Mpl 188, Differential 231; 961 compared, 1457 exact, 98 structure-only, 0 lint, ratchet ok, mutations 9/9 killed. Existing inventory gaps remain (753 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: directly compare `Primitive.or_ref` and `Primitive.and_ref`.

## Primitive reference-selection helpers (`mirror/a-primitive-ref`)

- Scope: `Primitive.or_ref(object, object)` and `and_ref(object, object)`.
- Original: inspected the installed `mplrun.dll` before implementation; tokens 0x06000366 and 0x06000367. `or_ref` returns `y` when `x` is null and otherwise returns `x`; `and_ref` returns `y` when `x` is non-null and otherwise returns null.
- Cases: null/non-null combinations, values selected from either argument, and explicit identity rendering.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered Primitive differential suite passed 28/28. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 28; 46 compared, 0 structure mismatch/uncovered/unsupported, 46 exact, lint 0, ratchet ok, mutation 62/62 killed. Run `5b485f25-73e8-4aa8-90aa-96e05f436f12`; receipt `/tmp/quire-validation-wg6rs863/summary.json`. The 40 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: both reference selectors compile to Mathcad's null branch shapes; Primitive rose from 44/86 to 46/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE for the complete diff; the reviewer rebuilt Release and checked both original/current IL bodies. Pending proposals: none.
- Final rebased branch tip `b211405dee8b35e537ff604d5923585253256703` passed the runtime gate and merged to main at `fbf7627d7e26`; main was pushed. Gate receipt `/tmp/quire-a-primitive-ref-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-ref-gate/run/summary.json`: Harness 86, Mpl 188, Differential 240; 970 compared, 1459 exact, 98 structure-only, 0 lint, ratchet ok, mutations 4/4 killed. Existing inventory gaps remain (744 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: implement and directly compare `Primitive.conv_i4` and `Primitive.conv_f8`.

## Primitive numeric conversion helpers (`mirror/a-primitive-conv`)

- Scope: `Primitive.conv_i4(double)` and `conv_f8(int)`.
- Original: inspected the installed `mplrun.dll` before implementation; tokens 0x06000380 and 0x06000381. `conv_i4` is a direct `conv.i4`; `conv_f8` is a direct `conv.r8`.
- Cases: fractional and boundary double-to-int conversions, out-of-range values, NaN and infinities; representative int-to-double values including both signed bounds.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered Primitive differential suite passed 30/30. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests|FullyQualifiedName~PrimitiveConversionDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 30; 48 compared, 0 structure mismatch/uncovered/unsupported, 48 exact, lint 0, ratchet ok, mutation 62/62 killed. Run `b0e39438-42b4-4f91-8997-6037bd0bbc01`; receipt `/tmp/quire-validation-e06izcc7/summary.json`. The 38 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: both conversions compile to the original `conv.i4` / `conv.r8` bodies; Primitive rose from 46/86 to 48/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE; the reviewer independently rebuilt Release and checked both conversion bodies. Pending proposals: none.
- Final branch tip `5a784f23ade1184707032d2653bb29d9052dc1ef` passed the runtime gate and merged to main at `2fe5ed63e710`; main was pushed. Gate receipt `/tmp/quire-a-primitive-conv-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-conv-gate/run/summary.json`: Harness 86, Mpl 188, Differential 242; 972 compared, 1461 exact, 98 structure-only, 0 lint, ratchet ok, mutations 0/0 selected by the gate. The scoped pre-gate mutation run killed 62/62. Existing inventory gaps remain (742 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: inspect `Primitive.arr_i4`, `arr_f8` and `arr_ref`.

## Primitive array constructors (`mirror/a-primitive-array`)

- Scope: `Primitive.arr_i4(int)`, `arr_f8(int)` and `arr_ref(int)`.
- Original: inspected the installed `mplrun.dll` before implementation; tokens 0x06000384–0x06000386. Each method loads the requested length, executes `newarr` for its element type, and returns the new array.
- Cases: lengths 0, 1, 3 and -1 for all three element types; explicit renderers compare array type and every element, and direct comparisons check the negative-length exception.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered Primitive differential suite passed 33/33. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests|FullyQualifiedName~PrimitiveConversionDifferentialTests|FullyQualifiedName~PrimitiveArrayDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 33; 51 compared, 0 structure mismatch/uncovered/unsupported, 51 exact, lint 0, ratchet ok, mutation 62/62 killed. Run `4fd2bb08-03ab-4ef6-b106-6cd89af267d8`; receipt `/tmp/quire-validation-bykdevvg/summary.json`. The 35 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: all three constructors compile to their original `ldarg.0; newarr; ret` bodies; Primitive rose from 48/86 to 51/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE; the reviewer independently rebuilt Release and confirmed all three `newarr` bodies. Pending proposals: none.
- Final branch tip `109d59a2492dab854f9eae9901cfb4525dc5168c` passed the runtime gate and merged to main at `9e5f3e3be52a`; main was pushed. Gate receipt `/tmp/quire-a-primitive-array-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-array-gate/run/summary.json`: Harness 86, Mpl 188, Differential 245; 975 compared, 1464 exact, 98 structure-only, 0 lint, ratchet ok, mutations 0/0 selected by the gate. The scoped pre-gate mutation run killed 62/62. Existing inventory gaps remain (739 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: inspect Primitive array accessors and setters.

## Primitive array element operations (`mirror/a-primitive-array-access`)

- Scope: integer, double and object array reads/writes, the closure object-array setter, and object-array length.
- Original: inspected the installed `mplrun.dll` before implementation; tokens 0x06000387–0x06000391. These methods use typed array access or report `Length`.
- Cases: valid indices, mutation and returned-array identity, null/out-of-range behavior, and array lengths.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered Primitive differential suite passed 37/37. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests|FullyQualifiedName~PrimitiveConversionDifferentialTests|FullyQualifiedName~PrimitiveArrayDifferentialTests|FullyQualifiedName~PrimitiveArrayAccessDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 37; 62 compared, 0 structure mismatch/uncovered/unsupported, 62 exact, lint 0, ratchet ok, mutation 62/62 killed. Run `cf818868-eb8a-48c3-9eac-cf1a4e14e279`; receipt `/tmp/quire-validation-vjr0hwk_/summary.json`. The 24 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: all eleven operations compile to their original typed array access/store and length bodies; Primitive rose from 51/86 to 62/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE. Pending proposals: none.
- Final branch tip `0d16a5c06c3ae495a6f54188ab608e63664e0106` passed the runtime gate and merged to main at `307ff70880ef`; main was pushed after the shared merge lock was released. Gate receipt `/tmp/quire-a-primitive-array-access-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-array-access-gate/run/summary.json`: Harness 86, Mpl 188, Differential 249; 986 compared, 1475 exact, 98 structure-only, 0 lint, ratchet ok, mutation 0/0 selected by the gate. The scoped pre-gate mutation run killed 62/62. Existing inventory gaps remain (728 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: inspect `Primitive.xor`, shifts and `nrt_f8`.

## Primitive bitwise, shift and nth-root helpers (`mirror/a-primitive-bitops`)

- Scope: `Primitive.xor`, `shl`, `shr`, `shr_u4`, and `nrt_f8`.
- Original: inspected the installed `mplrun.dll` before implementation; tokens `0x060003a0`–`0x060003a4`. XOR is direct; each shift masks the count with 31 (unsigned right shift uses `shr.un`); `nrt_f8` calls `Math.Pow(y, 1.0 / x)`.
- Cases: XOR bit patterns; signed and unsigned shifts over edge values with counts 0, 1, 31, 32, 33, -1 and `int.MinValue`; nth-root normal, zero, negative and non-finite inputs with bit-exact double renderers.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered `PrimitiveBitwiseDifferentialTests` passed 4/4. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests|FullyQualifiedName~PrimitiveConversionDifferentialTests|FullyQualifiedName~PrimitiveArrayDifferentialTests|FullyQualifiedName~PrimitiveArrayAccessDifferentialTests|FullyQualifiedName~PrimitiveBitwiseDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 41; 67 compared, 0 structure mismatch/uncovered/unsupported, 67 exact, lint 0, ratchet ok, 73/73 mutants killed. Run `ece77bff-1d47-4870-a06a-b8ed310252a3`; receipt `/tmp/quire-validation-6uzix4a2/summary.json`. The 19 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: all five helpers compile to the original XOR/shift/Math.Pow opcodes; Primitive rose from 62/86 to 67/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE. Pending proposals: none.
- Final branch tip `f961c9e8cb871a759fe80e50d885706dba322f50` passed the runtime gate and merged to main at `771dbe07ff0e`; main was pushed. Gate receipt `/tmp/quire-a-primitive-bitops-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-bitops-gate/run/summary.json`: Harness 86, Mpl 188, Differential 254; 998 compared, 1481 exact, 97 structure-only, 0 lint, ratchet ok, mutations 11/11 killed. The scoped pre-gate mutation run killed 73/73. Existing inventory gaps remain (716 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: inspect `Primitive.iv` and `with_bool`.

## Primitive scalar-value helpers (`mirror/a-primitive-values`)

- Scope: `Primitive.iv` and `with_bool`.
- Original: inspected the installed `mplrun.dll` before implementation; `iv(int)` token `0x0600039c` constructs `NumVal(i, 0.0)`. `with_bool(object)` token `0x0600039f` casts to `NumVal`, calls `AppException.Raise("must be scalar")` for other types, then returns null only when both `Data.rp` and `Data.ip` compare equal to zero; otherwise it returns `True.singleton`.
- Cases: integer bounds and representative values; scalar zero, signed zero, nonzero real and imaginary components, NaN and infinities; null and non-NumVal inputs.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered `PrimitiveValueDifferentialTests` passed 2/2. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests|FullyQualifiedName~PrimitiveConversionDifferentialTests|FullyQualifiedName~PrimitiveArrayDifferentialTests|FullyQualifiedName~PrimitiveArrayAccessDifferentialTests|FullyQualifiedName~PrimitiveBitwiseDifferentialTests|FullyQualifiedName~PrimitiveValueDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 43; 69 compared, 0 structure mismatch/uncovered/unsupported, 69 exact, lint 0, ratchet ok, 82/82 mutants killed. Run `4f3d5573-3f78-4a07-8848-5e1d3e4a66f7`; receipt `/tmp/quire-validation-wh44rjpf/summary.json`. The boxed-int invalid-input assertion explicitly passes `(object)17`, avoiding overload selection of the double comparison. The 17 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: both helpers compile to the original `NumVal` construction and scalar checks; Primitive rose from 67/86 to 69/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE. Pending proposals: none.
- Final rebased tip `2a47ab9740330398e88a597c4b384ecfe703c15a` passed the runtime gate and merged to main at `4b9b5cfddd6a`; main was pushed. Gate receipt `/tmp/quire-a-primitive-values-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-values-gate/run/summary.json`: Harness 86, Mpl 188, Differential 257; 1008 compared, 1483 exact, 97 structure-only, lint 0, ratchet ok; 163/167 mutants killed, 4 existing accepted survivors, 1 timeout retry. Existing inventory gaps remain (706 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: implement and directly compare the remaining private `Primitive` constructors.

## Primitive reflective call overloads (`mirror/a-primitive-call`)

- Scope: the seven `Call` and two `Call_c` overloads on `Primitive`.
- Original: re-read installed `mplrun.dll` using `tools/reference/reference.py Mpl.Run.Primitive --il`; artifact SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Tokens `0x0600035d`–`0x06000365`; each selects `m_primitive[prim].m_method`, invokes it with a newly allocated exact-size argument array, and returns the result directly or unboxes the requested primitive type.
- Direct tests: all overloads with primitive index 1 and mapped target methods; identity-aware renderers check returned object/array references and first/second argument selection, the 3-argument call mutates and returns the same array, and finite noncommutative integer/double subtraction checks binary argument order.
- Evidence: [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Checks: filtered `PrimitiveCallDifferentialTests` passed 2/2. Scoped `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~PrimitiveDifferentialTests|FullyQualifiedName~MplRunPrimitiveMechanicalDifferentialTests|FullyQualifiedName~PrimitivePredicateDifferentialTests|FullyQualifiedName~PrimitiveArithmeticDifferentialTests|FullyQualifiedName~PrimitiveReferenceDifferentialTests|FullyQualifiedName~PrimitiveConversionDifferentialTests|FullyQualifiedName~PrimitiveArrayDifferentialTests|FullyQualifiedName~PrimitiveArrayAccessDifferentialTests|FullyQualifiedName~PrimitiveBitwiseDifferentialTests|FullyQualifiedName~PrimitiveValueDifferentialTests|FullyQualifiedName~PrimitiveCallDifferentialTests' --mutate --jobs 2` passed Harness 86, Mpl 188 and Differential 45; 78 compared, 0 structure mismatch/uncovered/unsupported, 78 exact, lint 0, ratchet ok and 105/105 mutants killed. Run `ef343c45-93df-4781-a2fa-a422ab0a33c1`; receipt `/tmp/quire-validation-apoas5b3/summary.json`. The 8 missing methods are pre-existing Primitive stubs outside this batch.
- Exact IL before/after: all nine overloads match the original argument-array, `Invoke` and optional unboxing shapes; Primitive rose from 69/86 to 78/86 exact, with no baseline-exact method regressed.
- Independent review returned MERGE. Pending proposals: none.
- Final reviewed tip `982a403b788c` passed the runtime gate and merged to main at `4dad38b56e16`; main was pushed. Gate receipt `/tmp/quire-a-primitive-call-gate/gate.json`, validation receipt `/tmp/quire-a-primitive-call-gate/run/summary.json`: Harness 86, Mpl 188, Differential 259; 1017 compared, 1492 exact, 97 structure-only, lint 0, ratchet ok, mutations 29/29 killed. Existing inventory gaps remain (697 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported). No proposals pending.
- Next: implement and directly compare the remaining private `Primitive` constructors.

## Primitive private constructors (`mirror/a-primitive-constructors`)

- Scope: all eight private `Primitive` constructors (`0x060003a7`–`0x060003ae`), covering scalar/array opcodes, explicit and reflected `MethodInfo`, `Code`, and foreign definitions.
- Original: inspected the installed `mplrun.dll` C# and IL with `tools/reference/reference.py`; contracts are in [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Direct tests: all constructor overloads; compare field contents, method lookup, exact opcode-array/method reference retention and explicit null fields. Reviewer-requested strengthening adds a non-null `Code` argument with identity rendering and distinct `neg_i4`/`add_i4` opcodes for scalar and array constructors. Post-rebase `PrimitiveConstructorDifferentialTests` passed 1/1 on Windows .NET 10.
- Rebased from `2d03eea` onto `85003e9` as `2af717d`, then onto current main `d866b18` as `976d09a`; production and test hunks applied cleanly. The only conflict was `WORK.md`, resolved to point to the constructor gate and then the `Primitive` static initializer.
- Scoped validation: `validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~Primitive' --mutate --jobs 2`; Harness 86, Mpl 188 and Differential 46 passed, 0 failures/skips; 86 compared, 0 missing/structure mismatch/uncovered/unsupported, 86 exact, lint 0, ratchet ok, mutations 109/109 killed (41 cached), no survivors. Run `b0e97290-dee8-4f14-925c-e7a14e0e497b`; receipt `/mnt/c/q/quire-validation-primitive-constructors-final/summary.json`. The earlier narrow filter failed ratchet because it omitted previously verified Primitive tests (52 regressions); it is superseded by this complete scoped run. Historical Harness cleanup and Windows clone path errors are superseded by this run and the successful hybrid gate path.
- Exact IL before/after: all eight constructors compile exactly to the original field writes, opcode array allocations, `Type.GetMethod` lookup and explicit null writes; Primitive rose from 78/86 to 86/86 exact, with no baseline-exact method regressed.
- Independent review: MERGE for rebased production/tests/evidence; requested checkpoint next-action correction is reflected here. Pending proposals: none. The constructor batch passed the runtime gate and is on main at `f4188dd`; receipt `/mnt/c/q/quire-a-primitive-constructors-gate/gate.json`.
- Next: continue with the independent `DynamicType` name helpers (`TypeParameterName` overloads and `FieldName`) on `mirror/a-dynamic-type-name-helpers`.

## Primitive static initializer (`mirror/a-primitive-initializer`)

- Scope: `Primitive..cctor`, `m_primitive`, and `m_stelem_vec`; complete ordered 88-entry table and supporting `Code`/`CodeFunction` initialization, including Mathcad's dead `Math.Pow` reflection lookup.
- Original: installed `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; `.cctor` token `0x060003af`. Original C# and IL read via `tools/reference/reference.py Mpl.Run.Primitive --il`; contracts are in [Mpl.Run.Primitive](../evidence/Mpl.Run.Primitive.md).
- Direct test: `PrimitiveInitializerDifferentialTests` compares the initializer and both fields, traversing all 88 entries, nested arrays, method/type references and shared-object aliases. A first `Primitive.Call` comparison detects failures caused by malformed initializer indexes; the direct `.cctor` comparison remains on the normal passing path.
- Local checks: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH TMPDIR=/mnt/c/q python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.Primitive --diff-filter 'FullyQualifiedName~Primitive' --mutate --jobs 2 --dotnet /tmp/quire-dotnet --results /mnt/c/q/quire-validation-primitive-initializer-probe` passed: Harness 86, Mpl 188 and Differential 47 passed, 0 failures/skips; 87 compared, 87 exact, 0 missing/structure mismatch/uncovered/unsupported, lint 0, ratchet ok, 235/235 mutants killed, 0 survivors/timeouts/crashes. Run `11427340-8460-4da3-b29b-62391d8acfaa`; receipt `/mnt/c/q/quire-validation-primitive-initializer-probe/summary.json`.
- Exact IL before/after: Primitive was 86/86 exact before and 87/87 exact after; the new initializer matches all 707 Release instructions, and no previously exact method regressed.
- Independent review: MERGE for the initializer and updated startup-failure probe; targeted differential test passed 1/1 before and after rebase, lint 0. Scoped audit ran at `f4188dd`; the branch was rebased onto `38be34e`. Pending proposals: none. Runtime gate passed with existing inventory gaps; gate tip `5dac72594cc0` merged at `ea3a1b1f5a47`, main pushed. Receipt `/mnt/c/q/quire-a-primitive-initializer-gate/gate.json`; run `88d3f394-1801-4fab-9f9b-bec1d616d96c` at `/mnt/c/q/quire-a-primitive-initializer-gate/run/summary.json`. Gate: Harness 86, Mpl 188, Differential 267; 1060 compared, 1502 exact, 97 structure-only, 656 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported, lint 0, ratchet ok, 149/149 mutants killed, no timeouts/crashes. No baseline-exact method regressed.
- Next: complete the independent `DynamicType` name helpers on `mirror/a-dynamic-type-name-helpers`.

## DynamicType name helpers (`mirror/a-dynamic-type-name-helpers`)

- Scope: `TypeParameterName(int)` (`0x0600040c`), `TypeParameterName(string, int)` (`0x0600040d`), and `FieldName(int)` (`0x06000417`); no reflection-emit members.
- Original: installed `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; original C# and IL read with `tools/reference/reference.py Mpl.Run.DynamicType --il`. Source uses `String.Concat(object, object)` to retain Mathcad's integer boxing and exact call shape.
- Direct tests: `DynamicTypeNameHelpersDifferentialTests`, 3/3 passed on the rebased tree; the scoped run also passed the existing constructor comparison (4 differential tests total).
- Local checks: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH TMPDIR=/mnt/c/q python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet --results /mnt/c/q/quire-validation-dynamic-type-name-helpers-final` passed: Harness 86, Mpl 188, Differential 4, 0 failures/skips; 4 compared, 4 exact, 25 pre-existing missing stubs, no structure mismatches/uncovered/unsupported, lint 0, ratchet ok, 4/4 mutants killed. Run `5a430b2b-d4c3-42a1-a7e8-d5ec7892caa1`; receipt `/mnt/c/q/quire-validation-dynamic-type-name-helpers-final/summary.json`.
- Exact IL before/after: DynamicType was 1/29 exact before and 4/29 after; all three new methods match exactly, and no previously exact method regressed.
- Pending proposals: none. Independent review and runtime gate: pending.
- Next: obtain independent review, then pass the batch through the runtime gate.

## DynamicType field-name cache (`mirror/a-dynamic-type-field-cache`)

- Branch: `mirror/a-dynamic-type-field-cache`, based on `origin/main` at `f7afb21`, rebased cleanly onto `53036b0` as `15dd30c` before the runtime gate.
- Scope: five `DynamicType` static fields, `.cctor()` (`0x060003fd`) and `FieldNames(int)` (`0x06000418`).
- Original: inspected the installed `mplrun.dll` C# and IL with `tools/reference/reference.py Mpl.Run.DynamicType --il`; SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The initializer creates the field-name cache, sets the tuple prefix and delimiter, and fills the name/type maps. `FieldNames` returns the cached array or constructs and stores one using `FieldName(i)`.
- Direct test: `DynamicTypeFieldNamesDifferentialTests` compares initializer state and maps, field-name arrays for counts 0, 1 and 4, the negative-count exception, and repeated-array identity. Windows .NET command `dotnet test tests/Quire.Differential.Tests/Quire.Differential.Tests.csproj --filter FullyQualifiedName~DynamicTypeFieldNamesDifferentialTests -m:1 -nr:false -p:UseSharedCompilation=false` passed 1/1.
- Evidence: [Mpl.Run.DynamicType](../evidence/Mpl.Run.DynamicType.md).
- Scoped validation: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2`; Harness 86, Mpl 188 and Focused Differential 5 passed, 0 failures/skips; 6 compared, 24 pre-existing stubs missing, 0 structure mismatches/uncovered/unsupported, 6 exact, lint 0, ratchet ok, 11/11 mutants killed, no survivors/crashes, 1 timeout mutant killed after retry. Run `65dd2e2e-b832-4b4f-a194-5b04a15f876c`; receipt `/tmp/quire-dynamic-type-field-cache-final/summary.json`. Local checks pass; the type-scoped gate prediction is pass with the existing 24 missing stubs.
- Exact IL before/after: DynamicType was 4/29 exact before and 6/30 after; the initializer and `FieldNames` match Mathcad's Release IL exactly, and no previously exact method regressed.
- Independent review returned MERGE for the complete diff. No proposals.
- Runtime gate: passed on rebased tip `15dd30c1372b`; merged at `d8f2e26fb28d`, and main was pushed. Receipt `/tmp/quire-a-dynamic-type-field-cache-gate/gate.json`; run `89238d41-6523-4736-83f3-5a02786e087a` at `/tmp/quire-a-dynamic-type-field-cache-gate/run/summary.json`. Harness 86, Mpl 188 and Differential 280 passed with no failures/skips; 1105 compared, 1507 exact, 97 structure-only, lint 0, integrity complete and ratchet OK. Mutation 7/7 killed; one mutant timed out on its first attempt and was killed on retry. Existing gaps: 612 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported.
- Next: implement `DynamicType.NameToType(string)` and `TypeToName(Type)` on a fresh branch from current main; the original contracts are prepared and recorded in the field-cache session notes.

## MplDestructuringBind.as_pattern (`mirror/a-mpl-destructuring-bind`)

- Branch `mirror/a-mpl-destructuring-bind`, based on `main` `34ccce6`; work commit `98b875c`. Scope: `as_pattern(Sexpr)` (`0x06000144`), its differential test, generated stub removal and evidence.
- Original: installed `mplrun.dll` SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Read the original C# and IL with `tools/reference/reference.py Mpl.Run.MplDestructuringBind --il`. Cons cells recurse and become `(Cons head tail)`; a `Ca` head is unwrapped, `Nul` quotes, `Cm` unwraps, and other nodes pass through.
- Direct Windows .NET 10 test `MplDestructuringBindDifferentialTests` passed 1/1. It covers `Nul`, `Cm`, `Ca`, splice-head, recursive and ordinary lists, an unchanged `Qt`, and null input.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.MplDestructuringBind --diff-filter 'FullyQualifiedName~MplDestructuringBindDifferentialTests' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-mpl-destructuring-bind-final`. Run `11da75e3-3b74-4296-805f-c37de3fa150a`; summary `/tmp/quire-validation-mpl-destructuring-bind-final/summary.json`. Harness 86, Mpl 188 and Differential 1 passed with 0 failures/skips; 1 compared, 2 pre-existing stubs missing, 0 structure mismatches/uncovered/unsupported, 1 exact, lint 0, ratchet ok, 8/8 mutants killed. `LOCAL CHECKS: pass · GATE: would pass with gaps (2 missing)`.
- Exact Release IL before/after: 0/3 to 1/3 methods exact; `as_pattern` matches the original 116-byte body opcode for opcode. No previously exact method regressed.
- No proposals. Independent full-diff review and runtime gate remain.
- Next: implement and directly compare `MplSymbolMacro.ToString()` as recorded below; `MplDestructuringBind.ExpandCall` remains dependent on the generated `define_keyword.MakeDefine` stub reaching main.

## MplSymbolMacro.ToString (`mirror/a-mpl-symbol-macro`)

- Scope: implement `MplSymbolMacro.ToString()` (`0x06000132`), remove its generated stub, and add direct comparisons for normal and null-field cases.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The body calls both fields' virtual `ToString()` methods in order and concatenates four strings.
- Direct Windows .NET 10 test: `EnvDifferentialTests.MplSymbolMacro_members` passed 1/1, covering ordinary values and null `m_name` / `m_replacement` exceptions. Independent review returned MERGE for implementation and tests.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.MplSymbolMacro --diff-filter 'FullyQualifiedName~EnvDifferentialTests.MplSymbolMacro_members' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-mpl-symbol-macro`. Run `ae7c5315-8444-4253-ba4a-8255f942f636`; summary `/tmp/quire-validation-mpl-symbol-macro/summary.json`. Harness 86, Mpl 188 and Differential 1 passed, 0 failures/skips; 5 compared, 0 missing/uncovered/structure mismatches/unsupported, 5 exact, lint 0, integrity complete, ratchet ok, 1/1 mutant killed. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all]`.
- Exact Release IL before/after: 4/5 to 5/5 methods exact; `ToString()` now matches Mathcad's 38-byte body opcode for opcode. No baseline-exact method regressed. No proposals.
- Independent review returned MERGE for the implementation and tests. Runtime gate passed at `ad73d65fa208`; receipt `/tmp/quire-validation-mpl-symbol-macro-gate/gate.json`, run summary `/tmp/quire-validation-mpl-symbol-macro-gate/run/summary.json`. Gate: Harness 86, Mpl 188, Differential 281; 1,107 compared, 1,509 exact, 97 structure-only, 610 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 108/109 mutants killed, 1 accepted survivor, 4 timeouts retried. No baseline-exact method regressed. Main was pushed to origin.
- Next: continue with the independent `MplForeignMacro` members, as indexed in `WORK.md`; then proceed to the standalone `DynamicType.MplConcat3(object, object, object)` member already under implementation.

## MatrixIterator (`mirror/a-matrix-iterator`)

- Scope: implement `Iterator.MatrixIterator`'s fields, constructor, `first()` and `next()` in `src/Quire.Mpl/Mcd/Iterator.cs`; add an artifact-bound direct comparison of the cursor and iteration behavior.
- Evidence: [Mcd.RngVal](../evidence/Mcd.RngVal.md). The original `mplrun.dll` is SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Cursor order is rows then columns; matrix dimensions are read live; the last advance returns null.
- Test: `MatrixIteratorDifferentialTests.Matrix_iterator_traverses_rows_then_columns` compares constructor state, `forValue`'s initial cursor, each `first()` value, row/column state and identity on each `next()`, and terminal null over a 2x3 matrix.
- Rebased branch: `mirror/a-matrix-iterator` was reviewed at `6257a6398f39` on `ad73d65fa208`; after review found a branch-shape mismatch in `next()`, it was changed to the shared-return form and the scoped run was repeated. For the runtime gate, the branch rebased cleanly to `0cd49a5659d7`; gate-validated tip `cec9ebfce842`.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp python3 tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.Iterator+MatrixIterator' --diff-filter 'FullyQualifiedName~MatrixIteratorDifferentialTests' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-matrix-iterator-shared-return-nested`. Run `57b30bac-e677-4e94-9cac-673c3ee6e47c`; Harness 86, Mpl 188 and Differential 1 passed, 0 failures/skips; 3 methods compared, 0 missing/uncovered/structure mismatches/unsupported, all 3 exact, lint 0, integrity complete, ratchet ok, 15/15 mutants killed, no survivors/timeouts/crashes. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 type; the gate validates all]`.
- Exact Release IL before/after: 2/3 -> 3/3 exact; constructor and `first()` remained exact, and `next()` now matches exact opcode order. No baseline-exact method regressed. No proposals.
- Independent review returned MERGE after the shared-return fix. Runtime gate passed and merged to main at `118e98efdd2c`; main was pushed. Receipt `/tmp/quire-a-matrix-iterator-gate/gate.json`, run summary `/tmp/quire-a-matrix-iterator-gate/run/summary.json`: Harness 86, Mpl 188, Differential 283; 1,111 compared, 1,511 exact, 95 structure-only, 606 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 271/271 mutants killed, one timeout retried, no survivors/crashes. The existing inventory gaps remain ratcheted. No baseline-exact method regressed.
- Next: continue with the active `MplForeignMacro` batch in its separate worktree; then return to independent unowned Lane A members.

## Iterator.forValue (`mirror/a-iterator-forvalue`)

- Scope: promote `Iterator.forValue(object)` (`0x06000701`) from analyzed to verified and add direct differential cases for each empty matrix axis without touching the active `RngVal` conversion batch's test file.
- Original: installed `mplrun.dll` SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; original C# and IL inspected with `tools/reference/reference.py Mcd.Iterator --il`. `MatVal` dispatch precedes `RngVal`; either nonpositive matrix dimension returns null; other values including null produce a `ScalarIterator`.
- Direct comparisons: existing `RngValDifferentialTests.Iterators` covers ranges, number, string, null and 2x3, 1x3, 3x1 and 0x0 matrices. New `IteratorForValueDifferentialTests.forValue_checks_each_matrix_axis_and_retains_nonempty_matrix` directly compares 0x3, 3x0 and 2x2, and asserts the nonempty iterator retains the exact input matrix reference.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mcd.Iterator --diff-filter 'FullyQualifiedName~RngValDifferentialTests|FullyQualifiedName~IteratorForValueDifferentialTests' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-iterator-forvalue-reviewed-final`. Run `bb614024-a855-43d9-bbaf-731c75828f75`; Harness 86, Mpl 188 and Differential 6 passed, 0 failures/skips; 9 methods compared, 2 declaration-only, 1 pre-existing missing and 1 verified-unsupported; 10 exact, 0 structure-only, 0 uncovered; lint 0, integrity complete, ratchet ok, 16/16 mutants killed, no survivors/timeouts/crashes. `LOCAL CHECKS: pass · GATE: would pass with gaps (1 missing; 1 verified_unsupported)`.
- Exact Release IL before/after: selected IL match is 10 exact and 0 structure-only; `forValue` matches Mathcad's 61-byte body opcode for opcode before and after (the implementation body is unchanged; status and comparisons were added). No baseline-exact method regressed. No proposals. Independent review returned MERGE.
- Runtime gate: reviewed tip `c7b2288` rebased onto `aaa1a47`; gate-validated tip `8a1bcb9` passed and merged to main, followed by the baseline update at `36a4281`; main was pushed. Receipt `/tmp/quire-gate-iterator-forvalue/gate.json`, run `31a043be-75c7-410b-aafd-cc5009eff0b7`, summary `/tmp/quire-gate-iterator-forvalue/run/summary.json`: Harness 86, Mpl 188 and Differential 325 passed; 1,143 compared, 165 declaration-only, 95 excluded, 574 missing, 103 structure mismatches, 13 uncovered and 43 verified-unsupported; 1,521 exact and 93 structure-only; lint 0, integrity complete, ratchet ok, 16/16 mutants killed with no survivors, timeouts or crashes. Existing inventory gaps remain ratcheted. No baseline-exact method regressed.
- Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another independent unowned Lane A member.

## DynamicType.MplConcat3 (`mirror/a-dynamic-type-mplconcat3`)

- Scope: implement `MplConcat3(object, object, object)` (`0x06000405`), remove its generated stub, and add direct differential comparisons.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Mathcad substitutes the string `"null"` for each null argument and calls `String.Concat(object, object, object)`.
- Direct Windows .NET 10 test: `DynamicTypeMplConcat3DifferentialTests` passed 1/1, covering all-null, each individual null position, literal `"null"`, and mixed boxed values.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-dynamic-type-mplconcat3-wide`. Run `60067ba8-2e59-44a8-8198-c1c668f41ba1`; summary `/tmp/quire-validation-dynamic-type-mplconcat3-wide/summary.json`: Harness 86, Mpl 188, Differential 6 passed, no failures/skips; 7 compared, 23 pre-existing missing stubs, 0 structure mismatches/uncovered/verified-unsupported; 7 exact, lint 0, integrity complete, ratchet ok, 14/14 mutants killed (3 cached; 1 timed out then killed on retry). `LOCAL CHECKS: pass · GATE: would pass with gaps (23 missing)`.
- A diagnostic run filtered only to the new test passed its direct test and killed 3/3 mutants, but narrowed away six existing DynamicType comparisons and correctly failed the ratchet. The broad run above includes the existing test classes and is the scoped handoff result.
- Exact Release IL before/after: DynamicType was 6/30 exact before and 7/30 after; `MplConcat3` matches the original 39-byte body opcode for opcode. No previously exact method regressed. No proposals.
- Independent review returned MERGE for the full five-path diff at implementation tip `1457793`. For the runtime gate, the branch rebased cleanly to `118e98efdd2c`; gate-validated tip `daa2d0ef6b70`. No proposals.
- Runtime gate passed and merged to main at `c0a4e7a083f7`; main was pushed. Receipt `/tmp/quire-a-dynamic-type-mplconcat3-gate/gate.json`, run summary `/tmp/quire-a-dynamic-type-mplconcat3-gate/run/summary.json`: Harness 86, Mpl 188, Differential 284; 1,112 compared, 1,512 exact, 95 structure-only, 605 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 3/3 mutants killed, no survivors/timeouts/crashes. Existing inventory gaps remain ratcheted. No baseline-exact method regressed.
- Next: continue with the independent `MplForeignMacro` members indexed in `WORK.md`.

## DynamicType name maps (`mirror/a-dynamic-type-name-maps`)

- Branch `mirror/a-dynamic-type-name-maps`, code commit reviewed as `b171497`, rebased onto current main `c0a4e7a`; scope: `NameToType(string)` (`0x06000410`) and `TypeToName(Type)` (`0x06000411`), direct differential tests and generated-stub removals.
- Original: installed `mplrun.dll` SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. `NameToType` looks up the key and raises `demangle error` on a miss; `TypeToName` returns the mapped name or `.ref`. See [Mpl.Run.DynamicType](../evidence/Mpl.Run.DynamicType.md).
- Direct Windows .NET 10 test: `DynamicTypeNameMapsDifferentialTests` passed 2/2. The scoped run passed Harness 86, Mpl 188 and Differential 7 with no failures/skips. Cases cover every mapped key, unmapped and null keys.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH TMPDIR=/tmp python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --proposed-unreachable /tmp/quire-name-map-unreachable.txt --results /tmp/quire-validation-dynamic-type-name-maps-native`. Run `bb08d760-8950-4606-99b9-4286027568c5`; summary `/tmp/quire-validation-dynamic-type-name-maps-native/summary.json`: 8 compared, 22 pre-existing missing stubs, no structure mismatches/uncovered/unsupported, 8 exact, lint 0, ratchet ok, 14/14 mutants killed (one timeout killed on retry). `LOCAL CHECKS: pass · GATE: would block (1 proposed unreachable line pending acceptance)`.
- At the original scoped-run base, DynamicType exact totals were 6/30 before and 8/30 after; both added methods were exact. Current main also contains `MplConcat3`, so the protected runtime gate will report totals against the rebased baseline. No previously exact method regressed in the scoped run.
- The reviewed unreachable-line proposal is `Quire.Mpl.Run.DynamicType::NameToType`0([System.Runtime]System.String)->[System.Runtime]System.Type|True src/Quire.Mpl/Run/DynamicType.NameMaps.cs:15 sha256=e227ce95d7373cf69bba874573b5501acd84cc1e476b842fb42c9e2d9eb01883`: `AppException.Raise("demangle error")` always throws, and Mathcad token `0x06000410` has the same `pop; ret` tail, unreachable in the original too. The user approved it; the maintainer acceptance entry is included in `docs/mirror/unreachable-lines.txt` on this branch.
- Independent review of the resolved full diff returned MERGE at code commit `b171497`, including the protected unreachable-line proof and acceptance. Checkpoint-only amendments clarify the pre-rebase exact totals and record the review verdict; the implementation and protected entry are unchanged. No mutant proposals.
- Runtime gate passed with `--allow-protected`; gate-validated tip `c25a4fdc06c0`, merged to main at `1d5ebb7f2f38`; main was pushed. Receipt `/tmp/quire-a-dynamic-type-name-maps-gate/gate.json`, run summary `/tmp/quire-a-dynamic-type-name-maps-gate/run/summary.json`: Harness 86, Mpl 188, Differential 286; 1,114 compared, 1,514 exact, 95 structure-only, 603 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 3/3 mutants killed, no survivors/timeouts/crashes. The only protected path changed was `docs/mirror/unreachable-lines.txt`, containing the user-approved entry. Existing gaps remain ratcheted. No baseline-exact method regressed.
- Next: continue with the active `MplForeignMacro` batch; then proceed to independent unowned Lane A members.

## DynamicType.Concat2Mi (`mirror/a-dynamic-type-concat2mi`)

- Scope: implement private `Concat2Mi()` (`0x06000407`), remove its generated stub, and directly compare the reflected method signature.
- Original: installed `mplrun.dll` SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The body resolves `String.Concat(Object, Object)` by calling `typeof(string).GetMethod("Concat", new Type[] { typeof(object), typeof(object) })`.
- Test: `DynamicTypeConcat2MiDifferentialTests` invokes the private method on both artifacts and compares declaring type, method name, parameter types, return type and static flag.
- Branch rebased onto current main `0ac0569`; MplConcat3 and NameToType/TypeToName are retained from main in the shared `DynamicType` source, generated file and evidence.
- Scoped validation: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-dynamic-type-concat2mi-current`. Harness 86, Mpl 188 and Differential 9 passed with no failures/skips; 10 methods compared (all exact), 20 pre-existing stubs, no structure mismatches/uncovered/unsupported, lint 0, integrity complete, ratchet ok, 20/20 mutants killed, no survivors (one unrelated `FieldNames` mutant timed out in parallel and was killed again on its single-worker retry). `LOCAL CHECKS: pass · GATE: would pass with gaps (20 missing)`. Run summary: `/tmp/quire-validation-dynamic-type-concat2mi-current/summary.json`.
- Exact Release IL before/after: `Concat2Mi` was a generated stub and was not baseline-exact before; it is exact after. The six baseline-exact DynamicType members remain exact; the current scoped inventory has 10/30 exact methods, with no regressions. No proposals.
- Independent review returned MERGE at implementation tip `0bab5a631566`.
- Runtime gate: rebased base `0ac0569afb22`, gate-validated tip `0bab5a631566`; passed and merged to main at `0bab5a6`, followed by baseline update `ae26cbe`. Main was pushed. Gate receipt `/tmp/quire-gate-dynamic-type-concat2mi/gate.json`, summary `/tmp/quire-gate-dynamic-type-concat2mi/run/summary.json`: Harness 86, Mpl 188, Differential 287 passed; 1,115 compared, 1,515 exact, 95 structure-only, 602 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 3/3 selected mutants killed, no timeouts/survivors/crashes. Existing inventory gaps remain ratcheted. No baseline-exact method regressed; no proposals.
- Next: continue with the active `MplForeignMacro` batch; then proceed to other independent Lane A members.

## DynamicType.Concat3Mi (`mirror/a-dynamic-type-concat3mi`)

- Scope: implement private `Concat3Mi()` (`0x06000406`), remove its generated stub, and directly compare the resolved `MethodInfo` signature.
- Original: installed `mplrun.dll` SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Original IL loads `typeof(DynamicType)`, the name `MplConcat3`, and calls `Type.GetMethod(string)`, returning that method. REA inspected member token `0x06000406`; ILSpy C# confirms `typeof(DynamicType).GetMethod("MplConcat3")`.
- Test: `DynamicTypeConcat3MiDifferentialTests` directly invokes the private method in both assemblies and compares declaring type, method name, parameter types, return type and static flag.
- Branch base: `45aee7e` (current main after the Concat2Mi receipt); no overlapping active DynamicType implementation branch found.
- Scoped validation: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-dynamic-type-concat3mi-rerun`. Harness 86, Mpl 188 and Differential 10 passed with no failures/skips; 11 methods compared (all exact), 19 pre-existing stubs, no structure mismatches/uncovered/unsupported, lint 0, integrity complete, ratchet ok, 20/20 mutants killed, no survivors (one `FieldNames` mutant timed out in parallel and was killed again on its single-worker retry). `LOCAL CHECKS: pass · GATE: would pass with gaps (19 missing)`. Summary: `/tmp/quire-validation-dynamic-type-concat3mi-rerun/summary.json`.
- Exact Release IL before/after: `Concat3Mi` was a generated stub and was not baseline-exact before; it is exact after. All ten baseline-exact DynamicType methods remain exact; the current scoped inventory has 11/30 exact methods, with no regressions. No proposals.
- Independent review returned MERGE at implementation tip `2d1d08a0c1e8`.
- Runtime gate: rebased base `45aee7eb3f1d`, gate-validated tip `2d1d08a0c1e8`; passed and merged to main at `2d1d08a`, followed by baseline update `d15149e`. Main was pushed. Gate receipt `/tmp/quire-gate-dynamic-type-concat3mi/gate.json`, summary `/tmp/quire-gate-dynamic-type-concat3mi/run/summary.json`: Harness 86, Mpl 188, Differential 288 passed; 1,116 compared, 1,516 exact, 95 structure-only, 601 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 0 eligible selected mutants. Existing inventory gaps remain ratcheted. No baseline-exact method regressed; no proposals.
- No proposals. Next: continue with the active `MplForeignMacro` batch; then select another unowned Lane A member.

## DynamicType.GetShortName (`mirror/a-dynamic-type-getshortname`)

- Scope: implement the private static `GetShortName(string)` (`0x06000409`), remove its generated stub, and compare direct results against Mathcad.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. A colon after index zero returns the suffix beginning at the colon; a leading colon returns the full input; otherwise a dot after index zero returns the suffix after the dot. If neither separator applies, it returns the input. Null throws at `IndexOf`.
- Test: `DynamicTypeGetShortNameDifferentialTests` directly invokes the private member in both artifacts. Cases cover colon at indexes zero, one and later; a leading colon before a later dot; dot at indexes zero, one and later; trailing dot; plain text; and null.
- Branch base: `790fccc`; clean rebase onto `f29abb6`, implementation tip `ad250ac82665`. Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-dynamic-type-getshortname-rerun`. Run summary: `/tmp/quire-validation-dynamic-type-getshortname-rerun/summary.json`. Harness 86, Mpl 188 and Differential 11 passed with no failures/skips; 12 compared, 18 pre-existing stubs missing, 0 structure mismatches/uncovered/unsupported, 12 exact, lint 0, integrity complete and ratchet ok; 31/31 mutants killed, no survivors (one unrelated `FieldNames` mutant timed out in parallel and was killed on its serial retry). `LOCAL CHECKS: pass · GATE: would pass with gaps (18 missing)`.
- Exact Release IL before/after: 11/30 -> 12/30; `GetShortName` is exact and all 11 previously exact methods remain exact. No proposals.
- Independent review returned MERGE for the complete diff at `f5b4712`; the clean rebase preserved its reviewed hunks.
- Runtime gate passed on `ad250ac82665`, based on `f29abb6d8b27`, then merged to main at `20a80e0` and pushed. Receipt `/tmp/quire-gate-dynamic-type-getshortname/gate.json`, run `05325086-b071-4230-a118-a2e101d7bafe`, summary `/tmp/quire-gate-dynamic-type-getshortname/run/summary.json`: Harness 86, Mpl 188 and Differential 317 passed with no failures/skips; 1120 compared, 1519 exact, 93 structure-only, 597 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 11/11 mutants killed, no survivors/timeouts/crashes. Existing inventory gaps remain ratcheted; no baseline-exact method regressed. No proposals.
- Next: continue the active `MplForeignMacro` batch in its separate worktree; then take another unowned Lane A member.

## DynamicType.Explode (`mirror/a-dynamic-type-explode`)

- Scope: implement private static `Explode(string)` (`0x0600040e`), remove its generated stub, and compare its output arrays directly against Mathcad.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. The body returns `Name.Split(DelimiterString.ToCharArray())`; the initializer sets the delimiter to `_`. `String.Split(char[])` preserves empty entries here. Direct cases cover middle, comma-containing, repeated and trailing delimiters, empty input, no delimiter, and null; the output renderer encodes array length and length-prefixes each element.
- Branch base: `07bc80a`. The first gate attempt rejected an overbroad generated-file hunk that had also removed the adjacent `RemovePrefix` stub attribute; it was restored, and the final diff removes only `Explode`'s stub pair. Corrected scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-dynamic-type-explode-stub-fix`. Run summary: `/tmp/quire-validation-dynamic-type-explode-stub-fix/summary.json`. Harness 86, Mpl 188 and Differential 12 passed with no failures/skips; 13 compared, 17 existing stubs missing, 0 structure mismatches/uncovered/verified-unsupported; all 13 are exact, lint 0, integrity complete and ratchet ok. Mutation 31/31 killed, no survivors/crashes; one unrelated `FieldNames` mutant timed out in parallel and was killed on its serial retry. `Explode` itself has no eligible mutation operators in this run. `LOCAL CHECKS: pass · GATE: would pass with gaps (17 missing)`.
- Exact Release IL before/after: 12/31 -> 13/31; `Explode` is exact and all 12 previously exact methods remain exact. No proposals.
- Independent review returned MERGE for final implementation tip `9ff806b0ae8b`; it verified the injective array renderer and that only Explode's generated stub pair is removed.
- Runtime gate passed on `9ff806b0ae8b`, based on `07bc80aafefd`, then merged to main at `18f780e` and pushed. Receipt `/tmp/quire-gate-dynamic-type-explode/gate.json`, run `99a0e8ab-e1d5-4938-a15b-5b67bc9a545c`, summary `/tmp/quire-gate-dynamic-type-explode/run/summary.json`: Harness 86, Mpl 188 and Differential 318 passed with no failures/skips; 1121 compared, 1520 exact, 93 structure-only, 596 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 0 eligible mutants selected by the gate. Existing inventory gaps remain ratcheted; no baseline-exact method regressed. No proposals.
- Next: continue the active `MplForeignMacro` batch in its separate worktree; then take another unowned Lane A member.

## DynamicType.NameMangle (`mirror/a-dynamic-type-namemangle`)

- Scope: implement private static `NameMangle(string, Type[])` (`0x06000413`), remove its generated stub, and compare direct results against Mathcad.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Starts with `prefix`, loops over `types` in index order, appends `TypeToName(ty)` via `String.Concat(string, string)`, and returns the accumulated text. The verified type-name map returns `_I`, `_F`, `_O`, and `.ref` for unmapped types. Direct cases cover an empty array, mapped and unknown types, mixed order, null prefix and null array.
- Branch base: `a3ff845`. Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-dynamic-type-namemangle`. Run summary: `/tmp/quire-validation-dynamic-type-namemangle/summary.json`. Harness 86, Mpl 188 and Differential 13 passed with no failures/skips; 14 compared, 16 existing stubs missing, 0 structure mismatches/uncovered/verified-unsupported; all 14 are exact, lint 0, integrity complete and ratchet ok. Mutation 35/35 killed, no survivors/crashes; two mutants timed out in parallel and were killed on serial retry. `LOCAL CHECKS: pass · GATE: would pass with gaps (16 missing)`.
- Exact Release IL before/after: 13/31 -> 14/31; `NameMangle` is exact and all 13 previously exact methods remain exact. No proposals.
- Independent review returned MERGE for implementation tip `5f2e31bb2c8e`.
- Runtime gate passed on `5f2e31bb2c8e`, based on `a3ff845ae3fc`, then merged to main at `c96f51a` and pushed. Receipt `/tmp/quire-gate-dynamic-type-namemangle/gate.json`, run `c57d1e17-4464-4689-9074-f4c71ac47031`, summary `/tmp/quire-gate-dynamic-type-namemangle/run/summary.json`: Harness 86, Mpl 188 and Differential 319 passed with no failures/skips; 1122 compared, 1521 exact, 93 structure-only, 595 missing, 103 structure mismatches, 13 uncovered, 43 verified unsupported; lint 0, integrity complete, ratchet ok, 4/4 gate-selected mutants killed with no survivors (one mutant timed out in parallel and was killed on retry). Existing inventory gaps remain ratcheted; no baseline-exact method regressed. No proposals.
- Next: continue the active `MplForeignMacro` batch in its separate worktree; then take another unowned Lane A member.

## Bq.walk_expression (`mirror/a-bq-walk`)

- Scope: direct verification of `Bq`'s existing `walk_expression(MplEnv)` body (`0x0600005b`) and its sibling constructors and `ToString()` so the selected type is fully covered. Source change is only the status attribute; implementation is unchanged.
- Original contract: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Original `walk_expression` is 19 bytes and dispatches to `MplBackquoteMacro.singleton.ExpandCall(env, this)`, then walks the result.
- Direct tests: `BqDifferentialTests.Constructors_ToString_and_walk_expression` compares the object and `Sexpr` constructors with expression/null variants, an unrelated object for the object overload, `ToString()`, and the hostless `walk_expression(null)` failure after resetting the macro caches.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.Bq --diff-filter 'FullyQualifiedName~BqDifferentialTests' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-bq-walk-2`. Run `d180cad7-38dc-4df0-8500-7f947cac0305`: Harness 86, Mpl 188 and Differential 1 passed with no failures/skips; all 4 methods compared, 0 missing/uncovered/structure mismatches/unsupported; all 4 exact; lint 0, integrity complete, ratchet ok; 2/2 mutants killed, no survivors/timeouts/crashes. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 type; the gate validates all]`.
- Exact Release IL before/after: all four `Bq` methods were baseline exact before this batch and remain exact after it; `walk_expression`'s body is unchanged and matches Mathcad byte-for-byte. No proposals.
- Limits: successful `MplMacro.backquote` expansion through Mathcad's external host is not observable here; the direct walk case covers the hostless null-environment failure only. The contract and source evidence record this remaining boundary.
- Independent review: `/root/lane_a_next_research_2` returned MERGE for the Bq diff at `833a892`, confirming the null-environment comparison supports the recorded hostless failure case and that the external expansion limit is explicit. Original batch commit `3d3c507` was rebased into integration commit `1e50609`; Lane A owner. No proposals or user approvals were needed. After `main` advanced, the candidate was rebased onto `4ae034e`; final combined review returned MERGE at `0bd95b2`. The combined Bq/RngVal gate passed and merged as `55a10a6`; see the receipt below. Next: stop Lane A work as requested.
## RngVal matrix conversions (`mirror/a-rngval-value-conversions`)

- Branch base: `b609716`, rebased onto current main `353e4d5` after the focused run; one source, one new differential test, one evidence record and this lane checkpoint.
- Scope: `asVector`, `asMatrix`, `asRealMatrix` and `asRealVector` (`0x060006ee`–`0x060006f1`). The first three call `asRealVector`; that method builds a one-column matrix from the range's virtual `get` values and copies its dimension.
- Original: installed `mplrun.dll` SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. C# and IL were read using `tools/reference/reference.py Mcd.RngVal --il`; all four C# bodies match the reconstructed bodies.
- Direct comparisons: `RngValConversionsDifferentialTests` compares all four methods on ordinary, interpolated and empty ranges, rendering matrix shape, values and dimensions. The helper arguments are the same nondefault ignored values on each side.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp QUIRE_REQUIRE_MATHCAD=1 WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mcd.RngVal --diff-filter 'FullyQualifiedName~RngValDifferentialTests|FullyQualifiedName~RngValConversionsDifferentialTests' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-rngval-conversions-full`. Summary `/tmp/quire-validation-rngval-conversions-full/summary.json`: Harness 86, Mpl 188 and Differential 6 passed with no failures/skips; 26 compared, no missing, structure mismatches, uncovered or verified-unsupported methods; 25 exact, 1 structure-only; lint 0, integrity complete, ratchet ok; 88/88 mutants killed, no survivors/crashes (one timed out in parallel and was killed on retry). `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 type; the gate validates all]`.
- The diagnostic run filtered only to the new test passed its direct fact and killed 5/5 selected mutants, but omitted existing RngVal comparisons and failed the type ratchet. The broad run above is the handoff result.
- Exact Release IL before/after: all four methods were in the baseline exact set and remain exact; RngVal remains 25/26 exact with 1 structure-only. No baseline-exact method regressed. No proposals.
- Independent review returned MERGE for final batch tip `5bd4f67`; the combined diff also returned MERGE. Original batch commit `704c487` and evidence correction `5bd4f67` are integrated as `774310f` and `09a0874`; Lane A owner. No proposals or user approvals were needed. Combined candidate was rebased onto `4ae034e`; final review returned MERGE at `0bd95b2`. The combined gate passed and merged as `55a10a6`; see the receipt below. The user asked to stop Lane A work after this batch and Bq are merged and pushed.
- Next: stop Lane A work as requested; resume the queue only when directed.

## Bq and RngVal combined runtime gate receipt

- Integration candidate: `mirror/integration-bq-walk`, base `4ae034e`, reviewed tip `0bd95b2`. It combined Bq original commit `3d3c507` as `1e50609` and RngVal commits `704c487`/`5bd4f67` as `774310f`/`09a0874`. Independent review returned MERGE for the final full diff; no proposals or user approvals were pending.
- Runtime gate passed with existing ratcheted inventory gaps and merged the candidate to main at `55a10a6`; main was pushed to origin. Gate receipt: `/tmp/quire-bq-rngval-merge-gate/gate.json`. Validation run `2702bea5-76fe-442a-bca7-ac38b92229cf`; summary: `/tmp/quire-bq-rngval-merge-gate/run/summary.json`.
- Tests: Harness 94, Mpl 188 and Differential 363 passed, with no failures or skips. Method audit: 1,215 compared, 1,528 exact, 91 structure-only, 502 missing, 103 structure mismatches, 13 uncovered and 43 verified-unsupported; lint 0, integrity complete and ratchet OK. All 7/7 selected mutants were killed; one timed out in parallel and was killed on serial retry, with no survivors or crashes.
- Both Bq and all four RngVal conversion methods remain exact; no baseline-exact method regressed. Gate report retains the pre-existing completeness gaps above; no new exemption proposals were used.
- The user asked to stop after these batches were merged and pushed. Lane A work stops here; no next member was started.

## Resumed Lane A: DynamicType.TupleNameMangle (`mirror/a-dynamic-type-tuple-name-mangle`)

- The user resumed the Lane A queue and supplied a native ext4 worktree pipeline. This batch uses `/root/src/worktrees/a-dynamic-type-tuple-name-mangle`, branch `mirror/a-dynamic-type-tuple-name-mangle`, based on current `main` `50c84d8`.
- Scope: implement the public `TupleNameMangle(Type[])` (`0x06000415`) and `TupleNameMangle(int)` (`0x06000416`) overloads in `src/Quire.Mpl/Run/DynamicType.NameMangle.cs`, remove only their generated stub pairs, add direct differential cases and evidence.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; read via `tools/reference/reference.py Mpl.Run.DynamicType --il`. The array overload calls `NameMangle(TuplePrefix, types)`. The integer overload allocates `Type[n]`, caches `typeof(object)` in a local, fills the array with an indexed loop, and delegates to the array overload. Both reconstructed bodies follow the original C# and Release opcode sequence.
- Direct test: `DynamicTypeTupleNameMangleDifferentialTests` compares empty, mapped, fallback, mixed and null type arrays; and counts 0, 1, 3 and -1, including matching array-allocation exceptions. Both methods are directly invoked against the installed Mathcad artifact.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH QUIRE_REQUIRE_MATHCAD=1 python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.DynamicType --diff-filter 'FullyQualifiedName~DynamicType' --jobs 2 --results /tmp/quire-validation-dynamic-type-tuple-name-mangle-type`. Harness 94, Mpl 188 and Differential 15 passed with no failures or skips; 16 methods compared, 14 pre-existing methods remain missing, 0 structure mismatches/uncovered/verified-unsupported; all 16 compared methods exact, lint 0, integrity complete and ratchet OK. Mutation is deferred to the merge gate under the supplied pipeline. Summary: `/tmp/quire-validation-dynamic-type-tuple-name-mangle-type/summary.json`.
- Exact Release IL before/after: 14/31 -> 16/33; both new methods match Mathcad exactly and all 14 previously exact methods remain exact. No proposals.
- Independent full-diff review returned MERGE at `73ee62059078`. Runtime gate passed on base `50c84d8` / tip `73ee62059078` and merged to main at `a6121c2`; main was pushed. Receipt `/tmp/quire-gate-results-cm403w81/gate.json`, run `27d55cfc-db75-4a75-8039-b8a2e7e3d409`; summary `/tmp/quire-gate-results-cm403w81/run/summary.json`. Harness 94, Mpl 188 and Differential 365 passed with no failures/skips; 1,217 methods compared, 1,530 exact, 91 structure-only, 500 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. All 5/5 selected mutants were killed; one timed out in parallel and was killed on serial retry, with no survivors or crashes. No baseline-exact method regressed; no proposals.
- Next: continue with the independent `MplForeignMacro` work and then another unowned Lane A member.

## Environment helper status verification (`mirror/a-env-verification`)

- Scope: align existing differential evidence and source statuses for `BindingInfo`, `MacroBindingInfo`, `LocalEnv`, selected `MplEnv` helpers, `MplMacro` error helpers, and `MplSymbolMacro`. Runtime bodies and tests are unchanged.
- Original evidence: inspected `Mpl.Run.MplEnv`, `BindingInfo`, `LocalEnv`, `MplMacro`, and `MplSymbolMacro` C#/IL from the installed `mplrun.dll` 12.0.1.0 (SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`). Existing `EnvDifferentialTests` directly compares the selected constructors, lookups, mutations, macro behavior and error helpers; `MplRunMplMacroMechanicalDifferentialTests` covers `MplMacro.ToString()`.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.MplEnv --method-type Quire.Mpl.Run.BindingInfo --method-type Quire.Mpl.Run.LocalEnv --method-type Quire.Mpl.Run.MplMacro --method-type Quire.Mpl.Run.MplSymbolMacro --diff-filter 'FullyQualifiedName~EnvDifferentialTests|FullyQualifiedName~MplRunMplMacroMechanicalDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-env-verification-final`. Harness 94, Mpl 188 and Differential 8 passed; 25 compared, 1 declaration-only, 13 pre-existing missing and 1 pre-existing uncovered; 37 methods matched exact IL, 0 structure-only, lint 0, integrity complete, ratchet ok. The final run is complete with no proposed exemptions. Mutation is deferred to the runtime gate. `LoadMacro` remains `MirrorTested`: a root `LocalEnv` throws before the non-keyword/no-binding null return can be exercised; this known uncovered line remains at the baseline count.
- Exact Release IL before/after: 39/39 baseline-exact methods remain exact; all source changes are status attributes only. No method body changed and no exact method regressed. No proposals.
- Independent review returned MERGE on `e2f281a`. The runtime gate passed and merged the batch to main at `a8357c5109bf`; main was pushed. Receipt: `/tmp/quire-gate-results-kkq0vim6/gate.json`. Gate tests: Harness 94, Mpl 188, Differential 367 passed with no failures or skips; 1,250 methods compared, 1,530 exact, 91 structure-only, 467 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete, ratchet OK. No methods were selected for mutation because the changes only updated statuses/evidence. No exact Release-IL regressions; all 39 baseline-exact methods in the selected types remained exact. Next: continue with the independent `MplForeignMacro` work, then another unowned Lane A member.

## Historical batch results

These are results recorded at the named historical merges, not current completion claims. The old
Unit 4 claim of full settlement was a classification checkpoint and is superseded by the audit above.

| Unit | Types / Scope | Branch | Status | Direct Comparisons | Structure Mismatches | Lint | Ratchet | Mutants Killed |
|---|---|---|---|---|---|---|---|---|
| **Unit 1** | Sexpr family (18 types) | `main` (`929fc6d`) | **Merged to main** via `merge_gate.py --apply` | 157 | 0 | 0 | ok | 106 / 107 (1 accepted survivor) |
| **Unit 2** | `Cons`, `VecBuilder`, `True`, `Tuple`, `MplSignature` | `main` (`20b2017`) | **Merged to main** via `merge_gate.py --apply` | 40 | 0 | 0 | ok | 87 / 87 (0 survivors) |
| **Unit 3a** | `Rational`, `Utilities` | `main` (`4fc4a72`) | **Merged to main** via `merge_gate.py --apply` | 31 | 0 | 0 | ok | 114 / 117 (3 accepted survivors) |
| **Unit 3b** | `AppException`, `Type`, `UnitSystem`, `RuntimeOptions` | `main` (`d7b5a63`) | **Merged to main** via `merge_gate.py --apply` | 165 | 0 | 0 | ok | 655 / 678 (23 accepted survivors) |
| **Unit 4** | Remaining batch 2e unclassified members | `main` | **Historical classification checkpoint only; current audit gaps remain** | Not established | Not established | Not rerun | Not rerun | N/A |


Historical mutant proposals and reasoning remain in Git history. They are not a current approval queue;
consult `accepted-mutants.txt` for existing accepted keys. Under current PLAN rules, test-host crashes
count as killed mutants and must never be proposed for equivalence acceptance.


## Sexpr.walk_definitions (`mirror/a-sexpr-walk-definitions`)

- Scope: directly verify private static `walk_definitions(Cons, MplEnv)` (`0x0600001c`) with successful declaration forms that let the full traversal execute. Runtime body unchanged; status attribute promoted to Verified.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; inspected by `tools/reference/reference.py Mpl.Run.Sexpr --il`. The method loops through `Cons` cells, replaces each `Field1` with `walk_definition(Field1 as SCons, env)`, then returns the original head.
- Direct cases: `SexprDifferentialTests.Sexpr_base_and_errors` compares null, existing symbol-headed error, one and two successful `deftype_keyword` declarations, and a successful first declaration followed by a throwing second declaration. The two-cell error case makes advancing to the second declaration observable, and explicit assertions verify the original head is returned on both artifacts. The current keyword overrides return each input form, so these cases do not distinguish the `Field1` assignment from leaving that same form in place.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mpl.Run.Sexpr --diff-filter 'FullyQualifiedName~SexprDifferentialTests' --mutate --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-sexpr-walk-definitions-reviewed`. Harness 94, Mpl 188 and Differential 10 passed, no failures/skips; 28 methods compared, 0 declaration-only, 1 existing missing abstract constructor, 5 pre-existing uncovered throw-tail methods, 0 structure mismatches/verified-unsupported, 33 exact and 1 structure-only, lint 0, integrity complete, ratchet OK. Mutation: 25/25 killed, no survivors, timeouts or crashes. Summary `/tmp/quire-validation-sexpr-walk-definitions-reviewed/summary.json`; `LOCAL CHECKS: pass · GATE: would pass with gaps (1 missing; 5 uncovered)`.
- Exact Release IL before/after: `walk_definitions` was not baseline-exact before and remains structure-only after; its body is unchanged. All 33 exact methods in the scoped inventory remain exact; no baseline-exact method regressed. No proposals.
- Independent review returned MERGE for the complete diff at `fab375a`. The reviewed branch rebased from its prior base `bd5fddd` onto `cc082ef`; the runtime gate passed with existing ratcheted gaps at tip `396bb823` and main was pushed. Gate receipt `/tmp/quire-gate-results-_0q2aaim/gate.json`, run summary `/tmp/quire-gate-results-_0q2aaim/run/summary.json`: Harness 94, Mpl 188 and Differential 369 passed with no failures/skips; 1,282 methods compared, 1,530 exact, 91 structure-only, 435 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. Mutation selected 137, killed 136, with 1 existing accepted survivor and no unaccepted survivors, timeouts or crashes. The scoped `walk_definitions` result remains structure-only; all 33 scoped exact methods remain exact. No proposals. Next: continue the independent active `MplForeignMacro` batch, then select another unowned Lane A member.


## UnitSystem helper verification (`mirror/a-unit-system-helper-verification`)

- Scope: promote `GetDimensionIndex(Type)` (`0x060005db`), `GetDimensions(string)` (`0x060005dc`), `Inverse(double[,])` (`0x060005dd`), `IsSmall(double)` (`0x060005de`), and both `Count` overloads (`0x060005df`–`0x060005e0`) to Verified. Bodies and tests are unchanged; existing direct Mathcad cases and coverage support the status change.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Read via `tools/reference/reference.py Mcd.UnitSystem --il`; the six C# bodies match the reconstruction. `GetDimensionIndex` binary-searches sorted dimensions, `GetDimensions(string)` delegates to `Type.Parse(...).GetDimensionTypeProduct()`, `Inverse` performs the existing column-wise Gauss–Jordan path, `IsSmall` uses strict ±1e-12 comparisons, and each `Count` counts nonzero elements.
- Direct cases: `UnitSystemDifferentialTests.UnitSystem_private_methods_and_Default` directly compares all six methods. It covers all dimensions plus unknown lookups, two parsed signatures, 16 inverse matrices including pivoting and singular cases, values on/around the `IsSmall` threshold and NaN, and zero/nonzero integer and double vectors. The previous main gate report records each method as directly compared with every line and branch executed.
- Branch base: `733f58f`; current exact-IL inventory reports 27 exact and 5 structure-only methods across `UnitSystem`. All six selected methods are exact and fully covered; no source body changes, so existing exact methods are preserved.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mcd.UnitSystem --diff-filter 'FullyQualifiedName~UnitSystemDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-unit-system-helper-verification`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; all 32 methods compared, 27 exact and 5 structure-only, 0 missing/uncovered/structure mismatches/unsupported; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-unit-system-helper-verification/summary.json`.
- Exact Release IL before/after: all six selected methods were baseline-exact and remain exact; all 27 exact methods in UnitSystem remain exact, with 5 pre-existing structure-only methods. The source edits change statuses only; no bodies changed. No proposals.
- Independent review returned MERGE for the complete diff at `f503d29`. The runtime gate passed with existing ratcheted gaps at tip `f503d296` and main was pushed. Receipt `/tmp/quire-gate-results-xveo_b_p/gate.json`, summary `/tmp/quire-gate-results-xveo_b_p/run/summary.json`: Harness 94, Mpl 188 and Differential 369 passed with no failures/skips; 1,282 methods compared, 1,530 exact, 91 structure-only, 435 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation because the batch only changed statuses/evidence. All six selected methods remain exact; no proposals. Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another unowned Lane A member.

## Mcd.Type.Take verification (`mirror/a-type-take-verification`)

- Scope: promote private static `Take(Power[], int, int)` (`0x060005ac`) from Tested to Verified. Its implementation and the existing differential cases are unchanged.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; the artifact-bound evidence records the slice and whole-array identity contract. Existing `TypeDifferentialTests.Type_core_members_and_parsing` directly compares whole, prefix, empty, suffix, and an out-of-range count that throws. Its result observer checks both identity (`SAME`/`NEW`) and power contents. These cases support the recorded cases only; other indices/counts remain unestablished.
- Branch base: `32be392`. Target `Take` is structure-only before/after (not baseline-exact); its body is unchanged. Focused Release audit: 11 exact and 1 structure-only method in `Mcd.Type`; no baseline-exact method regressed.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 /root/src/worktrees/a-type-take-verification/tools/validation/validate.py --profile focused --method-type Quire.Mcd.Type --diff-filter 'FullyQualifiedName~TypeDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-type-take-verification`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; 11 methods compared, 2 declaration-only, 1 existing missing abstract constructor, 0 uncovered or structure mismatches, 11 exact and 1 structure-only; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass with gaps (1 missing) [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-type-take-verification/summary.json`.
- Independent review returned MERGE at `9cd3ac5`, correcting the test-case description before approval. The runtime gate passed and pushed `9cd3ac5` to main; receipt `/tmp/quire-gate-results-ey0b19o4/gate.json`, run summary `/tmp/quire-gate-results-ey0b19o4/run/summary.json`. Harness 94, Mpl 188 and Differential 372 passed with no failures/skips; 1,312 methods compared, 1,530 exact, 91 structure-only, 405 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation (`test class dependencies: none`) because the verified batch changed only a status attribute and evidence; no proposals. `Take` remains structure-only and all 11 exact methods in the scoped Type inventory remain exact. Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another unowned Lane A member.

## Mcd.Type.Parser.ParseExponent verification (`mirror/a-type-parse-exponent`)

- Scope: promote private `Parser.ParseExponent()` (`0x06000783`) from Tested to Verified. Source body and existing direct differential cases are unchanged.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; `tools/reference/reference.py Mcd.Type --il` confirms an optional minus, optional integer token parsed by `int.Parse`, default exponent 1, and natural overflow exceptions. `TypeDifferentialTests.Factory_Lexer_Parser_Printer_Formatter` directly compares `-12`, `5`, empty, `0`, `99999999999`, and `junk`; other sign/digit strings are not established.
- Branch base: `7a423ee`. The target was baseline-exact before and remains exact; no source body changed.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 /root/src/worktrees/a-type-parse-exponent/tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.Type+Parser' --diff-filter 'FullyQualifiedName~TypeDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-type-parse-exponent`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; all 9 methods compared, 7 exact and 2 structure-only, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-type-parse-exponent/summary.json`.
- Independent review returned MERGE at `4624726`. The runtime gate passed and pushed `4624726` to main; receipt `/tmp/quire-gate-results-ittc0mm0/gate.json`, run summary `/tmp/quire-gate-results-ittc0mm0/run/summary.json`. Harness 94, Mpl 188 and Differential 372 passed with no failures/skips; 1,312 methods compared, 1,530 exact, 91 structure-only, 405 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation (`test class dependencies: none`) because the batch changed only a status attribute and evidence; no proposals. `ParseExponent` remains baseline-exact and the 7 exact methods in the scoped Parser inventory remain exact. Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another unowned Lane A member.

## Mcd.Type.Parser.Parse verification (`mirror/a-type-parser-parse`)

- Scope: promote `Parser.Parse()` (`0x06000780`) from Tested to Verified. Source body and existing direct differential cases are unchanged.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; `tools/reference/reference.py Mcd.Type --il` shows an initial application followed by an optional right-recursive function arrow. `TypeDifferentialTests.Factory_Lexer_Parser_Printer_Formatter` directly compares function, product, tuple, list, application, vector-dimension, and variable/constant-product signatures. Other signatures are not established by these cases.
- Branch base: `00f9051`. `Parse()` was structure-only before and remains structure-only; it is not baseline-exact and its source body is unchanged. The focused Parser Release audit reports 7 exact and 2 structure-only methods; no exact method regressed.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 /root/src/worktrees/a-type-parser-parse/tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.Type+Parser' --diff-filter 'FullyQualifiedName~TypeDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-type-parser-parse`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; all 9 methods compared, 7 exact and 2 structure-only, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-type-parser-parse/summary.json`.
- Independent review returned MERGE at `9e98d50`. The runtime gate passed and pushed `9e98d50` to main; receipt `/tmp/quire-gate-results-d1cf8042/gate.json`, run summary `/tmp/quire-gate-results-d1cf8042/run/summary.json`. Harness 94, Mpl 188 and Differential 372 passed with no failures/skips; 1,312 methods compared, 1,530 exact, 91 structure-only, 405 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation (`test class dependencies: none`) because the batch changed only a status attribute and evidence; no proposals. `Parse()` remains structure-only; all 7 exact methods in the scoped Parser inventory remain exact. Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another unowned Lane A member.

## Mcd.Type Lexer and Parser helpers (`mirror/a-type-lexer-parser-verification`)

- Scope: promote `Lexer.ctor(string)`, `Lexer.GetToken(out string)`, `Parser.ctor(string)`, `Parser.ParseApplication()`, `Parser.ParseAtomic()`, `Parser.IsNextToken(string)`, `Parser.GetNextToken()`, `Parser.GetNextTokenIf(string)`, and `Parser.SkipToken(char)` to Verified. Implementations and direct cases are unchanged. Corrected the `Mcd.Type` evidence narrative to say that `TypeDifferentialTests` directly invokes many private nested members through reflection as well as exercising them indirectly.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; read using `tools/reference/reference.py Mcd.Type --il`. `Lexer.GetToken` is directly compared over 20 token steps across two inputs; `ParseApplication` has five direct signatures; `ParseAtomic` has 12 direct atomic forms. The token-state helpers have single recorded direct cases (including a nonmatching `GetNextTokenIf` case); all evidence rows limit the claim to their listed inputs.
- Branch base: `ba8fc98`. The 9 selected bodies are unchanged. Focused Release audit: 9 exact and 2 structure-only methods across `Lexer` and `Parser`; exact before/after classifications are unchanged, with no exact method regressed. The two structure-only methods are `Parser.Parse()` and `Parser.GetNextTokenIf(string)`; neither is baseline-exact.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 /root/src/worktrees/a-type-lexer-parser-verification/tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.Type+Lexer' --method-type 'Quire.Mcd.Type+Parser' --diff-filter 'FullyQualifiedName~TypeDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-type-lexer-parser-verification`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; all 11 methods compared, 9 exact and 2 structure-only, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 2 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-type-lexer-parser-verification/summary.json`.
- Independent review returned MERGE at `7b5f506`, including the corrections to the private-helper and token-state descriptions. The runtime gate passed and pushed `7b5f506` to main; receipt `/tmp/quire-gate-results-aqdtojkv/gate.json`, run summary `/tmp/quire-gate-results-aqdtojkv/run/summary.json`. Harness 94, Mpl 188 and Differential 372 passed with no failures/skips; 1,312 methods compared, 1,530 exact, 91 structure-only, 405 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation (`test class dependencies: none`) because the batch changed only status attributes and evidence; no proposals. The 9 exact methods in the selected Lexer/Parser inventory remain exact; both structure-only members remain non-exact with unchanged bodies. Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another unowned Lane A member.

## Mcd.UnitSystem.Default direct verification (`mirror/a-unit-default-verification`)

- Scope: promote the two private nested `Default` constructors, `GetHashCode`, `Equals`, and `CompareTo` to Verified. The source bodies and existing differential cases are unchanged.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; `tools/reference/reference.py Mcd.UnitSystem --il` confirms direction normalization by signed GCD, hash over normalized coordinates, length-directed equality, and descending order comparison.
- Direct cases: `UnitSystemDifferentialTests.UnitSystem_Default_methods` compares both constructors and hashes for 24 chosen vectors, including empty/zero vectors, positive and negative coordinates, common divisors and varied ranks. It compares eight equality cases and three ordering cases. The evidence rows state the remaining vector/type limits.
- Branch base: `a679ebc`. All five selected bodies are unchanged; Release audit before/after is 4 exact and 1 structure-only methods in `UnitSystem+Default`. No baseline-exact method regressed.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 /root/src/worktrees/a-unit-default-verification/tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.UnitSystem+Default' --diff-filter 'FullyQualifiedName~UnitSystemDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-unit-default-verification`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; all 5 methods compared, 4 exact and 1 structure-only, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-unit-default-verification/summary.json`.
- Independent review returned MERGE at `2ef5821`. The runtime gate passed and pushed `2ef5821` to main; receipt `/tmp/quire-gate-results-s56g28da/gate.json`, run summary `/tmp/quire-gate-results-s56g28da/run/summary.json`. Harness 94, Mpl 188 and Differential 377 passed with no failures/skips; 1,351 methods compared, 1,530 exact, 91 structure-only, 366 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation (`test class dependencies: none`) because the batch changed only status attributes and evidence; no proposals. Four selected methods remain exact and one remains structure-only; no baseline-exact method regressed. Next: continue the active `MplForeignMacro` batch in its separate worktree, then select another unowned Lane A member.

## Mcd.Type Printer and Formatter direct verification (`mirror/a-type-formatters-verification`)

- Scope: promote the five `Printer` methods and five `Formatter` methods to Verified. Implementations and direct differential tests are unchanged.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; inspected with `tools/reference/reference.py Mcd.Type --il`. Existing `TypeDifferentialTests.Factory_Lexer_Parser_Printer_Formatter` and `Formatter_OnFunction` invoke all selected private members directly and compare builder output/state. The evidence rows enumerate the tested terms and limits.
- Branch base: `10b4d82`. No method bodies or test code changed. The Release audit classified 8 of the 10 methods exact and 2 structure-only both before and after; `Formatter.OnFunction(Type,int)` and `Formatter.Visit(Product)` remain structure-only. No baseline-exact method regressed.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 /root/src/worktrees/a-type-formatters-verification/tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.Type+Printer' --method-type 'Quire.Mcd.Type+Formatter' --diff-filter 'FullyQualifiedName~TypeDifferentialTests' --jobs 2 --dotnet /tmp/quire-dotnet-dual --results /tmp/quire-validation-type-formatters-verification-final`. Harness 94, Mpl 188 and Differential 4 passed with no failures/skips; all 10 methods compared, 8 exact and 2 structure-only, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete and ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 2 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-type-formatters-verification-final/summary.json`.
- Independent review returned MERGE at `bc43e0d`. The runtime gate passed and pushed `bc43e0d` to main; receipt `/tmp/quire-gate-results-5qwo3y_k/gate.json`, run summary `/tmp/quire-gate-results-5qwo3y_k/run/summary.json`. Harness 94, Mpl 188 and Differential 377 passed with no failures/skips; 1,351 methods compared, 1,530 exact, 91 structure-only, 366 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. No methods were selected for mutation (`test class dependencies: none`) because the batch changed only status attributes and evidence; no proposals. Eight selected methods remain exact and two remain structure-only; no baseline-exact method regressed. Next: continue with the independent `MplForeignMacro` batch in its separate worktree, then take another unowned Lane A member.

## Mcd.RuntimeOptions.RuntimeOptionsInitializer constructor verification (`mirror/a-runtime-options-initializer-verification`)

- Scope: promote the nested `RuntimeOptionsInitializer.ctor()` (`0x060007e7`) to Verified after adding before/after state equality around its existing direct Mathcad comparison. The state includes the option counter, all RuntimeOptions thread statics, the default system and group, and `Dim.s_vec`.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; inspected with `tools/reference/reference.py Mcd.RuntimeOptions --il`. Direct constructor comparison plus full pre/post state comparison establishes the constructor's recorded behavior.
- Branch base: `a81ee7d`. The target was baseline-exact before and remains exact after (`ae03250635c7eadf8977c69eac41aac32d8b4d37c45433e1c26342b4b14bbd88`); no production body changed.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type 'Quire.Mcd.RuntimeOptions+RuntimeOptionsInitializer' --diff-filter 'FullyQualifiedName~RuntimeOptionsDifferentialTests' --dotnet /tmp/quire-dotnet-dual`. Harness 94, Mpl 188 and Differential 3 passed with no failures/skips; the only selected method was compared and exact, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete, ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-5hr6jglg/summary.json`.
- Independent review returned MERGE at `8032775`. The runtime gate passed and pushed `8032775` to main; receipt `/tmp/quire-gate-results-3w2b9x7b/gate.json`, run summary `/tmp/quire-gate-results-3w2b9x7b/run/summary.json`. Harness 94, Mpl 188 and Differential 377 passed with no failures/skips; 1,351 methods compared, 1,530 exact, 91 structure-only, 366 missing, 103 structure mismatches, 13 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. Mutation testing ran 79 mutants across 11 methods: 77 killed, 2 previously accepted survivors, zero live survivors, timeouts or crashes. The selected nested initializer constructor stayed exact before/after and no baseline-exact method regressed; no proposals. Next: continue the active `MplForeignMacro` batch in its separate worktree; avoid its files while it is owned, then select another unowned Lane A member.

## Mcd.Dim private constructor verification (`mirror/a-dim-private-constructors`)

- Scope: promote the private `Dim(int[])` and `Dim()` constructors from Tested to Verified. Existing direct constructor comparisons cover a fresh coordinate vector, duplicate-key exception, and uninterned marker; an additional assertion checks that `Dim(int[])` retains the exact input array on both artifacts.
- Original: `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`; inspected with `tools/reference/reference.py Mcd.Dim --il`. Constructor rows and recorded limits are in `docs/mirror/evidence/Mcd.Dim.md`.
- Branch base: `5f54991`. Both constructor bodies were baseline-exact and remain exact: array constructor `6b9b7bdea8d563dc71bcd08e4bfbe1d40659cce09a8c92e3ba74bf40743cbda3`; parameterless constructor `2e9b765699a3f56e5c86a5f39d2bff1bafa291a0f8cda355a186d8b8d107563d`.
- Scoped command: `DOTNET_ROOT=$HOME/.dotnet PATH=$HOME/.dotnet:$HOME/.dotnet/tools:$PATH EnableWindowsTargeting=true TMPDIR=/tmp WSLENV=QUIRE_REQUIRE_MATHCAD python3 tools/validation/validate.py --profile focused --method-type Quire.Mcd.Dim --diff-filter 'FullyQualifiedName~DimDifferentialTests' --dotnet /tmp/quire-dotnet-dual`. Harness 94, Mpl 188 and Differential 8 passed with no failures/skips; all 33 methods in the selected type compared, 27 exact and 6 structure-only, no missing/declaration-only/uncovered/structure mismatch/unsupported; lint 0, integrity complete and ratchet OK. `LOCAL CHECKS: pass · GATE: would pass [scoped to 1 types; the gate validates all] [mutation deferred to gate]`. Summary `/tmp/quire-validation-xy9jk5gq/summary.json` (rerun after adding the marker-array freshness assertions).
- Initial review requested a direct proof that the parameterless constructor allocates a fresh vector; added reference-identity checks against all five static marker arrays, then reran validation successfully. Independent re-review returned MERGE at `fe4957f`. The runtime gate passed and pushed `94763ea` to main; receipt `/tmp/quire-gate-results-pq6kh5tw/gate.json`, run summary `/tmp/quire-gate-results-pq6kh5tw/run/summary.json`. Harness 94, Mpl 188 and Differential 385 passed with no failures/skips; 1,364 methods compared, 1,530 exact, 91 structure-only, 350 missing, 103 structure mismatches, 16 uncovered and 43 verified unsupported; lint 0, integrity complete and ratchet OK. Mutation testing ran 151 mutants across 25 methods: 150 killed, 1 accepted survivor, no live survivors; six initial timeout entries were retried (seven retries total), and the completed gate recorded one crash. Both constructors stayed exact before/after, with no baseline-exact method regressed; no proposals. Next: continue the active `MplForeignMacro` batch in its separate worktree; avoid its files while it is owned, then select another unowned Lane A member.
