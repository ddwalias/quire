"""Search source shapes for a byte match of native exports in a linked DLL (D30), as decomp-permuter does.

    python tools/validation/native_permute.py --project Quire.Native.EfistatsciImpl --source mutils_mat.cpp \\
        --function matdbl_sum --inline validate --symbol matdbl_sum --minutes 20

Write the export first, in the shape you would land, so that the unchanged project builds. Then name the bodies to
search with --function (`name`, or `file:name` relative to the project; repeatable; the first definition of each name
is used) and the exports to match with --symbol. A shape often lives in a shared helper whose other users already
match: --inline NAME expands calls to NAME inside the searched bodies first, so the search varies a private copy.

Each worker copies the project directory into a scratch directory, so the working tree is never written. An attempt
applies one to three random rewrites to the searched bodies (operand order, comparisons, statement order and
placement, local types, temporaries, index forms, if splitting, compound assignment, loop forms, and working on a
store target instead of a local), rebuilds the linked DLL with the pinned MSVC and scores it: an attempt that breaks an
export that matched at the start is never kept; then more matching symbols win, then mean similarity
(native_linked.Comparison). A better score replaces the current source, an equal one does with a small probability,
so the search drifts across plateaus. A rewrite need not keep the code's meaning: only a byte match counts, and equal
machine code proves equal behavior.

After a match, the change is reduced to the hunks the match needs, temporaries the rewrites introduced are folded back
where the match allows, and the result is checked strictly against every export. The output directory then holds
`match.patch` (apply with `git apply` from the repository root, then run native_gate.py as for any change) and a
report of anything still unverified. Every intermediate improvement is kept there as a diff. Nothing of the original
DLL is written (D15). Ctrl+C stops after the builds in progress and still writes the best source.
"""
import argparse
import difflib
import os
import random
import re
import shutil
import subprocess
import sys
import tempfile
import threading
import time
from pathlib import Path

import native_linked
from native_bytes import DEFAULT_TARGETS, load_targets
from native_gate import original_dll
from release_il import ROOT, mathcad_install, msbuild, msvc_mismatch

OPERAND = r"(?:\((?:double|float|int|long|unsigned|short|char)\)\s*)?(?:[A-Za-z_]\w*(?:(?:->|\.)\w+)*(?:\[[^\[\]\n]+\])?|\d+(?:\.\d+)?)"
# An operand neither continues a longer expression (a member, index, call or unary prefix) nor is followed by one.
BEFORE, AFTER = r"(?<![\w>\]).*&!~-])", r"(?![\w(\[.]|->)"
COMMUTATIVE = re.compile(rf"{BEFORE}({OPERAND})(\s+)(\*|\+|==|!=|&|\||\^)(\s+)({OPERAND}){AFTER}")
COMPARISON = re.compile(rf"{BEFORE}({OPERAND})\s+(<=|>=|<|>)\s+({OPERAND}){AFTER}")
FLIPPED = {"<": ">", ">": "<", "<=": ">=", ">=": "<="}
LOCAL_TYPES = ["int", "long", "unsigned int", "__int64", "unsigned __int64"]
DECLARED_TYPE = re.compile(r"(?<!unsigned )(?<!long )(?<!short )\b(unsigned __int64|unsigned int|__int64|int|long)(\s+)(\w+)(\s*[=;])")
MEMBER = re.compile(r"\b([A-Za-z_]\w*)->(\w+)\b(?!\s*(?:=[^=]|\+=|-=|\+\+|--))")
DECLARATION = re.compile(r"^([ \t]*)(?:const\s+)?[A-Za-z_][\w:<>]*(?:\s+[\w:<>]+)*[\s*]+(\w+)\s*=\s*([^;{}]+);[ \t]*$",
                         re.M)
SIMPLE = re.compile(r"^[ \t]+(?!return\b|if\b|for\b|while\b|else\b|do\b|switch\b|case\b|break\b|[{}])\S.*;[ \t]*$")
CONTROL = ("if", "for", "while", "else", "return", "do", "switch")
LITERAL_OR_COMMENT = re.compile(r'"(?:\\.|[^"\\\n])*"|\'(?:\\.|[^\'\\\n])*\'|//[^\n]*|/\*.*?\*/', re.S)


def body_span(text, function):
    """(start, end) offsets of the body, inside its braces, of the first definition of `function` in `text`. Braces
    and names inside string or character literals and comments do not count."""
    text = LITERAL_OR_COMMENT.sub(lambda m: " " * len(m[0]), text)
    for m in re.finditer(rf"\b{re.escape(function)}\s*\(", text):
        depth, k = 0, m.end() - 1
        while k < len(text):
            depth += {"(": 1, ")": -1}.get(text[k], 0)
            if depth == 0:
                break
            k += 1
        brace = re.match(r"\s*(?:const\s*)?(?:noexcept\s*(?:\([^()]*\)\s*)?)?(?:(?:override|final)\s*)*"
                         r"(?:->\s*[^{;()]+?\s*)?\{", text[k + 1:])
        if not brace:
            continue  # a declaration or a call
        start = j = k + 1 + brace.end()
        depth = 1
        while depth:
            if j >= len(text):
                raise ValueError(f"{function}: unbalanced braces")
            depth += {"{": 1, "}": -1}.get(text[j], 0)
            j += 1
        return start, j - 1
    raise ValueError(f"no definition of {function} found")


