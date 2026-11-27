"""Resolve project source directories from docs/mirror/source-layout.json."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def _layouts(root):
    path = Path(root) / 'docs' / 'mirror' / 'source-layout.json'
    return json.loads(path.read_text(encoding='utf-8')) if path.is_file() else {}


def project_dir(root, name):
    """Project directory; projects without an override remain at src/<name>."""
    root = Path(root)
    relative = _layouts(root).get(name, name)
    directory = (root / 'src' / relative).resolve()
    src = (root / 'src').resolve()
    if directory != src and src not in directory.parents:
        raise ValueError(f'{name}: source layout must stay under src/: {relative!r}')
    return directory


def projects(root, ext):
    """Project names whose named project files exist at any depth below src, excluding build output."""
    root = Path(root)
    suffix = ext if ext.startswith('.') else '.' + ext
    return {p.stem for p in (root / 'src').rglob(f'*{suffix}')
            if not {'bin', 'obj'}.intersection(p.relative_to(root / 'src').parts)}


def project_file(root, name, ext):
    """Named project file within its resolved directory, preserving nested mixed-mode vcxproj layouts."""
    directory = project_dir(root, name)
    suffix = ext if ext.startswith('.') else '.' + ext
    expected = directory / f'{name}{suffix}'
    if expected.is_file():
        return expected
    if not directory.is_dir():
        return expected
    return next((path for path in directory.rglob(f'{name}{suffix}')
                 if not {'bin', 'obj'}.intersection(path.relative_to(directory).parts)
                 and project_of(path, root) == name), expected)


def project_of(path, root=ROOT):
    """Map a repository-relative path to its owning project using the longest configured project directory."""
    root = Path(root)
    path = Path(path)
    if path.is_absolute():
        try:
            path = path.resolve().relative_to(root.resolve())
        except ValueError:
            return None
    parts = path.as_posix().split('/')
    if not parts or parts[0] != 'src':
        return None
    candidates = []
    for name in _layouts(root):
        directory = project_dir(root, name).relative_to((root / 'src').resolve()).as_posix()
        prefix = f'src/{directory}/'
        if path.as_posix().startswith(prefix):
            candidates.append((len(prefix), name))
    if candidates:
        return max(candidates)[1]
    # Legacy flat layout: owning project is the first segment after src, unless it is a configured parent.
    if len(parts) >= 3:
        default = parts[1]
        project_file = len(parts) == 3 and Path(parts[-1]).stem == default \
            and Path(parts[-1]).suffix.lower() in ('.csproj', '.vcxproj')
        if default not in _layouts(root) and (default_projects(root, default) or project_file):
            return default
    return None


def default_projects(root, name):
    directory = Path(root) / 'src' / name
    return any((directory / f'{name}{extension}').is_file() for extension in ('.csproj', '.vcxproj'))
