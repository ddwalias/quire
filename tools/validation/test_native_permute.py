"""Checks for native_permute.py: body location, source rewrites, helper expansion, patches, reduction and the score."""
import random
import shutil
import subprocess
import tempfile
import unittest
from pathlib import Path
from types import SimpleNamespace
from unittest import mock

from native_permute import (MUTATIONS, Scorer, Source, body_span, compound_assignment, definition, final_report,
                            flip_comparison, hoist_member, index_form, inline_calls, inline_local, loop_form,
                            merge_into_store, move_declaration, mutate, reduce, retype_local, split_arguments,
                            split_condition, strip_parentheses, swap_operands, swap_statements, toggle_const,
                            Worker, logged_commands, write_patch)

SOURCE = """template <typename T>
const char *sum(Efi::EfiCallContext *, const typed_mat<T> *m, void *context, T *out);

template <typename T>
const char *sum(Efi::EfiCallContext *, const typed_mat<T> *m, void *context, T *out) {
    validate(m);
    if (out == nullptr) return "null pointer";
    T *data = m->data;
    T total = 0;
    for (int i = 0; i < m->size; ++i) {
        total += data[i];
    }
    *out = total;
    return nullptr;
}

const char *matdbl_sum(Efi::EfiCallContext *ctx, const _double_mat *m, void *context, double *out) {
    return sum(ctx, m, context, out);
}
"""


def body():
    start, end = body_span(SOURCE, "sum")
    return SOURCE[start:end]


class BodySpanTests(unittest.TestCase):
    def test_the_definition_is_found_past_declarations_and_calls(self):
        self.assertTrue(body().startswith("\n    validate(m);"))
        self.assertTrue(body().endswith("return nullptr;\n"))

    def test_braces_and_names_in_literals_and_comments_do_not_count(self):
        text = 'void f() {\n    g("}{ f()");  // }\n    h(\'}\');\n}\nvoid k() {}\n'
        start, end = body_span(text, "f")
        self.assertEqual(text[start:end], '\n    g("}{ f()");  // }\n    h(\'}\');\n')

    def test_definitions_with_specifiers_and_trailing_return_types_are_found(self):
        for header in ["int f() override {", "int f() const noexcept(true) {", "auto f() -> int {", "int f() final {"]:
            text = f"int f();\n{header}\n    return 1;\n}}\n"
            start, end = body_span(text, "f")
            self.assertEqual(text[start:end], "\n    return 1;\n", header)

    def test_a_missing_definition_fails(self):
        with self.assertRaises(ValueError):
            body_span("int f();", "f")


class Rewrites:
    def only(self, mutation, text):
        """Every distinct result of `mutation` over many seeds."""
        return {mutation(text, random.Random(seed)) for seed in range(40)}


