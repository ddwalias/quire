"""Run maintained test projects and reject empty, skipped, aborted or failing runs."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import shutil
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


def digest(paths, root):
    """SHA-256 of each file, keyed by its path relative to `root`."""
    return {Path(p).relative_to(root).as_posix(): hashlib.sha256(Path(p).read_bytes()).hexdigest() for p in paths}


def files_under(*directories):
    return sorted(p for d in directories if Path(d).is_dir() for p in Path(d).rglob("*") if p.is_file())


def tree_state(root):
    """The working tree relative to its index and commit: git's status (which notices changed size, mtime, ctime or
    inode) plus the content of every file it lists, so re-editing an already modified file is noticed too."""
    status = subprocess.run(["git", "status", "--porcelain=v1", "-z", "--untracked-files=all"], cwd=root, check=True,
                            capture_output=True).stdout.decode()
    entries = sorted(e for e in status.split("\0") if e)
    listed = [Path(root) / e[3:] for e in entries if len(e) > 3 and (Path(root) / e[3:]).is_file()]
    return {"status": entries, "content": digest(listed, root)}


class Seal:
    """Fingerprints of what code under test must not change: the whole working tree (sources the lint and audit read
    afterwards, maintainer-owned files) and the built test outputs. Agent code runs inside every dotnet test, so each
    run is followed by a check."""

    def __init__(self, root, outputs=()):
        self.root, self.outputs = Path(root), [Path(o) for o in outputs]
        self.tree, self.built = self.take()

    def take(self):
        return tree_state(self.root), digest(files_under(*self.outputs), self.root)

    def verify(self, after):
        tree, built = self.take()
        if tree != self.tree:
            changed = sorted(set(tree["status"]) ^ set(self.tree["status"])) or sorted(
                k for k in set(tree["content"]) | set(self.tree["content"]) if tree["content"].get(k) != self.tree["content"].get(k))
            raise ValueError(f"{after} changed the working tree: {', '.join(changed[:10])}")
        changed = sorted(k for k in set(built) | set(self.built) if built.get(k) != self.built.get(k))
        if changed:
            raise ValueError(f"{after} changed built test outputs: {', '.join(changed[:10])}")


def changed_types(coverage, since, git_output, previous=None):
    """Types to mutate after changes since `since`: those whose Quire.Mpl source changed and those whose comparisons
    live, or lived according to the baseline's `previous` method -> test classes map, in a changed differential test
    file (a method whose cases were cut from a file is mutated against what remains)."""
    names = git_output("diff", "-z", "--no-renames", "--diff-filter=AM", "--name-only", since)
    if names is None:
        raise ValueError(f"cannot diff against {since}")
    names = [n for n in names.split("\0") if n]
    root = ROOT.resolve().as_posix() + "/"
    sources = {}
    for method in coverage["methods"]:
        for document in method.get("source") or []:
            file = Path(document["file"]).as_posix()
            if not file.startswith(root):
                raise ValueError(f"PDB source {file} of {method['type']} lies outside the repository; "
                                 "path maps and #line directives would hide changes from mutation")
            sources.setdefault(file[len(root):], set()).add(method["type"])
    types = set()
    for name in names:
        # A file with no method bodies (enums, interfaces, attributes) has no sequence points and nothing to mutate.
        if name.lower().startswith("src/quire.mpl/") and name.lower().endswith(".cs"):
            types |= sources.get(name, set())
    # The lint holds each differential test file to one top-level class named after it; nested classes count as it.
    test_classes = {Path(n).stem.lower() for n in names
                    if n.lower().startswith("tests/quire.differential.tests/") and n.lower().endswith(".cs")}
    types |= {m["type"] for m in coverage["methods"]
              if any(test_class(case["test"]) in test_classes for case in m.get("cases", []))}
    known = {m["type"] for m in coverage["methods"]}
    types |= {name.split("::", 1)[0] for name, classes in (previous or {}).items()
              if test_classes & set(classes) and name.split("::", 1)[0] in known}
    return types


def test_class(test):
    """Top-level class of `Namespace.Class[+Nested].Method`, lower-cased."""
    return test.rsplit(".", 1)[0].rsplit(".", 1)[-1].split("+")[0].lower()


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
    parser.add_argument("--mutate", action="store_true", help="IL-mutate compared methods of the --method-type types")
    parser.add_argument("--mutate-since", metavar="REV", help="IL-mutate compared methods of types whose source changed since REV")
    parser.add_argument("--jobs", type=int, default=2, help="parallel mutant test runs (default: 2)")
    parser.add_argument("--baseline", type=Path, help="ratchet baseline (default: docs/mirror/validation-baseline.json)")
    parser.add_argument("--write-baseline", type=Path, help="write this run's counts as a baseline (unscoped runs only)")
    args = parser.parse_args()
    if args.timeout <= 0:
        parser.error("--timeout must be positive")
    if args.results and args.results.exists():
        parser.error("--results must name a new directory")
    if args.mutate and not args.method_type:
        parser.error("--mutate needs --method-type; use --mutate-since for unscoped runs")
    if (args.mutate or args.mutate_since) and args.profile != "focused":
        parser.error("mutation runs in the focused profile")
    if args.write_baseline and args.method_type:
        parser.error("--write-baseline needs an unscoped run")
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
              "complete": False, "gate_passed": False, "projects": [], "limitations":
              "Passing comparisons establish equivalence only for executed cases. Phase also requires render, coverage-excluded checks and independent review."}
    install = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"
                   if os.name == "nt" else "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0"))
    artifact = install / "mplrun.dll"
    report["original"] = {"path": str(artifact), "sha256": hashlib.sha256(artifact.read_bytes()).hexdigest() if artifact.is_file() else None}
    holder = {}
    try:
        # Everything the verdict depends on is loaded or built before any code under test runs; afterwards the seal
        # catches tests that rewrite maintainer-owned files or built outputs on disk.
        import branch_coverage, integrity, method_coverage, mutate, ratchet  # noqa: F401
        build_flags = ["-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false"]
        tool = ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj"
        tool_dir = holder["tool"] = Path(tempfile.mkdtemp(prefix="quire-audit-tool-"))
        print("Building the method audit and test projects", flush=True)
        with (output / "build.log").open("w") as log:
            for target, extra in [(tool, ["-o", str(tool_dir)])] + [
                    (ROOT / "tests" / p / f"{p}.csproj", []) for p in projects]:
                result = subprocess.run([args.dotnet, "build", str(target), *build_flags, *extra], cwd=ROOT,
                                        stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
                if result.returncode:
                    raise ValueError(f"build of {target.name} failed; inspect build.log")
        tool_digest = digest(files_under(tool_dir), tool_dir)
        seal = Seal(ROOT, [ROOT / "tests" / p / "bin" for p in projects])
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
            command = [args.dotnet, "test", str(ROOT / "tests" / project / f"{project}.csproj"), "--no-build",
                       "--logger", f"trx;LogFileName={trx.name}", "--results-directory", str(output)]
            if differential and args.diff_filter:
                command += ["--filter", args.diff_filter]
            print(f"Running {project}; log: {output / (project + '.log')}", flush=True)
            item = {"project": project, "command": command, "trx": str(trx)}
            report["projects"].append(item)
            with (output / f"{project}.log").open("w") as log:
                result = subprocess.run(command, cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
            item["exit_code"] = result.returncode
            seal.verify(project)
            if result.returncode:
                raise ValueError(f"{project}: dotnet test exited {result.returncode}; inspect its log")
            item.update(read_trx(trx, differential and args.allow_differential_skips))
            print(f"{project}: {item['passed']} passed / {item['failed']} failed / {item['skipped']} skipped", flush=True)
            if differential:
                # Coverage instrumentation rewrites the module, so recorded method tokens would be wrong:
                # the same tests run again, unrecorded, only to measure lines and branches.
                env.pop("QUIRE_METHOD_CASES", None)
                env.pop("QUIRE_VALIDATION_RUN", None)
                coverage_trx = output / f"{project}.coverage.trx"
                coverage_command = [c if not c.startswith("trx;") else f"trx;LogFileName={coverage_trx.name}" for c in command]
                coverage_command[coverage_command.index("--results-directory") + 1] = str(output / "coverage")
                coverage_command += ["--collect", "Code Coverage;Format=cobertura"]
                item["coverage_command"] = coverage_command
                print(f"Measuring branch coverage of {project}", flush=True)
                with (output / f"{project}.coverage.log").open("w") as log:
                    result = subprocess.run(coverage_command, cwd=ROOT, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
                seal.verify(f"{project} coverage run")
                if result.returncode:
                    raise ValueError(f"{project}: coverage run exited {result.returncode}; inspect its log")
                read_trx(output / "coverage" / coverage_trx.name, args.allow_differential_skips)
        if any(p["skipped"] for p in report["projects"]):
            return 2
        from method_coverage import audit, passed_tests
        inventory_path = output / "method-inventory.json"
        if digest(files_under(tool_dir), tool_dir) != tool_digest:
            raise ValueError("the method audit build changed while tests ran")
        host = "Quire.Focused.Differential.Tests" if args.profile == "focused" else "Quire.Mirror.Tests"
        framework = "net10.0" if args.profile == "focused" else "net10.0-windows"
        scan = [args.dotnet, str(tool_dir / "Quire.Tools.MethodAudit.dll"),
                "--repo", str(ROOT), "--assemblies", str(ROOT / "tests" / host / "bin/Debug" / framework),
                "--install", str(install), "--output", str(inventory_path)]
        if args.profile == "focused":
            scan += ["--project", "Quire.Mpl"]
        report["method_commands"] = [scan]
        print("Auditing every selected method, including private methods and accessors", flush=True)
        with (output / "method-audit.log").open("w") as log:
            result = subprocess.run(scan, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
            if result.returncode:
                raise ValueError("Method inventory failed; inspect method-audit.log")
        records = [json.loads(line) for path in (output / "method-cases").glob("*.jsonl")
                   for line in path.read_text().splitlines() if line]
        from branch_coverage import read_cobertura
        cobertura = sorted((output / "coverage").rglob("*.cobertura.xml"))
        report["coverage_reports"] = [str(p) for p in cobertura]
        coverage = audit(json.loads(inventory_path.read_text()), records,
                         passed_tests([Path(p["trx"]) for p in report["projects"]]), run, types=args.method_type,
                         lines=read_cobertura(cobertura))
        (output / "method-coverage.json").write_text(json.dumps(coverage, indent=2) + "\n")
        report["method_coverage"] = coverage["counts"]
        print(f"Method coverage: {coverage['counts']}; {output / 'method-coverage.json'}", flush=True)
        from integrity import check
        integrity = check(json.loads(inventory_path.read_text()))
        (output / "integrity.json").write_text(json.dumps(integrity, indent=2) + "\n")
        report["integrity"] = {"lint": len(integrity["lint"])}
        print(f"Integrity: {report['integrity']}; {output / 'integrity.json'}", flush=True)
        import ratchet
        ratchet_report = ratchet.check(coverage, integrity, bool(args.method_type), args.profile, args.baseline or ratchet.BASELINE)
        (output / "ratchet.json").write_text(json.dumps(ratchet_report, indent=2) + "\n")
        report["ratchet"] = {"ok": ratchet_report["ok"], "regressions": ratchet_report["regressions"]}
        print(f"Ratchet: {'ok' if ratchet_report['ok'] else ratchet_report['regressions']}", flush=True)
        if args.write_baseline:
            args.write_baseline.write_text(json.dumps(ratchet_report["snapshot"], indent=2, sort_keys=True) + "\n")
        mutation_ok = True
        mutate_types = set(args.method_type) if args.mutate else set()
        if args.mutate_since:
            baseline_path = args.baseline or ratchet.BASELINE
            previous = json.loads(baseline_path.read_text()).get("tests") if baseline_path.is_file() else None
            mutate_types |= changed_types(coverage, args.mutate_since, git_output, previous)
            report["mutate_since"] = args.mutate_since
        if mutate_types:
            import mutate
            print("Mutating compared methods; each mutant reruns the differential tests", flush=True)
            host_dir = ROOT / "tests/Quire.Focused.Differential.Tests/bin/Debug/net10.0"
            mutation = mutate.run(coverage, mutate_types, host_dir / "Quire.Differential.Tests.dll", "Quire.Mpl",
                                  args.dotnet, args.jobs, args.timeout)
            seal.verify("mutation")
            (output / "mutation.json").write_text(json.dumps(mutation, indent=2) + "\n")
            report["mutation"] = {k: mutation[k] for k in ("total", "killed", "accepted_survivors", "survivors")}
            mutation_ok = mutation["complete"]
            print(f"Mutation: {mutation['killed']}/{mutation['total']} killed; survivors {len(mutation['survivors'])}", flush=True)
        report["complete"] = coverage["complete"] and integrity["complete"] and mutation_ok
        # The merge gate needs green tests, no regression and no surviving mutant; completeness is the end goal.
        report["gate_passed"] = ratchet_report["ok"] and mutation_ok
        return 0 if report["complete"] else 1
    except (OSError, ValueError, KeyError, TypeError, ET.ParseError, subprocess.TimeoutExpired) as error:
        report["error"] = str(error)
        print(str(error), file=sys.stderr)
        return 1
    finally:
        if "tool" in holder:
            shutil.rmtree(holder["tool"], ignore_errors=True)
        (output / "summary.json").write_text(json.dumps(report, indent=2) + "\n")
        print(f"Report: {output / 'summary.json'}", flush=True)


if __name__ == "__main__":
    sys.exit(main())
