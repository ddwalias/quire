# Lane B checkpoint

## Active handoff

### Active Batch: MatVal views and export batch
- **Branch:** `mirror/b-matval-views`
- **Scope (28 methods in `src/Quire.Mpl/Mcd/MatVal.Views.cs`):**
  - View constructors and trimming: `MatVal(MatVal, int)` (0x06000493), `TrimPtrs(Hashtable)` (0x06000494), `MatVal(MatVal, int, int, int, int)` (0x0600049c), `SubMatrix(uint, uint, uint, uint)` (0x0600049d), `SubMatrixPtrs(MatVal, int, int, int, int)` (0x0600054e).
  - Column and row slicing: `Column(int)` (0x06000497), `real_column(int)` (0x06000498), `mcd_sym_column(object, int)` (0x06000495), `mcd_column(object, int)` (0x06000496), `mcd_sym_row(object, int)` (0x06000499), `mcd_row(object, int)` (0x0600049a), `Row(int)` (0x0600049b).
  - Transposition operations: `TransposeArr(double[])` (0x06000533), `TransposeReal()` (0x06000548), `TransposeImag()` (0x06000549), `Transpose()` (0x06000523), `set_ptrs_2_transpose` (0x06000551), `TransposePtrs` (0x06000552), `set_ptrs_2_reverse` (0x06000553), `ReversePtrs` (0x06000554).
  - Tuples and elements: `asTuple(uint, uint, uint)` (0x0600046d), `get_elements(uint, uint)` (0x060004c4), `get_elements()` (0x060004c5).
  - Comparison and string conversion: `Equals(object)` (0x06000472), `nearly_equal(DimVal, double)` (0x06000473), `ToString()` (0x06000471).
  - Immutability and export: `asImmutable()` (0x06000528), `Fix()` (0x06000529), `Fix(object)` (0x0600052a), `asResult()` (0x0600052b), `FixForExport(object)` (0x0600052c).
  - Nested pointer trimming context: `TrimSubMatrixPtrs` constructor (0x0600073c), `set_submatrix_ptrs` (0x0600073d).
- **Goal:** Resolve remaining IL structure differences against Mathcad Prime 12.0.1.0 `mplrun.dll`, achieve 100% exact Release IL matching across all methods, 0 lint findings, 0 ratchet regressions, and 100% mutation kill rate.

### Maintainer-gated handoff: MatDim makeArrayDim and matrix dimension factories
- **Branch:** `mirror/b-matdim-makearraydim` (at `0d400cb5`)
- **Status:** Complete implementation and verification. Pushed to origin. 144/147 mutants killed; 3 provably equivalent loop-init mutants documented with mathematical equivalence proof for maintainer review in `accepted-mutants.txt`.

### Maintainer-gated handoff: Generic Function.app differential coverage and equivalent mutants
- **Branch:** `mirror/b-function-app-coverage` (at `d61385d9`)
- **Status:** Complete differential coverage implementation. Pushed to origin. 144/158 mutants killed; 14 provably equivalent mutants documented with mathematical equivalence proof for maintainer review in `accepted-mutants.txt`.

### Maintainer-gated handoff: Engine mailbox and interfaces batch
- **Branch:** `mirror/b-engine-mailbox` (at `83f96c4a`)
- **Status:** Complete implementation and verification. Pushed to origin. Renamed differential tests to `EngineDataMechanicalDifferentialTests.cs` to prevent non-Mpl compilation in focused profile.

Everything below this summary is historical context and receipts, not a queue of unfinished work.

## Historical batch index

Checkpoint base: Linux main `28bee7883ed054238e55e8efb8b2fc63b2f21ae8`. Lane B history is retained compactly here; older worktree results are historical.

