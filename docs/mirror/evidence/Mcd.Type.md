# Mcd.Type

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/Type.cs`; tests: `Quire.Mpl.Tests.Mcd.TypeTests`,
  `Quire.Differential.Tests.TypeDifferentialTests`

An abstract term of Mpl's type language, parsed from signature text such as `V{Length 60000 Mass 60000 Time-120000}`.
The concrete terms are `Constant` (a name starting upper case), `Variable` (any other name), `Application` (an
operator and an operand array; tuples, functions and lists are applications of `()`, `>` and `[]`) and `Product` (an
array of base/exponent `Power`s, the first `m_var` with type-variable bases). All fields are public and mutable, and
no term is interned or copied. `Visitor` is the double-dispatch base; `Factory` builds terms on an untyped stack;
`Lexer`, `Parser`, `Printer` and `Formatter` are private.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Take(Power[], int, int)` | 0x060005ac | `1ba8817d` | The slice of `count` powers from `start`; returns the array itself, not a copy, when the slice is the whole array. | High; directly compared for whole, prefix, empty, suffix, and an out-of-range count, with result identity and contents observed. Other indices/counts are not established by these cases. | Verified |
| `Accept(Visitor)` | 0x060005ad | abstract | Declared only. | High | Abstract |
| `Name` | 0x060005ae | abstract | Declared only. | High | Abstract |
| `Root` | 0x060005af | `3325310d` | The first character of `Name`; so `Constant`/`Variable` give their initial, applications their operator's initial, products `{`. An empty name throws `IndexOutOfRangeException`. | High | Verified |
| `IsImplicitArgument` | 0x060005b0 | `5b55b32c` | False unless overridden. | High | Verified |
| `this[int]` (get) | 0x060005b1 | `3c917796` | Casts the term to `Application` (`InvalidCastException` otherwise); operand 0 is the operator, `i` the i-th operand (out-of-range indices throw `IndexOutOfRangeException`). | High | Verified |
| `this[int]` (set) | 0x060005b2 | `a39159a9` | Same addressing as the getter; replaces the operator or operand in place. | High | Verified |
| `GetDimensionTypeProduct()` | 0x060005b3 | `f3b4cb64` | Strips any number of `M` wrappers by taking operand 1. For an `F`, `R` or `V` term, its operand 1 if that is a product, otherwise a new one-power product of it with exponent 1 (polymorphic when the base is a `Variable`). A bare product returns itself; anything else returns null. Malformed wrappers (no operand) throw. | High | Verified |
| `ToString()` | 0x060005b4 | `6fb15e51` | The term in signature syntax, through `Printer`. | High | Verified |
| `Format()` | 0x060005b5 | `9724033f` | Display text through `Formatter`, using the calling thread's `RuntimeOptions.the_unit_system`. | High; needs a synced thread | Verified |
| `Format(StringBuilder)` | 0x060005b6 | `be662c14` | Appends the display text to the builder, using the thread's unit system. | High | Verified |
| `Format(StringBuilder, UnitSystem)` | 0x060005b7 | `c357f2b3` | Appends the display text, naming dimensions by the given system. | High | Verified |
| `Parse(string)` | 0x060005b8 | `886be8e7` | A new `Parser` over the text, parsed once. Trailing input after a complete term is ignored; syntax errors throw a plain `Exception` "ParseAtomic: syntax error". | High | Verified |
| `.ctor()` | 0x060005b9 | `41ad57d1` | Protected; no state. | High | Tested |
| `Named.GetHashCode()` | 0x06000752 | `801192b8` | The name's string hash; a null name throws `NullReferenceException`. | High | Verified |
| `Named.Equals(object)` | 0x06000753 | `97593601` | Ordinal name equality with any `Named` (a constant equals a variable of the same name). A non-`Named` argument throws `InvalidCastException`; null throws `NullReferenceException`. | High | Verified |
| `Named.CompareTo(object)` | 0x06000754 | `f656882d` | Culture-sensitive comparison of the names; same casting rules as `Equals`. | High; current culture only | Verified |
| `Named.Name` | 0x06000755 | `a8e56ddb` | The `m_nam` field. | High | Verified |
| `Named.ctor()` | 0x06000756 | `e6b9c09b` | Protected; leaves the name null. | High | Tested |
| `Constant.ctor(string)` | 0x06000757 | `f79c28d6` | Stores the name, null included. Same body as `Variable.ctor`. | High | Verified |
| `Constant.Accept(Visitor)` | 0x06000758 | `70c9ba18` | Calls the visitor's `Constant` overload. | High; directly compared with a `Printer`, including its appended output for the recorded name. | Verified |
| `Constant.IsImplicitArgument` | 0x06000759 | `d92fc96a` | True for the one-letter names C, O, P, T, U and W. | High | Verified |
| `Variable.ctor(string)` | 0x0600075a | `f79c28d6` | Stores the name. | High | Verified |
| `Variable.Accept(Visitor)` | 0x0600075b | `7034a4e1` | Calls the visitor's `Variable` overload. | High; directly compared with a `Printer`, including its appended output for the recorded name. | Verified |
| `Application.ctor(Type, Type[])` | 0x0600075c | `de3c6b2d` | Stores the operator and the operand array as given (no copy, nulls accepted). | High | Verified |
| `Application.Accept(Visitor)` | 0x0600075d | `7f534216` | Calls the visitor's `Application` overload. | High; directly compared with a `Printer`, including its appended output for the recorded application. | Verified |
| `Application.IsImplicitArgument` | 0x0600075e | `26d741f6` | True when the operator's name is `I`. | High | Verified |
| `Application.Name` | 0x0600075f | `a1b2dbde` | The operator's name. | High | Verified |
| `Application.Arity` | 0x06000760 | `10f9b18e` | The operand count; a null operand array throws `NullReferenceException`. | High | Verified |
| `Product.ctor(Power[], int)` | 0x06000761 | `22a66c0e` | Stores the array and the variable count `v`; the constant count is the rest. A null array throws `NullReferenceException`; `v` is not range-checked. | High | Verified |
| `Product.Accept(Visitor)` | 0x06000762 | `fc7f20f3` | Calls the visitor's `Product` overload. | High; directly compared with a `Printer`, including its appended output for the recorded product. | Verified |
| `Product.Name` | 0x06000763 | `4288c2da` | `{}`. | High | Verified |
| `Product.IsMonomorphic` | 0x06000764 | `d1552a46` | No variable powers. | High | Verified |
| `Product.IsPolymorphic` | 0x06000765 | `d45f4854` | Some variable powers (any nonzero count, negative included). | High | Verified |
| `Product.IsDimensionless` | 0x06000766 | `eb10c42e` | No powers at all. | High | Verified |
| `Product.Variables` | 0x06000767 | `9b461c9f` | The first `v` powers through `Take`, so the same array when all powers are variables; a count out of range throws. | High | Verified |
| `Product.Constants` | 0x06000768 | `3b970061` | The remaining powers through `Take`, so the same array when there are no variables. | High | Verified |
| `Visitor.Traverse(Type)` | 0x06000769 | `47a59941` | `t.Accept(this)`; children are not visited unless an overload does it. | High | Tested |
| `Visitor.Visit(Constant)` | 0x0600076a | `ae6db128` | Does nothing. | High | Tested |
| `Visitor.Visit(Variable)` | 0x0600076b | `ae6db128` | Does nothing. | High | Tested |
| `Visitor.Visit(Application)` | 0x0600076c | `ae6db128` | Does nothing. | High | Tested |
| `Visitor.Visit(Product)` | 0x0600076d | `ae6db128` | Does nothing. | High | Tested |
| `Visitor.ctor()` | 0x0600076e | `41ad57d1` | Protected; no state. | High | Tested |
| `Factory.Push(Type)` | 0x0600076f | `4ab3ddc4` | Pushes the term. | High | Verified |
| `Factory.Push(Type, int)` | 0x06000770 | `b31836db` | Pushes the base, then the boxed exponent. | High | Verified |
| `Factory.Pop()` | 0x06000771 | `9b2b592d` | Pops and casts to `Type`: an empty stack throws `InvalidOperationException`, a boxed exponent `InvalidCastException`. | High | Verified |
| `Factory.Pop(int)` | 0x06000772 | `bbf45288` | The top `n` terms in pushed order. A negative count throws `OverflowException` from the array allocation. | High | Verified |
| `Factory.NewConstant(string)` | 0x06000773 | `a32a4060` | A new `Constant`. | High | Verified |
| `Factory.NewVariable(string)` | 0x06000774 | `91d69f32` | A new `Variable`. | High | Verified |
| `Factory.NewApplication(Type, Type[])` | 0x06000775 | `526c8cd9` | The term itself when the operand array is null or empty, else a new `Application`. | High | Verified |
| `Factory.NewApplication(int)` | 0x06000776 | `e90e5a55` | Pops `n − 1` operands, then the operator, and always builds an `Application` (even of zero operands). `n = 0` throws. | High | Verified |
| `Factory.NewApplication(Type, int)` | 0x06000777 | `0bd6a8e4` | Pops `n` operands and defers to the array overload (so `n = 0` returns the term). | High | Verified |
| `Factory.NewTuple(int)` | 0x06000778 | `a16d9b99` | 0 gives the constant `()`, 1 pops and returns the single term, more gives `()` applied to the popped terms. Negative counts reach `Pop(int)` and throw. | High | Verified |
| `Factory.NewFunction(Type, Type)` | 0x06000779 | `38dd29b9` | `>` applied to domain and range. | High | Verified |
| `Factory.NewList(Type)` | 0x0600077a | `646676f0` | `[]` applied to the element. | High | Verified |
| `Factory.NewProduct(int, int)` | 0x0600077b | `654c8f11` | Pops `v + c` (exponent, base) pairs into a product, last pair last, with `v` variable powers. A term where an exponent is expected throws `InvalidCastException`. | High | Verified |
| `Factory.ctor()` | 0x0600077c | `c061bc4a` | An empty stack of capacity 32. | High | Verified |
| `Lexer.ctor(string)` | 0x0600077d | `4cee5f4c` | Appends a `$` sentinel to the input and starts at offset 0. A null input becomes just the sentinel. | High; directly compared with `V{Length}`. Other constructor inputs are not established. | Verified |
| `Lexer.GetToken(out string)` | 0x0600077e | `91aa9ddf` | Skips white space, then returns a token kind and its lexeme: `C` for a letter-or-digit run starting with an upper-case letter, `a` for one starting with any other letter, `i` for a digit run, `>` for `->`, the character itself otherwise (a lone `-` included). At the sentinel it returns `$` and does not advance, so an embedded `$` reads as end of input. | High; directly compared for 20 token reads across `V{Length 60000}` and `-> , -12 abc123   spaces   () [] {} $`; other source strings are not established. | Verified |
| `Parser.ctor(string)` | 0x0600077f | `ec5df1eb` | A new factory and lexer, no lookahead. | High; directly compared with `V{Length 60000}`. Other constructor inputs are not established. | Verified |
| `Parser.Parse()` | 0x06000780 | `6e0e8e29` | An application, then, if an arrow follows, a function to the recursively parsed rest (right-associative). | High; directly compared for function, product, tuple, list, application, vector dimension, and product with variable/constant exponents. Other signatures are not established by these cases. | Verified |
| `Parser.ParseApplication()` | 0x06000781 | `ac3cce0b` | An atomic head, then atomic operands while the lookahead is a name, `{`, `[` or `(`; the head alone when there are none. | High; directly compared for `F a b`, `a->b`, `a->b->c`, `a`, and `(a, b)`; other applications are not established. | Verified |
| `Parser.ParseAtomic()` | 0x06000782 | `2d32776e` | Constant, variable, list `[t]`, product `{…}` (variable powers first, then constant powers, each with an optional exponent) or tuple `(…)`. Closing brackets are consumed without checking what they are. Anything else throws "ParseAtomic: syntax error". | High; directly compared for 12 constant, variable, list, tuple, product, empty, grouped and nested forms listed in `TypeDifferentialTests`; other atomic forms are not established. | Verified |
| `Parser.ParseExponent()` | 0x06000783 | `5cbeeb15` | An optional `-` then optional digits; 1 by default, a lone `-` is −1. Digits past int range throw `OverflowException`. | High; directly compared for `-12`, `5`, empty, `0`, an overflowing integer, and `junk`; other sign/digit strings are not established by these cases. | Verified |
| `Parser.IsNextToken(string)` | 0x06000784 | `3054af1f` | Fills the lookahead if empty and reports whether its kind is in the set, without consuming it. | High; directly compared once on source `V{Length 60000}` with token set `V`; the uppercase token is categorized as `C`, so this case does not match. Other token sets/lookahead states are not established. | Verified |
| `Parser.GetNextToken()` | 0x06000785 | `ff43174d` | Consumes the lookahead if there is one, else reads a token. | High; directly compared once with a pending lookahead token after `IsNextToken`; other token states are not established. | Verified |
| `Parser.GetNextTokenIf(string)` | 0x06000786 | `a4812021` | Consumes the lookahead only when its kind is in the set. | High; directly compared once on source `V{Length 60000}` with token set `V` and pending token kind `C`; this case does not match. Matching token cases are not established. | Verified |
| `Parser.SkipToken(char)` | 0x06000787 | `39812059` | Consumes one token, ignoring the expected kind. | High; directly compared once with expected `{` and the next token from `V{Length 60000}` (`{`); other token states are not established. | Verified |
| `Printer.ctor(StringBuilder)` | 0x06000788 | `0b274f70` | Keeps the builder. | High; directly compared with a fresh builder. Other constructor effects are not established. | Verified |
| `Printer.Visit(Constant)` | 0x06000789 | `61825178` | Appends the name. | High; directly compared with constant `X`. Other names are not established. | Verified |
| `Printer.Visit(Variable)` | 0x0600078a | `61825178` | Appends the name. | High; directly compared with variable `y`. Other names are not established. | Verified |
| `Printer.Visit(Application)` | 0x0600078b | `9532602c` | Tuples as `(a, b)`, functions as `a->b` (no parentheses, so a function domain loses its grouping), other applications as `(F a b)`. | High; directly compared for `F a b`, `(a, b)`, `a->b`, and `[a]`; other forms are not established. | Verified |
| `Printer.Visit(Product)` | 0x0600078c | `c5ff4547` | `{` then base and exponent pairs separated by spaces, an exponent of 1 left out, then `}`. | High; directly compared for `{}`, `{a}`, `{Length -60000}`, `{a 2 Length 60000}`, and `{a 2 b-3 Mass}`; other forms are not established. | Verified |
| `Formatter.ctor(StringBuilder, UnitSystem)` | 0x0600078d | `cb48bb7b` | Keeps the builder and system and makes its own factory. | High; directly compared with a fresh builder and the default SI unit system. Other systems are not established. | Verified |
| `Formatter.Visit(Variable)` | 0x0600078e | `df3cc6fa` | One-letter names print as `any` plus the letter's offset from `a` plus one (so `a` is any1; other letters give whatever the arithmetic gives). `t` followed by a digit prints `any` plus the parsed number plus 26 (non-digits after it throw `FormatException`). Other names print as they are. | High; directly compared for `a`, `b`, `t1`, `t12`, and `custom`; other names and malformed `t` suffixes are not established. | Verified |
| `Formatter.Visit(Application)` | 0x0600078f | `17fa7d33` | `D`, `N` and `V` print only operand 0; `M` prints operand 0 in brackets; tuples print as in `Printer`; a function prints only its range when its domain is an implicit argument, else through `OnFunction`; other applications print operator and operands separated by spaces, without parentheses. | High; directly compared for `M(V{Length 60000})`, `(a, b)`, `a->b`, `[a]`, `F a b`, `(I a)->b`, `M(M(V{Time 60000}))`, `D a`, `N{Length 60000}`, and `V a`; other forms are not established. | Verified |
| `Formatter.OnFunction(Type, int)` | 0x06000790 | `b2e19e7c` | Pushes each curried domain while the term is a function, then prints `ƒ(d1, d2, …) → ` and the final range. | High; directly compared with `?`, constant `c` and `a->b->c`, each at argument index 0; other terms and indices are not established. | Verified |
| `Formatter.Visit(Product)` | 0x06000791 | `f73dc225` | `Unitless` for no powers; otherwise the variable powers, then the quantities of the system's simplified units, joined by ` · `, each with `^exponent` unless 1. Dimensions the system doesn't know throw `ArgumentException` "bad dimension". | High; directly compared for the 12 listed product forms in `TypeDifferentialTests`, with the default SI unit system; other systems and product forms are not established. | Verified |