class RewriteTests(Rewrites, unittest.TestCase):
    def test_operands_swap_around_a_commutative_operator(self):
        self.assertEqual(self.only(swap_operands, "x = a * b;"), {"x = b * a;"})
        self.assertEqual(self.only(swap_operands, "if (out == nullptr) f();"), {"if (nullptr == out) f();"})

    def test_a_pointer_declaration_is_not_a_product(self):
        self.assertEqual(self.only(swap_operands, "T *data = m->data;"), {None})

    def test_operands_inside_a_member_call_or_under_a_unary_prefix_stay(self):
        self.assertEqual(self.only(swap_operands, "x = a * b->f();"), {None})
        self.assertEqual(self.only(swap_operands, "x = *p * 2;"), {None})

    def test_comparisons_flip(self):
        self.assertEqual(self.only(flip_comparison, "if (i < m->size) f();"), {"if (m->size > i) f();"})

    def test_template_arguments_are_not_comparisons(self):
        self.assertEqual(self.only(flip_comparison, "typed_mat<T> *m;"), {None})

    def test_adjacent_simple_statements_swap_but_control_flow_does_not(self):
        self.assertEqual(self.only(swap_statements, "    a = 1;\n    b = 2;\n"), {"    b = 2;\n    a = 1;\n"})
        self.assertEqual(self.only(swap_statements, "    a = 1;\n    return a;\n"), {None})

    def test_local_types_change(self):
        out = self.only(retype_local, "    int size = m->size;")
        self.assertIn("    __int64 size = m->size;", out)
        self.assertNotIn("    int size = m->size;", out)

    def test_a_multiword_type_is_not_retyped_in_part(self):
        self.assertEqual(self.only(retype_local, "    unsigned long n = 0;"), {None})

    def test_a_member_read_moves_into_a_local(self):
        self.assertEqual(self.only(hoist_member, "    x = m->size;\n"), {"    auto t0 = m->size;\n    x = t0;\n"})

    def test_a_hoisted_name_neither_collides_nor_replaces_a_longer_member(self):
        self.assertEqual(self.only(hoist_member, "    int t0 = 1;\n    x = m->size + m->sizes;\n"),
                         {"    int t0 = 1;\n    auto t1 = m->size;\n    x = t1 + m->sizes;\n",
                          "    int t0 = 1;\n    auto t1 = m->sizes;\n    x = m->size + t1;\n"})

    def test_an_assigned_member_is_not_hoisted(self):
        self.assertEqual(self.only(hoist_member, "    m->size = 0;\n"), {None})

    def test_a_local_assigned_once_is_inlined_and_one_assigned_again_is_not(self):
        self.assertEqual(self.only(inline_local, "    int n = m->size;\n    f(n);\n"), {"    f(m->size);\n"})
        self.assertEqual(self.only(inline_local, "    int n = 0;\n    n += 1;\n"), {None})

    def test_index_forms_convert_both_ways(self):
        self.assertEqual(self.only(index_form, "x = p[i];"), {"x = *(p + i);"})
        self.assertEqual(self.only(index_form, "x = *(p + i);"), {"x = p[i];"})
        self.assertEqual(self.only(index_form, "x = m->data[i];"), {None})

    def test_an_or_condition_splits_and_two_ifs_merge(self):
        split = "    if (a <= 0) f();\n    if (b <= 0) f();"
        self.assertEqual(self.only(split_condition, "    if (a <= 0 || b <= 0) f();"), {split})
        self.assertEqual(self.only(split_condition, split), {"    if (a <= 0 || b <= 0) f();"})

    def test_compound_assignment_converts_both_ways(self):
        self.assertEqual(self.only(compound_assignment, "    total += x;"), {"    total = total + x;"})
        self.assertEqual(self.only(compound_assignment, "    total = total + x;"), {"    total += x;"})
        self.assertEqual(self.only(compound_assignment, "    x -= a - b;"), {"    x = x - (a - b);"})

    def test_const_toggles_on_a_local(self):
        self.assertEqual(self.only(toggle_const, "    int n = 1;"), {"    const int n = 1;"})
        self.assertEqual(self.only(toggle_const, "    const int n = 1;"), {"    int n = 1;"})

    def test_a_local_stored_to_one_place_gives_way_to_that_place(self):
        # The rewrite that turned efistatsci_'s parked sum loops into matches.
        text = "    *out = 0;\n    T total = 0;\n    for (int i = 0; i < n; ++i) {\n        total += data[i];\n" \
               "        *out = total;\n    }\n"
        self.assertEqual(self.only(merge_into_store, text),
                         {"    *out = 0;\n    *out = 0;\n    for (int i = 0; i < n; ++i) {\n        *out += data[i];\n"
                          "    }\n"})

    def test_a_store_of_a_parameter_is_left_alone(self):
        self.assertEqual(self.only(merge_into_store, "    *out = value;\n"), {None})

    def test_a_for_loop_becomes_a_scoped_while_or_guarded_do_while(self):
        out = self.only(loop_form, "    for (int i = 0; i < n; ++i) {\n        s(i);\n    }\n    t();\n")
        self.assertEqual(out, {"    {\n    int i = 0;\n    while (i < n) {\n        s(i);\n        ++i;\n    }\n    }\n    t();\n",
                               "    {\n    int i = 0;\n    if (i < n) do {\n        s(i);\n        ++i;\n    } while (i < n);\n"
                               "    }\n    t();\n"})
        single = self.only(loop_form, "    for (int i = 0; i < n; ++i) s(i);\n")
        self.assertIn("    {\n    int i = 0;\n    while (i < n) { s(i);\n        ++i;\n    }\n    }\n", single)

    def test_a_declaration_moves_to_its_first_use_or_to_the_top(self):
        text = "\n    f();\n    int n = 1;\n    g();\n    h(n);\n"
        self.assertEqual(self.only(move_declaration, text),
                         {"\n    f();\n    g();\n    int n = 1;\n    h(n);\n",
                          "\n    int n = 1;\n    f();\n    g();\n    h(n);\n"})


