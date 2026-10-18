# Engine.NodeStatus

- **Original:** iengine.dll 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`.
- **Reference:** ilspycmd 11.1.0.9782 (local metadata views under ignored `.mirror-cache/`).
- **Reference:** inspection record `ev_94dd6839de48104feff155f7a163092b4a5c6738543e2165fe4d9df42f1974c7` (`managed-member inspection`); artifact identity confirmed by inspection record `ev_e0c8f513a82d9d54bdc33a95482df72239a3fff47642216ca3a6341c75146be4` (`artifact identity check`).
- **Quire:** src/Quire.Engine/IEngine.cs; tests: declaration-only.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `value__` | 0x04000041 | underlying enum value type is Int32 | inspection field metadata; literal values corroborated by local reference view | Abstract |
| `NODE_INVALID` | 0x04000042 | literal value 0 | inspection field metadata; literal values corroborated by local reference view | Abstract |
| `NODE_COMPUTING` | 0x04000043 | literal value 1 | inspection field metadata; literal values corroborated by local reference view | Abstract |
| `NODE_NOT_COMPUTING` | 0x04000044 | literal value 2 | inspection field metadata; literal values corroborated by local reference view | Abstract |
| `NODE_READY` | 0x04000045 | literal value 3 | inspection field metadata; literal values corroborated by local reference view | Abstract |
| `NODE_TREATED_READY` | 0x04000046 | literal value 4 | inspection field metadata; literal values corroborated by local reference view | Abstract |

## Test cases

Observed from the installed iengine metadata and same-artifact member index. These are interface/delegate declarations or enum constants; there are no method bodies to reconstruct or invoke.

## Open

The inspection record identifies the mixed-mode artifact and selected members. Signature parsing is partial for native-int and marshal-related elements; the local reference index supplies complete declared signatures for these declarations. Native implementation bodies are outside this scope.
