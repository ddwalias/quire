#nullable disable

using System;
using Quire.Mpl.Run;
using Xunit;

namespace Quire.Mpl.Tests.Run;

public class EnvTests
{
    private sealed class TestMplEnv : MplEnv
    {
        public override BindingInfo lookup(Sym s) => null;
    }

    private sealed class TestMacro : MplMacro
    {
        public TestMacro(Sym sym) : base(sym) { }
    }

    [Fact]
    public void Base_MplEnv_defaults_and_modifications()
    {
        var env = new TestMplEnv();
        Assert.Equal(0, env.Level);

        var sym = new Sym("x", null);
        var ex1 = Assert.Throws<AppException>(() => env.AddLocal(sym, 1u));
        Assert.Equal("May not modify import environment", ex1.Message);

        var ex2 = Assert.Throws<AppException>(() => env.AddLocalEquation(sym, 1u));
        Assert.Equal("May not modify import environment", ex2.Message);

        var rep = int_constant.NewTok(42, 0u);
        var ex3 = Assert.Throws<AppException>(() => env.AddSymbolMacro(sym, 1u, rep));
        Assert.Equal("May not modify import environment", ex3.Message);

        var ex4 = Assert.Throws<AppException>(() => env.AddFunctionMacro(sym, 1u, env, rep));
        Assert.Equal("May not modify import environment", ex4.Message);

        var ex5 = Assert.Throws<AppException>(() => env.AddForeignMacro(sym, 1u, "sig"));
        Assert.Equal("May not modify import environment", ex5.Message);

        var ex6 = Assert.Throws<AppException>(() => env.LoadForeignMacro("sig"));
        Assert.Equal("Can not resolve signature", ex6.Message);

        var ex7 = Assert.Throws<AppException>(() => MplEnv.CanNotResolve("foo"));
        Assert.Equal("Can not resolve signature", ex7.Message);

        var ex8 = Assert.Throws<AppException>(() => MplEnv.MayNotModify());
        Assert.Equal("May not modify import environment", ex8.Message);
    }

    [Fact]
    public void BindingInfo_properties_and_constructors()
    {
        var b0 = new BindingInfo();
        Assert.Null(b0.macro);

        var b1 = new BindingInfo(10u);
        Assert.Null(b1.macro);

        var b2 = new BindingInfo(10u, 5);
        Assert.Null(b2.macro);

        var sym = new Sym("m", null);
        var rep = int_constant.NewTok(1, 0u);
        var macro = new MplSymbolMacro(sym, rep);

        var mb = new MacroBindingInfo(20u, macro);
        Assert.Same(macro, mb.macro);
    }

    [Fact]
    public void LocalEnv_scoping_and_lookup()
    {
        var root = new LocalEnv();
        Assert.Equal(0, root.Level);

        var child = new LocalEnv(root);
        Assert.Equal(1, child.Level);

        var grandchild = new LocalEnv(child);
        Assert.Equal(2, grandchild.Level);

        var x = new Sym("x", null);
        var y = new Sym("y", null);
        var z = new Sym("z", null);

        // Add to root
        root.AddLocal(x, 100u);
        Assert.Throws<ArgumentException>(() => root.AddLocal(x, 999u));

        // AddLocalEquation allows overwrite
        root.AddLocalEquation(y, 200u);
        root.AddLocalEquation(y, 201u);

        // AddSymbolMacro
        var rep = int_constant.NewTok(123, 0u);
        root.AddSymbolMacro(z, 300u, rep);
        Assert.Throws<ArgumentException>(() => root.AddSymbolMacro(z, 301u, rep));

        // lookup_local
        Assert.NotNull(root.lookup_local(x));
        Assert.NotNull(root.lookup_local(y));
        Assert.NotNull(root.lookup_local(z));
        Assert.Null(root.lookup_local(new Sym("missing", null)));
        Assert.Null(child.lookup_local(x));

        // lookup with delegation to parent
        Assert.NotNull(child.lookup(x));
        Assert.NotNull(grandchild.lookup(x));
        Assert.NotNull(grandchild.lookup(z));

        // missing lookup on root throws NullReferenceException (parent is null)
        Assert.Throws<NullReferenceException>(() => root.lookup(new Sym("missing", null)));

        // LoadForeignMacro on child with base MplEnv parent calls parent.LoadForeignMacro, throwing AppException("Can not resolve signature")
        var baseParentChild = new LocalEnv(new TestMplEnv());
        var ex = Assert.Throws<AppException>(() => baseParentChild.LoadForeignMacro("foreign_sig"));
        Assert.Equal("Can not resolve signature", ex.Message);

        // LoadForeignMacro on child with LocalEnv root parent (whose parent is null) throws NullReferenceException
        Assert.Throws<NullReferenceException>(() => child.LoadForeignMacro("foreign_sig"));

        // LoadForeignMacro on root with null parent throws NullReferenceException
        Assert.Throws<NullReferenceException>(() => root.LoadForeignMacro("foreign_sig"));
    }

