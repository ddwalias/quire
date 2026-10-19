#nullable disable
using System.Collections;
using System.Text;
using Quire.Mirror;

namespace Quire.Mcd;

/// <summary>
/// An Mpl type term, parsed from a signature such as <c>V{Length 60000 Mass 60000 Time-120000}</c>: named
/// constants (upper case) and variables (lower case), applications <c>F a b</c>, lists <c>[t]</c>, tuples
/// <c>(a, b)</c>, functions <c>a->b</c> and dimension products <c>{a 2 Length 60000}</c>. Behaviour follows mplrun
/// 12.0.1.0 member by member (docs/mirror/evidence/Mcd.Type.md).
/// </summary>
[Serializable]
public abstract partial class Type
{
    [MirrorTested("Mcd.Type")]
    protected Type() { }

    public abstract string Name { get; }

    /// <summary>The first character of <see cref="Name"/>, which tells the kind of term.</summary>
    [MirrorVerified("TypeDifferentialTests")]
    public char Root => Name[0];

    [MirrorVerified("TypeDifferentialTests")]
    public virtual bool IsImplicitArgument => false;

    /// <summary>Operand 0 is an application's operator, operand <c>i</c> its i-th argument; other terms throw.</summary>
    [MirrorVerified("TypeDifferentialTests")]
    public Type this[int operand]
    {
        get
        {
            Application application = (Application)this;
            if (operand == 0) return application.Operator;
            return application.Operands[operand - 1];
        }
        set
        {
            Application application = (Application)this;
            if (operand == 0) application.Operator = value;
            else application.Operands[operand - 1] = value;
        }
    }

    /// <summary>The slice <c>[start, start + count)</c>; the array itself when the slice is all of it.</summary>
    [MirrorVerified("TypeDifferentialTests")]
    private static Product.Power[] Take(Product.Power[] array, int start, int count)
    {
        if (start == 0 && count == array.Length) return array;
        var slice = new Product.Power[count];
        for (int i = 0; i < count; i++) slice[i] = array[start + i];
        return slice;
    }

    public abstract void Accept(Visitor _);

    /// <summary>
    /// The dimension product of a value type: under any number of <c>M</c> (matrix) wrappers, the first operand of
    /// an <c>F</c>, <c>R</c> or <c>V</c> application, a lone base becoming a one-power product (a variable one when
    /// the base is a type variable). A bare product is its own; anything else has none.
    /// </summary>
    [MirrorVerified("TypeDifferentialTests")]
    public Product GetDimensionTypeProduct()
    {
        Type type = this;
        while (true)
        {
            switch (type.Root)
            {
                case 'F':
                case 'R':
                case 'V':
                    type = type[1];
                    break;
                case 'M':
                    type = type[1];
                    continue;
                case '{':
                    return (Product)type;
                default:
                    return null;
            }
            break;
        }
        if (type is Product)
        {
            return (Product)type;
        }
        var powers = new Product.Power[1];
        powers[0].Base = type;
        powers[0].Exponent = 1;
        return new Product(powers, type is Variable ? 1 : 0);
    }

    /// <summary>The term in signature syntax.</summary>
    [MirrorVerified("TypeDifferentialTests")]
    public override string ToString()
    {
        var sb = new StringBuilder(256);
        Accept(new Printer(sb));
        return sb.ToString();
    }

    /// <summary>The term for display, with dimensions named by the calling thread's unit system.</summary>
    [MirrorVerified("TypeDifferentialTests")]
    public string Format()
    {
        var sb = new StringBuilder(256);
        Format(sb, RuntimeOptions.the_unit_system);
        return sb.ToString();
    }

    [MirrorVerified("TypeDifferentialTests")]
    public void Format(StringBuilder builder) => Format(builder, RuntimeOptions.the_unit_system);

    [MirrorVerified("TypeDifferentialTests")]
    public void Format(StringBuilder builder, UnitSystem system) => Accept(new Formatter(builder, system));

    [MirrorVerified("TypeDifferentialTests")]
    public static Type Parse(string signature)
    {
        var parser = new Parser(signature);
        return parser.Parse();
    }

    [Serializable]
    public abstract partial class Named : Type, IComparable
    {
        [MirrorVerified("TypeDifferentialTests")]
        public string m_nam;

        [MirrorTested("Mcd.Type")]
        protected Named() { }

        [MirrorVerified("TypeDifferentialTests")]
        public override string Name => m_nam;

