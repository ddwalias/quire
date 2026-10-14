"""Run maintained test projects and reject empty, skipped, aborted or failing runs."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import uuid
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
NS = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}


def read_trx(path, allow_skips=False):
    root = ET.parse(path).getroot()
    summary = root.find("t:ResultSummary", NS)
    results = root.findall("t:Results/t:UnitTestResult", NS)
    if summary is None or summary.get("outcome") not in ("Completed", "Passed"):
        raise ValueError("test run did not complete successfully")
    if not results:
        raise ValueError("no tests executed; check the filter and adapter")
    counts = {"passed": 0, "failed": 0, "skipped": 0}
    for result in results:
        outcome = result.get("outcome")
        if outcome == "Passed":
            counts["passed"] += 1
        elif outcome == "NotExecuted":
            counts["skipped"] += 1
        else:
            counts["failed"] += 1
    if counts["failed"] or (counts["skipped"] and not allow_skips):
        raise ValueError(f"unsuccessful test results: {counts}")
    if not counts["passed"]:
        raise ValueError("no passing tests; skips cannot establish verification")
    return counts


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--profile", choices=["focused", "phase"], default="focused")
    parser.add_argument("--method-type", action="append", default=[], help="exact type(s) for a batch method gate; default: all project methods")
    parser.add_argument("--diff-filter", help="VSTest filter for differential tests only")
    parser.add_argument("--allow-differential-skips", action="store_true",
                        help="permit some skipped cases; report is incomplete, never Verified")
    parser.add_argument("--dotnet", default="dotnet", help="path to the .NET 10 CLI")
    parser.add_argument("--timeout", type=int, default=600, help="maximum seconds per test project (default: 600)")
    parser.add_argument("--results", type=Path, help="new result directory (must not exist)")
    args = parser.parse_args()
    if args.timeout <= 0:
        parser.error("--timeout must be positive")
    if args.results and args.results.exists():
        parser.error("--results must name a new directory")
    if args.profile == "phase" and os.name != "nt":
        parser.error("phase tests require Windows and the Windows Desktop runtime; use focused for the portable runtime slice")
    if args.profile == "phase" and args.method_type:
        parser.error("phase validation must audit every method; omit --method-type")
    if args.profile == "phase" and args.diff_filter:
        parser.error("phase closure must run the full differential suite; omit --diff-filter")
    if args.profile == "phase" and args.allow_differential_skips:
        parser.error("phase validation requires differential execution; omit --allow-differential-skips")
    output = args.results.resolve() if args.results else Path(tempfile.mkdtemp(prefix="quire-validation-"))
    if args.results:
        output.mkdir(parents=True, exist_ok=False)
    projects = ["Quire.Harness.Tests", "Quire.Mpl.Tests", "Quire.Focused.Differential.Tests"]
    if args.profile == "phase":
        projects = sorted(p.parent.name for p in (ROOT / "tests").glob("*/*.csproj")
                          if p.parent.name != "Quire.Focused.Differential.Tests")
    def git_output(*git_args):
        result = subprocess.run(["git", *git_args], cwd=ROOT, text=True, capture_output=True)
        return result.stdout.strip() if result.returncode == 0 else None

    run = str(uuid.uuid4())
    report = {"run": run, "method_types": args.method_type, "revision": git_output("rev-parse", "HEAD"), "working_tree": git_output("status", "--porcelain"), "profile": args.profile, "diff_filter": args.diff_filter,
              "complete": False, "projects": [], "limitations":
              "Passing comparisons establish equivalence only for executed cases. Phase also requires render, coverage-excluded checks and independent review."}
    install = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"
                   if os.name == "nt" else "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0"))
    artifact = install / "mplrun.dll"
    report["original"] = {"path": str(artifact), "sha256": hashlib.sha256(artifact.read_bytes()).hexdigest() if artifact.is_file() else None}
    try:
        for project in projects:
            differential = "Differential" in project
            env = os.environ.copy()
            env["QUIRE_REQUIRE_MATHCAD"] = "0" if args.allow_differential_skips else "1"
            env.pop("QUIRE_METHOD_CASES", None)
            env.pop("QUIRE_VALIDATION_RUN", None)
            if differential:
                env["QUIRE_METHOD_CASES"] = str(output / "method-cases")
                env["QUIRE_VALIDATION_RUN"] = run
            trx = output / f"{project}.trx"
            command = [args.dotnet, "test", str(ROOT / "tests" / project / f"{project}.csproj"),
                       "-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false",
                       "--logger", f"trx;LogFileName={trx.name}", "--results-directory", str(output)]
            if differential and args.diff_filter:
                command += ["--filter", args.diff_filter]
            print(f"Running {project}; log: {output / (project + '.log')}", flush=True)
            item = {"project": project, "command": command, "trx": str(trx)}
            report["projects"].append(item)
            with (output / f"{project}.log").open("w") as log:
                result = subprocess.run(command, cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
            item["exit_code"] = result.returncode
            if result.returncode:
                raise ValueError(f"{project}: dotnet test exited {result.returncode}; inspect its log")
            item.update(read_trx(trx, differential and args.allow_differential_skips))
            print(f"{project}: {item['passed']} passed / {item['failed']} failed / {item['skipped']} skipped", flush=True)
        if any(p["skipped"] for p in report["projects"]):
            return 2
        from method_coverage import audit, passed_tests
        tool = ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj"
        inventory_path = output / "method-inventory.json"
        build = [args.dotnet, "build", str(tool), "-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false"]
        host = "Quire.Focused.Differential.Tests" if args.profile == "focused" else "Quire.Mirror.Tests"
        framework = "net10.0" if args.profile == "focused" else "net10.0-windows"
        scan = [args.dotnet, str(tool.parent / "bin/Debug/net10.0/Quire.Tools.MethodAudit.dll"),
                "--repo", str(ROOT), "--assemblies", str(ROOT / "tests" / host / "bin/Debug" / framework),
                "--install", str(install), "--output", str(inventory_path)]
        if args.profile == "focused":
            scan += ["--project", "Quire.Mpl"]
        report["method_commands"] = [build, scan]
        print("Auditing every selected method, including private methods and accessors", flush=True)
        with (output / "method-audit.log").open("w") as log:
            for command in (build, scan):
                result = subprocess.run(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
                if result.returncode:
                    raise ValueError("Method inventory failed; inspect method-audit.log")
        records = [json.loads(line) for path in (output / "method-cases").glob("*.jsonl")
                   for line in path.read_text().splitlines() if line]
        coverage = audit(json.loads(inventory_path.read_text()), records,
                         passed_tests([Path(p["trx"]) for p in report["projects"]]), run, types=args.method_type)
        (output / "method-coverage.json").write_text(json.dumps(coverage, indent=2) + "\n")
        report["method_coverage"] = coverage["counts"]
        report["complete"] = coverage["complete"]
        print(f"Method coverage: {coverage['counts']}; {output / 'method-coverage.json'}", flush=True)
        return 0 if report["complete"] else 1
    except (OSError, ValueError, KeyError, TypeError, ET.ParseError, subprocess.TimeoutExpired) as error:
        report["error"] = str(error)
        print(str(error), file=sys.stderr)
        return 1
    finally:
        (output / "summary.json").write_text(json.dumps(report, indent=2) + "\n")
        print(f"Report: {output / 'summary.json'}", flush=True)


if __name__ == "__main__":
    sys.exit(main())
