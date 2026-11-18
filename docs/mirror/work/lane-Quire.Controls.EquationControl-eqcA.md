# Quire.Controls.EquationControl half A lane checkpoint

- Owner: dispatched EquationControl worker (half A)
- Branch: `mirror/eqcontrol-half-a-20261119`
- Landed members:
  - `Controls.EquationControlConstants`: 26 members landed in commit `040627e9f`.
  - `Controls.InputControls.PythonDetector`: 12 exact members landed (`FindPythonPath`, `GetPythonPathFromEnvironmentVariables`, `GetPythonPathFromCommonLocations`, `ExtractVersionFromFolderName`, `IsVersionInRange(string)`, `IsVersionInRange(string, out Version)`, `IsValidPythonDirectory(string)`, `IsValidPythonDirectory(string, out Version)`, `FindPythonDllPath`, `GetMajorMinorVersion`, `GetPythonVersionFromExecutable`, `.cctor`, plus static fields `minVersion`, `maxVersion`, `PythonExecutable`, `possibleRegistryKeys`).
- Release IL: PASS on changed members; 2840 checked (.mirror-cache/release-il-550dae1c7f0b/release-il.json)
- Parked members:
  - `Controls.Internal.ConjugateLayoutManager.FindClosestChild`: parked due to Roslyn compiler emitting `.maxstack 2` while Mathcad 12 binary has `.maxstack 3` on identical 115 IL instructions.
  - `Controls.InputControls.PythonDetector.GetPythonInstallationPath`: branch target/instruction count offset mismatch (Roslyn emits 122 instructions with `br 101` vs Mathcad's 121 instructions with `br 100` across nested using/foreach blocks).

