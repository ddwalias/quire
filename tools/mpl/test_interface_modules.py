"""Compile interface-only Mpl sources and compare them with Prime Release modules."""
from __future__ import annotations

import json
import os
from pathlib import Path
import re
import subprocess
import unittest

ROOT = Path(__file__).resolve().parents[2]
INSTALL = Path(os.environ.get("MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"))
SOURCES = ROOT / "tools/mpl/interface-sources"
OUTPUT = ROOT / "tools/mpl/output"
CACHE = ROOT / ".mirror-cache/mpl-interface-emitter"
COMPILER_OUT = CACHE / "compiler"
AUDIT_OUT = CACHE / "method-audit"
MODULES = ["EfiAffine", "EfiFourier", "EfiSpecial", "EfiFinance", "EfiBessel",
           "EfiSixsigma", "EfiData", "EfiIO", "EfiWavelets", "EfiSignal", "EfiImage", "EfiStatistics", "McdEfi",
           "Mpl"]
PROJECTS = {"Mpl": "Quire.MplModule"}
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


def attributes(block: str) -> list[tuple[str, bytes]]:
    return [(name, bytes.fromhex(blob)) for name, blob in ATTRIBUTE.findall(block)]


def method_names(block: str) -> list[str]:
    return re.findall(r"\}\s*// end of method [^:]+::([^\s]+)", block)


def class_names(il: str) -> list[str]:
    return [line.split()[-1].strip("'") for line in il.splitlines() if line.startswith(".class ")]



class InterfaceModuleTests(unittest.TestCase):
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
            source = SOURCES / f"{module}.mpl"
            project = PROJECTS.get(module, f"Quire.{module}")
            run("dotnet", str(compiler), "--interface", str(source), str(OUTPUT / f"{project}.dll"),
                str(INSTALL), project)
        run("dotnet", "build", "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj",
            "-c", "Release", "-o", str(AUDIT_OUT), "-v", "quiet")

    def test_methods_attributes_and_metadata_order_match(self):
        amap = json.loads((ROOT / "docs/mirror/assembly-map.json").read_text(encoding="utf-8"))
        mapped = {Path(unit["original"]["file"]).stem.lower(): unit for unit in amap["units"]
                  if unit.get("language") == "Mpl"}
        auditor = AUDIT_OUT / "Quire.Tools.MethodAudit.dll"
        for module in MODULES:
            project = PROJECTS.get(module, f"Quire.{module}")
            original_path = INSTALL / mapped[module.lower()]["original"]["file"]
            candidate_path = OUTPUT / f"{project}.dll"
            original_il = run("ilspycmd", "-il", str(original_path))
            candidate_il = run("ilspycmd", "-il", str(candidate_path))
            original_type = f"{module}.values"
            candidate_type = f"{project}.values"
            original_block = class_block(original_il, original_type)
            candidate_block = class_block(candidate_il, candidate_type)

            original_attributes = [a for a in attributes(original_block)
                                   if a[0] in {"Mpl.Run.ModuleTextAttribute", "Mpl.Run.ModuleInterfaceAttribute"}]
            candidate_attributes = [a for a in attributes(candidate_block)
                                    if a[0] in {"Mpl.Run.ModuleTextAttribute", "Mpl.Run.ModuleInterfaceAttribute"}]
            self.assertEqual(original_attributes, candidate_attributes, module + " Mpl attribute payload/order")
            original_declaration = [line.strip() for line in original_block.splitlines()
                                    if line.startswith(".class ") or line.strip().startswith("extends ")]
            candidate_declaration = [line.strip().replace(candidate_type, original_type)
                                     for line in candidate_block.splitlines()
                                     if line.startswith(".class ") or line.strip().startswith("extends ")]
            self.assertEqual(original_declaration, candidate_declaration, module + " type flags/base")
            self.assertEqual([a[0] for a in original_attributes],
                             ["Mpl.Run.ModuleTextAttribute", "Mpl.Run.ModuleInterfaceAttribute"],
                             module + " original attribute order")
            self.assertEqual([a[0] for a in attributes(candidate_block)],
                             ["Mpl.Run.ModuleTextAttribute", "Mpl.Run.ModuleInterfaceAttribute",
                              "Quire.Mirror.MirrorOfAttribute"], module + " candidate attribute order")
            self.assertEqual(method_names(original_block), ["Update", ".cctor", ".ctor"], module + " original order")
            self.assertEqual(method_names(candidate_block), method_names(original_block), module + " candidate order")
            self.assertFalse(re.search(r"^\s*\.field\b", original_block, re.M))
            self.assertFalse(re.search(r"^\s*\.field\b", candidate_block, re.M))

            expected_candidate_types = [name.replace(original_type, candidate_type)
                                        for name in class_names(original_il)]
            self.assertEqual(class_names(candidate_il), expected_candidate_types, module + " type order")

            audit = run("dotnet", str(auditor), "--repo", str(ROOT), "--assemblies", str(OUTPUT),
                        "--install", str(INSTALL), "--project", project, "--diff", candidate_type)
            exact_lines = [line for line in audit.splitlines() if line.endswith(": exact")]
            self.assertEqual(len(exact_lines), 3, module + " MethodAudit must find three exact methods\n" + audit)


if __name__ == "__main__":
    unittest.main()
