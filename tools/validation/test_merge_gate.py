import contextlib
import io
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest import mock
import ratchet
import merge_gate
from merge_gate import (baseline_commit, baseline_regressions, case_collisions, on_branch, patterns, protected,
                        stub_deletions, unvalidated_paths, verdict)

RULES = patterns((Path(__file__).parent / 'protected.txt').read_text())
PASSED = {'projects': [{'project': 'P', 'exit_code': 0, 'passed': 3, 'failed': 0, 'skipped': 0}],
          'ratchet': {'ok': True, 'regressions': []}, 'gate_passed': True}


class MergeGateTests(unittest.TestCase):
    def test_harness_and_owned_documents_are_protected(self):
        self.assertEqual(protected([
            'tools/validation/mutate.py', 'tools/Quire.Tools.MethodAudit/Structure.cs',
            'tests/Quire.Differential.Tests/MethodComparison.cs', 'tests/Quire.Differential.Tests/Mathcad.cs',
            'docs/mirror/validation-baseline.json', 'docs/mirror/accepted-mutants.txt', 'docs/mirror/deviations.md',
            'src/Quire.Mirror/MirrorAttributes.cs', 'Directory.Build.props', 'tests/Quire.Differential.Tests/Reflect.cs',
            'tests/Quire.Differential.Tests/NewHelper.cs', 'src/Directory.Build.targets', 'tests/x/NuGet.Config',
            'global.json', 'src/Quire.Mpl/Quire.Mpl.csproj', 'tests/a.runsettings'], RULES), sorted([
            'tools/validation/mutate.py', 'tools/Quire.Tools.MethodAudit/Structure.cs',
            'tests/Quire.Differential.Tests/MethodComparison.cs', 'tests/Quire.Differential.Tests/Mathcad.cs',
            'docs/mirror/validation-baseline.json', 'docs/mirror/accepted-mutants.txt', 'docs/mirror/deviations.md',
            'src/Quire.Mirror/MirrorAttributes.cs', 'Directory.Build.props', 'tests/Quire.Differential.Tests/Reflect.cs',
            'tests/Quire.Differential.Tests/NewHelper.cs', 'src/Directory.Build.targets', 'tests/x/NuGet.Config',
            'global.json', 'src/Quire.Mpl/Quire.Mpl.csproj', 'tests/a.runsettings']))

    def test_work_paths_are_not_protected(self):
        self.assertEqual(protected([
            'src/Quire.Mpl/Mcd/NumVal.cs', 'tests/Quire.Differential.Tests/NumValDifferentialTests.cs',
            'tests/Quire.Mpl.Tests/Mcd/NumValTests.cs', 'docs/mirror/evidence/Mcd.NumVal.md',
            'docs/mirror/WORK.md', 'docs/mirror/status/Quire.Mpl.txt'], RULES), [])

    def test_case_variants_and_subdirectories_stay_protected(self):
        self.assertEqual(protected([
            'tests/Quire.Differential.Tests/Sub/EvilDifferentialTests.cs', 'src/Quire.Mpl/Generated/Mirror/X.cs',
            'src/Quire.Mpl/Mcd/Generated/Y.cs', 'TOOLS/validation/validate.py', 'src/Quire.Mpl/Mcd/bin/Evil.cs',
            '.gitattributes', 'docs/mirror/il/mplrun/Mcd.NumVal.il', 'docs/x/Mcd.Dim.IL', '.mirror-cache/a/Mcd.Dim.cs'], RULES), sorted([
            'tests/Quire.Differential.Tests/Sub/EvilDifferentialTests.cs', 'src/Quire.Mpl/Generated/Mirror/X.cs',
            'src/Quire.Mpl/Mcd/Generated/Y.cs', 'TOOLS/validation/validate.py', 'src/Quire.Mpl/Mcd/bin/Evil.cs',
            '.gitattributes', 'docs/mirror/il/mplrun/Mcd.NumVal.il', 'docs/x/Mcd.Dim.IL', '.mirror-cache/a/Mcd.Dim.cs']))
        # Writable, so integrity.py lints every such file whatever its case.
        self.assertEqual(protected(['tests/Quire.Differential.Tests/Evildifferentialtests.cs'], RULES), [])

    def test_paths_outside_the_focused_profile_are_unvalidated(self):
        self.assertEqual(unvalidated_paths([
            'src/Quire.Mpl/Mcd/A.cs', 'Src/Quire.Engine/B.cs', 'tests/Quire.Engine.Tests/C.cs',
            'tests/Quire.Mpl.Tests/D.cs', 'tests/Quire.Differential.Tests/EDifferentialTests.cs', 'docs/x.md']),
            ['Src/Quire.Engine/B.cs', 'tests/Quire.Engine.Tests/C.cs'])

    def test_case_collisions_found(self):
        self.assertEqual(case_collisions(['src/Quire.Mpl/A.cs', 'Src/Quire.Engine/B.cs', 'docs/x.md']), ['Src / src'])
        self.assertEqual(case_collisions(['src/a/A.cs', 'src/a/B.cs']), [])

    def test_baseline_commit_is_the_tip_tree_plus_the_baseline(self):
        import subprocess, tempfile
        with tempfile.TemporaryDirectory() as directory:
            repo = Path(directory)
            def git(*args):
                return subprocess.run(['git', '-c', 'user.name=t', '-c', 'user.email=t@t', *args], cwd=repo, check=True,
                                      capture_output=True, text=True).stdout.strip()
            git('init', '-q')
            (repo / 'src.cs').write_text('committed')
            git('add', '-A'); git('commit', '-qm', 'tip')
            tip = git('rev-parse', 'HEAD')
            (repo / 'src.cs').write_text('tampered in the checkout')
            hooks = repo / '.git/hooks'
            (hooks / 'pre-commit').write_text('#!/bin/sh\nexit 1\n'); (hooks / 'pre-commit').chmod(0o755)
            commit = baseline_commit(tip, b'{}\r\n', 'b', 'r', repo)
            self.assertEqual(git('rev-parse', f'{commit}^'), tip)
            self.assertEqual(git('show', f'{commit}:src.cs'), 'committed')
            # Exactly the bytes checked, not a file re-read later nor a text-mode conversion of them.
            self.assertEqual(subprocess.run(['git', 'cat-file', 'blob', f'{commit}:docs/mirror/validation-baseline.json'],
                                            cwd=repo, check=True, capture_output=True).stdout, b'{}\r\n')

    def test_gate_command_takes_no_proposals_and_no_cache(self):
        command = merge_gate.validation_command(Path('/t'), Path('/r'), 'main', Path('/b.json'), 60)
        self.assertIn('--no-mutant-cache', command)
        self.assertNotIn('--proposed-mutants', command)
        self.assertNotIn('--proposed-unreachable', command)
        self.assertEqual(command[command.index('--mutate-since') + 1], 'main')

    def test_clean_summary_passes(self):
        self.assertEqual(verdict(PASSED), [])

    def test_every_failure_blocks(self):
        cases = [
            (dict(PASSED, error='boom'), 'validation error'),
            (dict(PASSED, projects=[dict(PASSED['projects'][0], failed=1)]), 'tests did not all pass'),
            (dict(PASSED, projects=[dict(PASSED['projects'][0], skipped=1)]), 'tests did not all pass'),
            (dict(PASSED, projects=[dict(PASSED['projects'][0], passed=0)]), 'tests did not all pass'),
            ({k: v for k, v in PASSED.items() if k != 'ratchet'}, 'ratchet did not run'),
            (dict(PASSED, ratchet={'ok': False, 'regressions': ['T: missing rose']}), 'regression: T: missing rose'),
            (dict(PASSED, mutation={'survivors': ['T::M()@IL_0004 add -> sub']}), 'surviving mutant'),
            (dict(PASSED, proposed_mutants='/tmp/proposals.txt'), 'used --proposed-mutants'),
            (dict(PASSED, proposed_unreachable='/tmp/lines.txt'), 'used --proposed-unreachable'),
            (dict(PASSED, executed_unreachable=['T::M() src/T.cs:9 (accepted)']), 'listed unreachable line executed'),
            (dict(PASSED, gate_passed=False), 'did not report gate_passed'),
        ]
        for summary, reason in cases:
            with self.subTest(reason=reason):
                self.assertTrue(any(reason in r for r in verdict(summary)), verdict(summary))


