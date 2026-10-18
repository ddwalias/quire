# Ptc.Wpf.Printing.PrintJob

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfPrintingPrintJobMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600066a | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PrintableDocument get` | 0x06000658 | MemberGen: `public IDocumentPaginatorSource PrintableDocument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PrintableDocument set` | 0x06000659 | MemberGen: `public IDocumentPaginatorSource PrintableDocument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsRunning get` | 0x0600065a | MemberGen: `public bool IsRunning { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsRunning set` | 0x0600065b | MemberGen: `public bool IsRunning { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `JobStarted add` | 0x06000654 | MemberGen: `public event EventHandler<WritingProgressChangedEventArgs> JobStarted;` | Mechanical; Release IL equals Mathcad's | Verified |
| `JobStarted remove` | 0x06000655 | MemberGen: `public event EventHandler<WritingProgressChangedEventArgs> JobStarted;` | Mechanical; Release IL equals Mathcad's | Verified |
| `JobCompleted add` | 0x06000656 | MemberGen: `public event EventHandler<WritingCompletedEventArgs> JobCompleted;` | Mechanical; Release IL equals Mathcad's | Verified |
| `JobCompleted remove` | 0x06000657 | MemberGen: `public event EventHandler<WritingCompletedEventArgs> JobCompleted;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfPrintingPrintJobMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