class ExpansionTests(unittest.TestCase):
    CALLEE = "template <typename T>\nvoid check(const typed_mat<T> *m, int limit) {\n    if (m->rows > limit) f();\n}\n"

    def test_arguments_split_at_the_top_level_only(self):
        self.assertEqual(split_arguments("a->b, g(x, y), typed_mat<T, U> *m"), ["a->b", "g(x, y)", "typed_mat<T, U> *m"])

    def test_a_definition_gives_its_parameter_names_and_unnamed_ones_as_none(self):
        self.assertEqual(definition(self.CALLEE, "check")[0], ["m", "limit"])
        self.assertEqual(definition("int f(Efi::Ctx *, int n) { return n; }", "f")[0], [None, "n"])
        self.assertEqual(definition("void g(void) {}", "g")[0], [])

    def test_a_call_statement_expands_with_its_arguments_bound(self):
        parameters, callee = definition(self.CALLEE, "check")
        out = inline_calls("    x();\n    check(src->m, a + 1);\n", "check", parameters, callee)
        self.assertEqual(out, "    x();\n    if (src->m->rows > (a + 1)) f();\n")

    def test_a_callee_that_returns_early_is_refused(self):
        with self.assertRaises(ValueError):
            inline_calls("    g();\n", "g", [], "\n    if (a) return;\n    b();\n")

    def test_a_parameter_name_used_as_a_member_name_is_not_bound(self):
        out = inline_calls("    h(p, n);\n", "h", ["q", "size"], "\n    q->size = size;\n    q.size = 0;\n")
        self.assertEqual(out, "    p->size = n;\n    p.size = 0;\n")


class SourceTests(unittest.TestCase):
    def project(self, files):
        root = Path(tempfile.mkdtemp())
        subprocess.run(["git", "init", "-q", str(root)], check=True)
        project = root / "src" / "P"
        project.mkdir(parents=True)
        for name, text in files.items():
            (project / name).write_bytes(text.encode())
        return root, project

    def test_bodies_in_two_files_rebuild_their_files_and_the_patch_applies(self):
        root, project = self.project({"a.cpp": "int f() {\r\n    return 1;\r\n}\r\n", "b.h": "void g() {\n    h();\n}\n"})
        source = Source(project, ["a.cpp:f", "b.h:g"], None)
        bodies = {"a.cpp:f": "\n    return 2;\n", "b.h:g": source.initial["b.h:g"]}
        self.assertEqual(source.texts(bodies)["a.cpp"], "int f() {\n    return 2;\n}\n")
        patch = source.patch(bodies, "src/P")
        self.assertIn("+++ b/src/P/a.cpp", patch)
        self.assertNotIn("b.h", patch)
        check = subprocess.run(["git", "-C", str(root), "apply", "--check", "-"], input=patch.encode(),
                               capture_output=True)
        self.assertEqual(check.returncode, 0, check.stderr.decode())

    def test_a_patch_applies_to_a_file_with_mixed_line_ends_and_no_final_line_end(self):
        root, project = self.project({"a.cpp": "int f() {\r\n    return 1;\r\n}\r\nint g() {\n    return 2;\n}"})
        source = Source(project, ["a.cpp:f", "a.cpp:g"], None)
        # Through a file, as a lane applies it: the file must hold the patch's line ends unchanged.
        write_patch(root / "match.patch", source.patch({"a.cpp:f": "\n    return 3;\n", "a.cpp:g": "\n    return 4;\n"},
                                                       "src/P"))
        applied = subprocess.run(["git", "-C", str(root), "apply", "match.patch"], capture_output=True)
        self.assertEqual(applied.returncode, 0, applied.stderr.decode())
        self.assertEqual((project / "a.cpp").read_bytes(),
                         b"int f() {\r\n    return 3;\r\n}\r\nint g() {\n    return 4;\n}")

    def test_inline_expands_a_helper_defined_in_another_file(self):
        _, project = self.project({"a.cpp": "int f(M *m) {\n    check(m);\n    return 1;\n}\n",
                                   "h.h": "void check(M *x) {\n    if (!x) fail();\n}\n"})
        source = Source(project, ["f"], "a.cpp", ["check"])
        self.assertEqual(source.start["a.cpp:f"], "\n    if (!m) fail();\n    return 1;\n")
        self.assertEqual(source.initial["a.cpp:f"], "\n    check(m);\n    return 1;\n")

    def test_a_function_without_a_file_needs_source(self):
        _, project = self.project({"a.cpp": "int f() {}\n"})
        with self.assertRaises(ValueError):
            Source(project, ["f"], None)

    def test_a_qualified_name_with_or_without_a_file(self):
        _, project = self.project({"a.cpp": "int B::f() {\n    return 2;\n}\nint A::f() {\n    return 1;\n}\n"})
        self.assertEqual(Source(project, ["a.cpp:A::f"], None).initial, {"a.cpp:A::f": "\n    return 1;\n"})
        self.assertEqual(Source(project, ["A::f"], "a.cpp").initial, {"a.cpp:A::f": "\n    return 1;\n"})

    def test_inline_that_expands_nothing_fails_loudly(self):
        _, project = self.project({"a.cpp": "void check(M *x) {\n    g(x);\n}\nint f(M *m) {\n    if (!check(m)) h();\n}\n"})
        with self.assertRaises(ValueError):
            Source(project, ["f"], "a.cpp", ["check"])