class RepositoryTests(unittest.TestCase):
    """Against a scratch repository: `main` holds a baseline and the protected rules; `work` adds a document."""
    BASELINE = {'schema': ratchet.SCHEMA, 'profile': 'focused', 'types': {'T': {
        'compared': 1, 'total': 2, 'missing': 1, 'structure_mismatch': 0, 'uncovered': 0, 'verified_unsupported': 0}},
        'methods': {'T::A()': 'compared', 'T::B()': 'missing'}, 'tests': {}, 'lint': []}

    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.repo = Path(directory.name) / 'repo'
        (self.repo / 'docs/mirror').mkdir(parents=True)
        (self.repo / 'tools/validation').mkdir(parents=True)
        (self.repo / 'tools/validation/protected.txt').write_text('tools/*\ndocs/mirror/validation-baseline.json\n')
        (self.repo / 'docs/mirror/validation-baseline.json').write_text(json.dumps(self.BASELINE))
        self.git('init', '-q', '-b', 'main')
        self.git('add', '-A'); self.git('commit', '-qm', 'base')
        self.base = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-qb', 'work')
        (self.repo / 'docs/x.md').write_text('work')
        self.git('add', '-A'); self.git('commit', '-qm', 'work')
        self.git('checkout', '-q', 'main')

    def git(self, *args):
        return subprocess.run(['git', '-c', 'user.name=t', '-c', 'user.email=t@t', *args], cwd=self.repo, check=True,
                              capture_output=True, text=True).stdout.strip()

    def regressions(self, baseline):
        return baseline_regressions(self.base, (baseline if isinstance(baseline, str) else json.dumps(baseline)).encode(),
                                    self.repo)

    def test_new_baseline_must_parse_and_hold_the_ratchet(self):
        self.assertEqual(self.regressions(self.BASELINE), [])
        self.assertEqual(self.regressions(dict(self.BASELINE, methods={'T::A()': 'compared', 'T::B()': 'compared'})), [])
        for baseline, reason in [
                ('{"schema": 3,', 'unusable'), ('[]', 'unusable'), ('', 'unusable'), ({'profile': 'focused'}, 'schema'),
                (dict(self.BASELINE, methods={'T::A()': 'missing', 'T::B()': 'missing'}), 'compared became missing'),
                (dict(self.BASELINE, types={}), 'type disappeared'),
                (dict(self.BASELINE, lint=['a.cs | goto statement | goto x;']), 'new lint'),
                ({k: v for k, v in self.BASELINE.items() if k != 'types'}, 'unusable')]:
            with self.subTest(reason=reason):
                self.assertTrue(any(reason in r for r in self.regressions(baseline)), self.regressions(baseline))
        self.assertIn('unusable', baseline_regressions(self.base, b'\xff', self.repo)[0])

    def test_only_commits_on_a_local_branch_can_be_cloned(self):
        self.assertTrue(on_branch(self.git('rev-parse', 'work'), self.repo))
        dangling = self.git('commit-tree', f'{self.base}^{{tree}}', '-p', self.base, '-m', 'dangling')
        self.assertFalse(on_branch(dangling, self.repo))
        self.assertEqual(self.main(dangling)[0], 1)

    def main(self, branch, fail=None):
        """Exit code and gate.json (None when the gate stopped before validation) of a check against the scratch
        repository; git commands whose first argument is `fail` fail."""
        real, root = merge_gate.git, merge_gate.ROOT
        def git(*args, cwd=None, check=True):
            if args[0] == fail:
                raise RuntimeError(f'git {fail}: failed')
            return real(*args, cwd=self.repo if cwd in (None, root) else cwd, check=check)
        results = self.repo.parent / 'results'
        with mock.patch.object(merge_gate, 'git', git), mock.patch.object(merge_gate, 'ROOT', self.repo), \
                mock.patch('sys.argv', ['merge_gate.py', branch, '--results', str(results)]), \
                contextlib.redirect_stdout(io.StringIO()):
            code = merge_gate.main()
        gate = results / 'gate.json'
        return code, json.loads(gate.read_text()) if gate.is_file() else None

    def test_failed_clone_is_recorded_as_blocked(self):
        for command in ('clone', 'remote'):
            with self.subTest(command=command):
                code, gate = self.main('work', fail=command)
                self.assertEqual(code, 1)
                self.assertEqual(gate['reasons'], [f'cannot prepare the validation clone: git {command}: failed'])
                (self.repo.parent / 'results/gate.json').unlink(); (self.repo.parent / 'results').rmdir()

    def test_clone_leaves_no_repository_path_and_checks_out_the_tip(self):
        tree = self.repo.parent / 'tree'
        merge_gate.clone(self.git('rev-parse', 'work'), tree, self.repo)
        self.assertEqual((tree / 'docs/x.md').read_text(), 'work')
        recorded = [p for p in (tree / '.git').rglob('*') if p.is_file() and str(self.repo).encode() in p.read_bytes()]
        self.assertEqual(recorded, [])  # config, reflogs, FETCH_HEAD, packed-refs...


