# Automation.Native lane checkpoint

- Owner: Automation.Native lane; branch `mirror/automation-native-20261108`.
- Members: `Mathcad.Automation.PlainDataConversionOptions` field and constructor. Tried an idiomatic `double.NaN` field initializer; the Release IL gate matched the constructor exactly.
- Release IL report: PASS, 10 checked, `.mirror-cache/release-il-39c04defc71a/release-il.json`.
- Remaining: 484 unfinished members reported by the gate. `Mathcad.Automation.UnitPower` is absent from `automationmgd.dll`'s type list; no implementation attempted pending counterpart/mapping resolution. Full stub inventory is in the ignored local cache.