        [MirrorVerified("TypeDifferentialTests")]
        public override int GetHashCode() => m_nam.GetHashCode();

        /// <summary>Same name; <paramref name="o"/> must be a <see cref="Named"/> (of either kind).</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override bool Equals(object o) => m_nam.Equals(((Named)o).m_nam);

        /// <summary>Culture-sensitive name order.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public virtual int CompareTo(object o) => m_nam.CompareTo(((Named)o).m_nam);
    }

    [Serializable]
    public partial class Constant : Named
    {
        [MirrorVerified("TypeDifferentialTests")]
        public Constant(string n) { m_nam = n; }

        /// <summary>The one-letter constants C, O, P, T, U and W.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override bool IsImplicitArgument => m_nam.Length == 1 && "COPTUW".IndexOf(m_nam) != -1;

        [MirrorVerified("TypeDifferentialTests")]
        public override void Accept(Visitor v) => v.Visit(this);
    }

    [Serializable]
    public partial class Variable : Named
    {
        [MirrorVerified("TypeDifferentialTests")]
        public Variable(string n) { m_nam = n; }

        [MirrorVerified("TypeDifferentialTests")]
        public override void Accept(Visitor v) => v.Visit(this);
    }

    [Serializable]
    public partial class Application : Type
    {
        [MirrorVerified("TypeDifferentialTests")]
        public Type Operator;
        [MirrorVerified("TypeDifferentialTests")]
        public Type[] Operands;

        [MirrorVerified("TypeDifferentialTests")]
        public Application(Type o, Type[] a)
        {
            Operator = o;
            Operands = a;
        }

        /// <summary>Applications of <c>I</c>.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override bool IsImplicitArgument => Operator.Name == "I";

        [MirrorVerified("TypeDifferentialTests")]
        public override string Name => Operator.Name;

        [MirrorVerified("TypeDifferentialTests")]
        public int Arity => Operands.Length;

        [MirrorVerified("TypeDifferentialTests")]
        public override void Accept(Visitor v) => v.Visit(this);
    }

    /// <summary>A product of powers: the first <c>v</c> have type-variable bases, the rest named dimensions.</summary>
    [Serializable]
    public partial class Product : Type
    {
        [MirrorVerified("TypeDifferentialTests")]
        public Power[] Powers;
        [MirrorVerified("TypeDifferentialTests")]
        private int m_var;
        [MirrorTested("Mcd.Type")]
        private int m_con;

        [MirrorVerified("TypeDifferentialTests")]
        public Product(Power[] p, int v)
        {
            Powers = p;
            m_var = v;
            m_con = p.Length - v;
        }

        [MirrorVerified("TypeDifferentialTests")]
        public override string Name => "{}";

        [MirrorVerified("TypeDifferentialTests")]
        public bool IsMonomorphic => m_var == 0;

        [MirrorVerified("TypeDifferentialTests")]
        public bool IsPolymorphic => m_var != 0;

        [MirrorVerified("TypeDifferentialTests")]
        public bool IsDimensionless => Powers.Length == 0;

        [MirrorVerified("TypeDifferentialTests")]
        public Power[] Variables => Take(Powers, 0, m_var);

        [MirrorVerified("TypeDifferentialTests")]
        public Power[] Constants => Take(Powers, m_var, m_con);

        [MirrorVerified("TypeDifferentialTests")]
        public override void Accept(Visitor v) => v.Visit(this);

        [Serializable]
        public partial struct Power
        {
            [MirrorVerified("TypeDifferentialTests")]
            public Type Base;
            [MirrorVerified("TypeDifferentialTests")]
            public int Exponent;
        }
    }

    public abstract partial class Visitor
    {
        [MirrorTested("Mcd.Type")]
        protected Visitor() { }

        [MirrorTested("Mcd.Type")]
        public void Traverse(Type t) => t.Accept(this);

        [MirrorTested("Mcd.Type")]
        public virtual void Visit(Constant _) { }

        [MirrorTested("Mcd.Type")]
        public virtual void Visit(Variable _) { }

        [MirrorTested("Mcd.Type")]
        public virtual void Visit(Application _) { }

        [MirrorTested("Mcd.Type")]
        public virtual void Visit(Product _) { }
    }

    /// <summary>Builds terms bottom-up on a stack of terms and boxed exponents.</summary>
    public partial class Factory
    {
        [MirrorTested("Mcd.Type")]
        private Stack m_stk = new(32);