def pick(rng, matches):
    matches = list(matches)
    return rng.choice(matches) if matches else None


def replace_match(text, m, new):
    return text[:m.start()] + new + text[m.end():]


def swap_operands(body, rng):
    m = pick(rng, COMMUTATIVE.finditer(body))
    return m and replace_match(body, m, f"{m[5]}{m[2]}{m[3]}{m[4]}{m[1]}")


def flip_comparison(body, rng):
    m = pick(rng, COMPARISON.finditer(body))
    return m and replace_match(body, m, f"{m[3]} {FLIPPED[m[2]]} {m[1]}")


def swap_statements(body, rng):
    lines = body.split("\n")
    pairs = [i for i in range(len(lines) - 1) if SIMPLE.match(lines[i]) and SIMPLE.match(lines[i + 1])
             and len(lines[i]) - len(lines[i].lstrip()) == len(lines[i + 1]) - len(lines[i + 1].lstrip())]
    if not pairs:
        return None
    i = rng.choice(pairs)
    lines[i], lines[i + 1] = lines[i + 1], lines[i]
    return "\n".join(lines)


def retype_local(body, rng):
    m = pick(rng, DECLARED_TYPE.finditer(body))
    if not m:
        return None
    return replace_match(body, m, f"{rng.choice([t for t in LOCAL_TYPES if t != m[1]])}{m[2]}{m[3]}{m[4]}")


def hoist_member(body, rng):
    """Read `p->field` into a new local just before the line that uses it."""
    m = pick(rng, MEMBER.finditer(body))
    if not m:
        return None
    start = body.rfind("\n", 0, m.start()) + 1
    end = body.find("\n", m.end())
    end = len(body) if end < 0 else end
    line = body[start:end]
    if line.lstrip().startswith(("}", "else")):
        return None
    name = next(f"t{k}" for k in range(1000) if not re.search(rf"\bt{k}\b", body))
    indent = line[:len(line) - len(line.lstrip())]
    line = re.sub(rf"\b{re.escape(m[0])}\b", name, line)
    return f"{body[:start]}{indent}auto {name} = {m[0]};\n{line}{body[end:]}"


def inline_local(body, rng):
    """Remove a local that is assigned once and put its initializer at each use."""
    candidates = []
    for m in DECLARATION.finditer(body):
        name, rest = m[2], body[m.end():]
        if not re.search(rf"\b{name}\b", rest) or re.search(rf"\b{name}\s*(?:[-+*/|&^]?=[^=]|\+\+|--)|(?:\+\+|--){name}\b", rest):
            continue
        candidates.append(m)
    m = pick(rng, candidates)
    if not m:
        return None
    rest = re.sub(rf"\b{m[2]}\b", lambda _: grouped(m[3].strip()), body[m.end():])
    return body[:m.start()].rstrip(" \t") + rest.lstrip("\n")


def index_form(body, rng):
    found = [(m, f"*({m[1]} + {m[2]})") for m in re.finditer(r"(?<![>.])\b(\w+)\[(\w+)\]", body)]
    found += [(m, f"{m[1]}[{m[2]}]") for m in re.finditer(r"\*\((\w+) \+ (\w+)\)", body)]
    if not found:
        return None
    m, new = rng.choice(found)
    return replace_match(body, m, new)


def split_condition(body, rng):
    """`if (a || b) s;` becomes two ifs, and two ifs with the same statement merge back."""
    found = [(m, f"{m[1]}if ({m[2]}) {m[4]}\n{m[1]}if ({m[3]}) {m[4]}")
             for m in re.finditer(r"^([ \t]*)if \(([^()\n|]+(?:\([^()\n]*\))?[^()\n|]*) \|\| ([^\n|]+)\) ([^\n{]+;)[ \t]*$",
                                  body, re.M)]
    for m in re.finditer(r"^([ \t]*)if \(([^\n]+)\) ([^\n{]+;)\n\1if \(([^\n]+)\) \3[ \t]*$", body, re.M):
        found.append((m, f"{m[1]}if ({m[2]} || {m[4]}) {m[3]}"))
    if not found:
        return None
    m, new = rng.choice(found)
    return replace_match(body, m, new)


def grouped(expression):
    return expression if re.fullmatch(r"[\w.\[\]]+|\w+->\w+", expression.strip()) else f"({expression})"


def compound_assignment(body, rng):
    found = [(m, f"{m[1]}{m[2]} = {m[2]} {m[3]} {grouped(m[4])};") for m in
             re.finditer(r"^([ \t]*)(\*?[\w\->\[\]]+) ([-+*|&^])= ([^;\n]+);", body, re.M)]
    found += [(m, f"{m[1]}{m[2]} {m[3]}= {m[4]};") for m in
              re.finditer(r"^([ \t]*)(\*?[\w\->\[\]]+) = \2 ([-+*|&^]) ([^;\n]+);", body, re.M)]
    if not found:
        return None
    m, new = rng.choice(found)
    return replace_match(body, m, new)


