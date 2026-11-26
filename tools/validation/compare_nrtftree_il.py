"""Compare Release ILSpy listings for the vendored NRtfTree and Mathcad's copy."""

import argparse
import difflib
import os
from pathlib import Path
import re
import subprocess
import sys


def listing(assembly: Path) -> str:
    result = subprocess.run(["ilspycmd", "-il", str(assembly)], capture_output=True, text=True,
                            encoding="utf-8", errors="replace")
    if result.returncode:
        raise RuntimeError(result.stdout + result.stderr)
    return result.stdout


def methods(text: str) -> dict[tuple[str, str], str]:
    result = {}
    owner = ""
    lines = text.splitlines()
    i = 0
    while i < len(lines):
        if lines[i].startswith(".class "):
            owner = lines[i].split()[-1]
        if not lines[i].lstrip().startswith(".method "):
            i += 1
            continue

        start = i
        while i < len(lines) and "cil managed" not in lines[i]:
            i += 1
        if i == len(lines):
            break
        signature = " ".join(" ".join(lines[start:i + 1]).split())
        signature = normalize(signature)
        body_start = next((n for n in range(i + 1, len(lines)) if lines[n].strip() == "{"), None)
        if body_start is None:
            break
        end = next((n for n in range(body_start + 1, len(lines))
                    if lines[n].startswith("\t} // end of method ")), None)
        if end is None:
            break
        body = "\n".join(line for line in lines[body_start + 1:end]
                          if not line.lstrip().startswith("//"))
        body = re.sub(r"\[(?:System\.Private\.CoreLib|System\.Runtime|System\.Collections(?:\.NonGeneric)?|System\.Threading|System\.ObjectModel|netstandard)\]",
                      "[mscorlib]", body)
        body = re.sub(r"\[System\.Drawing(?:\.Common|\.Primitives)?\]", "[System.Drawing]", body)
        key = (owner, signature)
        if key in result:
            raise RuntimeError(f"duplicate method key: {owner} {signature}")
        result[key] = body
        i = end + 1
    return result


def normalize(text: str) -> str:
    text = re.sub(r"\[(?:System\.Private\.CoreLib|System\.Runtime|System\.Collections(?:\.NonGeneric)?|System\.Threading|System\.ObjectModel|netstandard)\]",
                  "[mscorlib]", text)
    text = re.sub(r"\[System\.Drawing(?:\.Common|\.Primitives)?\]", "[System.Drawing]", text)
    return re.sub(r"\s+", " ", text).strip()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("candidate", type=Path, help="Release nrtftree-library.dll built from the fork")
    parser.add_argument("--show", action="append", help="print a unified IL body diff for matching this method name")
    parser.add_argument("--mathcad", type=Path, default=Path(os.environ.get(
        "MATHCAD_DIR", r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0")) / "nrtftree-library.dll")
    args = parser.parse_args()
    original = methods(listing(args.mathcad))
    candidate = methods(listing(args.candidate))
    missing = sorted(original.keys() - candidate.keys())
    extra = sorted(candidate.keys() - original.keys())
    differing = sorted(key for key in original.keys() & candidate.keys()
                       if normalize(original[key]) != normalize(candidate[key]))
    print(f"Mathcad methods: {len(original)}; candidate methods: {len(candidate)}")
    for label, keys in (("missing", missing), ("extra", extra), ("body differs", differing)):
        for owner, signature in keys:
            print(f"{label}: {owner} :: {signature}")
    if args.show:
        for key in differing:
            if not any(needle in key[1] for needle in args.show):
                continue
            print(f"\nIL diff: {key[0]} :: {key[1]}")
            print("\n".join(difflib.unified_diff(original[key].splitlines(), candidate[key].splitlines(),
                                                  fromfile="Mathcad", tofile="fork", lineterm="")))
    print(f"Result: {len(missing)} missing, {len(extra)} extra, {len(differing)} body mismatches")
    return 1 if missing or extra or differing else 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except RuntimeError as error:
        sys.exit(str(error))
