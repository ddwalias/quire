#!/usr/bin/env python3
"""Fast, advisory comparison check for changed Quire.Mpl methods.

Examples:
    python3 tools/validation/quick_check.py
    python3 tools/validation/quick_check.py --method-type Quire.Mpl.Run.DynamicType
    python3 tools/validation/quick_check.py --method 'Quire.Mpl.Run.DynamicType::NameMangle`0(...)'
"""
import argparse
import atexit
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import time
import uuid

import branch_coverage
import method_coverage
import mutate
import validate


ROOT = validate.ROOT
PROJECT = "Quire.Focused.Differential.Tests"
TEST_PROJECT = ROOT / "tests" / PROJECT / f"{PROJECT}.csproj"
METHOD_AUDIT = ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj"
STRUCTURE_PROJECT = ROOT / "src/Quire.Mpl/Quire.Mpl.csproj"
BASELINE = ROOT / "docs/mirror/validation-baseline.json"
BUILD_FLAGS = ["-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false",
               "-p:EnableSourceControlManagerQueries=false"]


def run(command, *, cwd=None, env=None, timeout=600, quiet=False):
    result = subprocess.run(command, cwd=cwd, env=env, capture_output=quiet, text=True, timeout=timeout)
    if result.returncode:
        detail = (result.stderr or result.stdout)[-4000:] if quiet else ""
        raise RuntimeError(f"command failed ({result.returncode}): {' '.join(map(str, command))}\n{detail}")
    return result.stdout if quiet else ""


def git(*args):
    return run(["git", *args], cwd=ROOT, quiet=True).strip()


def changes_since(base):
    """Current branch/worktree changes since its fork point from the requested base branch."""
    merge_base = git("merge-base", base, "HEAD")
    if not merge_base:
        raise ValueError(f"no merge base found for {base}")
    return git("diff", "-z", "--no-renames", "--name-status", merge_base)


def load_inventory(path):
    return json.loads(path.read_text())


def method_name(row):
    return f"{row['type']}::{row['signature']}"


def class_filter(classes):
    """VSTest filter for the prior comparison classes selected by mutation's method/case expansion."""
    return "|".join(f"FullyQualifiedName~{name}" for name in sorted(classes))


def select_methods(inventory, baseline, args, changed_names, sources):
    methods = inventory["methods"]
    previous_tests = baseline.get("tests") or {}
    old_stages = baseline.get("methods") or {}
    for row in methods:
        key = method_name(row)
        row["cases"] = [{"test": f"Quire.Differential.Tests.{test_class}.PriorRun"}
                        for test_class in previous_tests.get(key, [])]
        row["validation"] = old_stages.get(key, "missing")

    if args.method:
        by_key = {method_name(row): row for row in methods}
        absent = sorted(set(args.method) - by_key.keys())
        if absent:
            raise ValueError("method key not in current MethodAudit inventory: " + ", ".join(absent))
        selected = set(args.method)
        notes = ["explicit method keys"]
    elif args.method_type:
        selected = {method_name(row) for row in methods if row["type"] in set(args.method_type)}
        unknown = set(args.method_type) - {row["type"] for row in methods}
        if unknown:
            raise ValueError("type absent from MethodAudit inventory: " + ", ".join(sorted(unknown)))
        notes = ["explicit method types"]
    else:
        baseline = dict(baseline)
        baseline.setdefault("schema", 4)
        reasons, notes = validate.changed_methods(inventory, changed_names, baseline,
                                                   sources=sources, test_types=inventory.get("test_types"))
        selected = set(reasons)
    if not selected:
        return set(), notes
    # Use mutation's method/test expansion so compiler-generated comparison bodies follow their handwritten owner.
    chosen = mutate.select(inventory, set(), selected)
    expanded = {method_name(row) for row, _ in chosen} | selected
    return expanded, notes


