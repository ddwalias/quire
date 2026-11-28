#nullable disable
using System;
using System.Collections.Generic;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Tree;

public partial class DefaultDescriptorTable
{
    private static partial class a_
    {
        public const int a = 10;
        public const int b = 10;
        public const int c = 11;
        public const int d = 20;
        public const int e = 20;
        public const int f = 21;
        public const int g = 30;
        public const int h = 40;
        public const int i = 41;
        public const int j = 50;
        public const int k = 51;
        public const int l = 70;
        public const int m = 71;
        public const int n = 72;
        private const int o = 80;
        public const int p = 80;
        public const int q = 80;
        public const int r = 80;
        public const int s = 80;
        public const int t = 80;
        public const int u = 80;
        public const int v = 81;
        public const int w = 90;
        public const int x = 90;
        public const int y = 100;
        public const int z = 100;
        public const int aa = 110;
        public const int ab = 110;
        public const int ac = 110;
        public const int ad = 110;
        public const int ae = 110;
        public const int af = 120;
        public const int ag = 120;
        public const int ah = 130;
        public const int ai = 130;
        public const int aj = 140;
        public const int ak = 140;
        public const int al = 141;
        public const int am = 142;
        public const int an = 143;
        public const int ao = 150;
        public const int ap = 150;
        public const int aq = 160;
        public const int ar = 160;
        public const int @as = 160;
        public const int at = 160;
        public const int au = 170;
        public const int av = 170;
        public const int aw = 180;
        public const int ax = 180;
        public const int ay = 180;
        public const int az = 180;
        public const int a0 = 180;
        public const int a1 = 190;
        public const int a2 = 190;
        public const int a3 = 200;
        public const int a4 = 200;
        public const int a5 = 200;
        public const int a6 = 200;
        public const int a7 = 200;
        public const int a8 = 290;
        public const int a9 = 290;
        public const int ba = 290;
        public const int bb = 290;
        public const int bc = 290;
        public const int bd = 290;
        public const int be = 299;
        public const int bf = 300;
        public const int bg = 300;
        public const int bh = 2147483647;
        public const int bi = 2147483647;
        public const int bj = 2147483647;
        public const int bk = 2147483647;
        public const int bl = 2147483647;
        public const int bm = 2147483647;
        public const int bn = 2147483647;
        public const int bo = 2147483647;
        public const int bp = 2147483647;
        public const int bq = 2147483647;
        public const int br = 2147483647;
        public const int bs = 2147483647;
        public const int bt = 2147483647;
        public const int bu = 2147483647;
        public const int bv = 2147483647;
    }

    private static partial class b_
    {
        public static System.Func<Quire.Native.PtcEquationEditor.r, Quire.EquationEditor.Tree.INodeInfo, Quire.EquationEditor.Tree.MathNode[], Quire.EquationEditor.Tree.MathNode> a;
        public static System.Func<Quire.Native.PtcEquationEditor.r, Quire.EquationEditor.Tree.INodeInfo, Quire.EquationEditor.Tree.MathNode[], Quire.EquationEditor.Tree.MathNode> b;
        public static System.Func<Quire.Native.PtcEquationEditor.r, Quire.EquationEditor.Tree.INodeInfo, Quire.EquationEditor.Tree.MathNode[], Quire.EquationEditor.Tree.MathNode> c;
    }

    partial class c_
    {
        [MirrorName("a")]
        public DefaultDescriptorTable a_;

        public List<string> b;

        [MirrorVerified("Release IL")]
        public c_() { }

        [MirrorStub]
        [MirrorBlocked("Mathcad calls get_Values with a non-virtual call on a field receiver, which Roslyn does not emit")]
        internal void a() => throw MirrorStub.NotImplemented();
    }
}