        [MirrorVerified("TypeDifferentialTests")]
        public Factory() { }

        [MirrorVerified("TypeDifferentialTests")]
        public void Push(Type type) => m_stk.Push(type);

        /// <summary>A product power: the base, then its exponent.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public void Push(Type type, int exponent)
        {
            m_stk.Push(type);
            m_stk.Push(exponent);
        }

        [MirrorVerified("TypeDifferentialTests")]
        public Type Pop() => (Type)m_stk.Pop();

        /// <summary>The top <paramref name="types"/> terms, in the order they were pushed.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public Type[] Pop(int types)
        {
            var terms = new Type[types];
            for (int i = types - 1; i >= 0; i--) terms[i] = Pop();
            return terms;
        }

        [MirrorVerified("TypeDifferentialTests")]
        public Type NewConstant(string name) => new Constant(name);

        [MirrorVerified("TypeDifferentialTests")]
        public Type NewVariable(string name) => new Variable(name);

        /// <summary>The application, or <paramref name="type"/> itself when there are no operands.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public Type NewApplication(Type type, Type[] operands) =>
            operands == null || operands.Length == 0 ? type : new Application(type, operands);

        /// <summary>An application of the bottom one of the top <paramref name="types"/> terms to the others.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public Type NewApplication(int types)
        {
            var operands = Pop(types - 1);
            return new Application(Pop(), operands);
        }

        [MirrorVerified("TypeDifferentialTests")]
        public Type NewApplication(Type type, int operands) => NewApplication(type, Pop(operands));

        /// <summary>The unit constant <c>()</c>, the single term itself, or an application of <c>()</c>.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public Type NewTuple(int operands)
        {
            if (operands == 0) return NewConstant("()");
            if (operands == 1) return Pop();
            return NewApplication(NewConstant("()"), operands);
        }

        [MirrorVerified("TypeDifferentialTests")]
        public Type NewFunction(Type domain, Type range) => new Application(NewConstant(">"), [domain, range]);

        [MirrorVerified("TypeDifferentialTests")]
        public Type NewList(Type element) => new Application(NewConstant("[]"), [element]);

        /// <summary>A product of the top <c>variables + constants</c> (base, exponent) pairs, in pushed order.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public Product NewProduct(int variables, int constants)
        {
            var powers = new Product.Power[variables + constants];
            for (int i = powers.Length - 1; i >= 0; i--)
            {
                powers[i].Exponent = (int)m_stk.Pop();
                powers[i].Base = (Type)m_stk.Pop();
            }
            return new Product(powers, variables);
        }
    }

    /// <summary>
    /// Splits a signature into one-character token kinds: <c>C</c> (a name starting upper case), <c>a</c> (any
    /// other name), <c>i</c> (digits), <c>&gt;</c> (the arrow <c>-&gt;</c>), <c>$</c> (end) or the character itself.
    /// </summary>
    private partial class Lexer
    {
        [MirrorTested("Mcd.Type")]
        private string m_src;
        [MirrorTested("Mcd.Type")]
        private int m_off;

        [MirrorVerified("TypeDifferentialTests")]
        public Lexer(string signature)
        {
            m_src = signature + "$";
            m_off = 0;
        }

        [MirrorVerified("TypeDifferentialTests")]
        public char GetToken(out string lexeme)
        {
            char c = m_src[m_off];
            while (char.IsWhiteSpace(c))
            {
                c = m_src[++m_off];
            }
            int start = m_off;
            if (char.IsLetter(c))
            {
                while (char.IsLetterOrDigit(m_src[++m_off])) { }
                c = (char.IsUpper(c) ? 'C' : 'a');
            }
            else if (char.IsDigit(c))
            {
                while (char.IsDigit(m_src[++m_off])) { }
                c = 'i';
            }
            else if (c == '-')
            {
                if (m_src[++m_off] == '>')
                {
                    m_off++;
                    start++;
                    c = '>';
                }
            }
            else if (c != '$')
            {
                m_off++;
            }
            lexeme = m_src.Substring(start, m_off - start);
            return c;
        }
    }

    /// <summary>
    /// Recursive descent with one token of lookahead (<c>$</c> meaning none). Nothing checks that the input ends
    /// after the term, and a closing bracket is skipped whatever token stands there.
    /// </summary>
    private partial class Parser
    {
        [MirrorTested("Mcd.Type")]
        private Factory m_fac;
        [MirrorTested("Mcd.Type")]
        private Lexer m_lex;
        [MirrorTested("Mcd.Type")]
        private char m_tok;
        [MirrorTested("Mcd.Type")]
        private string m_val;

