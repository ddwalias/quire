"""Dump Mpl module interface attributes from the installed Prime assemblies to .mirror-cache.

Requires the repo's ilspycmd tool and an installed Mathcad Prime 12.0.1.0. It never writes
original IL or binaries to the repository's tracked tree.
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
ATTRIBUTE = re.compile(
    r"\.custom instance void \[mplrun\]Mpl\.Run\.ModuleInterfaceAttribute::\.ctor\(string\) = \(\s*"
    r"((?:[0-9a-fA-F]{2}\s*)+)\)")
END_CLASS = re.compile(r"\}\s*// end of class\s+(.+?)\s*$")


def read_ser_string(blob: bytes) -> str:
    if blob[:2] != b"\x01\x00":
        raise ValueError("unexpected custom attribute prolog")
    if len(blob) < 3:
        raise ValueError("truncated custom attribute")
    first = blob[2]
    if first == 0xFF:
        return ""
    if first & 0x80 == 0:
        size, offset = first, 3
    elif first & 0xC0 == 0x80:
        if len(blob) < 4:
            raise ValueError("truncated 2-byte compressed string length")
        size, offset = ((first & 0x3F) << 8) | blob[3], 4
    elif first & 0xE0 == 0xC0:
        if len(blob) < 6:
            raise ValueError("truncated 4-byte compressed string length")
        size = ((first & 0x1F) << 24) | (blob[3] << 16) | (blob[4] << 8) | blob[5]
        offset = 6
    else:
        raise ValueError("invalid compressed string length")
    return blob[offset:offset + size].decode("utf-8")


def class_blocks(il: str):
    lines = il.splitlines()
    for end_index, line in enumerate(lines):
        match = END_CLASS.match(line)
        if not match:
            continue
        name = match.group(1)
        start_index = next((index for index in range(end_index - 1, -1, -1)
                            if lines[index].startswith(".class ")
                            and lines[index].split()[-1] == name), None)
        if start_index is not None:
            yield name, "\n".join(lines[start_index:end_index + 1])


def interfaces(assembly: str, path: Path):
    proc = subprocess.run(["ilspycmd", "-il", str(path)], capture_output=True,
                          text=True, encoding="utf-8", errors="replace")
    if proc.returncode:
        raise RuntimeError(f"ilspycmd failed for {path.name}: {proc.stderr[-2000:]}")
    result = []
    for type_name, block in class_blocks(proc.stdout):
        for match in ATTRIBUTE.finditer(block):
            text = read_ser_string(bytes.fromhex(match.group(1)))
            lines = [line.strip() for line in text.splitlines() if line.strip()]
            header = lines[0] if lines else ""
            declarations = [line for line in lines[1:] if line not in ("where", "{", "}")]
            result.append({"assembly": assembly, "type": type_name,
                           "interface": text, "module_header": header,
                           "declarations": declarations})
    return assembly, result


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--install", type=Path, default=DEFAULT_INSTALL)
    parser.add_argument("--workers", type=int, default=4)
    args = parser.parse_args()
    assembly_map = json.loads((ROOT / "docs/mirror/assembly-map.json").read_text(encoding="utf-8"))
    modules = sorted((unit["original"]["name"], args.install / unit["original"]["file"])
                     for unit in assembly_map["units"] if unit.get("language") == "Mpl")
    missing = [str(path) for _, path in modules if not path.is_file()]
    if missing:
        parser.error("missing original assemblies:\n" + "\n".join(missing))
    if len(modules) != 51:
        parser.error(f"expected 51 Mpl assemblies from assembly-map.json; found {len(modules)}")

    stamp = "\n".join(f"{name}:{hashlib.sha256(path.read_bytes()).hexdigest()}" for name, path in modules)
    key = hashlib.sha256(stamp.encode("ascii")).hexdigest()[:16]
    out = ROOT / ".mirror-cache" / "mpl-interfaces" / key
    out.mkdir(parents=True, exist_ok=True)
    all_interfaces = []
    with ThreadPoolExecutor(max_workers=max(1, args.workers)) as pool:
        futures = {pool.submit(interfaces, name, path): name for name, path in modules}
        for future in as_completed(futures):
            name, found = future.result()
            all_interfaces.extend(found)
            print(f"{name}: {len(found)} interface(s)", flush=True)

    all_interfaces.sort(key=lambda item: (item["assembly"].lower(), item["type"].lower()))
    for item in all_interfaces:
        stem = f"{item['assembly']}.{item['type']}"
        (out / f"{stem}.interface.txt").write_text(item["interface"], encoding="utf-8")
        (out / f"{stem}.declarations.json").write_text(
            json.dumps({"assembly": item["assembly"], "type": item["type"],
                        "module_header": item["module_header"],
                        "declarations": item["declarations"]}, indent=2, ensure_ascii=False) + "\n",
            encoding="utf-8")
    (out / "index.json").write_text(json.dumps({
        "interface_cache_key": key,
        "assemblies": [name for name, _ in modules],
        "types_with_interfaces": all_interfaces,
    }, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Wrote {len(all_interfaces)} ModuleInterface value(s) from {len(modules)} assemblies to {out}")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        print(error, file=sys.stderr)
        raise SystemExit(1)