def toggle_const(body, rng):
    found = [(m, f"{m[1]}{m[2]}") for m in re.finditer(r"^([ \t]+)const (\w)", body, re.M)]
    for m in DECLARATION.finditer(body):
        line = m[0]
        if not line.lstrip().startswith(("const ", *CONTROL)):
            found.append((m, f"{m[1]}const {line[len(m[1]):]}"))
    if not found:
        return None
    m, new = rng.choice(found)
    return replace_match(body, m, new)


STORE = re.compile(r"^[ \t]*(\*\w+|\w+->\w+|\w+\[\w+\]) = (\w+);[ \t]*\n?", re.M)
FOR = re.compile(r"^([ \t]*)for \(([^;\n]*); ([^;\n]*); ([^)\n]*)\) (\{)?([^\n]*)$", re.M)


def merge_into_store(body, rng):
    """Drop a local whose value is stored to one place (`*out = total;`) and work on that place instead."""
    found = []
    for store in STORE.finditer(body):
        declaration = re.compile(
            rf"^([ \t]*)(?:const\s+)?[A-Za-z_][\w:<>]*(?:\s+[\w:<>]+)*\s+{store[2]}\s*=\s*([^;{{}}\n]+);", re.M)
        if declaration.search(body):
            found.append((store, declaration))
    if not found:
        return None
    store, declaration = rng.choice(found)
    out = body[:store.start()] + body[store.end():]
    out = declaration.sub(lambda m: f"{m[1]}{store[1]} = {m[2]};", out, count=1)
    return re.sub(rf"\b{store[2]}\b", lambda _: store[1], out)


def closing_brace(text, at):
    """Offset of the brace that closes the block opened just before offset `at`; None when unbalanced."""
    depth = 1
    for j in range(at, len(text)):
        depth += {"{": 1, "}": -1}.get(text[j], 0)
        if depth == 0:
            return j
    return None


def loop_form(body, rng):
    """`for (init; cond; step) s` becomes a while loop or a guarded do-while, in a block that scopes `init`."""
    m = pick(rng, FOR.finditer(body))
    if not m:
        return None
    indent, init, condition, step = m[1], m[2], m[3], m[4]
    if m[5]:
        close = closing_brace(body, m.end(5))
        if close is None:
            return None
        inner, end = body[m.end(5):close], close + 1
    else:
        inner, end = f" {m[6]}", m.end()
    inner = f"{inner.rstrip()}\n{indent}    {step};\n{indent}"
    if rng.random() < 0.5:
        loop = f"while ({condition}) {{{inner}}}"
    else:
        loop = f"if ({condition}) do {{{inner}}} while ({condition});"
    return f"{body[:m.start()]}{indent}{{\n{indent}{init};\n{indent}{loop}\n{indent}}}{body[end:]}"


def move_declaration(body, rng):
    """Move a local's declaration down to just before its first use, or up to the top of the body."""
    lines = body.split("\n")
    found = [i for i, line in enumerate(lines) if DECLARATION.fullmatch(line)]
    if not found:
        return None
    i = rng.choice(found)
    line = lines.pop(i)
    name = DECLARATION.fullmatch(line)[2]
    if rng.random() < 0.5:
        j = next((k for k in range(i, len(lines)) if re.search(rf"\b{name}\b", lines[k])), None)
        if j is None or j == i:
            return None
    else:
        j = next(k for k, text in enumerate(lines) if text.strip())
        if j >= i:
            return None
    indent = lines[j][:len(lines[j]) - len(lines[j].lstrip())]
    lines.insert(j, indent + line.lstrip())
    return "\n".join(lines)


MUTATIONS = [swap_operands, flip_comparison, swap_statements, retype_local, hoist_member, inline_local, index_form,
             split_condition, compound_assignment, toggle_const, merge_into_store, loop_form, move_declaration]


def mutate(body, rng, count=None):
    """`body` after `count` (default one to three) random rewrites that apply; None when none applies."""
    count = count or rng.choice([1, 1, 2, 3])
    changed = body
    for _ in range(count):
        for _ in range(20):
            out = rng.choice(MUTATIONS)(changed, rng)
            if out and out != changed:
                changed = out
                break
    return changed if changed != body else None


def split_arguments(text):
    """The top-level comma-separated parts of a parameter or argument list (`->` is not a bracket)."""
    parts, depth, current = [], 0, ""
    for k, ch in enumerate(text):
        if ch in "(<[":
            depth += 1
        elif ch in ")]" or (ch == ">" and text[k - 1:k] != "-"):
            depth -= 1
        if ch == "," and depth == 0:
            parts.append(current)
            current = ""
        else:
            current += ch
    return [p.strip() for p in [*parts, current] if p.strip()]


def definition(text, name):
    """(parameter names, body) of the first definition of `name` in `text`; an unnamed parameter is None."""
    start, end = body_span(text, name)
    close = text.rindex(")", 0, start)
    depth, k = 0, close
    while True:
        depth += {")": 1, "(": -1}.get(text[k], 0)
        if depth == 0:
            break
        k -= 1
    names = []
    for parameter in split_arguments(text[k + 1:close]):
        parameter = parameter.split("=")[0].strip()
        if parameter == "void":
            continue
        words = re.findall(r"\w+", parameter)
        named = len(words) > 1 and re.search(r"\w$", parameter) and words[-1] != "const"
        names.append(words[-1] if named else None)
    return names, text[start:end]


