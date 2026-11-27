"""Regenerate the Mirror trees with the current MirrorGen writer, to a fixed point.

    python tools/validation/mirror_regenerate.py [--root PATH] [--no-stamp]

Emit reads each production project's Release build, and the build reads the Generated trees it emits, so a
regeneration is a loop: stamp, build, emit, and repeat until an emit writes nothing new.

1. Stamp (once per tree; a no-op after the first run). A Generated field or const written before the writer marked
   fields has no [MirrorStub], so its metadata reads as hand-written and emit would drop it. The stamp adds the marker
   the current writer emits. --no-stamp skips it, for a tree that is already stamped.
2. Build the production projects in Release into a generated solution (the projects emit reads; the tests project is
   left out).
3. Emit. Build and emit again while the emit changes files. A round that writes nothing is the fixed point; the run
   fails after MAX_ROUNDS rounds that still change files.

The product Generated trees change only through this script, and only when the maintainer runs it.
"""
import argparse
from pathlib import Path
import re
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[2]
MAX_ROUNDS = 3
FIELD = re.compile(r"^(\s+)(public|private|internal|protected)\b[^(=]*;\s*$")
CONST = re.compile(r"^(\s+)(public|private|internal|protected)\b.*\bconst\b.*;\s*$")
MARKER = "[global::Quire.Mirror.MirrorStub]"


def generated_files(root):
    """Every generated type file, in projects at any depth below src (nested source layouts)."""
    return sorted(p for p in (root / "src").glob("**/Generated/Mirror/*.g.cs")
                  if not {"bin", "obj"} & set(p.relative_to(root).parts))


def stamp(root):
    """Add the field marker the current writer emits to Generated fields and consts that lack it. Returns the count."""
    stamped = 0
    for path in sorted(generated_files(root)):
        out = []
        for line in path.read_text(encoding="utf-8").split("\n"):
            match = FIELD.match(line) or CONST.match(line)
            if match and not any(word in line for word in (" event ", "class ", "struct ")):
                # The attribute block above the field (MirrorName sits between the marker and the field) decides.
                attributes = []
                for above in reversed(out):
                    if not above.strip():
                        continue
                    if not above.strip().startswith("["):
                        break
                    attributes.append(above)
                if not any("MirrorStub" in a for a in attributes):
                    out.append(f"{match.group(1)}{MARKER}")
                    stamped += 1
            out.append(line)
        path.write_text("\n".join(out), encoding="utf-8")
    return stamped


def build(root):
    """Build the production projects in Release into a temporary solution; raise on failure."""
    projects = sorted(p for p in root.glob("src/**/*.csproj") if not {"bin", "obj"} & set(p.relative_to(root).parts))
    with tempfile.TemporaryDirectory() as work:
        solution = Path(work) / "production.slnx"
        solution.write_text("<Solution>\n" + "".join(f'  <Project Path="{p.as_posix()}" />\n' for p in projects)
                            + "</Solution>\n", encoding="utf-8")
        result = subprocess.run(["dotnet", "build", str(solution), "-c", "Release", "-nologo", "-v:q",
                                 "-p:EnableSourceControlManagerQueries=false"],
                                cwd=root, capture_output=True, text=True)
    if result.returncode != 0:
        errors = sorted({line.strip() for line in (result.stdout + result.stderr).splitlines() if " error " in line})
        raise RuntimeError("production build failed:\n" + "\n".join(errors[:30]))


def emit(root, tool):
    """Run MirrorGen emit; return the changed-file count it printed, from its summary line."""
    result = subprocess.run(["dotnet", str(tool), "emit"], cwd=root, capture_output=True, text=True)
    if result.returncode != 0:
        raise RuntimeError("MirrorGen emit failed:\n" + (result.stderr or result.stdout)[-2000:])
    return result.stdout.strip().splitlines()[-1] if result.stdout.strip() else ""


def snapshot(root):
    return {p: p.read_bytes() for p in sorted(generated_files(root))}


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    parser.add_argument("--root", type=Path, default=ROOT)
    parser.add_argument("--no-stamp", action="store_true")
    parser.add_argument("--tool", type=Path, default=None,
                        help="MirrorGen.dll (default: the Release build under tools/Quire.Tools.MirrorGen/bin)")
    args = parser.parse_args(argv)
    root = args.root.resolve()
    tool = args.tool or root / "tools" / "Quire.Tools.MirrorGen" / "bin" / "Release" / "net10.0" / "Quire.Tools.MirrorGen.dll"
    if not tool.is_file():
        print(f"build the tool first: dotnet build {root / 'tools' / 'Quire.Tools.MirrorGen'} -c Release", file=sys.stderr)
        return 2
    if not args.no_stamp:
        print(f"stamped {stamp(root)} field marker(s)")
    for round_number in range(1, MAX_ROUNDS + 1):
        build(root)
        before = snapshot(root)
        summary = emit(root, tool)
        changed = [p for p, data in snapshot(root).items() if before.get(p) != data]
        print(f"round {round_number}: {summary}; {len(changed)} generated file(s) changed")
        if not changed:
            print("fixed point reached")
            return 0
    print(f"no fixed point after {MAX_ROUNDS} rounds", file=sys.stderr)
    return 1


if __name__ == "__main__":
    sys.exit(main())
