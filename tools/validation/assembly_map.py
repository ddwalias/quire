"""Generate the assembly dependency map and the order in which assemblies become ready.

One planning unit per Mathcad assembly: its D20 target project, in the original's language.

    python tools/validation/assembly_map.py
    python tools/validation/assembly_map.py --assume-complete Quire.Core

Planning reference for the claim step, not a new acceptance gate. Evaluates project
references with MSBuild in Release and reads original assembly metadata without IL.
Framework/package dependencies are assumed available. Assumptions never mark an actual
assembly complete. Completion requires a full, current Release inventory covering all
mapped original methods, not merely a passing batch report.
"""
import argparse
import csv
from collections import defaultdict
from concurrent.futures import ThreadPoolExecutor
from datetime import datetime, timezone
import json
from pathlib import Path
import subprocess
import sys

from release_il import ROOT, native_pair, toolset_support


def command(args):
    result = subprocess.run(list(map(str, args)), cwd=ROOT, capture_output=True, text=True, check=True)
    return result.stdout


def evaluate(project, dotnet):
    data = json.loads(command([dotnet, 'msbuild', project, '-nologo', '-p:Configuration=Release',
                               '-getProperty:AssemblyName', '-getItem:ProjectReference', '-getItem:PackageReference']))
    references = data.get('Items', {}).get('ProjectReference', [])
    packages = data.get('Items', {}).get('PackageReference', [])
    return {'name': project.stem, 'assembly': data['Properties']['AssemblyName'],
            'dependencies': sorted({Path(r['FullPath']).stem for r in references}),
            'packages': sorted({p['Identity'] for p in packages}),
            'project_file': project.relative_to(ROOT).as_posix()}


def waves(graph, available):
    remaining = set(graph) - available
    done = set(available)
    layers = []
    while remaining:
        ready = sorted(p for p in remaining if set(graph[p]) <= done)
        if not ready:
            return layers, sorted(remaining)
        layers.append(ready)
        done.update(ready)
        remaining.difference_update(ready)
    return layers, []


def reduced_edges(graph):
    """Hide transitive edges in the diagram; the full reference list stays in JSON/tables."""
    def reachable(start, goal):
        pending, seen = [start], set()
        while pending:
            node = pending.pop()
            if node == goal:
                return True
            if node not in seen:
                seen.add(node)
                pending.extend(graph.get(node, []))
        return False
    return [(dependency, project) for project, dependencies in graph.items() for dependency in dependencies
            if not any(other != dependency and reachable(other, dependency) for other in dependencies)]


def source_stubs(project):
    count = 0
    for path in (p for pattern in ('*.cs', '*.cpp', '*.h') for p in (ROOT / 'src' / project).rglob(pattern)):
        if not {'bin', 'obj'}.intersection(path.relative_to(ROOT / 'src' / project).parts):
            text = path.read_text(encoding='utf-8-sig', errors='replace')
            count += text.count('MirrorStub.NotImplemented(') + text.count('MirrorStub::NotImplemented(')
    return count


def recorded_audits():
    """Most recent candidate inventories with their owning receipt (never base inventories)."""
    result = {}
    for path in sorted((ROOT / '.mirror-cache').rglob('release-il.json'), key=lambda p: p.stat().st_mtime):
        inventory = path.parent / 'inventory.json'
        if not inventory.is_file():
            continue
        receipt = json.loads(path.read_text(encoding='utf-8'))
        data = json.loads(inventory.read_text(encoding='utf-8'))
        for project in receipt.get('projects', []):
            rows = [r for r in data['methods'] if r['project'] == project]
            if rows:
                result[project] = {'rows': rows, 'receipt': receipt,
                                   'report': path.relative_to(ROOT).as_posix(), 'errors': data.get('errors', [])}
    return result


def mirror_status(originals, stub_count, audit, revision, clean_sources, split):
    if any(a.get('error') for a in originals):
        return 'reference unavailable', None
    if not audit or not audit['rows']:
        return ('incomplete' if stub_count else 'unconfirmed'), None
    rows = audit['rows']
    # D19: the gate settles native-to-native pairs and unpaired MSVC support members without an IL match; so does the map.
    settled = [r for r in rows if r.get('shape_matches') is True
               and (r.get('exact_match') is True or r.get('declaration_only') is True) or native_pair(r) or toolset_support(r)]
    counts = {'exact': sum(r.get('exact_match') is True for r in rows),
              'declarations': sum(r.get('declaration_only') is True and r.get('shape_matches') is True for r in rows),
              'native_pairs': sum(native_pair(r) for r in rows),
              'toolset_support': sum(toolset_support(r) for r in rows),
              'remaining': len(rows) - len(settled),
              'report': audit['report']}
    # Whole-assembly completion also needs original-to-Quire coverage, not just Quire-to-original matching.
    covered = {(r['original']['assembly'].lower(), r['original']['token_hex']) for r in settled if r.get('original')}
    expected = {(a['name'].lower(), token) for a in originals for token in a.get('expected_methods', [])}
    counts['missing_original_methods'] = len(expected - covered)
    identities = {r['original']['assembly'].lower(): r['original']['mvid'] for r in rows if r.get('original')}
    current = (clean_sources and audit['receipt'].get('tip') == revision and not audit['errors']
               and all(identities.get(a['name'].lower()) == a.get('mvid') for a in originals))
    # D20: an assembly is complete only in its own project, built in its original's language.
    complete = (split and current and not stub_count and not counts['remaining'] and bool(expected)
                and not counts['missing_original_methods'])
    counts['current_revision'] = current
    return ('complete' if complete else 'incomplete' if stub_count else 'unconfirmed'), counts


