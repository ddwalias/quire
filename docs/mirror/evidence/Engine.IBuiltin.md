# Engine.IBuiltin

- **Original:** iengine.dll 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`.
- **Reference:** ilspycmd 11.1.0.9782 (local metadata views under ignored `.mirror-cache/`).
- **Reference:** inspection record `ev_94dd6839de48104feff155f7a163092b4a5c6738543e2165fe4d9df42f1974c7` (`managed-member inspection`); artifact identity confirmed by inspection record `ev_e0c8f513a82d9d54bdc33a95482df72239a3fff47642216ca3a6341c75146be4` (`artifact identity check`).
- **Quire:** src/Quire.Engine/IEngine.cs and src/Quire.Engine/Generated/Mirror/Engine.IBuiltin.g.cs; tests: declaration-only.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetVariable([System.Runtime]System.String)->[System.Runtime]System.Object` | 0x060000a0 | Declaration only; GetVariable([System.Runtime]System.String)->[System.Runtime]System.Object | reference metadata and same-artifact reference index; no body | Abstract |
| `SetVariable([System.Runtime]System.String,[System.Runtime]System.Object)->[System.Runtime]System.Void` | 0x060000a1 | Declaration only; SetVariable([System.Runtime]System.String,[System.Runtime]System.Object)->[System.Runtime]System.Void | reference metadata and same-artifact reference index; no body | Abstract |
| `UnsetVariable([System.Runtime]System.String)->[System.Runtime]System.Void` | 0x060000a2 | Declaration only; UnsetVariable([System.Runtime]System.String)->[System.Runtime]System.Void | reference metadata and same-artifact reference index; no body | Abstract |
| `GetBuiltins()->[System.Runtime]System.Collections.IList` | 0x060000a3 | Declaration only; GetBuiltins()->[System.Runtime]System.Collections.IList | reference metadata and same-artifact reference index; no body | Abstract |

## Test cases

Observed from the installed iengine metadata and same-artifact member index. The original members are declarations without implementation bodies. Quire provides interface declarations.
