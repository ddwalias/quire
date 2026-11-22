#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Undo;

namespace Quire.Controls.Text;

public partial class Paragraph
{
    private double _innerWidth;

    private Size PageSize => WorksheetPropertiesManager.GetWorksheetProperties(this).PageSizeValue;

    private Thickness PageMargin => WorksheetPropertiesManager.GetWorksheetProperties(this).PageMargin;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public double InnerWidth
    {
        get
        {
            if (_innerWidth == 0.0)
            {
                _innerWidth = MinWidth;
            }
            return _innerWidth;
        }
        set
        {
            if (_innerWidth != value)
            {
                _innerWidth = value;
                FirePropertyChanged(new PropertyChangedEventArgs("InnerWidth"));
            }
        }
    }

    public bool NotHookupSizeChangedHandlerUntilLoaded { get; set; }

    static Paragraph()
    {
        _typeOfThis = typeof(Paragraph);
        _propertiesNotToSerialize = new string[4] { "Children", "Name", "Template", "Clip" };
        WorksheetPropertiesManager.RegisterToWorksheetPropertyNotifications(_typeOfThis);
        Canvas.LeftProperty.OverrideMetadata(typeof(Paragraph), new FrameworkPropertyMetadata(CanvasLeftChanged, CoerceCanvasLeft));
    }

    public Paragraph()
    {
        InitializeComponent();
    }

    protected override IUndoableCommand CreateTextChangedCommand(TextChangedEventArgs args)
    {
        bool oldProhibit = false;
        Action resetSizeChangedHandler = () =>
        {
            oldProhibit = AttachedProperties.GetProhibitSizeChangedEventHandler(this);
            if (!oldProhibit)
            {
                AttachedProperties.SetProhibitSizeChangedEventHandler(this, prohibit: true);
            }
        };
        Action restoreSizeChangedHandler = () =>
        {
            UpdateLayout();
            if (!oldProhibit)
            {
                AttachedProperties.ClearProhibitSizeChangedEventHandler(this);
            }
        };
        IUndoableCommand textChangedCmd = base.CreateTextChangedCommand(args);
        return new UndoableCommand("Paragraph Text changed", () => true, () =>
        {
            resetSizeChangedHandler();
            textChangedCmd.Undo();
            FireSplitItemIfNeeded();
            restoreSizeChangedHandler();
        }, () =>
        {
            resetSizeChangedHandler();
            textChangedCmd.Redo();
            FireSplitItemIfNeeded();
            restoreSizeChangedHandler();
        });
    }

    // Declared after CreateTextChangedCommand to preserve closure ordinals 13_0 and 14_x.
    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty CanvasLeftProperty;

    private static readonly string[] _propertiesNotToSerialize;

    private static object CoerceCanvasLeft(DependencyObject d, object basevalue)
    {
        DebugTools.DevAssert(basevalue is double, null);
        double num = (double)basevalue;
        if (!num.Equals(0.0))
        {
            num = 0.0;
        }
        return num;
    }

    private static void CanvasLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (_propertiesNotToSerialize.Contains(dp.Name))
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    public override void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        base.OnWorksheetPropertiesChanged(propertyNames, oldProperties, newProperties);
        if (propertyNames.Contains("PageSizeValue") || propertyNames.Contains("PageMargin"))
        {
            InnerWidth = WorksheetPageLayout.ContentAreaSize(PageSize, PageMargin).Width;
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
    }

    [Conditional("DEBUG")]
    private void LogSizeChange(SizeChangedInfo sizeInfo)
    {
        // Mathcad's Release build has an unreachable debug-logging body after return,
        // leaving .maxstack 8, .initlocals True, and these exact 7 local slots.
        return;
#pragma warning disable CS0162
        FrameworkElement element = null;
        Size size = default;
        int index = 0;
        double value = 0.0;
        System.IO.FileStream stream = null;
        DateTime timestamp = default;
        long ticks = 0L;
        if (index != 0)
        {
            index = index + (index + (index + (index + (index + (index + (index + index))))));
            Console.WriteLine(element);
            Console.WriteLine(size);
            Console.WriteLine(value);
            Console.WriteLine(stream);
            Console.WriteLine(timestamp);
            Console.WriteLine(ticks);
        }
#pragma warning restore CS0162
    }

    public override double VerticalAlignmentOffset => 0.0;

    protected override textType DoGetRegionPersistentDataInternal(IS11NProvider s11NProvider)
    {
        textType textType2 = base.DoGetRegionPersistentDataInternal(s11NProvider);
        textType2.istextblock = true;
        return textType2;
    }

    public event Action HookupSizeChangedHandler = () => { };

    public void FireHookupSizeChangedHandler()
    {
        HookupSizeChangedHandler();
    }

    public override object Clone()
    {
        return CloneParagraph();
    }

    public Paragraph CloneParagraph()
    {
        Paragraph paragraph = new Paragraph();
        paragraph.CloneContent(this, null);
        return paragraph;
    }

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Text;component/paragraph.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
