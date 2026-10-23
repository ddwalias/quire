# Mapping and project layout

## 2. Mapping rules

| Mathcad | Quire | Notes |
|---|---|---|
| `Ptc.X` | `Quire.X` | e.g. `Ptc.Controls.Worksheet.WorksheetModel` → `Quire.Controls.Worksheet.WorksheetModel` |
| `Spirit.X` (the codename inside `MathcadPrime.dll`) | `Quire.X` | The app name change, e.g. `Spirit.MruFiles` → `Quire.MruFiles` |
| `Ptc.MathcadPrime.X`, `Mathcad.X`, `Mathsoft.X`, `MathcadAutomationPrivate.X` | `Quire.X`, `Quire.AutomationPrivate.X` | Also the app (or former company) name; no collisions result |
| `Mpl.Run.X`, `Mpl.X` | `Quire.Mpl.Run.X`, `Quire.Mpl.X` | |
| `Mcd.X` | `Quire.Mcd.X` | **D1 (decided).** Today's `Quire.Values` code moves here in phase 1 |
| `Engine.X` (`iengine`, `mccore12`, `mceng`) | `Quire.Engine.X` | |
| `System.Reflection.Emit.PTC.Mathcad.Mpl.X` (Mathcad's own IL assembly builder in `mplasb.dll`, 48 types) | `System.Reflection.Emit.Quire.Mpl.X` | **D8 (decided):** literal mirror, only `PTC.Mathcad` replaced |
| `Efi.X`, `Hex.X`, `mslisp.X`, `Mathcad.Automation.X`, other roots | `Quire.<root>.X` | |
| Global-namespace types in native wrappers | `Quire.Native.<Assembly>.X` | e.g. `efiio_.dll` → `Quire.Native.EfiioImpl` |
| Nested types (`A+B`) | Nested types | |
| Mathcad assembly | Quire project of the same name | Except the engine assemblies, which are consolidated (**D2**, below) |
| Names C# can't spell (Mpl operators `==.`, primes `abs'`) | Spelled out: `_eq_eq_dot`, `abs_prime` | `[MirrorOf]` keeps the original; see `Identifier.cs` |

**D2 layout, as built.** The consolidated projects follow the layers of Mathcad's own assembly graph, which keeps
them acyclic: `Quire.Mpl.Emit` (mplasb) → `Quire.Mpl` (mplrun) → `Quire.Mpl.Modules` (compiled Mpl modules:
`mpl*`, `hashtable`, `mcdrun`, `mcdstaticunitsystem*`) → `Quire.Engine.Utl` (utl-mgd, C++/CLI per D19) → `Quire.Efi`
(efi*) → `Quire.Automation.Native` (automation, automationmgd, componentsmgd) → `Quire.Engine` (iengine,
engutils-mgd) → `Quire.Mcd` (the other mcd*) → `Quire.Engine.Native` (mccore12, mcfront12, mceng, msclr, msgen,
mslisp, mspcons-mgd). `Quire.Utils` holds utilsmgd and `Quire.Engine.Misc` holds mcmisc.

**Duplicates.** A type compiled into several assemblies from a shared header is defined once per Quire project when
it is internal (as Mathcad has it, one copy per DLL), and once overall when it is public, in the lowest project of the
assemblies that contain it. No separate `Quire.Efi.Common` project turned out to be needed.

**Existing Quire types with no Mathcad counterpart.** `LinearParser`, `Interpreter`, `Numerics`, `Boxes`,
`VisualNodeBuilder`, `PlotGraph` and others become `internal` implementation types under a `…Internal` namespace of
their assembly. They have to exist: Mathcad's equivalents are native or BAML code that isn't visible in metadata.

## 5. Moving the existing code (phase by phase)

| Current Quire type | Mirrored home |
|---|---|
| `Quire.Values.*` (`NumVal`, `MatVal`, `Dim`, `Unit`, `Units`, `UnitSystem`) | Same names under `Quire.Mcd` (D1) |
| `Quire.Mpl.Run.Sexpr`/`SList`/`Sym`/`Atom`, `SexprReader` | `Mpl.Run` types; the reader becomes `Mpl.Run.Io`-shaped |
| `Quire.Engine.Engine`, `IEngine`, `NodeStatus`, `NodeCallback` | `Engine.IEngine`, `Engine.Factory`; `NodeStatus` also exists in `Calculation.Core` |
| `Quire.Engine.Interpreter`, `Builtins`, `Numerics`, `MatrixOps` | `internal`; later split along `efi*` families in phase 10 |
| `Quire.Calculation.*` | `Ptc.Calculation` / `.Core` counterparts (18 types already match) |
| `Quire.PersistentDataObjects.*`, `WorksheetPackage`, `WorksheetConverter` | `Ptc.Controls.Worksheet.*PersistentData`, `Ptc.PersistentDataObjects`, `Ptc.Serialization` |
| `Quire.EquationEditor.Tree.*`, `View.*` | `Ptc.EquationEditor` counterparts; box layout becomes `internal` |
| `Quire.Controls.*` region controls, `WorksheetModel`, `ControlFactory` | Same names (largely already mirrored) |
| `Quire` app shell (`MainWindow`, `Palettes`, …) | `Spirit` counterparts |

The migration table is the original phase routing guide; Phase 1 has already moved the value and S-expression types. Confirm remaining moves against current source. `Rules.cs`, `Mapper.cs`, and `Identifier.cs` define the implemented mappings.
