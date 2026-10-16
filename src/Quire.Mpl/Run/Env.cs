#nullable disable

using System;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// Evaluation environment for MPL macros and local bindings (docs/mirror/evidence/Mpl.Run.MplEnv.md).
/// </summary>
public abstract partial class MplEnv
{
    public virtual int Level => 0;

    public virtual MplEnv AddLocal(Sym s, uint tag)
    {
        MayNotModify();
        return null;
    }

    public virtual MplEnv AddLocalEquation(Sym s, uint tag)
    {
        MayNotModify();
        return null;
    }

    public virtual BindingInfo AddSymbolMacro(Sym s, uint tag, Sexpr rep)
    {
        MayNotModify();
        return null;
    }

    public virtual MplEnv AddFunctionMacro(Sym s, uint tag, MplEnv env, Sexpr rep)
    {
        MayNotModify();
        return null;
    }

    public virtual BindingInfo AddForeignMacro(Sym s, uint tag, string sig)
    {
        MayNotModify();
        return null;
    }

    public abstract BindingInfo lookup(Sym s);

    public MplMacro LoadMacro(Sym s)
    {
        if (s is Keyword)
        {
            return null;
        }
        return lookup(s)?.macro;
    }

    public virtual MplMacro LoadForeignMacro(string foreign_id)
    {
        CanNotResolve(foreign_id);
        return null;
    }

    public static void CanNotResolve(string sig)
    {
        throw new AppException("Can not resolve signature");
    }

    public static void MayNotModify()
    {
        throw new AppException("May not modify import environment");
    }

    protected MplEnv()
    {
    }
}

/// <summary>
/// Represents a symbol binding record in an environment (docs/mirror/evidence/Mpl.Run.BindingInfo.md).
/// </summary>
public partial class BindingInfo
{
    private uint m_handle;
    private int m_zone;

    public virtual MplMacro macro => null;

    public BindingInfo()
        : this(0u, 0)
    {
    }

    public BindingInfo(uint handle, int zone)
    {
        m_handle = handle;
        m_zone = zone;
    }

    public BindingInfo(uint handle)
        : this(handle, 0)
    {
    }
}

/// <summary>
/// A binding record specifically wrapping a macro definition.
/// </summary>
public partial class MacroBindingInfo : BindingInfo
{
    private MplMacro m_macro;

    public override MplMacro macro => m_macro;

    public MacroBindingInfo(uint handle, MplMacro m)
        : base(handle, 1)
    {
        m_macro = m;
    }
}

/// <summary>
/// Scoped local environment for symbol lookup and macro expansion (docs/mirror/evidence/Mpl.Run.LocalEnv.md).
/// </summary>
public partial class LocalEnv : MplEnv
{
    private int m_level;
    private MplEnv m_parent;
    private SortedDictionary<string, BindingInfo> m_table;

    public override int Level => m_level;

    public LocalEnv(MplEnv parent)
    {
        m_level = parent.Level + 1;
        m_parent = parent;
        m_table = new SortedDictionary<string, BindingInfo>();
    }

    public LocalEnv()
    {
        m_level = 0;
        m_parent = null;
        m_table = new SortedDictionary<string, BindingInfo>();
    }

    public BindingInfo lookup_local(Sym s)
    {
        m_table.TryGetValue(s.name, out var value);
        return value;
    }

    public override BindingInfo lookup(Sym s)
    {
        BindingInfo bindingInfo = lookup_local(s);
        if (bindingInfo != null)
        {
            return bindingInfo;
        }
        return m_parent.lookup(s);
    }

    public override MplMacro LoadForeignMacro(string sig)
    {
        return m_parent.LoadForeignMacro(sig);
    }

    public override MplEnv AddLocal(Sym s, uint handle)
    {
        m_table.Add(s.name, new BindingInfo(handle));
        return this;
    }

    public override MplEnv AddLocalEquation(Sym s, uint handle)
    {
        m_table[s.name] = new BindingInfo(handle);
        return this;
    }

    public override BindingInfo AddSymbolMacro(Sym s, uint handle, Sexpr e)
    {
        BindingInfo bindingInfo = new MacroBindingInfo(handle, new MplSymbolMacro(s, e));
        m_table.Add(s.name, bindingInfo);
        return bindingInfo;
    }
}

/// <summary>
/// Base class for MPL macro definitions (docs/mirror/evidence/Mpl.Run.MplMacro.md).
/// </summary>
public abstract partial class MplMacro
{
    public Sym m_name;

    public MplMacro(Sym sym)
    {
        m_name = sym;
    }

    public virtual Sexpr ExpandRef(MplEnv e, Tok s)
    {
        return IllegalMacroRef(s);
    }

    public virtual Sexpr ExpandCall(MplEnv e, Sexpr form)
    {
        return IllegalMacroCall(form);
    }

    public Sexpr IllegalMacroCall(Sexpr form)
    {
        throw new AppException("illegal macro call", 0u, form.handle);
    }

    public Sexpr IllegalMacroRef(Tok form)
    {
        throw new AppException("illegal macro reference", 0u, form.handle);
    }

    public static void NotAtTopLevel(Sym s, uint handle)
    {
        throw new AppException("function macro definition not at top level", 0u, handle);
    }

    public static void CanNotExpand(uint handle)
    {
        throw new AppException("Can not expand this macro in the current compilation unit.", 0u, handle);
    }
}

/// <summary>
/// MPL symbol macro that replaces references or call heads with an expression (docs/mirror/evidence/Mpl.Run.MplSymbolMacro.md).
/// </summary>
public partial class MplSymbolMacro : MplMacro
{
    private Sexpr m_replacement;

    public MplSymbolMacro(Sym s, Sexpr rep)
        : base(s)
    {
        m_replacement = rep;
    }

    public override Sexpr ExpandRef(MplEnv e, Tok s)
    {
        return m_replacement;
    }

    public override Sexpr ExpandCall(MplEnv e, Sexpr f)
    {
        SCons sCons = f as SCons;
        return new SCons(m_replacement, sCons.tail);
    }

    public static MplMacro make(Atom s, int n)
    {
        return new MplSymbolMacro(s as Sym, int_constant.NewTok(n, 0u));
    }
}

/// <summary>
/// MPL quote macro that quotes its single argument: (quote x) -> 'x (docs/mirror/evidence/Mpl.Run.MplQuoteMacro.md).
/// </summary>
public partial class MplQuoteMacro : MplMacro
{
    [MirrorVerified("EnvDifferentialTests")]
    public static MplQuoteMacro singleton;

    private MplQuoteMacro() : base(new Sym("quote", null)) { }

    static MplQuoteMacro()
    {
        singleton = new MplQuoteMacro();
    }

    [MirrorVerified("EnvDifferentialTests")]
    public override Sexpr ExpandCall(MplEnv e, Sexpr f)
    {
        SCons scons = f as SCons;
        if (scons.Length != 2) IllegalMacroCall(scons);
        return new Qt(scons.second);
    }
}
