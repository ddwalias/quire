# Engine.NodeCallback

- **Original:** iengine.dll 1.1.0.0, SHA-256 `e34c1c468b07e2016dcd98c4397a139ca6c3a30cd2a14d95563db52e3419947b`, MVID `eefa0fd5-7185-4381-9183-8ef64202914c`.
- **Reference:** ilspycmd 11.1.0.9782 (local metadata views under ignored `.mirror-cache/`).
- **Reference:** REA `ev_94dd6839de48104feff155f7a163092b4a5c6738543e2165fe4d9df42f1974c7` (`inspect_managed_members`); artifact identity confirmed by REA `ev_e0c8f513a82d9d54bdc33a95482df72239a3fff47642216ca3a6341c75146be4` (`inspect_managed_artifact`).
- **Quire:** src/Quire.Engine/IEngine.cs; tests: declaration-only.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor([System.Runtime]System.Object,[System.Runtime]System.IntPtr)->[System.Runtime]System.Void` | 0x06000046 | Declaration only; .ctor([System.Runtime]System.Object,[System.Runtime]System.IntPtr)->[System.Runtime]System.Void | REA metadata and same-artifact reference index; no body | Abstract |
| `Invoke([System.Runtime]System.Int32,[iengine]Engine.NodeStatus,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Runtime]System.Void` | 0x06000047 | Declaration only; Invoke([System.Runtime]System.Int32,[iengine]Engine.NodeStatus,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Runtime]System.Void | REA metadata and same-artifact reference index; no body | Abstract |
| `BeginInvoke([System.Runtime]System.Int32,[iengine]Engine.NodeStatus,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.AsyncCallback,[System.Runtime]System.Object)->[System.Runtime]System.IAsyncResult` | 0x06000048 | Declaration only; BeginInvoke([System.Runtime]System.Int32,[iengine]Engine.NodeStatus,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.AsyncCallback,[System.Runtime]System.Object)->[System.Runtime]System.IAsyncResult | REA metadata and same-artifact reference index; no body | Abstract |
| `EndInvoke([System.Runtime]System.IAsyncResult)->[System.Runtime]System.Void` | 0x06000049 | Declaration only; EndInvoke([System.Runtime]System.IAsyncResult)->[System.Runtime]System.Void | REA metadata and same-artifact reference index; no body | Abstract |

## Test cases

Observed from the installed iengine metadata and same-artifact member index. These are interface/delegate declarations or enum constants; there are no method bodies to reconstruct or invoke.

## Open

REA decoded the mixed-mode artifact and identified these members, but reports partial C++/CLI signature parsing for native-int / marshal-related elements. The local reference index supplied the complete declared signatures for the selected declarations. No native implementation body is in this scope.
