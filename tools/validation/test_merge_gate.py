import contextlib
import io
import json
import os
import re
from pathlib import Path
import shutil
import subprocess
import tempfile
import textwrap
import unittest
from unittest import mock
import ratchet
import merge_gate
from merge_gate import (baseline_commit, baseline_regressions, case_collisions, on_branch, patterns, protected,
                        stub_deletions, unvalidated_paths, verdict)

RULES = patterns((Path(__file__).parent / 'protected.txt').read_text())
PASSED = {'projects': [{'project': 'P', 'exit_code': 0, 'passed': 3, 'failed': 0, 'skipped': 0}],
          'ratchet': {'ok': True, 'regressions': []}, 'gate_passed': True,
          'mutation': {'survivors': [], 'timeouts': 1, 'timed_out': ['T::M()@IL_0001 add -> sub'], 'timeout_retries': 3,
                       'crashes': 2}}


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

    def test_prose_allowlist_excludes_runtime_inputs_and_policy(self):
        for path in ('docs/mirror/evidence/Mcd.NumVal.md', 'docs/mirror/manifest/Quire.Mpl.txt',
                     'docs/mirror/type-map.csv', 'docs/mirror/validation-baseline.json',
                     'docs/mirror/accepted-mutants.txt', 'docs/mirror/PLAN.md', 'docs/mirror/validation.md',
                     'docs/mirror/decisions.md', 'tools/validation/merge_gate.py', 'src/Quire.Mpl/Mcd/Dim.cs',
                     'tests/Quire.Differential.Tests/DimDifferentialTests.cs', 'docs/unknown.md', 'readme.md'):
            with self.subTest(path=path):
                self.assertFalse(merge_gate.prose_only('base', 'tip', [path]))
                self.assertFalse(merge_gate.prose_only('base', 'tip', ['README.md', path]))
        self.assertFalse(merge_gate.prose_only('base', 'tip', []))

    def test_prose_allowlist_requires_regular_nonexecutable_files_in_both_trees(self):
        for entry in ('', '120000 blob sha\tREADME.md', '100755 blob sha\tREADME.md', '040000 tree sha\tREADME.md'):
            with self.subTest(entry=entry), mock.patch.object(merge_gate, 'git', return_value=entry):
                self.assertFalse(merge_gate.prose_only('base', 'tip', ['README.md']))
        for mode in ('120000', '100755'):
            with self.subTest(tip_mode=mode), mock.patch.object(merge_gate, 'git', side_effect=[
                    '100644 blob sha\tREADME.md', f'{mode} blob sha\tREADME.md']), \
                    mock.patch.object(merge_gate.subprocess, 'run', return_value=subprocess.CompletedProcess([], 0, b'Note')):
                self.assertFalse(merge_gate.prose_only('base', 'tip', ['README.md']))

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
            git('config', '--local', 'user.name', 't')
            git('config', '--local', 'user.email', 't@t')
            git('config', '--local', 'core.autocrlf', 'true')
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
        self.assertNotIn('--mutate-types-wide', command)
        self.assertIn('--mutate-types-wide', merge_gate.validation_command(Path('/t'), Path('/r'), 'main', Path('/b.json'), 60, True))
        self.assertIn('--mutate-all', merge_gate.validation_command(Path('/t'), Path('/r'), 'main', Path('/b.json'), 60, False, True))

    def test_gate_forwards_the_worker_count(self):
        command = merge_gate.validation_command(Path('/t'), Path('/r'), 'main', Path('/b.json'), 60, jobs=8)
        self.assertEqual(command[command.index('--jobs') + 1], '8')
        with mock.patch('validate.os.cpu_count', return_value=64):
            command = merge_gate.validation_command(Path('/t'), Path('/r'), 'main', Path('/b.json'), 60)
        self.assertEqual(command[command.index('--jobs') + 1], '12')

    def test_a_killed_validators_mutation_scratch_is_removed_and_nothing_else(self):
        import os
        from validate import SCRATCH_PREFIX, SCRATCH_RECORD
        with tempfile.TemporaryDirectory() as results:
            results = Path(results)
            scratch = Path(tempfile.mkdtemp(prefix=SCRATCH_PREFIX))
            (scratch / 'copy').mkdir()
            elsewhere = Path(tempfile.mkdtemp(prefix='quire-other-'))
            self.addCleanup(lambda: __import__('shutil').rmtree(elsewhere, ignore_errors=True))
            self.assertFalse(merge_gate.remove_scratch(results))  # no record: the validator never mutated
            for named in (elsewhere, results, Path('/'), scratch / 'copy'):
                (results / SCRATCH_RECORD).write_text(str(named))
                self.assertFalse(merge_gate.remove_scratch(results), named)
                self.assertTrue(named.exists())
            (results / SCRATCH_RECORD).write_text(f'{scratch}\n')
            if hasattr(os, 'getuid'):
                with mock.patch('merge_gate.os.getuid', return_value=os.getuid() + 1):
                    self.assertFalse(merge_gate.remove_scratch(results))
            self.assertTrue(merge_gate.remove_scratch(results))
            self.assertFalse(scratch.exists())

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

    def test_surviving_mutants_name_their_source_line(self):
        survivors = ['T::M()@IL_0004 add -> sub', 'T::N()@IL_0002 nop']
        details = [dict(key=survivors[0], location='src/T.cs:9', source='return a + b;'),
                   dict(key=survivors[1], location=None, source=None)]
        self.assertEqual(verdict(dict(PASSED, mutation={'survivors': survivors, 'survivor_details': details})),
                         ['surviving mutant: T::M()@IL_0004 add -> sub at src/T.cs:9: return a + b;',
                          'surviving mutant: T::N()@IL_0002 nop'])


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
        (self.repo / 'docs/mirror/WORK.md').write_text('Current checkpoint\n')
        (self.repo / 'README.md').write_text('Project overview\n')
        self.git('init', '-q', '-b', 'main')
        # Scratch repositories must not inherit the host user's global Git identity.
        self.git('config', '--local', 'user.name', 't')
        self.git('config', '--local', 'user.email', 't@t')
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

    def test_baseline_schemas_3_and_4_accepted(self):
        self.assertEqual(self.regressions(dict(self.BASELINE, schema=4, bodies={'T::A()': 'ab'})), [])
        # `exact` needs no new schema: a baseline without it holds none, so the first one with it is accepted.
        self.assertEqual(self.regressions(dict(self.BASELINE, schema=4, exact=['T::A()'])), [])
        self.assertIn('schema', self.regressions(dict(self.BASELINE, schema=5))[0])

    def test_clone_always_takes_the_base_baseline(self):
        files = ['tools/validation/validate.py', 'docs/mirror/validation-baseline.json', 'docs/x.md']
        rules = merge_gate.patterns('tools/*\ndocs/mirror/validation-baseline.json\n')
        self.assertEqual(sorted(merge_gate.restored_paths(files, rules, {}, False)),
                         ['docs/mirror/validation-baseline.json', 'tools/validation/validate.py'])
        self.assertEqual(merge_gate.restored_paths(files, rules, {}, True), ['docs/mirror/validation-baseline.json'])

    def test_checkout_paths_restores_large_literal_path_list(self):
        paths = [f'protected/{i:04d}/' + 'x' * 45 + '.txt' for i in range(600)]
        paths[0] = 'protected/0000/literal[ab].txt'
        for path in paths:
            target = self.repo / path
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text('base')
        self.git('add', '-A'); self.git('commit', '-qm', 'many protected paths')
        base = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-qb', 'many-path-changes')
        for path in paths:
            (self.repo / path).write_text('branch')
        self.git('add', '-A'); self.git('commit', '-qm', 'change many paths')

        merge_gate.checkout_paths(base, paths, cwd=self.repo)

        self.assertEqual((self.repo / paths[0]).read_text(), 'base')
        self.assertEqual((self.repo / paths[-1]).read_text(), 'base')

    def test_only_commits_on_a_local_branch_can_be_cloned(self):
        self.assertTrue(on_branch(self.git('rev-parse', 'work'), self.repo))
        dangling = self.git('commit-tree', f'{self.base}^{{tree}}', '-p', self.base, '-m', 'dangling')
        self.assertFalse(on_branch(dangling, self.repo))
        self.assertEqual(self.main(dangling)[0], 1)

    def main(self, branch, fail=None, extra=()):
        """Exit code and gate.json (None when the gate stopped before validation) of a check against the scratch
        repository; git commands whose first argument is `fail` fail."""
        real, root = merge_gate.git, merge_gate.ROOT
        def git(*args, cwd=None, check=True):
            if args[0] == fail:
                raise RuntimeError(f'git {fail}: failed')
            return real(*args, cwd=self.repo if cwd in (None, root) else cwd, check=check)
        results = self.repo.parent / 'results'
        with mock.patch.object(merge_gate, 'git', git), mock.patch.object(merge_gate, 'ROOT', self.repo), \
                mock.patch('sys.argv', ['merge_gate.py', branch, '--results', str(results), *extra]), \
                contextlib.redirect_stdout(io.StringIO()):
            code = merge_gate.main()
        gate = results / 'gate.json'
        return code, json.loads(gate.read_text()) if gate.is_file() else None

    @contextlib.contextmanager
    def validation(self):
        real, commands = subprocess.run, []
        def run(command, *args, **kwargs):
            if '--write-baseline' not in command:
                return real(command, *args, **kwargs)
            commands.append(command)
            results = Path(command[command.index('--results') + 1])
            results.mkdir(parents=True)
            (results / 'summary.json').write_text(json.dumps(PASSED))
            Path(command[command.index('--write-baseline') + 1]).write_text(json.dumps(self.BASELINE))
            return subprocess.CompletedProcess(command, 0)
        with mock.patch.object(merge_gate.subprocess, 'run', side_effect=run):
            yield commands

    def test_cli_worker_count_reaches_validation_and_the_gate_record(self):
        with self.validation() as commands:
            code, gate = self.main('work', extra=['--jobs', '8'])
        self.assertEqual(code, 0, gate)
        self.assertEqual(gate['mutation_jobs'], 8)
        self.assertEqual((gate['mutation_timeouts'], gate['mutation_timed_out'], gate['mutation_crashes']),
                         (1, ['T::M()@IL_0001 add -> sub'], 2))
        self.assertEqual(len(commands), 1)
        self.assertEqual(commands[0][commands[0].index('--jobs') + 1], '8')
        self.assertIn('--no-mutant-cache', commands[0])

    def prose_branch(self, path='docs/mirror/WORK.md', content=b'Updated checkpoint\n', name='notes'):
        self.git('checkout', '-qb', name, 'main')
        target = self.repo / path
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(content)
        self.git('add', '-A'); self.git('commit', '-qm', 'notes')
        tip = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-q', 'main')
        return tip

    def test_prose_apply_skips_runtime_validation_and_preserves_the_baseline(self):
        tip = self.prose_branch()
        baseline = self.git('show', 'main:docs/mirror/validation-baseline.json')
        with mock.patch.object(merge_gate, 'clone', side_effect=AssertionError('prose must not clone')), \
                mock.patch.object(merge_gate, 'baseline_commit', side_effect=AssertionError('baseline must not change')):
            code, gate = self.main('notes', extra=['--apply'])
        self.assertEqual(code, 0)
        self.assertEqual(gate['validation_mode'], 'prose')
        self.assertEqual(gate['baseline_reused_from'], self.base)
        self.assertIsNone(gate['validator_exit'])
        self.assertIsNone(gate['results'])
        self.assertEqual(self.git('rev-parse', 'main'), tip)
        self.assertEqual(self.git('show', 'main:docs/mirror/validation-baseline.json'), baseline)

    def test_prose_check_does_not_move_main(self):
        self.prose_branch(path='README.md')
        with mock.patch.object(merge_gate, 'clone', side_effect=AssertionError('prose must not clone')):
            code, gate = self.main('notes')
        self.assertEqual(code, 0)
        self.assertEqual(gate['validation_mode'], 'prose')
        self.assertEqual(self.git('rev-parse', 'main'), self.base)

    def test_explicit_validation_and_audits_override_the_prose_shortcut(self):
        import shutil
        self.prose_branch()
        for flag in ('--full-validation', '--mutate-all', '--mutate-types-wide'):
            with self.subTest(flag=flag), self.validation() as commands:
                code, gate = self.main('notes', extra=[flag])
                self.assertEqual(code, 0)
                self.assertEqual(gate['validation_mode'], 'runtime')
                self.assertEqual(len(commands), 1)
            shutil.rmtree(self.repo.parent / 'results')

    def test_mixed_prose_and_other_changes_use_runtime_validation(self):
        self.git('checkout', '-q', 'work')
        (self.repo / 'docs/mirror/WORK.md').write_text('Updated checkpoint')
        self.git('add', '-A'); self.git('commit', '-qm', 'notes with other docs')
        self.git('checkout', '-q', 'main')
        with self.validation() as commands:
            code, gate = self.main('work')
        self.assertEqual(code, 0)
        self.assertEqual(gate['validation_mode'], 'runtime')
        self.assertEqual(len(commands), 1)

    def test_new_or_deleted_allowlisted_documents_use_runtime_validation(self):
        import shutil
        self.prose_branch(path='docs/mirror/STATE.md')
        self.git('checkout', '-qb', 'removed', 'main')
        self.git('rm', 'README.md'); self.git('commit', '-qm', 'remove readme')
        self.git('checkout', '-q', 'main')
        for branch in ('notes', 'removed'):
            with self.subTest(branch=branch), self.validation() as commands:
                code, gate = self.main(branch)
                self.assertEqual(code, 0)
                self.assertEqual(gate['validation_mode'], 'runtime')
                self.assertEqual(len(commands), 1)
            shutil.rmtree(self.repo.parent / 'results')

    def test_binary_and_non_utf8_documents_use_runtime_validation(self):
        import shutil
        for i, content in enumerate((b'Note\0data', b'\xff')):
            branch = f'notes{i}'
            self.prose_branch(content=content, name=branch)
            with self.subTest(content=content), self.validation() as commands:
                code, gate = self.main(branch)
                self.assertEqual(code, 0)
                self.assertEqual(gate['validation_mode'], 'runtime')
                self.assertEqual(len(commands), 1)
            shutil.rmtree(self.repo.parent / 'results')

    def test_protected_prose_still_uses_runtime_validation(self):
        with (self.repo / 'tools/validation/protected.txt').open('a') as rules:
            rules.write('docs/mirror/WORK.md\n')
        self.git('add', '-A'); self.git('commit', '-qm', 'protect checkpoint')
        self.prose_branch()
        self.assertEqual(self.main('notes'), (1, None))
        with self.validation() as commands:
            code, gate = self.main('notes', extra=['--allow-protected'])
        self.assertEqual(code, 0)
        self.assertEqual(gate['validation_mode'], 'runtime')
        self.assertEqual(gate['protected_changed'], ['docs/mirror/WORK.md'])
        self.assertEqual(len(commands), 1)

    def test_stale_prose_branch_cannot_bypass_the_base_check(self):
        self.prose_branch()
        (self.repo / 'README.md').write_text('Main moved')
        self.git('add', '-A'); self.git('commit', '-qm', 'advance main')
        with mock.patch.object(merge_gate, 'clone', side_effect=AssertionError('stale branch must stop')):
            self.assertEqual(self.main('notes'), (1, None))

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


