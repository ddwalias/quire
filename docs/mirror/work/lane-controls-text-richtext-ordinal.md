# Quire.Controls.Text RichText closure order

- Owner: Lane Controls.Text; branch `feature/controls-text-richtext-ordinal`, based on `c067d216c`.
- Scope: `RichTextBase` first slice, all members before and including `GetInsertionPointAtPointer` (IL method index 135); the `RichTextWithEmbeddedItem` reorder remains.
- Method: for each generated closure `<Member>b__N_M`, local function, or iterator, derive the non-method offset as `N - IL method index`; place fields, properties, events, and nested types around the method stream to meet the anchors. The generated `SyncTextChangedCommand` shell is the first parent member; hand-bound `PropertyNames` and `BlockedSelectionDirection` are positioned in the hand-written partial.

## Derived anchor table

Outer `RichTextBase` anchors (29 positions, ordered by IL method index):

| IL method index | Member | Closure ordinal N | Offset N − index |
|---:|---|---:|---:|
| 0 | `get_CustomScriptsSizesForXamlSerialization` | 2 | 2 |
| 1 | `set_CustomScriptsSizesForXamlSerialization` | 3 | 2 |
| 31 | `.cctor` | 81 | 50 |
| 39 | `.ctor` | 93 | 54 |
| 41 | `InitializeFeatureFlagCommandBindings` | 95 | 54 |
| 50 | `SetStyleFromTextStyles` | 105 | 55 |
| 54 | `HandleApplyTextStyle` | 109 | 55 |
| 55 | `DoApplyTextStyle_internal` | 110 | 55 |
| 56 | `HandleClearTextFormat` | 111 | 55 |
| 57 | `ClearFormatPropertiesInRange` | 112 | 55 |
| 58 | `ClearSelectedRegionFormatProperties` | 113 | 55 |
| 61 | `HandleToggleSubscriptCommand` | 118 | 57 |
| 62 | `HandleToggleSuperscriptCommand` | 119 | 57 |
| 65 | `ToggleRunElementAccordingToScriptState` | 122 | 57 |
| 66 | `RecalculateTextScriptState` | 123 | 57 |
| 70 | `HandleSetCurrentProofingLanguage` | 127 | 57 |
| 98 | `PostPasting` | 159 | 61 |
| 103 | `WasScriptElementDeletedOnChangeEvent` | 165 | 62 |
| 104 | `UpdateCurrentRuns` | 167 | 63 |
| 106 | `ComputeProofingLanguage` | 169 | 63 |
| 113 | `ExecuteTextChangedAndReactivateCommand` | 177 | 64 |
| 128 | `GetTextFormatForWholeDocument` | 196 | 68 |
| 130 | `set_CaretOffset` | 199 | 69 |
| 135 | `GetInsertionPointAtPointer` | 207 | 72 |
| 142 | `EnumerateEmbeddedObjectsInRange` iterator | 217 | 75 |
| 160 | `IsInvalidOperation` | 235 | 75 |
| 177 | `Activate` | 261 | 84 |
| 192 | `SetTextFormatPropertyValue` | 284 | 92 |
| 203 | `IsFullySubscript` | 297 | 94 |

The two anchors inside nested `RichTextBase.SyncTextChangedCommand` are `.ctor` (nested method index 0, ordinal 9, offset 9) and `Undo` (index 1, ordinal 10, offset 9). This makes 31 anchors total; nested-class offsets use that nested type's method stream.

## RichTextWithEmbeddedItem anchors

Derived from `Ptc.Controls.Text.RichTextWithEmbeddedItem.il` and its root method order. The duplicate `DisplayClass52` and `DisplayClass53` anchors belong to `GetPotentialLinkEndPosition`.

| IL method index | Member | Closure ordinal N | Offset N − index |
|---:|---|---:|---:|
| 17 | `CanModifyHyperlink` | 20 | 3 |
| 41 | `TryPostChangeHyperlinkInsertion` display class | 52 | 11 |
| 42 | `GetPotentialLinkEndPosition` display class | 53 | 11 |
| 54 | `.ctor` | 86 | 32 |
| 58 | `OnPreviewTabForward` | 90 | 32 |
| 64 | `ExecuteChangeFontColor` display class | 96 | 32 |
| 65 | `ExecuteChangeHighlightColor` display class | 97 | 32 |
| 70 | `GetCombinedDocumentXaml` | 103 | 33 |
| 77 | `set_CaretOffset` display class | 113 | 36 |
| 78 | `EnumerateEmbeddedObjectsInRange` iterator | 114 | 36 |
| 81 | `ExecuteAddItemToListCommand` display class | 117 | 36 |
| 82 | `ExecuteRemoveItemFromListCommand` display class | 118 | 36 |
| 83 | `RestoreTextBoxContent` | 119 | 36 |
| 87 | `OnLostFocus` | 123 | 36 |
| 92 | `AddItems` | 128 | 36 |
| 93 | `RemoveItems` | 129 | 36 |
| 94 | `MoveItems` | 130 | 36 |
| 96 | `ResetCaretPositionWhenNecessary` display class | 132 | 36 |
| 102 | `CreateTextChangedCommand` | 138 | 36 |
| 109 | `get_IsTabNavigatable` | 149 | 40 |
| 110 | `TabInto` | 150 | 40 |
| 126 | `RemoveInlineRecursively` display class | 166 | 40 |
| 138 | `Activate` | 179 | 41 |
| 167 | `get_HideFromCalculationContext` | 219 | 52 |
| 168 | `set_HideFromCalculationContext` display class | 220 | 52 |
| 169 | `ToggleHideFromCalculationContext` | 221 | 52 |
| 176 | `UpdateHideFromCalculationButton` | 230 | 54 |
| 195 | `GetWhiteboardItemsInRange` display class | 257 | 62 |
| 196 | `HandleSelectionChanged` display class | 258 | 62 |
| 215 | `get_Lines` iterator | 284 | 69 |

