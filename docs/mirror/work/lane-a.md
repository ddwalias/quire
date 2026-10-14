# Lane A Work Checkpoint

## Lane Scope
1. **Unit 1: Sexpr family** (18 types: `Sexpr`, `Nul`, `SCons`, `Tok`, `Qt`, `Bq`, `Cm`, `Ca`, `SAppend`, `SList`, `Tag`, `Atom`, `Sym`, `Cons`, `char_constant`, `int_constant`, `real_constant`, `str_constant`). Tests: `SexprDifferentialTests.cs`.
2. **Unit 2: MplRunHelpersDifferentialTests.cs**: Cons lists, VecBuilder, Tuple, MplSignature.
3. **Unit 3: *DifferentialTests.cs**: AppException, Rational, Type, UnitSystem, RuntimeOptions, Utilities.
4. **Unit 4: Batch 2e**: Remaining unclassified/stub members.

Lane B owns `MatVal*`, `MatDim`, `Function*`, `FunctionUtilities`, `DimVal.ApplyMatVal`, and `Quire.Engine`.

---

## Unit 1 Status: Sexpr Family (mirror/a-sexpr)
- **Status:** Complete implementation & differential migration.
- **Test suite:** `tests/Quire.Differential.Tests/SexprDifferentialTests.cs` rewritten from legacy `Same(...)` to direct `MethodComparison.Compare(...)`.
- **Test execution:** 9/9 test methods passed (78/78 Quire.Focused.Differential.Tests passed).
- **Structure comparison:** 0 mismatches across all 18 types.
- **Integrity / Lint:** 0 lint findings.
- **Ratchet:** OK (0 regressions).
- **Mutation:** 106/107 killed (99.1% kill rate).
  - **Surviving mutant:** `Quire.Mpl.Run.SList::ToString`0()->[System.Runtime]System.String|False@IL_0030 ldc.i4.5 -> ldc.i4.6`
  - **Equivalence analysis:** In `SList.ToString()`, `return "(SLIST# " + FormatCons(Field1) + " " + Field2.ToString() + ")";` compiles to `string.Concat(new string[5] { ... })`. Mutating `ldc.i4.5` to `ldc.i4.6` allocates a 6-element array whose 6th element is `null`. In .NET, `string.Concat(string[])` ignores `null` elements (treating them as empty), returning an identical string with the exact same length and characters. This mutant cannot be observed or killed by any differential test.
  - **Next action for maintainer:** Add to `docs/mirror/accepted-mutants.txt`:
    `Quire.Mpl.Run.SList::ToString`0()->[System.Runtime]System.String|False@IL_0030 ldc.i4.5 -> ldc.i4.6 # trailing null in string.Concat string[] ignored by runtime`
