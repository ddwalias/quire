"""Referenced generated interop types keep the same identity regardless of audit scope."""
import json
import tempfile
import unittest

from method_audit_fixture import Case, MIRROR_OF, PROJECT, build_audit, dotnet


class GeneratedReferenceTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-generated-reference-')
        cls.addClassCleanup(cls.temp.cleanup)
        cls.case = Case(cls.temp.name, build_audit(cls.temp.name))
        cls.case.original('Orig.Interop', 'namespace Mathcad { public interface IValue { int Get(); } }')
        cls.case.compile('Quire.Interop', 'namespace Mathcad { public interface IValue { int Get(); } }', cls.case.assemblies)
        cls.case.repo.joinpath('docs/mirror/generated-assemblies.json').write_text(
            json.dumps({'Quire.Interop': {'assembly': 'Orig.Interop'}}), encoding='utf-8')

        def consumer(name, reference, source, output):
            project = cls.case.work / 'build' / name
            project.mkdir(parents=True)
            xml = PROJECT.format(name=name).replace('</Project>',
                f'<ItemGroup><Reference Include="{reference.stem}"><HintPath>{reference}</HintPath>'
                '</Reference></ItemGroup></Project>')
            (project / f'{name}.csproj').write_text(xml, encoding='utf-8')
            (project / 'Source.cs').write_text(source, encoding='utf-8')
            dotnet('build', project / f'{name}.csproj', '-c', 'Release', '-o', output,
                   '-nologo', '-v:q', cwd=project)

        consumer('Orig.Consumer', cls.case.install / 'Orig.Interop.dll',
                 'namespace N { public class T { public static int Read(Mathcad.IValue x) => x.Get(); } }',
                 cls.case.install)
        consumer('Quire.P', cls.case.assemblies / 'Quire.Interop.dll', MIRROR_OF +
                 'namespace Quire.N { [Quire.Mirror.MirrorOf("N.T, Orig.Consumer")] '
                 'public class T { public static int Read(Mathcad.IValue x) => x.Get(); } }',
                 cls.case.assemblies)
        cls.case.projects = ['Quire.P']
        cls.rows = [('Orig.Consumer.dll', 'N.T', 'Quire.N.T', 'missing'),
                    ('Orig.Interop.dll', 'Mathcad.IValue', 'Quire.IValue', 'missing')]

    def test_generated_reference_binds_without_auditing_its_project(self):
        inventory = self.case.audit(self.rows)
        self.assertEqual(inventory['errors'], [])
        read = next(row for row in inventory['methods'] if row['signature'].startswith('Read`'))
        self.assertTrue(read['exact_match'], read)

    def test_mapping_is_unchanged_when_generated_project_is_audited(self):
        self.case.projects.append('Quire.Interop')
        try:
            inventory = self.case.audit(self.rows)
            self.assertEqual(inventory['errors'], [])
            read = next(row for row in inventory['methods'] if row['signature'].startswith('Read`'))
            self.assertTrue(read['exact_match'], read)
        finally:
            self.case.projects.remove('Quire.Interop')

    def test_absent_registry_does_not_normalize_the_reference(self):
        registry = self.case.repo / 'docs/mirror/generated-assemblies.json'
        contents = registry.read_text(encoding='utf-8')
        try:
            registry.unlink()
            inventory = self.case.audit(self.rows)
            read = next(row for row in inventory['methods'] if row['signature'].startswith('Read`'))
            self.assertNotEqual(read['exact_match'], True)
        finally:
            registry.write_text(contents, encoding='utf-8')


if __name__ == '__main__':
    unittest.main()