def inline_calls(body, name, parameters, callee):
    """`body` with each statement `name(args);` replaced by `callee` (a body), its parameters bound to the arguments."""
    if re.search(r"\breturn\b", callee):
        raise ValueError(f"{name} returns from inside its body, so its calls cannot be expanded in place")
    code = callee.strip("\n")

    def expand(m):
        arguments = split_arguments(m[2])
        if len(arguments) != len(parameters):
            raise ValueError(f"a call to {name} passes {len(arguments)} arguments for {len(parameters)} parameters")
        bound = {p: grouped(a) for p, a in zip(parameters, arguments) if p}
        # A parameter's name after `->` or `.` is a member of the same name, not the parameter.
        pattern = rf"(?<!->)(?<!\.)\b({'|'.join(map(re.escape, bound))})\b"
        text = re.sub(pattern, lambda b: bound[b[1]], code) if bound else code
        lines = text.split("\n")
        margin = min((len(t) - len(t.lstrip()) for t in lines if t.strip()), default=0)
        return "\n".join(m[1] + t[margin:] if t.strip() else "" for t in lines)

    return re.sub(rf"^([ \t]*){re.escape(name)}\s*(?:<[^<>\n]*>)?\s*\(([^;\n]*)\);[ \t]*$", expand, body, flags=re.M)


def read_source(path):
    """(text with \\n line ends, the text as stored)."""
    raw = path.read_bytes().decode("utf-8")
    return raw.replace("\r\n", "\n"), raw


class Source:
    """The searched bodies (`file:name` keys) in the project's files, and the files rebuilt from given bodies."""

    def __init__(self, project_dir, functions, default_file, inline=()):
        self.project_dir, self.files, self.raw, self.spans = project_dir, {}, {}, {}
        for spec in functions:
            qualified = re.fullmatch(r"([^:]+\.\w+):(.+)", spec)  # file:name; a name may itself hold `::`
            file, name = (qualified[1], qualified[2]) if qualified else (default_file, spec)
            if not file:
                raise ValueError(f"{spec}: give it as file:name or pass --source")
            if file not in self.files:
                self.files[file], self.raw[file] = read_source(project_dir / file)
            self.spans[f"{file}:{name}"] = body_span(self.files[file], name)
        for file in self.files:
            spans = sorted(s for k, s in self.spans.items() if k.split(":", 1)[0] == file)
            if any(a[1] > b[0] for a, b in zip(spans, spans[1:])):
                raise ValueError(f"{file}: two searched bodies overlap")
        self.initial = {k: self.files[k.split(":", 1)[0]][s:e] for k, (s, e) in self.spans.items()}
        self.start = dict(self.initial)
        for name in inline:
            parameters, callee = self.find(name)
            expanded = {k: inline_calls(b, name, parameters, callee) for k, b in self.start.items()}
            if expanded == self.start:
                raise ValueError(f"--inline {name}: no call statement `{name}(...);` in the searched bodies (a call "
                                 f"inside an expression or under a qualified name is not expanded)")
            self.start = expanded

    def find(self, name):
        """(parameters, body) of `name`, from the searched files first, then the project's other sources."""
        others = sorted(p for p in self.project_dir.rglob("*") if p.suffix.lower() in (".h", ".hpp", ".cpp", ".cxx"))
        for text in [*self.files.values(), *(read_source(p)[0] for p in others)]:
            try:
                return definition(text, name)
            except ValueError:
                continue
        raise ValueError(f"--inline {name}: no definition found in {self.project_dir}")

    def texts(self, bodies):
        """File -> text with `bodies` in place of the searched bodies."""
        out = {}
        for file, text in self.files.items():
            for key in sorted((k for k in self.spans if k.split(":", 1)[0] == file), key=lambda k: -self.spans[k][0]):
                start, end = self.spans[key]
                text = text[:start] + bodies[key] + text[end:]
            out[file] = text
        return out

    def patch(self, bodies, prefix):
        """A `git apply` patch from the files as read to the files with `bodies`; paths are `prefix/file`. Lines the
        patch keeps or removes carry their own line ends, so a file with mixed line ends still applies; added lines
        take the file's most common line end."""
        out = []
        for file, text in self.texts(bodies).items():
            if text == self.files[file]:
                continue
            a = re.findall(r"[^\n]*\n|[^\n]+$", self.raw[file])
            eol = "\r\n" if 2 * sum(line.endswith("\r\n") for line in a) > len(a) else "\n"
            out += unified(a, lines_with(text, eol), f"{prefix}/{file}".lstrip("/"))
        return "".join(out)


