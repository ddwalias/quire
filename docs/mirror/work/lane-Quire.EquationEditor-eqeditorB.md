# Quire.EquationEditor, half B checkpoint

- Owner: EquationEditor half-B worker (task_a64a09191d34)
- Branch: `mirror/eqeditor-b-20261119-aj-bn`
- Scope: `Native.PtcEquationEditor` files from `a3` to the end of the sorted generated list; the `a2`-and-earlier half belongs to the other worker.

## Landed

- `Native.PtcEquationEditor.aj`: six constant fields `a`-`f` moved to a hand-written partial (0 stubs left).
- `Native.PtcEquationEditor.bn`: constructor and the four fields (`a`-`d`) moved to a hand-written partial. `[SkipLocalsInit]` on the constructor matches Mathcad's `.initlocals False`. The `bq()` override stays a generated stub.

## Release IL report

- `--project Quire.EquationEditor` (base main): PASS, 976 checked, 0 failures.
- `--type bn` still fails only on the unimplemented `bq()` stub, which the type gate selects.
- `--type aj` reports `selected type not found` because the tool does not select a struct that has only constants; no member fails.

## Parked (named blockers)

- Clash types (field and method share a name, so MirrorGen suffixes the field `_`): 44 files in this half, e.g. `a3` (field `a` vs `a()` methods). Parked per the lane brief.
- Members of `bh`, `bi`, `bm`, `bn`, `a8` that read base fields `((a0)this).b` or `((a0)this).c`: `a0` (the other half) has a method `c()` and a method `b()`, and the field of the same name is not generated, so the C# cannot name the base field. Blocked until `a0` is resolved.
- `bn.bq()` also has a stack shape: `bq.c(...)` is evaluated before `num = b + d.Length`, and a named local is not in the IL. Not yet resolved.
- `a9.bq()` depends on base helper names `g()`, `d()` and the `bq`/`bg` types; not attempted.

## Remaining

- Half B still has about 1,920 generated stubs across 59 files. Of those files, 15 have no `_`-suffixed field (`a8`, `a9`, `ag`, `ai`, `aj`, `bh`, `bi`, `bk`, `bm`, `bn`, `bs`, `j`, `m`, `n`, `PersistentDataObjects.MathTransformer`). Most of their remaining stubs are method bodies that call the other half's obfuscated base members, so they need the `a0` resolution first.
