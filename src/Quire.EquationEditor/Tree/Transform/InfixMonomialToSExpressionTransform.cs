#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree.Transform;

public partial class InfixMonomialToSExpressionTransform
{
    [MirrorName("a")]
    private const char a__ = '(';

    [MirrorName("b")]
    private const char b_ = ')';

    [MirrorName("c")]
    private const char c_ = '-';

    [MirrorName("d")]
    internal static readonly char[] d_;

    [MirrorName("e")]
    internal static readonly char[] e_;

    [MirrorName("f")]
    internal static readonly char[] f_;

    [MirrorName("g")]
    internal const string g_ = "([\\(\\)\\*/\\^])";

    [MirrorName("h")]
    private const string h_ = "(@NEG {0})";

    // Mathcad's compiler lowers the interpolation to string.Format.
    [MirrorName("i")]
    private readonly string i_ = string.Format("({0} {1} {{0}})", MathNodeToSExpressionTransform.IdLabelToken, IdLabels.Unit.ToString().ToUpper());

    [MirrorName("j")]
    private readonly Stack<string> j_ = new Stack<string>();

    [MirrorName("k")]
    private readonly Stack<string> k_ = new Stack<string>();

    private static InfixMonomialToSExpressionTransform l;

    private static InfixMonomialToSExpressionTransform m;

    private static Regex n;

    private readonly bool o;

    private InfixMonomialToSExpressionTransform(bool A_0)
    {
        o = A_0;
        n = DefaultDescriptorTable.Default["IDENTIFIER"].ValidValueRegex;
    }

    public static InfixMonomialToSExpressionTransform GetInstance(bool forceLabelFormat)
    {
        if (forceLabelFormat)
        {
            return m ?? (m = new InfixMonomialToSExpressionTransform(true));
        }
        return l ?? (l = new InfixMonomialToSExpressionTransform(false));
    }

    public string Transform(string expression)
    {
        string[] tokens = j(expression);
        int count = tokens.Count();
        bool ok = count > 0;
        int index = 0;
        while (ok && index < count)
        {
            string token = tokens[index];
            string next = index + 1 < count ? tokens[index + 1] : null;
            if (d(token))
            {
                if (next.IsNotEmpty() && g(next))
                {
                    k_.Push(token);
                }
                else
                {
                    ok = false;
                }
            }
            else if (g(token))
            {
                k_.Push(token);
                j_.Push(token);
            }
            else if (f(token))
            {
                bool foundOpen = false;
                while (ok && !k_.IsNullOrEmpty() && !(foundOpen = g(k_.Peek())))
                {
                    ok = a();
                }
                if (foundOpen)
                {
                    k_.Pop();
                    string top = j_.Pop();
                    if (!g(j_.Pop()))
                    {
                        ok = false;
                    }
                    j_.Push(top);
                }
                else
                {
                    ok = false;
                }
            }
            else if (h(token))
            {
                if (k_.IsNullOrEmpty() || a(token, k_.Peek()) > 0)
                {
                    k_.Push(token);
                }
                else
                {
                    while (ok && !k_.IsNullOrEmpty() && a(token, k_.Peek()) <= 0)
                    {
                        ok = a();
                    }
                    k_.Push(token);
                }
            }
            else if (a(token))
            {
                j_.Push(token);
            }
            else
            {
                ok = false;
            }
            index++;
        }
        while (ok && !k_.IsNullOrEmpty())
        {
            ok = a();
        }
        string result = ok ? k(j_.Peek()) : string.Empty;
        j_.Clear();
        k_.Clear();
        return result;
    }

