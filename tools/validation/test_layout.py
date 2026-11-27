"""Project source layout resolution, including nested-project ownership."""
import json
from pathlib import Path
import tempfile
import unittest

import layout


class ProjectLayoutTests(unittest.TestCase):
    def test_default_and_nested_projects(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            (root / 'docs/mirror').mkdir(parents=True)
            (root / 'src/Flat').mkdir(parents=True)
            (root / 'src/Flat/Flat.csproj').touch()
            (root / 'src/ThirdParty/DevComponents/DotNetBar.4.0.0.1/WpfDock').mkdir(parents=True)
            nested = root / 'src/ThirdParty/DevComponents/DotNetBar.4.0.0.1'
            (nested / 'Quire.DevComponents.WpfRibbon.csproj').touch()
            (nested / 'WpfDock/Quire.DevComponents.WpfDock.csproj').touch()
            (nested / 'bin/Release/Generated.csproj').parent.mkdir(parents=True)
            (nested / 'bin/Release/Generated.csproj').touch()
            mixed = root / 'src/Native/MixedMode'
            mixed.mkdir(parents=True)
            (mixed / 'Native.vcxproj').touch()
            (root / 'docs/mirror/source-layout.json').write_text(json.dumps({
                'Quire.DevComponents.WpfRibbon': 'ThirdParty/DevComponents/DotNetBar.4.0.0.1',
                'Quire.DevComponents.WpfDock': 'ThirdParty/DevComponents/DotNetBar.4.0.0.1/WpfDock',
                'Native': 'Native',
            }))

            self.assertEqual(layout.project_dir(root, 'Flat'), (root / 'src/Flat').resolve())
            self.assertEqual(layout.project_of('src/Flat/Source.cs', root), 'Flat')
            self.assertEqual(layout.project_dir(root, 'Quire.DevComponents.WpfDock'), (nested / 'WpfDock').resolve())
            self.assertEqual(layout.projects(root, '.csproj'), {
                'Flat', 'Quire.DevComponents.WpfRibbon', 'Quire.DevComponents.WpfDock'})
            self.assertEqual(layout.project_file(root, 'Native', '.vcxproj'), mixed / 'Native.vcxproj')
            self.assertEqual(layout.project_of('src/Gone/Gone.csproj', root), 'Gone')
            self.assertEqual(layout.project_of('src/ThirdParty/DevComponents/DotNetBar.4.0.0.1/WpfDock/a.cs', root),
                             'Quire.DevComponents.WpfDock')
            self.assertEqual(layout.project_of('src/ThirdParty/DevComponents/DotNetBar.4.0.0.1/a.cs', root),
                             'Quire.DevComponents.WpfRibbon')

    def test_rejects_override_outside_src(self):
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            (root / 'docs/mirror').mkdir(parents=True)
            (root / 'docs/mirror/source-layout.json').write_text('{"P":"../../outside"}')
            with self.assertRaises(ValueError):
                layout.project_dir(root, 'P')


if __name__ == '__main__':
    unittest.main()