class PhaseGateTests(unittest.TestCase):
    """A branch changing a Windows project: the phase profile runs after the focused one where Windows' dotnet.exe is
    at hand, and the gate blocks it as before where it is not. RepositoryTests' scratch repository, plus a phase
    baseline on `main` and an `engine` branch."""
    BASELINE, git, main = RepositoryTests.BASELINE, RepositoryTests.git, RepositoryTests.main
    PHASE = dict(RepositoryTests.BASELINE, profile='phase', types={'E': {
        'compared': 0, 'total': 1, 'missing': 1, 'structure_mismatch': 0, 'uncovered': 0, 'verified_unsupported': 0}},
        methods={'E::M()': 'missing'})

    def setUp(self):
        RepositoryTests.setUp(self)
        with (self.repo / 'tools/validation/protected.txt').open('a') as rules:
            rules.write(merge_gate.PHASE_BASELINE + '\n')
        (self.repo / merge_gate.PHASE_BASELINE).write_text(json.dumps(self.PHASE))
        self.git('add', '-A'); self.git('commit', '-qm', 'phase baseline')
        self.base = self.git('rev-parse', 'HEAD')
        self.git('checkout', '-qb', 'engine')
        (self.repo / 'src/Quire.Engine').mkdir(parents=True)
        (self.repo / 'src/Quire.Engine/A.cs').write_text('class A {}\n')
        self.git('add', '-A'); self.git('commit', '-qm', 'engine')
        self.git('checkout', '-q', 'main')
        self.temp = self.repo.parent / 'windows-temp'
        self.temp.mkdir()

    @contextlib.contextmanager
    def runs(self, dotnet='/mnt/c/dotnet/dotnet.exe', focused=PASSED, phase=PASSED, phase_baseline=None):
        real, commands = subprocess.run, []
        new_phase = phase_baseline or dict(self.PHASE, methods={'E::M()': 'compared'})
        def run(command, *args, **kwargs):
            if '--write-baseline' not in command:
                return real(command, *args, **kwargs)
            commands.append(command)
            is_phase = '--profile' in command
            results = Path(command[command.index('--results') + 1])
            results.mkdir(parents=True)
            (results / 'summary.json').write_text(json.dumps(phase if is_phase else focused))
            Path(command[command.index('--write-baseline') + 1]).write_text(
                json.dumps(new_phase if is_phase else self.BASELINE))
            return subprocess.CompletedProcess(command, 0)
        with mock.patch.object(merge_gate.subprocess, 'run', side_effect=run), \
                mock.patch.object(merge_gate, 'phase_dotnet', return_value=dotnet), \
                mock.patch.object(merge_gate, 'phase_temp', return_value=self.temp):
            yield commands

    def gate(self, branch, extra=()):
        """main's exit code, gate.json and what it printed."""
        printed = []
        with mock.patch.object(merge_gate, 'print', create=True, side_effect=lambda *a, **k: printed.append(' '.join(map(str, a)))):
            code, gate = self.main(branch, extra=extra)
        return code, gate, '\n'.join(printed)

    def branch(self, name, files, start='main'):
        self.git('checkout', '-qb', name, start)
        for path, text in files.items():
            (self.repo / path).parent.mkdir(parents=True, exist_ok=True)
            (self.repo / path).write_text(text)
        self.git('add', '-A'); self.git('commit', '-qm', name)
        self.git('checkout', '-q', 'main')

    def test_windows_project_changes_are_unvalidated(self):
        self.assertEqual(merge_gate.unvalidated_paths(['src/Quire.Engine/A.cs', 'src/Quire.Mpl/B.cs']),
                         ['src/Quire.Engine/A.cs'])

    def test_without_phase_the_gate_blocks_as_before(self):
        with self.runs(dotnet=None) as commands:
            out = io.StringIO()
            with contextlib.redirect_stdout(out):
                code, gate = self.main('engine')
        self.assertEqual((code, gate, commands), (1, None, []))

    def test_both_profiles_run_and_must_both_pass(self):
        with self.runs() as commands:
            code, gate = self.main('engine')
        self.assertEqual(code, 0, gate)
        self.assertEqual(len(commands), 2)
        focused, phase = commands
        self.assertNotIn('--profile', focused)
        self.assertEqual(phase[phase.index('--profile') + 1], 'phase')
        self.assertEqual(phase[phase.index('--dotnet') + 1], '/mnt/c/dotnet/dotnet.exe')
        self.assertTrue(phase[phase.index('--baseline') + 1].replace('\\', '/').endswith(merge_gate.PHASE_BASELINE))
        self.assertNotIn('--mutate-since', phase)
        # The phase clone and results live under the Windows temporary directory; the clone is removed.
        self.assertTrue(Path(phase[phase.index('--results') + 1]).is_relative_to(self.temp))
        self.assertFalse(list(self.temp.glob('quire-gate-phase-*/tree')))
        self.assertEqual(gate['phase']['validator_exit'], 0)
        self.assertEqual(gate['phase']['reasons'], [])
        self.assertEqual(gate['validation_mode'], 'runtime')

    def test_a_failing_phase_blocks(self):
        failing = dict(PASSED, projects=[{'project': 'Quire.Engine.Tests', 'exit_code': 1, 'passed': 3, 'failed': 1,
                                          'skipped': 0}])
        with self.runs(phase=failing):
            code, gate = self.main('engine')
        self.assertEqual(code, 1)
        self.assertEqual(gate['reasons'], ['phase: Quire.Engine.Tests: tests did not all pass'])

    def test_a_phase_baseline_regression_blocks(self):
        with self.runs(phase_baseline=dict(self.PHASE, types={})):
            code, gate = self.main('engine')
        self.assertEqual(code, 1)
        self.assertTrue(any(r.startswith('phase: new baseline: E: type disappeared') for r in gate['reasons']), gate)

    def test_a_failing_focused_run_skips_the_phase(self):
        with self.runs(focused=dict(PASSED, ratchet={'ok': False, 'regressions': ['T::A(): compared became missing']})) \
                as commands:
            code, gate = self.main('engine')
        self.assertEqual(code, 1)
        self.assertEqual(len(commands), 1)
        self.assertEqual(gate['phase'], {'skipped': 'the focused validation blocked'})

    def test_allow_unvalidated_keeps_the_maintainer_route(self):
        with self.runs() as commands:
            code, gate = self.main('engine', extra=['--allow-unvalidated'])
        self.assertEqual(code, 0, gate)
        self.assertEqual(len(commands), 1)
        self.assertIsNone(gate['phase'])

    def test_apply_commits_both_baselines(self):
        new_phase = dict(self.PHASE, methods={'E::M()': 'compared'})
        with self.runs(phase_baseline=new_phase):
            code, gate = self.main('engine', extra=['--apply'])
        self.assertEqual(code, 0, gate)
        head = self.git('rev-parse', 'main')
        self.assertEqual(self.git('rev-parse', f'{head}^'), self.git('rev-parse', 'engine'))
        self.assertEqual(sorted(self.git('diff', '--name-only', 'engine', head).splitlines()),
                         [merge_gate.PHASE_BASELINE])  # the focused baseline bytes equal the tip's here
        self.assertEqual(json.loads(self.git('show', f'{head}:{merge_gate.PHASE_BASELINE}')), new_phase)
        self.assertEqual(json.loads(self.git('show', f'{head}:{merge_gate.BASELINE}')), self.BASELINE)

    def test_phase_baseline_is_restored_from_the_base_and_protected(self):
        files = ['tools/validation/validate.py', merge_gate.BASELINE, merge_gate.PHASE_BASELINE, 'docs/x.md']
        rules = merge_gate.patterns((Path(__file__).parent / 'protected.txt').read_text())
        self.assertEqual(merge_gate.restored_paths(files, rules, {}, True), [merge_gate.BASELINE, merge_gate.PHASE_BASELINE])
        self.assertEqual(merge_gate.protected([merge_gate.PHASE_BASELINE], rules), [merge_gate.PHASE_BASELINE])

    def test_baseline_commit_writes_only_the_baselines(self):
        tip = self.git('rev-parse', 'engine')
        commit = baseline_commit(tip, b'{"f": 1}\n', 'engine', 'r', self.repo, b'{"p": 1}\n')
        self.assertEqual(sorted(self.git('diff', '--name-only', tip, commit).splitlines()),
                         sorted([merge_gate.BASELINE, merge_gate.PHASE_BASELINE]))
        only_focused = baseline_commit(tip, b'{"f": 2}\n', 'engine', 'r', self.repo)
        self.assertEqual(self.git('diff', '--name-only', tip, only_focused), merge_gate.BASELINE)

    def test_a_branch_cannot_change_the_phase_baseline_when_phase_does_not_run(self):
        # Even with every maintainer override that skips the phase run, main keeps the base's phase baseline.
        emptied = json.dumps(dict(self.PHASE, types={}))
        blob = self.git('rev-parse', f'main:{merge_gate.PHASE_BASELINE}')
        for name, files, extra in [
                ('probe', {merge_gate.PHASE_BASELINE: emptied}, ['--allow-protected', '--apply']),
                ('probe-engine', {merge_gate.PHASE_BASELINE: emptied, 'src/Quire.Engine/B.cs': 'class B {}\n'},
                 ['--allow-protected', '--allow-unvalidated', '--apply'])]:
            with self.subTest(name):
                self.branch(name, files)
                shutil.rmtree(self.repo.parent / 'results', ignore_errors=True)
                with self.runs() as commands:
                    code, gate, out = self.gate(name, extra)
                self.assertEqual(code, 0, out)
                self.assertEqual(len(commands), 1)  # the focused run only
                self.assertIsNone(gate['phase'])
                self.assertEqual(self.git('rev-parse', 'main^'), self.git('rev-parse', name))
                self.assertEqual(self.git('rev-parse', f'main:{merge_gate.PHASE_BASELINE}'), blob)
                self.assertEqual(self.git('diff', '--name-only', name, 'main'), merge_gate.PHASE_BASELINE)

    def test_baseline_commit_keeps_the_bases_phase_baseline(self):
        self.branch('probe', {merge_gate.PHASE_BASELINE: '{}'})
        tip = self.git('rev-parse', 'probe')
        commit = baseline_commit(tip, b'{"f": 1}\n', 'probe', 'r', self.repo, None, self.base)
        self.assertEqual(self.git('rev-parse', f'{commit}:{merge_gate.PHASE_BASELINE}'),
                         self.git('rev-parse', f'{self.base}:{merge_gate.PHASE_BASELINE}'))
        # A base without one: a branch cannot add it either.
        first = self.git('rev-list', '--max-parents=0', 'main')
        commit = baseline_commit(tip, b'{"f": 1}\n', 'probe', 'r', self.repo, None, first)
        self.assertNotIn(merge_gate.PHASE_BASELINE, self.git('ls-tree', '-r', '--name-only', commit).splitlines())
        # Any other failure to read the base's phase baseline is not taken for its absence.
        with self.assertRaises(subprocess.CalledProcessError):
            baseline_commit(tip, b'{"f": 1}\n', 'probe', 'r', self.repo, None, '0' * 40)

    def test_no_phase_baseline_on_the_base_blocks_before_any_run(self):
        self.git('rm', '-q', merge_gate.PHASE_BASELINE); self.git('commit', '-qm', 'no phase baseline')
        self.git('rebase', '-q', 'main', 'engine'); self.git('checkout', '-q', 'main')
        with self.runs() as commands:
            code, gate, out = self.gate('engine')
        self.assertEqual((code, gate, commands), (1, None, []))
        self.assertIn('a maintainer must create the phase baseline first', out)

    def test_an_unusable_phase_temp_blocks_before_any_run(self):
        with self.runs() as commands, mock.patch.object(merge_gate, 'phase_temp',
                                                        side_effect=ValueError('/tmp/x is not on a Windows drive')):
            code, gate, out = self.gate('engine')
        self.assertEqual((code, gate, commands), (1, None, []))
        self.assertIn('BLOCKED: phase: /tmp/x is not on a Windows drive', out)

    def test_old_phase_runs_are_pruned(self):
        for age in range(5):
            run = self.temp / f'quire-gate-phase-{age}'
            (run / 'run').mkdir(parents=True)
            os.utime(run, (1000 - age, 1000 - age))
        (self.temp / 'other').mkdir()
        with mock.patch.object(merge_gate, 'print', create=True):
            merge_gate.prune_phase_runs(self.temp, keep=3)
        self.assertEqual(sorted(d.name for d in self.temp.iterdir()), ['other', 'quire-gate-phase-0', 'quire-gate-phase-1'])

    def test_remove_tree_reports_what_it_could_not_remove(self):
        printed = []
        with mock.patch.object(merge_gate.shutil, 'rmtree', side_effect=lambda d, onexc: onexc(
                os.unlink, f'{d}/locked.dll', PermissionError('in use'))), \
                mock.patch.object(merge_gate, 'print', create=True, side_effect=lambda *a, **k: printed.extend(a)):
            self.assertFalse(merge_gate.remove_tree(self.temp))
        self.assertEqual(printed, [f'could not remove {self.temp}/locked.dll: in use'])

    def test_phase_cli_detection(self):
        with mock.patch.object(merge_gate.os, 'name', 'nt'):
            self.assertEqual(merge_gate.phase_dotnet({}), 'dotnet')
        with mock.patch.object(merge_gate.os, 'name', 'posix'):
            self.assertIsNone(merge_gate.phase_dotnet({}))  # plain Linux: no Windows CLI
            cli = self.repo.parent / 'dotnet.exe'
            self.assertIsNone(merge_gate.phase_dotnet({'WSL_DISTRO_NAME': 'x', 'QUIRE_WINDOWS_DOTNET': str(cli)}))
            cli.write_text('')
            self.assertEqual(merge_gate.phase_dotnet({'WSL_DISTRO_NAME': 'x', 'QUIRE_WINDOWS_DOTNET': str(cli)}), str(cli))

    def test_phase_command(self):
        command = merge_gate.phase_command(Path('/w/tree'), Path('/w/run'), Path('/w/b.json'), 900, 'dotnet.exe',
                                           'FullyQualifiedName~PtcPersistentDataOleUtilsDifferentialTests')
        self.assertEqual(command[2:], ['-B', str(Path('/w/tree/tools/validation/validate.py')), '--profile', 'phase',
                                       '--results', str(Path('/w/run')), '--baseline',
                                       str(Path('/w/tree') / merge_gate.PHASE_BASELINE), '--write-baseline',
                                       str(Path('/w/b.json')), '--timeout', '900', '--dotnet', 'dotnet.exe',
                                       '--allow-phase-differential-failures', '--diff-filter',
                                       'FullyQualifiedName~PtcPersistentDataOleUtilsDifferentialTests',
                                       '--allow-differential-skips'])

    def test_phase_diff_filter_uses_changed_differential_classes_but_not_shared_helpers(self):
        self.assertEqual(merge_gate.phase_diff_filter([
            'src/Quire.Core/PersistentData/OleUtils.cs',
            'tests/Quire.Differential.Tests/PtcPersistentDataOleUtilsDifferentialTests.cs']),
            'FullyQualifiedName~PtcPersistentDataOleUtilsDifferentialTests')
        self.assertIsNone(merge_gate.phase_diff_filter([
            'tests/Quire.Differential.Tests/MethodComparison.cs']))


