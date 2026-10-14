"""Reconstruction integrity: decompiler-artifact lint and committed IL listings bound to the original artifact."""
import argparse
import json
import os
from pathlib import Path
import re
from validate import ROOT

ALLOW = 'quire-lint: allow'
SKIPPED = {'Generated', 'bin', 'obj'}
ARTIFACTS = [
    ('goto statement', re.compile(r'\bgoto\b')),
    ('compiler-generated name', re.compile(r'CS\$|<>[A-Za-z0-9_]|<[A-Za-z_]\w*>[a-z]__')),
    ('IL offset label', re.compile(r'\bIL_[0-9A-Fa-f]{4}\b')),
    ('decompiler comment', re.compile(r'ILSpy|//\s*Warning:|/\*\s*(Error|OpCode not supported)')),
    ('decompiler local name', re.compile(
        r'\b(?:var|[A-Za-z_][\w.]*(?:<[^;=()]*>)?(?:\[,*\])*\??)\s+'
        r'(?:num|flag|obj|text|array|list|type|val|item|ex|result|enumerator|dictionary|disposable)\d+\b'
        r'\s*(?:=|;|\bin\b|\))')),
]
HEADER = re.compile(r'^// Tool: ilspycmd \S+\n// SHA-256: ([0-9a-f]{64})\n// MVID: ([0-9a-f-]{36})\n', re.M)


def lint(repo=ROOT):
    """Decompiler artifacts in hand-written source; generated mirror stubs and build output are skipped."""
    findings = []
    sources = []
    for directory, subdirectories, files in os.walk(Path(repo) / 'src'):
        subdirectories[:] = [d for d in subdirectories if d not in SKIPPED]
        sources += [Path(directory) / f for f in files if f.endswith('.cs')]
    for path in sorted(sources):
        for number, line in enumerate(path.read_text(encoding='utf-8-sig').splitlines(), 1):
            if ALLOW in line:
                continue
            for name, pattern in ARTIFACTS:
                if pattern.search(line):
                    findings.append({'file': path.relative_to(repo).as_posix(), 'line': number, 'rule': name, 'text': line.strip()})
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
