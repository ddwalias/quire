"""Source-style lint for hand-written Quire C# (AGENTS.md): no ILSpy-style local names, no unexplained goto/labels.

A ratchet, not a wall: a file fails only when it has MORE violations of a rule than the same file at the base commit, so
existing debt never blocks a change and cleanup shards need no baseline file (nothing to merge-conflict on).

  python tools/lint/lint.py --base <ref>      lint files changed since <ref> (lane_merge runs this)
  python tools/lint/lint.py --all             list every violation in the tree

Rules (suppress one line with a trailing `// lint-ok: reason`):
  ilspy-name   a declaration of a numbered ILSpy-style local (num2, text2, value3, flag2, ...). Parameters, fields and
               tuple members are not declarations of locals and are not flagged.
  goto         a goto statement or label without a `//` comment on the same or the preceding line saying what the IL
               requires.
"""
import argparse
from pathlib import Path
import re
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
RULES = ('ilspy-name', 'goto')
# ILSpy renames a clashing local to its stem plus a number >= 2: num2, text3, value12.
NAME = r'(?:num|text|array|value|list|flag|val|result|obj|item|count)(?:[2-9]|[1-9]\d)'
TYPE = (r'(?:var|int|uint|long|ulong|short|ushort|byte|sbyte|bool|char|float|double|decimal|string|object'
        r'|[A-Z_][\w.]*(?:<[^;=()]*>)?(?:\[[,\s]*\])*\??)')
DECLARATION = re.compile(rf'(?:^|[({{;,]|\b(?:foreach|for|using|out|ref|else|do)\b)\s*(?:const\s+)?{TYPE}\s+({NAME})\b'
                         rf'\s*(?:=(?!=)|;|\bin\b)|\bout\s+{TYPE}\s+{NAME}\b')
MODIFIERS = re.compile(r'^\s*(?:public|private|protected|internal|static|readonly|extern|unsafe|volatile|override'
                       r'|virtual|abstract|sealed|partial)\b')
GOTO = re.compile(r'\bgoto\b')
LABEL = re.compile(r'^\s*(?!default\b|case\b|else\b)[A-Za-z_]\w*:(?!:)\s*$')
IGNORE = re.compile(r'//\s*lint-ok\b')
STRINGS = re.compile(r'@?"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'')


def checked(path):
    parts = path.replace(chr(92), '/').split('/')
    return (path.endswith('.cs') and parts[0] == 'src'
            and not {'Generated', 'obj', 'bin'} & set(parts))


def code_of(line, in_block):
    """The line without comments and string contents, and whether a block comment is still open."""
    if in_block:
        if '*/' not in line:
            return '', True
        line = line.split('*/', 1)[1]
    line = STRINGS.sub('""', line)
    line = re.sub(r'/\*.*?\*/', '', line)
    if '/*' in line:
        return line.split('/*', 1)[0].split('//', 1)[0], True
    return line.split('//', 1)[0], False


def violations(text):
    """[(line number, rule, line)] for the source text."""
    found, lines, in_block = [], text.splitlines(), False
    for number, line in enumerate(lines, 1):
        code, in_block = code_of(line, in_block)
        if IGNORE.search(line) or not code.strip():
            continue
        if not MODIFIERS.match(code) and DECLARATION.search(code):
            found.append((number, 'ilspy-name', line.strip()))
        if GOTO.search(code) or LABEL.match(code):
            previous = lines[number - 2].strip() if number > 1 else ''
            if '//' not in line and not previous.startswith('//'):
                found.append((number, 'goto', line.strip()))
    return found


def git(*args):
    result = subprocess.run(['git', *args], cwd=ROOT, capture_output=True, text=True, encoding='utf-8',
                            errors='replace')
    return result.stdout if result.returncode == 0 else None


def changed(base):
    names = git('diff', '--name-only', '--diff-filter=AM', f'{base}...HEAD') or ''
    return [name for name in names.splitlines() if checked(name)]


def regressions(base):
    """[(path, rule, before, after, lines)] for files with more violations of a rule than at base."""
    out = []
    base = (git('merge-base', base, 'HEAD') or base).strip()
    for path in changed(base):
        now = violations((ROOT / path).read_text(encoding='utf-8', errors='replace'))
        old = violations(git('show', f'{base}:{path}') or '')
        for rule in RULES:
            after, before = sum(v[1] == rule for v in now), sum(v[1] == rule for v in old)
            if after > before:
                out.append((path, rule, before, after, [v for v in now if v[1] == rule]))
    return out


def report(items):
    for path, rule, before, after, lines in items:
        print(f'LINT {path}: {rule} {before} -> {after}', file=sys.stderr)
        for number, _, line in lines[:8]:
            print(f'    {path}:{number}: {line}', file=sys.stderr)
    if items:
        print('Name locals for what they hold (never num2/value2) and comment every goto/label with what the IL '
              'requires (AGENTS.md). Genuine exception: trailing `// lint-ok: reason`.', file=sys.stderr)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--base')
    parser.add_argument('--all', action='store_true')
    args = parser.parse_args(argv)
    if args.all:
        total = dict.fromkeys(RULES, 0)
        for path in filter(checked, (git('ls-files', 'src') or '').splitlines()):
            text = (ROOT / path).read_text(encoding='utf-8', errors='replace')
            for number, rule, line in violations(text):
                total[rule] += 1
                print(f'{path}:{number}: {rule}: {line}')
        print(total, file=sys.stderr)
        return 0
    if not args.base:
        parser.error('--base or --all')
    items = regressions(args.base)
    report(items)
    return 1 if items else 0


if __name__ == '__main__':
    sys.exit(main())
