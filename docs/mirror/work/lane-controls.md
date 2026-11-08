# Lane Controls — Quire.Controls (Ptc.Controls.dll)

- Owner: Controls (dispatch task_4af6509bb461). Branch `mirror/controls-20261108` from `main`. Assembly assigned by maintainer.
- Batch 1: 24 stubs across 11 types implemented and verified exact:
  - `ProductInfo.Constants` (ConverterProductName, ConverterExe, .ctor)
  - `DefaultPdfDriverNames` (MicrosoftPrintToPdf)
  - `Help.HelpIdDictionaryUids` (ToggleInputRegion, ToggleOutputRegion, GettingStarted)
  - `BooleanConverterWithDefaultValue` (.ctor, ConvertTo)
  - `ColorNullableConverter` (.ctor, ConvertTo)
  - `ThicknessNullableConverter` (.ctor, ConvertTo)
  - `PlotBackgroundTypeToXmlValueConverter` (.ctor, ConvertTo, ConvertFrom)
  - `DoubleToPercentDescriptionConverter` (Convert, ConvertBack)
  - `DetachedCopyOptions` (Empty, HandlingException, .cctor)
  - `RegularSaveOptions` (Empty, NonPrimeFormatForApi, WithStatesGeneration, .cctor)
  - `Properties.Settings` (MicrosoftPdfDriverName get/set, .cctor defaultInstance initializer)
- Release IL: PASS; 291 checked, 291 exact, 0 failures. Report: `.mirror-cache/release-il-07131876152f/release-il.json`.
