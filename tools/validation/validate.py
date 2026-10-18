"""Run maintained test projects and reject empty, skipped, aborted or failing runs."""
import argparse
import contextlib
import hashlib
import json
import os
from pathlib import Path
import re
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


def mutation_jobs(value=None):
    """Bound the default worker count; explicit overrides must be positive integers."""
    if value is None:
        return min(12, max(1, os.cpu_count() or 1))
    try:
        jobs = int(value)
    except (TypeError, ValueError):
        raise argparse.ArgumentTypeError("--jobs must be a positive integer") from None
    if jobs < 1:
        raise argparse.ArgumentTypeError("--jobs must be a positive integer")
    return jobs


def phase_projects(root=ROOT):
    """Keep the full phase test set, and reject solution tests the phase runner would omit."""
    paths = {p.relative_to(root).as_posix() for p in (root / 'tests').glob('*/*.csproj')
             if p.parent.name != 'Quire.Focused.Differential.Tests'}
    solution = {Path(p.attrib['Path'].replace('\\', '/')).as_posix()
                for p in ET.parse(root / 'Quire.slnx').iter('Project')}
    solution = {p for p in solution if p.startswith('tests/')}
    if missing := solution - paths:
        raise ValueError('phase profile would omit solution test projects: ' + ', '.join(sorted(missing)))
    return sorted(Path(p).parent.name for p in paths)


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


def digest_outputs(*directories):
    """Fingerprints of run-owned build outputs, compared again after code under test has run."""
    return [digest(files_under(d), d) for d in directories]


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


def method_audit(dotnet, tool_dir, structure_dir, install, inventory, profile, path=str):
    """The project to build optimized (Release) into `structure_dir`, with its build arguments, and the MethodAudit
    command. Structure is compared on that build, as Mathcad's assemblies are optimized; coverage, mutation and body
    hashes use the Debug build the tests run. `path` names a path for `dotnet` (windows_path from WSL)."""
    host, framework = (("Quire.Focused.Differential.Tests", "net10.0") if profile == "focused"
                       else ("Quire.Mirror.Tests", "net10.0-windows"))
    scan = [dotnet, path(tool_dir / "Quire.Tools.MethodAudit.dll"),
            "--repo", path(ROOT), "--assemblies", path(ROOT / "tests" / host / "bin/Debug" / framework),
            "--structure-assemblies", path(structure_dir), "--install", path(install), "--output", path(inventory)]
    if profile == "focused":
        scan += ["--project", "Quire.Mpl"]
    return (ROOT / "tests" / host / f"{host}.csproj", ["-c", "Release", "--no-incremental", "-o", path(structure_dir)]), scan


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


# The phase profile from WSL (validation.md, "Phase profile from WSL"): Windows' dotnet.exe builds, tests and audits,
# so everything it is given is a Windows path on a Windows drive, and it sees only the variables WSLENV names.
WINDOWS_DOTNET = "/mnt/c/Program Files/dotnet/dotnet.exe"
# What test code reads from its environment (Environment.GetEnvironmentVariable in tests/ and src/), beside the
# Windows user's own variables. QUIRE_METHOD_CASES is converted before it is set; `/p` converts MATHCAD_DIR.
FORWARDED = ("QUIRE_REQUIRE_MATHCAD", "QUIRE_METHOD_CASES", "QUIRE_VALIDATION_RUN", "MATHCAD_DIR/p")


def is_wsl(environ=os.environ):
    return os.name != "nt" and bool(environ.get("WSL_DISTRO_NAME") or environ.get("WSL_INTEROP"))


def windows_dotnet(requested="dotnet", environ=os.environ):
    """The Windows CLI a phase run from WSL drives, or None: `--dotnet` when given, else QUIRE_WINDOWS_DOTNET when
    set, else WINDOWS_DOTNET when installed. Only a Windows `.exe` qualifies."""
    if requested == "dotnet":
        requested = environ.get("QUIRE_WINDOWS_DOTNET") or (WINDOWS_DOTNET if Path(WINDOWS_DOTNET).is_file() else "")
    return requested if requested.lower().endswith(".exe") else None


def windows_path(path):
    """`path` as a Windows process started from WSL names it (`wslpath -w`). Only a path on a Windows drive
    (/mnt/<drive>) qualifies: anything else would reach Windows only through the \\\\wsl.localhost share."""
    path = os.path.realpath(path)
    result = subprocess.run(["wslpath", "-w", path], capture_output=True, text=True)
    converted = result.stdout.strip()
    if result.returncode or not converted or converted.startswith("\\\\"):
        raise ValueError(f"{path} is not on a Windows drive (/mnt/<drive>); a phase run from WSL needs the repository, "
                         "results and temporary directories there, where Windows' dotnet.exe can reach them")
    return converted