def unified(a, b, path):
    """Unified diff lines from the lines `a` to `b` (each with its line end), compared without line ends; context and
    removed lines are `a`'s exactly."""
    def bare(lines):
        return [line.rstrip("\r\n") for line in lines]

    def span(start, stop):
        return f"{start + 1}" if stop - start == 1 else f"{start + (1 if stop > start else 0)},{stop - start}"

    out = [f"--- a/{path}\n", f"+++ b/{path}\n"]
    for group in difflib.SequenceMatcher(None, bare(a), bare(b), autojunk=False).get_grouped_opcodes(3):
        (_, i1, _, j1, _), (_, _, i2, _, j2) = group[0], group[-1]
        out.append(f"@@ -{span(i1, i2)} +{span(j1, j2)} @@\n")
        for tag, a1, a2, b1, b2 in group:
            if tag == "equal":
                out += [" " + line for line in a[a1:a2]]
                continue
            # An added line ends as the line it replaces, or the line before it, does.
            near = a[a1] if a2 > a1 else a[a1 - 1] if a1 else ""
            end = "\r\n" if near.endswith("\r\n") else "\n" if near.endswith("\n") else None
            added = [line.rstrip("\r\n") + end if end and line.endswith("\n") else line for line in b[b1:b2]]
            out += ["-" + line for line in a[a1:a2]] + ["+" + line for line in added]
    return [line if line.endswith("\n") else f"{line}\n\\ No newline at end of file\n" for line in out]


def lines_with(text, eol):
    """The lines of `text` (\\n line ends) with `eol` ends; a last line without a line end keeps none."""
    parts = text.split("\n")
    return [p + eol for p in parts[:-1]] + ([parts[-1]] if parts[-1] else [])


def write_patch(path, patch):
    """Write `patch` byte for byte: a text-mode write on Windows would turn each line end into CRLF (and CRLF
    into CR CR LF), and the patch would no longer apply."""
    path.write_bytes(patch.encode("utf-8"))


def git_prefix(project_dir):
    """The project directory relative to its repository root ('' outside a repository), for patch paths."""
    result = subprocess.run(["git", "-C", str(project_dir), "rev-parse", "--show-prefix"], capture_output=True,
                            text=True)
    return result.stdout.strip().rstrip("/") if result.returncode == 0 else project_dir.name


class Worker:
    """One scratch copy of the project and its build outputs."""

    def __init__(self, k, project_dir, work):
        self.root = work / f"w{k}"
        self.project = self.root / project_dir.name
        shutil.copytree(project_dir, self.project, ignore=shutil.ignore_patterns(".vs", "x64", "*.user"))
        self.out, self.obj = self.root / "bin", self.root / "obj"
        self.vcxproj = next(self.project.glob("*.vcxproj"))

    def build(self, texts, timeout=600):
        """(DLL, map text) of the project with each file of `texts` (relative to the project) set to its text; None
        when the build fails."""
        for file, text in texts.items():
            (self.project / file).write_bytes(text.encode("utf-8"))
        env = {**os.environ, "_LINK_": f"/MAP:{self.out / 'candidate.map'}", "MSBUILDDISABLENODEREUSE": "1"}
        process = subprocess.Popen([str(msbuild()), str(self.vcxproj), "-t:Build", "-p:Configuration=Release",
                                    "-p:Platform=x64", f"-p:OutDir={self.out}{os.sep}", f"-p:IntDir={self.obj}{os.sep}",
                                    "-nologo", "-v:q", "-nr:false", "-m:1"],
                                   stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, env=env)
        try:
            returncode = process.wait(timeout=timeout)
        except subprocess.TimeoutExpired:
            # Killing MSBuild alone leaves cl.exe and link.exe holding this worker's files: end the whole tree.
            subprocess.run(["taskkill", "/T", "/F", "/PID", str(process.pid)], capture_output=True)
            process.wait()
            raise
        dlls = list(self.out.glob("*.dll"))
        if returncode or len(dlls) != 1:
            return None
        return dlls[0], (self.out / "candidate.map").read_text(encoding="utf-8", errors="replace")


def resolve(names, original):
    """Decorated export names of the original for `names`, given decorated or as the plain function name."""
    out = []
    for name in names:
        found = [e for e in original.image.exports if e == name or e.startswith(f"?{name}@")]
        if len(found) != 1:
            raise ValueError(f"{name}: {len(found)} exports of the original match")
        out.append(found[0])
    return out


def strict(result):
    return result.ok and not result.unverified


def code_of(candidate, name):
    """What native_gate.py fingerprints for the export `name` of a linked candidate (its instructions by name)."""
    rva = candidate.image.exports[name]
    return native_linked.fingerprint_text(candidate, name) if candidate.executable(rva) else name


class Scorer:
    """Scores builds against the original: (-exports broken, symbols matching, mean similarity), compared as a tuple.
    A symbol matches when nothing mismatches (an unverified target is reported at the end, not ranked). An export is
    broken when native_gate.py would fail it: one of the guard (exports that matched strictly at the start) that no
    longer does, or one of the watched (every other export, by its code at the start) whose code changed without
    matching."""

    def __init__(self, original, entries, symbols):
        self.original, self.entries, self.symbols = original, entries, symbols
        self.guard, self.watched = set(), {}

    def comparison(self, built):
        dll, map_text = built
        return native_linked.Comparison(self.original, native_linked.Linked.load(dll, map_text), self.entries)

    def baseline(self, built):
        """Take the guard and the watched exports from the build of the source as given."""
        comparison = self.comparison(built)
        others = [n for n in comparison.c.image.exports if n not in self.symbols and n in self.original.image.exports]
        self.guard = {n for n in others if strict(comparison.export(n))}
        self.watched = {n: code_of(comparison.c, n) for n in others if n not in self.guard}

    def broken(self, comparison):
        """[(export, why)] of the exports native_gate.py would fail in `comparison`'s candidate."""
        exports, out = comparison.c.image.exports, []
        for name in sorted(self.guard):
            r = comparison.export(name) if name in exports else None
            if r is None or not strict(r):
                out.append((name, f"{r.reason if r else 'not exported'} (it matched before the change)"))
        for name, code in sorted(self.watched.items()):
            if name in exports and code_of(comparison.c, name) != code and not strict(comparison.export(name)):
                out.append((name, "its code changed and it does not match: native_gate.py fails a changed export "
                                  "that does not pass; add it with --symbol or leave its code as it was"))
        return out

    def __call__(self, built):
        comparison = self.comparison(built)
        exports = comparison.c.image.exports
        if any(s not in exports for s in self.symbols):
            return -len(self.guard) - len(self.watched) - 1, -1, 0.0
        broken = len(self.broken(comparison))
        matching = sum(comparison.export(s).ok for s in self.symbols)
        return -broken, matching, sum(map(comparison.similarity, self.symbols)) / len(self.symbols)

    def matched(self, value):
        return value[0] == 0 and value[1] == len(self.symbols)


