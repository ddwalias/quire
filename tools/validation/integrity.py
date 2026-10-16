"""Reconstruction integrity: decompiler-artifact and harness-escape lint, and the differential test assembly's types."""
import argparse
from collections import Counter
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
    ('decompiler comment', re.compile(r'(?i:ilspy)|//\s*Warning:|/\*\s*(Error|OpCode not supported)|^\s*//\s*(SHA-256|MVID):')),
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
    ('assembly or module attribute', re.compile(r'\[\s*(assembly|module)\s*:')),
    ('file-local type', re.compile(r'\bfile\s+(?:(?:static|sealed|abstract|partial|readonly|unsafe|ref)\s+)*'
                                   r'(?:class|struct|record|interface|enum|delegate)\b')),
    ('forbidden test API', re.compile(
        r'\b(?:Environment|AppContext|Location|System\.IO|File|Directory|Path|Process|ModuleInitializer|DllImport|'
        r'LibraryImport|Emit|AssemblyLoadContext|Marshal|GetMethodBody|GetILAsByteArray|GetTypes|GetAssemblies|'
        r'DefinedTypes|ExportedTypes|GetExportedTypes|'
        r'(?:Get|Expand)EnvironmentVariables?|Sleep|SpinWait|Delay|XunitException|Xunit\.Sdk|MethodComparison\s*\)|'
        r'Type\.GetType|Unsafe|GCHandle)\b|\.\s*GetType\s*\(\s*[@$]*"|QUIRE_|typeof\s*\(\s*(?:MethodComparison|Reflect|'
        r'Mathcad|Runtime|Side|Scenario|MplRunSides|HarnessException)\s*\)')),
]
# Hand-written Quire.Mpl code also runs inside the harness process.
SRC_RULES = [
    ('forbidden harness reach', re.compile(
        r'\b(?:ModuleInitializer|Xunit|MethodComparison|GetMethodBody|GetILAsByteArray|GetAssemblies|'
        r'(?:Get|Expand)EnvironmentVariables?|EnvironmentVariables)\b|QUIRE_(?!MIRROR_)|Quire\.Differential')),
]
# Structure is compared on a Release build, tests, coverage and mutation on Debug: both must compile the same code.
# MethodAudit refuses [Conditional] code and Debug and Contract calls from the build itself.
CONFIGURATION_RULES = [('configuration-dependent code', re.compile(r'^\s*#\s*(?:if|elif)\b.*\b(?:DEBUG|RELEASE)\b'))]
TESTS = 'tests/Quire.Differential.Tests'
# C# ends a line comment at any of its line terminators, so code after one cannot hide as comment.
CODE = re.compile(r'//[^\n\r\x85\u2028\u2029]*|/\*.*?\*/|\$*("{3,}).*?\1|\$*@"(?:[^"]|"")*"|@?\$*"(?:[^"\\\n]|\\.)*"|'
                  r"'(?:[^'\\\n]|\\.)+'", re.S)
# `file` is also an English word: these rules skip prose in `//` comments and simple one-line strings (see prose).
PROSE_RULES = {'file-local type'}
# One-line tokens: a line comment; a token this lexer cannot follow (raw, verbatim or block comment); a simple
# string, interpolated string without nested quotes, or character; a stray quote or `$`; anything else.
TOKEN = re.compile(r"""//.*|(?P<stop>"{3}|@["$]|/\*)|"(?:[^"\\]|\\.)*"|\$"(?:[^"\\{}]|\\.|\{\{|\}\}|\{[^"'/@{}$\\]*\})*"|"""
                   r"""'(?:[^'\\]|\\.)+'|(?P<stray>["'$])|[^"'@$/]+|[@/]""")
NOT_LINE_BREAK = re.compile('[^\n\r\v\f\x1c-\x1e\x85\u2028\u2029]')


def allowed(path=ALLOWED):
    """Maintainer-reviewed exceptions: `<repo path> | <rule> | <exact line, trimmed>` per line, reasons in `#`
    comment lines above (protected file). Keyed on the whole line and counted, so an entry covers one occurrence of
    that line: an edited, new or copied line needs its own review."""
    entries = Counter()
    if not Path(path).is_file():
        return entries
    for line in Path(path).read_text().splitlines():
        if line.strip() and not line.startswith('#'):
            fields = [f.strip() for f in line.split(' | ', 2)]
            if len(fields) == 3:
                entries[tuple(fields)] += 1
    return entries


