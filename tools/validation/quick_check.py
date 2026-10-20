"""Compatibility command for a Release IL check of Quire.Mpl."""
import sys
from release_il import main

if __name__ == '__main__':
    args = ['--type' if a == '--method-type' else a for a in sys.argv[1:]]
    if '--project' not in args:
        args += ['--project', 'Quire.Mpl']
    sys.exit(main(args))
