"""Build MethodAudit and small assemblies for tests of what it reports, outside the repository tree.

A case is a disposable repository layout (type map, src directory names), an install directory holding a stand-in for a
Mathcad original, and the Release Quire assemblies that mirror it. Nothing here reads the Mathcad install.
"""
import json
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[2]
# What the gate's judge is built from (release_il.TOOL_TREES and the root build configuration).
JUDGE_SOURCES = ('tools/Quire.Tools.MethodAudit', 'tools/Quire.Tools.MirrorGen')
ROOT_FILES = ('Directory.Build.props', 'Directory.Build.targets', 'global.json')
PROJECT = '''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>{name}</AssemblyName>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <ProduceReferenceAssembly>false</ProduceReferenceAssembly>
    <Nullable>disable</Nullable>
  </PropertyGroup>
</Project>
'''
# The attribute MethodAudit reads a binding from; the product declares it in Quire.Mirror.
MIRROR_OF = '''namespace Quire.Mirror
{
    [System.AttributeUsage(System.AttributeTargets.All)]
    public sealed class MirrorOfAttribute : System.Attribute { public MirrorOfAttribute(string of) { } }
}
'''
TYPE_MAP_HEADER = 'mathcad_assembly,mathcad_type,kind,visibility,quire_type,status,phase,group\n'


def dotnet(*args, cwd):
    result = subprocess.run(['dotnet', *map(str, args)], cwd=cwd, capture_output=True, text=True)
    if result.returncode:
        raise RuntimeError(f"dotnet {' '.join(map(str, args))} failed:\n{result.stdout}{result.stderr}")
    return result.stdout


def build_audit(work, root=ROOT):
    """MethodAudit built in Release from a copy of root's judge sources under work; returns its DLL."""
    sources = Path(work) / 'judge'
    for tree in JUDGE_SOURCES:
        shutil.copytree(root / tree, sources / tree, ignore=shutil.ignore_patterns('bin', 'obj'))
    for name in ROOT_FILES:
        if (root / name).is_file():
            shutil.copyfile(root / name, sources / name)
    output = Path(work) / 'judge-bin'
    dotnet('build', sources / 'tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj', '-c', 'Release', '-o', output,
           '-nologo', '-v:q', cwd=sources)
    return output / 'Quire.Tools.MethodAudit.dll'


class Case:
    """One audit: originals in an install directory, Quire assemblies, and the type map that binds them."""

    def __init__(self, work, tool):
        self.work, self.tool = Path(work), tool
        self.repo, self.install, self.assemblies = self.work / 'repo', self.work / 'install', self.work / 'assemblies'
        for directory in (self.repo / 'docs/mirror', self.repo / 'src', self.install, self.assemblies):
            directory.mkdir(parents=True)
        self.projects = []

    def compile(self, name, source, output):
        project = self.work / 'build' / name
        project.mkdir(parents=True)
        (project / f'{name}.csproj').write_text(PROJECT.format(name=name), encoding='utf-8')
        (project / 'Source.cs').write_text(source, encoding='utf-8')
        dotnet('build', project / f'{name}.csproj', '-c', 'Release', '-o', output, '-nologo', '-v:q', cwd=project)

    def original(self, name, source):
        """A stand-in for the Mathcad assembly `name`, in the install directory."""
        self.compile(name, source, self.install)

    def quire(self, project, source):
        """The Release assembly of a Quire project; its types bind with [Quire.Mirror.MirrorOf("Type, Assembly")]."""
        (self.repo / 'src' / project).mkdir()
        self.compile(project, MIRROR_OF + source, self.assemblies)
        self.projects.append(project)

    def audit(self, rows):
        """The inventory MethodAudit writes for the case's projects; rows are (assembly file, Mathcad type, Quire type,
        status) of the type map."""
        (self.repo / 'docs/mirror/type-map.csv').write_text(TYPE_MAP_HEADER + ''.join(
            f'{assembly},{mathcad},class,public,{quire},{status},1,test\n' for assembly, mathcad, quire, status in rows),
            encoding='utf-8')
        output = self.work / 'inventory.json'
        result = subprocess.run(['dotnet', str(self.tool), '--repo', str(self.repo), '--assemblies', str(self.assemblies),
                                 '--install', str(self.install), '--output', str(output), '--release-il',
                                 *(arg for project in self.projects for arg in ('--project', project))],
                                capture_output=True, text=True)
        if not output.is_file():
            raise RuntimeError(f'MethodAudit wrote no inventory:\n{result.stdout}{result.stderr}')
        return json.loads(output.read_text(encoding='utf-8'))