def prose(lines):
    """Per line, the spans of `//` comments and simple one-line literals. Only what a simple lexer reads for certain
    counts: after the first token it cannot follow, no later span in the file does, and preprocessor lines (whose
    text is not tokens) have none, so a construct that fools it can only cause a finding, never hide one."""
    spans, certain = [], True
    for line in lines:
        found = []
        if certain and not re.match(r'\s*#', line):
            for m in TOKEN.finditer(line):
                if m['stop'] or m['stray']:
                    certain = False
                    break
                if m[0].startswith(('//', '"', '$', "'")):
                    found.append(m.span())
        spans.append(found)
    return spans


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
    allow = Counter(allowed(repo / 'docs/mirror/lint-allowed.txt') if allow is None else allow)
    sources = []
    for directory, subdirectories, files in os.walk(repo / 'src'):
        subdirectories[:] = [d for d in subdirectories if d not in SKIPPED]
        mpl = Path(directory).relative_to(repo).as_posix().lower().startswith('src/quire.mpl')
        rules = ARTIFACTS + CONFIGURATION_RULES + (SRC_RULES if mpl else [])
        sources += [(Path(directory) / f, rules) for f in files if f.lower().endswith('.cs')]
    tests = agent_tests(repo) if (repo / TESTS).is_dir() else []
    sources += [(path, ARTIFACTS + TEST_RULES) for path in tests]
    findings = []
    for path, rules in sorted(sources, key=lambda item: item[0]):
        name = path.relative_to(repo).as_posix()
        text = path.read_text(encoding='utf-8-sig')
        if path in tests:
            findings += [{'file': name, 'line': 0, 'rule': 'test file shape', 'match': p, 'text': ''}
                         for p in shape(path, text)]
        # Blanking keeps every line break, so code lines align with the text's.
        lines = text.splitlines()
        code = CODE.sub(lambda m: NOT_LINE_BREAK.sub(' ', m[0]), text).splitlines()
        spans = prose(lines)
        for number, line in enumerate(lines, 1):
            for rule, pattern in rules:
                matches = list(pattern.finditer(line))
                if rule in PROSE_RULES:
                    # The raw line, less prose; or the blanked code, which sees through comments between tokens.
                    matches = {m.start(): m for m in pattern.finditer(code[number - 1])} | {
                        m.start(): m for m in matches if not any(a <= m.start() < b for a, b in spans[number - 1])}
                    matches = [matches[k] for k in sorted(matches)]
                if matches and allow[(name, rule, line.strip())] > 0:
                    allow[(name, rule, line.strip())] -= 1
                    continue
                findings += [{'file': name, 'line': number, 'rule': rule, 'match': m[0].strip(), 'text': line.strip()}
                             for m in matches]
    return findings


def test_types(inventory, repo=ROOT):
    """Each top-level type compiled into the differential test assembly comes only from maintainer-owned files, or
    is the one class an agent test file is named after. Read from the binary, so no source trick can hide a type."""
    from merge_gate import is_protected, patterns
    rules = patterns((Path(__file__).parent / 'protected.txt').read_text())
    root = Path(repo).resolve().as_posix() + '/'
    findings = []
    for entry in inventory.get('test_types', []):
        name, documents = entry['name'], entry['documents']
        def problem(text):
            findings.append({'file': 'Quire.Differential.Tests.dll', 'line': 0, 'rule': 'test assembly type',
                             'match': name, 'text': f'{name}: {text}'})
        if re.match(r'<(>f__AnonymousType|PrivateImplementationDetails>|>[a-z]__)', name):
            continue  # Compiler-emitted helpers (anonymous types, constant data, inline arrays).
        if name.startswith('<'):
            problem('compiler-named type, such as a file-local class; cannot attribute it')
            continue
        if not documents:
            problem('no source lines; cannot attribute the type to a file')
            continue
        outside = [d for d in documents if not Path(d).as_posix().startswith(root)]
        if outside and (name.startswith('Quire.') or not all('/.nuget/packages/' in Path(d).as_posix().lower() for d in outside)):
            problem(f'source outside the repository: {outside[0]}')
            continue
        if outside:
            continue  # Restored package sources, such as the test SDK's entry point.
        relative = [Path(d).as_posix()[len(root):] for d in documents]
        writable = [r for r in relative if not is_protected(r, rules)]
        if writable and (len(relative) != 1 or name != f'Quire.Differential.Tests.{Path(writable[0]).stem}'):
            problem(f'declared in agent-writable {writable[0]}, which may contribute only its own class')
    return findings


def check(inventory, repo=ROOT, findings=None):
    """`findings`: the source lint, when the caller ran it before code under test could touch the sources."""
    findings = (lint(repo) if findings is None else list(findings)) + test_types(inventory, repo)
    return {'complete': not findings, 'lint': findings}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--inventory', required=True, type=Path)
    args = parser.parse_args()
    report = check(json.loads(args.inventory.read_text()))
    print(json.dumps(report, indent=2))
    return 0 if report['complete'] else 1


if __name__ == '__main__':
    raise SystemExit(main())
