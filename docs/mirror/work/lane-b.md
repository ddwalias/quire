# Lane B — Quire.Mpl.Emit

- Owner: Lane B (idiom cleanup).
- Branch: `mirror/b-mpl-emit-idiom-202610231735`.
- Worktree: `C:/Projects/Quire-wt/lane-b-mpl-emit`.
- Acceptance: `python tools/validation/release_il.py --base main --project Quire.Mpl.Emit` must PASS with 682 methods exact.

## Batch 1 — goto/label removal

- Scope: `HandleResolvers.cs`, `MethodHandleResolver.ResolveMethodBase.cs`, `MethodHandleResolver.ResolveMethodInfo.cs`, `MethodHandleResolver.ResolveConstructor.cs`, `Logger.ProcessLogQueue.cs`. 23 gotos removed; switch type patterns and `when` guards replace the labels; `ProcessLogQueue` is two nested `while` loops.
- Report: `.mirror-cache/release-il-83e6d40fb882/release-il.json` (PASS, 682 checked).
- Failed idiomatic forms: none.

## Batch 2 — decompiler-style local and parameter names

- Scope: digit-suffixed locals/parameters/lambda parameters in 19 hand-written files (`value2`, `item4`, `type6`, `methodBase5`, `handler3`, `num2`, `arg1`, and others). 274 renames, applied by line.
- Excluded: `Probe0`/`Probe1`/`Probe_Int32_Object` (probe names, not locals); the `v10` and `net10.0` strings; `kernel32`.
- Report: `.mirror-cache/release-il-9526d9e04f7b/release-il.json` (PASS, 682 checked).
- Failed idiomatic forms: none.

## Batch 3 — formatting and ILSpy-isms

- Scope: tab indentation and misnested blocks in `ILCodeConverter.Members.cs`, `CustomAttributeBuilder.EmitValue.cs`, `MetadataReferenceTracker.FMethodReference.cs`, `MetadataReferenceTracker.IConstructorReference.cs`, `ModuleBuilder.BAddTargetFrameworkAttribute.cs`; blank-line runs removed; method-local declarations moved to first use in `MetadataReferenceTracker.FMethodReference.cs` and `MetadataReferenceTracker.IConstructorReference.cs`.
- Failed idiomatic forms (kept, with a comment in the source):
  - `LocalBuilder.IsConstructor` / `IsMethod`: `is` instead of `as … != null` fails. The report says Mathcad calls `ConstructorInfo::op_Inequality` / `MethodInfo::op_Inequality`, which `is` drops.
  - `LoggerAssistant.GetFullNameWithAssembly`: pattern variables with formatters declared in each branch fails ("Release IL differs").
  - `SignatureHelper.MethodSignatureEnconder` (ParameterInfo overload): `?:` and `?.` in place of the null checks fails ("Release IL differs").
  - `StackBehaviourExtensions.GetPopCount`: pattern variables with `&&` short-circuit fails ("Release IL differs").
  - The last three were restructured together and not isolated one change at a time; the comments state only what was tested.
- Report: `.mirror-cache/release-il-e5f0992bcc34/release-il.json` (PASS, 682 checked).

## Batch 4 — null checks

- Scope: `(object)x == null` sites. The assignment counted 18; batch 1 already removed five of them with the switch conversions, leaving 13 (12 in `ILCodeConverter.Members.cs`, 1 in `MetadataReferenceTracker.JFieldReference.cs`). All 13 changed to `is null`.
- Failed idiomatic forms: none.
- Report: `.mirror-cache/release-il-a53eb31d5796/release-il.json` (PASS, 682 checked).

## Batch 5 — file-order prefixes

- Kept the prefixes and added one comment at the top of the first file in each set (`MetadataReferenceTracker.AState.cs`, `ModuleBuilder.AppendMetadata.cs`).
- Gate test, prefixes removed from `MetadataReferenceTracker.*.cs` (letters stripped): FAIL, 14 failures ("method removed", "calls differ", "no Mathcad counterpart" on closure display classes). Report: `.mirror-cache/release-il-80e268e9948f/release-il.json`.
- Gate test, `ModuleBuilder.AZDefineType.cs` and `BAddTargetFrameworkAttribute.cs` renamed without their letters: FAIL. Report: `.mirror-cache/release-il-19588f32948a/release-il.json`.
- Both renames were reverted. Final report: `.mirror-cache/release-il-c70255b5e22b/release-il.json` (PASS, 682 checked).

## Remaining mismatches

None reported at batch 5.

## Batch 6 — Logger static constructor

- Owner: Lane B. Branch: `mirror/b-mpl-emit-cctor-202610232359`. Worktree: `C:/Projects/Quire-wt/lane-b-mpl-emit-cctor`.
- Claimed: `Logger::.cctor` (mplasb 0x06000153) and its `<>c` closure `<.cctor>b__15_0` (0x06000297, `<>c..ctor`, `<>c..cctor`).
- Member moves:
  - Static fields with initializers `_lock`, `path1`, `_logFileName`, `_isEnabled`, `_logQueue`, `_logSignal` leave `Generated/` and move into `Logger.BStaticConstructor.cs`. `_logThread` has no initializer and stays generated.
  - `_isRunning` and `GetCurrentThreadId` leave `Mirrored/System.Reflection.Emit.Quire.Mpl.Logger.cs` (file removed) and move into `Logger.BStaticConstructor.cs`.
  - `Logger.Properties.cs` is renamed `Logger.AProperties.cs` so it compiles before the constructor.
- Compile order: partials sort by relative path. The constructor's ordinal (15) counts `_logThread` (generated), the six property members, and the eight members in `B`, so `Log`, `ProcessLogQueue` and `Shutdown` must sort after `B`.
- Result: `Logger::.cctor` and `<.cctor>b__15_0` exact; the other Logger members stay exact. Idiomatic form matched on the first try; the only unusual shape is the file-name letters A and B.
- Report: `.mirror-cache/release-il-d70f8d14531d/release-il.json` (PASS, 686 checked, 0 failures).
- Remaining mismatches: none.
