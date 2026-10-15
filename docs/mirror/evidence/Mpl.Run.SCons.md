# Mpl.Run.SCons

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

**Status rule used in this batch.** *Verified*: the differential test runs every branch of the member's own body
against Mathcad. *Tested*: Quire's behaviour tests run every branch (with test doubles where a branch needs a virtual
callee), but the differential doesn't reach all of them. *Analyzed*: implemented from the contract, but a branch
calls a member of another batch that is still a stub in Quire, so it can't run yet.

A list cell. Field values are kept as given: a null `Field2` is not read as the empty list (pre-§3c Quire converted it to `Nul`).

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Field1, Field2` | 0x04000010, 0x04000011 | — | Public mutable element and rest. | High | Verified |
| `token` | 0x04000012 | — | `Sym.NewTok("Cons", "MplMacro", 0)` (qualifier dropped). | High | Verified |
| `apply` | 0x04000013 | — | `new Sym("apply", "MplMacro")` (qualifier dropped). | High | Verified |
| `.ctor(object, object)` | 0x06000042 | `a5868a65` | Both converted with `as Sexpr`, so other objects become null. | High | Verified |
| `.ctor(Sexpr, Sexpr)` | 0x06000043 | `36f34cc9` | Stores both as given, null included. | High | Verified |
| `handle` | 0x06000034 | `34e57685` | `Field1.handle`, recursively; null head throws `NullReferenceException`. | High | Verified |
| `scons` | 0x06000035 | `339a830e` | this. | High | Verified |
| `head, car, first` | 0x06000036, 0x06000038, 0x0600003b | `ae1be909` | `Field1` (identical IL). | High | Verified |
| `tail` | 0x06000037 | `7ec9436b` | `Field2 as SCons`. | High | Verified |
| `cdr` | 0x06000039 | `37b87db1` | `Field2`. | High | Verified |
| `Key` | 0x0600003a | `ee30badb` | `Field1.sym`; null head throws. | High | Verified |
| `second … fifth` | 0x0600003c, 0x0600003d, 0x0600003e, 0x0600003f | `53a1c294` (first) | `tail.head`, `tail.tail.head`, …: a shorter list throws `NullReferenceException` (pre-§3c Quire returned null). | High | Verified |
| `Length` | 0x06000040 | `a13d6ec4` | Counts this cell and every cell reached through `tail`; an improper tail stops the count. | High | Verified |
| `.cctor` | 0x06000041 | — | Initialises token and apply. | High | Verified |
| `quote` | 0x06000044 | `a3eca7e8` | `list3(token, Field1.quote, Field2.quote)`. | High | Verified |
| `FormatTail()` | 0x06000045 | `7067159c` | `" " + Field1 + Field2.FormatTail()`. | High | Verified |
| `ToString()` | 0x06000046 | `7de30975` | `"(" + Field1 + Field2.FormatTail() + ")"`; a null `Field2` throws. | High | Verified |
| `walk_declaration_lhs(MplEnv)` | 0x06000047 | `38d77543` | `BadDeclaration(this)` without a head symbol, else `Key.walk_declaration_as(env, this)`. | High; verified across non-symbol heads, symbol heads without macro, function macros throwing AppException("illegal macro call"), and symbol macros expanding to non-symbols. | Tested |
| `walk_patterns(MplEnv, SCons)` | 0x06000048 | `e00e6f0f` | null gives `Nul.singleton`; otherwise walks the head as a pattern and the tail recursively, returning the original cell when both are unchanged (by reference), else a new cell. | High | Verified |
| `walk_pattern(MplEnv)` | 0x06000049 | `4ddb05a3` | No head symbol: `BadPattern`. A head symbol that isn't lower case: a new cell of the head and `walk_patterns(tail)` (always a new top cell). A lower-case one: its macro expands the form and walks the result as a pattern; with no macro a one-element list adds a local `(key, handle)` and returns this, a longer one is `BadPattern`. | High; differential and unit tests cover all paths. Uncovered sequence points follow the `BadPattern` throwing helper. | Tested |
| `walk_expressions(MplEnv, SCons)` | 0x0600004a | `cba4af7e` | As `walk_patterns`, with `walk_expression`. | High | Verified |
| `walk_expression(MplEnv)` | 0x0600004b | `15e2b46a` | A head symbol walks the form (`Key.walk_expression_as`). Otherwise the result is `(apply e1 e2 …)`: a new cell headed by `new Tok(apply, handle)` over `walk_expressions(env, this)`. The null-head `BadApplication` check is unreachable because `Key` already dereferences the head. An improper tail is dropped. | High | Verified |
| `list1 … list4` | 0x0600004c, 0x0600004d, 0x0600004e, 0x0600004f | `38a6bf2f` (first) | Proper lists ending in `Nul.singleton`. | High | Verified |
| `as_Sexpr(Cons)` | 0x06000050 | `f12a10a3` | null gives `Nul.singleton`; otherwise a cell per item through the `(object, object)` constructor, so items that aren't S-expressions become null. | High | Verified |

## Test cases

As in Mpl.Run.Sexpr.md. Direct differential comparisons against Mathcad in `SexprDifferentialTests` verify
`SCons.walk_expression`, `SCons.walk_expressions`, `SCons.walk_pattern`, and `SCons.walk_declaration_lhs` across
symbol macro calls, function macro calls throwing `AppException("illegal macro call")`, non-symbol heads `(1 2 3)`
and `((a b) c)` in expression and pattern positions, single-element lowercase patterns registering locals in
`LocalEnv`, and `walk_expressions` returning a new chain when macro arguments change.

## Open

- Null-environment cases in `walk_declaration_lhs` pass a null `MplEnv` to both sides; they show only that the
  environment is dereferenced before any other observable effect, not which branch follows.