    private bool a()
    {
        string op = k_.Pop();
        bool reduced;
        if (d(op))
        {
            if (j_.Count >= 1)
            {
                string operand = j_.Pop();
                if (!g(operand))
                {
                    operand = string.Format("(@NEG {0})", k(operand));
                    j_.Push(operand);
                    reduced = true;
                }
                else
                {
                    reduced = false;
                }
            }
            else
            {
                reduced = false;
            }
        }
        else if (h(op))
        {
            if (j_.Count >= 2)
            {
                string right = j_.Pop();
                string left = j_.Pop();
                if (!g(right) && !g(left))
                {
                    string item = string.Format("({0} {1} {2})", op, k(left), k(right));
                    j_.Push(item);
                    reduced = true;
                }
                else
                {
                    reduced = false;
                }
            }
            else
            {
                reduced = false;
            }
        }
        else if (g(op))
        {
            reduced = false;
        }
        else
        {
            DebugTools.FailAssertion("unexpected operator");
            reduced = false;
        }
        return reduced;
    }

    private static bool a(string A_0, out string A_1)
    {
        A_1 = A_0;
        if (A_0.Length <= 1 || !A_0.StartsWith("-"))
        {
            return false;
        }
        A_1 = A_0.Substring(1);
        return true;
    }

    private string k(string A_0)
    {
        bool negative = a(A_0, out string body);
        if (b(body) && o)
        {
            body = string.Format(i_, body);
        }
        if (!negative)
        {
            return body;
        }
        return string.Format("(@NEG {0})", body);
    }

    // Mathcad clears initlocals on this method.
    [System.Runtime.CompilerServices.SkipLocalsInit]
    internal string[] j(string A_0)
    {
        return Regex.Split(A_0, "([\\(\\)\\*/\\^])")
            .Where(a_.b_ ?? (a_.b_ = a_.a__.b))
            .Select(a_.c ?? (a_.c = a_.a__.a))
            .ToArray();
    }

    private static int i(string A_0)
    {
        if (!Enumerable.Contains(f_, A_0[0]))
        {
            if (!Enumerable.Contains(d_, A_0[0]))
            {
                return 2;
            }
            return 1;
        }
        return 0;
    }

    private static int? a(string A_0, string A_1)
    {
        if (DebugTools.DevAssert(c(A_0) && c(A_1), null).Passed)
        {
            return i(A_0) - i(A_1);
        }
        return null;
    }

    private static bool h(string A_0)
    {
        if (A_0.Length != 1)
        {
            return false;
        }
        if (!Enumerable.Contains(d_, A_0[0]))
        {
            return Enumerable.Contains(e_, A_0[0]);
        }
        return true;
    }

    private static bool g(string A_0)
    {
        if (A_0.Length != 1)
        {
            return false;
        }
        return A_0[0].Equals('(');
    }

    private static bool f(string A_0)
    {
        if (A_0.Length != 1)
        {
            return false;
        }
        return A_0[0].Equals(')');
    }

    private static bool e(string A_0)
    {
        if (!g(A_0))
        {
            return f(A_0);
        }
        return true;
    }

    private static bool d(string A_0)
    {
        if (A_0.Length != 1)
        {
            return false;
        }
        return A_0[0].Equals('-');
    }

    private static bool c(string A_0)
    {
        if (!h(A_0))
        {
            return e(A_0);
        }
        return true;
    }

    [MirrorStub]
    [MirrorBlocked("new char[7] {...} loads its data from Mathcad's obfuscated <PrivateImplementationDetails> container bs, which the gate does not pair with Quire's")]
    internal bool b(string A_0) => throw MirrorStub.NotImplemented();

    private bool a(string A_0)
    {
        a(A_0, out string body);
        if (!b(body))
        {
            return global::Quire.Native.PtcEquationEditor.e.a.IsMatch(body);
        }
        return true;
    }

    static InfixMonomialToSExpressionTransform()
    {
        d_ = new char[2] { '*', '/' };
        e_ = new char[1] { '^' };
        f_ = new char[2] { '(', ')' };
    }

    partial class a_
    {
        [MirrorName("a")]
        public static readonly a_ a__;
        [MirrorName("b")]
        public static Func<string, bool> b_;
        public static Func<string, string> c;

        static a_()
        {
            a__ = new a_();
        }

        [MirrorVerified("Release IL")]
        public a_() { }

        internal bool b(string A_0)
        {
            return !string.IsNullOrWhiteSpace(A_0);
        }

        [MirrorVerified("Release IL")]
        internal string a(string text) => text.Trim();
    }
}
