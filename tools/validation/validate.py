"""Run maintained test projects and reject empty, skipped, aborted or failing runs."""
import argparse
import contextlib
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

# `python -I` (how the private copy runs) leaves the script's directory off the path; the sibling modules live there.
sys.path.insert(0, str(Path(__file__).resolve().parent))

# The repository under validation; set when running from the private copy (see main).
ROOT = Path(os.environ.get("QUIRE_VALIDATOR_REPO") or Path(__file__).resolve().parents[2])
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


class Seal:
    """Content fingerprints of what the validator reads after code under test has run: the given repository files,
    and every file at any depth under the given directories (records and built test outputs), so a file added,
    nested or deleted anywhere beneath them shows. Everything else it needs is read before any test runs, and the
    merge is built from the tip's committed tree, so other changes to the checkout cannot reach a verdict or main."""

    def __init__(self, root, files, trees=()):
        self.root, self.files, self.trees = Path(root), [Path(f) for f in files], [Path(t) for t in trees]
        self.state = self.take()

    def take(self):
        present = [f for f in self.files if f.is_file()]
        return {**digest(present, self.root), **digest(files_under(*self.trees), self.root),
                **{f.relative_to(self.root).as_posix(): None for f in self.files if not f.is_file()}}

    def verify(self, after):
        state = self.take()
        changed = sorted(k for k in set(state) | set(self.state) if state.get(k, "gone") != self.state.get(k, "gone"))
        if changed:
            raise ValueError(f"{after} changed files the validator reads: {', '.join(changed[:10])}")


@contextlib.contextmanager
def sdk_directory(global_json):
    """A new working directory for one dotnet command. The CLI picks its SDK, and `dotnet test` its runner, from the
    nearest global.json at or above its working directory; this one holds `global_json`, the repository's read before
    any test runs (or `{}`, the CLI's defaults), so none planted in the checkout or above is consulted. Being new for
    each command, a process left running by an earlier test cannot have rewritten it in advance."""
    directory = Path(tempfile.mkdtemp(prefix="quire-dotnet-"))
    try:
        (directory / "global.json").write_bytes(global_json)
        yield directory
    finally:
        shutil.rmtree(directory, ignore_errors=True)


def missing_shape(dotnet, generator_dir, global_json, assembly, timeout):
    """Lines of the Quire.Mpl manifest (maintainer-owned) that the built assembly no longer matches: a type or member
    that disappeared or changed kind, access, modifiers or signature. Agents may delete generated stubs, and the
    method inventory keys neither fields nor modifiers. Checked before any test runs, from the build itself."""
    manifest = ROOT / "docs/mirror/manifest/Quire.Mpl.txt"
    expected = [line for line in manifest.read_text(encoding="utf-8").splitlines() if line.strip() and not line.startswith("#")]
    with sdk_directory(global_json) as sdk:
        result = subprocess.run([dotnet, str(generator_dir / "Quire.Tools.MirrorGen.dll"), "shape", str(assembly)],
                                cwd=sdk, capture_output=True, text=True, timeout=timeout)
    if result.returncode:
        raise ValueError(f"shape of {assembly.name} failed: {result.stderr.strip()[:500]}")
    actual = set(result.stdout.splitlines())
    return [line.strip() for line in expected if line not in actual]


def resolve_dotnet(name, root):
    """The absolute CLI path, found now: dotnet runs from a new directory each time, and on Windows a bare name would
    be looked up in the current directory (the repository) before PATH. One inside the repository is refused."""
    found = shutil.which(name)
    if not found:
        raise ValueError(f"dotnet CLI {name!r} not found")
    if Path(found).resolve().is_relative_to(Path(root).resolve()):
        raise ValueError(f"dotnet CLI {found} lies inside the repository under validation")
    return os.path.abspath(found)  # not resolved: a launcher such as snap's picks its tool by the name it is run as


