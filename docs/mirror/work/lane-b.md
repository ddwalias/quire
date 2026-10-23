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

## Remaining mismatches

None reported at batch 3.
