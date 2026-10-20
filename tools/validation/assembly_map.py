"""Generate the assembly dependency map and the order in which assemblies become ready.

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

from release_il import ROOT


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
    for path in (ROOT / 'src' / project).rglob('*.cs'):
        if not {'bin', 'obj'}.intersection(path.relative_to(ROOT / 'src' / project).parts):
            count += path.read_text(encoding='utf-8-sig').count('MirrorStub.NotImplemented(')
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


def mirror_status(project, originals, stub_count, audit, revision, clean_sources):
    if not originals:
        return 'support', None
    if any(a.get('error') for a in originals):
        return 'reference unavailable', None
    if not audit:
        return ('incomplete' if stub_count else 'unconfirmed'), None
    rows = audit['rows']
    counts = {'exact': sum(r.get('exact_match') is True for r in rows),
              'declarations': sum(r.get('declaration_only') is True and r.get('shape_matches') is True for r in rows),
              'remaining': sum(not (r.get('exact_match') is True and r.get('shape_matches') is True
                                   or r.get('declaration_only') is True and r.get('shape_matches') is True) for r in rows),
              'report': audit['report']}
    # Whole-assembly completion also needs original-to-Quire coverage, not just Quire-to-original matching.
    covered = {(r['original']['assembly'].lower(), r['original']['token_hex']) for r in rows
               if r.get('original') and r.get('shape_matches') is True
               and (r.get('exact_match') is True or r.get('declaration_only') is True)}
    expected = {(a['name'].lower(), token) for a in originals for token in a.get('expected_methods', [])}
    counts['missing_original_methods'] = len(expected - covered)
    identities = {r['original']['assembly'].lower(): r['original']['mvid'] for r in rows if r.get('original')}
    current = (clean_sources and audit['receipt'].get('tip') == revision and not audit['errors']
               and all(identities.get(a['name'].lower()) == a.get('mvid') for a in originals))
    complete = (current and not stub_count and not counts['remaining'] and bool(expected)
                and not counts['missing_original_methods'] and not any(a.get('error') for a in originals))
    counts['current_revision'] = current
    return ('complete' if complete else 'incomplete' if stub_count else 'unconfirmed'), counts


def render(data):
    projects = data['projects']
    by_name = {p['name']: p for p in projects}
    graph = {p['name']: p['dependencies'] for p in projects}
    ids = {name: f'a{i}' for i, name in enumerate(graph)}
    lines = ['# Assembly dependency map', '',
             'Generated from evaluated Release project references and installed Mathcad assembly metadata.',
             f"Snapshot: {data['generated_at']}; revision `{data['revision'][:12]}`.", '',
             'This is a planning aid for step 1 of the [five-step workflow](PLAN.md). It adds no validation step.', '',
             'An arrow **A → B** means B depends on A: complete A before starting B in an assembly-by-assembly plan.',
             'Frameworks and restored packages are treated as available. `Quire.Mirror` is support code, not a Mathcad mirror.', '',
             '## Assemblies with no references / no counterpart', '',
             '| Meaning | Assemblies |', '|---|---|',
             '| No Quire project references | ' + ', '.join(f"`{p['name']}`" for p in projects if not p['dependencies']) + ' |',
             '| No Mathcad counterpart | ' + ', '.join(f"`{p['name']}`" for p in projects if not p['originals']) + ' |',
             '| No unfinished mirroring prerequisite; ready now | ' + ', '.join(f'`{p}`' for p in data['ready_now']) + ' |', '',
             'Original Mathcad roots (no reference to another assembly in the mapped Mathcad set): ' +
             ', '.join(f"`{a}`" for a in data['original_roots']) + '.', '',
             'The planning graph combines actual project references with mapped original references. Stub projects',
             'can omit references that the finished implementation will need. Those original dependencies still count.', '',
             '“Ready” concerns dependencies, not ownership. Check [WORK](WORK.md) before claiming files.', '',
             '## What completing a ready assembly unlocks', '',
             'Each row assumes only that assembly is newly completed; other unfinished assemblies remain unfinished.', '',
             '| Complete next | Newly eligible assemblies |', '|---|---|']
    for name in data['ready_now']:
        unlocked = data['initial_unlocks'][name]
        lines.append(f"| `{name}` | " + (', '.join(f'`{p}`' for p in unlocked) or 'None yet; other prerequisites remain') + ' |')
    lines += ['',
             '## Quire assembly completion order', '',
             'Assemblies in a row can be worked on independently after every required earlier dependency is complete.',
             'The rows are dependency order, not a claim that any assembly is already complete.', '',
             '| Wave | Assemblies |', '|---|---|']
    for i, layer in enumerate(data['waves'], 1):
        lines.append(f"| {i} | " + ', '.join(f'`{p}`' for p in layer) + ' |')
    if data['blocked']:
        lines += ['', 'Unresolved dependency cycles or missing projects: ' + ', '.join(data['blocked'])]
    lines += ['', '## Original Mathcad assembly completion order', '',
              'This finer view follows the original binaries before Quire consolidates some of them into one project.',
              'It assumes no original binary has been independently confirmed complete. These are dependency waves,',
              'not completion claims. External framework/package references are omitted.', '',
              '| Wave | Original assemblies |', '|---|---|']
    for i, layer in enumerate(data['original_waves'], 1):
        lines.append(f"| {i} | " + ', '.join(f'`{p}`' for p in layer) + ' |')
    if data['original_blocked']:
        lines += ['', 'Original cycles or unresolved dependencies: ' + ', '.join(data['original_blocked'])]
    if data['assume_complete']:
        lines += ['', '## Scenario', '',
                  'Assumed complete (simulation only): ' + ', '.join(f'`{p}`' for p in data['assume_complete']),
                  '', 'Newly unlocked: ' + ', '.join(f'`{p}`' for p in data['scenario_unlocked']),
                  '', 'All ready under that assumption: ' + ', '.join(f'`{p}`' for p in data['scenario_ready'])]
    lines += ['', '## Dependency graph', '',
              'Solid arrows are current project references. Dashed arrows come from the original Mathcad assembly',
              'references and are not yet wired into the stub project. Transitive references are omitted here.', '',
              '```mermaid', 'flowchart TD']
    for name in graph:
        suffix = ' (support)' if by_name[name]['status'] == 'support' else ''
        lines.append(f'    {ids[name]}["{name}{suffix}"]')
    for dependency, project in reduced_edges(graph):
        arrow = '-->' if dependency in by_name[project]['project_references'] else '-.->'
        lines.append(f'    {ids[dependency]} {arrow} {ids[project]}')
    lines += ['```', '', '## Assembly map and current status', '',
              'Stub counts are source evidence of incompleteness. IL counts are the last recorded Release audit;',
              'a passing batch report alone does not establish whole-assembly completion.', '',
              'Scope follows the existing tier lists; dependency order does not authorize deferred/out-of-scope work.', '',
              '| Quire assembly | Mathcad counterpart(s) | Required assemblies | Stub bodies | Scope | Status | Last recorded exact IL |',
              '|---|---|---|---:|---|---|---|']
    for p in projects:
        originals = ', '.join(f"`{a['file']}`" for a in p['originals']) or 'None (support)'
        dependencies = ', '.join(f'`{d}`' for d in p['dependencies']) or 'None'
        audit = p['audit']
        count = f"{audit['exact']} exact; {audit['declarations']} declarations; {audit['remaining']} unresolved" if audit else 'Not recorded'
        lines.append(f"| `{p['name']}` | {originals} | {dependencies} | {p['stub_bodies']} | {p['scope']} | {p['status']} | {count} |")
    lines += ['', '<details>', '<summary>All original assembly references and their Quire targets</summary>', '',
              '| Original assembly | Quire target | Other mapped original references |', '|---|---|---|']
    for original in data['original_assemblies']:
        deps = data['original_graph'][original['name']]
        lines.append(f"| `{original['name']}` | `{original.get('project')}` | " +
                     (', '.join(f'`{p}`' for p in deps) or 'None') + ' |')
    lines += ['', '</details>']
    lines += ['', '## Regenerate / explore the next unlocks', '', '```powershell',
              'python tools/validation/assembly_map.py',
              'python tools/validation/assembly_map.py --assume-complete Quire.Core', '```', '',
              'The scenario changes the readiness calculation only. It does not grant an IL pass or edit completion records.',
              'Whole-assembly completion requires all mapped original methods to be represented and every compiled method',
              'to match, with current artifact identities. A source with no stubs is not sufficient.', '',
              'The companion `assembly-map.json` includes full project edges, external packages, original assembly',
              'reference edges, scope counts, and the original-to-Quire mapping. Original references within a consolidated',
              'Quire project are internal to that project. The planning order includes both original and current build dependencies.', '']
    return '\n'.join(lines)


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
    by_project = defaultdict(list)
    for original in originals:
        if original.get('target_exists'):
            by_project[original['project']].append(original)
    paths = sorted((ROOT / 'src').glob('*/*.csproj'))
    print('Evaluating Release project references', flush=True)
    with ThreadPoolExecutor(max_workers=4) as pool:
        projects = list(pool.map(lambda p: evaluate(p, args.dotnet), paths))
    revision = command(['git', 'rev-parse', 'HEAD']).strip()
    clean_sources = not command(['git', 'status', '--porcelain', '--', 'src',
                                 'Directory.Build.props', 'Directory.Build.targets', 'global.json']).strip()
    audits = recorded_audits()
    original_lookup = {a['name'].lower(): a for a in originals}
    tiers = defaultdict(lambda: defaultdict(int))
    with (ROOT / 'docs/mirror/tiers/types.tsv').open(encoding='utf-8', newline='') as stream:
        for row in csv.DictReader(stream, delimiter='\t'):
            original = original_lookup.get(Path(row['assembly']).stem.lower())
            if original:
                for tier in ('A', 'A2', 'B', 'C', 'D', 'abstract'):
                    tiers[original['project']][tier] += int(row[tier])
    for project in projects:
        project['originals'] = by_project[project['name']]
        project['project_references'] = project['dependencies']
        project['original_dependencies'] = sorted({original_lookup[r.lower()]['project']
            for a in project['originals'] for r in a.get('references', [])
            if r.lower() in original_lookup and original_lookup[r.lower()]['project'] != project['name']
            and original_lookup[r.lower()]['project'] is not None})
        project['dependencies'] = sorted(set(project['project_references']) | set(project['original_dependencies']))
        project['tiers'] = dict(tiers[project['name']])
        counts = project['tiers']
        project['scope'] = ('support' if not project['originals'] else 'active / mixed' if counts.get('A', 0) + counts.get('B', 0)
                            else 'A2 stretch' if counts.get('A2') else 'deferred / out of scope')
        project['stub_bodies'] = source_stubs(project['name'])
        project['status'], project['audit'] = mirror_status(project['name'], project['originals'],
            project['stub_bodies'], audits.get(project['name']), revision, clean_sources)
    graph = {p['name']: p['dependencies'] for p in projects}
    unknown = set(args.assume_complete) - graph.keys()
    if unknown:
        parser.error('unknown assemblies: ' + ', '.join(sorted(unknown)))
    available = {p['name'] for p in projects if p['status'] in ('support', 'complete')}
    layers, blocked = waves(graph, available)
    original_graph = {a['name']: sorted({original_lookup[r.lower()]['name'] for r in a.get('references', [])
        if r.lower() in original_lookup and r.lower() != a['name'].lower()}) for a in originals}
    original_layers, original_blocked = waves(original_graph, set())
    scenarios, _ = waves(graph, available | set(args.assume_complete))
    ready_now = layers[0] if layers else []
    initial_unlocks = {}
    for project in ready_now:
        next_layers, _ = waves(graph, available | {project})
        initial_unlocks[project] = sorted(set(next_layers[0] if next_layers else []) - set(ready_now))
    # Token lists are used for completeness, not duplicated into the public map.
    for original in originals:
        original['expected_method_count'] = len(original.pop('expected_methods', []))
    data = {'generated_at': datetime.now(timezone.utc).isoformat(timespec='seconds'), 'revision': revision,
            'projects': projects, 'original_assemblies': originals,
            'original_graph': original_graph, 'original_waves': original_layers, 'original_blocked': original_blocked,
            'original_roots': sorted(a['name'] for a in originals if not a.get('error')
                and not any(r.lower() in original_lookup and r.lower() != a['name'].lower() for r in a.get('references', []))),
            'available': sorted(available), 'ready_now': ready_now, 'initial_unlocks': initial_unlocks,
            'waves': layers, 'blocked': blocked, 'assume_complete': args.assume_complete,
            'scenario_ready': scenarios[0] if scenarios else [],
            'scenario_unlocked': sorted(set(scenarios[0] if scenarios else []) - set(layers[0] if layers else []))}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(render(data), encoding='utf-8')
    args.output.with_suffix('.json').write_text(json.dumps(data, indent=2) + '\n', encoding='utf-8')
    print(f"Map: {args.output}; {len(projects)} projects / {len(originals)} originals; "
          f"{len(layers)} project waves / {len(original_layers)} original waves; {len(blocked)} blocked")
    print('Ready now: ' + ', '.join(data['ready_now']))
    if args.assume_complete:
        print('Ready in scenario: ' + ', '.join(data['scenario_ready']))
        print('Newly unlocked: ' + ', '.join(data['scenario_unlocked']))
    return 0


if __name__ == '__main__':
    try:
        sys.exit(main())
    except (OSError, ValueError, subprocess.SubprocessError) as error:
        print(error, file=sys.stderr)
        sys.exit(1)