def changed_types(coverage, name_status, previous=None):
    """Types to mutate given `git diff -z --no-renames --name-status` output: those whose Quire.Mpl source was added
    or changed, and those whose comparisons live, or lived according to the baseline's `previous` method -> test
    classes map, in an added, changed or deleted differential test file (a method whose cases were cut or whose
    test file was deleted is mutated against what remains)."""
    fields = [f for f in name_status.split("\0") if f]
    changes = list(zip(fields[0::2], fields[1::2]))
    names = [n for status, n in changes if status[0] in "AM"]
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
    test_classes = {Path(n).stem.lower() for status, n in changes if status[0] in "AMD"
                    and n.lower().startswith("tests/quire.differential.tests/") and n.lower().endswith(".cs")}
    types |= {m["type"] for m in coverage["methods"]
              if any(test_class(case["test"]) in test_classes for case in m.get("cases", []))}
    known = {m["type"] for m in coverage["methods"]}
    types |= {name.split("::", 1)[0] for name, classes in (previous or {}).items()
              if test_classes & set(classes) and name.split("::", 1)[0] in known}
    return types


def test_class(test):
    """Top-level class of `Namespace.Class[+Nested].Method`, lower-cased."""
    return test.rsplit(".", 1)[0].rsplit(".", 1)[-1].split("+")[0].lower()


def mutant_cache_allowed(args):
    """The mutant cache is local-only: code under test runs as the same user and could forge entries, so the gate
    (--mutate-since, --write-baseline) never reads or writes it."""
    return not (args.no_mutant_cache or args.mutate_since or args.write_baseline)