def search(args):
    project_dir = (args.project_dir or ROOT / "src" / args.project).resolve()
    source = Source(project_dir, args.function, args.source, args.inline or ())
    print(f"loading the original {original_dll(ROOT, args.project)} ...", flush=True)
    original = native_linked.Linked.load(Path(args.install or mathcad_install()) / original_dll(ROOT, args.project))
    entries = load_targets(args.targets, original.image) if args.targets.is_file() else {}
    scorer = Scorer(original, entries, resolve(args.symbol, original))
    symbols = scorer.symbols
    first = re.sub(r"\W+", "_", args.function[0].split(":")[-1])
    out_dir = Path(args.out or ROOT / ".mirror-cache" / "permute" / f"{first}-{time.strftime('%Y%m%d-%H%M%S')}")
    out_dir.mkdir(parents=True, exist_ok=True)
    work = Path(tempfile.mkdtemp(prefix="permute-", dir=args.scratch))
    try:
        return search_in(args, source, scorer, out_dir, work, project_dir)
    finally:
        if not args.keep:
            shutil.rmtree(work, ignore_errors=True)


def search_in(args, source, scorer, out_dir, work, project_dir):
    symbols, original = scorer.symbols, scorer.original
    workers = [Worker(k, project_dir, work) for k in range(args.workers)]
    prefix = git_prefix(project_dir)

    built = workers[0].build(source.texts(source.initial))
    if built is None:
        raise RuntimeError(f"the project as given does not build; build it once yourself ({project_dir})")
    if reason := msvc_mismatch(built[0]):
        raise RuntimeError(f"{built[0].name}: {reason}")
    missing = [s for s in symbols if s not in native_linked.Linked.load(*built).image.exports]
    if missing:
        raise RuntimeError(f"the build does not export {missing}: write the exports before searching their shape")
    scorer.baseline(built)
    if source.start != source.initial:
        built = workers[0].build(source.texts(source.start))
        if built is None:
            raise RuntimeError(f"the project does not build after --inline {args.inline}; see {workers[0].project}")
    state = {"score": scorer(built), "bodies": dict(source.start), "tries": 0, "failed": 0, "improved": 0}
    print(f"baseline: {state['score'][1]}/{len(symbols)} matching, similarity {state['score'][2]:.4f}; "
          f"{len(scorer.guard)} other exports match and are guarded, {len(scorer.watched)} more are watched",
          flush=True)
    seen = {tuple(sorted(state["bodies"].items()))}
    lock, stop = threading.Lock(), threading.Event()
    deadline = time.monotonic() + args.minutes * 60
    start_score = state["score"]

    def run(k):
        # Each worker climbs its own lineage, so one stuck on a misleading path does not hold the others. A lineage
        # that stalls restarts from the start or from the best source found so far.
        rng = random.Random(args.seed * 1000 + k)
        current, current_score = dict(source.start), start_score
        misses = stale = 0
        while not stop.is_set() and time.monotonic() < deadline and not scorer.matched(state["score"]):
            if stale >= args.restart:
                with lock:
                    best = (dict(state["bodies"]), state["score"])
                current, current_score = best if rng.random() < 0.5 else (dict(source.start), start_score)
                stale = 0
            bodies = dict(current)
            key = rng.choice(sorted(bodies))
            body = mutate(bodies[key], rng)
            with lock:
                bodies[key] = body
                fingerprint = tuple(sorted(bodies.items()))
                if body is None or fingerprint in seen:
                    misses += 1
                    if misses > 2000:
                        return  # the rewrites find nothing new anywhere near here
                    if misses % 200 == 0:
                        stale = args.restart
                    continue
                misses = 0
                seen.add(fingerprint)
                state["tries"] += 1
            stale += 1
            try:
                result = workers[k].build(source.texts(bodies), max(60, deadline - time.monotonic() + 60))
                value = scorer(result) if result else None
            except Exception as e:  # one bad build or image must not end this worker's lineage
                print(f"worker {k}: {type(e).__name__}: {e}", file=sys.stderr, flush=True)
                value = None
            with lock:
                if value is None:
                    state["failed"] += 1
                    continue
                if value > state["score"]:
                    state["improved"] += 1
                    name = f"{state['improved']:03d}-matching{value[1]}-sim{value[2]:.4f}.diff"
                    write_patch(out_dir / name, source.patch(bodies, prefix))
                    print(f"[{state['tries']}] {value[1]}/{len(symbols)} matching, similarity {value[2]:.4f} -> {name}",
                          flush=True)
                    state["score"], state["bodies"] = value, bodies
            if value > current_score:
                current, current_score, stale = bodies, value, 0
            elif value == current_score and rng.random() < args.drift:
                current = bodies

    threads = [threading.Thread(target=run, args=(k,)) for k in range(args.workers)]
    for t in threads:
        t.start()
    try:
        while any(t.is_alive() for t in threads):
            for t in threads:
                t.join(0.5)  # a timed join lets Ctrl+C reach this thread
    except KeyboardInterrupt:
        print("stopping after the builds in progress ...", flush=True)
        stop.set()
        for t in threads:
            t.join()
    bodies = state["bodies"]
    write_patch(out_dir / "best.patch", source.patch(bodies, prefix))
    print(f"{state['tries']} attempts ({state['failed']} did not build); best {state['score'][1]}/{len(symbols)} "
          f"matching, similarity {state['score'][2]:.4f}", flush=True)
    if scorer.matched(state["score"]) and not args.no_reduce and not stop.is_set():
        reduce_deadline = time.monotonic() + args.reduce_minutes * 60

        def matches(candidate):
            if time.monotonic() > reduce_deadline:
                return False  # out of time: keep what is reduced so far
            built = workers[0].build(source.texts(candidate))
            return built is not None and scorer.matched(scorer(built))

        print("reducing the change to what the match needs ...", flush=True)
        try:
            # Toward the expanded source first (its lines are what the search rewrote), then toward the source as
            # written, which can fold an expanded helper back into its call.
            bodies = reduce(source.start, bodies, matches)
            if source.start != source.initial:
                bodies = reduce(source.initial, bodies, matches)
        except KeyboardInterrupt:
            print("reduction stopped; keeping the match as found", flush=True)
        write_patch(out_dir / "best.patch", source.patch(bodies, prefix))
    status, report = final_report(scorer, workers[0].build(source.texts(bodies)))
    if status != "NO MATCH":
        shutil.copyfile(out_dir / "best.patch", out_dir / "match.patch")
    (out_dir / "report.txt").write_text(report, encoding="utf-8")
    print(report)
    print(f"output: {out_dir}")
    return 0 if status == "MATCH" else 1