        [MirrorVerified("TypeDifferentialTests")]
        public Parser(string signature)
        {
            m_fac = new Factory();
            m_lex = new Lexer(signature);
            m_tok = '$';
            m_val = null;
        }

        /// <summary><c>application [-> type]</c>, the arrow associating to the right.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public Type Parse()
        {
            var domain = ParseApplication();
            if (GetNextTokenIf(">"))
            {
                return m_fac.NewFunction(domain, Parse());
            }
            return domain;
        }

        /// <summary>An atomic term followed by atomic operands while the next token can start one.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        private Type ParseApplication()
        {
            var head = ParseAtomic();
            int operands = 0;
            for (; IsNextToken("Ca{[("); operands++) m_fac.Push(ParseAtomic());
            return operands == 0 ? head : m_fac.NewApplication(head, operands);
        }

        [MirrorVerified("TypeDifferentialTests")]
        private Type ParseAtomic()
        {
            switch (GetNextToken())
            {
                case 'C':
                    return m_fac.NewConstant(m_val);
                case 'a':
                    return m_fac.NewVariable(m_val);
                case '[':
                    var element = Parse();
                    SkipToken(']');
                    return m_fac.NewList(element);
                case '{':
                    // variable powers come first, then constant ones
                    int variables = 0, constants = 0;
                    for (; IsNextToken("a"); variables++) m_fac.Push(ParseAtomic(), ParseExponent());
                    for (; IsNextToken("C"); constants++) m_fac.Push(ParseAtomic(), ParseExponent());
                    SkipToken('}');
                    return m_fac.NewProduct(variables, constants);
                case '(':
                    if (GetNextTokenIf(")")) return m_fac.NewTuple(0);
                    int count = 1;
                    m_fac.Push(Parse());
                    for (; GetNextTokenIf(","); count++) m_fac.Push(Parse());
                    SkipToken(')');
                    return m_fac.NewTuple(count);
                default:
                    throw new Exception("ParseAtomic: syntax error");
            }
        }

        /// <summary>An optional <c>-</c> and optional digits, 1 by default (a lone <c>-</c> is −1).</summary>
        [MirrorVerified("TypeDifferentialTests")]
        private int ParseExponent()
        {
            int exponent = 1;
            if (GetNextTokenIf("-")) exponent = -exponent;
            if (GetNextTokenIf("i")) exponent *= int.Parse(m_val);
            return exponent;
        }

        [MirrorVerified("TypeDifferentialTests")]
        private bool IsNextToken(string tokens)
        {
            if (m_tok == '$') m_tok = m_lex.GetToken(out m_val);
            return tokens.IndexOf(m_tok) != -1;
        }

        [MirrorVerified("TypeDifferentialTests")]
        private char GetNextToken()
        {
            if (m_tok != '$')
            {
                char tok = m_tok;
                m_tok = '$';
                return tok;
            }
            return m_lex.GetToken(out m_val);
        }

        [MirrorVerified("TypeDifferentialTests")]
        private bool GetNextTokenIf(string tokens)
        {
            if (IsNextToken(tokens))
            {
                m_tok = '$';
                return true;
            }
            return false;
        }

        [MirrorVerified("TypeDifferentialTests")]
        private void SkipToken(char token) => GetNextToken();
    }

    /// <summary>Writes a term back in signature syntax.</summary>
    private partial class Printer : Visitor
    {
        [MirrorTested("Mcd.Type")]
        protected StringBuilder m_out;

