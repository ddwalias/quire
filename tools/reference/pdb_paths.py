"""List the PDB (build) path each Mathcad binary embeds in its debug directory: the original source layout.

  python tools/reference/pdb_paths.py [--install DIR] [--markdown]

Reads only the CodeView debug-directory entry of each PE file (never IL or code); prints name, kind and path.
"""
import argparse
import os
from pathlib import Path
import struct
import sys


def pdb_path(path):
    data = Path(path).read_bytes()
    if data[:2] != b'MZ':
        return None
    pe = struct.unpack_from('<I', data, 0x3C)[0]
    if data[pe:pe + 4] != b'PE\0\0':
        return None
    sections_n, = struct.unpack_from('<H', data, pe + 6)
    opt_size, = struct.unpack_from('<H', data, pe + 20)
    opt = pe + 24
    magic, = struct.unpack_from('<H', data, opt)
    dd = opt + (112 if magic == 0x20B else 96)
    rva, size = struct.unpack_from('<II', data, dd + 6 * 8)
    if not rva:
        return None
    sections = [struct.unpack_from('<4xIII', data, opt + opt_size + i * 40 + 8 - 8) for i in range(sections_n)]
    sections = [(struct.unpack_from('<I', data, opt + opt_size + i * 40 + 12)[0],
                 struct.unpack_from('<I', data, opt + opt_size + i * 40 + 8)[0],
                 struct.unpack_from('<I', data, opt + opt_size + i * 40 + 20)[0]) for i in range(sections_n)]

    def offset(r):
        for va, vsize, raw in sections:
            if va <= r < va + max(vsize, 1):
                return r - va + raw
        return None
    start = offset(rva)
    if start is None:
        return None
    for i in range(size // 28):
        _, _, _, _, kind, dsize, _, raw = struct.unpack_from('<IIHHIIII', data, start + i * 28)
        if kind == 2 and data[raw:raw + 4] == b'RSDS':
            return data[raw + 24:raw + dsize].split(b'\0')[0].decode('utf-8', 'replace')
    return None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--install', default=os.environ.get('MATHCAD_DIR', r'C:\Program Files\PTC\Mathcad Prime 12.0.1.0'))
    args = parser.parse_args()
    rows = []
    for f in sorted(Path(args.install).rglob('*')):
        if f.suffix.lower() in ('.dll', '.exe') and f.is_file():
            try:
                p = pdb_path(f)
            except (struct.error, OSError, IndexError):
                p = None
            rows.append((str(f.relative_to(args.install)), p))
    for name, p in rows:
        print(f'{name}\t{p or ""}')


if __name__ == '__main__':
    sys.exit(main())