def wsl_document(path, windows_root, root=ROOT):
    """A path a Windows build recorded (PDB documents, coverage files, MSBuild properties) as the validator reads it:
    inside the repository (case-insensitively, as on Windows) the repository's own path; elsewhere on a drive its
    /mnt/<drive> path; anything else unchanged."""
    prefix, normal = windows_root.rstrip("\\") + "\\", path.replace("/", "\\")
    if normal.lower().startswith(prefix.lower()):
        return Path(root).as_posix().rstrip("/") + "/" + normal[len(prefix):].replace("\\", "/")
    if re.match(r"[A-Za-z]:\\", normal):
        return f"/mnt/{normal[0].lower()}/" + normal[3:].replace("\\", "/")
    return path


def wsl_inventory(inventory, windows_root, root=ROOT):
    """The method inventory with the source documents of its methods and test types read back through WSL."""
    for method in inventory.get("methods", []):
        for document in method.get("source") or []:
            document["file"] = wsl_document(document["file"], windows_root, root)
    for test_type in inventory.get("test_types", []):
        test_type["documents"] = [wsl_document(d, windows_root, root) for d in test_type["documents"]]
    return inventory


def wslenv(env):
    """`env` with WSLENV also naming FORWARDED, the variables the Windows test process must see."""
    names = {f.split("/")[0] for f in FORWARDED}
    kept = [n for n in env.get("WSLENV", "").split(":") if n and n.split("/")[0] not in names]
    return dict(env, WSLENV=":".join(kept + list(FORWARDED)))


def windows_temp(environ=os.environ, root=ROOT):
    """The temporary directory of a phase run from WSL, created if missing: QUIRE_WINDOWS_TEMP, else
    `.quire-windows-temp` beside the checkout. Refused unless it is on a Windows drive."""
    directory = Path(environ.get("QUIRE_WINDOWS_TEMP") or Path(root).resolve().parent / ".quire-windows-temp").resolve()
    windows_path(directory)
    directory.mkdir(parents=True, exist_ok=True)
    return str(directory)


# The mutation scratch directory (in the system temporary directory, so the stale sweep covers it), and the file in
# the results naming it, so the merge gate can remove it if the validator dies.
SCRATCH_PREFIX, SCRATCH_RECORD = "quire-mutant-scratch-", "mutation-scratch.txt"
DIFFERENTIAL_TESTS = "tests/Quire.Differential.Tests/"
# Callers of one dispatch slot above which a run notes the slot (object.ToString and the like): they are all still
# selected, since leaving any out would be unsound, but the note says why a selection is large.
WIDE_SLOT = 50


def default_baseline(profile):
    """The baseline a profile ratchets against unless --baseline names another: each profile has its own."""
    import ratchet
    return ratchet.PHASE_BASELINE if profile == "phase" else ratchet.BASELINE


def test_sources(root=None):
    """Repository-relative path -> text of every C# file the differential test assemblies compile, read before any
    test runs (see changed_test_classes)."""
    root = Path(root or ROOT)
    return {p.relative_to(root).as_posix(): p.read_text(encoding="utf-8", errors="replace")
            for p in sorted((root / DIFFERENTIAL_TESTS).rglob("*.cs")) if p.is_file()}


def name_changes(name_status):
    """(status, path) pairs of `git diff -z --no-renames --name-status` output."""
    fields = [f for f in name_status.split("\0") if f]
    return list(zip(fields[0::2], fields[1::2]))


def changed_types(coverage, name_status, previous=None, sources=None, test_types=None):
    """Types to mutate given `git diff -z --no-renames --name-status` output: those whose Quire.Mpl source was added
    or changed, and those whose comparisons live, or lived according to the baseline's `previous` method -> test
    classes map, in a test class changed_test_classes reports (a method whose cases were cut or whose test file was
    deleted is mutated against what remains)."""
    changes = name_changes(name_status)
    names = [n for status, n in changes if status[0] in "AM"]
    root = ROOT.resolve().as_posix() + "/"
    sources_by_file = {}
    for method in coverage["methods"]:
        for document in method.get("source") or []:
            file = Path(document["file"]).as_posix()
            if not file.startswith(root):
                raise ValueError(f"PDB source {file} of {method['type']} lies outside the repository; "
                                 "path maps and #line directives would hide changes from mutation")
            sources_by_file.setdefault(file[len(root):], set()).add(method["type"])
    types = set()
    for name in names:
        # A file with no method bodies (enums, interfaces, attributes) has no sequence points and nothing to mutate.
        if name.lower().startswith("src/quire.mpl/") and name.lower().endswith(".cs"):
            types |= sources_by_file.get(name, set())
    test_classes, _ = changed_test_classes(changes, sources, test_types)
    types |= {m["type"] for m in coverage["methods"]
              if touches(test_classes, {test_class(case["test"]) for case in m.get("cases", [])})}
    known = {m["type"] for m in coverage["methods"]}
    types |= {name.split("::", 1)[0] for name, classes in (previous or {}).items()
              if touches(test_classes, set(classes)) and name.split("::", 1)[0] in known}
    return types