class ReduceTests(unittest.TestCase):
    def test_hunks_the_match_does_not_need_go_back_and_temporaries_fold(self):
        initial = {"k": "\n    a();\n    b(m->x);\n    c();\n"}
        found = {"k": "\n    z();\n    auto t0 = m->x;\n    b(t0);\n    c((y));\n"}
        def matches(bodies):
            body = bodies["k"]
            compiles = "b(t0)" not in body or "auto t0" in body
            return compiles and ("b(m->x)" in body or "b(t0)" in body) and "z()" in body and "c(" in body
        self.assertEqual(reduce(initial, found, matches), {"k": "\n    z();\n    b(m->x);\n    c();\n"})

    def test_a_moved_line_goes_back_and_a_duplicate_goes(self):
        initial = {"k": "\n    if (m == 0) f();\n    int c = m->c;\n    *o = 0;\n"}
        found = {"k": "\n    int c = m->c;\n    if (m == 0) f();\n    *o = 0;\n    *o = 0;\n    g();\n"}
        self.assertEqual(reduce(initial, found, lambda bodies: "g()" in bodies["k"]),
                         {"k": "\n    if (m == 0) f();\n    int c = m->c;\n    *o = 0;\n    g();\n"})

    def test_an_added_declaration_sinks_below_the_check_it_preceded(self):
        initial = {"k": "\n    validate(m);\n    g();\n"}
        found = {"k": "\n    int c = m->c;\n    if (m == 0) f();\n    if (c <= 0) f();\n    g();\n"}
        def matches(bodies):
            body = bodies["k"]
            if "if (m == 0)" not in body or "if (c <= 0)" not in body or "int c = m->c;" not in body:
                return False
            return body.index("int c") < body.index("if (c <= 0)")

        self.assertEqual(reduce(initial, found, matches),
                         {"k": "\n    if (m == 0) f();\n    int c = m->c;\n    if (c <= 0) f();\n    g();\n"})

    def test_a_declaration_sinks_only_as_far_as_the_match_allows(self):
        # As in matdbl_sum: below the null test, but still above the line that first uses it.
        initial = {"k": "\n    validate(m);\n"}
        found = {"k": "\n    int c = m->c;\n    if (m == 0) f();\n    int r = m->r;\n    if (r <= 0 || c <= 0) f();\n"}

        needed = ["    if (m == 0) f();", "    int c = m->c;", "    int r = m->r;", "    if (r <= 0 || c <= 0) f();"]

        def matches(bodies):
            lines = bodies["k"].split("\n")
            return all(n in lines for n in needed) and lines.index(needed[1]) < lines.index(needed[2])

        self.assertEqual(reduce(initial, found, matches)["k"],
                         "\n    if (m == 0) f();\n    int c = m->c;\n    int r = m->r;\n    if (r <= 0 || c <= 0) f();\n")

    def test_parentheses_around_names_go_but_not_after_keywords_casts_or_calls(self):
        self.assertEqual(strip_parentheses("if ((m->rows) <= 0) f((x)); y = (double)z + (n);"),
                         "if (m->rows <= 0) f(x); y = (double)z + n;")

    def test_parentheses_stay_in_literals_template_casts_and_unchanged_lines(self):
        self.assertEqual(strip_parentheses('    g("(n) must"); x = static_cast<int>(y) + (z);'),
                         '    g("(n) must"); x = static_cast<int>(y) + z;')
        self.assertEqual(strip_parentheses("    a = (b);\n    c = (d);", {"    a = (b);"}), "    a = (b);\n    c = d;")

    def test_only_temporaries_the_search_added_fold_and_an_odd_one_does_not_crash(self):
        initial = {"k": "\n    auto t1 = m->x;\n    f(t1);\n"}
        found = {"k": "\n    auto t1 = m->x;\n    f(t1);\n    auto t0 = m->y; // note\n    g(t0);\n"}
        def matches(bodies):
            body = bodies["k"]
            return "g(" in body and ("g(t0)" not in body or "auto t0" in body)

        self.assertEqual(reduce(initial, found, matches), {"k": "\n    auto t1 = m->x;\n    f(t1);\n    g(m->y);\n"})


