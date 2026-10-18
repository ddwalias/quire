# Mpl.Run.AppException

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Run/AppException.cs`; tests: `Quire.Mpl.Tests.Run.AppExceptionTests`,
  `Quire.Differential.Tests.AppExceptionDifferentialTests`

Covers `Mpl.Run.AppException`, `AbortRequestedException` and `NonCriticalException`. `AppException` is a
serializable, COM-visible `ApplicationException` with two private uints: the offending argument's index
(`uint.MaxValue` for none) and a handle packing a region (high 16 bits) and tag (low 16). Messages can carry both
in text: `tag>message` wraps the error in one with that handle, and `message@argument` sets the argument; the
delimiters are the public static chars `errorTagDelimiter` (`>`) and `errorArgDelimiter` (`@`). Every constructor
resets the calling thread's Mpl stack budget. `NonCriticalException` is a separate `ApplicationException` whose
message is one of the static reason names `Default`, `Unexpected` and `ArgumentIsNull`. Every member below is
differential-verified for the cases in "Test cases", except `NonCriticalException`'s type initializer.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.ctor(string)` | 0x060003c5 | `7836461f` | No inner exception, no argument, handle 0. | High | Verified |
| `.ctor(string, Exception)` | 0x060003c6 | `a5c56c5e` | With an inner exception (null allowed); no argument, handle 0. | High | Verified |
| `.ctor(string, uint)` | 0x060003c7 | `e4639fc1` | With an argument; handle 0. | High | Verified |
| `.ctor(Exception, uint)` | 0x060003c8 | `aeb111c5` | Wraps the inner exception, taking its message, with the given handle and no argument; a null inner throws `NullReferenceException`. | High | Verified |
| `.ctor(string, Exception, uint)` | 0x060003c9 | `0102bf04` | Inner exception and handle; no argument. | High | Verified |
| `.ctor(string, uint, uint)` | 0x060003ca | `34860223` | Argument and handle. | High | Verified |
| `.ctor(string, Exception, uint, uint, uint)` | 0x060003cb | `663487a0` | Message, inner and argument with handle 0, then sets `Tag`, then `Region`, so tags over 16 bits carry into the region before it is added. | High | Verified |
| `.ctor(string, Exception, uint, uint)` | 0x060003cc | `d7858594` | The full form: stores argument and handle, then resets the thread's stack budget to `Rts.m_max_stack`. | High | Verified |
| `Region` (get) | 0x060003cd | `0f9cc0a3` | The handle's high 16 bits. | High | Verified |
| `Region` (set) | 0x060003ce | `3c96dc56` | Adds the value shifted left 16 to the handle (wrapping) rather than replacing the high bits, so it is exact only when the region was 0. | High | Verified |
| `Tag` (get) | 0x060003cf | `b845d988` | The handle's low 16 bits. | High | Verified |
| `Tag` (set) | 0x060003d0 | `60fe3e07` | Clears the low half and adds the value, so values past 16 bits carry into the region. | High | Verified |
| `Handle` | 0x060003d1 | `ca4aff1c` | The packed handle. | High | Verified |
| `Argument` | 0x060003d2 | `b3bc709d` | The argument index. | High | Verified |
| `EfiMessage` | 0x060003d3 | `ea064b68` | The message, prefixed by the argument and a dot when there is one (`3.msg`). | High | Verified |
| `asAppException(Exception)` | 0x060003d4 | `f3ef1e72` | The first `AppException` in the exception's inner chain (the exception itself included); otherwise a new wrapper with handle 0. Null throws `NullReferenceException` from the wrapper. | High | Verified |
| `Raise()` | 0x060003d5 | `a6bebde4` | Throws "unknown mpl exception". | High | Verified |
| `Raise(object[], uint)` | 0x060003d6 | `cf772325` | Null or empty: `Raise("error", handle)`. One element: its text. More: the elements' texts joined by tabs (null elements after the first as empty). Raised through `Raise(string, uint)` with the handle passed as the argument, so tags are parsed. A null first element throws `NullReferenceException`. | High | Tested |
| `ChainTaggedError(string, uint)` | 0x060003d7 | `a1321499` | When the first `>` is past position 0, the text before it parses as a uint (current culture, integer style: leading/trailing white space and a leading sign allowed) and something follows it, returns the error for the rest wrapped in one with that tag as handle; tags therefore nest outward and the argument lands on the innermost. Otherwise a plain error with the message and argument. | High | Verified |
| `ChainTagAndArgument(string)` | 0x060003d8 | `3ad6f13e` | When the first `@` is past position 0, not last, and the text after it parses as a uint, splits it off as the argument; then `ChainTaggedError`. Otherwise `ChainTaggedError` with no argument. | High | Verified |
| `Raise(object)` | 0x060003d9 | `6072a6e1` | `Raise(string)` of the value's `Convert.ToString` (null becomes empty). | High | Verified |
| `Raise(string)` | 0x060003da | `a7fafbe5` | Throws `ChainTagAndArgument(message)`; null throws `NullReferenceException`. | High | Verified |
| `Raise(string, uint)` | 0x060003db | `c7b38f38` | With argument `uint.MaxValue`, the message may carry one (`ChainTagAndArgument`); otherwise only tags are parsed and the given argument is used. | High | Verified |
| `Raise(string, uint, uint)` | 0x060003dc | `7a3e9abe` | Throws a plain error with that argument and handle; nothing is parsed. | High | Verified |
| `Raise(string, Exception)` | 0x060003dd | `a8c20b80` | Throws an error with the inner exception; nothing is parsed. | High | Verified |
| `Raise(Exception, uint)` | 0x060003de | `45d4ebcf` | Throws a wrapper of the inner exception with the handle. | High | Verified |
| `FatalException(string)` | 0x060003df | `d12d71e4` | Throws a plain error with the message (unparsed). | High | Verified |
| `StackOverflowException()` | 0x060003e0 | `634c0ece` | `FatalException("stack overflow")`. | High | Verified |
| `ProbeStack()` | 0x060003e1 | `9e810a10` | Decrements the thread's stack budget; once it goes negative, throws "stack overflow" (whose constructor resets the budget). Then throws `AbortRequestedException` if the thread's registered cancellation source has been cancelled (reported once, see `Rts.IsCancellationRequested`). | High | Verified |
| `RestoreStack()` | 0x060003e2 | `38fe43c5` | Increments the thread's stack budget. | High | Verified |
| `.cctor` | 0x060003e3 | `da48185d` | Sets the delimiters to `>` and `@`. | High; compared by rerunning the type initializer and rendering both delimiters | Verified |
| `AbortRequestedException.ctor()` | 0x060003e4 | `4902bc06` | An `AppException` "abort requested" with no argument and handle 0. | High | Verified |
| `NonCriticalException.ctor()` | 0x060003e5 | `522686e1` | Message `Default`. | High | Verified |
| `NonCriticalException.ctor(string)` | 0x060003e6 | `a3b4dbf7` | The given message (null gives the framework's default message). | High | Verified |
| `NonCriticalException.cctor` | 0x060003e7 | `00a3e423` | Sets the reason names `Default`, `Unexpected` and `ArgumentIsNull` to themselves. | High | — (type initializer; not counted) |

**Type initializers.** Quire writes the static fields with initializers and no explicit static constructor; the
mirror tooling doesn't count type initializers, so those rows carry no status attribute. `AppException.cctor` is
compared directly by rerunning it; for `NonCriticalException.cctor` the differential test compares the fields it sets.

**Constructor order.** The two-parameter constructors `(string, Exception)` and `(string, uint)` have the same IL size;
their rows follow declaration order, which matches Quire's source order for all eight.

Fields: the two delimiters and `NonCriticalException`'s three reason names are Verified. `m_argument` and `m_handle`
are Verified from the direct constructor, property, and mutation comparisons: `Dump` renders both public values
(`Argument` and `Handle`) for every compared exception, including inner exception chains.

## Test cases

**Observed** (`AppExceptionDifferentialTests`, against the installed `mplrun.dll`; each exception compared as type,
message, argument, handle, region, tag, `EfiMessage` and its inner chain):
- every `AppException` constructor with messages `"m"`, `""` and null, null and non-null inner exceptions, and every
  pair from the arguments/handles 0, 1, 3, 0xFFFF, 0x10000, `uint.MaxValue − 1` and `uint.MaxValue` (as argument,
  handle, tag and region); `(Exception, uint)` with a null inner;
- the `Region` and `Tag` setters on each of those handles with values 0, 1, 0xFFFF, 0x10000 and 0x12345;
- `AbortRequestedException`, `NonCriticalException()` and with `"x"` and null, the three reason names and the two
  delimiters;
- `Raise()`; `Raise(string)`, `Raise(object)` and `ChainTagAndArgument` on 29 messages (empty, plain, non-numeric and
  numeric tags, empty rest, nested tags, tags with an argument, arguments at 0, empty, non-numeric, −1,
  `uint.MaxValue` and one past it, repeated `@`, `@` before `>`, padded, signed and zero-padded tags, `,` grouping,
  Arabic-Indic digits, a tab and a tab-separated dimension error); `Raise(string, uint)`, `Raise(string, uint, uint)`
  and `ChainTaggedError` on each with the seven arguments;
- `Raise(object)` of 12.5 and null, `Raise(string)` of null; `Raise(object[], uint)` on empty, one-, four- and
  two-element (tagged) arrays and `[null]` with arguments 0, 4 and `uint.MaxValue`, and on a null array;
  `Raise(string, Exception)`, `Raise(Exception, uint)`, `FatalException` and `StackOverflowException`;
- `asAppException` on a plain exception, one with an `AppException` inside, an `AppException`, and null;
- on a fresh thread: up to 260 `ProbeStack` calls until the first failure and the budget then, two `RestoreStack`s,
  a new exception resetting the budget, and probes with a registered cancellation source before and after it is
  cancelled, and once more.

**Inferred** (`AppExceptionTests`): tags nesting outward with the argument on the innermost error, malformed tags and
arguments staying in the message, the adding `Region` and `Tag` setters, `Raise` joining messages with tabs, and
probing spending the stack budget and checking cancellation.

## Open

- Tag and argument parsing use the current culture's uint parsing; only the test process's culture was compared.
- `Raise(object[], uint)`: its closing brace never runs, since every path returns a call that always throws (as in
  the original); the branch-coverage gate reports that line as uncovered.
- Binary serialization and the COM class interface (`AutoDual`, fixed GUID) are declared as in the original but not
  exercised.