| Batch | Main status |
|---|---|
| Function / FunctionUtilities, function matrix bridge and follow-up | Merged after passing runtime gates; Function evidence refresh recorded at `26e402e`, follow-up at `8da5693`. |
| Engine interfaces | Merged; focused gate passed. |
| MatVal collection/growth and views/export | Merged after passing runtime gates at `e2532e3` and `459eb23`. Evidence records nine view/export structure differences. |
| MatVal pointer properties | Merged to main at `59df412` (`e68e5ac`). Five pointer/uniformity properties; 40 direct comparisons. |
| MatVal dimension properties | Merged to main at `e36fe93` (`f51e281`). `IsDimless`, `IsUniformDimOf`, `PhysicalDimension`, `UniformDimension`; 31 direct comparisons. |
| MatVal pointer helper remediation | Merged to main at `d1f7449` (`5674094`). Four pointer helpers; 51 mutants killed. |
| MatVal CanonicalizePtrs | Merged to main at `5026ad8` (`b3a12ce`). 18 direct comparisons. |
| MatVal ReinsertPtrs / ReinsertPtrs1 | Merged to main at `e4a2af9` (`caad393`). 23 direct comparisons. |
| MatVal contains(object) | Merged to main at `278aa9b` (`fb34932`). 15 direct comparisons. |
| DimVal.ApplyMatVal | Merged to main at `dffea4d` via `mirror/b-dimval-apply-current` (`a00b684`). 5 direct comparisons; stub removed. |
| MatVal ColDimArray / RowDimArray | Merged to main at `f447eed` (`339fcad`). 22 direct comparisons. |
| MatVal IsRange / IsIntegral accessors | Merged to main at `3dda994` (`78d636f`). Four accessors; exact IL; 18 direct comparisons. |
| MatVal asRealVector(object) | Merged to main at `dc92792` (`f0ae439`). Exact IL; 9 direct comparisons. |
| MatDim RowDimensionArray / ColDimensionArray | Merged to main at `2930f1a` (`5bd227c`). Exact IL; 8 direct comparisons. |
| FunctionUtilities but / take | Merged to main at `27c06a0` (`6e27dc2`). Exact IL; 17 direct comparisons. |
| MatDim base getters | Merged to main at `e38ab20` (`3138ad2`). Exact IL; 8 direct comparisons. |
| MatDim c_dim constructor / factory | Merged to main at `c4529f9` (`93d9ee2`). Exact IL; 8 direct comparisons. |
| MatDim r_dim constructor / factory | Merged to main at `7ee2d00` (`3c83bbf`). Exact IL; 8 direct comparisons. |
| MatDim rc_dim constructor / factory | Merged to main at `67615ea` (`a63d30e`). Exact IL; 12 direct comparisons. |
| MatDim crazy_dim constructor / factory | Merged to main at `d2f8f67` (`cb23481`). Exact IL; 12 direct comparisons. |
| MatDim c_dim equality | Merged to main at `0167351` (`6434c8a`). Exact IL; 13 direct comparisons. |
| MatDim r_dim equality | Merged to main at `ae8e99b` (`8aad7a6`). Exact IL; 9 direct comparisons. |
| MatDim rc_dim equality | Merged to main at `8f84097` (`8a1bbc1`). Exact IL; 13 direct comparisons. |
| MatDim crazy_dim equality | Merged to main at `561f85b` (`0cf9fef`). Exact IL; 16 direct comparisons. |
| MatDim uniform_dim equality | Merged to main at `2d7c3ae` (`74c9787`). Exact IL; 12 direct comparisons. |
| MatVal real classifiers | Merged to main at `28bee78` (`ad7e8b4`). 5 classifiers; exact IL; 46 direct comparisons. |
| MatVal assertions and casts | Merged to main at `41f234b` (`50a27be`). 7 methods; exact IL; 30 direct comparisons. |
| MatDim uniform_dim construction | Merged to main at `8bcc959` (`c8a7843`). 7 methods; exact IL; 26 direct comparisons. |
| MatDim uniform_dim arithmetic | Merged to main at `5b1a7cc` (`d0f0b59`). 8 arithmetic methods; exact IL; 25 direct comparisons. |
| MatDim r_dim coordinates | Merged to main at `51def4e` (`d34b1e2`). 5 coordinate methods; exact IL; 28 direct comparisons. |
| MatDim r_dim arithmetic | Merged to main at `df8b3a8` (`ac3d573`). 9 arithmetic methods; exact IL; 30 direct comparisons. |
| MatDim c_dim coordinates | Merged to main at `d866b18` (`637cf69`). 5 coordinate methods; exact IL; 28 direct comparisons. |
| MatDim c_dim arithmetic | Merged to main at `38be34e` (`86f7e1c`). 9 arithmetic methods; exact IL; 29 direct comparisons. |
| MatDim rc_dim coordinates | Merged to main at `2a8ce47` (`32ed171`). 5 coordinate methods; exact IL; 31 direct comparisons. |
| MatDim rc_dim arithmetic | Merged to main at `e0f81c9` (`2490dd7`). 7 arithmetic methods; exact IL; 40 direct comparisons. |
| DelegateFunction invocation and projections | Merged to main at `53036b0` (`e7f720a`). 31 methods; exact IL for 17 `inv`, `invVec`, 4 `AsF`; 67 direct comparisons. |
| Function core dispatch | Merged to main at `f29abb6` (`9549041`). `Function.clo<CLO>(CLO)`, `Function.clovec<CLO>(CLO[])`, `Function.appVec<R>(out R, object[])`; exact IL; 28 direct comparisons. |
| Fn_clo partial closure | Merged to main at `aaa1a47` (`edc808f`). `Fn_clo<CLO>` 21 methods (`.ctor` x2, `Fixpoint`, `Name`, `invVec`, `inv` x16); exact IL; 21 direct comparisons. |
| FnCompose function composition | Merged to main at `bf985ce`. Seven methods; exact IL; seven direct comparisons. |
| MatDim/MatVal dimension/index | Implementations are present on main; direct tests and evidence recorded across merged batches. |
| MatVal collections, ranges, and vectorize | Merged to main at 2082723 (31ee559). 7 methods exact IL, 10 vectorize differential test cases, 100% mutation kill rate (597/601, 0 survivors). |
| MatVal indexed access, axis enumeration and scaling | Merged to main at d0436ed (2fc429a). 33 methods; 31 exact Release IL matches, 2 structure-only; 100% mutation kill rate (710/718, 0 survivors). |
| MatVal memory growth, resize, and row/column mutations | Merged to main at b52edf9 (54d2299). 37 methods; 37 exact Release IL matches; 100% mutation kill rate (662/666, 0 survivors). |
| MatVal element accessors and mutators | Merged to main at e7c190b (5f4a591). 50 methods; 50 exact Release IL matches; 100% mutation kill rate (793/797, 0 survivors). |

The earlier report about missing `Mcd.NumVal.Compare` is not a blocker: the manifest and original member inventory have no such member. See [NumVal evidence](../evidence/Mcd.NumVal.md).

## Historical MatDim/MatVal feature branch

Historical feature branch: `mirror/b-matval-gate-feature` (`0028f2c`). The subsequent full runtime gate record at `/tmp/quire-matval-final-runtime-gate/gate.json` was interrupted at 1,180/2,112 mutants. Work from this branch was broken down into coherent, individually gated batches (pointer properties, dimension properties, pointer helpers, canonicalize/reinsert pointers, contains, and dimension arrays), which have all been merged into main.

## DimVal batch status

The DimVal proposals (47 unreachable lines and 5 dead-constant mutants) and 22 equivalent mutants were approved and merged to main at `26aceb5` and `275b377`. `DimVal.ApplyMatVal` was implemented, differentially verified (5 direct cases), and merged to main at `dffea4d` (`a00b684`).

## Merged MatVal and DimVal batches

### MatVal pointer properties (merged to main at 59df412)

Base: `6bfaea5f1646e45f81330beebaaa625d3ac220ac`; branch `mirror/b-matval-pointer-properties` (`e68e5ac`).
Original identity: mplrun Prime 12.0.1.0 SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
Exact direct member tokens recorded in `docs/mirror/evidence/Mcd.MatVal.md`; five test facts compare 40 cases (HasPtrs 9, RepHasPtrs 4, IsUniformScalar 8, IsUniformScalarPtrs 9, IsUniform 10), including return/exception, storage/table/pointer identity and mutation states.
Tests passed (8 Quire.Mpl.Tests, 5 Differential facts / 40 direct comparisons). Merged via `merge_gate.py --apply`.

### MatVal dimension properties (merged to main at e36fe93)

Branch `mirror/b-matval-dimension-properties` (`f51e281`).
Covers four dimension queries: `get_IsDimless`, `IsUniformDimOf`, `get_PhysicalDimension`, and `get_UniformDimension`.
Four direct-comparison facts cover 31 finite cases (IsDimless 7, IsUniformDimOf 10, PhysicalDimension 6, UniformDimension 8).
Tests passed (14 Quire.Mpl.Tests, 4 Differential facts / 31 direct comparisons). Merged via `merge_gate.py --apply`.

