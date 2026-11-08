# Automation.Native lane checkpoint

- Owner: Automation.Native lane; branch `mirror/automation-native-20261108`.
- Members: `Mathcad.Automation.PlainDataConversionOptions` field and constructor. Tried an idiomatic `double.NaN` field initializer; the Release IL gate matched the constructor exactly.
- Release IL reports: PASS, 10 checked (`.mirror-cache/release-il-39c04defc71a/release-il.json`); PASS, 12 checked (`.mirror-cache/release-il-e99701c32c75/release-il.json`); PASS, 13 checked (`.mirror-cache/release-il-7e4e8026efb2/release-il.json`); PASS, 17 checked (`.mirror-cache/release-il-b3da47a804e5/release-il.json`). Main-checkout landing gates passed after refreshed-base revalidation (`C:/Projects/Quire/.mirror-cache/gate-il-32fc6cf0d948/gate.json`, `gate-il-cf691e3e076f/gate.json`).
- Members: `Mathcad.Automation.SerializerCOMStreamAdaptor` constructor and `Commit`; passed the original stream to the base constructor and preserved the IL's handled `NotImplementedException` region around `base.Commit()`.
- Members: `Mathcad.Automation.MPLRunSerializationBinder` constructor and `BindToType`; matched the framework base call, assembly-name comparison, and `MatVal` assembly type lookup.
- Members: `Mathcad.Automation.SymbolicValueProxy` field, properties, and constructor; returned its field for `Core`/`Symbolic`, `this` for `AsSymbolic`, and delegated constructor arguments to the base before storing the field.
- Remaining: 477 unfinished members reported by the latest gate. `Mathcad.Automation.UnitPower` is absent from `automationmgd.dll`'s type list; no implementation attempted pending counterpart/mapping resolution. Full stub inventory is in the ignored local cache.
