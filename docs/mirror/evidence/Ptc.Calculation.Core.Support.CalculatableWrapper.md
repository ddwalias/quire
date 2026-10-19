# Ptc.Calculation.Core.Support.CalculatableWrapper

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference/reference.py`); original event-member records also retain their MemberGen mechanical case notes
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `HandleRequestEvaluation(ICalculatable)` | 0x060002e3 | MemberGen: `FireRequestEvaluation(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestEvaluation add` | 0x060002e7 | MemberGen: `public event Action<ICalculatable> RequestEvaluation;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestEvaluation remove` | 0x060002e8 | MemberGen: `public event Action<ICalculatable> RequestEvaluation;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValueOfIdentifierBefore add` | 0x060002e9 | MemberGen: `public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValueOfIdentifierBefore remove` | 0x060002ea | MemberGen: `public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TreatAsUpToDate add` | 0x060002eb | MemberGen: `public event Action<ICalculatable> TreatAsUpToDate;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TreatAsUpToDate remove` | 0x060002ec | MemberGen: `public event Action<ICalculatable> TreatAsUpToDate;` | Mechanical; Release IL equals Mathcad's | Verified |
| `WrappedCalculatable get` | `0x060002e1` | Returns `_wrappedCalculatable`. | Compared with generated wrapped-calculatable inputs. | Verified for recorded cases |
| `WrappedCalculatable set` | `0x060002e2` | If the reference changes, unhooks the old object, stores the new one, and hooks the new object's events. | Compared with generated old/new reference cases. | Verified for recorded cases |
| `Rl get` | `0x060002ed` | Forwards `Rl` from `WrappedCalculatable`. | Compared with generated property values. | Verified for recorded cases |
| `ResultsProcessingPriority get` | `0x060002fa` | Returns null unless the wrapped object implements `ICalcWithPriority`; otherwise forwards its priority. | Compared with generated interface-proxy cases. | Verified for recorded cases |
| `ContainsSymbolicEvaluation get` | `0x060002fb` | Forwards the property when the wrapped object implements `ICanContainSymbolicCalculation`; otherwise returns false. | Compared with generated interface-proxy cases. | Verified for recorded cases |
| `.ctor(ICalculatable)` | `0x060002e0` | Stores the wrapped object and subscribes to its three calculatable events when non-null. | Compared with generated null and non-null objects. | Verified for recorded cases |
| `HandleGetValueOfIdentifier(ICalculatable,string)` | `0x060002e4` | Invokes `GetValueOfIdentifierBefore` with this wrapper and the identifier, returning the event result or null. | Compared with generated event subscriber and null cases. | Verified for recorded cases |
| `HandleTreatAsUpToDate(ICalculatable)` | `0x060002e5` | Invokes `TreatAsUpToDate` with this wrapper when a handler exists. | Compared with generated event subscriber and null cases. | Verified for recorded cases |
| `FireRequestEvaluation(ICalculatable)` | `0x060002e6` | Invokes `RequestEvaluation` with this wrapper when a handler exists. | Compared with generated event subscriber and null cases. | Verified for recorded cases |
| `Commit()` | `0x060002ee` | Forwards `Commit` to the wrapped object. | Direct comparison records the called interface member. | Verified for recorded cases |
| `SetStatusSynchronized()` | `0x060002ef` | Forwards `SetStatusSynchronized`. | Direct comparison records the called interface member. | Verified for recorded cases |
| `SetSynchronizationStatus(SynchronizationStatus)` | `0x060002f0` | Forwards the supplied status. | Direct proxy comparison normalizes enum values and records the call. | Verified for recorded cases |
| `SetCalculatingStatus(CalculatingStatus)` | `0x060002f1` | Forwards the supplied status. | Direct proxy comparison normalizes enum values and records the call. | Verified for recorded cases |
| `SetResult(IResultNode)` | `0x060002f2` | Forwards the supplied result. | Compared with generated result-node inputs. | Verified for recorded cases |
| `SetError(EngineError)` | `0x060002f3` | Forwards the supplied error. | Direct proxy comparison records the call with a null error case. | Verified for recorded cases |
| `SetIdentifierLabels(IDictionary<ResolvedId,string[]>)` | `0x060002f4` | Copies the map; when a key refers to this wrapper, creates a replacement key referring to `WrappedCalculatable`, then forwards the copy. | Generated direct comparison covers non-self-referential keys. The self-reference path constructs Tier B `ResolvedId` and remains unverified. | Verified for recorded cases |
| `SetAutoSelectAlgorithm(IDictionary<int,string>)` | `0x060002f5` | Forwards the supplied map. | Compared with generated dictionary inputs. | Verified for recorded cases |
| `SetRedefinitionWarnings(IList<EngineRedefInfo>)` | `0x060002f6` | Forwards the supplied warning list. | Compared with generated list inputs. | Verified for recorded cases |
| `Dispose()` | `0x060002f7` | Unsubscribes the wrapper's three handlers from `WrappedCalculatable`. | Compared with generated event proxy state. | Verified for recorded cases |
| `HookupCalculatableEvents(ICalculatable)` | `0x060002f8` | For a non-null calculatable, subscribes request, identifier lookup, and up-to-date handlers. | Compared with generated event proxy state. | Verified for recorded cases |
| `UnhookCalculatableEvents(ICalculatable)` | `0x060002f9` | For a non-null calculatable, unsubscribes the same handlers. | Compared with generated event proxy state. | Verified for recorded cases |

## Test cases

**Observed** (`PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests`): `$env:QUIRE_REQUIRE_MATHCAD = '1'; dotnet test tests\Quire.Differential.Tests\Quire.Differential.Tests.csproj -c Release --filter 'FullyQualifiedName~PtcCalculationCoreSupportCalculatableWrapperMechanicalDifferentialTests'` — 28 passed, 0 failed, 0 skipped. The existing seven event/request comparisons remain; the remaining 21 Tier A methods were added. Three forwarding cases use direct `MethodComparison.Compare` with an `ICalculatable` dispatch proxy, including enum normalization. Release MethodAudit: 28/28 exact, 28/28 structurally matched, 0 inspection errors; base implementations were 0/21 exact, 0/21 structural. Report: `%TEMP%\lane-c-calculatable-wrapper-release-audit.json`.
