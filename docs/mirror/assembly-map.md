# Assembly dependency map

Generated from installed Mathcad assembly metadata, PE imports and evaluated Release project references.
Snapshot: 2026-11-27T18:54:17+00:00; revision `402088638f7a`.

This is a planning aid for step 1 of the [five-step workflow](PLAN.md). It adds no validation step.

Each Mathcad assembly has one Quire target project in its original language ([D20](decisions.md)).
An arrow **A → B** means B depends on A: complete A before starting B. Frameworks and packages are
treated as available. Native DLLs are mirrored as native projects and share the completion order: a
stopgap import library does not make one available. They are outside the IL gate (D10). Mpl modules
also depend on the Mpl compiler
(`mplasb`, `mplrun`, `msgen`, `msclr`, and native ports of `msmpl` and `mshex`).

“Ready” concerns dependencies, not ownership. Check [WORK](WORK.md) before claiming files.

## Ready now

| Complete next | Language | Newly eligible assemblies |
|---|---|---|
| `Quire.Automation` | CSharp | None yet; other prerequisites remain |
| `Quire.AutomationPrivate` | CSharp | None yet; other prerequisites remain |
| `Quire.Core` | CSharp | `Quire.Calculation.Core`, `Quire.DevComponents.WpfRibbon`, `Quire.Media3D` |
| `Quire.DevComponents.WpfDock` | CSharp | None yet; other prerequisites remain |
| `Quire.EngNotebook.Automation` | CSharp | None yet; other prerequisites remain |
| `Quire.EngNotebookPlus.Automation` | CSharp | None yet; other prerequisites remain |
| `Quire.Engine.Misc` | CSharp | None yet; other prerequisites remain |
| `Quire.Interop.Mathcad` | CSharp | None yet; other prerequisites remain |
| `Quire.Interop.MathcadOld` | CSharp | None yet; other prerequisites remain |
| `Quire.Interop.VSInterfacesLib` | CSharp | None yet; other prerequisites remain |
| `Quire.Mpl.Emit` | CSharp | `Quire.Mpl` |
| `Quire.Native.Creo3dfit` | Native | None yet; other prerequisites remain |
| `Quire.Native.Mshex` | Native | `Quire.Native.Engutils` |
| `Quire.Native.Mspcons` | Native | None yet; other prerequisites remain |
| `Quire.Native.Utl` | Native | `Quire.Engine.Utl`, `Quire.Native.EfisymbolicsUmgd`, `Quire.Native.Mcaduser` |
| `Quire.Utils` | CSharp | None yet; other prerequisites remain |

## Completion order

Assemblies in a row can be worked on independently after every earlier dependency is complete.
The rows are dependency order, not a claim that any assembly is already complete.