        [MirrorVerified("TypeDifferentialTests")]
        public Printer(StringBuilder builder) { m_out = builder; }

        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Constant t) => m_out.Append(t.Name);

        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Variable t) => m_out.Append(t.Name);

        /// <summary><c>(a, b)</c> for tuples, <c>a->b</c> for functions, <c>(F a b)</c> otherwise.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Application t)
        {
            switch (t.Root)
            {
                case '(':
                    bool first = true;
                    m_out.Append('(');
                    foreach (var operand in t.Operands)
                    {
                        if (first) first = false;
                        else m_out.Append(", ");
                        Traverse(operand);
                    }
                    m_out.Append(')');
                    break;
                case '>':
                    Traverse(t.Operands[0]);
                    m_out.Append("->");
                    Traverse(t.Operands[1]);
                    break;
                default:
                    m_out.Append('(');
                    Traverse(t.Operator);
                    foreach (var operand in t.Operands)
                    {
                        m_out.Append(' ');
                        Traverse(operand);
                    }
                    m_out.Append(')');
                    break;
            }
        }

        /// <summary><c>{base exponent ...}</c>, leaving out exponents of 1.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Product t)
        {
            bool first = true;
            m_out.Append('{');
            foreach (var power in t.Powers)
            {
                if (first) first = false;
                else m_out.Append(' ');
                Traverse(power.Base);
                if (power.Exponent != 1)
                {
                    m_out.Append(' ');
                    m_out.Append(power.Exponent);
                }
            }
            m_out.Append('}');
        }
    }

    /// <summary>
    /// Writes a term for display: type variables as <c>anyN</c>, value wrappers dropped, matrices in brackets,
    /// functions as <c>ƒ(a, b) → r</c>, and dimensions as the quantities of <paramref name="system"/>'s simplified units.
    /// </summary>
    private partial class Formatter : Printer
    {
        [MirrorTested("Mcd.Type")]
        private UnitSystem m_sys;
        [MirrorTested("Mcd.Type")]
        private Factory m_fac;

        [MirrorVerified("TypeDifferentialTests")]
        public Formatter(StringBuilder builder, UnitSystem system) : base(builder)
        {
            m_sys = system;
            m_fac = new Factory();
        }

        /// <summary><c>a</c>…<c>z</c> are any1…any26, <c>t</c>N is any(N + 26); other names print as they are.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Variable t)
        {
            if (t.Name.Length == 1)
            {
                m_out.Append("any");
                m_out.Append(t.Name[0] - 'a' + 1);
            }
            else if (t.Name[0] == 't' && char.IsDigit(t.Name[1]))
            {
                uint parsed = uint.Parse(t.Name.Substring(1));
                m_out.Append("any");
                m_out.Append(parsed + 26);
            }
            else
            {
                m_out.Append(t.Name);
            }
        }

        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Application t)
        {
            switch (t.Root)
            {
                case 'D' or 'N' or 'V':
                    Traverse(t.Operands[0]);
                    return;
                case 'M':
                    m_out.Append('[');
                    Traverse(t.Operands[0]);
                    m_out.Append(']');
                    return;
                case '(':
                    base.Visit(t);
                    return;
                case '>':
                    if (t.Operands[0].IsImplicitArgument) Traverse(t.Operands[1]);
                    else OnFunction(t, 0);
                    return;
            }
            Traverse(t.Operator);
            foreach (var operand in t.Operands)
            {
                m_out.Append(' ');
                Traverse(operand);
            }
        }

        /// <summary>Collects the curried domains of <paramref name="t"/> (<paramref name="n"/> so far), then writes
        /// them as one argument list followed by the final range.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        private void OnFunction(Type t, int n)
        {
            if (t.Root == '>')
            {
                var app = t as Application;
                m_fac.Push(app.Operands[0]);
                OnFunction(app.Operands[1], n + 1);
                return;
            }
            bool first = true;
            m_out.Append("ƒ(");
            var domains = m_fac.Pop(n);
            foreach (var domain in domains)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    m_out.Append(", ");
                }
                Traverse(domain);
            }
            m_out.Append(") → ");
            Traverse(t);
        }

        /// <summary><c>Unitless</c>, or the variable powers then the simplified units' quantities, joined by
        /// <c> · </c> with <c>^exponent</c> where it isn't 1.</summary>
        [MirrorVerified("TypeDifferentialTests")]
        public override void Visit(Product t)
        {
            if (t.IsDimensionless)
            {
                m_out.Append("Unitless");
                return;
            }
            bool first = true;
            foreach (var power in t.Variables)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    m_out.Append(" · ");
                }
                Traverse(power.Base);
                if (power.Exponent != 1)
                {
                    m_out.Append('^');
                    m_out.Append(power.Exponent);
                }
            }
            foreach (var power in m_sys.Simplify(t).Powers)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    m_out.Append(" · ");
                }
                m_out.Append(power.Base.Quantity);
                if (power.Exponent != 1)
                {
                    m_out.Append('^');
                    m_out.Append(power.Exponent.ToString());
                }
            }
        }
    }
}
