# Ptc.Calculation.ThreadExtensions

- **Original:** Ptc.Calculation 12.0.1.0, SHA-256 `fe8cf4f1b2791b7a223d203785600667bb00a3e2b30e975a949fe412cbf07e4b`, MVID `82391b36-0d84-45c3-b21a-f7dccf288c68`
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference/reference.py`)
- **Quire:** `src/Quire.Calculation/ThreadExtensions.Mirror.cs`; tests: `PtcCalculationThreadExtensions_MechanicalDifferentialTests`

| Member | Mathcad token | Quire token | Contract | Confidence / limits | Status |
|---|---:|---:|---|---|---|
| `CaptureUiThreadCulture()` | `0x06000061` | `0x0600008e` | If `appCulture` is null, stores the current thread's `CurrentCulture` and `CurrentUICulture`, in that order. An `InvalidOperationException` is passed to `DebugTools.FailAssertion`. | Directly compared with `en-US` and `fr-FR`; exception path is not covered because `Ptc.Core.DebugTools.FailAssertion(Exception)` remains a Tier B mirror stub. | Verified for recorded case |
| `SyncWithUICulture(Thread)` | `0x06000062` | `0x0600008f` | Returns if either cached culture is null; otherwise applies both cached values to the supplied thread. `ArgumentNullException`, `InvalidOperationException`, and other `Exception` values are passed to `DebugTools.FailAssertion`. | Directly compared on the current thread, applying the captured cultures. Exception paths remain dependent on Tier B `Ptc.Core.DebugTools.FailAssertion(Exception)`. | Verified for recorded case |

## Release comparison

`$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationThreadExtensions_MechanicalDifferentialTests'` — 2 passed, 0 failed, 0 skipped. Both methods use `MethodComparison.Compare` against Mathcad. Release MethodAudit had 0 inspection errors: base was 0/2 exact and 0/2 structural; candidate is 2/2 exact and 2/2 structural. Report: `%TEMP%\lane-c-thread-extensions-release-audit.json`.

The current comparisons cover successful culture capture and synchronization. Failure handlers call the Tier B `DebugTools.FailAssertion(Exception)` mirror and are not claimed equivalent on those paths; that member remains generated and was not edited.