# A type declaration starting a line, after any modifiers: a type named in a comment or string does not count.
DECLARATION = re.compile(r"^[ \t]*(?:(?:public|private|protected|internal|static|sealed|abstract|partial|readonly|ref|"
                         r"unsafe|file|new)\s+)*(?:class|struct|interface|enum|record(?:\s+(?:class|struct))?)\s+"
                         r"([A-Za-z_]\w*)", re.M)
# changed_test_classes' answer when it cannot tell which classes a change reaches: every one.
ALL_TEST_CLASSES = "all"


def touches(test_classes, classes):
    return bool(classes) and (test_classes == ALL_TEST_CLASSES or bool(test_classes & classes))


def changed_test_classes(changes, sources=None, test_types=None):
    """(lower-cased test classes, how) whose behaviour an added, changed or deleted differential test file can change:
    the classes declared in it (the lint holds an agent test file to one top-level class named after it; nested
    classes count as it), and, transitively, every class that uses one of those, since shared helpers
    (`MatValDifferentialTests.Storage`, extension methods, `[MemberData(..., MemberType = ...)]`) are used across
    classes. Uses come from the built test assembly's IL and attributes, `test_types` (the inventory's `test_types`,
    with `references`), united with a textual identifier match over `sources` (path -> text) when given: each covers
    what the other misses (`how` = "IL" or "IL and text"). Without `test_types`, or with an empty one, every class
    counts as changed ("all"): the text alone misses extension-method calls."""
    files = [(status, path) for status, path in changes if status[0] in "AMD"
             and path.lower().startswith(DIFFERENTIAL_TESTS.lower()) and path.lower().endswith(".cs")]
    if not files:
        return set(), "none"
    if not test_types or not all("references" in t for t in test_types):
        return ALL_TEST_CLASSES, "all"
    root = ROOT.resolve().as_posix().lower() + "/"
    paths = {path.lower() for _, path in files}
    def relative(document):
        document = Path(document).as_posix().lower()
        return document[len(root):] if document.startswith(root) else document
    simple = lambda name: name.rsplit(".", 1)[-1].split("`")[0]
    users = {}
    for t in test_types:
        for reference in t["references"]:
            users.setdefault(reference, set()).add(t["name"])
    names = {Path(path).stem for _, path in files}
    affected = {t["name"] for t in test_types if any(relative(d) in paths for d in t["documents"])}
    scanned = {path for _, path in files}
    for path in scanned:
        names |= set(DECLARATION.findall((sources or {}).get(path, "")))
    while True:
        known = len(names)
        affected |= {t["name"] for t in test_types if simple(t["name"]) in names}
        pending = list(affected)
        while pending:
            for user in users.get(pending.pop(), ()):
                if user not in affected:
                    affected.add(user)
                    pending.append(user)
        names |= {simple(n) for n in affected}
        if sources is not None:
            pattern = re.compile(r"\b(?:" + "|".join(sorted(map(re.escape, names))) + r")\b")
            grown = {path for path, text in sources.items() if path not in scanned and pattern.search(text)}
            scanned |= grown
            for path in grown:
                names |= {Path(path).stem, *DECLARATION.findall(sources[path])}
        if len(names) == known:
            return {n.lower() for n in names}, "IL" if sources is None else "IL and text"


def is_accessor(method):
    """A property, indexer or event accessor, or a compiler-generated method inside one."""
    names = [method["signature"].split("`")[0], (method.get("owner") or {}).get("name") or ""]
    return any(n.split(".")[-1].startswith(("get_", "set_", "add_", "remove_")) for n in names)


def slot_of_key(signature):
    """The dispatch slot (`name`arity/parameter count`, name unqualified) of an inventory signature
    `Name`arity(parameters)->return|static`, as MethodAudit's StructureReader.SlotOf writes it."""
    match = re.match(r"(.*?)`(\d+)\(", signature)
    if not match:
        raise ValueError(f"unexpected method signature {signature!r}")
    depth, count, empty = 0, 0, True
    for char in signature[match.end():]:
        if char == ")" and depth == 0:
            break
        empty = False
        if char in "<[(":
            depth += 1
        elif char in ">])":
            depth -= 1
        elif char == "," and depth == 0:
            count += 1
    return f"{match.group(1).split('.')[-1]}`{match.group(2)}/{0 if empty else count + 1}"


