# Mpl language notes and source prototype

## What the install reveals

The 51 Mpl assemblies carry a `ModuleInterfaceAttribute` on their module value type; their sampled `ModuleTextAttribute` values are empty. The interface attribute is a UTF-8 custom-attribute string with module exports, data declarations, types, optional external-name annotations, and imports. `python tools/mpl/dump_interfaces.py` decodes every interface and writes its text plus line-oriented declaration records beneath the ignored `.mirror-cache/mpl-interfaces/<artifact-set-hash>/` directory. Re-running it after an install change produces a separate content-addressed directory.

Observed notation is ML-like: `module ... (...) where`, `data`, `import`, `::`, type variables, tuples/lists, and right-associative `->`. Optional `@ "..."` annotations bind source declarations to generated/external names. CLR signatures erase many Mpl values to `object`; `Mpl.Run` supplies runtime value and function classes. These facts do not recover the missing source expression grammar, type inference rules, or original function bodies.

The following is an authored example of the *interface notation*, not text copied from Mathcad:

```mpl
module Example (identity, Box(..)) where
  data Box a = Box a
  identity :: a -> a
```

## Executable prototype subset

`tools/mpl/Quire.Mpl.Compiler` is a deliberately tiny source-to-IL compiler experiment on `Quire.Mpl.Emit`. Its input grammar is only:

```text
module <name> (<exports...>) where
<function> <arguments...> = <expression>
```

The three sample expressions use unit `()`, a qualified field/call expression, or a qualified call with a field value. The examples are authored to test three lowering shapes; they are not asserted to be original Mathcad source. The compiler checks the declared forms, then emits their bodies through `Quire.Mpl.Emit`:

```mpl
module Example (Update, lookup) where
Update () = ()
lookup table key = find (Catalog.Entry table) key
```

`python tools/mpl/test_prototype.py` builds the compiler in Release, emits the sample modules into `.mirror-cache`, builds MethodAudit, and requires exact Release IL for `Hashtable.values::Update`, `Hashtable.values::mlookupInv`, and `McdUnitSystemUS.values::op_L`. The compiler currently recognizes only these three complete examples; it is a proof of source parsing/build wiring and selected IL lowering, not a general Mpl implementation.

## Remaining work

Recover expression syntax and static typing from a broader body corpus, generalize the parser/type checker/lowerer, preserve source member and closure order, and make the compiler emit full module interfaces, attributes, metadata, and generated members. The cache-only interface dump and tiny compiler provide reproducible starting points without checking original binaries, IL, or decompiler output into the repository.
