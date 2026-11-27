# Original source layout migration map

Original folders come from the original binaries’ CodeView PDB paths (`python tools/reference/pdb_paths.py`), cross-referenced with `docs/mirror/assembly-map.json`. Client paths are taken after `Mathcad_Client\` through the folder before `\obj`; Engine paths are taken after `Mathcad_Engine\` through the folder before `\obj`, `\tgt`, or `\x64`. `(repo root)` means that one of those markers immediately follows the repository root; `unknown` means no usable path was embedded or the binary is outside those trees.

For assembly-map units, the Quire path column is the current path (`src/<unit>`), before migration. “Already at target” identifies a Client unit whose original folder is a flat `Ptc.*` project folder and therefore stays flat at `src/<Quire unit>`; all other units differ and need an explicit source-layout override when moved. No source directories are moved by this plan.

The DevComponents binaries are currently treated as substitutes rather than assembly-map units, but are included because they are the first planned nested-layout consumers:

| Planned unit | Language | Original project folder | Planned Quire path | Status |
| --- | --- | --- | --- | --- |
| `Quire.DevComponents.WpfRibbon` | CSharp | `ThirdParty/DevComponents/DotNetBar.4.0.0.1` | `src/ThirdParty/DevComponents/DotNetBar.4.0.0.1` | differs |
| `Quire.DevComponents.WpfDock` | CSharp | `ThirdParty/DevComponents/DotNetBar.4.0.0.1/WpfDock` | `src/ThirdParty/DevComponents/DotNetBar.4.0.0.1/WpfDock` | differs |

| Unit | Language | Original project folder | Current Quire path | Status |
| --- | --- | --- | --- | --- |
| `Quire.Automation.Native` | CSharp | `mcd/sources/automation` | `src/Quire.Automation.Native` | differs |
| `Quire.Automationmgd` | CSharp | `mcd/sources/automationmgd` | `src/Quire.Automationmgd` | differs |
| `Quire.Componentsmgd` | CSharp | `mcd/sources/componentsmgd` | `src/Quire.Componentsmgd` | differs |
| `Quire.EfiAffine` | Mpl | `unknown` | `src/Quire.EfiAffine` | differs |
| `Quire.EfiBessel` | Mpl | `unknown` | `src/Quire.EfiBessel` | differs |
| `Quire.EfibesselImpl` | CppCli | `(repo root)` | `src/Quire.EfibesselImpl` | differs |
| `Quire.EfiCalculus` | Mpl | `unknown` | `src/Quire.EfiCalculus` | differs |
| `Quire.EficalculusImpl` | CppCli | `(repo root)` | `src/Quire.EficalculusImpl` | differs |
| `Quire.EfiCore` | Mpl | `unknown` | `src/Quire.EfiCore` | differs |
| `Quire.EficoreImpl` | CppCli | `(repo root)` | `src/Quire.EficoreImpl` | differs |
| `Quire.EfiData` | Mpl | `unknown` | `src/Quire.EfiData` | differs |
| `Quire.EfidataImpl` | CppCli | `(repo root)` | `src/Quire.EfidataImpl` | differs |
| `Quire.EfiDebug` | Mpl | `unknown` | `src/Quire.EfiDebug` | differs |
| `Quire.EfidebugImpl` | CppCli | `(repo root)` | `src/Quire.EfidebugImpl` | differs |
| `Quire.EfiDiffEQ` | Mpl | `unknown` | `src/Quire.EfiDiffEQ` | differs |
| `Quire.EfidiffeqImpl` | CppCli | `(repo root)` | `src/Quire.EfidiffeqImpl` | differs |
| `Quire.EfiFinance` | Mpl | `unknown` | `src/Quire.EfiFinance` | differs |
| `Quire.EfifinanceImpl` | CppCli | `(repo root)` | `src/Quire.EfifinanceImpl` | differs |
| `Quire.EfiFitting` | Mpl | `unknown` | `src/Quire.EfiFitting` | differs |
| `Quire.EfifittingImpl` | CppCli | `(repo root)` | `src/Quire.EfifittingImpl` | differs |
| `Quire.EfiFourier` | Mpl | `unknown` | `src/Quire.EfiFourier` | differs |
| `Quire.EfifourierImpl` | CppCli | `efi/efifourier` | `src/Quire.EfifourierImpl` | differs |
| `Quire.EfiGraphics` | Mpl | `unknown` | `src/Quire.EfiGraphics` | differs |
| `Quire.EfigraphicsImpl` | CppCli | `(repo root)` | `src/Quire.EfigraphicsImpl` | differs |
| `Quire.EfiImage` | Mpl | `unknown` | `src/Quire.EfiImage` | differs |
| `Quire.EfiimageImpl` | CppCli | `efi/efiimage` | `src/Quire.EfiimageImpl` | differs |
| `Quire.EfiIO` | Mpl | `unknown` | `src/Quire.EfiIO` | differs |
| `Quire.EfiioImpl` | CppCli | `efi/efiio` | `src/Quire.EfiioImpl` | differs |
| `Quire.EfiioCsharp` | CSharp | `efi/efiio-csharp` | `src/Quire.EfiioCsharp` | differs |
| `Quire.EfiMatrix` | Mpl | `unknown` | `src/Quire.EfiMatrix` | differs |
| `Quire.EfimatrixImpl` | CppCli | `(repo root)` | `src/Quire.EfimatrixImpl` | differs |
| `Quire.EfiPlot` | Mpl | `unknown` | `src/Quire.EfiPlot` | differs |
| `Quire.EfiplotImpl` | CSharp | `efi/efiplot` | `src/Quire.EfiplotImpl` | differs |
| `Quire.EfiSignal` | Mpl | `unknown` | `src/Quire.EfiSignal` | differs |
| `Quire.EfisignalImpl` | CppCli | `(repo root)` | `src/Quire.EfisignalImpl` | differs |
| `Quire.EfiSixsigma` | Mpl | `unknown` | `src/Quire.EfiSixsigma` | differs |
| `Quire.EfisixsigmaImpl` | CSharp | `efi/efisixsigma` | `src/Quire.EfisixsigmaImpl` | differs |
| `Quire.EfiSolver` | Mpl | `unknown` | `src/Quire.EfiSolver` | differs |
| `Quire.EfisolverImpl` | CppCli | `(repo root)` | `src/Quire.EfisolverImpl` | differs |
| `Quire.EfiSpecial` | Mpl | `unknown` | `src/Quire.EfiSpecial` | differs |
| `Quire.EfispecialImpl` | CppCli | `(repo root)` | `src/Quire.EfispecialImpl` | differs |
| `Quire.EfiStatistics` | Mpl | `unknown` | `src/Quire.EfiStatistics` | differs |
| `Quire.EfistatisticsImpl` | CppCli | `(repo root)` | `src/Quire.EfistatisticsImpl` | differs |
| `Quire.EfiSymbolics` | Mpl | `unknown` | `src/Quire.EfiSymbolics` | differs |
| `Quire.EfisymbolicsImpl` | CppCli | `(repo root)` | `src/Quire.EfisymbolicsImpl` | differs |
| `Quire.EfiSymbolicsEmitter` | Mpl | `unknown` | `src/Quire.EfiSymbolicsEmitter` | differs |
| `Quire.EfiSymbolicsLabels` | Mpl | `unknown` | `src/Quire.EfiSymbolicsLabels` | differs |
| `Quire.EfiSymbolicsLexer` | Mpl | `unknown` | `src/Quire.EfiSymbolicsLexer` | differs |
| `Quire.EfiSymbolicsMacros` | Mpl | `unknown` | `src/Quire.EfiSymbolicsMacros` | differs |
| `Quire.EfiSymbolicsMap` | Mpl | `unknown` | `src/Quire.EfiSymbolicsMap` | differs |
| `Quire.EfiSymbolicsParser` | Mpl | `unknown` | `src/Quire.EfiSymbolicsParser` | differs |
| `Quire.Efiutl` | CppCli | `(repo root)` | `src/Quire.Efiutl` | differs |
| `Quire.EfiWavelets` | Mpl | `unknown` | `src/Quire.EfiWavelets` | differs |
| `Quire.EfiwaveletsImpl` | CppCli | `efi/efiwavelets` | `src/Quire.EfiwaveletsImpl` | differs |
| `Quire.EngutilsMgd` | CppCli | `eng/engutils-mgd` | `src/Quire.EngutilsMgd` | differs |
| `Quire.Hashtable` | Mpl | `unknown` | `src/Quire.Hashtable` | differs |
| `Quire.Engine` | CppCli | `eng/iengine` | `src/Quire.Engine` | differs |
| `Quire.Interop.Mathcad` | CSharp | `unknown` | `src/Quire.Interop.Mathcad` | differs |
| `Quire.AutomationPrivate` | CSharp | `unknown` | `src/Quire.AutomationPrivate` | differs |
| `Quire.Interop.MathcadOld` | CSharp | `unknown` | `src/Quire.Interop.MathcadOld` | differs |
| `Quire.Interop.VSInterfacesLib` | CSharp | `unknown` | `src/Quire.Interop.VSInterfacesLib` | differs |
| `Quire` | CSharp | `Spirit` | `src/Quire` | differs |
| `Quire.Mccore12` | CppCli | `eng/mccore12` | `src/Quire.Mccore12` | differs |
| `Quire.McdDynamicEfi` | Mpl | `unknown` | `src/Quire.McdDynamicEfi` | differs |
| `Quire.McdEfi` | Mpl | `unknown` | `src/Quire.McdEfi` | differs |
| `Quire.Mcdepman` | CppCli | `eng/mcdepman` | `src/Quire.Mcdepman` | differs |
| `Quire.McdRun` | Mpl | `unknown` | `src/Quire.McdRun` | differs |
| `Quire.McdStaticUnitSystem` | Mpl | `unknown` | `src/Quire.McdStaticUnitSystem` | differs |
| `Quire.McdStaticUnitSystemCGS` | Mpl | `unknown` | `src/Quire.McdStaticUnitSystemCGS` | differs |
| `Quire.McdStaticUnitSystemMKS` | Mpl | `unknown` | `src/Quire.McdStaticUnitSystemMKS` | differs |
| `Quire.McdStaticUnitSystemNone` | Mpl | `unknown` | `src/Quire.McdStaticUnitSystemNone` | differs |
| `Quire.McdStaticUnitSystemSI` | Mpl | `unknown` | `src/Quire.McdStaticUnitSystemSI` | differs |
| `Quire.McdStaticUnitSystemUS` | Mpl | `unknown` | `src/Quire.McdStaticUnitSystemUS` | differs |
| `Quire.McdTranslator` | Mpl | `unknown` | `src/Quire.McdTranslator` | differs |
| `Quire.McdUnitSystemCGS` | Mpl | `unknown` | `src/Quire.McdUnitSystemCGS` | differs |
| `Quire.McdUnitSystemMKS` | Mpl | `unknown` | `src/Quire.McdUnitSystemMKS` | differs |
| `Quire.McdUnitSystemNone` | Mpl | `unknown` | `src/Quire.McdUnitSystemNone` | differs |
| `Quire.McdUnitSystemSI` | Mpl | `unknown` | `src/Quire.McdUnitSystemSI` | differs |
| `Quire.McdUnitSystemUS` | Mpl | `unknown` | `src/Quire.McdUnitSystemUS` | differs |
| `Quire.Mceng` | CppCli | `eng/mceng` | `src/Quire.Mceng` | differs |
| `Quire.Mcfront12` | CppCli | `eng/mcfront12` | `src/Quire.Mcfront12` | differs |
| `Quire.Engine.Misc` | CSharp | `utl/mcmisc` | `src/Quire.Engine.Misc` | differs |
| `Quire.MplModule` | Mpl | `unknown` | `src/Quire.MplModule` | differs |
| `Quire.Mpl.Emit` | CSharp | `mpl/mplasb` | `src/Quire.Mpl.Emit` | differs |
| `Quire.MplConstants` | Mpl | `unknown` | `src/Quire.MplConstants` | differs |
| `Quire.MplMacro` | Mpl | `unknown` | `src/Quire.MplMacro` | differs |
| `Quire.MplPrelude` | Mpl | `unknown` | `src/Quire.MplPrelude` | differs |
| `Quire.MplPrimitives` | Mpl | `unknown` | `src/Quire.MplPrimitives` | differs |
| `Quire.Mpl` | CSharp | `mpl/mplrun` | `src/Quire.Mpl` | differs |
| `Quire.MplSexpr` | Mpl | `unknown` | `src/Quire.MplSexpr` | differs |
| `Quire.MplUnitSystemSI` | Mpl | `unknown` | `src/Quire.MplUnitSystemSI` | differs |
| `Quire.Msclr` | CppCli | `mpl/msclr` | `src/Quire.Msclr` | differs |
| `Quire.Msgen` | CppCli | `mpl/msgen` | `src/Quire.Msgen` | differs |
| `Quire.Mslisp` | CppCli | `eng/lisp/mslisp` | `src/Quire.Mslisp` | differs |
| `Quire.MspconsMgd` | CppCli | `eng/mspcons-mgd` | `src/Quire.MspconsMgd` | differs |
| `Quire.Calculation` | CSharp | `Ptc.Calculation` | `src/Quire.Calculation` | already at target |
| `Quire.Calculation.Core` | CSharp | `Ptc.Calculation.Core` | `src/Quire.Calculation.Core` | already at target |
| `Quire.Controls` | CSharp | `Ptc.Controls` | `src/Quire.Controls` | already at target |
| `Quire.Controls.CollapsibleArea` | CSharp | `Ptc.Controls.CollapsibleArea` | `src/Quire.Controls.CollapsibleArea` | already at target |
| `Quire.Controls.Core` | CSharp | `Ptc.Controls.Core` | `src/Quire.Controls.Core` | already at target |
| `Quire.Controls.EquationControl` | CSharp | `Ptc.Controls.EquationControl` | `src/Quire.Controls.EquationControl` | already at target |
| `Quire.Controls.ExcelComponent` | CSharp | `Ptc.Controls.ExcelComponent` | `src/Quire.Controls.ExcelComponent` | already at target |
| `Quire.Controls.Include` | CSharp | `Ptc.Controls.Include` | `src/Quire.Controls.Include` | already at target |
| `Quire.Controls.Plots` | CSharp | `Ptc.Controls.Plots` | `src/Quire.Controls.Plots` | already at target |
| `Quire.Controls.SolveBlock` | CSharp | `Ptc.Controls.SolveBlock` | `src/Quire.Controls.SolveBlock` | already at target |
| `Quire.Controls.Text` | CSharp | `Ptc.Controls.Text` | `src/Quire.Controls.Text` | already at target |
| `Quire.Controls.Whiteboard` | CSharp | `Ptc.Controls.Whiteboard` | `src/Quire.Controls.Whiteboard` | already at target |
| `Quire.Controls.Worksheet` | CSharp | `Ptc.Controls.Worksheet` | `src/Quire.Controls.Worksheet` | already at target |
| `Quire.Core` | CSharp | `Ptc.Core` | `src/Quire.Core` | already at target |
| `Quire.EngNotebook.Automation` | CSharp | `Ptc.COM.EngNotebook.Automation` | `src/Quire.EngNotebook.Automation` | already at target |
| `Quire.EngNotebookPlus.Automation` | CSharp | `Ptc.COM.EngNotebookPlus.Automation` | `src/Quire.EngNotebookPlus.Automation` | already at target |
| `Quire.EquationEditor` | CSharp | `unknown` | `src/Quire.EquationEditor` | differs |
| `Quire.Automation` | CSharp | `Ptc.COM.Automation` | `src/Quire.Automation` | already at target |
| `Quire.Media3D` | CSharp | `Ptc.Media3D` | `src/Quire.Media3D` | already at target |
| `Quire.Utils` | CSharp | `mcd/sources/utilsmgd` | `src/Quire.Utils` | differs |
| `Quire.Engine.Utl` | CppCli | `utl/utl-mgd` | `src/Quire.Engine.Utl` | differs |
| `Quire.Native.Creo3dfit` | Native | `unknown` | `src/Quire.Native.Creo3dfit` | differs |
| `Quire.Native.EfistatsciImpl` | Native | `efi/efistatsci` | `src/Quire.Native.EfistatsciImpl` | differs |
| `Quire.Native.EfisymbolicsUmgd` | Native | `efi/efisymbolics-umgd` | `src/Quire.Native.EfisymbolicsUmgd` | differs |
| `Quire.Native.Engutils` | Native | `eng/engutils` | `src/Quire.Native.Engutils` | differs |
| `Quire.Native.Mcaduser` | Native | `eng/mcaduser` | `src/Quire.Native.Mcaduser` | differs |
| `Quire.Native.Mctranslator` | Native | `eng/mctranslator` | `src/Quire.Native.Mctranslator` | differs |
| `Quire.Native.Mshex` | Native | `mpl/mshex` | `src/Quire.Native.Mshex` | differs |
| `Quire.Native.Msmpl` | Native | `mpl/msmpl` | `src/Quire.Native.Msmpl` | differs |
| `Quire.Native.Mspcons` | Native | `eng/mspcons` | `src/Quire.Native.Mspcons` | differs |
| `Quire.Native.Msprolog` | Native | `eng/prolog/msprolog` | `src/Quire.Native.Msprolog` | differs |
| `Quire.Native.Utl` | Native | `utl/utl` | `src/Quire.Native.Utl` | differs |
