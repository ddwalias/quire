# Quire.Controls.EquationControl half A lane checkpoint

- Owner: dispatched EquationControl worker (half A)
- Branch: `mirror/eqcontrol-half-a-20261119`
- Selected members:
  - `Controls.EquationControlConstants`: `EquationHelpContext`, `ScaledEquationHelpContext`, `Identifier`, `Operator`, `Odesolve`, `Pdesolve`, `Numol`, `Genfit`, `Find`, `Maximize`, `Minerr`, `Minimize`, `polyroots`, `MultiStart`, `Optimize`, `DefiniteIntegralContextMenu`, `OdeSolveContextMenu`, `NumolAndPdesolveContextMenu`, `GenfitContextMenu`, `FindAndMinerrContextMenu`, `MinimizeAndMaximizeContextMenu`, `PolyrootsContextMenu`, `SymbolicEvalContextMenu`, `DefaultAlgorithms`, `EnabledAlgorithmsForOptions`, `specialIdentifiers` (26 members)
- Release IL: PASS; 2820 checked (.mirror-cache/release-il-b261093be309/release-il.json)
- Parked members:
  - `Controls.Internal.ConjugateLayoutManager.FindClosestChild`: parked due to Roslyn compiler emitting `.maxstack 2` while Mathcad 12 binary has `.maxstack 3` on identical 115 IL instructions.