class ScoreTests(unittest.TestCase):
    def scorer(self, results, guard=()):
        """A Scorer whose comparison reports `results` (name -> (ok, unverified)) and similarity 0.5."""
        scorer = Scorer(None, {}, ["s"])
        scorer.guard = set(guard)
        comparison = SimpleNamespace(
            c=SimpleNamespace(image=SimpleNamespace(exports={n: 0 for n in results})),
            export=lambda n: SimpleNamespace(ok=results[n][0], unverified=results[n][1], reason="r"),
            similarity=lambda n: 0.5)
        scorer.comparison = lambda built: comparison
        return scorer

    def test_a_watched_export_whose_code_changes_without_matching_is_broken(self):
        # A shared body the search rewrote also feeds `w`, which did not match before: native_gate.py would fail it.
        scorer = self.scorer({"s": (True, 0), "w": (False, 0)})
        scorer.watched = {"w": "old code"}
        with mock.patch("native_permute.code_of", lambda candidate, name: "new code"):
            self.assertEqual(scorer(None)[0], -1)
            status, text = final_report(scorer, object())
        self.assertEqual(status, "NO MATCH")
        self.assertIn("BROKE w: its code changed", text)
        with mock.patch("native_permute.code_of", lambda candidate, name: "old code"):
            self.assertEqual(scorer(None)[0], 0)

    def test_breaking_a_guarded_export_ranks_below_any_progress(self):
        broke = self.scorer({"s": (True, 0), "g": (False, 0)}, ["g"])(None)
        kept = self.scorer({"s": (False, 0), "g": (True, 0)}, ["g"])(None)
        self.assertLess(broke, kept)
        self.assertFalse(broke[0] == 0)

    def test_the_report_tells_a_strict_match_from_an_unverified_one_and_a_break(self):
        self.assertEqual(final_report(self.scorer({"s": (True, 0)}), object())[0], "MATCH")
        self.assertEqual(final_report(self.scorer({"s": (True, 2)}), object())[0], "MATCH WITH UNVERIFIED TARGETS")
        status, text = final_report(self.scorer({"s": (True, 0), "g": (False, 0)}, ["g"]), object())
        self.assertEqual(status, "NO MATCH")
        self.assertIn("BROKE g", text)
        self.assertEqual(final_report(self.scorer({}), None)[0], "NO MATCH")


