"""Record reviewed internal targets of a native original in the manifest that native_bytes.py checks them against.

Each --name is a decorated symbol that the candidate objects reference and the original does not export, reviewed as
the same function or data as the original's. Its RVA is read from the original at a relocation site of a candidate
function whose bytes already match the original (an export, or an entry already recorded), so an entry records where
the original's own code points; the targets of a recorded entry are searched in turn. A function entry must start a
.pdata function, or be a leaf function in an executable section outside every .pdata range (native_bytes.py). A data entry takes the size of the candidate's definition. Sites that disagree, names that no
matching site references, and existing entries that would change fail without writing.

Each --toolchain is recorded the same way, as a toolchain entry: a symbol the toolset's static libraries supply (such as
the C runtime's `??_7type_info@@6B@`), which the candidates reference but must not define: a name any --candidate
object defines is refused. It records the RVA only.

The manifest holds names, RVAs and sizes only, never bytes of the original (D15). Review the written diff before
committing it: native_bytes.py compares every entry the candidate objects define on each run without --symbol.
"""
import argparse
import json
import sys
from pathlib import Path

from native_bytes import (DEFAULT_TARGETS, PeImage, _addend, _resolve, anonymous, compare_code,
                          definitions_in_objects, targets_from)


def record(image, definitions, names, doc, toolchain=()):
    """Record `names`, and `toolchain` as toolchain entries, for the original `image` in the manifest document `doc`
    (changed in place, and returned).

    `definitions` are the candidates' definitions by anonymous() name. Names match tag-insensitively (anonymous()): an
    entry already stored under another anonymous-namespace tag keeps its stored name. Raises ValueError when the names
    cannot be recorded.
    """
    entries = targets_from(doc, image)
    given = {anonymous(n): n for n in [*names, *toolchain]}
    names, toolchain = {anonymous(n) for n in names}, {anonymous(n) for n in toolchain}
    defined = sorted(given[n] for n in toolchain & set(definitions))
    if defined:
        raise ValueError(f"{', '.join(defined)}: defined by the candidate objects, so not a toolchain entry")

    def placed(d, rva):
        if d.data:
            return rva, image.read(rva, len(d.code))
        extent = image.function_extent(rva, [e.rva for e in entries.values() if e.size is None])
        return (rva, image.read(rva, extent[1] - rva)) if extent is not None else None

    queue = []
    for d in definitions.values():
        if not d.data and d.name in image.exports:
            queue.append((d, image.exports[d.name], image.export_code(d.name)))
        elif anonymous(d.name) in entries:
            at = placed(d, entries[anonymous(d.name)].rva)
            if at:
                queue.append((d, *at))
    wanted, found, searched = set(names) | set(toolchain), {}, set()
    while queue:
        d, begin, code = queue.pop()
        if d.name in searched or not compare_code(d.name, code, d.code, d.relocs).ok:
            continue
        searched.add(d.name)
        for r in d.relocs:
            name = d.targets.get(r.offset) or d.statics.get(r.offset)
            name = name and anonymous(name)
            if name not in wanted:
                continue
            rva = _resolve(image, begin + r.offset, r.kind) - _addend(d.code, r)
            if found.setdefault(name, rva) != rva:
                raise ValueError(f"{name}: sites disagree, RVA {found[name]:#x} and {rva:#x} (from {d.name})")
            callee = definitions.get(name)
            at = placed(callee, rva) if callee is not None else None
            if at:
                queue.append((callee, *at))

    missing = sorted(given[n] for n in wanted - set(found))
    if missing:
        raise ValueError("not referenced by any candidate function that matches the original: " + ", ".join(missing))
    bound = next((doc[k] for k in doc if k.lower() == image.name.lower()), None)
    if bound is None:
        bound = doc[image.name] = {"sha256": image.sha256, "targets": {}}
    for name in sorted(found):
        defn = definitions.get(name)
        entry = {"rva": f"{found[name]:#x}"}
        if name in toolchain:
            entry["toolchain"] = True
        elif defn is None:
            raise ValueError(f"{name}: not defined by the candidate objects, so its kind and size are unknown")
        elif defn.data:
            entry["size"] = len(defn.code)
        elif image.function_extent(found[name]) is None:
            raise ValueError(f"{name}: RVA {found[name]:#x} does not start a .pdata function or a leaf function")
        stored = next((k for k in bound["targets"] if anonymous(k) == name), given[name])
        old = bound["targets"].get(stored)
        if old is not None and old != entry:
            raise ValueError(f"{stored}: manifest has {old}, original gives {entry}; remove the entry to re-record it")
        bound["targets"][stored] = entry
    bound["targets"] = dict(sorted(bound["targets"].items()))
    targets_from(doc, image)  # never write a manifest that native_bytes.py would reject
    return doc


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--original", type=Path, required=True, help="original DLL (read only)")
    parser.add_argument("--candidate", type=Path, action="append", required=True,
                        help="COFF object compiled with the original's flags (repeatable)")
    parser.add_argument("--name", action="append", default=[], help="reviewed decorated target name (repeatable)")
    parser.add_argument("--toolchain", action="append", default=[],
                        help="reviewed decorated name the toolset supplies, recorded as a toolchain entry (repeatable)")
    parser.add_argument("--targets", type=Path, default=DEFAULT_TARGETS, help="manifest to update")
    args = parser.parse_args(argv)
    if not args.name and not args.toolchain:
        parser.error("give --name, --toolchain or both")
    image = PeImage(args.original)
    try:
        definitions = definitions_in_objects(args.candidate)
        doc = json.loads(args.targets.read_text(encoding="utf-8")) if args.targets.is_file() else {}
        record(image, definitions, args.name, doc, args.toolchain)
    except ValueError as e:  # includes ManifestError and malformed JSON
        print(f"FAIL {e}")
        return 1
    args.targets.write_text(json.dumps(doc, indent=2) + "\n", encoding="utf-8")
    bound = next(doc[k] for k in doc if k.lower() == image.name.lower())
    for name in sorted(set(args.name) | set(args.toolchain)):
        stored = next(k for k in bound["targets"] if anonymous(k) == anonymous(name))
        print(f"recorded {stored}: {bound['targets'][stored]}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