def changed_methods(coverage, name_status, baseline, sources=None, test_types=None):
    """(method key -> reasons, notes) for the methods the gate mutates (validation.md, "What the gate mutates"). A
    mutant's outcome depends on its method's IL, the tests that compare the method, and what runs under those tests.
    Changed are methods whose IL body hash differs from the baseline's (new methods included), and the hand-written
    owners of changed compiler-generated methods (lambdas, iterator and async state machines). Selected are the
    changed methods; methods compared by tests changed_test_classes reports, now or according to the baseline's
    method -> test classes map; methods compared now but not in the baseline; direct callers of a changed method by
    token, or through a virtual call to its slot, or to the slot of a method the baseline has and the build lacks (a
    removed override); readers of a field that a changed or removed method writes, now or in the baseline, where
    reading a field that holds a mutable reference (an array or collection a method may fill) counts as writing it;
    and, when such a reader is an accessor, its callers, through chains of accessors. `baseline` must hold `bodies`
    (schema 4)."""
    test_classes, how = changed_test_classes(name_changes(name_status), sources, test_types)
    bodies, previous, stages = baseline["bodies"], baseline.get("tests") or {}, baseline.get("methods") or {}
    old_writes = baseline.get("writes") or {}
    rows = {f"{m['type']}::{m['signature']}": m for m in coverage["methods"]}
    for name, m in rows.items():
        if "body_sha256" not in m or (m.get("source") or m.get("mutants")) and not m["body_sha256"]:
            raise ValueError(f"{name}: the inventory has no IL body hash, but the baseline selects mutants by them; "
                             "is MethodAudit out of date?")
        if m.get("slot") and m["slot"] != slot_of_key(m["signature"]):
            raise ValueError(f"{name}: MethodAudit's slot {m['slot']!r} differs from {slot_of_key(m['signature'])!r}")
    reasons, notes = {}, []
    def select(name, reason):
        if reason not in reasons.setdefault(name, []):
            reasons[name].append(reason)
    changed = set()
    for name, m in rows.items():
        if m.get("body_sha256") and bodies.get(name) != m["body_sha256"]:
            changed.add(name)
            select(name, "IL changed" if name in bodies else "new body")
        if touches(test_classes, {test_class(c["test"]) for c in m.get("cases", [])} | set(previous.get(name, []))):
            select(name, "comparing tests changed")
        if m.get("validation") == "compared" and stages.get(name) != "compared":
            select(name, "newly compared")
    for name in list(changed):
        owner = rows[name].get("owner")
        for other, m in rows.items():
            if owner and m["type"] == owner["type"] and m["signature"].split("`")[0] == owner["name"]:
                changed.add(other)
                select(other, "compiler-generated part changed")
    removed = {name for name in bodies if name not in rows}
    tokens = {(rows[n].get("project"), rows[n]["token"]) for n in changed}
    slots = {rows[n]["slot"] for n in changed if rows[n].get("slot")}
    slots |= {slot_of_key(n.split("::", 1)[1]) for n in removed if n.endswith("|False")}
    written = {f for n in changed for f in rows[n].get("field_writes", []) + rows[n].get("reference_reads", [])}
    written |= {f for n in changed | removed for f in old_writes.get(n, [])}
    callers, readers = {}, []
    for name, m in rows.items():
        if any((m.get("project"), t) in tokens for t in m.get("calls", [])):
            select(name, "calls a changed method")
        for slot in slots & set(m.get("virtual_calls", [])):
            select(name, "calls a changed method")
            callers[slot] = callers.get(slot, 0) + 1
        if written & set(m.get("field_reads", [])):
            select(name, "reads a field a changed method writes")
            readers.append(name)
    # A getter that reads such a field passes its value on: its callers see the change too, through accessor chains.
    pending = [n for n in readers if is_accessor(rows[n])]
    pending += [o for n in pending for o, m in rows.items() if rows[n].get("owner")
                and m["type"] == rows[n]["owner"]["type"] and m["signature"].split("`")[0] == rows[n]["owner"]["name"]]
    seen = set(pending)
    while pending:
        accessor = rows[pending.pop()]
        token, slot = (accessor.get("project"), accessor["token"]), accessor.get("slot")
        for name, m in rows.items():
            if token in {(m.get("project"), t) for t in m.get("calls", [])} or slot in m.get("virtual_calls", []):
                select(name, "calls an accessor that reads a changed field")
                if is_accessor(m) and name not in seen:
                    seen.add(name)
                    pending.append(name)
    notes += [f"slot {slot} selects {n} callers (all kept)" for slot, n in sorted(callers.items()) if n > WIDE_SLOT]
    notes.append(f"test class dependencies: {how}")
    return reasons, notes


