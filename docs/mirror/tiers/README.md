# Tier lists (D11)

Historical scope classification only. Current ownership comes from [WORK](../WORK.md).
The A–D assembly assignments supersede tier deferrals within those four assemblies (D18);
the numbered lanes below are retired. Tier membership never establishes IL completion.

- `tier-a.txt`, `tier-a2.txt`: the Quire method keys of the Tier A and A2 members, sorted, one per line, in the format the validation baseline and MethodAudit use (``Owner::Name`G(params)->ret|IsStatic``). Properties and events are listed as their accessors; fields are never A/A2.
- `tier-b.txt`: the Quire method keys of the Tier B members, in the same format and mapped the same way: the allowlist of the member generator (tool-member-gen, not yet merged), which by default emits only members in this list; its `--include-tier-a` option widens that to Tier A/A2. It lists methods, constructors and property/event accessors only (7,396 members): MethodAudit keys methods, so the 9,606 Tier B fields are not listed. `tier-b-unmapped.txt` (222) names the B members with no Quire key, as for A.
- `types.tsv`: per Mathcad type, its Quire type, member counts per tier, the dominant tier (largest count; ties go A, A2, B, C, D, abstract) and the lane that owns it.
- **Inputs:** a scratch C# scanner (`System.Reflection.Metadata`, not committed) over the 112 in-scope Mathcad Prime 12.0.1.0 assemblies, which writes each member's body class and IL size, a 213,600-edge call graph and the type hierarchy. MethodAudit inventories of the Quire build supply the Quire keys.
- **Classification:** 60,163 members, after dropping enum values and compiler-generated members. A 3,789; A2 1,481; B 17,002; C 18,651; D 14,066; abstract 5,174. A = real bodies reached from the core roots. A2 = real bodies reached from the equation tree and not in A. B = mechanical members (fields, auto/field accessors, forwarders, base-ctor-only, const/empty/throw bodies) in live areas. C = other live bodies, plus the MPL-compiled Phase 10 libraries. D = shell, automation, Excel, Media3D, Plot3D, Whiteboard/Mathboard, OLE, `FunctionalitiesLimitation`, and the managed side of the mixed C++/CLI assemblies.
- **Roots:** call-graph closure with rapid type analysis, so a virtual call reaches overrides only in types that are constructed somewhere.
  - Core roots: every method of `iengine`, `Ptc.Calculation`, `Ptc.Calculation.Core` and `mplrun`; plus `Ptc.PersistentData*`, `Ptc.Xml`, `Ptc.IO`, `Ptc.Controls.Core.Serialization*` and `Ptc.Controls.IO*`.
  - Equation-tree roots: `Ptc.EquationEditor.Tree*` and `Ptc.EquationEditor.Equation*`.
  - C vs D is by area, not reach: a non-A/A2 member is D when it lies in Phase 8 or 9, `Ptc.Controls.Whiteboard`, an OLE, Plot3D, Whiteboard or Mathboard namespace, or `Ptc.FunctionalitiesLimitation` (`dead()` in `tiers.py`), and C otherwise. Mechanical members there are D unless the core reaches them; mixed-mode Phase 10 members are D and MPL-compiled Phase 10 members are C.
- **Lanes:** 1 `mplrun`/`iengine`; 2 `Mcd.MatVal*`, `Mcd.MatDim*`, `Mcd.mat_val*`, `Mpl.Run.Function*`; 3 `Ptc.Calculation*` and the Tier A `automationmgd` types; 4 `Ptc.PersistentData*` in `Ptc.Core`; 5 the rest of `Ptc.Core` I/O (`Ptc.Serialization`, `Ptc.Xml`, `Ptc.IO`, `Ptc.Wpf`) and any `Ptc.Core` type with A members; 6 `Ptc.Controls.Core.Serialization`, `Ptc.Controls.IO` and any `Ptc.Controls*` type with A members; 7 the `Ptc.EquationEditor` assembly; 8 any other type where B dominates. The rules apply in that order. A type with no A members that is not B-dominant has no lane; its A2 members (6 types, 24 members) stay unassigned until A2 starts.
- **Limits:**
  - The graph misses XAML/BAML wiring, reflection, MPL dynamic loading and native callbacks, so reach is a lower bound.
  - "Mechanical" is a heuristic. The exact-IL generator decides the final Tier B.
  - Members whose Quire shape differs have no key. `tier-a-unmapped.txt` (166) and `tier-a2-unmapped.txt` (81) name them as `assembly<TAB>type::member(params)`; most are `.cctor`s Quire does not declare yet, obfuscated iterator methods Quire names as explicit interface implementations, and overloads Quire lacks. Explicit interface implementations match with the interface named as Mathcad names it. They are still Tier A/A2 work: the lane that owns the type covers them. The one exception is an A2 `.cctor` in `MC14MathConversionValidation`, one of those unassigned A2 types.
- **Regenerate keys from Release only:** build production projects and MethodAudit with `-c Release`.
  Run MethodAudit against their Release output directories, then run
  `python tools/scope/tiers.py keys <inventory.json> <keys.tsv>`. Classification uses
  `python tools/scope/tiers.py classify DATA KEYS docs/mirror/tiers`, where DATA is the
  original metadata scanner output. A test host is not needed. Tier membership selects scope;
  completion comes only from the current Release IL report.
- These files hold member names and counts only: no Mathcad IL, no decompiled code.