| Wave | Assemblies |
|---|---|
| 1 | `Quire.Automation`, `Quire.AutomationPrivate`, `Quire.Core`, `Quire.DevComponents.WpfDock`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`, `Quire.Engine.Misc`, `Quire.Interop.Mathcad`, `Quire.Interop.MathcadOld`, `Quire.Interop.VSInterfacesLib`, `Quire.Mpl.Emit`, `Quire.Native.Creo3dfit` (native), `Quire.Native.Mshex` (native), `Quire.Native.Mspcons` (native), `Quire.Native.Utl` (native), `Quire.Utils` |
| 2 | `Quire.Calculation.Core`, `Quire.DevComponents.WpfRibbon`, `Quire.Engine.Utl`, `Quire.Media3D`, `Quire.Mpl`, `Quire.Native.EfisymbolicsUmgd` (native), `Quire.Native.Engutils` (native), `Quire.Native.Mcaduser` (native), `Quire.Native.Msprolog` (native) |
| 3 | `Quire.Controls.Core`, `Quire.Msclr`, `Quire.Msgen` |
| 4 | `Quire.EquationEditor`, `Quire.Native.Msmpl` (native) |
| 5 | `Quire.Controls.EquationControl`, `Quire.EfiAffine`, `Quire.EfiBessel`, `Quire.EfiData`, `Quire.EfiDebug`, `Quire.EfiFinance`, `Quire.EfiFourier`, `Quire.EfiIO`, `Quire.EfiImage`, `Quire.EfiSignal`, `Quire.EfiSixsigma`, `Quire.EfiSpecial`, `Quire.EfiStatistics`, `Quire.EfiWavelets`, `Quire.McdEfi`, `Quire.MplConstants`, `Quire.MplModule`, `Quire.MplPrelude`, `Quire.MplPrimitives`, `Quire.MplSexpr` |
| 6 | `Quire.Controls.Include`, `Quire.Controls.Text`, `Quire.Efiutl`, `Quire.Hashtable`, `Quire.McdRun`, `Quire.McdStaticUnitSystem`, `Quire.MplMacro`, `Quire.MplUnitSystemSI` |
| 7 | `Quire.Controls.Plots`, `Quire.Controls.Whiteboard`, `Quire.EfibesselImpl`, `Quire.EficoreImpl`, `Quire.EfidebugImpl`, `Quire.EfigraphicsImpl`, `Quire.EfispecialImpl`, `Quire.EfisymbolicsImpl`, `Quire.McdStaticUnitSystemCGS`, `Quire.McdStaticUnitSystemNone`, `Quire.McdStaticUnitSystemSI`, `Quire.Native.EfistatsciImpl` (native), `Quire.Native.Mctranslator` (native) |
| 8 | `Quire.Controls.CollapsibleArea`, `Quire.Controls.SolveBlock`, `Quire.EfiCore`, `Quire.EfiGraphics`, `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsMacros`, `Quire.EfiioCsharp`, `Quire.EfimatrixImpl`, `Quire.McdStaticUnitSystemMKS`, `Quire.McdStaticUnitSystemUS`, `Quire.McdUnitSystemCGS`, `Quire.McdUnitSystemNone` |
| 9 | `Quire.EfiMatrix`, `Quire.EfiSymbolicsMap`, `Quire.EficalculusImpl`, `Quire.EfidiffeqImpl`, `Quire.EfifourierImpl`, `Quire.EfiioImpl`, `Quire.McdUnitSystemSI` |
| 10 | `Quire.EfiCalculus`, `Quire.EfiDiffEQ`, `Quire.EfiSymbolicsLexer`, `Quire.EfiimageImpl`, `Quire.EfiplotImpl`, `Quire.EfisignalImpl`, `Quire.EfistatisticsImpl`, `Quire.EfiwaveletsImpl`, `Quire.McdUnitSystemMKS`, `Quire.McdUnitSystemUS` |
| 11 | `Quire.EfiPlot`, `Quire.EfiSymbolicsParser`, `Quire.EfifittingImpl` |
| 12 | `Quire.EfiFitting`, `Quire.EfiSymbolicsEmitter`, `Quire.EfidataImpl`, `Quire.EfifinanceImpl`, `Quire.EfisolverImpl` |
| 13 | `Quire.Automationmgd`, `Quire.EfiSolver`, `Quire.EfiSymbolics`, `Quire.EfisixsigmaImpl` |
| 14 | `Quire.Automation.Native`, `Quire.McdDynamicEfi` |
| 15 | `Quire.Componentsmgd`, `Quire.EngutilsMgd` |
| 16 | `Quire.Engine`, `Quire.McdTranslator`, `Quire.MspconsMgd` |
| 17 | `Quire.Calculation`, `Quire.Mcdepman`, `Quire.Mslisp` |
| 18 | `Quire.Controls.ExcelComponent`, `Quire.Mccore12` |
| 19 | `Quire.Controls.Worksheet`, `Quire.Mcfront12` |
| 20 | `Quire.Controls`, `Quire.Mceng` |
| 21 | `Quire` |

## Hosts and support projects

A D2 host still holds assemblies whose target project does not exist yet in the original language.
Splitting one out means creating its target and moving its members in the same change.

| Project | Role | Mathcad assemblies it still hosts |
|---|---|---|
| `Quire.Automation.Native` | target and D2 host | `automationmgd` |
| `Quire.Efi` | D2 host | `EfiAffine`, `EfiBessel`, `EfiCalculus`, `EfiCore`, `EfiData`, `EfiDebug`, `EfiDiffEQ`, `EfiFinance`, `EfiFitting`, `EfiFourier`, `EfiGraphics`, `EfiIO`, `EfiImage`, `EfiMatrix`, `EfiPlot`, `EfiSignal`, `EfiSixsigma`, `EfiSolver`, `EfiSpecial`, `EfiStatistics`, `EfiSymbolics`, `EfiSymbolicsEmitter`, `EfiSymbolicsLabels`, `EfiSymbolicsLexer`, `EfiSymbolicsMacros`, `EfiSymbolicsMap`, `EfiSymbolicsParser`, `EfiWavelets`, `efibessel_`, `eficalculus_`, `eficore_`, `efidata_`, `efidebug_`, `efidiffeq_`, `efifinance_`, `efifitting_`, `efifourier_`, `efigraphics_`, `efiimage_`, `efiio-csharp`, `efiio_`, `efimatrix_`, `efiplot_`, `efisignal_`, `efisixsigma_`, `efisolver_`, `efispecial_`, `efistatistics_`, `efisymbolics_`, `efiutl`, `efiwavelets_` |
| `Quire.Mcd` | D2 host | `McdDynamicEfi`, `McdEfi`, `McdTranslator`, `McdUnitSystemCGS`, `McdUnitSystemMKS`, `McdUnitSystemNone`, `McdUnitSystemSI`, `McdUnitSystemUS` |
| `Quire.Mpl.Modules` | D2 host | `Hashtable`, `McdRun`, `McdStaticUnitSystem`, `McdStaticUnitSystemCGS`, `McdStaticUnitSystemMKS`, `McdStaticUnitSystemNone`, `McdStaticUnitSystemSI`, `McdStaticUnitSystemUS`, `Mpl`, `MplConstants`, `MplMacro`, `MplPrelude`, `MplPrimitives`, `MplSexpr`, `MplUnitSystemSI` |
| `Quire.Engine.Native` | support (no Mathcad counterpart) | None |
| `Quire.Mirror` | support (no Mathcad counterpart) | None |
| `Quire.Mpl.Runtime` | support (no Mathcad counterpart) | None |

## Native projects

| Target | Original | Imports from Mathcad DLLs | Imported by |
|---|---|---|---|
| `Quire.Native.Creo3dfit` | `creo3dfit.dll` | None | `Quire.EfigraphicsImpl` |
| `Quire.Native.EfistatsciImpl` | `efistatsci_.dll` | `Quire.Efiutl` | `Quire.EfiimageImpl`, `Quire.EfiioImpl`, `Quire.EfisignalImpl`, `Quire.EfistatisticsImpl` |
| `Quire.Native.EfisymbolicsUmgd` | `efisymbolics-umgd.dll` | `Quire.Native.Utl` | None |
| `Quire.Native.Engutils` | `engutils.dll` | `Quire.Native.Mshex` | `Quire.EngutilsMgd`, `Quire.Mccore12`, `Quire.Native.Mctranslator` |
| `Quire.Native.Mcaduser` | `mcaduser.dll` | `Quire.Native.Utl` | `Quire.Mccore12` |
| `Quire.Native.Mctranslator` | `mctranslator.dll` | `Quire.Efiutl`, `Quire.Native.Engutils`, `Quire.Native.Mshex`, `Quire.Native.Msmpl`, `Quire.Native.Mspcons`, `Quire.Native.Msprolog`, `Quire.Native.Utl` | `Quire.Mccore12` |
| `Quire.Native.Mshex` | `mshex.dll` | None | `Quire.EfiAffine`, `Quire.EfiBessel`, `Quire.EfiCalculus`, `Quire.EfiCore`, `Quire.EfiData`, `Quire.EfiDebug`, `Quire.EfiDiffEQ`, `Quire.EfiFinance`, `Quire.EfiFitting`, `Quire.EfiFourier`, `Quire.EfiGraphics`, `Quire.EfiIO`, `Quire.EfiImage`, `Quire.EfiMatrix`, `Quire.EfiPlot`, `Quire.EfiSignal`, `Quire.EfiSixsigma`, `Quire.EfiSolver`, `Quire.EfiSpecial`, `Quire.EfiStatistics`, `Quire.EfiSymbolics`, `Quire.EfiSymbolicsEmitter`, `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsLexer`, `Quire.EfiSymbolicsMacros`, `Quire.EfiSymbolicsMap`, `Quire.EfiSymbolicsParser`, `Quire.EfiWavelets`, `Quire.Hashtable`, `Quire.Mccore12`, `Quire.McdDynamicEfi`, `Quire.McdEfi`, `Quire.McdRun`, `Quire.McdStaticUnitSystem`, `Quire.McdStaticUnitSystemCGS`, `Quire.McdStaticUnitSystemMKS`, `Quire.McdStaticUnitSystemNone`, `Quire.McdStaticUnitSystemSI`, `Quire.McdStaticUnitSystemUS`, `Quire.McdTranslator`, `Quire.McdUnitSystemCGS`, `Quire.McdUnitSystemMKS`, `Quire.McdUnitSystemNone`, `Quire.McdUnitSystemSI`, `Quire.McdUnitSystemUS`, `Quire.MplConstants`, `Quire.MplMacro`, `Quire.MplModule`, `Quire.MplPrelude`, `Quire.MplPrimitives`, `Quire.MplSexpr`, `Quire.MplUnitSystemSI`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Engutils`, `Quire.Native.Mctranslator`, `Quire.Native.Msmpl` |
| `Quire.Native.Msmpl` | `msmpl.dll` | `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Utl` | `Quire.EfiAffine`, `Quire.EfiBessel`, `Quire.EfiCalculus`, `Quire.EfiCore`, `Quire.EfiData`, `Quire.EfiDebug`, `Quire.EfiDiffEQ`, `Quire.EfiFinance`, `Quire.EfiFitting`, `Quire.EfiFourier`, `Quire.EfiGraphics`, `Quire.EfiIO`, `Quire.EfiImage`, `Quire.EfiMatrix`, `Quire.EfiPlot`, `Quire.EfiSignal`, `Quire.EfiSixsigma`, `Quire.EfiSolver`, `Quire.EfiSpecial`, `Quire.EfiStatistics`, `Quire.EfiSymbolics`, `Quire.EfiSymbolicsEmitter`, `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsLexer`, `Quire.EfiSymbolicsMacros`, `Quire.EfiSymbolicsMap`, `Quire.EfiSymbolicsParser`, `Quire.EfiWavelets`, `Quire.EngutilsMgd`, `Quire.Hashtable`, `Quire.Mccore12`, `Quire.McdDynamicEfi`, `Quire.McdEfi`, `Quire.McdRun`, `Quire.McdStaticUnitSystem`, `Quire.McdStaticUnitSystemCGS`, `Quire.McdStaticUnitSystemMKS`, `Quire.McdStaticUnitSystemNone`, `Quire.McdStaticUnitSystemSI`, `Quire.McdStaticUnitSystemUS`, `Quire.McdTranslator`, `Quire.McdUnitSystemCGS`, `Quire.McdUnitSystemMKS`, `Quire.McdUnitSystemNone`, `Quire.McdUnitSystemSI`, `Quire.McdUnitSystemUS`, `Quire.MplConstants`, `Quire.MplMacro`, `Quire.MplModule`, `Quire.MplPrelude`, `Quire.MplPrimitives`, `Quire.MplSexpr`, `Quire.MplUnitSystemSI`, `Quire.Native.Mctranslator` |
| `Quire.Native.Mspcons` | `mspcons.dll` | None | `Quire.Mcfront12`, `Quire.Native.Mctranslator`, `Quire.Native.Msprolog` |
| `Quire.Native.Msprolog` | `msprolog.dll` | `Quire.Native.Mspcons`, `Quire.Native.Utl` | `Quire.Mcfront12`, `Quire.Native.Mctranslator` |
| `Quire.Native.Utl` | `utl.dll` | None | `Quire.EfimatrixImpl`, `Quire.EfisymbolicsImpl`, `Quire.Efiutl`, `Quire.Engine`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mccore12`, `Quire.Mcdepman`, `Quire.Mceng`, `Quire.Mcfront12`, `Quire.Mslisp`, `Quire.Native.EfisymbolicsUmgd`, `Quire.Native.Mcaduser`, `Quire.Native.Mctranslator`, `Quire.Native.Msmpl`, `Quire.Native.Msprolog` |

## Dependency graph

Solid arrows are references of split projects. Dashed arrows come from the original Mathcad references
and native imports. Transitive references are omitted.

```mermaid
flowchart TD
    a0["Quire.Automation.Native (CSharp)"]
    a1["Quire.Automationmgd (CSharp)"]
    a2["Quire.Componentsmgd (CSharp)"]
    a3["Quire.DevComponents.WpfDock (CSharp)"]
    a4["Quire.DevComponents.WpfRibbon (CSharp)"]
    a5["Quire.EfiAffine (Mpl)"]
    a6["Quire.EfiBessel (Mpl)"]
    a7["Quire.EfibesselImpl (CppCli)"]
    a8["Quire.EfiCalculus (Mpl)"]
    a9["Quire.EficalculusImpl (CppCli)"]
    a10["Quire.EfiCore (Mpl)"]
    a11["Quire.EficoreImpl (CppCli)"]
    a12["Quire.EfiData (Mpl)"]
    a13["Quire.EfidataImpl (CppCli)"]
    a14["Quire.EfiDebug (Mpl)"]
    a15["Quire.EfidebugImpl (CppCli)"]
    a16["Quire.EfiDiffEQ (Mpl)"]
    a17["Quire.EfidiffeqImpl (CppCli)"]
    a18["Quire.EfiFinance (Mpl)"]
    a19["Quire.EfifinanceImpl (CppCli)"]
    a20["Quire.EfiFitting (Mpl)"]
    a21["Quire.EfifittingImpl (CppCli)"]
    a22["Quire.EfiFourier (Mpl)"]
    a23["Quire.EfifourierImpl (CppCli)"]
    a24["Quire.EfiGraphics (Mpl)"]
    a25["Quire.EfigraphicsImpl (CppCli)"]
    a26["Quire.EfiImage (Mpl)"]
    a27["Quire.EfiimageImpl (CppCli)"]
    a28["Quire.EfiIO (Mpl)"]
    a29["Quire.EfiioImpl (CppCli)"]
    a30["Quire.EfiioCsharp (CSharp)"]
    a31["Quire.EfiMatrix (Mpl)"]
    a32["Quire.EfimatrixImpl (CppCli)"]
    a33["Quire.EfiPlot (Mpl)"]
    a34["Quire.EfiplotImpl (CSharp)"]
    a35["Quire.EfiSignal (Mpl)"]
    a36["Quire.EfisignalImpl (CppCli)"]
    a37["Quire.EfiSixsigma (Mpl)"]
    a38["Quire.EfisixsigmaImpl (CSharp)"]
    a39["Quire.EfiSolver (Mpl)"]
    a40["Quire.EfisolverImpl (CppCli)"]
    a41["Quire.EfiSpecial (Mpl)"]
    a42["Quire.EfispecialImpl (CppCli)"]
    a43["Quire.EfiStatistics (Mpl)"]
    a44["Quire.EfistatisticsImpl (CppCli)"]
    a45["Quire.EfiSymbolics (Mpl)"]
    a46["Quire.EfisymbolicsImpl (CppCli)"]
    a47["Quire.EfiSymbolicsEmitter (Mpl)"]
    a48["Quire.EfiSymbolicsLabels (Mpl)"]
    a49["Quire.EfiSymbolicsLexer (Mpl)"]
    a50["Quire.EfiSymbolicsMacros (Mpl)"]
    a51["Quire.EfiSymbolicsMap (Mpl)"]
    a52["Quire.EfiSymbolicsParser (Mpl)"]
    a53["Quire.Efiutl (CppCli)"]
    a54["Quire.EfiWavelets (Mpl)"]
    a55["Quire.EfiwaveletsImpl (CppCli)"]
    a56["Quire.EngutilsMgd (CppCli)"]
    a57["Quire.Hashtable (Mpl)"]
    a58["Quire.Engine (CppCli)"]
    a59["Quire.Interop.Mathcad (CSharp)"]
    a60["Quire.AutomationPrivate (CSharp)"]
    a61["Quire.Interop.MathcadOld (CSharp)"]
    a62["Quire.Interop.VSInterfacesLib (CSharp)"]
    a63["Quire (CSharp)"]
    a64["Quire.Mccore12 (CppCli)"]
    a65["Quire.McdDynamicEfi (Mpl)"]
    a66["Quire.McdEfi (Mpl)"]
    a67["Quire.Mcdepman (CppCli)"]
    a68["Quire.McdRun (Mpl)"]
    a69["Quire.McdStaticUnitSystem (Mpl)"]
    a70["Quire.McdStaticUnitSystemCGS (Mpl)"]
    a71["Quire.McdStaticUnitSystemMKS (Mpl)"]
    a72["Quire.McdStaticUnitSystemNone (Mpl)"]
    a73["Quire.McdStaticUnitSystemSI (Mpl)"]
    a74["Quire.McdStaticUnitSystemUS (Mpl)"]
    a75["Quire.McdTranslator (Mpl)"]
    a76["Quire.McdUnitSystemCGS (Mpl)"]
    a77["Quire.McdUnitSystemMKS (Mpl)"]
    a78["Quire.McdUnitSystemNone (Mpl)"]
    a79["Quire.McdUnitSystemSI (Mpl)"]
    a80["Quire.McdUnitSystemUS (Mpl)"]
    a81["Quire.Mceng (CppCli)"]
    a82["Quire.Mcfront12 (CppCli)"]
    a83["Quire.Engine.Misc (CSharp)"]
    a84["Quire.MplModule (Mpl)"]
    a85["Quire.Mpl.Emit (CSharp)"]
    a86["Quire.MplConstants (Mpl)"]
    a87["Quire.MplMacro (Mpl)"]
    a88["Quire.MplPrelude (Mpl)"]
    a89["Quire.MplPrimitives (Mpl)"]
    a90["Quire.Mpl (CSharp)"]
    a91["Quire.MplSexpr (Mpl)"]
    a92["Quire.MplUnitSystemSI (Mpl)"]
    a93["Quire.Msclr (CppCli)"]
    a94["Quire.Msgen (CppCli)"]
    a95["Quire.Mslisp (CppCli)"]
    a96["Quire.MspconsMgd (CppCli)"]
    a97["Quire.Calculation (CSharp)"]
    a98["Quire.Calculation.Core (CSharp)"]
    a99["Quire.Controls (CSharp)"]
    a100["Quire.Controls.CollapsibleArea (CSharp)"]
    a101["Quire.Controls.Core (CSharp)"]
    a102["Quire.Controls.EquationControl (CSharp)"]
    a103["Quire.Controls.ExcelComponent (CSharp)"]
    a104["Quire.Controls.Include (CSharp)"]
    a105["Quire.Controls.Plots (CSharp)"]
    a106["Quire.Controls.SolveBlock (CSharp)"]
    a107["Quire.Controls.Text (CSharp)"]
    a108["Quire.Controls.Whiteboard (CSharp)"]
    a109["Quire.Controls.Worksheet (CSharp)"]
    a110["Quire.Core (CSharp)"]
    a111["Quire.EngNotebook.Automation (CSharp)"]
    a112["Quire.EngNotebookPlus.Automation (CSharp)"]
    a113["Quire.EquationEditor (CSharp)"]
    a114["Quire.Automation (CSharp)"]
    a115["Quire.Media3D (CSharp)"]
    a116["Quire.Utils (CSharp)"]
    a117["Quire.Engine.Utl (CppCli)"]
    a118["Quire.Native.Creo3dfit (Native)"]
    a119["Quire.Native.EfistatsciImpl (Native)"]
    a120["Quire.Native.EfisymbolicsUmgd (Native)"]
    a121["Quire.Native.Engutils (Native)"]
    a122["Quire.Native.Mcaduser (Native)"]
    a123["Quire.Native.Mctranslator (Native)"]
    a124["Quire.Native.Mshex (Native)"]
    a125["Quire.Native.Msmpl (Native)"]
    a126["Quire.Native.Mspcons (Native)"]
    a127["Quire.Native.Msprolog (Native)"]
    a128["Quire.Native.Utl (Native)"]
    a1 -.-> a0
    a59 -.-> a0
    a47 -.-> a1
    a116 -.-> a1
    a0 --> a2
    a62 --> a2
    a110 --> a4
    a125 -.-> a5
    a125 -.-> a6
    a53 -.-> a7
    a9 -.-> a8
    a32 -.-> a9
    a11 -.-> a10
    a57 -.-> a10
    a68 -.-> a10
    a53 -.-> a11
    a125 -.-> a12
    a9 -.-> a13
    a21 -.-> a13
    a44 -.-> a13
    a83 -.-> a13
    a125 -.-> a14
    a53 -.-> a15
    a17 -.-> a16
    a68 -.-> a16
    a32 -.-> a17
    a125 -.-> a18
    a21 -.-> a19
    a21 -.-> a20
    a27 -.-> a21
    a125 -.-> a22
    a32 -.-> a23
    a25 -.-> a24
    a68 -.-> a24
    a53 -.-> a25
    a118 -.-> a25
    a125 -.-> a26
    a23 -.-> a27
    a119 -.-> a27
    a125 -.-> a28
    a30 -.-> a29
    a32 -.-> a29
    a119 -.-> a29
    a11 -.-> a30
    a10 -.-> a31
    a32 -.-> a31
    a11 -.-> a32
    a34 -.-> a33
    a31 -.-> a34
    a9 -.-> a34
    a25 -.-> a34
    a125 -.-> a35
    a23 -.-> a36
    a119 -.-> a36
    a125 -.-> a37
    a13 -.-> a38
    a40 -.-> a39
    a68 -.-> a39
    a9 -.-> a40
    a21 -.-> a40
    a125 -.-> a41
    a53 -.-> a42
    a125 -.-> a43
    a23 -.-> a44
    a119 -.-> a44
    a47 -.-> a45
    a53 -.-> a46
    a117 -.-> a46
    a52 -.-> a47
    a46 -.-> a48
    a68 -.-> a48
    a50 -.-> a49
    a51 -.-> a49
    a46 -.-> a50
    a68 -.-> a50
    a87 -.-> a50
    a48 -.-> a51
    a57 -.-> a51
    a49 -.-> a52
    a32 -.-> a52
    a84 -.-> a53
    a125 -.-> a54
    a23 -.-> a55
    a0 --> a56
    a121 -.-> a56
    a88 -.-> a57
    a56 -.-> a58
    a114 --> a63
    a99 --> a63
    a3 --> a63
    a111 --> a63
    a112 --> a63
    a67 -.-> a64
    a122 -.-> a64
    a123 -.-> a64
    a14 -.-> a65
    a16 -.-> a65
    a24 -.-> a65
    a7 -.-> a65
    a15 -.-> a65
    a19 -.-> a65
    a29 -.-> a65
    a34 -.-> a65
    a36 -.-> a65
    a38 -.-> a65
    a42 -.-> a65
    a55 -.-> a65
    a125 -.-> a66
    a58 --> a67
    a88 -.-> a68
    a88 -.-> a69
    a68 -.-> a70
    a69 -.-> a70
    a73 -.-> a71
    a68 -.-> a72
    a68 -.-> a73
    a69 -.-> a73
    a73 -.-> a74
    a2 -.-> a75
    a45 -.-> a75
    a34 -.-> a75
    a70 -.-> a76
    a71 -.-> a77
    a79 -.-> a77
    a72 -.-> a78
    a32 -.-> a79
    a73 -.-> a79
    a74 -.-> a80
    a79 -.-> a80
    a82 -.-> a81
    a60 -.-> a82
    a83 -.-> a82
    a64 -.-> a82
    a95 --> a82
    a125 -.-> a84
    a125 -.-> a86
    a88 -.-> a87
    a125 -.-> a88
    a125 -.-> a89
    a85 --> a90
    a125 -.-> a91
    a86 -.-> a92
    a88 -.-> a92
    a90 --> a93
    a124 -.-> a93
    a90 --> a94
    a124 -.-> a94
    a96 --> a95
    a56 --> a96
    a98 --> a97
    a58 --> a97
    a110 --> a98
    a109 --> a99
    a108 --> a100
    a98 --> a101
    a4 --> a102
    a113 --> a102
    a97 --> a103
    a108 --> a103
    a102 --> a104
    a107 --> a105
    a115 --> a105
    a108 --> a106
    a102 --> a107
    a104 --> a108
    a107 --> a108
    a100 --> a109
    a103 --> a109
    a105 --> a109
    a106 --> a109
    a101 --> a113
    a90 --> a113
    a110 --> a115
    a128 -.-> a117
    a53 -.-> a119
    a128 -.-> a120
    a124 -.-> a121
    a128 -.-> a122
    a53 -.-> a123
    a121 -.-> a123
    a127 -.-> a123
    a93 -.-> a125
    a94 -.-> a125
    a128 -.-> a125
    a126 -.-> a127
    a128 -.-> a127
