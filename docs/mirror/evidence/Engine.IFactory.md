# Engine.IFactory

- **Original:** iengine.dll 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`.
- **Reference:** ilspycmd 11.1.0.9782 (local metadata views under ignored `.mirror-cache/`).
- **Reference:** REA `ev_94dd6839de48104feff155f7a163092b4a5c6738543e2165fe4d9df42f1974c7` (`inspect_managed_members`); artifact identity confirmed by REA `ev_e0c8f513a82d9d54bdc33a95482df72239a3fff47642216ca3a6341c75146be4` (`inspect_managed_artifact`).
- **Quire:** src/Quire.Engine/Factory.cs and src/Quire.Engine/Generated/Mirror/Engine.IFactory.g.cs; tests: declaration-only.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Initialize([System.Runtime]System.String)->[System.Runtime]System.Void` | 0x060000b5 | Declaration only; Initialize([System.Runtime]System.String)->[System.Runtime]System.Void | REA metadata and same-artifact reference index; original declaration has no body; Quire has a generated MirrorStub body | Stub |
| `VerifyIntegrity()->[System.Runtime]System.Boolean` | 0x060000b6 | Declaration only; VerifyIntegrity()->[System.Runtime]System.Boolean | REA metadata and same-artifact reference index; original declaration has no body; Quire has a generated MirrorStub body | Stub |
| `get_WorkerStackSize()->[System.Runtime]System.Int32` | 0x060000b7 | Declaration only; get_WorkerStackSize()->[System.Runtime]System.Int32 | REA metadata and same-artifact reference index; no body | Abstract |
| `set_WorkerStackSize([System.Runtime]System.Int32)->[System.Runtime]System.Void` | 0x060000b8 | Declaration only; set_WorkerStackSize([System.Runtime]System.Int32)->[System.Runtime]System.Void | REA metadata and same-artifact reference index; no body | Abstract |
| `NewEngine()->[iengine]Engine.IEngine` | 0x060000b9 | Declaration only; NewEngine()->[iengine]Engine.IEngine | REA metadata and same-artifact reference index; no body | Abstract |
| `GetEngines()->[System.Runtime]System.Collections.ICollection` | 0x060000ba | Declaration only; GetEngines()->[System.Runtime]System.Collections.ICollection | REA metadata and same-artifact reference index; no body | Abstract |

## Test cases

Observed from the installed iengine metadata and same-artifact member index. The original members are declarations without implementation bodies. Quire provides generated MirrorStub default bodies for rows marked Stub; rows marked Abstract are bare Quire interface declarations. No executable original body is available to reconstruct or invoke.

## Open

REA decoded the mixed-mode artifact and identified these members, but reports partial C++/CLI signature parsing for native-int / marshal-related elements. The local reference index supplied the complete declared signatures for the selected declarations. No native implementation body is in this scope.