def unit_audit(audit, original, split):
    """The recorded rows for one original. A split project owns all its rows; a D2 host's rows are attributed by
    their original assembly, and its rows without a counterpart are not reported for any unit."""
    if not audit:
        return None
    rows = audit['rows'] if split else [r for r in audit['rows'] if r.get('original')
                                         and r['original']['assembly'].lower() == original['name'].lower()]
    return {**audit, 'rows': rows}


def render(data):
    units = data['units']
    by_name = {u['name']: u for u in units}
    graph = {u['name']: u['dependencies'] for u in units}
    ids = {name: f'a{i}' for i, name in enumerate(graph)}
    managed = [u for u in units if u['language'] != 'Native']
    lines = ['# Assembly dependency map', '',
             'Generated from installed Mathcad assembly metadata, PE imports and evaluated Release project references.',
             f"Snapshot: {data['generated_at']}; revision `{data['revision'][:12]}`.", '',
             'This is a planning aid for step 1 of the [five-step workflow](PLAN.md). It adds no validation step.', '',
             'Each Mathcad assembly has one Quire target project in its original language ([D20](decisions.md)).',
             'An arrow **A → B** means B depends on A: complete A before starting B. Frameworks and packages are',
             'treated as available. Native projects are link inputs outside the IL gate (D10): their stopgap import',
             'libraries make them available, so they never block a wave. Mpl modules also depend on the Mpl compiler',
             '(`mplasb`, `mplrun`, `msgen`, `msclr`, and native ports of `msmpl` and `mshex`).', '',
             '“Ready” concerns dependencies, not ownership. Check [WORK](WORK.md) before claiming files.', '',
             '## Ready now', '',
             '| Complete next | Language | Newly eligible assemblies |', '|---|---|---|']
    for name in data['ready_now']:
        unlocked = data['initial_unlocks'][name]
        lines.append(f"| `{name}` | {by_name[name]['language']} | " +
                     (', '.join(f'`{p}`' for p in unlocked) or 'None yet; other prerequisites remain') + ' |')
    lines += ['', '## Completion order', '',
              'Assemblies in a row can be worked on independently after every earlier dependency is complete.',
              'The rows are dependency order, not a claim that any assembly is already complete.', '',
              '| Wave | Assemblies |', '|---|---|']
    for i, layer in enumerate(data['waves'], 1):
        lines.append(f"| {i} | " + ', '.join(f'`{p}`' for p in layer) + ' |')
    if data['blocked']:
        lines += ['', '**Cycles or unresolved dependencies (an error under D20):** ' + ', '.join(data['blocked'])]
    if data['assume_complete']:
        lines += ['', '## Scenario', '',
                  'Assumed complete (simulation only): ' + ', '.join(f'`{p}`' for p in data['assume_complete']),
                  '', 'Newly unlocked: ' + ', '.join(f'`{p}`' for p in data['scenario_unlocked']),
                  '', 'All ready under that assumption: ' + ', '.join(f'`{p}`' for p in data['scenario_ready'])]
    lines += ['', '## Hosts and support projects', '',
              'A D2 host still holds assemblies whose target project does not exist yet in the original language.',
              'Splitting one out means creating its target and moving its members in the same change.', '',
              '| Project | Role | Mathcad assemblies it still hosts |', '|---|---|---|']
    for host in data['hosts']:
        lines.append(f"| `{host['name']}` | {host['role']} | " + (', '.join(f'`{a}`' for a in host['hosts']) or 'None') + ' |')
    lines += ['', '## Native projects', '',
              '| Target | Original | Imports from Mathcad DLLs | Imported by |', '|---|---|---|---|']
    for u in units:
        if u['language'] == 'Native':
            lines.append(f"| `{u['name']}` | `{u['original']['file']}` | " +
                         (', '.join(f'`{d}`' for d in u['dependencies']) or 'None') + ' | ' +
                         (', '.join(f'`{d}`' for d in u['dependents']) or 'None') + ' |')
    lines += ['', '## Dependency graph', '',
              'Solid arrows are references of split projects. Dashed arrows come from the original Mathcad references',
              'and native imports. Transitive references are omitted.', '',
              '```mermaid', 'flowchart TD']
    for name in graph:
        lines.append(f'    {ids[name]}["{name} ({by_name[name]["language"]})"]')
    for dependency, unit in reduced_edges(graph):
        arrow = '-->' if dependency in by_name[unit]['project_references'] else '-.->'
        lines.append(f'    {ids[dependency]} {arrow} {ids[unit]}')
    lines += ['```', '', '## Status', '',
              'Only a split project can be complete. Stub counts of an unsplit assembly are its host’s.',
              'IL counts are the last recorded Release audit of that original’s rows.', '',
              '| Target | Original | Language | Host | Required | Stub bodies | Scope | Status | Last recorded exact IL |',
              '|---|---|---|---|---|---:|---|---|---|']
    for u in managed:
        audit = u['audit']
        count = (f"{audit['exact']} exact; {audit['declarations']} declarations; {audit['native_pairs']} native pairs; "
                 f"{audit['remaining']} unresolved") if audit else 'Not recorded'
        host = 'split' if u['split'] else f"`{u['host']}`"
        stubs = u['stub_bodies'] if u['split'] else f"{u['stub_bodies']} (host)"
        lines.append(f"| `{u['name']}` | `{u['original']['file']}` | {u['language']} | {host} | " +
                     (', '.join(f'`{d}`' for d in u['dependencies']) or 'None') +
                     f" | {stubs} | {u['scope']} | {u['status']} | {count} |")
    lines += ['', '## Regenerate / explore the next unlocks', '', '```powershell',
              'python tools/validation/assembly_map.py',
              'python tools/validation/assembly_map.py --assume-complete Quire.Core', '```', '',
              'The scenario changes the readiness calculation only. It does not grant an IL pass or edit completion records.',
              'Whole-assembly completion requires the assembly’s own project, all mapped original methods represented and',
              'every compiled method matching, with current artifact identities. A source with no stubs is not sufficient.', '',
              'The companion `assembly-map.json` holds every unit with its original references, native imports, scope',
              'counts, host and project references.', '']
    return '\n'.join(lines)


