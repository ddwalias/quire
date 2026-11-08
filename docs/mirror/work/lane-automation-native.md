# Automation.Native lane checkpoint

- Owner: Automation.Native lane; branch `mirror/automation-native-20261108`.
- Members: `Mathcad.Automation.PlainDataConversionOptions` field and constructor. Tried an idiomatic `double.NaN` field initializer; the Release IL gate matched the constructor exactly.
- Release IL reports: PASS, 10 checked (`.mirror-cache/release-il-39c04defc71a/release-il.json`); PASS, 12 checked (`.mirror-cache/release-il-e99701c32c75/release-il.json`).
- Members: `Mathcad.Automation.SerializerCOMStreamAdaptor` constructor and `Commit`; passed the original stream to the base constructor and preserved the IL's handled `NotImplementedException` region around `base.Commit()`.
- Remaining: 482 unfinished members reported by the latest gate. `Mathcad.Automation.UnitPower` is absent from `automationmgd.dll`'s type list; no implementation attempted pending counterpart/mapping resolution. Full stub inventory is in the ignored local cache.
