"""A COM interface the compiler embeds from an interop assembly (NoPIA, D22) binds to the original's embedded copy."""
import tempfile
import unittest

from method_audit_fixture import Case, MIRROR_OF, PROJECT, dotnet, build_audit

INTEROP = '''
using System.Runtime.InteropServices;
[assembly: Guid("11111111-2222-3333-4444-555555555555")]
[assembly: ImportedFromTypeLib("Test")]
namespace Mathcad
{
    [ComImport, Guid("66666666-7777-8888-9999-000000000000"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IValue { int Get(); }
}
'''
READER = 'namespace N { public class T { public static int Read(Mathcad.IValue x) => x.Get(); } }'


class EmbeddedInteropTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-embedded-interop-')
        cls.addClassCleanup(cls.temp.cleanup)
        cls.case = Case(cls.temp.name, build_audit(cls.temp.name))
        # Each side embeds its own interop assembly into the assembly that uses it, as Mathcad's and Quire's builds do.
        cls.case.original('Orig.Interop', INTEROP)
        cls.case.compile('Quire.Interop', INTEROP, cls.case.assemblies)
        cls.build('Orig.Consumer', cls.case.install / 'Orig.Interop.dll', READER, cls.case.install)
        cls.build('Quire.P', cls.case.assemblies / 'Quire.Interop.dll', MIRROR_OF +
                  'namespace Quire.N { [Quire.Mirror.MirrorOf("N.T, Orig.Consumer")] '
                  'public class T { public static int Read(Mathcad.IValue x) => x.Get(); } }', cls.case.assemblies)
        cls.case.projects = ['Quire.P']
        cls.rows = [('Orig.Consumer.dll', 'N.T', 'Quire.N.T', 'missing')]

    @classmethod
    def build(cls, name, reference, source, output):
        project = cls.case.work / 'build' / name
        project.mkdir(parents=True)
        xml = PROJECT.format(name=name).replace('</Project>',
            f'<ItemGroup><Reference Include="{reference.stem}"><HintPath>{reference}</HintPath>'
            '<EmbedInteropTypes>true</EmbedInteropTypes></Reference></ItemGroup></Project>')
        (project / f'{name}.csproj').write_text(xml, encoding='utf-8')
        (project / 'Source.cs').write_text(source, encoding='utf-8')
        dotnet('build', project / f'{name}.csproj', '-c', 'Release', '-o', output, '-nologo', '-v:q', cwd=project)

    def test_embedded_copy_pairs_with_the_originals_embedded_copy(self):
        inventory = self.case.audit(self.rows)
        self.assertEqual(inventory['errors'], [])
        read = next(row for row in inventory['methods'] if row['signature'].startswith('Read`'))
        self.assertTrue(read['exact_match'], read)
        get = next(row for row in inventory['methods'] if row['type'] == 'Mathcad.IValue')
        self.assertEqual(get['original']['assembly'], 'Orig.Consumer')
        self.assertTrue(get['shape_matches'], get)
        self.assertTrue(get['declaration_only'], get)
        self.assertEqual(inventory['missing'], [])


if __name__ == '__main__':
    unittest.main()
