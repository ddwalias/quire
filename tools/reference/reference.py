"""Generate a local, uncommitted reference for one original Mathcad type: its ILSpy C# view, its member index (tokens
and signatures an evidence record cites) and, on request, its IL. Output goes to a gitignored cache keyed by the
artifact's SHA-256, so a different Mathcad build never serves a stale view. Nothing here is ever committed."""
import argparse
import hashlib
import os
from pathlib import Path
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_INSTALL = (r"C:\Program Files\PTC\Mathcad Prime 12.0.1.0" if os.name == "nt"
                   else "/mnt/c/Program Files/PTC/Mathcad Prime 12.0.1.0")


def run(command, **kwargs):
    result = subprocess.run(command, capture_output=True, text=True, **kwargs)
    if result.returncode:
        sys.exit(f"{' '.join(map(str, command))} failed:\n{result.stdout}{result.stderr}")
    return result.stdout


def type_il(listing, name):
    """The `.class` block of a top-level type (nested types included) from a whole-assembly IL listing; ilspycmd
    ignores its type filter when printing IL."""
    lines = listing.splitlines()
    end = next((i for i, line in enumerate(lines) if line.rstrip() == f"}} // end of class {name}"), None)
    if end is None:
        sys.exit(f"{name}: no top-level class of that name in the IL listing")
    start = next(i for i in range(end, -1, -1) if lines[i].startswith(".class "))
    return "\n".join(lines[start:end + 1]) + "\n"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("type", help="top-level Mathcad type, e.g. Mcd.Dim")
    parser.add_argument("--assembly", default="mplrun", help="original assembly name (default: mplrun)")
    parser.add_argument("--install", type=Path, default=Path(os.environ.get("MATHCAD_DIR", DEFAULT_INSTALL)))
    parser.add_argument("--il", action="store_true", help="also extract the type's IL")
    parser.add_argument("--force", action="store_true", help="regenerate cached files")
    args = parser.parse_args()
    dll = args.install / f"{args.assembly}.dll"
    if not dll.is_file():
        sys.exit(f"{dll} not found; set MATHCAD_DIR or --install")
    sha = hashlib.sha256(dll.read_bytes()).hexdigest()
    cache = ROOT / ".mirror-cache" / args.assembly / sha[:16]
    cache.mkdir(parents=True, exist_ok=True)
    tool = run(["ilspycmd", "--version"]).splitlines()[0].split()[-1]

    members = cache / f"{args.type}.members.tsv"
    if args.force or not members.is_file():
        audit = ROOT / ".mirror-cache" / ".method-audit"
        run(["dotnet", "build", str(ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj"),
             "-c", "Release", "-o", str(audit), "-nologo", "-v", "q"])
        members.write_text(run(["dotnet", str(audit / "Quire.Tools.MethodAudit.dll"), "members", str(dll), args.type]))
    identity = "".join(line for line in members.read_text().splitlines(keepends=True) if line.startswith("#"))

    source = cache / f"{args.type}.cs"
    if args.force or not source.is_file():
        source.write_text(f"// ilspycmd {tool}\n" + identity.replace("# ", "// ")
                          + run(["ilspycmd", "-t", args.type, str(dll)]))
    outputs = [members, source]

    if args.il:
        listing = cache / f"{args.assembly}.il"
        if args.force or not listing.is_file():
            listing.write_text(run(["ilspycmd", "-il", str(dll)]))
        il = cache / f"{args.type}.il"
        if args.force or not il.is_file():
            il.write_text(f"// ilspycmd {tool}\n" + identity.replace("# ", "// ") + type_il(listing.read_text(), args.type))
        outputs.append(il)
    print(f"ilspycmd {tool}\n{identity}" + "".join(f"{path.relative_to(ROOT)}\n" for path in outputs), end="")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
