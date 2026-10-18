# Engine.Factory

- **Original:** iengine.dll 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`.
- **Reference:** ilspycmd 11.1.0.9782 (local metadata views under ignored `.mirror-cache/`).
- **Reference:** inspection record `ev_94dd6839de48104feff155f7a163092b4a5c6738543e2165fe4d9df42f1974c7` (`managed-member inspection`); artifact identity confirmed by inspection record `ev_e0c8f513a82d9d54bdc33a95482df72239a3fff47642216ca3a6341c75146be4` (`artifact identity check`).
- **Quire:** src/Quire.Engine/Factory.cs and src/Quire.Engine/Generated/Mirror/Engine.Factory.g.cs; tests: tests/Quire.Engine.Tests/EngineApiTests.cs, tests/Quire.Differential.Tests/EngineFactoryDifferentialTests.cs.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()->[System.Runtime]System.Void` | 0x060000bd | Initializes Factory instance | High; mechanical differential comparison against Mathcad iengine | Verified |
| `AppendPath([System.Runtime]System.String)->[System.Runtime]System.Void` | 0x060000be | Appends path segment to Rts.AssemblyPath | High; differential tests against Mathcad iengine with multiple path patterns and null | Tested |
| `NewFactory()->[iengine]Engine.IFactory` | 0x060000bf | Creates IFactory instance | High; verified via EngineApiTests | Tested |
| `CreateFactory()->[iengine]Engine.IFactory` | 0x060000c0 | Static factory helper returning EngineFactory | High; internal factory method verified via NewFactory tests | Tested |

## Test cases

- Mechanical differential comparison: `EngineFactoryDifferentialTests.ctor_060000bd` verifies `.ctor` matches Mathcad Prime field state.
- Differential test: `EngineFactoryDifferentialTests.Factory_AppendPath_differential` tests `AppendPath` against Mathcad Prime's `Engine.Factory.AppendPath` with null, empty, single, and multiple paths.
- Engine API unit tests: `EngineApiTests.Factory_AppendPath_appends_to_assembly_path` and `EngineApiTests.Factory_creates_engines_and_tracks_them`.