def verdict(report):
    """The one-line answer a run gives: whether its own checks held (tests, ratchet, mutation, unreachable lines),
    and what the merge gate would make of the same results. `gate` follows the exit status: `would pass` (0,
    complete), `would pass with gaps` (3), `would block` (1 or 2). Proposals awaiting approval block the gate but
    are not local failures: a run whose only blockers are pending proposals is the expected review state."""
    failures, pending, gaps = [], [], []
    if report.get("error"):
        failures.append(f"validation error: {report['error'][:200]}")
    for project in report.get("projects", []):
        if project.get("exit_code") or project.get("failed") or project.get("skipped"):
            failures.append(f"{project['project']}: tests did not all pass")
    ratchet_report = report.get("ratchet")
    if ratchet_report and not ratchet_report["ok"]:
        failures.append(f"{len(ratchet_report['regressions'])} ratchet regressions")
    mutation = report.get("mutation") or {}
    if mutation.get("survivors"):
        failures.append(f"{len(mutation['survivors'])} surviving mutants")
    if report.get("executed_unreachable"):
        failures.append(f"{len(report['executed_unreachable'])} listed unreachable lines executed")
    if mutation.get("proposed_survivors"):
        pending.append(f"{len(mutation['proposed_survivors'])} proposed mutants pending approval")
    if report.get("proposed_exempt_lines"):
        pending.append(f"{report['proposed_exempt_lines']} proposed unreachable lines pending approval")
    if not failures and not ratchet_report:
        failures.append("checks did not finish")
    if not report.get("complete"):
        gaps = [f"{n} {state}" for state, n in (report.get("method_coverage") or {}).items()
                if state in ("missing", "structure_mismatch", "uncovered", "verified_unsupported") and n]
        if report.get("stale_unreachable"):
            gaps.append(f"{report['stale_unreachable']} stale unreachable-line entries")
        if report.get("stale_accepted"):
            gaps.append(f"{len(report['stale_accepted'])} stale accepted-mutant entries")
        if (report.get("integrity") or {}).get("complete") is False:
            gaps.append("integrity incomplete")
        if mutation and mutation.get("complete") is False:
            gaps.append("mutation incomplete")
    if report.get("complete"):
        gate, reasons = "would pass", []
    elif report.get("gate_passed"):
        gate, reasons = "would pass with gaps", gaps
    else:
        reasons = failures + pending
        if not reasons and (report.get("proposed_mutants") or report.get("proposed_unreachable")):
            reasons = ["run used proposals; rerun without them"]
        gate = "would block"
    local = "fail" if failures else "pass"
    scope = report.get("method_types")
    line = (f"LOCAL CHECKS: {local} · GATE: {gate}" + (f" ({'; '.join(reasons)})" if reasons else "")
            + (f" [scoped to {len(scope)} types; the gate validates all]" if scope else ""))
    if report.get("profile") == "focused" and report.get("mutation_requested") is False:
        line += " [mutation deferred to gate]"
    return {"local_checks": local, "gate": gate, "reasons": reasons, "pending_approval": pending, "line": line}


def test_class(test):
    """Top-level class of `Namespace.Class[+Nested].Method`, lower-cased."""
    return test.rsplit(".", 1)[0].rsplit(".", 1)[-1].split("+")[0].lower()