### MatVal pointer helper remediation (merged to main at d1f7449)

Branch `mirror/b-matval-pointer-helper-remediation` (`5674094`).
Exact tokens: `Canonicalize()` 0x0600043b, `TidyPtrs(int,int,Dim,bool)` 0x06000442, `IsUniformPtrs()` 0x06000458, and `IsUniformPtrsNoChange()` 0x06000459. Contracts recorded in `docs/mirror/evidence/Mcd.MatVal.md`.
Direct suite has 46 comparisons across four facts (Canonicalize 10, TidyPtrs 15, IsUniformPtrs 15, IsUniformPtrsNoChange 6). Scoped mutation killed 230/230 mutants (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal CanonicalizePtrs (merged to main at 5026ad8)

Branch `mirror/b-matval-canonicalize-ptrs` (`b3a12ce`).
Directly compares private `CanonicalizePtrs()`, token `0x06000441`. Contract recorded in `docs/mirror/evidence/Mcd.MatVal.md`.
18 direct cases cover dimension normalization/order, logical nonpointer versus pointer-only dimensions, stale/live pointers, matching/incompatible/zero dimensions, non-NumVal pointers, full/partial coverage, both reinsertion paths, shared padded storage, and exceptions.
Scoped mutation killed 179/179 mutants (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal ReinsertPtrs / ReinsertPtrs1 (merged to main at e4a2af9)

Branch `mirror/b-matval-reinsert-ptrs` (`caad393`).
Exact members: `ReinsertPtrs()` token `0x06000443` and `ReinsertPtrs1()` token `0x06000444`. Contracts in `docs/mirror/evidence/Mcd.MatVal.md`.
23 direct cases (11 for `ReinsertPtrs`, 12 for `ReinsertPtrs1`). Scoped mutation killed 179/179 mutants (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal contains(object) (merged to main at 278aa9b)

Branch `mirror/b-matval-contains` (`fb34932`).
Exact token: `Mcd.MatVal.contains(object) -> bool`, token `0x0600051a`. Contract in `docs/mirror/evidence/Mcd.MatVal.md`.
15 finite cases cover null/empty tables, null query/value, same and equal-but-distinct references, arbitrary keys, stale values, unrelated live pointer slots, nested present/absent values, and unchanged identities. Merged via `merge_gate.py --apply`.

### DimVal.ApplyMatVal (merged to main at dffea4d)

Branch `mirror/b-dimval-apply-current` (`a00b684`).
Public static `DimVal.ApplyMatVal(DimVal fv, DimVal mv)` (`0x060006b0`). Contract in `docs/mirror/evidence/Mcd.DimVal.md`.
Generated `[MirrorStub]` removed. Five direct `MethodComparison` cases in `DimValApplyMatValDifferentialTests`. Merged via `merge_gate.py --apply`.

### MatVal ColDimArray / RowDimArray (merged to main at f447eed)

Branch `mirror/b-matval-dimension-arrays` (`339fcad`).
Exact signatures and tokens: `ColDimArray(int) -> Dim[]` `0x06000438` and `RowDimArray(int) -> Dim[]` `0x06000439`. Contracts in `docs/mirror/evidence/Mcd.MatVal.md`.
22 direct comparisons across two facts (11 per method) in `MatValDimensionArraysDifferentialTests`. Merged via `merge_gate.py --apply`.

### MatVal IsRange / IsIntegral accessors (merged to main at 3dda994)

Branch `mirror/b-matval-range-integral` (`78d636f`), based on clean main `9e982ce09a19`. Verified status annotations and contracts for `get_IsRange` (0x06000464), `set_IsRange` (0x06000465), `get_IsIntegral` (0x06000466), and `set_IsIntegral` (0x06000467). Setters use compound assignments (`|=` / `&=`) matching Mathcad IL exactly; all four accessors are exact opcode matches in Release IL. 18 direct MethodComparison cases in `MatValRangeIntegralDifferentialTests`. Scoped mutation on `Quire.Mcd.MatVal` passed with 286/286 mutants killed (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal asRealVector(object) (merged to main at dc92792)

Branch `mirror/b-matval-asrealvector` (`f0ae439`), based on clean main `3dda994`. Added direct differential tests, verified status annotation, and contract in [Mcd.MatVal evidence](../evidence/Mcd.MatVal.md). Exact original signature, token 0x06000468, and baseline hash `ef8374270a85abf2d28331ffad9e8c7ee7292fea727d03ddc032d37d525d19e4` (exact IL match in baseline). 9 direct MethodComparison cases in `MatValAsRealVectorDifferentialTests`. Merged via `merge_gate.py --apply`.

### MatDim RowDimensionArray / ColDimensionArray (merged to main at 2930f1a)

Branch `mirror/b-matdim-dimension-arrays` (`5bd227c`), based on clean main `dc92792`. Added direct differential tests in `MatDimDimensionArraysDifferentialTests.cs`, verified status annotations on `RowDimensionArray(int)` (token 0x06000613) and `ColDimensionArray(int)` (token 0x06000614) in `src/Quire.Mpl/Mcd/MatDim.cs`, and contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures and hashes are in that table. Both methods compile to exact opcode matches in Release IL. 8 direct MethodComparison cases (4 per method) in `MatDimDimensionArraysDifferentialTests`. Scoped mutation on `Quire.Mcd.MatDim` passed with 10/10 mutants killed (0 survivors). Merged via `merge_gate.py --apply`.

### FunctionUtilities take / but boundary cases (merged to main at 27c06a0)

Branch `mirror/b-functionutilities-take-negative` (`6e27dc2`), based on clean main `2930f1a`. Added direct differential tests in `FunctionUtilitiesButDifferentialTests.cs` and `FunctionUtilitiesTakeNegativeDifferentialTests.cs`, verified status annotations on `but<T>` (token 0x0600027a) and `take<T>` (token 0x0600027b) in `src/Quire.Mpl/Run/FunctionUtilities.cs`, and updated contracts in [Mpl.Run.FunctionUtilities evidence](../evidence/Mpl.Run.FunctionUtilities.md). Exact original signatures and tokens are in that table; both methods are exact IL matches in baseline (`exact: True`). 17 direct MethodComparison cases against the pinned artifact. Merged via `merge_gate.py --apply`.

### MatDim base getters (merged to main at e38ab20)

Branch `mirror/b-matdim-base-getters` (`3138ad2`), based on clean main `27c06a0`. Added direct differential tests in `MatDimBaseGettersDifferentialTests.cs`, verified status annotations on `get_UniformDim` (token 0x0600060e), `get_RowDimensions` (token 0x06000611), `get_ColDimensions` (token 0x06000612), and `get_IsLinear` (token 0x06000619) in `src/Quire.Mpl/Mcd/MatDim.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f`) are in that table. All four base getters return `null` (`ldnull; ret`), matching Mathcad IL exactly in Release builds. 8 direct MethodComparison cases against the pinned artifact. Merged via `merge_gate.py --apply`.

### MatDim c_dim constructor and factory (merged to main at c4529f9)

Branch `mirror/b-matdim-c-dim-constructor` (`93d9ee2`), based on clean main `e38ab20`. Added direct differential tests in `MatDimCDimConstructorDifferentialTests.cs`, verified status annotations on `c_dim..ctor(Dim[], int)` (token 0x060007d5) and `c_dim.make(Dim[], int)` (token 0x060007d6) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`3ed8e46824c37c277efd830868b9b37158da6f3f688e0bc88d50f509a4c5c4ee` and `936602215b8315df780417f85d815de32230a6876e5ad336e53ff7c72c277f1d`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. 8 direct MethodComparison cases against the pinned artifact. Merged via `merge_gate.py --apply`.

### MatDim r_dim constructor and factory (merged to main at 7ee2d00)

Branch `mirror/b-matdim-r-dim-constructor` (`3c83bbf`), based on clean main `c4529f9`. The batch adds direct differential tests in `MatDimRDimConstructorDifferentialTests.cs`, verified status annotations on `r_dim..ctor(Dim[], int)` (token 0x060007c3) and `r_dim.make(Dim[], int)` (token 0x060007c4) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`5d60a56b9ab2c338259a2f61867e4e387da38cb6ec2bd577d9177372b8720549` and `b8375eed03f58bc3c1a15dcc3dad1dd507a2224acdb8fd7523413eb1ad3faf80`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim rc_dim constructor and factory (merged to main at 67615ea)

Branch `mirror/b-matdim-rc-dim-constructor` (`a63d30e`), based on clean main `7ee2d00`. The batch adds direct differential tests in `MatDimRcDimConstructionDifferentialTests.cs`, verified status annotations on `rc_dim..ctor(Dim[], Dim[])` (token 0x060007b3) and `rc_dim.make_rc(Dim[], Dim[])` (token 0x060007b4) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`36f0aab7306cee9c64d72148666c18aa9084f1bfa0a74b4233e0a739d7294d9b` and `137790307c7b431b8b12f55af9aecd00aea49189f013cf73acb27b81411dbf0d`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim crazy_dim constructor and factory (merged to main at d2f8f67)

Branch `mirror/b-matdim-crazy-dim-constructor` (`cb23481`), based on clean main `67615ea`. The batch adds direct differential tests in `MatDimCrazyDimConstructionDifferentialTests.cs`, verified status annotations on `crazy_dim..ctor(MatVal)` (token 0x060007ad) and `crazy_dim.make(MatVal)` (token 0x060007b2) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`7ff0599165db240939134df5fcff1eaede0e162d44cb06e96bde6851d83fb242` and `de5613a5a2ce61aa5c74eb1ca33d5ee947b3cb2f30c50bfeb8932bd417d837aa`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim c_dim equality (merged to main at 0167351)

Branch `mirror/b-matdim-c-dim-equality` (`6434c8a`), based on clean main `d2f8f67`. The batch adds direct differential tests in `MatDimCDimEqualityDifferentialTests.cs`, verified status annotations on `c_dim.eq_b(MatDim)` (token 0x060007dc) and `c_dim.eq2_b(c_dim)` (token 0x060007dd) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`219987b242c90329d2a289fbc1c8b7278516009ad009ecbdde0f3845519961ff` and `1a882f6aa8d0535aacbb4952b2fffab6cee55f5b9d6507780ad42b3a3e9c0e80`) are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim r_dim equality (merged to main at ae8e99b)

Branch `mirror/b-matdim-r-dim-equality` (`8aad7a6`), based on clean main `0167351`. The batch adds direct differential tests in `MatDimRDimEqualityDifferentialTests.cs`, verified status annotations on `r_dim.eq_b(MatDim)` (token 0x060007ca) and `r_dim.eq2_b(r_dim)` (token 0x060007cb) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`5136ca7ec8ab545b25ddc94d7ac0ea83f137514de0df7057adc1a8b85ff9f759` and `11e3d83aa704fa79d5d85c650b79c66d22a02c6d945185ac95cac169efa96493`) are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim rc_dim equality (merged to main at 8f84097)

Branch `mirror/b-matdim-rc-dim-equality` (`8a1bbc1`), based on clean main `ae8e99b`. The batch adds direct differential tests in `MatDimRcDimEqualityDifferentialTests.cs`, verified status annotations on `rc_dim.eq_b(MatDim)` (token 0x060007ba) and `rc_dim.eq2_b(rc_dim)` (token 0x060007bb) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`f1a3e954a080eec13df74fa453d26474ca4751efae3bef0d366e12a979d788f5` and `e7178386af3591bf902bfdd49bd41cbeff74a23e0b7c727f2d9c576c6df97948`) are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim crazy_dim equality (merged to main at 561f85b)