```

## Status

Only a split project can be complete. Stub counts of an unsplit assembly are its host’s.
IL counts are the last recorded Release audit of that original’s rows.

| Target | Original | Language | Host | Required | Stub bodies | Scope | Status | Last recorded exact IL |
|---|---|---|---|---|---:|---|---|---|
| `Quire.Automation.Native` | `automation.dll` | CSharp | `Quire.Automation.Native` | `Quire.Automationmgd`, `Quire.Interop.Mathcad`, `Quire.Mpl` | 0 (host) | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Automationmgd` | `automationmgd.dll` | CSharp | `Quire.Automation.Native` | `Quire.EfiSymbolicsEmitter`, `Quire.Mpl`, `Quire.Utils` | 0 (host) | active / mixed | unconfirmed | Not recorded |
| `Quire.Componentsmgd` | `componentsmgd.dll` | CSharp | split | `Quire.Automation.Native`, `Quire.Automationmgd`, `Quire.Interop.Mathcad`, `Quire.Interop.VSInterfacesLib`, `Quire.Mpl` | 10 | deferred / out of scope | incomplete | Not recorded |
| `Quire.DevComponents.WpfDock` | `DevComponents.WpfDock.dll` | CSharp | split | None | 593 | deferred / out of scope | incomplete | 28 exact; 14 declarations; 0 native pairs; 731 unresolved |
| `Quire.DevComponents.WpfRibbon` | `DevComponents.WpfRibbon.dll` | CSharp | split | `Quire.Core` | 1916 | deferred / out of scope | incomplete | 56 exact; 15 declarations; 0 native pairs; 1607 unresolved |
| `Quire.EfiAffine` | `efiaffine.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiBessel` | `efibessel.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfibesselImpl` | `efibessel_.dll` | CppCli | `Quire.Efi` | `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiCalculus` | `eficalculus.dll` | Mpl | `Quire.Efi` | `Quire.EficalculusImpl`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EficalculusImpl` | `eficalculus_.dll` | CppCli | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiCore` | `eficore.dll` | Mpl | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.Hashtable`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EficoreImpl` | `eficore_.dll` | CppCli | `Quire.Efi` | `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiData` | `efidata.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfidataImpl` | `efidata_.dll` | CppCli | `Quire.Efi` | `Quire.EficalculusImpl`, `Quire.EfifittingImpl`, `Quire.EfistatisticsImpl`, `Quire.Efiutl`, `Quire.Engine.Misc`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiDebug` | `efidebug.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfidebugImpl` | `efidebug_.dll` | CppCli | `Quire.Efi` | `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiDiffEQ` | `efidiffeq.dll` | Mpl | `Quire.Efi` | `Quire.EfidiffeqImpl`, `Quire.EfimatrixImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfidiffeqImpl` | `efidiffeq_.dll` | CppCli | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiFinance` | `efifinance.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfifinanceImpl` | `efifinance_.dll` | CppCli | `Quire.Efi` | `Quire.EfifittingImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiFitting` | `efifitting.dll` | Mpl | `Quire.Efi` | `Quire.EfifittingImpl`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfifittingImpl` | `efifitting_.dll` | CppCli | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.EfiimageImpl`, `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiFourier` | `efifourier.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfifourierImpl` | `efifourier_.dll` | CppCli | `Quire.Efi` | `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiGraphics` | `efigraphics.dll` | Mpl | `Quire.Efi` | `Quire.EfigraphicsImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfigraphicsImpl` | `efigraphics_.dll` | CppCli | `Quire.Efi` | `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.Creo3dfit` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiImage` | `efiimage.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiimageImpl` | `efiimage_.dll` | CppCli | `Quire.Efi` | `Quire.EfifourierImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.EfistatsciImpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiIO` | `efiio.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiioImpl` | `efiio_.dll` | CppCli | `Quire.Efi` | `Quire.EfiioCsharp`, `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.EfistatsciImpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiioCsharp` | `efiio-csharp.dll` | CSharp | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.Mpl` | 2121 (host) | active / mixed | incomplete | Not recorded |
| `Quire.EfiMatrix` | `efimatrix.dll` | Mpl | `Quire.Efi` | `Quire.EfiCore`, `Quire.EficoreImpl`, `Quire.EfimatrixImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfimatrixImpl` | `efimatrix_.dll` | CppCli | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.Utl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiPlot` | `efiplot.dll` | Mpl | `Quire.Efi` | `Quire.EfiMatrix`, `Quire.EfiplotImpl`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiplotImpl` | `efiplot_.dll` | CSharp | `Quire.Efi` | `Quire.EfiMatrix`, `Quire.EficalculusImpl`, `Quire.EficoreImpl`, `Quire.EfigraphicsImpl`, `Quire.Efiutl`, `Quire.Mpl` | 2121 (host) | active / mixed | incomplete | Not recorded |
| `Quire.EfiSignal` | `efisignal.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfisignalImpl` | `efisignal_.dll` | CppCli | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.EfifourierImpl`, `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.EfistatsciImpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSixsigma` | `efisixsigma.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfisixsigmaImpl` | `efisixsigma_.dll` | CSharp | `Quire.Efi` | `Quire.EfidataImpl`, `Quire.EfifittingImpl`, `Quire.EfimatrixImpl`, `Quire.EfistatisticsImpl`, `Quire.Efiutl`, `Quire.Mpl` | 2121 (host) | active / mixed | incomplete | Not recorded |
| `Quire.EfiSolver` | `efisolver.dll` | Mpl | `Quire.Efi` | `Quire.EficoreImpl`, `Quire.EfisolverImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfisolverImpl` | `efisolver_.dll` | CppCli | `Quire.Efi` | `Quire.EficalculusImpl`, `Quire.EficoreImpl`, `Quire.EfifittingImpl`, `Quire.EfimatrixImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSpecial` | `efispecial.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfispecialImpl` | `efispecial_.dll` | CppCli | `Quire.Efi` | `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiStatistics` | `efistatistics.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfistatisticsImpl` | `efistatistics_.dll` | CppCli | `Quire.Efi` | `Quire.EfifourierImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.EfistatsciImpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolics` | `efisymbolics.dll` | Mpl | `Quire.Efi` | `Quire.EfiSymbolicsEmitter`, `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsLexer`, `Quire.EfiSymbolicsMap`, `Quire.EfiSymbolicsParser`, `Quire.EficoreImpl`, `Quire.EfisymbolicsImpl`, `Quire.Hashtable`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfisymbolicsImpl` | `efisymbolics_.dll` | CppCli | `Quire.Efi` | `Quire.Efiutl`, `Quire.Engine.Utl`, `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.Utl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolicsEmitter` | `efisymbolicsemitter.dll` | Mpl | `Quire.Efi` | `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsLexer`, `Quire.EfiSymbolicsMacros`, `Quire.EfiSymbolicsMap`, `Quire.EfiSymbolicsParser`, `Quire.EfimatrixImpl`, `Quire.EfisymbolicsImpl`, `Quire.Engine.Utl`, `Quire.Hashtable`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolicsLabels` | `efisymbolicslabels.dll` | Mpl | `Quire.Efi` | `Quire.EfisymbolicsImpl`, `Quire.Engine.Utl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolicsLexer` | `efisymbolicslexer.dll` | Mpl | `Quire.Efi` | `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsMacros`, `Quire.EfiSymbolicsMap`, `Quire.EfisymbolicsImpl`, `Quire.Engine.Utl`, `Quire.Hashtable`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolicsMacros` | `efisymbolicsmacros.dll` | Mpl | `Quire.Efi` | `Quire.EfisymbolicsImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplMacro`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolicsMap` | `efisymbolicsmap.dll` | Mpl | `Quire.Efi` | `Quire.EfiSymbolicsLabels`, `Quire.EfisymbolicsImpl`, `Quire.Engine.Utl`, `Quire.Hashtable`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiSymbolicsParser` | `efisymbolicsparser.dll` | Mpl | `Quire.Efi` | `Quire.EfiSymbolicsLabels`, `Quire.EfiSymbolicsLexer`, `Quire.EfiSymbolicsMacros`, `Quire.EfimatrixImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Efiutl` | `efiutl.dll` | CppCli | `Quire.Efi` | `Quire.Mpl`, `Quire.MplModule`, `Quire.Native.Utl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiWavelets` | `efiwavelets.dll` | Mpl | `Quire.Efi` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EfiwaveletsImpl` | `efiwavelets_.dll` | CppCli | `Quire.Efi` | `Quire.EfifourierImpl`, `Quire.Efiutl`, `Quire.Mpl`, `Quire.MplModule` | 2121 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.EngutilsMgd` | `engutils-mgd.dll` | CppCli | split | `Quire.Automation.Native`, `Quire.Automationmgd`, `Quire.Mpl`, `Quire.Native.Engutils`, `Quire.Native.Msmpl`, `Quire.Native.Utl` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Hashtable` | `hashtable.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Engine` | `iengine.dll` | CppCli | `Quire.Engine` | `Quire.Automationmgd`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Interop.Mathcad`, `Quire.Mpl`, `Quire.Native.Utl` | 137 (host) | active / mixed | incomplete | Not recorded |
| `Quire.Interop.Mathcad` | `interop.mathcad.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.AutomationPrivate` | `interop.mathcadautomationprivate.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Interop.MathcadOld` | `interop.mathcadold.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Interop.VSInterfacesLib` | `interop.vsinterfaceslib.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire` | `MathcadPrime.dll` | CSharp | split | `Quire.Automation`, `Quire.Calculation`, `Quire.Calculation.Core`, `Quire.Controls`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.ExcelComponent`, `Quire.Controls.Include`, `Quire.Controls.Plots`, `Quire.Controls.Text`, `Quire.Controls.Worksheet`, `Quire.Core`, `Quire.DevComponents.WpfDock`, `Quire.DevComponents.WpfRibbon`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`, `Quire.EquationEditor` | 766 | deferred / out of scope | incomplete | Not recorded |
| `Quire.Mccore12` | `mccore12.dll` | CppCli | split | `Quire.EfisymbolicsImpl`, `Quire.Engine`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mcdepman`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Native.Engutils`, `Quire.Native.Mcaduser`, `Quire.Native.Mctranslator`, `Quire.Native.Mshex`, `Quire.Native.Msmpl`, `Quire.Native.Utl` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.McdDynamicEfi` | `mcddynamicefi.dll` | Mpl | `Quire.Mcd` | `Quire.EfiCore`, `Quire.EfiDebug`, `Quire.EfiDiffEQ`, `Quire.EfiGraphics`, `Quire.EfiMatrix`, `Quire.EfibesselImpl`, `Quire.EficoreImpl`, `Quire.EfidataImpl`, `Quire.EfidebugImpl`, `Quire.EfidiffeqImpl`, `Quire.EfifinanceImpl`, `Quire.EfifittingImpl`, `Quire.EfifourierImpl`, `Quire.EfigraphicsImpl`, `Quire.EfiimageImpl`, `Quire.EfiioImpl`, `Quire.EfimatrixImpl`, `Quire.EfiplotImpl`, `Quire.EfisignalImpl`, `Quire.EfisixsigmaImpl`, `Quire.EfispecialImpl`, `Quire.EfistatisticsImpl`, `Quire.EfiwaveletsImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdEfi` | `mcdefi.dll` | Mpl | `Quire.Mcd` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Mcdepman` | `mcdepman.dll` | CppCli | split | `Quire.EfisymbolicsImpl`, `Quire.Engine`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mpl`, `Quire.Native.Utl` | 2 | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdRun` | `mcdrun.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdStaticUnitSystem` | `mcdstaticunitsystem.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdStaticUnitSystemCGS` | `mcdstaticunitsystemcgs.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.McdRun`, `Quire.McdStaticUnitSystem`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdStaticUnitSystemMKS` | `mcdstaticunitsystemmks.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.McdRun`, `Quire.McdStaticUnitSystemSI`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdStaticUnitSystemNone` | `mcdstaticunitsystemnone.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdStaticUnitSystemSI` | `mcdstaticunitsystemsi.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.McdRun`, `Quire.McdStaticUnitSystem`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdStaticUnitSystemUS` | `mcdstaticunitsystemus.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.McdRun`, `Quire.McdStaticUnitSystemSI`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdTranslator` | `mcdtranslator.dll` | Mpl | `Quire.Mcd` | `Quire.Automationmgd`, `Quire.Componentsmgd`, `Quire.EfiSymbolics`, `Quire.EfiSymbolicsParser`, `Quire.EficoreImpl`, `Quire.EfigraphicsImpl`, `Quire.EfimatrixImpl`, `Quire.EfiplotImpl`, `Quire.McdRun`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdUnitSystemCGS` | `mcdunitsystemcgs.dll` | Mpl | `Quire.Mcd` | `Quire.McdRun`, `Quire.McdStaticUnitSystemCGS`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdUnitSystemMKS` | `mcdunitsystemmks.dll` | Mpl | `Quire.Mcd` | `Quire.McdRun`, `Quire.McdStaticUnitSystemMKS`, `Quire.McdUnitSystemSI`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdUnitSystemNone` | `mcdunitsystemnone.dll` | Mpl | `Quire.Mcd` | `Quire.McdRun`, `Quire.McdStaticUnitSystemNone`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdUnitSystemSI` | `mcdunitsystemsi.dll` | Mpl | `Quire.Mcd` | `Quire.EficoreImpl`, `Quire.EfimatrixImpl`, `Quire.McdRun`, `Quire.McdStaticUnitSystemSI`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.McdUnitSystemUS` | `mcdunitsystemus.dll` | Mpl | `Quire.Mcd` | `Quire.McdRun`, `Quire.McdStaticUnitSystemUS`, `Quire.McdUnitSystemSI`, `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 504 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Mceng` | `mceng.dll` | CppCli | split | `Quire.Engine`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mccore12`, `Quire.Mcfront12`, `Quire.Mpl`, `Quire.Mslisp`, `Quire.Native.Utl` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Mcfront12` | `mcfront12.dll` | CppCli | split | `Quire.AutomationPrivate`, `Quire.Automationmgd`, `Quire.Engine`, `Quire.Engine.Misc`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Interop.Mathcad`, `Quire.Mccore12`, `Quire.Mcdepman`, `Quire.Mpl`, `Quire.Mslisp`, `Quire.MspconsMgd`, `Quire.Native.Mspcons`, `Quire.Native.Msprolog`, `Quire.Native.Utl` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Engine.Misc` | `mcmisc.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.MplModule` | `mpl.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Mpl.Emit` | `mplasb.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.MplConstants` | `mplconstants.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.MplMacro` | `mplmacro.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.MplPrelude` | `mplprelude.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.MplPrimitives` | `mplprimitives.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Mpl` | `mplrun.dll` | CSharp | split | `Quire.Mpl.Emit` | 0 | active / mixed | unconfirmed | Not recorded |
| `Quire.MplSexpr` | `mplsexpr.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.MplUnitSystemSI` | `mplunitsystemsi.dll` | Mpl | `Quire.Mpl.Modules` | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.MplConstants`, `Quire.MplPrelude`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Msmpl` | 381 (host) | deferred / out of scope | incomplete | Not recorded |
| `Quire.Msclr` | `msclr.dll` | CppCli | split | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Native.Mshex` | 1 | deferred / out of scope | incomplete | Not recorded |
| `Quire.Msgen` | `msgen.dll` | CppCli | split | `Quire.Mpl`, `Quire.Mpl.Emit`, `Quire.Native.Mshex` | 7 | deferred / out of scope | incomplete | Not recorded |
| `Quire.Mslisp` | `mslisp.dll` | CppCli | split | `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.MspconsMgd`, `Quire.Native.Utl` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.MspconsMgd` | `mspcons-mgd.dll` | CppCli | split | `Quire.EngutilsMgd` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Calculation` | `Ptc.Calculation.dll` | CSharp | split | `Quire.Automation.Native`, `Quire.Automationmgd`, `Quire.Calculation.Core`, `Quire.Core`, `Quire.Engine`, `Quire.Mpl` | 23 | active / mixed | incomplete | Not recorded |
| `Quire.Calculation.Core` | `Ptc.Calculation.Core.dll` | CSharp | split | `Quire.Core` | 0 | active / mixed | unconfirmed | Not recorded |
| `Quire.Controls` | `Ptc.Controls.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.ExcelComponent`, `Quire.Controls.Include`, `Quire.Controls.Plots`, `Quire.Controls.Worksheet`, `Quire.Core`, `Quire.DevComponents.WpfRibbon`, `Quire.EquationEditor` | 4 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.CollapsibleArea` | `Ptc.Controls.CollapsibleArea.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.Whiteboard`, `Quire.Core` | 3 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.Core` | `Ptc.Controls.Core.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Core` | 3 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.EquationControl` | `Ptc.Controls.EquationControl.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Core`, `Quire.DevComponents.WpfRibbon`, `Quire.EquationEditor` | 720 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.ExcelComponent` | `Ptc.Controls.ExcelComponent.dll` | CSharp | split | `Quire.Automation.Native`, `Quire.Automationmgd`, `Quire.Calculation`, `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.Whiteboard`, `Quire.Core` | 69 | deferred / out of scope | incomplete | Not recorded |
| `Quire.Controls.Include` | `Ptc.Controls.Include.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Core` | 0 | active / mixed | unconfirmed | Not recorded |
| `Quire.Controls.Plots` | `Ptc.Controls.Plots.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.Text`, `Quire.Core`, `Quire.DevComponents.WpfRibbon`, `Quire.EquationEditor`, `Quire.Media3D` | 290 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.SolveBlock` | `Ptc.Controls.SolveBlock.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.Whiteboard`, `Quire.Core`, `Quire.EquationEditor` | 3 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.Text` | `Ptc.Controls.Text.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Core` | 29 | active / mixed | incomplete | Not recorded |
| `Quire.Controls.Whiteboard` | `Ptc.Controls.Whiteboard.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.Include`, `Quire.Controls.Text`, `Quire.Core` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Controls.Worksheet` | `Ptc.Controls.Worksheet.dll` | CSharp | split | `Quire.Calculation`, `Quire.Calculation.Core`, `Quire.Controls.CollapsibleArea`, `Quire.Controls.Core`, `Quire.Controls.EquationControl`, `Quire.Controls.ExcelComponent`, `Quire.Controls.Include`, `Quire.Controls.Plots`, `Quire.Controls.SolveBlock`, `Quire.Controls.Text`, `Quire.Controls.Whiteboard`, `Quire.Core`, `Quire.DevComponents.WpfRibbon`, `Quire.Engine`, `Quire.EquationEditor` | 395 | active / mixed | incomplete | Not recorded |
| `Quire.Core` | `Ptc.Core.dll` | CSharp | split | None | 0 | active / mixed | unconfirmed | Not recorded |
| `Quire.EngNotebook.Automation` | `Ptc.EngNotebook.Automation.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.EngNotebookPlus.Automation` | `Ptc.EngNotebookPlus.Automation.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.EquationEditor` | `Ptc.EquationEditor.dll` | CSharp | split | `Quire.Calculation.Core`, `Quire.Controls.Core`, `Quire.Core`, `Quire.Mpl` | 2052 | active / mixed | incomplete | Not recorded |
| `Quire.Automation` | `Ptc.MathcadPrime.Automation.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Media3D` | `Ptc.Media3D.dll` | CSharp | split | `Quire.Core` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Utils` | `utilsmgd.dll` | CSharp | split | None | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Engine.Utl` | `utl-mgd.dll` | CppCli | split | `Quire.Native.Utl` | 0 | deferred / out of scope | unconfirmed | Not recorded |
| `Quire.Native.Creo3dfit` | `creo3dfit.dll` | Native | native project | None | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.EfistatsciImpl` | `efistatsci_.dll` | Native | native project | `Quire.Efiutl` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.EfisymbolicsUmgd` | `efisymbolics-umgd.dll` | Native | native project | `Quire.Native.Utl` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Engutils` | `engutils.dll` | Native | native project | `Quire.Native.Mshex` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Mcaduser` | `mcaduser.dll` | Native | native project | `Quire.Native.Utl` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Mctranslator` | `mctranslator.dll` | Native | native project | `Quire.Efiutl`, `Quire.Native.Engutils`, `Quire.Native.Mshex`, `Quire.Native.Msmpl`, `Quire.Native.Mspcons`, `Quire.Native.Msprolog`, `Quire.Native.Utl` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Mshex` | `mshex.dll` | Native | native project | None | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Msmpl` | `msmpl.dll` | Native | native project | `Quire.Msclr`, `Quire.Msgen`, `Quire.Native.Mshex`, `Quire.Native.Utl` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Mspcons` | `mspcons.dll` | Native | native project | None | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Msprolog` | `msprolog.dll` | Native | native project | `Quire.Native.Mspcons`, `Quire.Native.Utl` | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |
| `Quire.Native.Utl` | `utl.dll` | Native | native project | None | 0 | outside the IL gate (D10) | unconfirmed | Not recorded |