class OptimisticGateTests(unittest.TestCase):
    """--validate-only outside the merge lock, then --apply-validated under it: a passed run lands without validating
    again only when nothing main gained since can change its verdict; otherwise exit 4 (STALE) and validate again.
    PhaseGateTests' scratch repository, with a focused branch `mpl` beside its phase branch `engine`."""
    BASELINE, PHASE = PhaseGateTests.BASELINE, PhaseGateTests.PHASE
    git, main, runs, gate, branch = (RepositoryTests.git, RepositoryTests.main, PhaseGateTests.runs,
                                     PhaseGateTests.gate, PhaseGateTests.branch)

    def setUp(self):
        PhaseGateTests.setUp(self)
        (self.repo / 'src/Quire.Mpl').mkdir(parents=True)
        (self.repo / 'src/Quire.Mpl/Other.cs').write_text('class Other {}\n')
        (self.repo / 'src/Quire.Mpl/M.cs').write_text('class M {}\n')
        self.git('add', '-A'); self.git('commit', '-qm', 'mpl sources')
        self.git('rebase', '-q', 'main', 'engine'); self.git('checkout', '-q', 'main')
        self.branch('mpl', {'src/Quire.Mpl/M.cs': 'class M { int F() => 1; }\n'})

    def validate(self, branch='mpl', extra=()):
        shutil.rmtree(self.repo.parent / 'results', ignore_errors=True)
        with self.runs() as commands:
            code, gate, out = self.gate(branch, ['--validate-only', *extra])
        return code, gate, out, commands

    def apply(self, branch='mpl'):
        code, _, out = self.gate(branch, ['--apply-validated'])
        return code, out

    def on_main(self, files, message='main moves'):
        for path, text in files.items():
            if text is None:
                self.git('rm', '-q', path)
            else:
                (self.repo / path).parent.mkdir(parents=True, exist_ok=True)
                (self.repo / path).write_text(text)
        self.git('add', '-A'); self.git('commit', '-qm', message)

    def record(self, branch='mpl'):
        return merge_gate.read_record(branch, self.repo)

    def assert_stale(self, reason, branch='mpl', setup=None):
        if self.git('merge-base', 'main', branch) != self.git('rev-parse', 'main'):  # after an earlier subtest
            self.git('rebase', '-q', 'main', branch); self.git('checkout', '-q', 'main')
        code, gate, out, _ = self.validate(branch)
        self.assertEqual(code, 0, out)
        main = self.git('rev-parse', 'main')
        (setup or (lambda: None))()
        moved = self.git('rev-parse', 'main')
        code, out = self.apply(branch)
        self.assertEqual(code, merge_gate.STALE, out)
        self.assertIn(reason, out)
        self.assertIn('fall back to --apply under the lock', out)
        self.assertIsNone(self.record(branch))  # deleted: the next attempt must validate again
        self.assertEqual(self.git('rev-parse', 'main'), moved)
        return main

    def test_a_passed_run_records_and_lands_on_an_unchanged_main(self):
        code, gate, out, commands = self.validate()
        self.assertEqual(code, 0, out)
        self.assertEqual(len(commands), 1)
        self.assertIn('RECORDED: refs/quire-gate/mpl', out)
        record, files = self.record()
        self.assertEqual((record['base'], record['tip']), (self.git('rev-parse', 'main'), self.git('rev-parse', 'mpl')))
        self.assertEqual(record['commits'], [f"{self.git('rev-parse', '--short', 'mpl')} mpl"])
        self.assertEqual(record['tools'], merge_gate.tool_blobs(record['base'], self.repo))
        self.assertIn('focused', record['durations'])
        self.assertEqual(set(files), {'baseline.json'})
        self.assertEqual(self.git('rev-parse', 'main'), record['base'])  # validating merges nothing
        code, out = self.apply()
        self.assertEqual(code, 0, out)
        self.assertEqual(self.git('rev-parse', 'main'), self.git('rev-parse', 'mpl'))
        self.assertIsNone(self.record())

    def test_a_blocked_run_records_nothing_and_clears_an_older_record(self):
        self.assertEqual(self.validate()[0], 0)
        with self.runs(focused=dict(PASSED, gate_passed=False)):
            shutil.rmtree(self.repo.parent / 'results')
            code, _, _ = self.gate('mpl', ['--validate-only'])
        self.assertEqual(code, 1)
        self.assertIsNone(self.record())
        code, out = self.apply()
        self.assertEqual(code, 1)
        self.assertIn('no record of a passed --validate-only run', out)

    def test_prose_moves_on_main_replay_the_branch_onto_it(self):
        self.validate()
        self.on_main({'docs/mirror/WORK.md': 'Receipt\n'})
        main = self.git('rev-parse', 'main')
        code, out = self.apply()
        self.assertEqual(code, 0, out)
        self.assertIn('replayed onto', out)
        self.assertEqual(self.git('rev-parse', 'main^'), main)
        self.assertEqual(self.git('show', 'main:docs/mirror/WORK.md'), 'Receipt')
        self.assertEqual(self.git('show', 'main:src/Quire.Mpl/M.cs'), 'class M { int F() => 1; }')
        self.assertEqual(self.git('log', '-1', '--format=%s%x00%an', 'main'), 'mpl\x00t')

    def test_a_prose_only_branch_records_and_replays_without_a_baseline(self):
        self.branch('receipt', {'docs/mirror/WORK.md': 'Receipt\n'})
        code, gate, out, commands = self.validate('receipt')
        self.assertEqual((code, commands), (0, []), out)
        self.assertIn('RECORDED', out)
        self.assertEqual(self.record('receipt')[0]['validation_mode'], 'prose')
        self.on_main({'README.md': 'New overview\n'})
        code, out = self.apply('receipt')
        self.assertEqual(code, 0, out)
        self.assertEqual((self.git('show', 'main:docs/mirror/WORK.md'), self.git('show', 'main:README.md')),
                         ('Receipt', 'New overview'))

    def test_a_phase_only_merge_on_main_does_not_stale_a_focused_run(self):
        self.validate()
        self.on_main({'src/Quire.Engine/A.cs': 'class A { }\n',
                      merge_gate.PHASE_BASELINE: json.dumps(dict(self.PHASE, methods={'E::M()': 'compared'}))})
        main = self.git('rev-parse', 'main')
        code, out = self.apply()
        self.assertEqual(code, 0, out)
        # The phase baseline is main's: a focused run cannot change it.
        self.assertEqual(self.git('rev-parse', f'main:{merge_gate.PHASE_BASELINE}'),
                         self.git('rev-parse', f'{main}:{merge_gate.PHASE_BASELINE}'))

    def test_replay_keeps_the_branchs_modes_and_deletions(self):
        self.git('checkout', '-qb', 'modes', 'main')
        (self.repo / 'src/Quire.Mpl/run.sh').write_text('#!/bin/sh\n')
        (self.repo / 'src/Quire.Mpl/run.sh').chmod(0o755)
        self.git('rm', '-q', 'src/Quire.Mpl/Other.cs')
        self.git('add', '-A'); self.git('update-index', '--chmod=+x', 'src/Quire.Mpl/run.sh')
        self.git('commit', '-qm', 'modes')
        self.git('checkout', '-q', 'main')
        self.validate('modes')
        self.on_main({'README.md': 'New overview\n'})
        code, out = self.apply('modes')
        self.assertEqual(code, 0, out)
        self.assertTrue(self.git('ls-tree', 'main', '--', 'src/Quire.Mpl/run.sh').startswith('100755 '))
        self.assertEqual(self.git('ls-tree', 'main', '--', 'src/Quire.Mpl/Other.cs'), '')
        self.assertEqual(self.git('show', 'main:README.md'), 'New overview')

    def test_main_changing_an_input_of_the_focused_run_is_stale(self):
        for path in ['src/Quire.Mpl/Other.cs', 'tests/Quire.Differential.Tests/T.cs', 'global.json',
                     'Directory.Build.props', 'docs/mirror/type-map.csv', 'docs/mirror/manifest/Quire.Mpl.txt']:
            with self.subTest(path):
                self.assert_stale(f'{path}: main changed an input of the focused run',
                                  setup=lambda: self.on_main({path: 'changed\n'}))

    def test_overlapping_changes_are_stale(self):
        self.assert_stale('src/Quire.Mpl/M.cs: changed on main and by the branch',
                          setup=lambda: self.on_main({'src/Quire.Mpl/M.cs': 'class M { }\n'}))

    def test_a_rename_on_main_touching_the_branchs_paths_is_stale(self):
        def rename():
            self.git('mv', 'src/Quire.Mpl/M.cs', 'src/Quire.Mpl/N.cs'); self.git('commit', '-qm', 'rename')
        self.assert_stale('src/Quire.Mpl/M.cs: changed on main and by the branch', setup=rename)

    def test_a_different_baseline_on_main_is_stale(self):
        self.assert_stale(f'{merge_gate.BASELINE}: main has a different one',
                          setup=lambda: self.on_main({merge_gate.BASELINE: json.dumps(self.BASELINE, indent=1)}))

    def test_a_protected_change_on_main_is_stale(self):
        self.assert_stale('tools/validation/protected.txt: protected path changed on main', setup=lambda: self.on_main(
            {'tools/validation/protected.txt': 'tools/*\ndocs/mirror/validation-baseline.json\ndocs/x.md\n'}))

    def test_a_moved_branch_is_stale(self):
        self.assert_stale('mpl moved from the validated', setup=lambda: self.branch_commit('mpl'))

    def branch_commit(self, branch):
        self.git('checkout', '-q', branch)
        (self.repo / 'src/Quire.Mpl/M.cs').write_text('class M { int F() => 2; }\n')
        self.git('commit', '-qam', 'more'); self.git('checkout', '-q', 'main')

    def test_a_branch_with_merge_commits_lands_only_on_its_base(self):
        self.git('checkout', '-qb', 'merged', 'main')
        self.git('checkout', '-qb', 'side', 'main')
        (self.repo / 'src/Quire.Mpl/S.cs').write_text('class S {}\n')
        self.git('add', '-A'); self.git('commit', '-qm', 'side')
        self.git('checkout', '-q', 'merged')
        self.git('merge', '-q', '--no-ff', '-m', 'merge side', 'side')
        self.git('checkout', '-q', 'main')
        self.assert_stale('the branch has merge commits', 'merged',
                          setup=lambda: self.on_main({'docs/mirror/WORK.md': 'Receipt\n'}))

    def test_maintainer_overrides_land_only_on_their_base(self):
        code, _, out, _ = self.validate('engine', ['--allow-unvalidated'])
        self.assertEqual(code, 0, out)
        self.on_main({'docs/mirror/WORK.md': 'Receipt\n'})
        code, out = self.apply('engine')
        self.assertEqual(code, merge_gate.STALE, out)
        self.assertIn('the run used a maintainer override', out)

    def test_a_phase_run_is_stale_for_any_code_change_on_main(self):
        self.assert_stale('src/Quire.Mpl/Other.cs: main changed an input of the phase run', 'engine',
                          setup=lambda: self.on_main({'src/Quire.Mpl/Other.cs': 'class Other { }\n'}))
        self.assert_stale(f'{merge_gate.PHASE_BASELINE}: main has a different one', 'engine', setup=lambda: self.on_main(
            {merge_gate.PHASE_BASELINE: json.dumps(self.PHASE, indent=1)}))

    def test_a_phase_run_lands_with_its_phase_baseline_over_prose_moves(self):
        code, _, out, commands = self.validate('engine')
        self.assertEqual((code, len(commands)), (0, 2), out)
        self.assertIn('phase', self.record('engine')[0]['durations'])
        self.on_main({'docs/mirror/WORK.md': 'Receipt\n'})
        code, out = self.apply('engine')
        self.assertEqual(code, 0, out)
        self.assertEqual(json.loads(self.git('show', f'main:{merge_gate.PHASE_BASELINE}')),
                         dict(self.PHASE, methods={'E::M()': 'compared'}))

    def test_stale_reports_how_long_the_one_shot_fallback_holds_the_lock(self):
        self.validate()
        record, files = self.record()
        merge_gate.write_record('mpl', dict(record, durations={'focused': 240, 'phase': 3000}), files, self.repo)
        self.on_main({'src/Quire.Mpl/Other.cs': 'class Other { }\n'})
        code, out = self.apply()
        self.assertEqual(code, merge_gate.STALE)
        self.assertIn('holds the lock for the whole validation, about 54 min going by this run (focused 4 min, '
                      'phase 50 min)', out)

    def test_main_not_containing_the_base_is_stale(self):
        self.validate()
        record, _ = self.record()
        other = self.git('commit-tree', f"{self.base}^{{tree}}", '-m', 'unrelated')
        self.assertEqual(merge_gate.stale_reasons(record, other, self.repo),
                         [f'{other[:12]} does not contain the validated base {record["base"][:12]}'])

    def test_records_that_are_not_passed_runs_of_this_gate_are_refused(self):
        self.validate()
        record, files = self.record()
        for name, forged, extra in [('failed', dict(record, reasons=['x']), files),
                                    ('other tools', dict(record, tools={'tools/validation/merge_gate.py': '0' * 40}), files),
                                    ('other branch', dict(record, branch='engine'), files),
                                    ('no baseline', record, {})]:
            with self.subTest(name):
                merge_gate.write_record('mpl', forged, extra, self.repo)
                code, out = self.apply()
                self.assertEqual(code, 1, out)
                self.assertEqual(self.git('rev-parse', 'main'), record['base'])

    def test_a_baseline_regression_against_main_is_stale(self):
        self.validate()
        record, files = self.record()
        merge_gate.write_record('mpl', record, {'baseline.json': json.dumps(dict(self.BASELINE, types={})).encode()},
                                self.repo)
        code, out = self.apply()
        self.assertEqual(code, merge_gate.STALE, out)
        self.assertIn('T: type disappeared', out)

    def test_modes_do_not_combine(self):
        for extra in (['--apply', '--validate-only'], ['--apply', '--apply-validated'],
                      ['--validate-only', '--apply-validated']):
            with self.subTest(extra), self.assertRaises(SystemExit), contextlib.redirect_stderr(io.StringIO()):
                self.main('mpl', extra=extra)

    def test_is_input(self):
        for path, focused, phase in [('docs/mirror/WORK.md', False, False), ('README.md', False, False),
                                     ('src/Quire.Engine/A.cs', False, True), ('tests/Quire.Engine.Tests/T.cs', False, True),
                                     ('src/Quire.Mpl.Emit/E.cs', True, True), ('src/Quire.Mirror/A.cs', True, True), ('src/Quire.Mpl/M.cs', True, True),
                                     ('tests/Quire.Harness.Tests/H.cs', True, True), ('tools/x.py', True, True),
                                     ('docs/mirror/accepted-mutants.txt', True, True), ('.editorconfig', True, True),
                                     ('docs/mirror/work/notes.md', True, True)]:
            with self.subTest(path):
                self.assertEqual((merge_gate.is_input(path, False), merge_gate.is_input(path, True)), (focused, phase))

    def test_focused_inputs_cover_the_focused_projects_csproj_closure(self):
        # ProjectReference, Compile and Import items, followed through every project and imported .props/.targets. As
        # MSBuild resolves them, an Import is relative to the file that has it, an item to the project importing it.
        root = Path(__file__).resolve().parents[2]
        pending = [(root / 'tests' / p / f'{p}.csproj',) * 2 for p in (
            'Quire.Mpl.Tests', 'Quire.Focused.Differential.Tests', 'Quire.Harness.Tests')]
        seen = set()
        while pending:
            file, project = pending.pop()
            if file in seen:
                continue
            seen.add(file)
            for kind, include in re.findall(r'<(ProjectReference|Compile|Import)\s+(?:Include|Project)="([^"]+)"',
                                            file.read_text()):
                self.assertNotIn('$(', include, f'{file}: unresolved MSBuild property; extend this test')
                path = ((file if kind == 'Import' else project).parent / include.replace('\\', '/')).resolve()
                relative = path.relative_to(root).as_posix() if path.is_relative_to(root) else None
                self.assertIsNotNone(relative, include)
                self.assertTrue(not relative.lower().startswith(('src/', 'tests/'))
                                or relative.lower().startswith(merge_gate.FOCUSED_INPUTS), relative)
                if kind != 'Compile':
                    self.assertTrue(path.is_file(), f'{file}: {include} resolves to no file')
                    pending.append((path, path if kind == 'ProjectReference' else project))
        for project in ('src/Quire.Mpl/Quire.Mpl.csproj', 'src/Quire.Mpl/Generated/Mirror.props',
                        'src/Quire.Mpl.Emit/Quire.Mpl.Emit.csproj', 'src/Quire.Mirror/Quire.Mirror.csproj'):
            self.assertIn(root / project, seen)

    def test_main_changing_a_project_quire_mpl_references_is_stale(self):
        for path in ('src/Quire.Mpl.Emit/Stand.cs', 'src/Quire.Mirror/M.cs'):
            with self.subTest(path):
                self.assert_stale(f'{path}: main changed an input of the focused run',
                                  setup=lambda: self.on_main({path: 'namespace Q { public class S {} }\n'}))

    def test_a_lint_change_on_main_outside_the_focused_projects_is_stale(self):
        # Another project's source is no focused input, but the lint, whose findings the baseline records, reads it.
        self.assert_stale("the source lint on the landed tree differs from the validated baseline's", setup=lambda:
                          self.on_main({'src/Quire.Engine/Dec.cs': 'class D { void F() { goto IL_0001; IL_0001: ; } }\n'}))
        self.assertNotEqual(merge_gate.landed_lint('main', self.repo), [])

    def test_a_prose_record_lands_over_a_lint_change_on_main(self):
        # A maintainer commit that changes the lint (main's committed baseline does not record it) must not leave
        # prose records stale forever: a prose run lints nothing, in the one-shot gate too.
        dec = {'src/Quire.Engine/Dec.cs': 'class D { void F() { goto IL_0001; IL_0001: ; } }\n'}
        self.on_main(dec)
        self.branch('receipt', {'docs/mirror/WORK.md': 'Receipt\n'})
        self.assertEqual(self.validate('receipt')[0], 0)
        code, out = self.apply('receipt')  # V == M
        self.assertEqual(code, 0, out)
        self.branch('receipt2', {'README.md': 'Receipt\n'})
        self.assertEqual(self.validate('receipt2')[0], 0)
        self.on_main({'src/Quire.Engine/Dec2.cs': dec['src/Quire.Engine/Dec.cs']})
        code, out = self.apply('receipt2')  # V != M, main's move changes the lint again
        self.assertEqual(code, 0, out)
        self.assertNotEqual(merge_gate.landed_lint('main', self.repo), [])

    def test_the_branchs_changes_come_from_git_not_the_record(self):
        self.validate()
        record, files = self.record()
        merge_gate.write_record('mpl', dict(record, changed=[]), files, self.repo)
        self.on_main({'src/Quire.Mpl/M.cs': 'class M { }\n'})
        code, out = self.apply()
        self.assertEqual(code, merge_gate.STALE, out)
        self.assertIn('src/Quire.Mpl/M.cs: changed on main and by the branch', out)

    def test_a_record_cannot_land_unvalidated_or_protected_changes(self):
        self.validate()
        record, files = self.record()
        for name, paths, reason in [
                ('engine-only', {'src/Quire.Engine/B.cs': 'class B {}\n'}, 'record has no phase run'),
                ('tools-edit', {'tools/x.py': 'x\n'}, 'not of an --allow-protected run')]:
            with self.subTest(name):
                self.branch(name, paths)
                tip = self.git('rev-parse', name)
                merge_gate.write_record(name, dict(record, branch=name, tip=tip, changed=[]), files, self.repo)
                code, out = self.apply(name)
                self.assertEqual(code, 1, out)
                self.assertIn(reason, out)

    def test_a_runtime_record_relabelled_as_prose_is_blocked(self):
        # Prose records carry no baseline; whether a branch is prose comes from git, not from the record.
        self.validate()
        record, _ = self.record()
        merge_gate.write_record('mpl', dict(record, validation_mode='prose'), {}, self.repo)
        code, out = self.apply()
        self.assertEqual(code, 1, out)
        self.assertIn('changes more than allowlisted prose', out)
        self.assertEqual(self.git('rev-parse', 'main'), record['base'])

    def test_a_corrupt_record_is_blocked(self):
        self.validate()
        _, files = self.record()
        for name, data in [('not json', b'{'), ('not an object', b'[]')]:
            with self.subTest(name):
                tree = subprocess.run(['git', 'mktree'], cwd=self.repo, capture_output=True, check=True, input=(
                    '100644 blob ' + subprocess.run(['git', 'hash-object', '-w', '--stdin'], cwd=self.repo, input=data,
                                                    capture_output=True, check=True).stdout.decode().strip()
                    + '\tgate.json\n').encode()).stdout.decode().strip()
                commit = self.git('commit-tree', tree, '-m', 'corrupt')
                self.git('update-ref', 'refs/quire-gate/mpl', commit)
                code, out = self.apply()
                self.assertEqual(code, 1, out)
                self.assertIn('unreadable', out)

    def test_a_file_directory_conflict_in_the_replay_is_blocked(self):
        self.branch('file', {'docs/notes': 'a file\n'})
        self.on_main({'docs/notes/a.md': 'in a directory\n'})
        base = self.git('rev-parse', 'main^')
        with self.assertRaises(merge_gate.ReplayConflict) as raised:
            merge_gate.replay(base, self.git('rev-parse', 'file'), self.git('rev-parse', 'main'), self.repo)
        self.assertIn('is a file on one side and a directory on the other', str(raised.exception))
        self.assertIn('rebase the branch onto main, resolve it, and run --validate-only again', str(raised.exception))
        with mock.patch.object(merge_gate, 'stale_reasons', return_value=[]), \
                mock.patch.object(merge_gate, 'read_record', return_value=(
                    {'branch': 'file', 'base': base, 'tip': self.git('rev-parse', 'file'), 'validation_mode': 'runtime',
                     'results': 'r', 'reasons': [], 'tools': merge_gate.tool_blobs(base, self.repo)},
                    {'baseline.json': self.git('show', f'main:{merge_gate.BASELINE}').encode()})):
            code, out = self.apply('file')
        self.assertEqual(code, 1, out)
        self.assertIn('BLOCKED: cannot replay', out)

    def test_case_collisions_on_the_landed_tree_are_blocked(self):
        self.validate()
        self.on_main({'docs/mirror/work/lane-a.md': 'a\n'})
        real, root = merge_gate.git, merge_gate.ROOT
        def git(*args, cwd=None, check=True):
            result = real(*args, cwd=self.repo if cwd in (None, root) else cwd, check=check)
            if args[:3] == ('ls-tree', '-r', '--name-only'):
                return result + '\nDocs/mirror/work/LANE-A.md'
            return result
        with mock.patch.object(merge_gate, 'git', git), mock.patch.object(merge_gate, 'stale_reasons', return_value=[]):
            code, out = self.apply()
        self.assertEqual(code, 1, out)
        self.assertIn('paths differ only in case', out)

    def test_a_branch_named_like_a_record_resolves_to_the_branch(self):
        self.validate()  # refs/quire-gate/mpl
        self.branch('quire-gate/mpl', {'src/Quire.Mpl/Q.cs': 'class Q {}\n'})
        real, root = merge_gate.git, merge_gate.ROOT
        with mock.patch.object(merge_gate, 'git', lambda *a, cwd=None, check=True: real(
                *a, cwd=self.repo if cwd in (None, root) else cwd, check=check)):
            self.assertEqual(merge_gate.branch_tip('quire-gate/mpl'), self.git('rev-parse', 'refs/heads/quire-gate/mpl'))
            self.assertEqual(merge_gate.branch_tip('mpl'), self.git('rev-parse', 'refs/heads/mpl'))
        code, gate, out, _ = self.validate('quire-gate/mpl')
        self.assertEqual(code, 0, out)
        self.assertEqual(gate['tip'], self.git('rev-parse', 'refs/heads/quire-gate/mpl'))


