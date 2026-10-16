"""Tier lists for the tiered mirror scope (D11): which Mathcad members are Tier A, A2, B, C, D or abstract.

    tiers.py keys INVENTORY.json KEYS.tsv   reduce one MethodAudit inventory to the columns the mapping needs
    tiers.py classify DATA KEYS OUT         classify, map to Quire keys and write the tier lists and types.tsv

DATA is the original-IL scan (methods.tsv, edges.tsv, hier_*.tsv, types.tsv, assemblies.tsv); KEYS is a directory of
reduced inventories, one per project. Standard library only; docs/mirror/tiers/README.md explains the method."""
import argparse, collections, glob, json, os, re, sys

TIERS = ['A', 'A2', 'B', 'C', 'D', 'abstract']
MECH = {'auto-accessor', 'field-accessor', 'trivial', 'const', 'empty', 'ctor-base', 'forwarder', 'throw',
        'ctor-simple'}
NAT = {'native-mixed', 'native-thunk', 'pinvoke', 'extern', 'nativebody'}
# IL-only Phase 10 assemblies the MPL compiler produced (Tier C, not native).
MPL = ('mpl', 'mcd', 'efisymbolics', 'hashtable', 'efiaffine', 'mcmisc', 'utilsmgd') + tuple(
    f'efi{x}.' for x in ('bessel calculus core data debug diffeq finance fitting fourier graphics image io matrix plot '
                         'signal sixsigma solver special statistics wavelets').split())
PRIM = ('Boolean Char SByte Byte Int16 UInt16 Int32 UInt32 Int64 UInt64 Single Double String Object Void IntPtr '
        'UIntPtr TypedReference').split()


def read(path):
    with open(path, encoding='utf-8', newline='\n') as f:
        return f.read()


def write(path, text):
    with open(path, 'w', encoding='utf-8', newline='\n') as f:
        f.write(text)


def tsv(path):
    return [line.split('\t') for line in read(path).split('\n')[:-1]]


def keys(inventory, out):
    """Reduce a MethodAudit inventory to type, signature, exclusion, original type and original signature."""
    lines = []
    for r in json.loads(read(inventory))['methods']:
        o = r['original'] or {}
        lines.append('\t'.join([r['type'], r['signature'], r['exclusion'] or '', o.get('type', ''),
                                o.get('signature', '')]) + '\n')
    write(out, ''.join(lines))


def accessor_of(name):
    """The property or event an accessor belongs to, explicit interface implementations included."""
    if '.' in name and any(x in name for x in ('.get_', '.set_', '.add_', '.remove_')):
        return name.rsplit('.', 1)[0] + '.' + name.rsplit('.', 1)[1].split('_', 1)[1]
    return name.split('_', 1)[1] if '_' in name else name


def body_class(classes):
    """The body tier of a member from its own class (methods) or its accessors' classes (properties, events)."""
    if not classes:
        return 'mech'
    if classes <= {'abstract'}:
        return 'abstract'
    if classes & NAT:
        return 'native'
    if classes <= MECH | {'abstract'}:
        return 'mech'
    return 'body'


def dead(x):
    """Areas the product never ships: Phases 8/9, Whiteboard/Mathboard, OLE, Plot3D and licensing gating."""
    t = x['type']
    if x['phase'] in ('8', '9') or x['asm'] == 'Ptc.Controls.Whiteboard.dll' or '.Ole.' in t or '.Plot3D' in t:
        return True
    return t.startswith(('Ptc.FunctionalitiesLimitation', 'Ptc.Controls.Core.Ole', 'Ptc.Controls.Core.Whiteboard',
                         'Ptc.Controls.Mathboard'))


def split_params(s):
    out, depth, cur = [], 0, ''
    for ch in s:
        depth += ch in '<('
        depth -= ch in '>)'
        if ch == ',' and depth == 0:
            out.append(cur)
            cur = ''
        else:
            cur += ch
    return out + [cur] if cur else out


def parse(sig):
    """Name without generic arity, and the parameter list, of a MethodAudit signature."""
    i, j = sig.index('('), sig.rindex(')->')
    return sig[:i].rsplit('`', 1)[0], sig[i + 1:j]


