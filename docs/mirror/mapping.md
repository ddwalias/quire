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
| Mathcad assembly | One Quire project of its own, in the original's language | **D20**, below |
| Names C# can't spell (Mpl operators `==.`, primes `abs'`) | Spelled out: `_eq_eq_dot`, `abs_prime` | `[MirrorOf]` keeps the original; see `Identifier.cs` |

**D20 layout.** Every in-scope Mathcad assembly has exactly one target project, named by `Rules.ProjectFor`, in the
original's language ([decisions](decisions.md#d20-one-quire-project-per-mathcad-assembly-in-its-original-language-2026-10-23)):

| Original | Target project | Example |
|---|---|---|
| `Ptc.X`, `MathcadPrime`, `Ptc.MathcadPrime.Automation` | as in the namespace rules | `Ptc.Controls.Text` → `Quire.Controls.Text` |
| Projects already 1:1 before D20 | kept: `mplasb` → `Quire.Mpl.Emit`, `mplrun` → `Quire.Mpl`, `utilsmgd` → `Quire.Utils`, `utl-mgd` → `Quire.Engine.Utl`, `mcmisc` → `Quire.Engine.Misc`, `iengine` → `Quire.Engine` | |
| Other managed originals | `Quire.` + the metadata assembly name; lower-case names use `NativeAssemblyName` (`_` → `Impl`, `-x` → `X`) | `EfiBessel` → `Quire.EfiBessel`, `efibessel_` → `Quire.EfibesselImpl`, `engutils-mgd` → `Quire.EngutilsMgd` |
| Name collisions | `automation` → `Quire.Automation.Native` (`Quire.Automation` is `Ptc.MathcadPrime.Automation`); `mpl` → `Quire.MplModule` (`Quire.Mpl` is mplrun) | |
| Native DLLs | `Quire.Native.` + `NativeAssemblyName` | `utl` → `Quire.Native.Utl`, `efistatsci_` → `Quire.Native.EfistatsciImpl` |

**Hosting during the migration.** A target counts as split once its project exists in the original's language
(`.csproj` for C#, `.vcxproj` for C++/CLI). The Mpl project format arrives with the Mpl toolchain, so no Mpl target is
split yet. Until then MirrorGen keeps writing an assembly's members to its D2 host: `Quire.Mpl.Modules` (`mpl*`,
`hashtable`, `mcdrun`, `mcdstaticunitsystem*`), `Quire.Efi` (`efi*`), `Quire.Automation.Native` (`automation`,
`automationmgd`, `componentsmgd`), `Quire.Engine` (`iengine`, `engutils-mgd`), `Quire.Mcd` (the other `mcd*`) and
`Quire.Engine.Native` (`mccore12`, `mcfront12`, `mceng`, `msclr`, `msgen`, `mslisp`, `mspcons-mgd`). MirrorGen never
writes C# into a split C++/CLI target, and refuses to run while one still hosts another assembly (split
`engutils-mgd` out of `Quire.Engine` before `iengine` becomes C++/CLI). Each split moves that assembly's hand-written
members to the target in the same change.

**Duplicates.** Each project mirrors its original's metadata. If the original defines a type compiled from a shared
header, the project defines it too (C++/CLI projects include the one shared header, so each compiles its own copy);
if the original only references it, the project references the owner. The C# public duplicates `Mathcad.IValue` and
`IMatrixValue` are embedded COM interop types. MirrorGen still applies the D2 rule (one copy per project when
internal, one overall when public) to the D2 hosts; the per-original rule takes effect as each assembly splits.

**Support projects.** Projects with no Mathcad counterpart are an explicit list: `Quire.Mirror` (attributes and
`MirrorStub`), `Quire.Mpl.Runtime` (`BuiltinUnits`, `AffineUnit`), and a home for `Quire.Engine`'s own interpreter
code before `iengine` becomes C++/CLI. They are not completion units. Mirrored method bodies never call into them.

**Existing Quire types with no Mathcad counterpart.** `LinearParser`, `Interpreter`, `Numerics`, `Boxes`,
`VisualNodeBuilder`, `PlotGraph` and others become `internal` implementation types under a `…Internal` namespace of
their assembly, or move to a support project. They have to exist: Mathcad's equivalents are native or BAML code that
isn't visible in metadata.

## 5. Moving the existing code (phase by phase)

| Current Quire type | Mirrored home |
|---|---|
| `Quire.Values.*` (`NumVal`, `MatVal`, `Dim`, `Unit`, `Units`, `UnitSystem`) | Same names under `Quire.Mcd` (D1) |
| `Quire.Mpl.Run.Sexpr`/`SList`/`Sym`/`Atom`, `SexprReader` | `Mpl.Run` types; the reader becomes `Mpl.Run.Io`-shaped |
| `Quire.Engine.Engine`, `IEngine`, `NodeStatus`, `NodeCallback` | `Engine.IEngine`, `Engine.Factory`; `NodeStatus` also exists in `Calculation.Core` |
| `Quire.Engine.Interpreter`, `Builtins`, `Numerics`, `MatrixOps` | A support project (D20); the `efi*` behaviour moves to the per-assembly `efi*` mirrors |
| `Quire.Calculation.*` | `Ptc.Calculation` / `.Core` counterparts (18 types already match) |
| `Quire.PersistentDataObjects.*`, `WorksheetPackage`, `WorksheetConverter` | `Ptc.Controls.Worksheet.*PersistentData`, `Ptc.PersistentDataObjects`, `Ptc.Serialization` |
| `Quire.EquationEditor.Tree.*`, `View.*` | `Ptc.EquationEditor` counterparts; box layout becomes `internal` |
| `Quire.Controls.*` region controls, `WorksheetModel`, `ControlFactory` | Same names (largely already mirrored) |
| `Quire` app shell (`MainWindow`, `Palettes`, …) | `Spirit` counterparts |

The migration table is the original phase routing guide; Phase 1 has already moved the value and S-expression types. Confirm remaining moves against current source. `Rules.cs`, `Mapper.cs`, and `Identifier.cs` define the implemented mappings.