def main():
    if not os.environ.get("QUIRE_VALIDATOR_REPO"):
        # Run from a private copy, isolated from PYTHONPATH and bytecode caches, so nothing code under test writes
        # into the repository's tools/ can change the validator mid-run.
        copy = Path(tempfile.mkdtemp(prefix="quire-validator-"))
        try:
            shutil.copytree(Path(__file__).parent, copy / "validation", ignore=shutil.ignore_patterns("__pycache__", "*.pyc"))
            return subprocess.run([sys.executable, "-I", "-B", str(copy / "validation/validate.py"), *sys.argv[1:]],
                                  env=dict(os.environ, QUIRE_VALIDATOR_REPO=str(ROOT))).returncode
        finally:
            shutil.rmtree(copy, ignore_errors=True)
    # Every module the verdict uses is loaded now, before anything else runs.
    import branch_coverage, merge_gate, method_coverage, mutate, ratchet  # noqa: F401
    import integrity as integrity_module
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
    parser.add_argument("--jobs", type=int, default=4, help="parallel mutant test runs (default: 4)")
    parser.add_argument("--baseline", type=Path, help="ratchet baseline (default: docs/mirror/validation-baseline.json)")
    parser.add_argument("--write-baseline", type=Path, help="write this run's counts as a baseline (unscoped runs only)")
    parser.add_argument("--proposed-mutants", type=Path, metavar="PATH",
                        help="local runs only: `<key> # reason` equivalent mutants proposed for maintainer review; "
                             "their survivors are reported apart and do not fail the run")
    parser.add_argument("--proposed-unreachable", type=Path, metavar="PATH",
                        help="local runs only: `<method key> <path>:<line> sha256=<hash> # reason` unreachable lines "
                             "proposed for maintainer review (generate each with `method_coverage.py "
                             "unreachable-entry`); they exempt coverage like accepted ones but leave the run incomplete")
    parser.add_argument("--no-mutant-cache", action="store_true",
                        help="neither reuse nor record mutant outcomes (implied by --mutate-since and --write-baseline)")
    args = parser.parse_args()
    if args.timeout <= 0:
        parser.error("--timeout must be positive")
    if args.results and args.results.exists():
        parser.error("--results must name a new directory")
    if args.mutate and not args.method_type:
        parser.error("--mutate needs --method-type; use --mutate-since for unscoped runs")
    if (args.mutate or args.mutate_since) and args.profile != "focused":
        parser.error("mutation runs in the focused profile")
    if args.proposed_mutants and (args.mutate_since or args.write_baseline):
        parser.error("--proposed-mutants is for local runs; the gate (--mutate-since, --write-baseline) accepts only "
                     "docs/mirror/accepted-mutants.txt")
    if args.proposed_mutants and not args.proposed_mutants.is_file():
        parser.error(f"--proposed-mutants: {args.proposed_mutants} is not a file")
    if args.proposed_unreachable and (args.mutate_since or args.write_baseline):
        parser.error("--proposed-unreachable is for local runs; the gate (--mutate-since, --write-baseline) accepts "
                     "only docs/mirror/unreachable-lines.txt")
    if args.proposed_unreachable and not args.proposed_unreachable.is_file():
        parser.error(f"--proposed-unreachable: {args.proposed_unreachable} is not a file")
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
    # dotnet runs from a new directory each time (see sdk_directory), so every path it is given must be absolute.
    args.baseline = args.baseline and args.baseline.resolve()
    args.write_baseline = args.write_baseline and args.write_baseline.resolve()
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
        # Verdict inputs are read, and the source lint run, before any code under test can touch them.
        baseline_path = args.baseline or ratchet.BASELINE
        baseline = json.loads(baseline_path.read_text()) if baseline_path.is_file() else None
        deviations = method_coverage.accepted_deviations(ROOT)
        accepted_mutants = mutate.accepted()
        proposed_mutants = mutate.reasons(args.proposed_mutants) if args.proposed_mutants else {}
        unreachable = method_coverage.unreachable_lines()
        proposed_unreachable = (method_coverage.unreachable_lines(args.proposed_unreachable)
                                if args.proposed_unreachable else {})
        # The source each entry's hash and the PDB checksums pin, as it is before the build and any code under test.
        unreachable_sources = method_coverage.read_sources(unreachable, proposed_unreachable)
        lint_findings = integrity_module.lint(ROOT)
        global_json = (ROOT / "global.json").read_bytes() if (ROOT / "global.json").is_file() else b"{}\n"
        args.dotnet = resolve_dotnet(args.dotnet, ROOT)
        changed_names = None
        if args.mutate_since:
            changed_names = git_output("diff", "-z", "--no-renames", "--name-status", args.mutate_since)
            if changed_names is None:
                raise ValueError(f"cannot diff against {args.mutate_since}")
        coverage_dir = Path(tempfile.mkdtemp(prefix="quire-coverage-"))
        # No source control queries: they stamp the commit into every assembly, so a docs-only commit would change
        # the build output and miss the mutant cache.
        build_flags = ["-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false",
                       "-p:EnableSourceControlManagerQueries=false"]
        tool = ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj"
        tool_dir = holder["tool"] = Path(tempfile.mkdtemp(prefix="quire-audit-tool-"))
        generator = ROOT / "tools/Quire.Tools.MirrorGen/Quire.Tools.MirrorGen.csproj"
        generator_dir = holder["generator"] = Path(tempfile.mkdtemp(prefix="quire-mirrorgen-"))
        print("Building the method audit and test projects", flush=True)
        with (output / "build.log").open("w") as log:
            for target, extra in [(tool, ["-o", str(tool_dir)]), (generator, ["-o", str(generator_dir)])] + [
                    (ROOT / "tests" / p / f"{p}.csproj", []) for p in projects]:
                with sdk_directory(global_json) as sdk:
                    result = subprocess.run([args.dotnet, "build", str(target), *build_flags, *extra], cwd=sdk,
                                            stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
                if result.returncode:
                    raise ValueError(f"build of {target.name} failed; inspect build.log")
        tool_digest = digest(files_under(tool_dir), tool_dir)
        # Test assemblies are run directly rather than through their projects, so no MSBuild evaluation (which
        # imports obj/*.props and *.targets that earlier tests could plant) happens after code under test has run.
        # Run that way, VSTest finds the coverage collector only through the directory the Microsoft.CodeCoverage
        # package records in TraceDataCollectorDirectoryPath.
        assemblies, collectors = {}, {}
        for project in projects:
            with sdk_directory(global_json) as sdk:
                query = subprocess.run([args.dotnet, "msbuild", str(ROOT / "tests" / project / f"{project}.csproj"),
                                        "-getProperty:TargetPath", "-getProperty:TraceDataCollectorDirectoryPath",
                                        "-nologo"], cwd=sdk, capture_output=True, text=True, timeout=args.timeout)
            try:
                properties = json.loads(query.stdout)["Properties"]
            except (ValueError, KeyError, TypeError):
                properties = {}
            path = properties.get("TargetPath", "")
            if not path or not Path(path).is_file():
                raise ValueError(f"{project}: built assembly not found ({query.stdout.strip()!r})")
            assemblies[project] = Path(path)
            collectors[project] = properties.get("TraceDataCollectorDirectoryPath", "")
        if args.profile == "focused":
            report["missing_shape"] = missing_shape(args.dotnet, generator_dir, global_json,
                                                    assemblies["Quire.Focused.Differential.Tests"].parent / "Quire.Mpl.dll",
                                                    args.timeout)
            if report["missing_shape"]:
                raise ValueError("the Quire.Mpl build no longer matches its manifest (a deleted stub must come back with "
                                 "the same shape): " + "; ".join(report["missing_shape"][:10]))
        trees = [ROOT / "docs/mirror/evidence", ROOT / "docs/mirror/manifest", *(ROOT / "tests" / p / "bin" for p in projects)]
        seal = Seal(ROOT, [ROOT / "docs/mirror/type-map.csv"], trees)
        for project in projects:
            differential = "Differential" in project
            env = os.environ.copy()
            env["QUIRE_REQUIRE_MATHCAD"] = "0" if args.allow_differential_skips else "1"
            env.pop("QUIRE_METHOD_CASES", None)
            env.pop("QUIRE_VALIDATION_RUN", None)
            env.pop("QUIRE_MUTANT_CACHE", None)
            if differential:
                env["QUIRE_METHOD_CASES"] = str(output / "method-cases")
                env["QUIRE_VALIDATION_RUN"] = run
            trx = output / f"{project}.trx"
            command = [args.dotnet, "test", str(assemblies[project]),
                       "--logger", f"trx;LogFileName={trx.name}", "--results-directory", str(output)]
            if differential and args.diff_filter:
                command += ["--filter", args.diff_filter]
            print(f"Running {project}; log: {output / (project + '.log')}", flush=True)
            item = {"project": project, "command": command, "trx": str(trx)}
            report["projects"].append(item)
            with (output / f"{project}.log").open("w") as log, sdk_directory(global_json) as sdk:
                result = subprocess.run(command, cwd=sdk, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
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
                coverage_command[coverage_command.index("--results-directory") + 1] = str(coverage_dir)
                if not collectors[project] or not Path(collectors[project]).is_dir():
                    raise ValueError(f"{project}: no coverage collector; does it reference Microsoft.NET.Test.Sdk?")
                coverage_command += ["--collect", "Code Coverage;Format=cobertura",
                                     "--test-adapter-path", str(Path(collectors[project]))]
                item["coverage_command"] = coverage_command
                print(f"Measuring branch coverage of {project}", flush=True)
                with (output / f"{project}.coverage.log").open("w") as log, sdk_directory(global_json) as sdk:
                    result = subprocess.run(coverage_command, cwd=sdk, env=env, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
                seal.verify(f"{project} coverage run")
                if result.returncode:
                    raise ValueError(f"{project}: coverage run exited {result.returncode}; inspect its log")
                read_trx(coverage_dir / coverage_trx.name, args.allow_differential_skips)
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
        with (output / "method-audit.log").open("w") as log, sdk_directory(global_json) as sdk:
            result = subprocess.run(scan, cwd=sdk, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
            if result.returncode:
                raise ValueError("Method inventory failed; inspect method-audit.log")
        records = [json.loads(line) for path in (output / "method-cases").glob("*.jsonl")
                   for line in path.read_text().splitlines() if line]
        from branch_coverage import read_cobertura
        cobertura = sorted(coverage_dir.rglob("*.cobertura.xml"))
        report["coverage_reports"] = [str(p) for p in cobertura]
        coverage = audit(json.loads(inventory_path.read_text()), records,
                         passed_tests([Path(p["trx"]) for p in report["projects"]]), run, types=args.method_type,
                         lines=read_cobertura(cobertura), deviations=deviations, unreachable=unreachable,
                         proposed_unreachable=proposed_unreachable, sources=unreachable_sources)
        (output / "method-coverage.json").write_text(json.dumps(coverage, indent=2) + "\n")
        report["method_coverage"] = coverage["counts"]
        print(f"Method coverage: {coverage['counts']}; {output / 'method-coverage.json'}", flush=True)
        # An executed line is reachable, so the entry listing it is wrong: that fails the run and the gate.
        report["executed_unreachable"] = [f"{e['key']} {e['line']} ({e['source']})"
                                          for e in coverage["stale_unreachable"] if e["problem"] == "executed"]
        report["stale_unreachable"] = sum(e["source"] == "accepted" for e in coverage["stale_unreachable"])
        if coverage["stale_unreachable"]:
            print(f"Stale unreachable-line entries: {coverage['stale_unreachable']}", flush=True)
        if any(m["proposed_exempt_lines"] for m in coverage["methods"]):
            print("Coverage passes only with proposed unreachable lines, which await maintainer acceptance", flush=True)
        integrity = integrity_module.check(json.loads(inventory_path.read_text()), findings=lint_findings)
        (output / "integrity.json").write_text(json.dumps(integrity, indent=2) + "\n")
        report["integrity"] = {"lint": len(integrity["lint"])}
        print(f"Integrity: {report['integrity']}; {output / 'integrity.json'}", flush=True)
        ratchet_report = ratchet.check(coverage, integrity, bool(args.method_type), args.profile, baseline_path, baseline)
        (output / "ratchet.json").write_text(json.dumps(ratchet_report, indent=2) + "\n")
        report["ratchet"] = {"ok": ratchet_report["ok"], "regressions": ratchet_report["regressions"]}
        print(f"Ratchet: {'ok' if ratchet_report['ok'] else ratchet_report['regressions']}", flush=True)
        mutation_ok = mutation_complete = True
        mutate_types = set(args.method_type) if args.mutate else set()
        if args.mutate_since:
            mutate_types |= changed_types(coverage, changed_names, (baseline or {}).get("tests"))
            report["mutate_since"] = args.mutate_since
        if mutate_types:
            print("Mutating compared methods; each mutant reruns the differential tests", flush=True)
            mutation = mutate.run(coverage, mutate_types, assemblies["Quire.Focused.Differential.Tests"], "Quire.Mpl",
                                  args.dotnet, args.jobs, args.timeout, accepted_mutants, global_json,
                                  proposed_mutants, mutant_cache_allowed(args), install)
            seal.verify("mutation")
            (output / "mutation.json").write_text(json.dumps(mutation, indent=2) + "\n")
            report["mutation"] = {k: mutation[k] for k in ("total", "killed", "accepted_survivors", "survivors",
                                                           "proposed_survivors", "stale_proposed")}
            # Proposed survivors keep the run incomplete, but only accepted-mutants.txt is gate-relevant; the merge
            # gate never passes proposals anyway.
            mutation_ok = not mutation["survivors"]
            mutation_complete = mutation["complete"]
            print(f"Mutation: {mutation['killed']}/{mutation['total']} killed ({mutation['cached']} cached); "
                  f"survivors {len(mutation['survivors'])}", flush=True)
            if mutation["proposed_survivors"]:
                print(f"Mutation passes only with proposed mutants: {len(mutation['proposed_survivors'])} survivors "
                      f"await maintainer acceptance", flush=True)
            if args.proposed_mutants:
                print(f"Stale proposals: {mutation['stale_proposed']}", flush=True)
        if args.proposed_mutants:
            report["proposed_mutants"] = str(args.proposed_mutants.resolve())
        if args.proposed_unreachable:
            report["proposed_unreachable"] = str(args.proposed_unreachable.resolve())
        # The merge gate needs green tests, no regression, no surviving mutant and no executed "unreachable" line;
        # completeness is the end goal. A run with proposals is neither: it records whether the gate's requirements
        # hold once the proposals are accepted.
        gate_ok = ratchet_report["ok"] and mutation_ok and not report["executed_unreachable"]
        proposals = bool(args.proposed_mutants or args.proposed_unreachable)
        if proposals:
            report["passes_with_proposals"] = gate_ok
            print(f"Gate requirements {'hold' if gate_ok else 'fail'} with the proposals accepted", flush=True)
        report["complete"] = (coverage["complete"] and integrity["complete"] and mutation_ok and mutation_complete
                              and not proposals)
        report["gate_passed"] = gate_ok and not proposals
        seal.verify("the run")
        # Written only now that no code under test runs again; the merge gate still checks it against main's.
        if args.write_baseline:
            args.write_baseline.write_text(json.dumps(ratchet_report["snapshot"], indent=2, sort_keys=True) + "\n",
                                           newline="\n")
        # 0: complete; 3: the merge gate's requirements hold but gaps remain; 1: blocked (always with proposals).
        return 0 if report["complete"] else 3 if report["gate_passed"] else 1
    except (OSError, ValueError, KeyError, TypeError, ET.ParseError, subprocess.TimeoutExpired) as error:
        report["error"] = str(error)
        print(str(error), file=sys.stderr)
        return 1
    finally:
        for directory in holder.values():
            shutil.rmtree(directory, ignore_errors=True)
        (output / "summary.json").write_text(json.dumps(report, indent=2) + "\n")
        print(f"Report: {output / 'summary.json'}", flush=True)


if __name__ == "__main__":
    sys.exit(main())