Branch `mirror/b-matdim-crazy-dim-equality` (`0cf9fef`), based on clean main `8f84097`. The batch adds direct differential tests in `MatDimCrazyDimEqualityDifferentialTests.cs`, verified status annotations on `crazy_dim.eq_b(MatDim)` (token 0x060007b0) and `crazy_dim.eq2_b(crazy_dim)` (token 0x060007b1) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`1aaa594cfb249e74c879b2c072d8c63e6e4acb519b17138baf488e7fa772ac0b` and `6ac593fa6e76cfd5ea7a17da09eb21e7aadb3f985e7ae7447a870a6cecb7a5bf`) are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim uniform_dim equality (merged to main at 2d7c3ae)

Branch `mirror/b-matdim-uniform-dim-equality` (`74c9787`), based on clean main `561f85b`. The batch adds direct differential tests in `MatDimUniformEqBDifferentialTests.cs` and `MatDimUniformEq2BDifferentialTests.cs`, verified status annotations on `uniform_dim.eq_b(MatDim)` (token 0x060007a2) and `uniform_dim.eq2_b(Dim)` (token 0x060007a3) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`c1ab43140edd236a05dcf85a3883e24cf988f49903ee1b8ff97b568d54e6dbe5` and `1e6e753b6c063dcaf6acb620558a456be804d0a774385708691e19fbfe5067ec`) are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatVal real classifiers (merged to main at 28bee78)