def reduce(initial, bodies, matches):
    """The smallest change from the `initial` bodies that still `matches`: each changed hunk is put back where the
    match allows, whole or line by line, then temporaries the rewrites introduced are folded and redundant
    parentheses dropped."""
    bodies = dict(bodies)
    for key in sorted(bodies):
        progress = True
        while progress:
            progress = False
            a, b = initial[key].split("\n"), bodies[key].split("\n")
            hunks = [h for h in difflib.SequenceMatcher(None, a, b, autojunk=False).get_opcodes() if h[0] != "equal"]
            whole, restored, dropped = [], [], []
            for _, i1, i2, j1, j2 in reversed(hunks):
                whole.append(b[:j1] + a[i1:i2] + b[j2:])
                # A hunk can mix a line the match needs with lines it does not: put back the original line a changed
                # line most resembles (moved lines do not pair up by position), or failing that drop the line.
                for j in range(j1, j2):
                    closest = max(a[i1:i2], key=lambda line: difflib.SequenceMatcher(None, line, b[j]).ratio(),
                                  default=None)
                    if closest is not None and closest not in b:
                        restored.append(b[:j] + [closest] + b[j + 1:])
                    dropped.append(b[:j] + b[j + 1:])
                # A line that moved: put it back where the original had it.
                for i in range(i1, i2):
                    if a[i] in b[:j1] + b[j2:]:
                        rest = [line for line in b if line != a[i]]
                        restored.append(rest[:j1] + [a[i]] + rest[j1:])
            for trial in whole + restored + dropped:
                if matches({**bodies, key: "\n".join(trial)}):
                    bodies[key], progress = "\n".join(trial), True
                    break
        # Fold the temporaries the rewrites introduced (hoist_member's `auto tN`), never the source's own.
        for name in re.findall(r"^[ \t]*auto (t\d+) = ", bodies[key], re.M):
            m = re.search(rf"^[ \t]*auto {name} = ([^;\n]+);[^\n]*\n", bodies[key], re.M)
            if m is None or re.search(rf"\b{name}\b", initial[key]):
                continue
            rest = re.sub(rf"\b{name}\b", lambda _: grouped(m[1]), bodies[key][m.end():])
            trial = bodies[key][:m.start()] + rest
            if matches({**bodies, key: trial}):
                bodies[key] = trial
        trial = strip_parentheses(bodies[key], set(initial[key].split("\n")))
        if trial != bodies[key] and matches({**bodies, key: trial}):
            bodies[key] = trial
        # A declaration the search added may sit above a check it should follow (a read before a null test):
        # move it down to its first use where the match allows.
        for line in [line for line in bodies[key].split("\n")
                     if DECLARATION.fullmatch(line) and line not in initial[key].split("\n")]:
            for trial in sink(bodies[key], line):
                if matches({**bodies, key: trial}):
                    bodies[key] = trial
                    break
    return bodies


