"""Reconstruction integrity: decompiler-artifact lint and committed IL listings bound to the original artifact."""
import argparse
import json
import os
from pathlib import Path
import re
from validate import ROOT

ALLOWED = ROOT / 'docs/mirror/lint-allowed.txt'
SKIPPED = {'Generated', 'bin', 'obj'}
# Hand-written src/ and agent-editable differential tests.
ARTIFACTS = [
    ('goto statement', re.compile(r'\bgoto\b')),
    ('compiler-generated name', re.compile(r'CS\$|<>[A-Za-z0-9_]|<[A-Za-z_]\w*>[a-z]__')),
    ('IL offset label', re.compile(r'\bIL_[0-9A-Fa-f]{4}\b')),
    ('decompiler comment', re.compile(r'ILSpy|//\s*Warning:|/\*\s*(Error|OpCode not supported)')),
    ('decompiler local name', re.compile(
        r'\b(?:var|[A-Za-z_][\w.]*(?:<[^;=()]*>)?(?:\[,*\])*\??)\s+'
        r'(?:num|flag|obj|text|array|list|type|val|item|ex|result|enumerator|dictionary|disposable)\d+\b'
        r'\s*(?:=|;|\bin\b|\))')),
    ('line directive', re.compile(r'^\s*#\s*(line|pragma\s+checksum)\b')),
]
# Differential test files compile into the harness assembly: they may not reach the process environment, files,
# the validator's variables, the harness's internals or the mutated IL, generate code or stall. A token blocklist is
# defence in depth only; test diffs still need review.
TEST_RULES = [
    ('global using', re.compile(r'^\s*global\s+using\b')),
    ('forbidden test API', re.compile(
        r'\b(?:Environment|AppContext|Location|System\.IO|File|Directory|Path|Process|ModuleInitializer|DllImport|'
        r'LibraryImport|Emit|AssemblyLoadContext|Marshal|GetMethodBody|GetILAsByteArray|GetTypes|GetAssemblies|'
        r'GetEnvironmentVariable|Sleep|SpinWait|Delay|XunitException|Xunit\.Sdk|MethodComparison\s*\)|'
        r'Type\.GetType|Unsafe|GCHandle)\b|QUIRE_|typeof\s*\(\s*(?:MethodComparison|Reflect|'
        r'Mathcad|Runtime|Side|Scenario|MplRunSides|HarnessException)\s*\)')),
]
# Hand-written Quire.Mpl code also runs inside the harness process.
SRC_RULES = [
    ('forbidden harness reach', re.compile(
        r'\b(?:ModuleInitializer|Xunit|MethodComparison|GetMethodBody|GetILAsByteArray|GetAssemblies|'
        r'GetEnvironmentVariable)\b|QUIRE_(?!MIRROR_)')),
]
TESTS = 'tests/Quire.Differential.Tests'
HEADER = re.compile(r'^// Tool: ilspycmd \S+\n// SHA-256: ([0-9a-f]{64})\n// MVID: ([0-9a-f-]{36})\n', re.M)
CODE = re.compile(r'//[^\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|\$?"(?:[^"\\\n]|\\.)*"|\'(?:[^\'\\\n]|\\.)+\'', re.S)


def allowed(path=ALLOWED):
    """Maintainer-reviewed exceptions: `<repo path> | <rule> | <exact line, trimmed>` per line, reasons in `#`
    comment lines above (protected file). Keyed on the whole line, so an exception never covers new code."""
    if not Path(path).is_file():
        return set()
    entries = set()
    for line in Path(path).read_text().splitlines():
        if line.strip() and not line.startswith('#'):
            fields = [f.strip() for f in line.split(' | ', 2)]
            if len(fields) == 3:
                entries.add(tuple(fields))
    return entries


def top_level(text):
    """Code outside any braces, with comments and string literals blanked: namespaces and top-level types."""
    code = CODE.sub(lambda m: ' ' * len(m[0]) if '\n' not in m[0] else '\n' * m[0].count('\n'), text)
    depth, outside = 0, []
    for ch in code:
        if ch == '{':
            depth += 1
        elif ch == '}':
            depth = max(0, depth - 1)
        elif depth == 0:
            outside.append(ch)
    return ''.join(outside)