Branch `mirror/b-matval-real-classifiers` (`ad7e8b4`), based on clean main `168be17`. The batch adds direct differential tests in `MatValRealClassifiersDifferentialTests.cs`, verified status annotations on `IsVector` (0x0600045b), `IsRealMatrix` (0x0600045c), `IsScalarMatrix` (0x0600045d), `IsRealMatrixMixed` (0x0600045e), and `IsRealVector` (0x0600045f) in `src/Quire.Mpl/Mcd/MatVal.Access.cs`, and updated contracts in [Mcd.MatVal evidence](../evidence/Mcd.MatVal.md). Exact original signatures, tokens, and hashes are recorded in that table. Exact Release IL matches confirmed for `IsVector`, `IsRealMatrix`, and `IsRealVector`. 46 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`. Merged via `merge_gate.py --apply`.

### MatVal assertions and casts (merged to main at 41f234b)

Branch `mirror/b-matval-assertions-casts` (`50a27be`), based on clean main `28bee78`. The batch adds direct differential tests in `MatValAssertionsAndCastsDifferentialTests.cs`, verified status annotations on `asVector` (0x06000469), `asRealVector` (0x0600046a), `asMatrix` (0x0600046b), `asRealMatrix` (0x0600046c), `assertRealVector()` (0x0600046e), `assertRealVector(uint)` (0x0600046f), and `assertRealMatrix(uint)` (0x06000470) in `src/Quire.Mpl/Mcd/MatVal.Access.cs`, and updated contracts in [Mcd.MatVal evidence](../evidence/Mcd.MatVal.md). Exact original signatures, tokens, and hashes are in that table. All 7 methods match Mathcad IL exactly in Release builds. 30 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`; 100% mutation kill rate (16/16). Merged via `merge_gate.py --apply`.

### MatDim uniform_dim construction and coordinates (merged to main at 8bcc959)

Branch `mirror/b-matdim-uniform-dim-construction` (`c8a7843`), based on clean main `41f234b`. The batch adds direct differential tests in `MatDimUniformDimConstructionDifferentialTests.cs`, verified status annotations on `uniform_dim..ctor(Dim, int, int)` (token 0x0600079c), `uniform_dim.make(Dim, int, int)` (token 0x0600079d), `uniform_dim.dim(int, int)` (token 0x0600079e), `uniform_dim.matdim(int, int)` (token 0x0600079f), `uniform_dim.get_UniformDim()` (token 0x060007a0), `uniform_dim.get_IsLinear()` (token 0x060007a1), and `uniform_dim.transpose()` (token 0x060007a9) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. All 7 methods match Mathcad IL exactly in Release builds. 26 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`; 100% mutation kill rate (100/100). Merged via `merge_gate.py --apply`.

### MatDim uniform_dim arithmetic operations (merged to main at 5b1a7cc)

Branch `mirror/b-matdim-uniform-dim-arithmetic` (`d0f0b59`), based on clean main `8bcc959`. The batch adds direct differential tests in `MatDimUniformDimArithmeticDifferentialTests.cs`, verified status annotations on `uniform_dim.mmult1(MatDim)` (token 0x060007a4), `uniform_dim.mmult2(uniform_dim)` (token 0x060007a5), `uniform_dim.mmult2(r_dim)` (token 0x060007a6), `uniform_dim.dmult1(MatDim)` (token 0x060007a7), `uniform_dim.dmult2(uniform_dim)` (token 0x060007a8), `uniform_dim.det(int)` (token 0x060007aa), `uniform_dim.invert()` (token 0x060007ab), and `uniform_dim.pwr(uint)` (token 0x060007ac) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. All 8 methods match Mathcad IL exactly in Release builds. 25 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`; 100% mutation kill rate (1/1). Merged via `merge_gate.py --apply`.

### MatDim r_dim coordinates and properties (merged to main at 51def4e)

Branch `mirror/b-matdim-r-dim-coordinates` (`d34b1e2`), based on clean main `5b1a7cc`. The batch adds direct differential tests in `MatDimRDimCoordinatesDifferentialTests.cs`, verified status annotations on `r_dim.dim(int, int)` (token 0x060007c5), `r_dim.matdim(int, int)` (token 0x060007c6), `r_dim.get_IsLinear()` (token 0x060007c7), `r_dim.get_RowDimensions()` (token 0x060007c8), and `r_dim.ColDimensionArray(int)` (token 0x060007c9) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are recorded in that table. All 5 methods match Mathcad IL exactly in Release builds. 28 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`; 100% mutation kill rate (2/2). Merged via `merge_gate.py --apply`.

### MatDim r_dim arithmetic operations (merged to main at df8b3a8)

Branch `mirror/b-matdim-r-dim-arithmetic` (`ac3d573`), based on clean main `28d0434`. The batch adds direct differential tests in `MatDimRDimArithmeticDifferentialTests.cs`, verified status annotations on `r_dim.mmult1(MatDim)` (token 0x060007cc), `r_dim.mmult2(c_dim)` (token 0x060007cd), `r_dim.mmult2(rc_dim)` (token 0x060007ce), `r_dim.dmult1(MatDim)` (token 0x060007cf), `r_dim.dmult2(r_dim)` (token 0x060007d0), `r_dim.dmult2(c_dim)` (token 0x060007d1), `r_dim.transpose()` (token 0x060007d2), `r_dim.det(int)` (token 0x060007d3), and `r_dim.invert()` (token 0x060007d4) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. All 9 methods match Mathcad IL exactly in Release builds. 30 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`; 100% mutation kill rate (2/2). Merged via `merge_gate.py --apply`.

