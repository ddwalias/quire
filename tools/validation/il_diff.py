"""Where Quire's Release IL first differs from Mathcad's, for the focused profile's methods (Quire.Mpl):

    python3 tools/validation/il_diff.py KEY [KEY ...]

KEY is a type (`Quire.Mcd.MatDim`), `Type::Name` (every overload) or an inventory key (`Type::signature`). Builds what
validate.py's method audit reads (MethodAudit, the Release structure build and the Debug test build), then runs
MethodAudit --diff: per method its verdict and, when the instructions differ, the first mismatching one with three
instructions either side, Quire's and Mathcad's, each Quire instruction with its source line from the Release PDB.
Mathcad's instructions go to stdout only; nothing is written to the repository but the usual Debug build output."""
import argparse
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path
from validate import ROOT, mathcad_install, method_audit, resolve_dotnet, sdk_directory


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("key", nargs="+", help="type, Type::Name or Type::signature")
    parser.add_argument("--dotnet", default="dotnet", help="dotnet CLI (default: dotnet on PATH)")
    parser.add_argument("--timeout", type=int, default=1800, help="seconds per build")
    args = parser.parse_args(argv)
    dotnet = resolve_dotnet(args.dotnet, ROOT)
    global_json = (ROOT / "global.json").read_bytes() if (ROOT / "global.json").is_file() else b"{}\n"
    tool_dir = Path(tempfile.mkdtemp(prefix="quire-audit-tool-"))
    structure_dir = Path(tempfile.mkdtemp(prefix="quire-audit-release-"))
    try:
        structure_build, scan = method_audit(dotnet, tool_dir, structure_dir, mathcad_install(), None, "focused")
        at = scan.index("--output")
        del scan[at:at + 2]
        host = structure_build[0]
        for target, extra in [(ROOT / "tools/Quire.Tools.MethodAudit/Quire.Tools.MethodAudit.csproj", ["-o", str(tool_dir)]),
                              structure_build, (host, [])]:
            with sdk_directory(global_json) as sdk:
                result = subprocess.run([dotnet, "build", str(target), "-nologo", "-v", "q", *extra], cwd=sdk,
                                        capture_output=True, text=True, timeout=args.timeout)
            if result.returncode:
                sys.stderr.write(result.stdout[-4000:] + result.stderr[-4000:])
                print(f"build of {target.name} failed", file=sys.stderr)
                return 2
        with sdk_directory(global_json) as sdk:
            return subprocess.run(scan + [a for key in args.key for a in ("--diff", key)], cwd=sdk,
                                  timeout=args.timeout).returncode
    finally:
        shutil.rmtree(tool_dir, ignore_errors=True)
        shutil.rmtree(structure_dir, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
