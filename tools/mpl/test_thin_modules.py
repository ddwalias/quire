"""Compile thin Mpl source and verify selected modules against Release IL."""
from __future__ import annotations

import json
import os
from pathlib import Path
import re
import subprocess
import unittest

ROOT = Path(__file__).resolve().parents[2]
INSTALL = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"))
SOURCES = ROOT / "tools/mpl/thin-sources"
OUTPUT = ROOT / "tools/mpl/output"
CACHE = ROOT / ".mirror-cache/mpl-thin-emitter"
COMPILER_OUT = CACHE / "compiler"
AUDIT_OUT = CACHE / "method-audit"
MODULES = ["EfiFitting", "MplUnitSystemSI", "EfiDebug", "EfiPlot", "McdUnitSystemNone", "McdStaticUnitSystemNone", "McdStaticUnitSystemMKS", "McdStaticUnitSystemUS", "McdUnitSystemMKS", "McdUnitSystemUS", "McdUnitSystemCGS", "MplConstants", "MplPrimitives"]
ATTRIBUTE = re.compile(
    r"\.custom instance void \[[^]]+\]([^\s]+)::\.ctor\(string\) = \(\s*((?:[0-9a-fA-F]{2}\s*)+)\)", re.S)


def run(*command: str) -> str:
    result = subprocess.run(command, cwd=ROOT, text=True, encoding="utf-8", errors="replace",
                            capture_output=True)
    if result.returncode:
        raise AssertionError(f"command failed ({result.returncode}): {' '.join(command)}\n"
                             f"{result.stdout}\n{result.stderr}")
    return result.stdout


def class_block(il: str, full_name: str) -> str:
    lines = il.splitlines()
    start = next((i for i, line in enumerate(lines)
                  if line.startswith(".class ") and line.split()[-1].strip("'") == full_name), None)
    if start is None:
        raise AssertionError(f"missing type {full_name}")
    end = next((i for i in range(start + 1, len(lines))
                if lines[i].rstrip() == f"}} // end of class {full_name}"), None)
    if end is None:
        raise AssertionError(f"missing end of class {full_name}")
    return "\n".join(lines[start:end + 1])


def custom_attributes(block: str) -> list[tuple[str, bytes]]:
    return [(name, bytes.fromhex(blob)) for name, blob in ATTRIBUTE.findall(block)]


def member_names(block: str, kind: str) -> list[str]:
    pattern = r"\}\s*// end of method [^:]+::([^\s]+)" if kind == "method" else r"^\s*\.field\s+.*?\s+(\w+)\s*$"
    return re.findall(pattern, block, re.M)


class ThinModuleTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if not INSTALL.is_dir():
            raise AssertionError(f"Mathcad install not found: {INSTALL}; set MATHCAD_DIR")
        CACHE.mkdir(parents=True, exist_ok=True)
        OUTPUT.mkdir(parents=True, exist_ok=True)
        run("dotnet", "build", "tools/mpl/Quire.Mpl.Compiler/Quire.Mpl.Compiler.csproj", "-c", "Release",
            "-o", str(COMPILER_OUT), "-v", "quiet")
        compiler = COMPILER_OUT / "Quire.Mpl.Compiler.dll"
        for module in MODULES:
            project = f"Quire.{module}"
            mode = "--mpl" if module in ("EfiFitting", "MplUnitSystemSI", "EfiDebug", "EfiPlot", "McdUnitSystemNone", "McdStaticUnitSystemNone", "McdStaticUnitSystemMKS", "McdStaticUnitSystemUS", "McdUnitSystemCGS") else "--thin"
            run("dotnet", str(compiler), mode, str(SOURCES / f"{module}.mpl"),
                str(OUTPUT / f"{project}.dll"), str(INSTALL), project)
        run("dotnet", "build", "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj",
            "-c", "Release", "-o", str(AUDIT_OUT), "-v", "quiet")

    def test_methods_attributes_and_fields_match(self):
        amap = json.loads((ROOT / "docs/mirror/assembly-map.json").read_text(encoding="utf-8"))
        mapped = {Path(unit["original"]["file"]).stem.lower(): unit for unit in amap["units"]
                  if unit.get("language") == "Mpl"}
        auditor = AUDIT_OUT / "Quire.Tools.MethodAudit.dll"
        for module in MODULES:
            project = f"Quire.{module}"
            original_path = INSTALL / mapped[module.lower()]["original"]["file"]
            candidate_path = OUTPUT / f"{project}.dll"
            original_block = class_block(run("ilspycmd", "-il", str(original_path)), f"{module}.values")
            candidate_block = class_block(run("ilspycmd", "-il", str(candidate_path)), f"{project}.values")
            original_attrs = custom_attributes(original_block)
            candidate_attrs = custom_attributes(candidate_block)
            self.assertEqual(original_attrs, candidate_attrs[:2], module + " Mpl attribute payloads")
            self.assertEqual([name for name, _ in original_attrs],
                             ["Mpl.Run.ModuleTextAttribute", "Mpl.Run.ModuleInterfaceAttribute"],
                             module + " original attribute order")
            self.assertEqual([name for name, _ in candidate_attrs],
                             ["Mpl.Run.ModuleTextAttribute", "Mpl.Run.ModuleInterfaceAttribute",
                              "Quire.Mirror.MirrorOfAttribute"], module + " candidate attribute order")
            expected_fields = (member_names(original_block, "field") if module in ("MplUnitSystemSI", "EfiPlot", "McdUnitSystemNone", "McdStaticUnitSystemNone", "McdUnitSystemCGS")
                               else ["gamma", "pi", "e"] if module == "MplConstants"
                               else [] if module in ("MplPrimitives", "EfiFitting", "EfiDebug") else ["system"])
            self.assertEqual(member_names(original_block, "field"), expected_fields, module + " field order")
            self.assertEqual(member_names(candidate_block, "field"), expected_fields,
                             module + " candidate field order")
            expected_methods = member_names(original_block, "method")
            self.assertEqual(member_names(candidate_block, "method"), expected_methods, module + " method order")
            audit = run("dotnet", str(auditor), "--repo", str(ROOT), "--assemblies", str(OUTPUT),
                        "--install", str(INSTALL), "--project", project, "--diff", f"{project}.values")
            exact_lines = [line for line in audit.splitlines() if line.endswith(": exact")]
            self.assertEqual(len(exact_lines), len(expected_methods), module + " MethodAudit exact bodies\n" + audit)


if __name__ == "__main__":
    unittest.main()