## Not mirrored

Every other DLL and EXE in the install, with the reason it has no Quire project ([scope](scope.md),
[decisions](decisions.md)). “Used by” lists the units whose original references or imports it.

| File | Kind | Treatment | Used by |
|---|---|---|---|
| `asyncoremt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `axiom/win_nt/x86e_win64/AXIOMSys.exe` | native | dependency: FriCAS for Axiom | None |
| `baselibmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/asyncoremt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/baselibmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/chartappole_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/chartdir70.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/coretools_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/coreutils_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/dlmmgrmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/i18nmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/i18nxmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/ic_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/libgifmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/libjpegmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/libpngmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/libtiffmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/libui_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_chinese_cn_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_chinese_tw_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_french_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_german_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_italian_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_japanese_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_korean_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_portuguese_br_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_russian_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/chartl10n/text/compiled_resource/chartl10n_spanish_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_chinese_cn_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_chinese_tw_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_french_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_german_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_italian_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_japanese_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_korean_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_portuguese_br_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_russian_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/apps/chartapp/text/compiled_resource/chartapp_spanish_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_chinese_cn_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_chinese_tw_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_french_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_german_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_italian_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_japanese_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_korean_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_portuguese_br_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_russian_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/proe/uitools/text/compiled_resource/uitools_spanish_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/resources/text/compiled_resource/pro_default_resources.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/rtlcoremt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/ucore68.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/udata68.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/ui18n68.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/unifilesmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/utfstrmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `chartappole_sh/zlib-ngmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `ClearScript.Core.dll` | managed | dependency: Microsoft.ClearScript | `Quire.Controls.EquationControl` |
| `ClearScript.Windows.Core.dll` | managed | dependency: Microsoft.ClearScript | `Quire.Controls.EquationControl` |
| `ClearScript.Windows.dll` | managed | dependency: Microsoft.ClearScript | `Quire.Controls.EquationControl` |
| `concrt140.dll` | native | not needed: MSVC runtime redistributable | None |
| `coretools_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `coreutils_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `cryptomt.dll` | native | not needed: .NET cryptography replaces OpenSSL | None |
| `dlmmgrmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `DocumentFormat.OpenXml.dll` | managed | dependency: DocumentFormat.OpenXml | `Quire.Controls.Core` |
| `exitlogger_app_sh.dll` | native | deferred: PTC Creo platform (D5) | None |
| `i18nmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `Ijwhost.dll` | native | toolchain: .NET host shim the C++/CLI build ships | `Quire.EfibesselImpl`, `Quire.EficalculusImpl`, `Quire.EficoreImpl`, `Quire.EfidataImpl`, `Quire.EfidebugImpl`, `Quire.EfidiffeqImpl`, `Quire.EfifinanceImpl`, `Quire.EfifittingImpl`, `Quire.EfifourierImpl`, `Quire.EfigraphicsImpl`, `Quire.EfiimageImpl`, `Quire.EfiioImpl`, `Quire.EfimatrixImpl`, `Quire.EfisignalImpl`, `Quire.EfisolverImpl`, `Quire.EfispecialImpl`, `Quire.EfistatisticsImpl`, `Quire.EfisymbolicsImpl`, `Quire.Efiutl`, `Quire.EfiwaveletsImpl`, `Quire.Engine`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mccore12`, `Quire.Mcdepman`, `Quire.Mceng`, `Quire.Mcfront12`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Mslisp`, `Quire.MspconsMgd` |
| `knitro1510.dll` | native | substitute: IPOPT or NLopt for Knitro (D6) | `Quire.EfisolverImpl` |
| `liappcoremt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `libimalloc.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | None |
| `libiomp5md.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | None |
| `LicenseWizard.dll` | managed | deferred: licensing (D5) | None |
| `LicenseWizard.exe` | native | deferred: licensing (D5) | None |
| `licommonmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `limanifestmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `lutilsmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `MathcadPrime.exe` | native | app host: built with the Quire project | None |
| `mfc140u.dll` | native | not needed: MSVC runtime redistributable | `Quire.EfiioImpl` |
| `Microsoft.Extensions.ObjectPool.dll` | managed | dependency: Microsoft.Extensions.ObjectPool | None |
| `mkl_core.2.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | None |
| `mkl_def.2.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | None |
| `mkl_intel_thread.2.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | `Quire.EfidiffeqImpl`, `Quire.EfifourierImpl`, `Quire.EfimatrixImpl` |
| `mkl_vml_cmpt.2.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | None |
| `mkl_vml_def.2.dll` | native | substitute: MathNet.Numerics for Intel oneMKL (D6) | None |
| `msvcp140.dll` | native | not needed: MSVC runtime redistributable | `Quire.EfidataImpl`, `Quire.EfidiffeqImpl`, `Quire.EfigraphicsImpl`, `Quire.EfiimageImpl`, `Quire.EfiioImpl`, `Quire.EfisignalImpl`, `Quire.EfisymbolicsImpl`, `Quire.Efiutl`, `Quire.EfiwaveletsImpl`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mccore12`, `Quire.Mcdepman`, `Quire.Mceng`, `Quire.Mcfront12`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Mslisp`, `Quire.Native.EfisymbolicsUmgd`, `Quire.Native.Engutils`, `Quire.Native.Mcaduser`, `Quire.Native.Mctranslator`, `Quire.Native.Mshex`, `Quire.Native.Msmpl`, `Quire.Native.Mspcons`, `Quire.Native.Msprolog`, `Quire.Native.Utl` |
| `nrtftree-library.dll` | managed | dependency: vendored NRtfTree LGPL compatibility fork | `Quire.Controls.Core`, `Quire.Controls.Worksheet` |
| `prod_dcu.exe` | native | deferred: PTC Creo platform (D5) | None |
| `Ptc.License.Plpf.dll` | managed | deferred: licensing (D5) | `Quire` |
| `Ptc.OleInterop.dll` | managed | deferred (D5) | `Quire.Controls.ExcelComponent` |
| `Python.Runtime.dll` | managed | dependency: pythonnet | `Quire.Controls.EquationControl` |
| `rtlcoremt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `runtimes/win/lib/net10.0/System.Management.dll` | managed | dependency: System.Management | None |
| `srvcmgrmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `System.ServiceModel.Duplex.dll` | managed | dependency: System.ServiceModel (dotnet/wcf) | None |
| `System.ServiceModel.Http.dll` | managed | dependency: System.ServiceModel (dotnet/wcf) | None |
| `System.ServiceModel.Primitives.dll` | managed | dependency: System.ServiceModel (dotnet/wcf) | None |
| `System.ServiceModel.Security.dll` | managed | dependency: System.ServiceModel (dotnet/wcf) | None |
| `System.Windows.Interactivity.dll` | managed | substitute: Microsoft.Xaml.Behaviors.Wpf | `Quire` |
| `telecon_utils.dll` | native | deferred: PTC Creo platform (D5) | None |
| `ucore68.dll` | native | not needed: .NET globalisation replaces ICU | `Quire.Native.Creo3dfit` |
| `udata68.dll` | native | not needed: .NET globalisation replaces ICU | None |
| `utfstrmt.dll` | native | deferred: PTC Creo platform (D5) | None |
| `vcomp140.dll` | native | not needed: MSVC runtime redistributable | `Quire.Native.Creo3dfit` |
| `vcruntime140.dll` | native | not needed: MSVC runtime redistributable | `Quire.EfibesselImpl`, `Quire.EficalculusImpl`, `Quire.EficoreImpl`, `Quire.EfidataImpl`, `Quire.EfidebugImpl`, `Quire.EfidiffeqImpl`, `Quire.EfifinanceImpl`, `Quire.EfifittingImpl`, `Quire.EfifourierImpl`, `Quire.EfigraphicsImpl`, `Quire.EfiimageImpl`, `Quire.EfiioImpl`, `Quire.EfimatrixImpl`, `Quire.EfisignalImpl`, `Quire.EfisolverImpl`, `Quire.EfispecialImpl`, `Quire.EfistatisticsImpl`, `Quire.EfisymbolicsImpl`, `Quire.Efiutl`, `Quire.EfiwaveletsImpl`, `Quire.Engine`, `Quire.Engine.Utl`, `Quire.EngutilsMgd`, `Quire.Mccore12`, `Quire.Mcdepman`, `Quire.Mceng`, `Quire.Mcfront12`, `Quire.Msclr`, `Quire.Msgen`, `Quire.Mslisp`, `Quire.MspconsMgd`, `Quire.Native.EfistatsciImpl`, `Quire.Native.EfisymbolicsUmgd`, `Quire.Native.Engutils`, `Quire.Native.Mcaduser`, `Quire.Native.Mctranslator`, `Quire.Native.Mshex`, `Quire.Native.Msmpl`, `Quire.Native.Mspcons`, `Quire.Native.Msprolog`, `Quire.Native.Utl` |
| `vcruntime140_1.dll` | native | not needed: MSVC runtime redistributable | `Quire.EfidataImpl`, `Quire.EfidiffeqImpl`, `Quire.EfifittingImpl`, `Quire.EfifourierImpl`, `Quire.EfiimageImpl`, `Quire.EfiioImpl`, `Quire.EfimatrixImpl`, `Quire.EfisignalImpl`, `Quire.EfispecialImpl`, `Quire.EfistatisticsImpl`, `Quire.EfisymbolicsImpl`, `Quire.Efiutl`, `Quire.EfiwaveletsImpl`, `Quire.Mccore12`, `Quire.Mcfront12`, `Quire.Native.EfistatsciImpl`, `Quire.Native.EfisymbolicsUmgd`, `Quire.Native.Engutils`, `Quire.Native.Mcaduser`, `Quire.Native.Mctranslator`, `Quire.Native.Mshex`, `Quire.Native.Msmpl`, `Quire.Native.Mspcons`, `Quire.Native.Msprolog`, `Quire.Native.Utl` |
| `xercescmt.dll` | native | not needed: System.Xml replaces Xerces-C++ | None |

## Satellite resource assemblies

Localised resources ship beside their assembly in one folder per culture. Quire builds its own from
Quire-authored resources ([D21](decisions.md)).

| Assembly | Cultures |
|---|---|
| `LicenseWizard.dll` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Calculation.Core` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.CollapsibleArea` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.Core` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.EquationControl` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.ExcelComponent` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.Include` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.Plots` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.SolveBlock` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.Text` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.Whiteboard` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.Controls.Worksheet` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.DevComponents.WpfDock` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `Quire.DevComponents.WpfRibbon` | de, en-US, es, fr, it, ja, ko, ru, zh-CN, zh-TW |
| `System.Windows.Interactivity.dll` | de, en, es, fr, it, ja, ko, ru, zh-Hans, zh-Hant |

## Regenerate / explore the next unlocks

```powershell
python tools/validation/assembly_map.py
python tools/validation/assembly_map.py --assume-complete Quire.Core
```

The scenario changes the readiness calculation only. It does not grant an IL pass or edit completion records.
Whole-assembly completion requires the assembly’s own project, all mapped original methods represented and
every compiled method matching, with current artifact identities. A source with no stubs is not sufficient.

The companion `assembly-map.json` holds every unit with its original references, native imports, scope
counts, host and project references.