**Private helpers.** `Take` is a private method; `Lexer`, `Parser`, `Printer` and `Formatter` are private nested
types in both assemblies. `TypeDifferentialTests` invokes many of their members directly through reflection and also exercises
these helpers through `Parse`, `ToString` and the three `Format` overloads. Each status remains case-limited to its
recorded direct comparisons; indirect calls alone do not establish a member's Verified status. The four `Accept`
overrides and `Visitor.Traverse` are also directly compared. The indexer is directly compared but carries
`[MirrorTested]` in the source.

**Token order.** The four `Visitor.Visit` overloads share one body, and so do the two `Printer.Visit` overloads for
names; their rows follow declaration order (Constant, Variable, Application, Product). The three
`Factory.NewApplication` overloads are told apart by parameter count and IL size.

Fields: `Named.m_nam`, `Application.Operator`/`Operands`, `Product.Powers`/`m_var` and `Power.Base`/`Exponent` are Verified (dumped by `TypeDifferentialTests`); `Product.m_con` and the private parser, lexer, printer, formatter and factory state are Tested.

## Test cases

**Observed** (`TypeDifferentialTests`, against the installed `mplrun.dll`; outcomes compared as text, field dumps
or exception type and message):
- `Parse` of 98 signatures, each term dumped field by field with `ToString`, `Name`, `Root`, `IsImplicitArgument`,
  `GetDimensionTypeProduct`, `Format(sb, default SI)`, `Arity` for applications and, for products,
  `IsMonomorphic`, `IsPolymorphic`, `IsDimensionless`, `Variables` and `Constants` (including whether the slice is
  the product's own array); then `Format()` and `Format(sb)` on the thread's unit system. The signatures cover
  dimension products and value wrappers (`V`, `F`, `R`, `M`, `D`, `N`, nested `M`, empty products, repeated and
  unknown dimensions, missing and huge exponents, variable bases), applications, functions (curried, grouped, with
  implicit-argument domains), tuples and lists, name lexing (the implicit one-letter constants, `t1`, `t12`, Greek
  and accented letters, `$`, digits, underscores, white space), syntax errors and trailing input;
- `Constant` and `Variable` constructors on 10 names (empty, null, Greek, `COPTUW` among them), `Equals` and
  `CompareTo` across all pairs and across kinds, `Equals(null)`, `Equals(product)` and `GetHashCode`;
- `Application` with empty and null operands, `Product` with a null array and with variable counts −1 to 3;
- the four concrete `Accept` overrides with `Printer` visitors: `Constant("X")`, `Variable("y")`, an `F(a, b)` application and a two-power product; after each direct comparison the visitor output buffer is compared between artifacts;
- the indexer, read and written, at operands −1 to 4 on six terms (applications, a function, a tuple, a variable
  and a product);
- one script of `Factory` calls: every `New*` method, `Push` of terms and powers, `Pop` and `Pop(n)` including an
  empty stack, a negative count and a boxed exponent on top, a mismatched `NewProduct`, `NewTuple(-1)` and
  `NewApplication(0)`.

**Inferred** (`TypeTests`): value signatures as `V` applied to a product, variable-first ordering and default
exponents, full slices being the same array, functions/tuples/lists, syntax errors and name kinds, implicit
arguments, indexer access, dimension products of value types, `Format`'s `anyN` names and quantities, name
comparison of named terms, `Accept` dispatch to each visitor overload (with the base overloads doing nothing), and
the factory's stack discipline.

## Open

- No direct comparison is possible for the protected constructors of the abstract `Type`, `Named` and `Visitor` or
  for the four `Visitor.Visit` base bodies: reflection cannot construct an abstract type, and the only concrete
  `Visitor` in the installed managed assemblies (mplrun's `Printer`; `mccore12` and `msclr` derive none) overrides all
  four overloads, so a virtual invocation never reaches the base. Comparing them needs the harness to run an
  instance constructor on a supplied object (or a non-virtual call), which `MethodComparison` does not offer.

- `Named.CompareTo` is culture-sensitive; only the test process's culture was compared.
- `Formatter.Visit(Variable)` on single characters outside `a`–`z` and `Formatter` on a system other than the
  default SI were not compared.
- `Parse` of `null` was not compared.