def shape(path, text):
    """A differential test file declares `namespace Quire.Differential.Tests;` and one top-level class named after
    the file, so it cannot shadow harness names and every test maps back to its file."""
    outside = top_level(text)
    namespaces = re.findall(r'\bnamespace\s+([\w.]+)\s*([;{]?)', outside)
    types = re.findall(r'\b(class|struct|record|interface|enum|delegate)\s+(?:class\s+|struct\s+)?(\w+)', outside)
    problems = []
    if namespaces != [('Quire.Differential.Tests', ';')]:
        problems.append('file must declare exactly `namespace Quire.Differential.Tests;`')
    if types != [('class', Path(path).stem)]:
        problems.append(f'file must declare exactly one top-level type, `class {Path(path).stem}`; found {types}')
    return problems


def agent_tests(repo):
    """Every C# file under the differential test directory that agents may write (matching protected.txt)."""
    from merge_gate import is_protected, patterns
    rules = patterns((Path(__file__).parent / 'protected.txt').read_text())
    found = []
    for directory, subdirectories, files in os.walk(repo / TESTS):
        subdirectories[:] = [d for d in subdirectories if d not in ('bin', 'obj')]
        for f in files:
            path = Path(directory) / f
            if f.lower().endswith('.cs') and not is_protected(path.relative_to(repo).as_posix(), rules):
                found.append(path)
    return found


def lint(repo=ROOT, allow=None):
    """Decompiler artifacts in hand-written source and harness escapes in differential tests and Quire.Mpl."""
    repo = Path(repo)
    allow = allowed(repo / 'docs/mirror/lint-allowed.txt') if allow is None else allow
    sources = []
    for directory, subdirectories, files in os.walk(repo / 'src'):
        subdirectories[:] = [d for d in subdirectories if d not in SKIPPED]
        mpl = Path(directory).relative_to(repo).as_posix().lower().startswith('src/quire.mpl')
        sources += [(Path(directory) / f, ARTIFACTS + (SRC_RULES if mpl else [])) for f in files if f.endswith('.cs')]
    tests = agent_tests(repo) if (repo / TESTS).is_dir() else []
    sources += [(path, ARTIFACTS + TEST_RULES) for path in tests]
    findings = []
    for path, rules in sorted(sources, key=lambda item: item[0]):
        name = path.relative_to(repo).as_posix()
        text = path.read_text(encoding='utf-8-sig')
        if path in tests:
            findings += [{'file': name, 'line': 0, 'rule': 'test file shape', 'match': p, 'text': ''}
                         for p in shape(path, text)]
        for number, line in enumerate(text.splitlines(), 1):
            for rule, pattern in rules:
                for match in pattern.finditer(line):
                    if (name, rule, line.strip()) not in allow:
                        findings.append({'file': name, 'line': number, 'rule': rule, 'match': match[0].strip(), 'text': line.strip()})
    return findings


def listings(inventory, repo=ROOT, types=()):
    """Every analyzed-or-later Mathcad type needs a listing whose header names the artifact the inventory loaded."""
    directory = Path(repo) / 'docs/mirror/il'
    artifacts, required = {}, {}
    for method in inventory['methods']:
        original = method.get('original')
        if not original:
            continue
        artifacts[original['assembly']] = (original['sha256'].lower(), original['mvid'].lower())
        if method['status'] in ('Stub', 'Unclassified') or (types and method['type'] not in types):
            continue
        top = original['type'].split('+')[0]
        required.setdefault((original['assembly'], top), method['type'])
    errors, unchecked = [], []
    for (assembly, top), quire in sorted(required.items()):
        if not (directory / assembly / f'{top}.il').is_file():
            errors.append(f'{quire}: missing IL listing il/{assembly}/{top}.il')
    for path in sorted(directory.glob('*/*.il')):
        name = path.relative_to(directory).as_posix()
        if path.parent.name not in artifacts:
            unchecked.append(f'{name}: assembly not loaded by this audit')
            continue
        match = HEADER.search(path.read_text(encoding='utf-8', errors='replace')[:4096])
        if not match:
            errors.append(f'{name}: header lacks the "// Tool / SHA-256 / MVID" lines')
        elif (match[1], match[2]) != artifacts[path.parent.name]:
            errors.append(f'{name}: header names another artifact; regenerate from the pinned original')
    return {'required': len(required), 'errors': errors, 'unchecked': unchecked}


def check(inventory, repo=ROOT, types=()):
    findings = lint(repo)
    il = listings(inventory, repo, types)
    return {'complete': not findings and not il['errors'], 'lint': findings, 'listings': il}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--inventory', required=True, type=Path)
    parser.add_argument('--type', action='append', default=[], help='exact Quire declaring type; omit for all')
    args = parser.parse_args()
    report = check(json.loads(args.inventory.read_text()), types=args.type)
    print(json.dumps(report, indent=2))
    return 0 if report['complete'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
