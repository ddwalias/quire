"""MirrorGen reads a field-like event's backing field from the accessors' IL, so the generated source can leave out the
obfuscated Mathcad field the event declaration recreates (built assemblies; no install)."""
from pathlib import Path
import subprocess
import tempfile
import unittest

from method_audit_fixture import ROOT, Case, ROOT_FILES, JUDGE_SOURCES, dotnet
from test_method_audit_event_fields import obfuscated
import shutil

AUTO = 'namespace N { public class T { public event System.Action ValueChanged; public event System.Action<int> Other; } }'
EXPLICIT = '''namespace N { public class T { private System.Action b;
    public event System.Action ValueChanged { add { b += value; } remove { b -= value; } } } }'''


class EventBackingTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.temp = tempfile.TemporaryDirectory(prefix='quire-event-backing-')
        cls.addClassCleanup(cls.temp.cleanup)
        work = Path(cls.temp.name)
        sources = work / 'judge'
        for tree in JUDGE_SOURCES:
            shutil.copytree(ROOT / tree, sources / tree, ignore=shutil.ignore_patterns('bin', 'obj'))
        for name in ROOT_FILES:
            if (ROOT / name).is_file():
                shutil.copyfile(ROOT / name, sources / name)
        dotnet('build', sources / 'tools/Quire.Tools.MirrorGen/Quire.Tools.MirrorGen.csproj', '-c', 'Release', '-o', work / 'mirrorgen',
               '-nologo', '-v:q', cwd=sources)
        cls.tool = work / 'mirrorgen' / 'Quire.Tools.MirrorGen.dll'
        cls.case = Case(work / 'case', work)

    def fields(self, name, source):
        self.case.work.mkdir(parents=True, exist_ok=True)
        output = self.case.work / name
        self.case.compile(name, source, output)
        result = subprocess.run(['dotnet', str(self.tool), 'event-fields', str(output / f'{name}.dll')],
                                capture_output=True, text=True, check=True)
        return result.stdout.split()

    def test_an_obfuscated_backing_field_is_found(self):
        self.assertEqual(self.fields('Obfuscated', obfuscated('b', 'b')), ['N.T::ValueChanged', '=', 'b'])

    def test_a_field_like_event_names_its_own_field(self):
        self.assertEqual(self.fields('Auto', AUTO),
                         ['N.T::ValueChanged', '=', 'ValueChanged', 'N.T::Other', '=', 'Other'])

    def test_accessors_on_different_fields_have_no_backing_field(self):
        self.assertEqual(self.fields('Split', obfuscated('b', 'c')), [])

    def test_explicit_accessors_without_a_compare_exchange_loop_have_none(self):
        self.assertEqual(self.fields('Explicit', EXPLICIT), [])


if __name__ == '__main__':
    unittest.main()