### MatDim c_dim coordinates and properties (merged to main at d866b18)

Branch `mirror/b-matdim-c-dim-coordinates` (`637cf69`), based on clean main `df8b3a8` (merged to main at `d866b18` after rebase). The batch adds direct differential tests in `MatDimCDimCoordinatesDifferentialTests.cs`, verified status annotations on `c_dim.dim(int, int)` (token 0x060007d7), `c_dim.matdim(int, int)` (token 0x060007d8), `c_dim.get_IsLinear()` (token 0x060007d9), `c_dim.get_ColDimensions()` (token 0x060007da), and `c_dim.RowDimensionArray(int)` (token 0x060007db) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are recorded in that table. All 5 methods match Mathcad IL exactly in Release builds. 28 direct MethodComparison cases against Prime 12.0.1.0 `mplrun.dll`; 100% mutation kill rate (2/2). Merged via `merge_gate.py --apply`.

### MatDim c_dim arithmetic operations (merged to main at 38be34e)

Branch `mirror/b-matdim-c-dim-arithmetic` (`86f7e1c`), based on clean main `d866b18` (merged to main at `38be34e` after rebase). The batch adds direct differential tests in `MatDimCDimArithmeticDifferentialTests.cs`, verified status annotations on `c_dim.mmult1(MatDim)` (token 0x060007de), `c_dim.mmult2(uniform_dim)` (token 0x060007df), `c_dim.mmult2(r_dim)` (token 0x060007e0), `c_dim.dmult1(MatDim)` (token 0x060007e1), `c_dim.dmult2(r_dim)` (token 0x060007e2), `c_dim.dmult2(c_dim)` (token 0x060007e3), `c_dim.transpose()` (token 0x060007e4), `c_dim.det(int)` (token 0x060007e5), and `c_dim.invert()` (token 0x060007e6) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. All 9 methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. 29 direct MethodComparison cases; 100% mutation kill rate (0 survivors). Merged via `merge_gate.py --apply`.

### MatDim rc_dim coordinates and properties (merged to main at 2a8ce47)

Branch `mirror/b-matdim-rc-dim-coordinates` (`32ed171`), based on clean main `38be34e` (merged to main at `2a8ce47` after rebase). The batch adds direct differential tests in `MatDimRcDimCoordinatesDifferentialTests.cs`, verified status annotations on `rc_dim.dim(int, int)` (token 0x060007b5), `rc_dim.matdim(int, int)` (token 0x060007b6), `rc_dim.get_IsLinear()` (token 0x060007b7), `rc_dim.RowDimensionArray(int)` (token 0x060007b8), and `rc_dim.ColDimensionArray(int)` (token 0x060007b9) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. All 5 methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. 31 direct MethodComparison cases; 100% mutation kill rate (2/2). Merged via `merge_gate.py --apply`.

### MatDim rc_dim arithmetic operations (merged to main at e0f81c9)

Branch `mirror/b-matdim-rc-dim-arithmetic` (`2490dd7`), based on clean main `2a8ce47` (merged to main at `e0f81c9` after rebase). The batch adds direct differential tests in `MatDimRcDimArithmeticDifferentialTests.cs`, verified status annotations on `rc_dim.mmult1(MatDim)` (token 0x060007bc), `rc_dim.mmult2(c_dim)` (token 0x060007bd), `rc_dim.mmult2(rc_dim)` (token 0x060007be), `rc_dim.transpose()` (token 0x060007bf), `rc_dim.det(int)` (token 0x060007c0), `rc_dim.invert()` (token 0x060007c1), and `rc_dim.pwr(uint)` (token 0x060007c2) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. All 7 methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. 40 direct MethodComparison cases; 100% mutation kill rate (0 survivors). Merged via `merge_gate.py --apply`.

### MatDim crazy_dim coordinates (merged to main at f7afb21)

Branch `mirror/b-matdim-crazy-dim-coordinates` (`58659d6`), based on clean main `e0f81c9` (merged to main at `f7afb21`). The batch adds direct differential tests in `MatDimCrazyDimCoordinatesDifferentialTests.cs`, verified status annotations on `crazy_dim.dim(int, int)` (token 0x060007ae) and `crazy_dim.matdim(int, int)` (token 0x060007af) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. 24 direct MethodComparison cases; 100% mutation kill rate (0 survivors). Merged via `merge_gate.py --apply`.

### DelegateFunction invocation & projections (merged to main at 53036b0)

Branch `mirror/b-delegatefunction-inv` (`e7f720a`), based on clean main `f7afb21` (merged to main at `53036b0`). The batch adds direct differential tests in `DelegateFunctionDifferentialTests.cs`, verified status annotations on all 31 remaining methods of `DelegateFunction` in `src/Quire.Mpl/Run/DelegateFunction.cs`, and updated contracts in [Mpl.Run.Function evidence](../evidence/Mpl.Run.Function.md).
Exact original signatures, tokens, and hashes are in that table. All 17 generic `inv` overloads (`0x060001fa` - `0x0600020a`), `invVec` (`0x060001f9`), and 4 `AsF` projections (`0x0600020b` - `0x0600020e`) match Mathcad IL exactly in Release builds. 67 direct MethodComparison cases across 7 facts; 100% mutation kill rate (201/201 mutants killed, 0 survivors). Merged via `merge_gate.py --apply`.

### MatVal collections, ranges, and vectorize (merged to main at 2082723)

Branch `mirror/b-matval-collections` (`31ee559`).
Exact tokens: `MatVal.vectorize(Function, object[])` (0x06000522), `MatVal.MakeVector0(object[])` (0x060004a0), `MatVal.PushVectorArg(object)` (0x060004ae), `MatVal.map(F<object, object>, DimVal)` (0x06000485), `MatVal.FillThis(object, int, int, int)` (0x06000525), `MatVal.FillThisPtrs(object, int, int, int)` (0x06000527), `MatVal.FillArr(double[], double, int, int, int)` (0x06000532).
All 7 methods match Mathcad Prime 12.0.1.0 Release IL exactly.
Direct differential comparisons added in `tests/Quire.Differential.Tests/MatValCollectionDifferentialTests.cs`:
- `Collection_factories_push_and_shape`: `reserve(int, int)` (2x3), `Singleton` (string & NumVal), `Pair` (strings & NumVals), `Triple` (strings), `Push(object)` (strings & NumVal), `PushRaw1(object)`, `PushRaw(object)` (string), `PushRaw(int)` (42), `PushVectorArg(object)` (valid Cons list and non-Cons invalid argument error path), `SetShape()` and `SetShape1()`.
- `Ranges_map_and_vectorize`: `mkRange2` (scalars and dimensioned), `mkRange3` (scalar stepped), `as_object_vector` (vector to object array), `map(Function, DimVal)` (unary mapping), `map(Function, DimVal, DimVal)` (binary mapping), and comprehensive `vectorize(Function, object[])` suite (10 distinct branches/cases).
100% mutation kill rate on compared methods (597/601 mutants killed, 0 survivors). Merged via `merge_gate.py --apply`.

