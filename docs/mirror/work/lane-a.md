# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Members: `IO.PackageOperationsProvider.CreateNewImagePart`, `MouseEventGenerator.NormalizeCoordinates(Point, out int, out int)`, `MouseEventGenerator.GenerateMouseClick(Point, int)`, `MouseEventGenerator.SendMouseInput(int, int, SendMouseInputFlags)`, `PersistentData.ProtectableTypesRegistry.GetXmlSerializer(string, bool)`, `PersistentData.ProtectableTypesRegistry.Register(string, Type)`, `PersistentData.ProtectableTypesRegistry..cctor`, `PersistentData.AreaType.GetXmlSerializerStatic(bool)`, `PersistentData.OleUtils.IsMicrosoftEquationGuid(Guid)`, `PersistentData.OleUtils.IsSupportedExcelGuid(Guid)`, `PersistentData.OleUtils.GetExcelSheetGuid(string)`, `PersistentData.OleUtils.IsOleAppSupportZoom(string)`, `PersistentData.OleUtils.IsVersionInstalled(object)`, `PersistentData.OleUtils.ProgIDFromCLSID(ref Guid)`, `PersistentData.OleUtils..cctor`, `PersistentData.oleBaseContainerType.CopyFrom(IOleSizeData)`, `FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper.GetFunctionalityBehaviorAccordingParameter(object, IHasFunctionalityKeyAccordingToParameter)`, `FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper.CanExecute(object)`, `CanExecute(object, IInputElement)`, `Execute(object, IInputElement)`, `PersistentData.RegionCollectionPreSerializer.PreSerialize()`, `Properties.SettingsHelper.GetMathcadPrimeLicenseConfigValues(string[])` landed.
- Release IL report: `.mirror-cache/release-il-2f042cb192dd/release-il.json` — PASS, 3,012 exact, 539 unfinished; no mismatches.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
