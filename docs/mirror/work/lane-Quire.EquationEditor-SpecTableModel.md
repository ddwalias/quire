# Quire.EquationEditor SpecTableModel lane checkpoint

- Owner: SpecTableModel dispatched worker (`task_e47b7f5afa35`)
- Branch: `feature/codex-eqed-spectablemodel`
- Type: `Quire.EquationEditor.SpecTable.Model.SpecTableModel` and nested types

## Batch 1 — table accessors

- Implemented `Columns` and `ColumnCount` in the hand-written partial and removed their generated stub declarations.
- Release IL report: pass against the branch base (`.mirror-cache/release-il-4b6536f7abe7/release-il.json`); lint passes.

## Batch 2 — row and column queries

- Added `NumberOfDataCells`, `CellCount`, `IsExtraDataCellOrEmptyScale`, `RemoveEmptyColumns`, `IsRowEmpty`, `IsColumnEmpty`, `GetColumn`, `AddColumnAtEnd`, and `AddRowAtEnd`; removed their generated stubs.
- Release IL report: pending in the lane gate; lint passes.
- Remaining: other SpecTableModel members and nested/closure types.
