# Mpl.Run.Rts

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/Rts.cs`, `src/Quire.Mpl/Run/True.cs`, `src/Quire.Mpl/Generated/Mirror/Mpl.Run.Rts.g.cs`
  (stub); tests: `Quire.Mpl.Tests.Run.RtsTests`, `Quire.Differential.Tests.RtsDifferentialTests`

`RtsDifferentialTests` compares every declared static field's rendered value immediately after directly invoking the
type initializer on both artifacts. This establishes the recorded field state for that test thread; it does not
observe the AppDomain event's internal delegate list. The initializer remains Tested because the event subscription
itself is not observable in that direct comparison. The private `ms_thread_cancellation_token` field is also
observed through its null and cancelled-source transitions in the later cases.

Covers `Mpl.Run.Rts` and `Mpl.Run.True`. `Rts` is a static class of runtime services for compiled Mpl code: a
per-thread stack budget (thread-static, lazily set to the public static `m_max_stack`, 250), a per-thread
cancellation source, reflection handles that the code generator emits calls to, and a semicolon-separated assembly
search path consulted from the current domain's `AssemblyResolve` event. `m_worker_stack_size` is a public static
int that nothing here reads. `True` is Mpl's boolean true: a class with a public static `singleton`. Every member
below is differential-verified for the cases in "Test cases", except the type initializer and the stub.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `m_stack_depth` (get) | 0x060003b2 | `30d21902` | The calling thread's remaining budget; the thread's first read (unless set earlier) initialises it to the current `m_max_stack`. | High | Verified |
| `m_stack_depth` (set) | 0x060003b3 | `1e26019b` | Sets the thread's budget without marking it initialised, so a set before any read is overwritten by the first read. | High | Verified |
| `RegisterThreadForCancellationChecking(CancellationTokenSource)` | 0x060003b4 | `d556c89f` | Makes the calling thread's cancellation checks follow the source; null unregisters. | High | Verified |
| `IsCancellationRequested()` | 0x060003b5 | `a382b514` | False with no source or with the internal never-cancelled one. When the thread's source is cancelled, switches the thread to the never-cancelled source and returns true, so cancellation is reported once until a source is registered again. A disposed source's state is read through its token, which throws `ObjectDisposedException`. | High | Verified |
| `.cctor` | 0x060003b6 | `c604bfee` | Sets `m_max_stack` to 250 and the search path to empty, creates the never-cancelled source, looks up the handles returned below, sets the function type to `object`, and subscribes to the current domain's `AssemblyResolve`. | High | Tested |
| `ResetStackDepth()` | 0x060003b7 | `24121b53` | Sets the thread's budget to `m_max_stack`. | High | Verified |
| `ExceptionCtor()` | 0x060003b8 | `b0024145` | The `AppException(string)` constructor. | High | Verified |
| `RethrowCtor()` | 0x060003b9 | `b16b3028` | The `AppException(Exception, uint)` constructor. | High | Verified |
| `TagCtor()` | 0x060003ba | `ba4abe24` | The `Tag(int)` constructor. | High | Verified |
| `FcnType()` | 0x060003bb | `326ef886` | `object`, the static type of compiled function values. | High | Verified |
| `BoolTrue()` | 0x060003bc | `a32dc0a3` | The `True.singleton` field. | High | Verified |
| `TagField()` | 0x060003bd | `05b8c5b4` | The `Tag.m_tag` field. | High | Verified |
| `StackDepth` | 0x060003be | `c14026e2` | Looks up a public static field `m_stack_depth` on `Rts` and caches it; since that member is a property, the result is always null (and the lookup repeats). | High | Verified |
| `VoidType()` | 0x060003bf | `821768c3` | `void`. | High | Verified |
| `TagType()` | 0x060003c0 | `a5dfe751` | `Tag`. | High | Verified |
| `GetTypeInitializer(TypeBuilder)` | 0x060003c1 | `39e9d5fc` | Not implemented: the generated stub throws. | — | Stub |
| `AssemblyPath` (get) | 0x060003c2 | `e43ca95f` | The search directories joined by `;`. | High | Verified |
| `AssemblyPath` (set) | 0x060003c3 | `091e2324` | Splits the value on `;`, keeping empty entries; null throws `NullReferenceException`. | High | Verified |
| `OnResolveAssembly(object, ResolveEventArgs)` | 0x060003c4 | `190a85c2` | Answers only when the sender is the current domain: a null sender gives null, a sender that isn't an `AppDomain` throws `InvalidCastException`. Takes the simple name (text before the first comma) and returns the first `<dir>\<name>.dll` that exists in the search directories, loaded by path; otherwise null. | High | Verified |
| `True.cctor` | 0x060003e8 | `899b6ce4` | Creates the singleton. | High | Verified |
| `True.ToString()` | 0x060003e9 | `c5de8e74` | `True`. | High | Verified |
| `True.ctor()` | 0x060003ea | `41ad57d1` | No state; other instances besides the singleton can be made. | High | Verified |

**Search path.** Empty entries are kept, so an empty path is one empty directory, which resolves names against
the process's current directory. The handler is registered for the lifetime of the process once `Rts` initialises,
in Quire as in Mathcad.

Fields: `m_max_stack`, `m_worker_stack_size` and `True.singleton` are Verified (read directly by `RtsDifferentialTests`). The ten private static fields of `Rts` are Verified for the directly compared initializer state and the additional transitions recorded above.

## Test cases

**Observed** (`RtsDifferentialTests`, against the installed `mplrun.dll`; reflection handles compared by declaring
type, name, parameter or field type and staticness):
- the eight handle methods; `m_max_stack`, `m_worker_stack_size`, `StackDepth` and `AssemblyPath`; `AssemblyPath`
  set to `a;b`, empty, `single`, `;;` and a trailing-semicolon path and read back, and set to null; `True.singleton`
  and `new True()` as text;
- on a fresh thread: `IsCancellationRequested` with nothing registered, the initial budget, setting it to 3,
  `ResetStackDepth`, a registered live source, the same source after `Cancel` (true then false), re-registering the
  cancelled source, a null source, and a disposed source;
- the budget a new thread starts with after `m_max_stack` is changed to 12;
- `OnResolveAssembly` with the search path `<missing dir>;<dir holding Probe.dll>` for `Probe`, a full display name,
  an absent name and the empty name, each with the current domain and a null sender, and with a string sender.

**Inferred** (`RtsTests`): each thread starting with the full budget, cancellation reported once, the handles for
generated code, and unresolved assemblies found on the search path through the real `AssemblyResolve` event.

## Open

- `GetTypeInitializer` stays a generated stub: it hands back an initializer from the Mpl code generator
  (`Quire.Mpl.Emit`), which isn't mirrored yet.
- `m_worker_stack_size` is carried but nothing reads it.
- The differential test calls `OnResolveAssembly` directly; the event subscription itself is covered by the
  behaviour test alone.
