using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Tree;

public partial class MathNodeDescriptor
{
    private static readonly int[] a = new int[0];

    private readonly string b;
    private int c;
    private Fixity d;
    private int e;
    private int[] f = a;
    private int[] g = a;
    private bool h;
    private int[] i = a;
    private string[] j = new string[0];
    private Regex k;
    private Regex l;
    private bool m;
    private IValueCleaner n;
    private string o = string.Empty;
    private string p = string.Empty;
    private string q = string.Empty;
    private string r = string.Empty;
    private string s = string.Empty;
    private char? t;
    private SymbolMargin u;
    private bool v;
    private bool w;
    private bool x = true;
    private bool y;
    private bool z;
    private bool aa;
    private bool ab;
    private bool ac;
    private bool ad = true;
    private bool ae;
    private bool af;
    private bool ag;
    private bool ah;
    private readonly az ai = new az();
    private readonly az aj = new az();
    private readonly az ak = new az();
    private readonly az al = new az();
    private readonly az am = new az();
    private readonly az an = new az();
    private readonly az ao = new az();
    private readonly az ap = new az();
    private readonly az aq = new az();
    private readonly Dictionary<Rule, Panacea> ar = new Dictionary<Rule, Panacea>();
    private bool @as;
    private bool at;
    private bool au;
    private readonly List<DirectionalPair<Panacea, az>> av = new List<DirectionalPair<Panacea, az>>();
    private bool aw;
    private int? ax;
    private readonly Dictionary<int, string> ay = new Dictionary<int, string>();
    internal Func<Native.PtcEquationEditor.r, INodeInfo, MathNode[], MathNode> az;
    private bool a0;
    private OperatorFamily? a1;
    private Associativity a2;
    private bool a3;
    private bool a4;
    private bool a5;
    private bool a6;
    private bool a7 = true;
    private bool a8 = true;
    private int a9;
    private int[] ba = new int[0];
    private int? bb;
    private bool bc;
    private int bd;
    private bool be;
    private Direction[] bf;
    private int[] bg = a;
    private int[] bh = a;
    private int[] bi = a;
    private int[] bj = a;
    private string bk;
    private LineSpan bl;
    private NodeValueType bm;
    private bool bn;
    private bool bo;
    private bool bp;
    private MathNodeGroups? bq;
    private Type br;
    private bool bs;
    private bool bt;
    private string bu;
    private string bv;
    private string bw;
    private string bx;
    private string by;
    private bool bz;
    private string b0;
    private string b1;
    private readonly bool b2;

    public char? OperatorSymbol
    {
        get => t;
        set => t = value;
    }

    public SymbolMargin OperatorSymbolMargin
    {
        get => u;
        set => u = value;
    }

    public string OperatorSExpName
    {
        get => r;
        set => r = value ?? string.Empty;
    }

    public string OperatorVisualRepresentation
    {
        get => s;
        set => s = value ?? string.Empty;
    }

    public string SExpFormat
    {
        get => o;
        set => o = value ?? string.Empty;
    }

    public string RLFormat
    {
        get => p;
        set => p = value ?? string.Empty;
    }

    public string XMLFormat
    {
        get => q;
        set => q = value ?? string.Empty;
    }

    public string SplitOposite
    {
        get => bk;
        set => bk = value;
    }

    public LineSpan LineSpan
    {
        get => bl;
        set => bl = value;
    }

    public string[] Alternates
    {
        get => j;
        set => j = value ?? new string[0];
    }

    public NodeValueType ValueType
    {
        get => bm;
        set => bm = value;
    }

    public bool CarriesValue => !ValueType.Equals(NodeValueType.None);

    public bool HasCaretPositions
    {
        get => bn;
        set => bn = value;
    }

    public int ChildCount
    {
        get => c;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException();
            }

