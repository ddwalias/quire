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