def stale_accepted(inventory, accepted_mutants):
    """Accepted-mutant entries that excuse nothing (mutate.stale_entries). An accepted mutant was proven equivalent for
    one method body; once that body changes the entry excuses nothing. Checked against the whole inventory, not a
    --method-type selection, so a scoped run does not report other types' entries as naming no method."""
    import mutate
    return mutate.stale_entries(inventory["methods"], accepted_mutants)


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
    parser.add_argument("--mutate-since", metavar="REV",
                        help="IL-mutate compared methods whose IL, comparing tests or callees changed since the baseline "
                             "(test files: since REV)")
    parser.add_argument("--mutate-types-wide", action="store_true",
                        help="maintainer audits: with --mutate-since, mutate every compared method of each changed type")
    parser.add_argument("--mutate-all", action="store_true",
                        help="maintainer audits: mutate every compared method (unscoped runs)")
    parser.add_argument("--jobs", type=mutation_jobs, default=mutation_jobs(),
                        help="parallel mutant test runs (default: CPU count, at most 12)")
    parser.add_argument("--baseline", type=Path, help="ratchet baseline (default: docs/mirror/validation-baseline.json)")
    parser.add_argument("--write-baseline", type=Path, help="write this run's counts as a baseline (unscoped runs only)")
    parser.add_argument("--proposed-mutants", type=Path, metavar="PATH",
                        help="local runs only: `<key> [body=<body_sha256>] # reason` equivalent mutants proposed for "
                             "maintainer review; "
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
    if args.mutate_types_wide and not args.mutate_since:
        parser.error("--mutate-types-wide widens --mutate-since")
    if args.mutate_all and args.method_type:
        parser.error("--mutate-all needs an unscoped run")
    if (args.mutate or args.mutate_since or args.mutate_all) and args.profile != "focused":
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
    windows = None  # Windows' dotnet.exe, for a phase run from WSL
    if args.profile == "phase" and os.name != "nt":
        windows = windows_dotnet(args.dotnet) if is_wsl() else None
        if not windows:
            parser.error("phase tests require Windows and the Windows Desktop runtime: run on Windows, or from WSL with "
                         f"a Windows dotnet.exe (--dotnet, QUIRE_WINDOWS_DOTNET or {WINDOWS_DOTNET}); use focused for "
                         "the portable runtime slice")
        args.dotnet = windows
        try:
            # Every temporary directory (dotnet's working directories, build outputs, coverage, default results).
            tempfile.tempdir = windows_temp()
            windows_path(ROOT)
            if args.results:
                windows_path(args.results.resolve().parent)
        except ValueError as error:
            parser.error(str(error))
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
    def git_output(*git_args):
        result = subprocess.run(["git", *git_args], cwd=ROOT, text=True, capture_output=True)
        return result.stdout.strip() if result.returncode == 0 else None

    run = str(uuid.uuid4())
    report = {"run": run, "method_types": args.method_type, "revision": git_output("rev-parse", "HEAD"), "working_tree": git_output("status", "--porcelain"), "profile": args.profile, "diff_filter": args.diff_filter,
              "mutation_requested": bool(args.mutate or args.mutate_since or args.mutate_all),
              "complete": False, "gate_passed": False, "projects": [], "limitations":
              "Passing comparisons establish equivalence only for executed cases. Phase also requires render, coverage-excluded checks and independent review."}
    install = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"
                   if os.name == "nt" else "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0"))
    artifact = install / "mplrun.dll"
    report["original"] = {"path": str(artifact), "sha256": hashlib.sha256(artifact.read_bytes()).hexdigest() if artifact.is_file() else None}
    holder = {}
    # How dotnet names a path it is given; from WSL, Windows paths, and a Windows build's paths read back.
    to_dotnet = windows_path if windows else str
    if windows:
        windows_root = windows_path(ROOT)
        report["wsl"] = {"dotnet": windows, "temp": tempfile.tempdir, "windows_root": windows_root}
    from_dotnet = (lambda p: wsl_document(p, windows_root)) if windows else (lambda p: p)
    try:
        if args.profile == "phase":
            projects = phase_projects()
        # Everything the verdict depends on is loaded or built before any code under test runs; afterwards the seal
        # catches tests that rewrite maintainer-owned files or built outputs on disk.
        # Verdict inputs are read, and the source lint run, before any code under test can touch them.
        baseline_path = args.baseline or default_baseline(args.profile)
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
        changed_names = sources = None
        if args.mutate_since:
            sources = test_sources()
            changed_names = git_output("diff", "-z", "--no-renames", "--name-status", args.mutate_since)
            if changed_names is None:
                raise ValueError(f"cannot diff against {args.mutate_since}")
        coverage_dir = holder["coverage"] = Path(tempfile.mkdtemp(prefix="quire-coverage-"))
        # No source control queries: they stamp the commit into every assembly, so a docs-only commit would change
        # the build output and miss the mutant cache.
        build_flags = ["-m:1", "-nr:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false",
                       "-p:EnableSourceControlManagerQueries=false"]
        tool = ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj"
        tool_dir = holder["tool"] = Path(tempfile.mkdtemp(prefix="quire-audit-tool-"))
        generator = ROOT / "tools/Quire.Tools.MirrorGen/Quire.Tools.MirrorGen.csproj"
        generator_dir = holder["generator"] = Path(tempfile.mkdtemp(prefix="quire-mirrorgen-"))
        structure_dir = holder["structure"] = Path(tempfile.mkdtemp(prefix="quire-audit-release-"))
        inventory_path = output / "method-inventory.json"
        structure_build, scan = method_audit(args.dotnet, tool_dir, structure_dir, install, inventory_path, args.profile,
                                             to_dotnet)
        print("Building the method audit and test projects", flush=True)
        with (output / "build.log").open("w") as log:
            for target, extra in [(tool, ["-o", to_dotnet(tool_dir)]), (generator, ["-o", to_dotnet(generator_dir)]),
                                  structure_build] + [(ROOT / "tests" / p / f"{p}.csproj", []) for p in projects]:
                with sdk_directory(global_json) as sdk:
                    result = subprocess.run([args.dotnet, "build", to_dotnet(target), *build_flags, *extra], cwd=sdk,
                                            stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
                if result.returncode:
                    raise ValueError(f"build of {target.name} failed; inspect build.log")
        sealed_digest = digest_outputs(tool_dir, structure_dir)
        # Test assemblies are run directly rather than through their projects, so no MSBuild evaluation (which
        # imports obj/*.props and *.targets that earlier tests could plant) happens after code under test has run.
        # Run that way, VSTest finds the coverage collector only through the directory the Microsoft.CodeCoverage
        # package records in TraceDataCollectorDirectoryPath.
        assemblies, collectors = {}, {}
        for project in projects:
            with sdk_directory(global_json) as sdk:
                query = subprocess.run([args.dotnet, "msbuild", to_dotnet(ROOT / "tests" / project / f"{project}.csproj"),
                                        "-getProperty:TargetPath", "-getProperty:TraceDataCollectorDirectoryPath",
                                        "-nologo"], cwd=sdk, capture_output=True, text=True, timeout=args.timeout)
            try:
                properties = json.loads(query.stdout)["Properties"]
            except (ValueError, KeyError, TypeError):
                properties = {}
            path = from_dotnet(properties.get("TargetPath", ""))
            if not path or not Path(path).is_file():
                raise ValueError(f"{project}: built assembly not found ({query.stdout.strip()!r})")
            assemblies[project] = Path(path)
            collectors[project] = from_dotnet(properties.get("TraceDataCollectorDirectoryPath", ""))
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
                env["QUIRE_METHOD_CASES"] = to_dotnet(output / "method-cases")
                env["QUIRE_VALIDATION_RUN"] = run
            trx = output / f"{project}.trx"
            if windows:
                env = wslenv(env)
            command = [args.dotnet, "test", to_dotnet(assemblies[project]),
                       "--logger", f"trx;LogFileName={trx.name}", "--results-directory", to_dotnet(output)]
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
                coverage_command[coverage_command.index("--results-directory") + 1] = to_dotnet(coverage_dir)
                if not collectors[project] or not Path(collectors[project]).is_dir():
                    raise ValueError(f"{project}: no coverage collector; does it reference Microsoft.NET.Test.Sdk?")
                coverage_command += ["--collect", "Code Coverage;Format=cobertura",
                                     "--test-adapter-path", to_dotnet(Path(collectors[project]))]
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
        if digest_outputs(tool_dir, structure_dir) != sealed_digest:
            raise ValueError("the method audit or Release build changed while tests ran")
        report["method_commands"] = [[args.dotnet, "build", to_dotnet(structure_build[0]), *build_flags, *structure_build[1]],
                                     scan]
        print("Auditing every selected method, including private methods and accessors", flush=True)
        with (output / "method-audit.log").open("w") as log, sdk_directory(global_json) as sdk:
            result = subprocess.run(scan, cwd=sdk, stdout=log, stderr=subprocess.STDOUT, timeout=args.timeout)
            if result.returncode:
                raise ValueError("Method inventory failed; inspect method-audit.log")
        records = [json.loads(line) for path in (output / "method-cases").glob("*.jsonl")
                   for line in path.read_text().splitlines() if line]
        from branch_coverage import read_cobertura
        cobertura = sorted(coverage_dir.rglob("*.cobertura.xml"))
        def read_inventory():
            inventory = json.loads(inventory_path.read_text())
            return wsl_inventory(inventory, windows_root) if windows else inventory
        coverage = audit(read_inventory(), records,
                         passed_tests([Path(p["trx"]) for p in report["projects"]]), run, types=args.method_type,
                         lines=read_cobertura(cobertura, from_dotnet), deviations=deviations, unreachable=unreachable,
                         proposed_unreachable=proposed_unreachable, sources=unreachable_sources)
        (output / "method-coverage.json").write_text(json.dumps(coverage, indent=2) + "\n")
        report["method_coverage"] = coverage["counts"]
        print(f"Method coverage: {coverage['counts']}; {output / 'method-coverage.json'}", flush=True)
        exact = sum(m.get("exact_match") is True for m in coverage["methods"])
        report["il_match"] = {"exact": exact, "structure_only": sum(
            m.get("structure_matches") is True for m in coverage["methods"]) - exact}
        print(f"IL match: {exact} exact, {report['il_match']['structure_only']} structure only", flush=True)
        # An executed line is reachable, so the entry listing it is wrong: that fails the run and the gate.
        report["executed_unreachable"] = [f"{e['key']} {e['line']} ({e['source']})"
                                          for e in coverage["stale_unreachable"] if e["problem"] == "executed"]
        report["stale_unreachable"] = sum(e["source"] == "accepted" for e in coverage["stale_unreachable"])
        if coverage["stale_unreachable"]:
            print(f"Stale unreachable-line entries: {coverage['stale_unreachable']}", flush=True)
        report["proposed_exempt_lines"] = sum(len(m["proposed_exempt_lines"]) for m in coverage["methods"]
                                              if m["validation"] == "compared")
        if any(m["proposed_exempt_lines"] for m in coverage["methods"]):
            print("Coverage passes only with proposed unreachable lines, which await maintainer acceptance", flush=True)
        inventory = read_inventory()
        report["stale_accepted"] = stale_accepted(inventory, accepted_mutants)
        if report["stale_accepted"]:
            print(f"Stale accepted-mutant entries: {report['stale_accepted']}", flush=True)
        integrity = integrity_module.check(inventory, findings=lint_findings)
        (output / "integrity.json").write_text(json.dumps(integrity, indent=2) + "\n")
        report["integrity"] = {"lint": len(integrity["lint"]), "complete": integrity["complete"]}
        print(f"Integrity: {report['integrity']}; {output / 'integrity.json'}", flush=True)
        ratchet_report = ratchet.check(coverage, integrity, bool(args.method_type), args.profile, baseline_path, baseline)
        (output / "ratchet.json").write_text(json.dumps(ratchet_report, indent=2) + "\n")
        report["ratchet"] = {"ok": ratchet_report["ok"], "regressions": ratchet_report["regressions"]}
        print(f"Ratchet: {'ok' if ratchet_report['ok'] else ratchet_report['regressions']}", flush=True)
        mutation_ok = mutation_complete = True
        mutate_types = set(args.method_type) if args.mutate else set()
        mutate_methods = {}
        if args.mutate_since:
            # Also refuses PDB paths outside the repository, whichever scope applies.
            types = changed_types(coverage, changed_names, (baseline or {}).get("tests"), sources,
                                  inventory.get("test_types"))
            report["mutate_since"] = args.mutate_since
            if args.mutate_all or args.mutate_types_wide or not (baseline or {}).get("bodies"):
                # Without the baseline's body hashes nothing says which methods changed: whole types, as before.
                mutate_types |= types
                report["mutation_scope"] = ("types (requested)" if args.mutate_types_wide or args.mutate_all
                                            else "types (baseline has no body hashes)")
            else:
                mutate_methods, notes = changed_methods(coverage, changed_names, baseline, sources,
                                                        inventory.get("test_types"))
                report["mutation_scope"] = "changed methods"
                report["mutation_notes"] = notes
                for note in notes:
                    print(f"Mutation selection: {note}", flush=True)
        if args.mutate_all:
            mutate_types |= {m["type"] for m in coverage["methods"]}
            report["mutation_scope"] = "all compared methods (requested)"
        if mutate_types or mutate_methods:
            mutant_workers = mutate.effective_jobs(args.jobs)
            print(f"Mutating compared methods with {mutant_workers} workers (requested {args.jobs}); "
                  "each mutant reruns the differential tests", flush=True)
            # Copies of the test output live in a directory of their own, outside the results that code under test
            # could otherwise reach from them; removed afterwards, and by the merge gate if this process is killed.
            scratch = holder["mutation"] = Path(tempfile.mkdtemp(prefix=SCRATCH_PREFIX))
            (output / SCRATCH_RECORD).write_text(str(scratch) + "\n")
            mutation = mutate.run(coverage, mutate_types, assemblies["Quire.Focused.Differential.Tests"], "Quire.Mpl",
                                  args.dotnet, args.jobs, args.timeout, accepted_mutants, global_json,
                                  proposed_mutants, mutant_cache_allowed(args), install, methods=mutate_methods,
                                  scratch=scratch)
            seal.verify("mutation")
            (output / "mutation.json").write_text(json.dumps(mutation, indent=2) + "\n")
            report["mutation"] = {k: mutation[k] for k in ("total", "killed", "accepted_survivors", "survivors",
                                                           "proposed_survivors", "stale_proposed")}
            # Timeouts that held after retry, and crashes: kills without a comparison against Mathcad.
            report["mutation"].update({k: mutation[k] for k in (
                "timeouts", "timed_out", "timeout_retries", "timeout_retry_jobs", "timeout_retry_controls",
                "timeout_retry_elapsed_seconds", "crashes")})
            report["mutation"]["mutated_methods"] = len(mutation["mutated_methods"])
            report["mutation"]["jobs"] = mutation["jobs"]
            report["mutation"]["requested_jobs"] = args.jobs
            report["mutation"]["complete"] = mutation["complete"]
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
                              and not proposals and not report["stale_accepted"])
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
        try:
            report["verdict"] = verdict(report)
        except Exception as error:  # the summary must still be written
            report["verdict"] = {"line": f"LOCAL CHECKS: fail · GATE: would block (verdict failed: {error!r})",
                                 "local_checks": "fail", "gate": "would block", "reasons": [repr(error)],
                                 "pending_approval": []}
        (output / "summary.json").write_text(json.dumps(report, indent=2) + "\n")
        print(f"Report: {output / 'summary.json'}", flush=True)
        print(report["verdict"]["line"], flush=True)


if __name__ == "__main__":
    sys.exit(main())