            c = value;
        }
    }

    public int[] ChildrenToRemoveOnDelete
    {
        get => i;
        set => i = value ?? a;
    }

    public int[] ContainerSlots
    {
        get => f;
        set => f = value ?? a;
    }

    public int[] ReadonlySlots
    {
        get => g;
        set => g = value ?? a;
    }

    public bool HasContainerSlot => f.Length != 0;

    public Associativity Associativity
    {
        get => a2;
        set => a2 = value;
    }

    public Fixity Fixity
    {
        get => d;
        set => d = value;
    }

    public virtual string DescriptorName => b;

    public int Precedence
    {
        get => e;
        set => e = value;
    }

    public bool IsFullContainer
    {
        get
        {
            for (int slot = 0; slot < ChildCount; slot++)
            {
                if (!IsContainerSlot(slot))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool AllChildSlotsAreContainerSlots
    {
        get => h;
        set => h = value;
    }

    public bool IgnoreOperandsOnOvertype
    {
        get => ah;
        set => ah = value;
    }

    public bool IsInfix => Fixity == Fixity.Infix;

    public bool IsNary
    {
        get => aa;
        set => aa = value;
    }

    public bool IsLinearNary
    {
        get => ab;
        set => ab = value;
    }

    public bool IsUnary
    {
        get
        {
            if (Fixity == Fixity.Prefix || Fixity == Fixity.Suffix || Fixity == Fixity.Outfix)
            {
                return !IsNary;
            }

            return false;
        }
    }

    public bool IsPlaceholder
    {
        get => v;
        set => v = value;
    }

    public bool IsEvaluatable
    {
        get => ac;
        set => ac = value;
    }

    public bool IsSkipGenerateVisual
    {
        get => bo;
        set => bo = value;
    }

    public virtual bool IsEvaluation
    {
        get => bp;
        set => bp = value;
    }

    public bool IsScopable
    {
        get => ad;
        set => ad = value;
    }

    public bool IsScopingBoundary
    {
        get => ae;
        set => ae = value;
    }

    public bool IsImplicit
    {
        get => a0;
        set => a0 = value;
    }

    public bool JumpThrough
    {
        get => af;
        set => af = value;
    }

    public bool RestrictsDescendants => ak.Count != 0;

    public bool ProhibitsDescendants => ao.Count != 0;

    public bool RestrictsAncestors => al.Count != 0;

    public Regex ValidValueRegex
    {
        get => k;
        set => k = value;
    }

    public Regex MatchRegex
    {
        get => l ?? k;
        set => l = value;
    }

    public bool BinOctHexModeOn
    {
        get => m;
        set => m = value;
    }

    public IValueCleaner ValueFixer
    {
        get => n;
        set => n = value;
    }

    public bool AlsoRemoveParent
    {
        get => w;
        set => w = value;
    }

    public bool HasVisualRepresentation
    {
        get => x;
        set => x = value;
    }

    public bool AllowCaretOnLeft
    {
        get => y;
        set => y = value;
    }

    public bool AllowCaretOnRight
    {
        get => z;
        set => z = value;
    }

    public bool DeleteParentInsteadOfNode
    {
        get => @as;
        set => @as = value;
    }

    public bool Temporal
    {
        get => ag;
        set => ag = true;
    }

    public bool MustScopeParent
    {
        get => at;
        set => at = value;
    }

    public bool RestrictFromNavigation
    {
        get => au;
        set => au = value;
    }

    public bool ShowOnlyWhenFocused
    {
        get => aw;
        set => aw = value;
    }

    public int? ReplaceWithChildOnDelete
    {
        get => ax;
        set => ax = value;
    }

    public bool NoLogicOnlyVisual
    {
        get => a6;
        set => a6 = value;
    }

    public bool PreFenceNavigatable
    {
        get => a7;
        set => a7 = value;
    }

    public bool PostFenceNavigatable
    {
        get => a8;
        set => a8 = value;
    }

    public Direction[] PrimalChildTypeoverExtendDirections
    {
        get => bf;
        set => bf = value ?? new Direction[0];
    }

    public IDictionary<int, string> DefaultNodeOverrides => ay;

    public OperatorFamily? Family
    {
        get => a1;
        set => a1 = value;
    }

    public MathNodeGroups? Group
    {
        get => bq;
        set => bq = value;
    }

    public bool DistinctNodeEdges
    {
        get => a3;
        set => a3 = value;
    }

    public bool ExplicitValue
    {
        get => a4;
        set => a4 = value;
    }

    public bool IsLiteral
    {
        get => a5;
        set => a5 = value;
    }

    public bool IsIdentifier => DescriptorName == "IDENTIFIER";

    public int NavigablePartCount
    {
        get => a9;
        set => a9 = value;
    }

    public int[] NavigablePartPositions
    {
        get => ba;
        set => ba = value;
    }

    public int? PrimalChildPosition
    {
        get => bb;
        set => bb = value;
    }

    public int InitialChildPosition
    {
        get => bd;
        set => bd = value;
    }

    public bool HasPanaceaux => ar.Count != 0;

    public Type InfoType
    {
        get => br;
        set => br = value;
    }

    public bool AllowVerticalNavigation
    {
        get => be;
        set => be = value;
    }

    public bool IsLeafOperator
    {
        get
        {
            if (ChildCount == 0 && !IsPlaceholder)
            {
                return !CarriesValue;
            }

            return false;
        }
    }

    public int[] InherentChildren
    {
        get => bg;
        set => bg = value ?? a;
    }

    public bool JumpIntoChildren
    {
        get => bs;
        set => bs = value;
    }

    public int[] NavigatablePartRows
    {
        get => bh;
        set => bh = value ?? a;
    }

    public int[] ChildrenRows
    {
        get => bi;
        set => bi = value ?? a;
    }

    public int[] ProhibitedAutoLabelIdDescendants => bj;

    public int RowCount
    {
        get
        {
            if (NavigatablePartRows.Length != 0 && ChildrenRows.Length != 0)
            {
                return Math.Max(NavigatablePartRows[NavigablePartCount - 1], ChildrenRows[ChildCount - 1]) + 1;
            }

            return 1;
        }
    }

    public bool IsPairNary
    {
        get => bt;
        set => bt = value;
    }

    public string FirstElementDescriptorName
    {
        get => bu;
        set => bu = value;
    }

    public string MiddleElementDescriptorName
    {
        get => bv;
        set => bv = value;
    }

    public string LastElementDescriptorName
    {
        get => bw;
        set => bw = value;
    }

    public string AlternateMiddleElementDescriptorName
    {
        get => bx;
        set => bx = value;
    }

    public string AlternateOperatorSExpName
    {
        get => by;
        set => by = value;
    }

    public bool IsPairNarySubElement
    {
        get => bz;
        set => bz = value;
    }

    public string ParentPairNaryDescriptorName
    {
        get => b0;
        set => b0 = value;
    }

    public string OperatorText
    {
        get => b1;
        set => b1 = value;
    }

    public bool IsBinOctHexModeOn => b2;

    // Mathcad's constructor has .initlocals False.
    [System.Runtime.CompilerServices.SkipLocalsInit]
    public MathNodeDescriptor(string name)
    {
        SplitOposite = null;
        b = name;
    }

    public void SetChildrenToRemoveOnDelete(params int[] childSlots)
    {
        ChildrenToRemoveOnDelete = childSlots;
    }

    public bool IsContainerSlot(int slotIndex)
    {
        return Array.IndexOf(f, slotIndex) >= 0;
    }

    public bool IsReadonlySlot(int slotIndex)
    {
        return Array.IndexOf(g, slotIndex) >= 0;
    }

    public bool IsChildAllowed(string childName, int childIndex)
    {
        bool allowed = true;
        if (aj.a(childIndex))
        {
            allowed &= aj.c(childName, childIndex);
        }

        if (an.a(childIndex))
        {
            allowed &= !an.c(childName, childIndex);
        }

        return allowed;
    }

    public void SetValueFixer<T>() where T : IValueCleaner, new()
    {
        ValueFixer = new T();
    }

    public bool AllowCaretOn(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return AllowCaretOnLeft;
            case Direction.Right:
                return AllowCaretOnRight;
            default:
                return false;
        }
    }

    public void SetContainerSlots(params int[] slots)
    {
        ContainerSlots = slots;
    }

    public void SetReadonlySlots(params int[] slots)
    {
        ReadonlySlots = slots;
    }

    public void SetPrimalChildTypeoverExtendDirections(params Direction[] directions)
    {
        PrimalChildTypeoverExtendDirections = directions;
    }
}
