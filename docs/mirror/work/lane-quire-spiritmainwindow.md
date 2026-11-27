# Quire SpiritMainWindow lane checkpoint

- Owner: Orca worker `task_06cb9306969f`; branch `feature/quire-spiritmainwindow-20261127`.
- Batch 1: moved 11 field-like events into the hand-written partial; implemented their 22 accessors, 31 property/accessor methods, `InitializeComponent`, and `_CreateDelegate`. Generated twins were removed in the same change.
- Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,438 checked, 0 failures; report `.mirror-cache/release-il-552f7c850460/release-il.json`.
- Remaining: `IComponentConnector.Connect` stays a generated stub because connection ID 12 requires the unmapped `DevComponents.WpfDock.DockWindowGroup` field `_mainDockWindowGroup`; `LastCustomMarginValueSetting.set` stays a placeholder because the current idiomatic nullable comparison lowers differently. Closure/iterator members remain for a later batch and must use whole-type layout in Mathcad IL order.
- Batch 2: added 16 exact methods (`OpenStartPage`, command handlers, template helpers, and Prime activation handlers) in `SpiritMainWindow.Batch2.cs`; `HandleShowPropertyGrid` remains a generated stub because Roslyn emits `call Window.Show` for `new DebugPropertyWindow().Show()`, while Mathcad IL requires `callvirt`.
- Batch 2 Release IL: `python tools/validation/release_il.py --base main --project Quire` — PASS, 1,454 checked, 0 failures; report `.mirror-cache/release-il-0b14a24be1a5/release-il.json`. Combined transition count: 62 newly exact methods across batches 1 and 2.