### MatVal indexed access, axis enumeration and scaling (merged to main at d0436ed)

Branch `mirror/b-matval-indexed` (`2fc429a`).
Exact tokens (33 methods in `src/Quire.Mpl/Mcd/MatVal.Indexed.cs`):
- Dynamic calls: `DynCall(object)` (0x060004b5), `DynCall(object, object)` (0x060004b6).
- Checked vector getters: `mcd_get_1(object, object)` (0x060004b7), `mcd_getm_1(object, int)` (0x060004be), `mcd_get_1(uint)` (0x060004bf), `mcd_sym_get_1(uint)` (0x060004c0), `mcd_get_1(int)` (0x060004c6), `mcd_sym_get_1(int)` (0x060004c7), `matrix_get_1(int)` (0x060004b8), `sym_matrix_get_1(int)` (0x060004b9), `aref_1(int)` (0x060004ca).
- Checked matrix getters: `mcd_getm_2(object, int, int)` (0x060004c1), `mcd_get_2(uint, uint)` (0x060004c2), `mcd_sym_get_2(uint, uint)` (0x060004c3), `mcd_get_2(int, int)` (0x060004c8), `mcd_sym_get_2(int, int)` (0x060004c9), `matrix_get_2(int, int)` (0x060004ba), `sym_matrix_get_2(int, int)` (0x060004bb), `aref_2(int, int)` (0x060004cb).
- Iterator getter: `iterator_get(int)` (0x060004bd).
- Axis offsets and enumeration: `offsets(MatAxis, int)` (0x060004dc), `get_type_2(int, int, double)` (0x060004db), `get_type(int, double)` (0x060004dd), `element_types(MatAxis, int, double)` (0x060004de), `get_ptrs(MatAxis, int)` (0x060004df), `get_double(int, double, double)` (0x060004e0), `get_doubles(MatAxis, int, double, double)` (0x060004e1), `get_data(int, double, double)` (0x060004e2), `get_data(MatAxis, int, double, double)` (0x060004e3).
- Object packing: `PackObjects()` (0x06000539), `PackObjects(int)` (0x06000538).
- Matrix division and scaling: `op_Division(MatVal, double)` (0x06000556), `Scale(double)` (0x06000557).

100% exact Release IL opcode structure match against Mathcad Prime 12.0.1.0 `mplrun.dll` across methods (31 exact matches, 2 structure-only).
Direct differential comparisons added in `tests/Quire.Differential.Tests/MatValIndexedDifferentialTests.cs`:
- `Checked_and_symbolic_index_getters_match_mathcad`: `mcd_get_1(uint)` (0, 1, oob, matrix_fails), `mcd_sym_get_1(uint)` (0, oob, matrix_fails), `mcd_get_2(uint, uint)` (0_0, 1_1, row_oob, col_oob), `mcd_sym_get_2(uint, uint)` (0_0, row_oob, col_oob), `mcd_get_1(int)`, `mcd_sym_get_1(int)`, `mcd_get_2(int, int)`, `mcd_sym_get_2(int, int)`, `matrix_get_1(int)`, `sym_matrix_get_1(int)`, `matrix_get_2(int, int)`, `sym_matrix_get_2(int, int)`, `aref_1(int)`, `aref_2(int, int)`, `iterator_get(int)` (0, 1, 2, 3).
- `Dynamic_and_static_call_dispatch_matches_mathcad`: `DynCall(object)` (valid vector and non-integer exception path), `DynCall(object, object)` (valid matrix and non-integer exception paths for both coordinates), `mcd_get_1(object, object)` (static valid, non_vector, non_integer), `mcd_getm_1(object, int)` (static valid, non_vector), `mcd_getm_2(object, int, int)` (static valid, non_matrix).
- `Axis_enumeration_and_classification_match_mathcad`: `get_type_2(int, int, double)` (finite, nan_thresh, cell_1_1_nan, col2_nan), `element_types(MatAxis, int, double)` (row0, col1), `get_ptrs(MatAxis, int)` (row0, col0), `get_doubles(MatAxis, int, double, double)` (row0), `get_data(MatAxis, int, double, double)` (row0), `PackObjects()` (default and offset2), `op_Division(MatVal, double)` (unpacked, packed_mutable in-place, and packed_immutable copy).
100% mutation kill rate on compared methods (710/718 mutants killed, 0 survivors). Merged via `lane_merge.py` at commit `d0436ed`.
### MatVal memory growth, resize, and row/column mutations (merged to main at b52edf9)

Base: `d37614481356e59da6db689b58723fabde11e0b9`; branch `mirror/b-matval-growth` (`54d2299545d0`).
Original identity: mplrun Prime 12.0.1.0 SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
Scope: 37 methods in `src/Quire.Mpl/Mcd/MatVal.Growth.cs`.
- Exact tokens:
  - Reallocation: `realloc(object, int, int)` (0x06000492), `ReAlloc(object, int)` (0x06000505), `ReAlloc(object, int, int)` (0x06000506), `ReAllocate(int)` (0x06000507), `ReAllocate(int, int)` (0x06000508).
  - Array resize & zero tidying: `Resize(double[], int)` (0x0600053e), `CopyResize(double[], int, int)` (0x0600053f), `Resize(double[], int, int)` (0x06000540), `TidyZeros(int, int, int, int)` (0x0600053b).
  - Stretch operations: `Stretch(int)` (0x0600053a), `Stretch(int, int)` (0x0600053c), `StretchWithColumnFill(int, int, DimVal)` (0x0600053d).
  - Extension operations: `extend_1(int)` (0x060004f3), `extend_2(int, int)` (0x060004f9).
  - Mutation setters: `mcd_setm_1` (0x060004f4), `mcd_setm_2` (0x060004f8), `mcd_set_1` (0x060004f5), `mcd_set_2` (0x060004fa), `mcd_upd_1` (0x060004f6, 0x060004f7), `mcd_upd_2` (0x060004fb, 0x060004fc).
  - Column and row mutations: `mcd_setcolumn` (0x060004fd), `setcolumn` (0x060004fe), `setcol` (0x060004ff), `mcd_setrow` (0x06000500), `setrow` (0x06000501), `setblock` (0x06000502), `setmatrix` (0x06000503), `setmatrix_ptrs` (0x06000504).
  - Pointer transformation: `AssignMatrixPtrs` (0x0600073e, 0x0600073f), `ResizeMatrixPtrs` (0x06000740, 0x06000741), `AssignPtrs` (0x0600054f), `ResizePtrs` (0x06000550), `MatVal(MatVal, int, int)` (0x06000491).