def il_label(row):
    if row.get("exact_match") is True:
        return "exact IL"
    if row.get("structure_matches") is True:
        return "structure only"
    return "mismatch"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("base", nargs="?", default="main", help="branch/revision to diff against (default: main)")
    parser.add_argument("--method-type", action="append", default=[], help="explicit declaring type; repeatable")
    parser.add_argument("--method", action="append", default=[], help="exact Type::signature method key; repeatable")
    parser.add_argument("--dotnet", default="dotnet")
    parser.add_argument("--timeout", type=int, default=600, help="timeout in seconds per build/test command")
    parser.add_argument("--results", type=Path, help="new result directory (defaults to a temporary directory)")
    args = parser.parse_args()
    if args.method and args.method_type:
        parser.error("use --method or --method-type, not both")
    if args.timeout <= 0:
        parser.error("--timeout must be positive")

    started = time.monotonic()
    dotnet = validate.resolve_dotnet(args.dotnet, ROOT)
    output = args.results.resolve() if args.results else Path(tempfile.mkdtemp(prefix="quire-quick-check-"))
    if args.results:
        output.mkdir(parents=True, exist_ok=False)
    output.mkdir(parents=True, exist_ok=True)
    tool_dir = Path(tempfile.mkdtemp(prefix="quire-quick-audit-tool-"))
    assembly_dir = TEST_PROJECT.parent / "bin" / "Debug" / "net10.0"
    structure_dir = STRUCTURE_PROJECT.parent / "bin" / "Release" / "net10.0"
    atexit.register(shutil.rmtree, tool_dir, ignore_errors=True)
    test_assembly = assembly_dir / "Quire.Differential.Tests.dll"
    install = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"
                   if os.name == "nt" else "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0"))
    if not (install / "mplrun.dll").is_file():
        raise ValueError(f"Mathcad original assembly not found: {install / 'mplrun.dll'}; set MATHCAD_DIR")

    global_json = (ROOT / "global.json").read_bytes() if (ROOT / "global.json").is_file() else b"{}\n"
    with validate.sdk_directory(global_json) as sdk:
        for project, config, out in ((METHOD_AUDIT, "Release", tool_dir), (TEST_PROJECT, None, None),
                                     (STRUCTURE_PROJECT, "Release", None)):
            command = [dotnet, "build", str(project), *(["-c", config] if config else []), *BUILD_FLAGS]
            if out is not None:
                command += ["-o", str(out)]
            output_log = run(command, cwd=sdk, timeout=args.timeout, quiet=True)
            with (output / "build.log").open("a") as log:
                log.write(output_log)

    inventory_path = output / "method-inventory.json"
    scan = [dotnet, str(tool_dir / "Quire.Tools.MethodAudit.dll"), "--repo", str(ROOT),
            "--assemblies", str(assembly_dir), "--structure-assemblies", str(structure_dir),
            "--install", str(install), "--output", str(inventory_path), "--project", "Quire.Mpl"]
    run([*scan, "--inventory-only"], cwd=ROOT, timeout=args.timeout)
    inventory = load_inventory(inventory_path)
    baseline = json.loads(BASELINE.read_text())
    sources = validate.test_sources()
    name_status = changes_since(args.base)
    selected, notes = select_methods(inventory, baseline, args, name_status, sources)
    if not selected:
        print("No changed methods selected; tests: not run")
        return 0

    changed_types = sorted({name.split("::", 1)[0] for name in selected})
    cases = mutate.select(inventory, set(), selected)
    selected_classes = {validate.test_class(case["test"]) for method, _ in cases for case in method.get("cases", [])}
    scoped_inventory_path = output / "method-inventory-scoped.json"
    scoped_scan = list(scan)
    scoped_scan[scoped_scan.index("--output") + 1] = str(scoped_inventory_path)
    for type_name in changed_types:
        scoped_scan += ["--type", type_name]
    run(scoped_scan, cwd=ROOT, timeout=args.timeout)
    inventory = load_inventory(scoped_inventory_path)

    if not selected_classes:
        report = method_coverage.audit(inventory, [], set(), str(uuid.uuid4()), types=changed_types)
        (output / "method-coverage.json").write_text(json.dumps(report, indent=2) + "\n")
        rows = {method_name(row): row for row in report["methods"]}
        inventory_rows = {method_name(row): row for row in inventory["methods"]}
        for name in sorted(selected):
            row, raw = rows.get(name), inventory_rows.get(name)
            print(f"{name}: {il_label(raw)}; {row['validation'] if row else 'missing'}")
        summary = {"elapsed_seconds": round(time.monotonic() - started, 3), "selected_methods": len(selected),
                   "selected_types": changed_types, "selected_test_classes": 0,
                   "passed": 0, "failed": 0, "skipped": 0,
                   "method_coverage": report["counts"], "changed_methods": sorted(selected),
                   "selection_notes": notes}
        (output / "summary.json").write_text(json.dumps(summary, indent=2) + "\n")
        print(f"Selected {len(selected)} methods; no previous comparison tests matched; results: {output}")
        print("Advisory result: NOT RUN (no known test selection)")
        return 0
    run_id = str(uuid.uuid4())
    cases_dir = output / "method-cases"
    cases_dir.mkdir()
    env = os.environ.copy()
    env.update({"QUIRE_REQUIRE_MATHCAD": "1", "QUIRE_METHOD_CASES": str(cases_dir), "QUIRE_VALIDATION_RUN": run_id})
    env.pop("QUIRE_MUTANT_CACHE", None)
    test_trx = output / "Quire.Focused.Differential.Tests.trx"
    test_command = [dotnet, "test", str(test_assembly), "--no-build", "--logger", f"trx;LogFileName={test_trx.name}",
                    "--results-directory", str(output)]
    test_command += ["--filter", class_filter(selected_classes)]
    query = run([dotnet, "msbuild", str(TEST_PROJECT), "-getProperty:TargetPath",
                 "-getProperty:TraceDataCollectorDirectoryPath", "-nologo",
                 "-p:Configuration=Release"], cwd=ROOT, timeout=args.timeout, quiet=True)
    try:
        collector = Path(json.loads(query)["Properties"]["TraceDataCollectorDirectoryPath"])
    except (ValueError, KeyError, TypeError):
        raise RuntimeError("could not locate the Code Coverage collector for the focused differential project")
    if not collector.is_dir():
        raise RuntimeError(f"Code Coverage collector directory not found: {collector}")
    with (output / "tests.uninstrumented.log").open("w") as log:
        result = subprocess.run(test_command, cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT,
                                timeout=args.timeout)
    if result.returncode and not test_trx.is_file():
        raise RuntimeError(f"focused differential tests exited {result.returncode}; inspect {output / 'tests.uninstrumented.log'}")
    test_result = validate.read_trx(test_trx)
    if sum(test_result[key] for key in ("passed", "failed", "skipped")) == 0:
        raise RuntimeError("no tests matched the selected comparison classes")
    coverage_trx = output / "coverage" / "Quire.Focused.Differential.Tests.coverage.trx"
    coverage_command = [c if not c.startswith("trx;") else f"trx;LogFileName={coverage_trx.name}" for c in test_command]
    coverage_command[coverage_command.index("--results-directory") + 1] = str(output / "coverage")
    (output / "coverage").mkdir()
    coverage_command += ["--collect", "Code Coverage;Format=cobertura", "--test-adapter-path", str(collector)]
    coverage_env = dict(env)
    coverage_env.pop("QUIRE_METHOD_CASES", None)
    coverage_env.pop("QUIRE_VALIDATION_RUN", None)
    with (output / "tests.coverage.log").open("w") as log:
        result = subprocess.run(coverage_command, cwd=ROOT, env=coverage_env, stdout=log, stderr=subprocess.STDOUT,
                                timeout=args.timeout)
    if result.returncode and not coverage_trx.is_file():
        raise RuntimeError(f"focused differential tests exited {result.returncode}; inspect {output / 'tests.coverage.log'}")
    coverage_result = validate.read_trx(coverage_trx)
    if coverage_result != test_result:
        raise RuntimeError("unfiltered and coverage test results differ; inspect both TRX files")
    passed = method_coverage.passed_tests([test_trx])
    records = []
    for path in cases_dir.glob("*.jsonl"):
        for line in path.read_text().splitlines():
            if line:
                record = json.loads(line)
                if record.get("test") in passed:
                    records.append(record)
    coverage_files = sorted((output / "coverage").rglob("*.cobertura.xml"))
    if not coverage_files:
        raise RuntimeError("focused tests passed but the coverage collector produced no Cobertura report")
    lines = branch_coverage.read_cobertura(coverage_files, str)
    report = method_coverage.audit(inventory, records, passed, run_id,
                                   types=changed_types, lines=lines)
    (output / "method-coverage.json").write_text(json.dumps(report, indent=2) + "\n")
    rows = {method_name(row): row for row in report["methods"]}
    inventory_rows = {method_name(row): row for row in inventory["methods"]}
    duration = time.monotonic() - started
    summary = {"elapsed_seconds": round(duration, 3), "selected_methods": len(selected),
               "selected_types": changed_types, "selected_test_classes": len(selected_classes),
               "passed": test_result["passed"], "failed": test_result["failed"],
               "skipped": test_result["skipped"], "coverage_passed": coverage_result["passed"],
               "method_coverage": report["counts"],
               "changed_methods": sorted(selected), "selection_notes": notes}
    (output / "summary.json").write_text(json.dumps(summary, indent=2) + "\n")
    test_count = sum(test_result[key] for key in ("passed", "failed", "skipped"))
    print(f"Selected {len(selected)} methods; ran {test_count} tests in {len(selected_classes)} comparison classes "
          f"({test_result['passed']} passed, "
          f"{test_result['failed']} failed, {test_result['skipped']} skipped) in {duration:.1f}s")
    for name in sorted(selected):
        row = rows.get(name)
        raw = inventory_rows[name]
        state = row["validation"] if row else "missing"
        print(f"{name}: {il_label(raw)}; {state}")
    print(f"Method coverage: {report['counts']}; results: {output}")
    print("Advisory result: " + ("PASS" if test_result["failed"] == 0 and test_result["skipped"] == 0 else "FAIL"))
    return 0 if test_result["failed"] == 0 and test_result["skipped"] == 0 else 1


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (OSError, ValueError, RuntimeError, subprocess.TimeoutExpired, json.JSONDecodeError) as error:
        print(f"quick check failed: {error}", file=sys.stderr)
        raise SystemExit(2)
