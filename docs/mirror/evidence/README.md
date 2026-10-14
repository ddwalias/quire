# Evidence records (PLAN.md §3c rule 2)

One file per Mathcad type, named after the Mathcad type (`Mcd.Rational.md`). A member's
`[MirrorAnalyzed("Mcd.Rational")]` or `[MirrorTested("Mcd.Rational")]` points here; `[MirrorVerified(test)]` members
have a record here too. `Quire.Mirror.Tests` fails when a referenced record is missing.

## How the analysis is done

1. **REA, managed.** `inspect_managed_members` on the original assembly gives each method's metadata token,
   signature, IL size, IL and normalized-IL SHA-256, call edges, field accesses and operand-bearing instruction
   anchors. Its Evidence ID and the artifact's SHA-256 and MVID go in the record header; each member row cites its
   token and normalized-IL hash, so the finding is bound to one exact body. Equal normalized-IL hashes prove two bodies
   identical, which REA reports without any decompiler.
2. **Decompiled view.** REA's anchors omit operand-less instructions (arithmetic, `ldarg`, `ret`), so the body's
   control and data flow is read from an ILSpy decompilation (`ilspycmd`, recorded per record) of the same file.
   REA's `import_managed_reconstruction` is the tool for binding such output to its Evidence, but it takes the full
   member Evidence (megabytes per assembly) as input, so the binding is recorded here by token and hash instead.
   Raw decompiled C# stays outside the repository; the IL listing below is committed.
3. **IL listing.** Commit the top-level type's IL, nested types included, as
   `docs/mirror/il/<assembly>/<Mathcad type>.il`, produced by `ilspycmd --ilcode -t <type>` from the pinned
   artifact. The validator requires these consecutive header lines (lowercase hex):
   ```
   // Tool: ilspycmd <version>
   // SHA-256: <assembly sha-256>
   // MVID: <module mvid>
   ```
   Regenerate it whenever the artifact identity changes.
4. **Native.** For bodies in native code, REA's Ghidra provider (`analyze_function`, `procedure_pseudo_code`).
5. **Contract and reconstruction.** Summarize each member as a contract: inputs, outputs, errors, mutation, edge
   cases and every branch, which test cases are chosen from. Quire's body is a reconstruction from the IL of the
   original source: same control flow, evaluation order, calls, locals, exceptions and mutation, written as
   idiomatic C# with the constructs the compiler evidently lowered (`foreach`, `using`, `switch`, `??`, lambdas,
   iterators, properties). No `goto`, compiler-generated names or other decompiler artifacts. Where the IL admits
   several source forms, choose the most idiomatic one and note material ambiguity under Confidence / limits.
   The [structural gate](../validation.md#reconstruction-integrity) compares the result with the original IL. Branch
   layout counts: `if`/`return` and `?:` order their arms oppositely, so pick the form whose IL order matches. Where
   the IL shows an older lowering (`string.Format` for interpolation), write that call.

## Record format

```markdown
# <Mathcad type>

- **Original:** <assembly> <version>, SHA-256 <…>, MVID <…>
- **REA:** <evidence id> (inspect_managed_members); decompiled view: ilspycmd <version>
- **IL:** il/<assembly>/<Mathcad type>.il
- **Quire:** <source path(s)>; tests: <test classes>

| Member | Token | Normalized IL (prefix) | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|

## Test cases
Observed (differential or black-box against Mathcad) versus inferred (expected values worked out from the contract).

## Open
Anything not settled; such members stay Analyzed or Blocked.
```

A member is **Tested** when Quire's behaviour tests for its contract pass, and **Verified** only for the cases a
differential test compared with the installed Mathcad.