def sink(body, declaration):
    """`body` with the line `declaration` moved down one line, two lines, ... up to just before the first later line
    that uses its name: the nearest first, since the read order the match needs may allow only a short move."""
    lines = body.split("\n")
    i = lines.index(declaration)
    name = DECLARATION.fullmatch(declaration)[2]
    rest = lines[:i] + lines[i + 1:]
    j = next((k for k in range(i, len(rest)) if re.search(rf"\b{name}\b", rest[k])), i)
    return ["\n".join(rest[:k] + [declaration] + rest[k:]) for k in range(i + 1, j + 1)]


def strip_parentheses(body, unchanged=()):
    """`body` without parentheses around a lone name, member or number, except on `unchanged` lines, inside literals
    and comments, after if/while/for/switch/sizeof, and where they make a cast or call (`f(x)`,
    `static_cast<int>(y)`)."""
    out = []
    for line in body.split("\n"):
        if line not in unchanged:
            masked = LITERAL_OR_COMMENT.sub(lambda m: " " * len(m[0]), line)
            for m in reversed(list(re.finditer(r"\((\w+(?:->\w+)*|\d+(?:\.\d+)?)\)(?![\w(])", masked))):
                if not re.search(r"(?:\bif|\bwhile|\bfor|\bswitch|\bsizeof|[\w)\]>])\s*$", masked[:m.start()]):
                    line = line[:m.start()] + m[1] + line[m.end():]
        out.append(line)
    return "\n".join(out)


def final_report(scorer, built):
    """(status, text): MATCH when every symbol matches strictly and no guarded export broke; MATCH WITH UNVERIFIED
    TARGETS when the instructions match but a target is unverified, which native_gate.py still fails; else NO
    MATCH."""
    if built is None:
        return "NO MATCH", "NO MATCH: the best source does not build\n"
    comparison = scorer.comparison(built)
    exports = comparison.c.image.exports
    lines, unverified, failed = [], 0, 0
    for symbol in scorer.symbols:
        r = comparison.export(symbol) if symbol in exports else None
        if r is None or not r.ok:
            failed += 1
            lines.append(f"FAIL {symbol}: {r.reason if r else 'not exported'}")
        elif r.unverified:
            unverified += 1
            lines.append(f"UNVERIFIED {symbol}: {r.unverified} targets no rule settles (native_linked.py lists the rules)")
        else:
            lines.append(f"OK   {symbol}")
    for name, why in scorer.broken(comparison):
        failed += 1
        lines.append(f"BROKE {name}: {why}")
    status = "NO MATCH" if failed else "MATCH WITH UNVERIFIED TARGETS" if unverified else "MATCH"
    advice = {"MATCH": "Apply match.patch with `git apply` from the repository root, then run native_gate.py.",
              "MATCH WITH UNVERIFIED TARGETS": "The instructions match; native_gate.py fails until the unverified "
                                               "targets verify. Apply match.patch and inspect them with native_linked.py.",
              "NO MATCH": "best.patch holds the closest source found; the diff files show each improvement."}[status]
    matching = sum(line.startswith(("OK", "UNVERIFIED")) for line in lines)
    return status, "\n".join([f"{status}: {matching}/{len(scorer.symbols)} symbols, {len(scorer.guard)} guarded and "
                              f"{len(scorer.watched)} watched exports checked", *lines, advice,
                              "FAIL lines quote the original's instructions: keep this report out of commits and "
                              "pull requests (D15)."]) + "\n"


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--project", required=True, help="native project name (its original DLL comes from the map)")
    parser.add_argument("--project-dir", type=Path, help="project directory to copy (default: src/<project>)")
    parser.add_argument("--source", help="file holding the searched functions, relative to the project")
    parser.add_argument("--function", action="append", required=True,
                        help="body to search: name, or file:name relative to the project (repeatable)")
    parser.add_argument("--inline", action="append", help="expand calls to this function in the searched bodies first")
    parser.add_argument("--symbol", action="append", required=True, help="export to match (repeatable)")
    parser.add_argument("--workers", type=int, default=max(1, (os.cpu_count() or 2) // 2))
    parser.add_argument("--minutes", type=float, default=30)
    parser.add_argument("--seed", type=int, default=1)
    parser.add_argument("--drift", type=float, default=0.1, help="chance an equal score replaces the current source")
    parser.add_argument("--restart", type=int, default=25,
                        help="attempts without progress after which a worker restarts its lineage")
    parser.add_argument("--targets", type=Path, default=DEFAULT_TARGETS)
    parser.add_argument("--install", type=Path, help="Mathcad install (default: MATHCAD_DIR or the standard path)")
    parser.add_argument("--out", type=Path, help="where results go (default: .mirror-cache/permute/...)")
    parser.add_argument("--scratch", type=Path, help="parent of the per-worker build copies (default: system temp)")
    parser.add_argument("--keep", action="store_true", help="keep the build copies")
    parser.add_argument("--no-reduce", action="store_true", help="keep the match as found, without reducing it")
    parser.add_argument("--reduce-minutes", type=float, default=20, help="time limit for reducing a match")
    return search(parser.parse_args(argv))


if __name__ == "__main__":
    sys.exit(main())
