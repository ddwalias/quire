"""Line and branch coverage of each compared method, from the differential run's cobertura reports."""
from pathlib import Path
import re
import xml.etree.ElementTree as ET

CONDITIONS = re.compile(r'\((\d+)/(\d+)\)')


def read_cobertura(paths):
    """{(file, line): (hits, covered conditions, total conditions)}, merged over reports and duplicate entries."""
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
                old = lines.get(key, (0, 0, 0))
                lines[key] = (max(old[0], hits), max(old[1], covered), max(old[2], total))
    if not lines:
        raise ValueError('Coverage reports contain no lines; was the collector enabled?')
    return lines


def gaps(source, lines):
    """Uncovered statement lines and partially covered branch lines of one method; None when the PDB had no source."""
    if not source:
        return None
    name = Path(source['file']).as_posix()
    uncovered, partial = [], []
    for number in source['lines']:
        hits, covered, total = lines.get((name, number), (0, 0, 0))
        if not hits:
            uncovered.append(number)
        elif covered < total:
            partial.append(f'{number} ({covered}/{total})')
    return {'file': name, 'uncovered_lines': uncovered, 'partial_branches': partial}
