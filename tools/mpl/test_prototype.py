"""Release IL acceptance test for the tiny Mpl source/compiler proof.

Run from the repository root with `python tools/mpl/test_prototype.py`.
"""

from __future__ import annotations

import os
from pathlib import Path
import subprocess
import unittest


ROOT = Path(__file__).resolve().parents[2]
INSTALL = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"))
CACHE = ROOT / ".mirror-cache" / "mpl-compiler-prototype"
ASSEMBLIES = CACHE / "assemblies"
COMPILER_OUT = CACHE / "compiler"
METHOD_AUDIT_OUT = ROOT / ".mirror-cache" / ".method-audit"


def run(*command: str) -> str:
    result = subprocess.run(command, cwd=ROOT, text=True, encoding="utf-8", errors="replace",
                            capture_output=True)
    if result.returncode:
        raise AssertionError(f"command failed ({result.returncode}): {' '.join(command)}\n"
                             f"{result.stdout}\n{result.stderr}")
    return result.stdout


class MplPrototypeTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if not INSTALL.is_dir():
            raise AssertionError(f"Mathcad install not found: {INSTALL}; set MATHCAD_DIR")
        run("dotnet", "build", "src/Quire.Mpl.Compiler/Quire.Mpl.Compiler.csproj", "-c", "Release",
            "-o", str(COMPILER_OUT), "-v", "quiet")
        ASSEMBLIES.mkdir(parents=True, exist_ok=True)
        compiler = COMPILER_OUT / "Quire.Mpl.Compiler.dll"
        for source, assembly in (("Hashtable.mpl", "Quire.Mpl.Modules.dll"),
                                 ("McdUnitSystemUS.mpl", "Quire.McdUnitSystemUS.dll")):
            run("dotnet", str(compiler), str(ROOT / "src/Quire.Mpl.Compiler/examples" / source),
                str(ASSEMBLIES / assembly), str(INSTALL))
        run("dotnet", "build", "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj",
            "-c", "Release", "-o", str(METHOD_AUDIT_OUT), "-v", "quiet")

    def assert_release_method_exact(self, project: str, method: str):
        stdout = run("dotnet", str(METHOD_AUDIT_OUT / "Quire.Tools.MethodAudit.dll"),
                     "--repo", str(ROOT), "--assemblies", str(ASSEMBLIES), "--install", str(INSTALL),
                     "--project", project, "--diff", method)
        self.assertIn(": exact", stdout, stdout)

    def test_hashtable_methods_match_installed_release_il(self):
        self.assert_release_method_exact("Quire.Mpl.Modules", "Quire.Hashtable.values::Update")
        self.assert_release_method_exact("Quire.Mpl.Modules", "Quire.Hashtable.values::mlookupInv")

    def test_unit_operation_matches_installed_release_il(self):
        self.assert_release_method_exact("Quire.McdUnitSystemUS", "Quire.McdUnitSystemUS.values::op_L")


if __name__ == "__main__":
    unittest.main()
