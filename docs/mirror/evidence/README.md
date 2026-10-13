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
   Decompiled text never enters the repository.
3. **Native.** For bodies in native code, REA's Ghidra provider (`analyze_function`, `procedure_pseudo_code`).
4. **Findings are written in our own words**, as a contract: inputs, outputs, errors, mutation, edge cases. No
   decompiled code or pseudocode is pasted. Quire's implementation is written from the contract.

## Record format

```markdown
# <Mathcad type>

- **Original:** <assembly> <version>, SHA-256 <…>, MVID <…>
- **REA:** <evidence id> (inspect_managed_members); decompiled view: ilspycmd <version>
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