def lane(a, t, c, dominant):
    def under(*names):
        return any(t == p or t.startswith(p + '.') or t.startswith(p + '+') for p in names)
    if a == 'mplrun.dll' and t.startswith(('Mcd.MatVal', 'Mcd.MatDim', 'Mcd.mat_val', 'Mpl.Run.Function')):
        return '2'
    if a in ('mplrun.dll', 'iengine.dll'):
        return '1'
    if t.startswith('Ptc.Calculation') or a == 'automationmgd.dll' and c['A']:
        return '3'
    if a == 'Ptc.Core.dll' and t.startswith('Ptc.PersistentData'):
        return '4'
    if a == 'Ptc.Core.dll' and (under('Ptc.Serialization', 'Ptc.Xml', 'Ptc.IO', 'Ptc.Wpf') or c['A']):
        return '5'
    if under('Ptc.Controls.Core.Serialization', 'Ptc.Controls.IO') or t.startswith('Ptc.Controls') and c['A']:
        return '6'
    if a == 'Ptc.EquationEditor.dll':  # the assembly: its obfuscated types have no namespace
        return '7'
    return '8' if dominant == 'B' else ''


def classify(D, KEYS, OUT):
    """Tier every member, map Tier A/A2 to Quire keys, and write the lists, the unmapped names and types.tsv."""
    header, *rows = tsv(D + '/methods.tsv')
    rows = [dict(zip(header, r)) for r in rows]
    asmkind = dict(tsv(D + '/assemblies.tsv'))
    types = tsv(D + '/types.tsv')
    bases_of = {(p[0], p[1]): p[8] for p in types}

    # Members: methods and constructors by their own body; properties and events by their accessors' bodies.
    accessors = collections.defaultdict(list)
    for r in rows:
        if r['kind'] in ('A', 'EA'):
            accessors[(r['asm'], r['type'], accessor_of(r['member'].split('(')[0]))].append(r)
    m = []
    for r in rows:
        if r['kind'] == 'F':
            m.append(dict(r, tier='mech-field'))
        elif r['kind'] in ('P', 'E'):
            m.append(dict(r, tier=body_class({x['cls'] for x in accessors[(r['asm'], r['type'], r['member'])]})))
        elif r['kind'] in ('M', 'C'):
            m.append(dict(r, tier=body_class({r['cls']})))

    # Call graph closure with rapid type analysis: a virtual call reaches an override only in an instantiated type.
    methods_by_type = collections.defaultdict(list)
    by_namesig = collections.defaultdict(list)
    for r in rows:
        if r['kind'] in ('M', 'C', 'A', 'EA'):
            n = f"{r['asm']}|{r['type']}::{r['member']}"
            methods_by_type[f"{r['asm']}|{r['type']}"].append(n)
            by_namesig[r['member']].append(n)
    all_nodes = {n for v in methods_by_type.values() for n in v}
    virtuals = {f"{r['asm']}|{r['type']}::{r['member']}" for r in rows
                if r['kind'] in ('M', 'A', 'EA') and 'virtual' in r['vis']}
    edges = collections.defaultdict(list)
    for a, b in tsv(D + '/edges.tsv'):
        edges[a].append(b)
    bases = collections.defaultdict(list)
    for f in glob.glob(D + '/hier_*.tsv'):
        for a, b in tsv(f):
            bases[a].append(b)
    anc = {}

    def ancestors(t):
        if t not in anc:
            res, stack = set(), [t]
            while stack:
                for b in bases.get(stack.pop(), []):
                    if b not in res:
                        res.add(b)
                        stack.append(b)
            anc[t] = res
        return anc[t]

    def tname(n):
        return n.split('::')[0]

    def closure(roots):
        seen, stack, inst = set(), [], set()
        pending = collections.defaultdict(set)  # name+signature -> declaring types of the virtual calls seen

        def add(n):
            if n in all_nodes and n not in seen:
                seen.add(n)
                stack.append(n)

        def instantiate(t):
            if t in inst or t not in methods_by_type:
                return
            inst.add(t)
            an = ancestors(t) | {t}
            for x in methods_by_type[t]:
                ms = x.split('::', 1)[1]
                # An override of a framework virtual is reachable once its type is instantiated.
                if x in virtuals and (any(d in an for d in pending.get(ms, ()))
                                      or any(b not in methods_by_type for b in an if b != t)):
                    add(x)
                if '.cctor(' in ms:
                    add(x)
        for r in roots:
            add(r)
        while stack:
            n = stack.pop()
            if '::.ctor(' in n:
                instantiate(tname(n))
            for x in methods_by_type[tname(n)]:
                if '::.cctor(' in x:
                    add(x)
            for e in edges.get(n, []):
                virt = e.startswith('v:')
                if virt:
                    e = e[2:]
                if e in all_nodes:
                    add(e)
                    if '::.ctor(' in e:
                        instantiate(tname(e))
                if not virt:
                    continue
                dt, ms = e.split('::', 1)
                if dt in methods_by_type and dt not in pending[ms]:
                    pending[ms].add(dt)
                    for c in by_namesig.get(ms, []):
                        if tname(c) in inst and dt in ancestors(tname(c)):
                            add(c)
        return seen

    def reached_members(reached):
        names = collections.defaultdict(set)
        for n in reached:
            t, ms = n.split('::', 1)
            names[t].add(ms.split('(')[0])
        out = set()
        for i, x in enumerate(m):
            key = f"{x['asm']}|{x['type']}"
            if key not in names or x['kind'] in ('M', 'C') and f"{key}::{x['member']}" not in reached:
                continue
            if x['kind'] in ('P', 'E') and not any(q.endswith('_' + x['member']) for q in names[key]):
                continue
            out.add(i)
        return out

    def ns(t):
        top = t.split('+')[0]
        return top.rsplit('.', 1)[0] if '.' in top else ''

    def roots(pred):
        return [n for t, v in methods_by_type.items() for n in v if pred(*t.split('|'))]

    def core_root(a, t):
        return (a in ('iengine.dll', 'Ptc.Calculation.dll', 'Ptc.Calculation.Core.dll', 'mplrun.dll')
                or a == 'Ptc.Core.dll' and ns(t) in ('Ptc.PersistentData', 'Ptc.PersistentDataObjects', 'Ptc.Xml',
                                                     'Ptc.IO')
                or a == 'Ptc.Controls.Core.dll' and ns(t).startswith('Ptc.Controls.Core.Serialization')
                or a == 'Ptc.Controls.dll' and ns(t).startswith('Ptc.Controls.IO'))

    def tree_root(a, t):
        return a == 'Ptc.EquationEditor.dll' and ns(t).startswith(('Ptc.EquationEditor.Tree',
                                                                   'Ptc.EquationEditor.Equation'))
    # A = engine + calculation + mplrun + document I/O; A2 = the equation tree, less A.
    core = reached_members(closure(roots(core_root)))
    tree = reached_members(closure(roots(tree_root))) - core

    def tier(i, x):
        """None for members the scan leaves out (enum values, compiler-generated)."""
        if x['kind'] == 'F' and 'System.Enum' in bases_of.get((x['asm'], x['type']), ''):
            return None
        if '<' in x['member'].split('(')[0]:
            return None
        a, tr = x['asm'], x['tier']
        if tr == 'abstract':
            return 'abstract'
        if x['phase'] == '10' and asmkind[a] == 'mixed':
            return 'D'
        if x['phase'] == '10' and a.startswith(MPL) and asmkind.get(a) == 'il' and a != 'mplrun.dll':
            return 'C'
        if tr in ('mech', 'mech-field'):
            return 'D' if dead(x) and i not in core else 'B'
        if i in core:
            return 'A'
        if i in tree:
            return 'A2'
        return 'D' if dead(x) else 'C'
    tiers = [tier(i, x) for i, x in enumerate(m)]
    total = collections.Counter(tiers)
    print('original:', ', '.join(f'{t} {total[t]}' for t in TIERS))
    for t in ('A', 'A2'):
        phases = collections.Counter(x['phase'] for x, k in zip(m, tiers) if k == t)
        print(f'  {t} by phase:', dict(sorted(phases.items(), key=lambda p: int(p[0]))))

    # Quire keys: same owner, name and parameter types, with assembly scopes stripped.
    q2o = {p[2]: p[1] for p in types}

    def norm(t, quire=False):
        t = re.sub(r'\[[A-Za-z_][^\]]*\]', '', t)  # [scope] prefixes, not array ranks
        while '<' in t:
            t = re.sub(r'<[^<>]*>', '', t)
        head = re.match(r'[^&*\[]*', t).group(0)
        base = q2o.get(head, head) if quire else head  # Quire names to Mathcad's
        if base.startswith('System.') and base[7:] in PRIM:
            base = base[7:]
        return base + t[len(head):]
    quire = collections.defaultdict(lambda: collections.defaultdict(list))  # owner -> name -> [(key, params)]
    for f in sorted(glob.glob(KEYS + '/*.tsv')):
        for qt, qs, excl, ot, osig in tsv(f):
            if excl:
                continue
            name, ps = parse(qs)
            if '.' in name.lstrip('.'):  # explicit interface implementation: name the interface as Mathcad does
                iface, method = name.removeprefix('global::').rsplit('.', 1)
                name = q2o.get(iface, iface) + '.' + method
            if osig:
                params = [norm(p) for p in split_params(parse(osig)[1])]
            else:
                params = [norm(p, True) for p in split_params(ps)]
            quire[qt][name].append((f'{qt}::{qs}', params))
    same = collections.Counter((r['asm'], r['type'], r['member']) for r in rows)

    def keys_of(r):
        """The scan omits generic arity and return type, so overloads that differ only there map as a group."""
        name, ps = r['member'].split('(')[0], r['member'][r['member'].index('(') + 1:-1]
        params = [norm(p) for p in split_params(ps)]
        c = [x for x in quire.get(r['quire'], {}).get(name, []) if x[1] == params]
        return [x[0] for x in c] if len(c) == same[(r['asm'], r['type'], r['member'])] else []
    os.makedirs(OUT, exist_ok=True)
    listed = {}
    for t in ('A', 'A2', 'B'):
        found, missing = set(), set()
        for x, k in zip(m, tiers):
            if k != t or x['kind'] == 'F':  # MethodAudit keys methods only; Tier B fields are not listed
                continue
            rs = accessors[(x['asm'], x['type'], x['member'])] if x['kind'] in ('P', 'E') else [x]
            got = [keys_of(r) for r in rs]
            found.update(key for g in got for key in g)
            if not rs or [] in got:
                missing.add(f"{x['asm']}\t{x['type']}::{x['member']}")
        listed[t] = found, missing
    if listed['B'][0] & (listed['A'][0] | listed['A2'][0]):
        sys.exit('a Quire key is in Tier B and in Tier A/A2')  # tier-b.txt is the generator's allowlist
    for t, (found, missing) in listed.items():
        write(f'{OUT}/tier-{t.lower()}.txt', ''.join(k + '\n' for k in sorted(found)))
        write(f'{OUT}/tier-{t.lower()}-unmapped.txt', ''.join(k + '\n' for k in sorted(missing)))
        print(f'mapped {t}: {len(found)} Quire keys; {len(missing)} members not (fully) mapped')

    # Types: tier counts, dominant tier (ties in TIERS order) and the lane that owns the type.
    counts = collections.defaultdict(collections.Counter)
    for x, k in zip(m, tiers):
        if k:
            counts[(x['asm'], x['type'], x['quire'])][k] += 1
    lines = ['assembly\toriginal type\tQuire type\t' + '\t'.join(TIERS) + '\tdominant\tlane\n']
    lanes = collections.Counter()
    for (a, t, q), c in sorted(counts.items()):
        dominant = max(TIERS, key=lambda k: (c[k], -TIERS.index(k)))
        ln = lane(a, t, c, dominant)
        lanes[ln] += 1
        lines.append('\t'.join([a, t, q] + [str(c[k]) for k in TIERS] + [dominant, ln]) + '\n')
    write(f'{OUT}/types.tsv', ''.join(lines))
    print('types per lane:', dict(sorted(lanes.items())))


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    commands = parser.add_subparsers(dest='command', required=True)
    k = commands.add_parser('keys', help=keys.__doc__)
    k.add_argument('inventory')
    k.add_argument('out')
    c = commands.add_parser('classify', help=classify.__doc__)
    c.add_argument('data')
    c.add_argument('keys')
    c.add_argument('out')
    args = parser.parse_args()
    if args.command == 'keys':
        keys(args.inventory, args.out)
    else:
        classify(args.data, args.keys, args.out)
