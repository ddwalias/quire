"""Collect reproducible structure/code-shape evidence for mapped Mpl assemblies.

All original IL and decoded interface payloads are written under ignored
.mirror-cache. The committed output is a compact numeric analysis only.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
from concurrent.futures import ThreadPoolExecutor, as_completed
from pathlib import Path
import re
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_INSTALL = Path(os.environ.get(
    "MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0"))
CLASS = re.compile(r"^\.class\s+.*?\s+([^\s]+)\s*$", re.M)
METHOD_START = re.compile(r"^\s*\.method\b", re.M)
METHOD_END = re.compile(r"\}\s*// end of method\s+(.+?)\s*$", re.M)
INSN = re.compile(r"^\s*IL_[0-9a-fA-F]+:\s+([.\w]+)", re.M)
ATTR = re.compile(r"\.custom instance void \[mplrun\]Mpl\.Run\.(ModuleText|ModuleInterface)Attribute::\.ctor\(string\) = \(\s*((?:[0-9a-fA-F]{2}\s*)+)\)", re.S)


def decode_ser_string(raw: bytes) -> str:
    if raw[:2] != b"\x01\x00":
        raise ValueError("invalid custom-attribute prolog")
    first = raw[2]
    if first == 0xff:
        return ""
    if first < 0x80:
        size, pos = first, 3
    elif first < 0xc0:
        size, pos = ((first & 0x3f) << 8) | raw[3], 4
    else:
        size, pos = ((first & 0x1f) << 24) | (raw[3] << 16) | (raw[4] << 8) | raw[5], 6
    return raw[pos:pos + size].decode("utf-8")


def run_module(name: str, path: Path, cache: Path) -> dict:
    cached_il = cache / f"{name}.il"
    if cached_il.is_file():
        il = cached_il.read_text(encoding="utf-8")
    else:
        proc = subprocess.run(["ilspycmd", "-il", str(path)], capture_output=True,
                              text=True, encoding="utf-8", errors="replace")
        if proc.returncode:
            raise RuntimeError(f"ilspycmd failed for {path.name}: {proc.stderr[-1000:]}")
        il = proc.stdout
        cached_il.write_text(il, encoding="utf-8")
    type_names = CLASS.findall(il)
    methods = []
    starts = list(METHOD_START.finditer(il))
    ends = list(METHOD_END.finditer(il))
    end_by_pos = iter(ends)
    end = next(end_by_pos, None)
    for start in starts:
        while end and end.start() < start.end():
            end = next(end_by_pos, None)
        if not end:
            break
        block = il[start.start():end.start()]
        opcodes = INSN.findall(block)
        methods.append({"name": end.group(1), "opcodes": opcodes, "body": block})
        end = next(end_by_pos, None)
    attrs = {k: decode_ser_string(bytes.fromhex(v)) for k, v in ATTR.findall(il)}
    interface = attrs.get("ModuleInterface", "")
    real = [m for m in methods if len(m["opcodes"]) > 6]
    thin = [m for m in methods if len(m["opcodes"]) <= 6]
    boilerplate = ([], ["ret"], ["call", "ret"], ["ldarg.0", "call", "ret"])
    signatures = sum(" :: " in line for line in interface.splitlines())
    if not real and signatures and len(methods) == 3 and all(m["opcodes"] in boilerplate for m in methods):
        shape = "interface-only" if any(m["opcodes"] == ["ret"] for m in methods) else "trivial-only"
    elif not real and len(methods) == 3 and all(m["opcodes"] in boilerplate for m in methods):
        shape = "trivial-only"
    elif real:
        shape = "real-code"
    else:
        shape = "thin-only"
    family_counts: dict[str, int] = {}
    for method in methods:
        label = family(method)
        family_counts[label] = family_counts.get(label, 0) + 1
    return {
        "assembly": name,
        "dll_bytes": path.stat().st_size,
        "types": len(type_names),
        "methods": len(methods),
        "thin_bodies_le_6_ops": len(thin),
        "empty_ret_bodies": sum(m["opcodes"] == ["ret"] for m in methods),
        "real_bodies_gt_6_ops": len(real),
        "shape": shape,
        "module_text": attrs.get("ModuleText", "<missing>"),
        "module_interface_chars": len(interface),
        "interface_signature_lines": signatures,
        "data_declaration_lines": sum(line.lstrip().startswith("data ") for line in interface.splitlines()),
        "import_lines": sum(line.lstrip().startswith("import ") for line in interface.splitlines()),
        "at_annotations": interface.count(" @ "),
        "arrow_tokens": interface.count("->"),
        "module_interface": interface,
        "families": family_counts,
        "methods_detail": methods,
    }


def normalize(method: dict) -> str:
    return " ".join(method["opcodes"])


def pe_exports(path: Path) -> list[str]:
    data = path.read_bytes()
    pe = int.from_bytes(data[0x3c:0x40], "little")
    sections_count = int.from_bytes(data[pe + 6:pe + 8], "little")
    optional_size = int.from_bytes(data[pe + 20:pe + 22], "little")
    optional = pe + 24
    magic = int.from_bytes(data[optional:optional + 2], "little")
    directory = optional + (112 if magic == 0x20b else 96)
    export_rva = int.from_bytes(data[directory:directory + 4], "little")
    section_start = optional + optional_size
    sections = []
    for i in range(sections_count):
        off = section_start + 40 * i
        virtual_size = int.from_bytes(data[off + 8:off + 12], "little")
        virtual_address = int.from_bytes(data[off + 12:off + 16], "little")
        raw_size = int.from_bytes(data[off + 16:off + 20], "little")
        raw_pointer = int.from_bytes(data[off + 20:off + 24], "little")
        sections.append((virtual_address, max(virtual_size, raw_size), raw_pointer))

    def file_offset(rva: int) -> int:
        for va, size, pointer in sections:
            if va <= rva < va + size:
                return pointer + rva - va
        raise ValueError(f"RVA 0x{rva:x} not in PE section")

    if not export_rva:
        return []
    directory_off = file_offset(export_rva)
    name_count = int.from_bytes(data[directory_off + 24:directory_off + 28], "little")
    names_rva = int.from_bytes(data[directory_off + 32:directory_off + 36], "little")
    names_off = file_offset(names_rva)
    result = []
    for i in range(name_count):
        string_off = file_offset(int.from_bytes(data[names_off + i * 4:names_off + i * 4 + 4], "little"))
        end = data.index(b"\0", string_off)
        result.append(data[string_off:end].decode("ascii", errors="replace"))
    return result


def family(method: dict) -> str:
    ops = method["opcodes"]
    body = method["body"]
    if len(ops) <= 6:
        return "thin-wrapper-or-init"
    if ".try" in body or "catch " in body or "finally" in body:
        return "exception"
    if "switch " in body:
        return "dispatch"
    branches = re.findall(r"IL_([0-9a-fA-F]+):\s+(?:br\w*|leave\w*)\s+IL_([0-9a-fA-F]+)", body)
    if any(int(target, 16) <= int(source, 16) for source, target in branches):
        return "loop"
    if any(op.startswith("br") or op.startswith("leave") for op in ops):
        return "conditional"
    if any(op in {"newarr", "ldelem", "ldelem.ref", "stelem", "stelem.ref"} or op.startswith(("ldelem.", "stelem.")) for op in ops):
        return "array"
    if any(op.startswith(("ldfld", "ldsfld", "stfld", "stsfld")) for op in ops):
        return "fields"
    if any(op in {"call", "callvirt", "calli"} for op in ops):
        return "calls"
    if any(op.startswith(("add", "sub", "mul", "div", "rem", "and", "or", "xor", "shl", "shr", "ceq", "cgt", "clt")) for op in ops):
        return "arithmetic"
    if "newobj" in ops:
        return "allocation"
    return "straightline"


def main() -> int:
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--install", type=Path, default=DEFAULT_INSTALL)
    p.add_argument("--workers", type=int, default=2)
    args = p.parse_args()
    amap = json.loads((ROOT / "docs/mirror/assembly-map.json").read_text(encoding="utf-8"))
    modules = sorted((u["original"]["name"], args.install / u["original"]["file"])
                     for u in amap["units"] if u.get("language") == "Mpl")
    if len(modules) != 51 or any(not f.is_file() for _, f in modules):
        p.error(f"expected 51 installed Mpl assemblies; found {len(modules)}")
    stamp = "\n".join(f"{n}:{hashlib.sha256(f.read_bytes()).hexdigest()}" for n, f in modules)
    key = hashlib.sha256(stamp.encode("ascii")).hexdigest()[:16]
    out = ROOT / ".mirror-cache" / "mpl-il-families" / key
    out.mkdir(parents=True, exist_ok=True)
    msgen = args.install / "msgen.dll"
    msgen_types = subprocess.run(["ilspycmd", "--list", "types", str(msgen)], capture_output=True,
                                 text=True, encoding="utf-8", errors="replace")
    if msgen_types.returncode:
        p.error(f"ilspycmd type listing failed for msgen.dll: {msgen_types.stderr[-1000:]}")
    (out / "msgen.types.txt").write_text(msgen_types.stdout, encoding="utf-8")
    gen_types = [line for line in msgen_types.stdout.splitlines() if line.startswith(("Struct gen.", "Class gen."))]
    rows = []
    with ThreadPoolExecutor(max_workers=max(1, args.workers)) as pool:
        futures = {pool.submit(run_module, n, f, out): n for n, f in modules}
        for future in as_completed(futures):
            row = future.result()
            rows.append(row)
            print(f"{row['assembly']}: {row['methods']} methods, {row['real_bodies_gt_6_ops']} real-code bodies", flush=True)
    rows.sort(key=lambda row: row["assembly"].lower())
    aggregate = {"assemblies": len(rows), "methods": sum(r["methods"] for r in rows),
                 "types": sum(r["types"] for r in rows), "dll_bytes": sum(r["dll_bytes"] for r in rows),
                 "thin_bodies": sum(r["thin_bodies_le_6_ops"] for r in rows),
                 "empty_ret_bodies": sum(r["empty_ret_bodies"] for r in rows),
                 "real_bodies": sum(r["real_bodies_gt_6_ops"] for r in rows),
                 "nonempty_module_text": sum(bool(r["module_text"]) for r in rows),
                 "missing_module_text": sum(r["module_text"] == "<missing>" for r in rows),
                 "module_interface_chars": sum(r["module_interface_chars"] for r in rows),
                 "interface_signature_lines": sum(r["interface_signature_lines"] for r in rows),
                 "data_declaration_lines": sum(r["data_declaration_lines"] for r in rows),
                 "import_lines": sum(r["import_lines"] for r in rows),
                 "at_annotations": sum(r["at_annotations"] for r in rows),
                 "arrow_tokens": sum(r["arrow_tokens"] for r in rows)}
    maxstack = {}
    locals_init = 0
    for _, assembly_path in modules:
        il = (out / f"{assembly_path.stem}.il").read_text(encoding="utf-8")
        for value in re.findall(r"^\s*\.maxstack\s+(\d+)", il, re.M):
            maxstack[value] = maxstack.get(value, 0) + 1
        locals_init += len(re.findall(r"^\s*\.locals init\b", il, re.M))
    aggregate["maxstack_histogram"] = dict(sorted(maxstack.items(), key=lambda item: int(item[0])))
    aggregate["methods_with_locals_init"] = locals_init
    aggregate["pdb_sibling_count"] = sum((args.install / (assembly_path.stem + ".pdb")).is_file()
                                         for _, assembly_path in modules)
    samples = ["Hashtable", "McdRun", "McdTranslator", "MplPrelude", "EfiSymbolicsLexer"]
    sample_methods = [m for row in rows if row["assembly"] in samples for m in row["methods_detail"]]
    exact = {}
    for m in sample_methods:
        sig = normalize(m)
        exact[sig] = exact.get(sig, 0) + 1
    top = sorted(exact.items(), key=lambda item: (-item[1], item[0]))[:10]
    report = {"install_hash_key": key, "aggregate": aggregate,
              "module_facts": [{k: v for k, v in row.items() if k not in ("methods_detail", "module_interface")} for row in rows],
              "sample_modules": samples,
              "sample_total_methods": len(sample_methods),
              "sample_construct_families": {family: sum(1 for m in sample_methods if globals()["family"](m) == family)
                  for family in sorted({family(m) for m in sample_methods})},
              "sample_method_opcode_sequences": {"unique": len(exact), "top_10": top,
                  "top_10_coverage": sum(n for _, n in top),
                  "top_10_fraction": sum(n for _, n in top) / max(1, len(sample_methods))},
              "generator_probe": {"msgen_exports": pe_exports(msgen), "msgen_types_total": len(msgen_types.stdout.splitlines()),
                  "gen_types": gen_types},
              "module_methods": [{"assembly": row["assembly"], "methods": row["methods_detail"]} for row in rows]}
    (out / "report.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(aggregate, indent=2))
    print(f"Report and IL cache: {out}")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        print(error, file=sys.stderr)
        raise SystemExit(1)
