# Evidence records (PLAN.md §3c rule 2)

One file per Mathcad type, named after the Mathcad type (`Mcd.Rational.md`). A member's
`[MirrorAnalyzed("Mcd.Rational")]` or `[MirrorTested("Mcd.Rational")]` points here; `[MirrorVerified(test)]` members
have a record here too. `Quire.Mirror.Tests` fails when a referenced record is missing.

## How the analysis is done

1. **Reference.** `python tools\reference\reference.py <Mathcad type> [--il]` writes, under the gitignored
   `.mirror-cache/<assembly>/<sha>/`, the type's ilspycmd C# view, a member index (token, owner, signature, access of
   every method, nested types included) headed by the artifact's version, SHA-256 and MVID, and with `--il` the
   type's IL. Read the C# view; turn to the IL where the C# hides a detail: evaluation order, `checked` arithmetic,
   numeric conversions (`conv.r4`/`conv.r8`), how `==` was lowered, exception filters, branch layout. None of it is
   ever committed: the merge gate rejects `.il` files, `docs/mirror/il/` and `.mirror-cache/`.
2. **Limits.** Use `ilspycmd` for readable managed IL and C# views. Native bodies and some mixed-mode C++/CLI
   implementation details may not be visible in those views. Record the metadata and direct observations that
   are available, and leave unavailable or ambiguous behavior open or blocked; do not infer it.
3. **Contract and reconstruction.** Summarize each member as a contract: inputs, outputs, errors, mutation, edge
   cases and every branch, which test cases are chosen from. Quire's body is a reconstruction of the original
   source: same control flow, evaluation order, calls, locals, exceptions and mutation, written as idiomatic C#
   with the constructs the compiler evidently lowered (`foreach`, `using`, `switch`, `??`, lambdas, iterators,
   properties). No `goto`, compiler-generated names or other decompiler artifacts; the decompiled view is a
   reading aid, never the implementation. Where the original admits several source forms, choose the most
   idiomatic one and note material ambiguity under Confidence / limits. The
   [structural gate](../validation.md#reconstruction-integrity) compares the result with the original IL read
   from the artifact. Branch layout counts: `if`/`return` and `?:` order their arms oppositely, so pick the form
   whose IL order matches. Where the IL shows an older lowering (`string.Format` for interpolation), write that call.

## Record format

The validator binds every direct comparison to this record: the record must name the original's SHA-256 and MVID
and the token of each compared member, all copied from the member index.

```markdown
# <Mathcad type>

- **Original:** <assembly> <version>, SHA-256 <…>, MVID <…>
- **Reference:** ilspycmd <version> (tools/reference); record any unavailable native or mixed-mode details under Confidence / limits
- **Quire:** <source path(s)>; tests: <test classes>

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|

## Test cases
Observed (differential or black-box against Mathcad) versus inferred (expected values worked out from the contract).

## Open
Anything not settled; such members stay Analyzed or Blocked.
```

Older records may include legacy inspection identifiers or normalized-IL hash columns; they are not required and
may be dropped when a record is next edited.

A member is **Tested** when Quire's behaviour tests for its contract pass, and **Verified** only for the cases a
differential test compared with the installed Mathcad.
