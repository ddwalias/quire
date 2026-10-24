"""Record reviewed internal targets of a native original in the manifest that native_bytes.py checks them against.

Each --name is a decorated symbol that the candidate objects reference and the original does not export, reviewed as
the same function or data as the original's. Its RVA is read from the original at a relocation site of a candidate
function whose bytes already match the original (an export, or an entry already recorded), so an entry records where
the original's own code points; the targets of a recorded entry are searched in turn. A function entry must start a
.pdata function. A data entry takes the size of the candidate's definition. Sites that disagree, names that no
matching site references, and existing entries that would change fail without writing.

The manifest holds names, RVAs and sizes only, never bytes of the original (D19). Review the written diff before
committing it: native_bytes.py compares every entry the candidate objects define on each run without --symbol.
"""
import argparse
import json
import sys
from pathlib import Path

from native_bytes import (DEFAULT_TARGETS, PeImage, _addend, _resolve, compare_code, definitions_in_objects,
                          targets_from)


def record(image, definitions, names, doc):
    """Record `names` for the original `image` in the manifest document `doc` (changed in place, and returned).

    `definitions` are the candidates' definitions by name. Raises ValueError when the names cannot be recorded.
    """
    entries = targets_from(doc, image)

    def placed(d, rva):
        if d.data:
            return rva, image.read(rva, len(d.code))
        end = image.functions.get(rva)
        return (rva, image.read(rva, end - rva)) if end is not None else None

    queue = []
    for d in definitions.values():
        if not d.data and d.name in image.exports:
            queue.append((d, image.exports[d.name], image.export_code(d.name)))
        elif d.name in entries:
            at = placed(d, entries[d.name].rva)
            if at:
                queue.append((d, *at))
    wanted, found, searched = set(names), {}, set()
    while queue:
        d, begin, code = queue.pop()
        if d.name in searched or not compare_code(d.name, code, d.relocs, d.code, d.relocs).ok:
            continue
        searched.add(d.name)
        for r in d.relocs:
            name = d.targets.get(r.offset) or d.statics.get(r.offset)
            if name not in wanted:
                continue
            rva = _resolve(image, begin + r.offset, r.kind) - _addend(d.code, r)
            if found.setdefault(name, rva) != rva:
                raise ValueError(f"{name}: sites disagree, RVA {found[name]:#x} and {rva:#x} (from {d.name})")
            callee = definitions.get(name)
            at = placed(callee, rva) if callee is not None else None
            if at:
                queue.append((callee, *at))

    missing = sorted(wanted - set(found))
    if missing:
        raise ValueError("not referenced by any candidate function that matches the original: " + ", ".join(missing))
    bound = next((doc[k] for k in doc if k.lower() == image.name.lower()), None)
    if bound is None:
        bound = doc[image.name] = {"sha256": image.sha256, "targets": {}}
    for name in sorted(found):
        defn = definitions.get(name)
        if defn is None:
            raise ValueError(f"{name}: not defined by the candidate objects, so its kind and size are unknown")
        entry = {"rva": f"{found[name]:#x}"}
        if defn.data:
            entry["size"] = len(defn.code)
        elif found[name] not in image.functions:
            raise ValueError(f"{name}: RVA {found[name]:#x} does not start a .pdata function")
        old = bound["targets"].get(name)
        if old is not None and old != entry:
            raise ValueError(f"{name}: manifest has {old}, original gives {entry}; remove the entry to re-record it")
        bound["targets"][name] = entry
    bound["targets"] = dict(sorted(bound["targets"].items()))
    targets_from(doc, image)  # never write a manifest that native_bytes.py would reject
    return doc


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--original", type=Path, required=True, help="original DLL (read only)")
    parser.add_argument("--candidate", type=Path, action="append", required=True,
                        help="COFF object compiled with the original's flags (repeatable)")
    parser.add_argument("--name", action="append", required=True, help="reviewed decorated target name (repeatable)")
    parser.add_argument("--targets", type=Path, default=DEFAULT_TARGETS, help="manifest to update")
    args = parser.parse_args(argv)
    image = PeImage(args.original)
    try:
        definitions = definitions_in_objects(args.candidate)
        doc = json.loads(args.targets.read_text(encoding="utf-8")) if args.targets.is_file() else {}
        record(image, definitions, args.name, doc)
    except ValueError as e:  # includes ManifestError and malformed JSON
        print(f"FAIL {e}")
        return 1
    args.targets.write_text(json.dumps(doc, indent=2) + "\n", encoding="utf-8")
    bound = next(doc[k] for k in doc if k.lower() == image.name.lower())
    for name in sorted(set(args.name)):
        print(f"recorded {name}: {bound['targets'][name]}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
