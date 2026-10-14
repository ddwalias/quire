"""Line and branch coverage of each compared method, from the differential run's cobertura reports."""
from pathlib import Path
import re
import xml.etree.ElementTree as ET

CONDITIONS = re.compile(r'\((\d+)/(\d+)\)')


def read_cobertura(paths):
    """{(file, line): (hits, covered conditions, total conditions)}. Duplicate entries keep the most executions and
    the least covered branch ratio, so one fully covered entry cannot hide another's partial branch."""
    lines = {}
    for path in paths:
        root = ET.parse(path).getroot()
        for cls in root.iter('class'):
            name = Path(cls.get('filename', '')).as_posix()
            for line in cls.iter('line'):
                key = (name, int(line.get('number')))
                hits = int(line.get('hits', '0'))
                match = CONDITIONS.search(line.get('condition-coverage', ''))
                covered, total = (int(match[1]), int(match[2])) if match else (0, 0)
                old = lines.get(key)
                if old is None:
                    lines[key] = (hits, covered, total)
                    continue
                worst = min((old[1], old[2]), (covered, total), key=lambda c: (c[0] / c[1]) if c[1] else 1.0)
                lines[key] = (max(old[0], hits), *worst)
    if not lines:
        raise ValueError('Coverage reports contain no lines; was the collector enabled?')
    return lines


def gaps(source, lines):
    """Uncovered statement lines and partially covered branch lines of one method, per source document; None when
    the PDB had no source."""
    if not source:
        return None
    uncovered, partial = [], []
    for document in source:
        name = Path(document['file']).as_posix()
        for number in document['lines']:
            hits, covered, total = lines.get((name, number), (0, 0, 0))
            if not hits:
                uncovered.append(f'{name}:{number}')
            elif covered < total:
                partial.append(f'{name}:{number} ({covered}/{total})')
    return {'uncovered_lines': uncovered, 'partial_branches': partial}
