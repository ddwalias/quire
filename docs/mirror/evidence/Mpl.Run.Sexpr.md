# Mpl.Run.Sexpr

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Sexpr.cs`; tests: `Quire.Mpl.Tests.Run.SexprTests`, `Quire.Differential.Tests.SexprDifferentialTests`

**Status rule used in this batch.** *Verified*: the differential test runs every branch of the member's own body
against Mathcad. *Tested*: Quire's behaviour tests run every branch (with test doubles where a branch needs a virtual
callee), but the differential doesn't reach all of them. *Analyzed*: implemented from the contract, but a branch
calls a member of another batch that is still a stub in Quire, so it can't run yet.

Abstract base of the S-expression family. It overrides nothing from `object`, so equality is by reference.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `NulTag … SListTag` | 0x04000005–0x0400000d | — | Constants 0 (Nul), 1 (Cons), 2 (Tok), 3 (Qt), 4 (Bq), 5 (Cm), 6 (Ca), 7 (SAppend), 8 (SList). | High | Verified |
| `.ctor(int)` | 0x0600000c | `3a7dcc25` | Passes the tag to `Tag`. Same normalized IL as `Atom`'s constructor. | High | Tested |
| `handle` | 0x0600000d | `5b55b32c` | 0. | High | Verified |
| `Key` | 0x0600000e | `ce2965e0` | null. Same IL as `sym`, `str` and `walk_declaration_lhs`. | High | Verified |
| `sym` | 0x0600000f | `ce2965e0` | null. | High | Verified |
| `str` | 0x06000010 | `ce2965e0` | null. | High | Verified |
| `AsSym` | 0x06000011 | `feb5278b` | `sym`, or `NotASymbol(this)` when null. | High | Verified |
| `AsStr` | 0x06000012 | `f98947eb` | `str`, or `NotAString(this)` when null. | High | Verified |
| `AsKey` | 0x06000013 | `f45f1d23` | `Key`, or `NotAKeyword(this)` when null. | High | Verified |
| `quotation` | 0x06000014 | `1c7bba9a` | `NotAQuotation(this)`. | High | Tested |
| `quote` | 0x06000015 | `321df6ab` | `QuotationNotImplemented(this)`. | High | Tested |
| `scons` | 0x06000016 | `b124948e` | `ExpectedAList(this)`; only lists override it (Nul gives null). | High | Tested |
| `FormatTail()` | 0x06000017 | `297fc615` | `" . " + ToString()`: the dotted tail of an improper list, without the closing parenthesis (the list adds it). | High | Verified |
| `FormatAppend()` | 0x06000018 | `05f10a6f` | `" " + ToString()`. | High | Verified |
| `Walk(MplEnv, Sexpr)` | 0x06000019 | `14fee3d4` | `block.walk(env, out e)`, returned as `[e, definitions]`. | High | Verified |
| `WalkModule(MplEnv, Sexpr)` | 0x0600001a | `273fd630` | `block.walk(env, out _)`, then a list headed by `block_keyword.NewTok(0)` followed by `SCons.as_Sexpr` of the definitions. | High | Verified |
| `walk(MplEnv, out MplEnv)` | 0x0600001b | `f1c250b4` | Makes `new LocalEnv(env)` the out environment, then `walk_definitions(walk_declaration_lhs(outenv), outenv)`. | High; Quire's `LocalEnv(parent)` is still a no-op stub, so the parent link isn't observable yet | Verified |
| `walk_definitions(Cons, MplEnv)` | 0x0600001c | `8d61c081` | For each cell, replaces `Field1` with `walk_definition(Field1 as SCons, env)`, in place; returns the first cell. | High | Tested |
| `walk_definition(SCons, MplEnv)` | 0x0600001d | `c824ae41` | `decl.Key.walk_definition_as(env, decl)`; a non-symbol head throws `NullReferenceException`. | High | Tested |
| `walk_declaration_lhs(MplEnv)` | 0x0600001e | `ce2965e0` | null. | High | Verified |
| `walk_pattern(MplEnv)` | 0x0600001f | `a1b033ff` | `BadPattern(this)`. | High | Tested |
| `walk_expression(MplEnv)` | 0x06000020 | `f11dedb3` | `BadExpression(this)`. | High | Tested |
| `walk_with_bindings(MplEnv, SCons, out Sexpr)` | 0x06000021 | `07826449` | In `new LocalEnv(env)`: the out value is `SCons.walk_patterns(scope, pats)`, the result `walk_expression(scope)` on this. | High | Verified |
| `BadDeclaration … QuotationNotImplemented` | 0x06000022, 0x06000023, 0x06000024, 0x06000025, 0x06000026, 0x06000027, 0x06000028, 0x06000029, 0x0600002a, 0x0600002b, 0x0600002c, 0x0600002d | `9751fe68` (first) | Each throws `new AppException(message, 0, form.handle)` with its message: "illegal declaration", "bad pattern", "bad expression", "expected a list", "syntax error", "bad application", "expected a symbol", "expected a string", "expected a keyword", "not implemented", "not a quotation", "quotation not implemented". A null form throws `NullReferenceException` first. | High; messages not compared (Quire `AppException` is a stub) | Verified |

## Test cases

**Observed** (`SexprDifferentialTests`, against the installed `mplrun.dll`; results compared as structure — type and
mplrun fields, recursively — text, identity or exception type): about 50 trees covering every subclass (handles 0
and non-zero, symbols upper/lower/empty/null/non-ASCII, qualified and keyword-tagged symbols, int/real/string/char
atoms including NaN, -0 and null text, null atoms, improper lists, null `Field1`/`Field2`, shared cells, nested
lists, quote forms, `SAPPEND` chains, `SLIST`s ending in `Nul.singleton`, in another `Nul` and in a token); on each,
`ToString`, `FormatTail`, `FormatAppend`, `handle`, `Key`, `sym`, `str`, `AsSym`, `AsStr`, `AsKey`, `quotation`,
`quote`, `scons`, `m_tag`, and on lists `Length`, `head`, `tail`, `car`, `cdr`, `first`…`fifth`. Environment-free
walks (`walk_expression`, `walk_pattern`, `walk_declaration_lhs`, `walk_patterns`, `walk_expressions`,
`walk_with_bindings`, `walk`, `Walk`, `as_Sexpr`, `Sym.walk_definition_as`) on inputs that never reach a lower-case
symbol in expression position or an upper-case one in pattern position, with a Mathcad `LocalEnv` on Mathcad's side;
every error helper on four forms (including null); every constructor and factory on ordinary and edge arguments;
every constant and static field.

Exception messages and `AppException.Handle` are not compared: Quire's `AppException` is still a stub (its
constructors drop the message), owned by another batch. Only the exception type is compared.

**Inferred** (`SexprTests`, expected values worked out from the contracts): the printed forms, dotted tails, `SLIST`
spacing, hex handles, head handles, `NullReferenceException`s from short lists and null heads, the `apply` rewrite,
identity preservation in walks, and the `walk`/`walk_definitions`/`walk_definition`/`walk_declaration_lhs` dispatch
through test doubles (a `Sexpr` subclass returning given declarations, a `Sym` subclass defining them).

## Open

- `WalkModule` waits for `block_keyword.NewTok`; `walk`'s environment chaining waits for `LocalEnv`. Compared so far:
  a block whose declaration has no head symbol (`BadDeclaration`) and a null block.
- Paths that need the environment are compared with a null environment on both sides, so they stop at the first
  environment call (`MplEnv.LoadMacro`, `MplEnv.AddLocal`) with `NullReferenceException`; what follows waits for
  `MplEnv`/`LocalEnv`/`MplMacro`/`Keyword`.
- Unreachable lines: the `return null;` (and closing brace) after the always-throwing helper in the base `quotation`,
  `quote`, `scons`, `walk_pattern` and `walk_expression`, as in the original IL; the branch-coverage gate reports them
  as uncovered.
- `.ctor(int)` (here and on `Atom`) belongs to an abstract class: `MethodComparison` invokes constructors with
  `ConstructorInfo.Invoke(args)`, which cannot instantiate an abstract type, so no direct comparison is possible.
- Messages and handles of the thrown `AppException`s can be compared once `AppException` is implemented.