## Slice 1

- Reordered the direct declarations through `GetInsertionPointAtPointer` in Mathcad method order, with non-method declarations interleaved to hit the anchor offsets. Implemented `GetInsertionPointAtPointer` from the reference shape; its closure emits `<GetInsertionPointAtPointer>b__207_0`.
- Release IL report: `.mirror-cache/release-il-f742d506ade8/release-il.json` — PASS, 695 checked, zero failures; 76 generated stubs, 44 no-pair members, 3 unchanged/unmatched.
- Landed as `58b12a5a47adeeb9bb25735ba6ae3d971b8231f2`; lane receipt: `C:/Projects/Quire/.mirror-cache/receipts/gate-il-b1ea435585cc.json`. MirrorGen `emit` left the target generated file byte-identical; unrelated emitter output was reverted.

## Slice 2

- Implemented `EnumerateEmbeddedObjectsInRange`; iterator closure `<EnumerateEmbeddedObjectsInRange>d__217` now matches at ordinal 217. The current compiler omits the iterator-field clear emitted by Mathcad, so the method explicitly clears `curPos` after the loop to preserve Release IL.
- Release IL report: `.mirror-cache/release-il-0ad0645d1d89/release-il.json` — PASS, 704 checked, zero failures; 75 generated stubs, 44 no-pair members, 3 unchanged/unmatched.
- Landed as `1d387175dfb61de9d0e83ac6e2a6bcd5e4e484b8`; lane receipt: `C:/Projects/Quire/.mirror-cache/receipts/gate-il-747b359a7178.json`. MirrorGen `emit` left the target generated file byte-identical; unrelated emitter output was reverted.

## Slice 3

- Implemented `Activate`; callback closure `<Activate>b__261_0` matches at ordinal 261.
- Release IL report: `.mirror-cache/release-il-cebf9a722ab6/release-il.json` — PASS, 706 checked, zero failures; 74 generated stubs, 43 no-pair members, 3 unchanged/unmatched.
- `IsInvalidOperation` (ordinal 235) remains a placeholder. The idiomatic decompilation did not match closure allocation/control-flow IL under the current compiler and was reverted to keep the gate clean.

## Slice 4

- Implemented `SetTextFormatPropertyValue`; its closure anchor at ordinal 284 now matches.
- Release IL report: `.mirror-cache/release-il-8b940b406f48/release-il.json` — PASS, 711 checked, zero failures; 73 generated stubs, 43 no-pair members, 3 unchanged/unmatched.
- Landed as `e3a64cf36f8d9aa264bf45151ef64b4bbf53f166`; lane receipt: `C:/Projects/Quire/.mirror-cache/receipts/gate-il-66841c8de9b0.json`. MirrorGen `emit` left the target generated file byte-identical; unrelated emitter output was reverted.

## Slice 5

- Implemented `IsFullySubscript`; its closure anchor at ordinal 297 now matches.
- Release IL report: `.mirror-cache/release-il-155c2a18f60e/release-il.json` — PASS, 713 checked, zero failures; 72 generated stubs, 42 no-pair members, 3 unchanged/unmatched.
- Remaining: continue the later `RichTextBase` anchors and reorder `RichTextWithEmbeddedItem`; `IsInvalidOperation` remains pending. Request a landing slot before each merge.


## RichTextWithEmbeddedItem slice 1

- Reordered the direct declarations into Mathcad IL method order, including unmarked placeholders, and implemented `CanModifyHyperlink`; closure `<CanModifyHyperlink>b__20_0` now matches ordinal 20.
- Kept `_savedActiveItem` after the closure anchors. Placing it before the color-command closures shifted subsequent ordinals by one; this position preserves the anchors listed above.
- Release IL report: `.mirror-cache/release-il-61d293b4ba80/release-il.json` — PASS, 715 checked, zero failures; 52 generated stubs, 41 no-pair members, 22 unchanged/unmatched.
- Release build succeeded; MirrorGen `emit` left `Controls.Text.RichTextWithEmbeddedItem.g.cs` byte-identical.


## RichTextWithEmbeddedItem slice 2

- Implemented `TryPostChangeHyperlinkInsertion`; closure `<TryPostChangeHyperlinkInsertion>b__0` matches ordinal 52. The nested guards retain the shared exit block required by the reference Release IL.
- Release IL report: `.mirror-cache/release-il-49e0c48e699c/release-il.json` — PASS, 718 checked, zero failures; 51 generated stubs, 41 no-pair members, 22 unchanged/unmatched.
- Release build succeeded; MirrorGen `emit` left `Controls.Text.RichTextWithEmbeddedItem.g.cs` byte-identical.