class FastBuildTests(unittest.TestCase):
    """The direct cl/link path: MSBuild's logged commands, and which sources an attempt recompiles."""

    def worker(self, files):
        root = Path(tempfile.mkdtemp())
        self.addCleanup(shutil.rmtree, root, ignore_errors=True)
        project = root / "src" / "P"
        project.mkdir(parents=True)
        (project / "P.vcxproj").write_text("<Project/>")
        for name, text in files.items():
            (project / name).write_text(text)
        worker = Worker(0, project, root / "work")
        (worker.obj / "P.tlog").mkdir(parents=True)
        a, b = str(worker.project / "a.cpp").upper(), str(worker.project / "b.cpp").upper()
        (worker.obj / "P.tlog" / "CL.command.1.tlog").write_bytes(
            f"^{a}\r\n/c /O2 /GL {a}\r\n^{b}\r\n/c /O2 /GL {b}\r\n".encode("utf-16"))
        (worker.obj / "P.tlog" / "link.command.1.tlog").write_bytes(
            '^A.OBJ|B.OBJ\r\n/OUT:"X.DLL" /LTCG "A.OBJ"\r\n"B.OBJ"\r\n'.encode("utf-16"))
        worker.enable_fast(("cl.exe", "link.exe", {}))
        worker.ran = []
        worker.run = lambda command, env, timeout: worker.ran.append(command) or 0
        worker.result = lambda: ("dll", "map")
        return worker, a, b

    def test_the_logged_commands_are_read_per_source_and_the_link_line_is_joined(self):
        worker, a, b = self.worker({"a.cpp": "", "b.cpp": "", "h.h": ""})
        compile_lines, link = logged_commands(worker.obj)
        self.assertEqual(compile_lines, {a: f"/c /O2 /GL {a}", b: f"/c /O2 /GL {b}"})
        self.assertEqual(link, '/OUT:"X.DLL" /LTCG "A.OBJ" "B.OBJ"')

    def test_only_a_changed_source_recompiles_and_a_changed_header_recompiles_all(self):
        worker, a, b = self.worker({"a.cpp": "", "b.cpp": "", "h.h": ""})
        worker.build({"a.cpp": "int x;"})
        self.assertEqual([c.split()[-1] for c in worker.ran], [a, '"B.OBJ"'])
        worker.ran.clear()
        worker.build({"a.cpp": "int x;"})  # unchanged: relink only
        self.assertEqual(len(worker.ran), 1)
        worker.ran.clear()
        worker.build({"h.h": "#define N 1"})
        self.assertEqual(sorted(c.split()[-1] for c in worker.ran[:-1]), sorted([a, b]))

    def test_a_build_cut_short_by_an_error_recompiles_next_time(self):
        worker, a, _ = self.worker({"a.cpp": "", "b.cpp": ""})

        def timeout(command, env, timeout):
            raise subprocess.TimeoutExpired(command, timeout)

        worker.run = timeout
        with self.assertRaises(subprocess.TimeoutExpired):
            worker.build({"a.cpp": "int x;"})
        worker.run = lambda command, env, timeout: worker.ran.append(command) or 0
        worker.build({"a.cpp": "int x;"})  # same text as the build that never compiled
        self.assertIn(a, worker.ran[0])

    def test_a_failed_compile_is_retried_on_the_next_build(self):
        worker, a, _ = self.worker({"a.cpp": "", "b.cpp": ""})
        worker.run = lambda command, env, timeout: worker.ran.append(command) or (1 if "/c" in command else 0)
        self.assertIsNone(worker.build({"a.cpp": "bad"}))
        worker.ran.clear()
        worker.run = lambda command, env, timeout: worker.ran.append(command) or 0
        worker.build({"a.cpp": "bad"})
        self.assertIn(a, worker.ran[0])


class MutateTests(unittest.TestCase):
    def test_every_rewrite_applies_somewhere_in_a_real_body(self):
        applied = {m.__name__ for m in MUTATIONS for seed in range(40) if m(body(), random.Random(seed))}
        self.assertTrue({"swap_operands", "flip_comparison", "hoist_member", "index_form", "compound_assignment",
                         "toggle_const", "retype_local", "merge_into_store", "loop_form", "move_declaration"} <= applied,
                        applied)

    def test_mutate_changes_the_body_or_returns_none(self):
        for seed in range(50):
            out = mutate(body(), random.Random(seed))
            self.assertNotEqual(out, body())


if __name__ == "__main__":
    unittest.main()
