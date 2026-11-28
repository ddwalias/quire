# Quire.EquationEditor MathNode lane checkpoint

- Owner: MathNode dispatched worker (`task_93312286ecb8`)
- Branch: `feature/codex-eqed-mathnode`
- Assembly: `Quire.EquationEditor`
- Scope: `Quire.EquationEditor.Tree.MathNode` and nested types only.

## Release IL checkpoint

- Last full project report before parking non-exact members: `.mirror-cache/release-il-02a034222f0c/release-il.json` (1,848 checked, 9 MathNode IL mismatches).
- 174 MathNode members were exact in that report; every known non-exact member has been restored to a generated placeholder except the three compiler call-opcode blockers below.
- Exact groups include constructors, descriptor and node info accessors, child list operations, navigation and row helpers, clone, labeling recursion, and descriptor validation helpers.

## MirrorBlocked members

- `MathNode+a_+a__::a()`: the Mathcad call to `MathNode+a::a(int32)` uses the captured `c` field receiver; the Quire method is non-virtual, but Roslyn emits `callvirt` for that field receiver.
- `MathNode+b_::a()`: the Mathcad NodeInfo access uses the captured outer `MathNode` field receiver; Roslyn emits `callvirt`.
- `MathNode+c_::a()`: the Mathcad NodeInfo access uses the captured outer `MathNode` field receiver; Roslyn emits `callvirt`.

The generated declarations for these members were replaced by handwritten `[MirrorStub]` / `[MirrorBlocked]` placeholders so MirrorGen will preserve the explicit blockers. Other mismatches from the cited report remain generated stubs. Remaining generated MathNode stubs are not yet implemented.