MPL_TOOLCHAIN = ['mplasb', 'mplrun', 'msgen', 'msclr', 'msmpl', 'mshex']


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument('--assume-complete', action='append', default=[])
    parser.add_argument('--output', type=Path, default=ROOT / 'docs/mirror/assembly-map.md')
    parser.add_argument('--dotnet', default='dotnet')
    args = parser.parse_args(argv)
    cache = ROOT / '.mirror-cache/assembly-map'
    cache.mkdir(parents=True, exist_ok=True)
    command([args.dotnet, 'build', ROOT / 'tools/Quire.Tools.MirrorGen/Quire.Tools.MirrorGen.csproj',
             '-c', 'Release', '-o', cache / 'tool', '-nologo', '-v:q'])
    originals = json.loads(command([args.dotnet, cache / 'tool/Quire.Tools.MirrorGen.dll', 'assembly-map']))
    errors = [f"{a['file']}: {a['error']}" for a in originals if a.get('error')]
    if errors:
        raise ValueError('Original metadata unavailable; refusing an incomplete dependency map: ' + '; '.join(errors))
    # Deferred originals (D5) have no D20 target in scope; they are not planning units.
    originals = [a for a in originals if a.get('project') and a.get('language') and not a.get('deferred')]
    lookup = {a['name'].lower(): a for a in originals}
    lookup.update({Path(a['file']).stem.lower(): a for a in originals})

    # C++/CLI projects are not evaluated: the .NET SDK cannot load the Visual C++ targets, and their original
    # references already give their edges.
    paths = sorted((ROOT / 'src').glob('*/*.csproj'))
    print('Evaluating Release project references', flush=True)
    with ThreadPoolExecutor(max_workers=4) as pool:
        projects = {p['name']: p for p in pool.map(lambda p: evaluate(p, args.dotnet), paths)}
    revision = command(['git', 'rev-parse', 'HEAD']).strip()
    clean_sources = not command(['git', 'status', '--porcelain', '--', 'src',
                                 'Directory.Build.props', 'Directory.Build.targets', 'global.json']).strip()
    audits = recorded_audits()
    hosted = defaultdict(list)
    for a in originals:
        if a.get('host'):
            hosted[a['host']].append(a)
    tiers = defaultdict(lambda: defaultdict(int))
    with (ROOT / 'docs/mirror/tiers/types.tsv').open(encoding='utf-8', newline='') as stream:
        for row in csv.DictReader(stream, delimiter='\t'):
            original = lookup.get(Path(row['assembly']).stem.lower())
            if original:
                for tier in ('A', 'A2', 'B', 'C', 'D', 'abstract'):
                    tiers[original['project']][tier] += int(row[tier])

    targets = {a['project'] for a in originals}
    support = sorted(p for p in projects if p not in targets and p not in hosted)
    units = []
    for a in originals:
        name = a['project']
        # Split: MirrorGen's Rules.IsSplit, and the target hosts nothing else (D2 hosts reuse some names).
        split = a['language'] != 'Native' and a['target_exists'] and a.get('host') == name and len(hosted[name]) == 1
        refs = [r for r in a.get('references', []) + a.get('native_imports', [])
                if r.lower() in lookup and r.lower() != a['name'].lower()]
        dependencies = {lookup[r.lower()]['project'] for r in refs}
        if a['language'] == 'Mpl':
            dependencies |= {lookup[t]['project'] for t in MPL_TOOLCHAIN if t in lookup}
        project_references = []
        if split and name in projects:
            # Support projects are always available, so they add no planning edge.
            project_references = [r for r in projects[name]['dependencies'] if r in targets]
        dependencies = sorted((dependencies | set(project_references)) - {name})
        unit = {'name': name, 'original': {k: a.get(k) for k in ('name', 'file', 'mvid', 'references', 'native_imports')},
                'language': a['language'], 'host': a.get('host'), 'split': split,
                'project_references': project_references, 'dependencies': dependencies,
                'tiers': dict(tiers[name])}
        counts = unit['tiers']
        unit['scope'] = ('outside the IL gate (D10)' if a['language'] == 'Native'
                         else 'active / mixed' if counts.get('A', 0) + counts.get('B', 0)
                         else 'A2 stretch' if counts.get('A2') else 'deferred / out of scope')
        if a['language'] == 'Native':
            unit['stub_bodies'], unit['status'], unit['audit'] = 0, 'link input', None
        else:
            source = name if split else a['host']
            unit['stub_bodies'] = source_stubs(source)
            unit['status'], unit['audit'] = mirror_status([a], unit['stub_bodies'],
                unit_audit(audits.get(source), a, split), revision, clean_sources, split)
        units.append(unit)
    for unit in units:
        unit['dependents'] = sorted(u['name'] for u in units if unit['name'] in u['dependencies'])

    graph = {u['name']: u['dependencies'] for u in units}
    names = graph.keys() | set(support)
    unknown = set(args.assume_complete) - names
    if unknown:
        parser.error('unknown assemblies: ' + ', '.join(sorted(unknown)))
    available = set(support) | {u['name'] for u in units if u['status'] in ('complete', 'link input')}
    layers, blocked = waves(graph, available)
    scenarios, _ = waves(graph, available | set(args.assume_complete))
    ready_now = layers[0] if layers else []
    initial_unlocks = {}
    for unit in ready_now:
        next_layers, _ = waves(graph, available | {unit})
        initial_unlocks[unit] = sorted(set(next_layers[0] if next_layers else []) - set(ready_now))
    hosts = [{'name': p, 'role': 'D2 host' if p not in targets else 'target and D2 host',
              'hosts': sorted(a['name'] for a in hosted[p] if a['project'] != p)}
             for p in sorted(hosted) if any(a['project'] != p for a in hosted[p])]
    hosts += [{'name': p, 'role': 'support (no Mathcad counterpart)', 'hosts': []} for p in support]
    data = {'generated_at': datetime.now(timezone.utc).isoformat(timespec='seconds'), 'revision': revision,
            'units': units, 'hosts': hosts, 'support': support,
            'available': sorted(available), 'ready_now': ready_now, 'initial_unlocks': initial_unlocks,
            'waves': layers, 'blocked': blocked, 'assume_complete': args.assume_complete,
            'scenario_ready': scenarios[0] if scenarios else [],
            'scenario_unlocked': sorted(set(scenarios[0] if scenarios else []) - set(layers[0] if layers else []))}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(render(data), encoding='utf-8')
    args.output.with_suffix('.json').write_text(json.dumps(data, indent=2) + '\n', encoding='utf-8')
    print(f"Map: {args.output}; {len(units)} units ({sum(u['split'] for u in units)} split), {len(support)} support; "
          f"{len(layers)} waves; {len(blocked)} blocked")
    print('Ready now: ' + ', '.join(data['ready_now']))
    if args.assume_complete:
        print('Ready in scenario: ' + ', '.join(data['scenario_ready']))
        print('Newly unlocked: ' + ', '.join(data['scenario_unlocked']))
    return 1 if blocked else 0


if __name__ == '__main__':
    try:
        sys.exit(main())
    except (OSError, ValueError, subprocess.SubprocessError) as error:
        print(error, file=sys.stderr)
        sys.exit(1)
