# Lane controls-core — batch checkpoint (Quire.Controls.Core)

- Owner: dispatched worker, Orca task `task_113b76a9ec27`; branch `feature/controls-core-claude`.
- Classification: `docs/mirror/work/lane-controls-core-classify.md` (1,108 stubs: 901 unblocked, 90 blocked, 117 deferred as touched by lane N).

## Landed batches (on `main`, hash after rebase)

| Batch | Commit on main | Members | Release IL report |
|---|---|---|---|
| 1 | `ca9d4f2cc` CalculationVariables | 30 | PASS, 2,327 checked, 0 failures |
| 2 | `27a25180b` UIElementFacade | 16 | PASS, 2,343 checked, 0 failures |
| 3 | `e012da1bc` DummyWorksheetPropertiesSource | 23 | PASS, 2,362 checked, 0 failures |
| 4 | `01651d5e3` NumberFormat constants and backing fields | 26 (no method rows) | PASS, 2,362 checked, 0 failures |
| 5 | `de1b31515` FrameworkElementFacade | 15 | PASS, 2,377 checked, 0 failures |

## Attempted and not landed

- `MathFormatAttachedProperties` (27 unblocked): `CoerceFontSize` needs `blt.un` (unordered branch); `HandleMathFormatChanged` needs the Mathcad early-return layout; `CoerceFontFamily` needs `call` on a `new Typeface(...)` receiver (Roslyn emits `callvirt`). The static constructor registers `FontFamilyName` with that coerce callback, so the constructor cannot match in part. Reverted.

## Remaining, by reason (still `[MirrorStub]` in the generated files)

- Closure or lambda ordinal (87): `OleContainerModel_1`, `OleContainerViewModel_3`, `MC14Conversion.Mathcad14TextStyle`, `MathFormat`, `MatrixFormat`, `Serialization.*`, `TextStylesCollectionManager`, `Whiteboard.ContainerItemManager`, `ImageControl`, `IORegions.IOItemsAliasManager`, `FrameworkElementFacade.ExecuteOnUnload`, `CalculationOptions`, `Controls.Text.CustomScriptRunBase`, and others (see classify doc).
- XAML / InitializeComponent (2): `NumericUpDown` constructor and `InitializeComponent`.
- COM / native (1): `OleContainerModel_1.SetVisibleSize`.
- Shape-blocked, found this session: `FrameworkElementFacade.GetFrameworkElement` (Mathcad pops the null test before `throw`); `SetSize`, `SetHeight`, `SetWidth`, `SetMaxSize`, `ClearMaxSize` (Mathcad reuses the element through `dup` chains without a local); `ExtendedFont.CoerceFontFamily` (same `call`/`callvirt` problem as above).
- Deferred, touched by lane N (117): `WorksheetPageLayout`, `RegionBorder`, `AttachedProperties`, `OleContainer_4`, `ExcelReader`, `ProtectDialog`, `RtfHelper`, `IEquationControl`.
- Unblocked and not landed: 791 (901 classified unblocked minus the 110 landed above). This includes the `MathFormatAttachedProperties` group and the shape-blocked members listed above; the rest have not been attempted. See the unblocked table in the classify doc.