class StubDeletionTests(unittest.TestCase):
    """Agents may delete whole `[MirrorStub]` methods, properties and constructors from Quire.Mpl's generated mirror
    files, and nothing else there; files are compared byte for byte (most real ones are CRLF)."""
    PATH = 'src/Quire.Mpl/Generated/Mirror/Mcd.T.g.cs'
    STUB = '    [global::Quire.Mirror.MirrorStub]'
    LINES = ['// <auto-generated>', '// </auto-generated>', '#nullable disable', '', 'namespace Quire.Mcd;', '',
             '[global::Quire.Mirror.MirrorOf("Mcd.T, mplrun")]', 'public partial class T', '{',
             STUB, '    public static object A() => throw global::Quire.Mirror.MirrorStub.NotImplemented();',
             STUB, '    public int m_field;',
             STUB, '    public T(int x) { }',
             '    [global::Quire.Mirror.MirrorOf("Mcd.T+N, mplrun")]', '    public partial class N', '    {',
             STUB, '        public override string ToString() => throw global::Quire.Mirror.MirrorStub.NotImplemented();',
             '    }',
             STUB, '    public override string Name { get => base.Name; }',
             '    [global::System.Runtime.CompilerServices.IndexerName("Item")]',
             STUB, '    public int this[int i] { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }',
             '}']

    def setUp(self):
        directory = tempfile.TemporaryDirectory()
        self.addCleanup(directory.cleanup)
        self.repo = Path(directory.name)
        self.git('init', '-q', '-b', 'main')
        self.base = self.commit(self.LINES)

    def git(self, *args):
        return subprocess.run(['git', '-c', 'user.name=t', '-c', 'user.email=t@t', '-c', 'core.autocrlf=false', *args],
                              cwd=self.repo, check=True, capture_output=True, text=True).stdout.strip()

    def commit(self, lines, path=PATH, newline='\r\n'):
        file = self.repo / path
        file.parent.mkdir(parents=True, exist_ok=True)
        file.write_bytes((newline.join(lines) + newline).encode())
        self.git('add', '-A'); self.git('commit', '-qm', 'x', '--allow-empty')
        return self.git('rev-parse', 'HEAD')

    def without(self, *indexes):
        return [line for i, line in enumerate(self.LINES) if i not in indexes]

    def deleted(self, lines, path=PATH, newline='\r\n'):
        return stub_deletions(self.base, self.commit(lines, path, newline), path, self.repo)

    def test_whole_methods_properties_and_constructors_may_be_deleted(self):
        for indexes, members in [((9, 10), ['public static object A()']), ((13, 14), ['public T(int x) { }']),
                                 ((18, 19), ['public override string ToString()']),
                                 ((21, 22), ['public override string Name { get => base.Name; }']),
                                 ((9, 10, 13, 14, 18, 19), ['public static object A()', 'public T(int x) { }',
                                                            'public override string ToString()'])]:
            with self.subTest(indexes=indexes):
                found = self.deleted(self.without(*indexes))
                self.assertEqual(len(found), len(members))
                for line, start in zip(found, members):
                    self.assertTrue(line.startswith(start), line)

    def test_line_endings_may_change_with_a_deletion(self):
        self.assertEqual(len(self.deleted(self.without(9, 10), newline='\n')), 1)

    def test_field_stubs_may_go_and_are_checked_against_the_manifest_instead(self):
        self.assertEqual(self.deleted(self.without(11, 12)), ['public int m_field;'])

    def test_constant_stubs_stay(self):
        self.base = self.commit(self.LINES[:-1] + [self.STUB, '    public const int Key = 9;', '}'])
        self.assertIsNone(self.deleted(self.LINES))

    def test_anything_else_stays_maintainer_only(self):
        for name, lines, newline in [
                ('stub under another attribute', self.without(24, 25), '\r\n'),
                ('attribute only', self.without(9), '\r\n'), ('member only', self.without(10), '\r\n'),
                ('header', self.without(0), '\r\n'), ('MirrorOf', self.without(6), '\r\n'),
                ('nested MirrorOf', self.without(15), '\r\n'),
                ('type declaration', self.without(16, 17, 18, 19, 20), '\r\n'), ('blank line', self.without(3), '\r\n'),
                ('edit', [l.replace('NotImplemented()', 'NotImplemented() ') for l in self.LINES], '\r\n'),
                ('addition', self.LINES[:-1] + [self.STUB, '    public int M() => 1;', '}'], '\r\n'),
                ('stub moved', self.without(9, 10)[:-1] + self.LINES[9:11] + ['}'], '\r\n'),
                ('nothing deleted', self.LINES + [''], '\r\n'),
                ('line endings only', self.LINES, '\n'),
                ('form feed hides the next line in a comment',
                 self.LINES[:1] + [self.LINES[1] + '\x0c' + self.LINES[2]] + self.LINES[3:], '\r\n')]:
            with self.subTest(name=name):
                self.assertIsNone(self.deleted(lines, newline=newline))

    def test_only_existing_quire_mpl_generated_mirror_files_qualify(self):
        self.assertIsNone(self.deleted(self.without(9, 10), 'src/Quire.Mpl/Mcd/T.cs'))
        self.assertIsNone(self.deleted(self.without(9, 10), 'src/Quire.Engine/Generated/Mirror/T.g.cs'))
        self.git('rm', '-q', self.PATH); self.git('commit', '-qm', 'gone')
        self.assertIsNone(stub_deletions(self.base, self.git('rev-parse', 'HEAD'), self.PATH, self.repo))


if __name__ == '__main__':
    unittest.main()
