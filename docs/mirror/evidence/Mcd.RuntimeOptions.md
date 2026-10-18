# Mcd.RuntimeOptions

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/RuntimeOptions.cs`; tests: `Quire.Mpl.Tests.Mcd.RuntimeOptionsTests`,
  `Quire.Differential.Tests.RuntimeOptionsDifferentialTests`

Covers `Mcd.RuntimeOptions`, its private nested `RuntimeOptionsInitializer`, and `Mcd.UnitSystemGroup`. A
`RuntimeOptions` instance holds one worksheet's calculation options in public fields: eight flags (strict
singularity checking and strings indexed from origin on, the rest off), the mangled-identifier options, the unit
system and unit-system group (both the built-in default SI), and an options number one past the static counter.
`SyncToThread` copies them into thread-static fields of the same names without the `m_` prefix, which the runtime
reads. `s_default_unit_system` is Mathcad's SI: eight base units (kilogram, meter, second, ampere, kelvin, candela,
mole, base currency) and fourteen derived defaults. A `UnitSystemGroup` (a `MarshalByRefObject`) maps options
numbers to unit systems, the one it was built with under key 0. Every member below is differential-verified for the
cases in "Test cases", except the initializer.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `UnitSystemGroup.ctor(UnitSystem)` | 0x060005e1 | `11d63d09` | A map holding the system under key 0; the group's name is the system's. A null system throws `NullReferenceException` (after the map is made). | High | Verified |
| `UnitSystemGroup.Name` | 0x060005e2 | `a1fda1ae` | The name taken at construction; later changes to the members don't affect it. | High | Verified |
| `UnitSystemGroup.findMember(int)` | 0x060005e3 | `94bb451c` | The member under that key, or null. | High | Verified |
| `UnitSystemGroup.changeMember(int, Unit[], Unit[])` | 0x060005e4 | `b7b62f78` | Sets the member's basis, then its defaults; does nothing when there is no such member. Setter exceptions propagate. | High | Verified |
| `UnitSystemGroup.addMember(UnitSystem)` | 0x060005e5 | `edfc7ab4` | Adds the system under its `RO_number` unless that key is taken, in which case the existing member stays. | High | Verified |
| `UnitSystemGroup.removeMember(int)` | 0x060005e6 | `6475e269` | Removes the key if present; key 0 can be removed too. | High | Verified |
| `UnitSystemGroup.resetMember(int)` | 0x060005e7 | `d480ecf2` | For a key other than 0 whose member exists, and while member 0 exists, gives it member 0's current basis and defaults (the same arrays). Otherwise does nothing. | High | Verified |
| `UnitSystemGroup.Reset(int)` | 0x060005e8 | `dc389374` | Calls `UnitSystem.Reset` on the member under the key, or else on member 0, and returns it; null when neither exists. | High | Verified |
| `UnitSystemGroup.addCopyWithRONumber(int)` | 0x060005e9 | `43372673` | The existing member under the key; otherwise a new system with member 0's name, code, guid and current basis and defaults (shared arrays), numbered with the key, added and returned. Null when member 0 is gone. | High | Verified |
| `SyncToThread()` | 0x0600064e | `5f3c63f1` | Under a lock on the instance, copies all twelve option fields into the calling thread's static fields, then gives the thread a `Dim` scratch vector if it has none. | High | Verified |
| `RemoveUnitSystem()` | 0x0600064f | `16288b57` | Removes this instance's options number from its unit-system group; a null group throws `NullReferenceException`. | High | Verified |
| `SaveThreadOptions()` | 0x06000650 | `9ba6e537` | A new instance whose fields are the calling thread's current static options (so on a never-synced thread: all flags off, no unit system or group, number 0). The new instance's own initial group is replaced. | High | Verified |
| `.ctor()` | 0x06000651 | `0d962389` | The defaults described above; each instance gets a new group around the default SI. Doesn't change the counter. | High | Verified |
| `.cctor` | 0x06000652 | `79eec023` | Sets restricted mode off and the counter to 0, builds the default SI system, then runs the initializer, which syncs default options to the initialising thread and increments the counter. | High | — (type initializer; not counted) |
| `RuntimeOptionsInitializer.ctor()` | 0x060007e7 | `a01bce3f` | Creates default options, syncs them to the calling thread (including `Dim.s_vec` if absent) and increments `s_ro_counter`. | High | Verified |

**Type initializer.** Quire has no explicit static constructor; the static field initializers, declared in this
order with the initializer instance last, are the `.cctor`. The mirror tooling doesn't count type initializers, so
its row carries no status attribute; its effects are compared through the statics and the fresh-thread scenario
below. The method gate's direct case invokes both type initializers, which the runtime does not rerun, and compares
the statics they left (every unit of the default system in full). Because the initializer increments the counter
after building the first instance, instances created later have options number 2.

Fields: every public instance and thread-static option field, `s_restricted_mode`, `s_ro_counter` and `s_default_unit_system` are Verified (read directly by `RuntimeOptionsDifferentialTests`); `s_init` and `UnitSystemGroup`'s private fields are Tested.

## Test cases

**Observed** (`RuntimeOptionsDifferentialTests`, against the installed `mplrun.dll`; outcomes compared as text):
- on a fresh thread for either side: `s_restricted_mode`, `s_ro_counter`, the default system's units, code and guid;
  the thread's static options and `Dim.s_vec` before any sync (unset); `SaveThreadOptions` on that thread; a new
  instance's fields (flags, mangled identifiers, unit system being the default, group with member 0 the default,
  options number); then all flags inverted, mangled identifiers set to 2 and the number to 17, `SyncToThread`, and
  the thread's fields and scratch-vector length after it; `SaveThreadOptions` again and whether its group is the
  thread's; `addCopyWithRONumber(17)` on the instance's group, `RemoveUnitSystem` and the group afterwards, and
  `RemoveUnitSystem` with a null group;
- direct `RuntimeOptionsInitializer.ctor()` invocation, with the full static/thread option state, `s_ro_counter`,
  and `Dim.s_vec` compared before and after on both artifacts;
- a group script: `Name`, `findMember` of 0 and a missing key, `addMember` of a new and of a duplicate key,
  `changeMember` of an existing member with a one-unit basis (the padded basis throws), of the same member with the
  default SI basis and a one-unit default list, and of a missing member, `resetMember` of a member and of 0,
  `Reset` of a member and of a missing key (falling back to member 0), `addCopyWithRONumber` of an existing and a
  new key and whether the copy shares member 0's basis, `removeMember` of a missing key and of 0, then
  `resetMember`, `Reset` and `addCopyWithRONumber` without member 0, and a group built from null;
- `new RuntimeOptions()` with the default system in full, first in its test: when that test runs on its own (as
  mutant runs of the initializer do) this is the first touch of either type, so a failing initializer is a compared
  `TypeInitializationException`; in a full run another test may initialize the types first. Then `.cctor` invoked
  directly on both sides (a no-op once initialised): restricted mode, the counter, the initializer, every unit of
  the default system and the calling thread's options and group.

**Inferred** (`RuntimeOptionsTests`): defaults of a new instance, only synced threads having options, saving
copying the thread fields, and group members keyed by options number.

## Open

- The thread-static options (the unit system included) and the `Dim` scratch vector exist only on the thread that
  ran the type initializer or that called `SyncToThread`, as in the original. Quire calls an internal
  `RuntimeThread.Ensure()` (`src/Quire.Mpl/Internal/RuntimeThread.cs`), which syncs default options when the
  calling thread has no unit system, from the `BuiltinUnits` type initializer, `WorksheetEngine.CalculateNow` and
  the test bases. Mathcad's own hosting is assumed to sync the UI thread before evaluation; that assumption isn't
  verified.
- Restricted mode (`s_restricted_mode`) and the mangled-identifier options are carried but not exercised by any
  behaviour.
- Concurrent `SyncToThread` on one instance (the lock) and `MarshalByRefObject` use of the group were not tested.