- 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll` across all 37 methods.
- All prior structural differences resolved.
- Full test suites passed: Quire.Harness.Tests (97/97), Quire.Mpl.Tests (188/188), Quire.Focused.Differential.Tests (396/396 Debug, 396/396 Release).
- Reconstruction integrity: 0 lint findings.
- Mutation coverage: 662/666 killed (0 cached); 0 survivors.
- Merged cleanly to `main` via `lane_merge.py` at commit `b52edf9`.

### MatVal element accessors and mutators (merged to main at e7c190b)

Base: `e2a03812223dde76b20a0208ea3b3c9fa9eba285`; branch `mirror/b-matval-access` (`5f4a591c8e7a`).
Original identity: mplrun Prime 12.0.1.0 SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
Scope: 50 element accessor and mutator methods in `src/Quire.Mpl/Mcd/MatVal.Access.cs`.
- Exact tokens:
  - Direct element getters: `get_1(int)` (0x060004d0), `get_2(int, int)` (0x060004d1), `get(int)` (0x060004da), `get_1_dim(int)` (0x060004d2), `get_2_dim(int, int)` (0x060004d3), `get_dim(int)` (0x060004e4), `get_1_matdim(int)` (0x060004d4), `get_2_matdim(int, int)` (0x060004d5), `get_matdim(int)` (0x060004e5), `get_1_ptr(int)` (0x060004d6), `get_2_ptr(int, int)` (0x060004d7), `get_ptr(int)` (0x060004e7), `get_1_na(NumVal, int)` (0x060004d8), `get_2_na(NumVal, int, int)` (0x060004d9), `get_na(NumVal, int)` (0x060004e6).
  - Real, imaginary, and mixed-real getters: `get_real(int)` (0x060004e8), `get_real(int, int)` (0x060004e9), `get_real_raw(int)` (0x060004ec), `get_imag(int)` (0x060004ea), `get_imag(int, int)` (0x060004eb), `get_imag_raw(int)` (0x060004ed), `get_real_mixed(int)` (0x060004ee), `get_real_mixed(int, int)` (0x060004ef), `get_real_mixed_handle(int, uint)` (0x060004f0), `get_real_mixed_handle(int, int, uint)` (0x060004f1), `get_real_mixed_raw(int, uint)` (0x060004f2).
  - Direct setters: `set_1(int, object)` (0x06000509), `set_2(int, int, object)` (0x0600050a), `set(int, object)` (0x0600050c), `setNaN(int, object)` (0x0600050b), `setspec(int, object)` (0x0600050d), `set_1(int, double, Dim)` (0x0600050e), `set_2(int, int, double, Dim)` (0x0600050f), `set(int, double, Dim)` (0x06000512), `set_1(int, double, double, Dim)` (0x06000510), `set_2(int, int, double, double, Dim)` (0x06000511), `set(int, double, double, Dim)` (0x06000514), `set(int, double)` (0x06000513), `set(int, double, double)` (0x06000515), `set(int, NumVal)` (0x06000516), `set_ptrs_1(int, object)` (0x06000517), `set_ptrs_2(int, int, object)` (0x06000518), `set_ptrs(int, object)` (0x06000519), `set_ptrs_raw(int, object)` (0x0600051b), `set_ptrs_at(int, object)` (0x06000521), `set_2i(int, int, double)` (0x0600051c), `set_1(int, double)` (0x0600051d), `set_2(int, int, double)` (0x0600051e), `set_at(int, double)` (0x0600051f), `set_at(int, double, double)` (0x06000520), `AllocImaginary(double[])` (0x06000531), `DimActual()` (0x06000440), `dvm(Dim)` (0x06000445).
- 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll` across all target methods.
- Full test suites passed: Quire.Harness.Tests (97/97), Quire.Mpl.Tests (188/188), Quire.Focused.Differential.Tests (399/399 Debug, 399/399 Release).
- Reconstruction integrity: 0 lint findings.
- Mutation coverage: 793/797 killed (0 cached); 0 survivors.
- Merged cleanly to `main` via `lane_merge.py` at commit `e7c190b`.


## Function closure dispatch — active checkpoint

Branch `mirror/b-function-closure`, based on clean main `53036b0`. The batch adds direct differential tests in `FunctionClosureDirectDifferentialTests.cs`, verified status annotation on `Function.closure<CLO>(Delegate, CLO, int)` (token `0x060001a3`) in `src/Quire.Mpl/Run/Function.cs`, and updated contract in [Mpl.Run.Function evidence](../evidence/Mpl.Run.Function.md).
Original signature, token, and hash are in that table. The method compiles to 100% exact Release IL opcode match against Mathcad Prime 12.0.1.0 `mplrun.dll`.

Direct differential tests cover 14 direct comparisons against Prime 12.0.1.0 `mplrun.dll`:
- Purities 1–6 covering every specialized wrapper arm (purities 2..5) and the default boundary arms (purities 1 and 6).
- Typed-helper fallback with non-`F<>` delegate at purity 2.
- Downstream `Apply` on all resulting closure instances verifying state and reference retention of captured objects.
- 100% mutation kill rate (79/79 mutants killed, 0 survivors).

## Historical next-queue note (superseded)

The old queue listed Function core dispatch and FnCompose; both have since merged to main. Continue with Engine
interfaces and factories if current ownership and the tier inventory still show unowned work.