    [Fact]
    public void MplEnv_LoadMacro_dispatch()
    {
        var env = new LocalEnv();
        var key = new define_keyword();
        Assert.Null(env.LoadMacro(key));

        var x = new Sym("x", null);
        env.AddLocal(x, 10u);
        Assert.Null(env.LoadMacro(x));

        var m = new Sym("m", null);
        var macro = new MplSymbolMacro(m, int_constant.NewTok(42, 0u));
        env.AddSymbolMacro(m, 20u, macro.ExpandRef(env, new Tok(m, 0u)));
        Assert.NotNull(env.LoadMacro(m));

        Assert.Throws<NullReferenceException>(() => env.LoadMacro(new Sym("not_found", null)));
        Assert.Throws<NullReferenceException>(() => env.LoadMacro(null!));
    }

    [Fact]
    public void MplMacro_base_behaviour_and_exceptions()
    {
        var sym = new Sym("testMacro", null);
        var macro = new TestMacro(sym);

        var tok = new Tok(sym, 12u);
        var exRef = Assert.Throws<AppException>(() => macro.ExpandRef(null, tok));
        Assert.Equal("illegal macro reference", exRef.Message);
        Assert.Equal(12u, exRef.Handle);

        var form = new SCons(tok, null);
        var exCall = Assert.Throws<AppException>(() => macro.ExpandCall(null, form));
        Assert.Equal("illegal macro call", exCall.Message);
        Assert.Equal(12u, exCall.Handle);

        var exTop = Assert.Throws<AppException>(() => MplMacro.NotAtTopLevel(sym, 34u));
        Assert.Equal("function macro definition not at top level", exTop.Message);
        Assert.Equal(34u, exTop.Handle);

        var exExpand = Assert.Throws<AppException>(() => MplMacro.CanNotExpand(56u));
        Assert.Equal("Can not expand this macro in the current compilation unit.", exExpand.Message);
        Assert.Equal(56u, exExpand.Handle);
    }

    [Fact]
    public void MplSymbolMacro_expansion()
    {
        var sym = new Sym("mySym", null);
        var rep = int_constant.NewTok(77, 0u);
        var sm = new MplSymbolMacro(sym, rep);

        Assert.Same(rep, sm.ExpandRef(null, new Tok(sym, 0u)));

        var arg = int_constant.NewTok(1, 0u);
        var call = SCons.list2(new Tok(sym, 0u), arg);
        var expanded = sm.ExpandCall(null, call) as SCons;
        Assert.NotNull(expanded);
        Assert.Same(rep, expanded.head);
        Assert.Same(arg, expanded.second);

        // make
        var smMade = MplSymbolMacro.make(sym, 88);
        Assert.NotNull(smMade);

        var nonSymMade = MplSymbolMacro.make(new int_constant(1), 99);
        Assert.NotNull(nonSymMade);
        Assert.Null(nonSymMade.m_name);
    }
}