class NightlyTests(unittest.TestCase):
    """--nightly mutates every compared method of main's tip in a clone and merges nothing."""
    git, main = RepositoryTests.git, RepositoryTests.main
    BASELINE = RepositoryTests.BASELINE
    setUp = RepositoryTests.setUp

    def nightly(self, summary):
        real, commands = subprocess.run, []
        def run(command, *args, **kwargs):
            if '--mutate-all' not in command:
                return real(command, *args, **kwargs)
            commands.append(command)
            results = Path(command[command.index('--results') + 1])
            results.mkdir(parents=True)
            (results / 'summary.json').write_text(json.dumps(summary))
            return subprocess.CompletedProcess(command, 0 if not summary.get('mutation', {}).get('survivors') else 1)
        results, git, root = self.repo.parent / 'nightly', merge_gate.git, merge_gate.ROOT
        with mock.patch.object(merge_gate.subprocess, 'run', side_effect=run), \
                mock.patch.object(merge_gate, 'ROOT', self.repo), \
                mock.patch.object(merge_gate, 'git', lambda *a, cwd=None, check=True: git(
                    *a, cwd=self.repo if cwd in (None, root) else cwd, check=check)), \
                mock.patch('sys.argv', ['merge_gate.py', '--nightly', '--results', str(results)]), \
                contextlib.redirect_stdout(io.StringIO()):
            code = merge_gate.main()
        return code, commands, json.loads((results / 'nightly.json').read_text())

    def test_nightly_mutates_everything_and_merges_nothing(self):
        main = self.git('rev-parse', 'main')
        code, commands, report = self.nightly(dict(PASSED, mutation={'total': 9, 'killed': 9, 'survivors': []}))
        self.assertEqual(code, 0, report)
        self.assertEqual(len(commands), 1)
        self.assertIn('--no-mutant-cache', commands[0])
        self.assertNotIn('--write-baseline', commands[0])
        self.assertEqual((report['tip'], report['mutants'], report['reasons']), (main, 9, []))
        self.assertEqual(self.git('rev-parse', 'main'), main)
        self.assertEqual(self.git('for-each-ref', 'refs/quire-gate'), '')

    def test_nightly_reports_survivors(self):
        code, _, report = self.nightly(dict(PASSED, gate_passed=False, mutation={
            'total': 9, 'killed': 8, 'survivors': ['T::A() m3']}))
        self.assertEqual(code, 1)
        self.assertEqual(report['survivors'], ['T::A() m3'])
        self.assertIn('surviving mutant: T::A() m3', report['reasons'])


