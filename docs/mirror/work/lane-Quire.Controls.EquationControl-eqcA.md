# Quire.Controls.EquationControl half A lane checkpoint

- Owner: dispatched EquationControl worker (half A)
- Branch: `mirror/eqcontrol-half-a-20261119`
- Landed members:
  - `Controls.EquationControlConstants`: 26 members landed in commit `040627e9f`.
  - `Controls.InputControls.PythonDetector`: 12 exact members landed in commit `e877e4a05` (`FindPythonPath`, `GetPythonPathFromEnvironmentVariables`, `GetPythonPathFromCommonLocations`, `ExtractVersionFromFolderName`, `IsVersionInRange(string)`, `IsVersionInRange(string, out Version)`, `IsValidPythonDirectory(string)`, `IsValidPythonDirectory(string, out Version)`, `FindPythonDllPath`, `GetMajorMinorVersion`, `GetPythonVersionFromExecutable`, `.cctor`, plus static fields `minVersion`, `maxVersion`, `PythonExecutable`, `possibleRegistryKeys`).
  - `Controls.Internal.DerivativeLayoutManager`: 9 exact members landed in commit `4214b2756` (`EXPRESSION_LEFT_PADDING`, `OperatorSymbolRect`, `VariableOperandRect`, `OrderOperandRect`, `ExpressionOperandRect`, `DerivativeSymbolVerticalOffset`, `BaseOperandsOffset`, `.ctor`, `Arrange`).
  - `Controls.Internal.GestureBinder`: 9 exact members (`Bind(VisualMathRegionNode, GestureManager)`, `Bind(VisualTextNodeEditor, GestureManager)`, `Bind<T>(VisualMathNodeEditor<T>, GestureManager)`, `Bind(VisualScoper, GestureManager)`, `LimitSideNodeGestureBind(VisualMathRegionNode, GestureManager)`, `InsertOperatorCommand(VisualMathRegionNode, string, EnumFunctionalityKey)`, `InsertOperatorCommand(VisualMathRegionNode, VisualOperatorDescriptor)`, `FireInsertOperatorWrapper(VisualMathRegionNode, EnumFunctionalityKey, RoutedCommand, object)`, `InsertStringCommand(VisualMathRegionNode, string)`).
- Release IL: PASS on changed members; 2911 checked (.mirror-cache/release-il-7e28b8816030/release-il.json)
- Parked members:
  - `Controls.Internal.DerivativeLayoutManager.FindClosestChild`: parked due to Roslyn compiler emitting `.maxstack 2` while Mathcad 12 binary has `.maxstack 3`.
  - `Controls.Internal.ConjugateLayoutManager.FindClosestChild`: parked due to Roslyn compiler emitting `.maxstack 2` while Mathcad 12 binary has `.maxstack 3` on identical 115 IL instructions.
  - `Controls.InputControls.PythonDetector.GetPythonInstallationPath`: branch target/instruction count offset mismatch (Roslyn emits 122 instructions with `br 101` vs Mathcad's 121 instructions with `br 100` across nested using/foreach blocks).