class LaneMergeTests(unittest.TestCase):
    """lane_merge.py with real git, the real merge lock (on a private file) and a stub gate: it defaults to the
    worktree that has `main` checked out, pulls and lands only under the lock and validates outside it, pushes only
    after a merge, passes the gate's exit codes back out, and falls back to the one-shot gate after two stale results."""
    GATE = textwrap.dedent('''
        import os, sys
        sys.path.insert(0, os.environ['TOOLS'])
        from merge_lock import is_held
        mode = next(a for a in sys.argv if a in ('--validate-only', '--apply-validated', '--apply'))
        jobs = sys.argv[sys.argv.index('--jobs') + 1] if '--jobs' in sys.argv else None
        if os.path.realpath(os.getcwd()) != os.path.realpath(os.path.join(os.environ['T'], 'main')) \\
                or jobs != (None if mode == '--apply-validated' else '14') or is_held() != (mode != '--validate-only'):
            sys.exit(99)  # in the worktree that has main, 14 validating workers, landing under the lock only
        path = os.path.join(os.environ['T'], 'codes' + mode)
        codes = open(path).read().split() if os.path.exists(path) else ['0']
        open(path, 'w').write(' '.join(codes[1:] or codes))
        open(os.path.join(os.environ['T'], 'log'), 'a').write(f'{mode} {codes[0]}\\n')
        sys.exit(int(codes[0]))
    ''')

    def run_script(self, branch='b', main_checked_out=True, **codes):
        import lane_merge, merge_lock
        with tempfile.TemporaryDirectory() as t:
            t = Path(t).resolve()
            main, lane, log = t / 'main', t / 'lane', t / 'log'
            # As on the host, main is checked out in a linked worktree (not the clone's own checkout), main has moved
            # past b's base, and b is checked out in a lane worktree: the script rebases b there.
            clone = t / 'clone'
            clone.mkdir()
            commit = ['-c', 'user.name=t', '-c', 'user.email=t@t', 'commit', '-q', '--allow-empty', '-m']
            for command in (['init', '-q', '-b', 'main'], [*commit, 'base'], ['branch', 'b'], [*commit, 'moved'],
                            ['checkout', '-q', '-b', 'other'], ['worktree', 'add', '-q', str(main), 'main'],
                            ['worktree', 'add', '-q', str(lane), 'b'],
                            *([] if main_checked_out else [['-C', str(main), 'checkout', '-q', '--detach']])):
                subprocess.run(['git', *command], cwd=clone, check=True, capture_output=True)
            (main / 'tools/validation').mkdir(parents=True)
            (main / 'tools/validation/merge_gate.py').write_text(self.GATE)
            for mode, sequence in codes.items():
                (t / f"codes--{mode.replace('_', '-')}").write_text(sequence)
            def logged(word):
                def step(where):
                    self.assertEqual(Path(where).resolve(), main)
                    self.assertTrue(merge_lock.is_held(), f'{word} outside the lock')
                    with log.open('a') as f:
                        f.write(word + '\n')
                    return 0
                return step
            stderr, cwd = io.StringIO(), os.getcwd()
            os.chdir(lane)
            try:
                with mock.patch.dict(os.environ, T=str(t), TOOLS=str(Path(__file__).parent),
                                     QUIRE_MERGE_LOCK=str(t / 'lock')), \
                        mock.patch.object(lane_merge, 'pull', logged('PULL')), \
                        mock.patch.object(lane_merge, 'push', logged('PUSHED')), \
                        contextlib.redirect_stderr(stderr):
                    code = lane_merge.main([branch])
            finally:
                os.chdir(cwd)
            if branch != 'b' or not main_checked_out:
                return code, stderr.getvalue()
            git = lambda *a: subprocess.run(['git', *a], cwd=lane, capture_output=True, text=True).stdout.strip()
            self.assertEqual(git('merge-base', 'main', 'b'), git('rev-parse', 'main'))
            return code, log.read_text().split('\n')[:-1]

    def test_the_lane_script(self):
        # Each validation starts from a fresh main (PULL), and each landing pulls first under the lock.
        v, a, one, pull = '--validate-only 0', '--apply-validated', '--apply', 'PULL'
        for codes, expected in [
                ({'apply_validated': '0'}, (0, [pull, v, pull, f'{a} 0', 'PUSHED'])),
                ({'apply_validated': '1'}, (1, [pull, v, pull, f'{a} 1'])),
                ({'validate_only': '1'}, (1, [pull, '--validate-only 1'])),
                ({'apply_validated': '4 0'}, (0, [pull, v, pull, f'{a} 4', pull, v, pull, f'{a} 0', 'PUSHED'])),
                ({'apply_validated': '4 4', 'apply': '0'},
                 (0, [pull, v, pull, f'{a} 4', pull, v, pull, f'{a} 4', pull, f'{one} 0', 'PUSHED'])),
                ({'apply_validated': '4 4', 'apply': '1'},
                 (1, [pull, v, pull, f'{a} 4', pull, v, pull, f'{a} 4', pull, f'{one} 1']))]:
            with self.subTest(codes):
                self.assertEqual(self.run_script(**codes), expected)
        # A failed rebase reports git's own error and stops before any validation.
        code, stderr = self.run_script('no-such-branch')
        self.assertEqual(code, 1)
        self.assertIn('git rebase main no-such-branch failed: ', stderr)
        self.assertIn('no-such-branch', stderr.split('failed: ', 1)[1])
        # Without a worktree that has main checked out, and no --main, nothing runs.
        self.assertEqual(self.run_script(main_checked_out=False),
                         (1, 'no worktree has main checked out; pass --main\n'))


class StubDeletionTests(unittest.TestCase):
    """Agents may delete whole `[MirrorStub]` members from any project's generated mirror files, and nothing else
    there; files are compared byte for byte (most real ones are CRLF)."""
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

    def test_only_existing_generated_mirror_files_qualify_across_projects(self):
        self.assertIsNone(self.deleted(self.without(9, 10), 'src/Quire.Mpl/Mcd/T.cs'))
        for path in ('src/Quire.Engine/Generated/Mirror/T.g.cs',
                     'src/Quire.Calculation.Core/Generated/Mirror/T.g.cs'):
            with self.subTest(path=path):
                self.base = self.commit(self.LINES, path)
                self.assertEqual(self.deleted(self.without(9, 10), path),
                                 ['public static object A() => throw global::Quire.Mirror.MirrorStub.NotImplemented();'])
        self.git('rm', '-q', self.PATH); self.git('commit', '-qm', 'gone')
        self.assertIsNone(stub_deletions(self.base, self.git('rev-parse', 'HEAD'), self.PATH, self.repo))


if __name__ == '__main__':
    unittest.main()
