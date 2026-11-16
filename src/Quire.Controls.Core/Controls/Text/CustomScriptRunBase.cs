#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Quire.Controls.Core;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Text;

public abstract partial class CustomScriptRunBase : Run, ICustomFontSizeOwner
{
    private static readonly Type TypeOfThis;

    public static readonly DependencyProperty StyleKeyProperty;

    public static readonly DependencyProperty WpfRecreateScriptsProperty;

    public static readonly DependencyProperty IsInSerializationProperty;

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    public static readonly DependencyProperty RevertCoerceAfterSerializationProcessProperty;

    public bool WpfRecreateScripts
    {
        get
        {
            return (bool)GetValue(WpfRecreateScriptsProperty);
        }
        internal set
        {
            if (value && WpfRecreateScripts)
            {
                DebugTools.FailAssertion("check re-entrance");
            }
            SetValue(WpfRecreateScriptsProperty, value);
        }
    }

    public bool IsInSerialization => (bool)GetValue(IsInSerializationProperty);

    public bool RevertCoerceAfterSerializationProcess => (bool)GetValue(RevertCoerceAfterSerializationProcessProperty);

    public void SetRevertCoerceAfterSerializationProcess()
    {
        SetValue(RevertCoerceAfterSerializationProcessProperty, true);
    }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    public void ClearRevertCoerceAfterSerializationProcess()
    {
        ClearValue(RevertCoerceAfterSerializationProcessProperty);
    }

    static CustomScriptRunBase()
    {
        TypeOfThis = typeof(CustomScriptRunBase);
        WpfRecreateScriptsProperty = DependencyProperty.RegisterAttached("WpfRecreateScripts", typeof(bool), TypeOfThis, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        IsInSerializationProperty = DependencyProperty.RegisterAttached("IsInSerialization", typeof(bool), typeof(CustomScriptRunBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
        {
            (o as CustomScriptRunBase)?.HandleAnySerializationChanged((bool)args.NewValue);
        }));
        RevertCoerceAfterSerializationProcessProperty = DependencyProperty.RegisterAttached("RevertCoerceAfterSerializationProcess", typeof(bool), typeof(CustomScriptRunBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        FontSizeForReportPropertyKey = DependencyProperty.RegisterReadOnly("FontSizeForReport", typeof(double), TypeOfThis, new PropertyMetadata((double)TextElement.FontSizeProperty.DefaultMetadata.DefaultValue));
        FontSizeForReportProperty = FontSizeForReportPropertyKey.DependencyProperty;
        TextElement.FontSizeProperty.OverrideMetadata(TypeOfThis, new FrameworkPropertyMetadata((double)TextElement.FontSizeProperty.DefaultMetadata.DefaultValue * CustomScriptsSettings.FontRatio, null, CoerceCustomScriptFontSizeCallback));
        Typography.VariantsProperty.OverrideMetadata(TypeOfThis, new FrameworkPropertyMetadata(Typography.VariantsProperty.DefaultMetadata.DefaultValue, null, CoerceVariantsProperty));
        FrameworkContentElement.DefaultStyleKeyProperty.OverrideMetadata(TypeOfThis, new FrameworkPropertyMetadata(TypeOfThis));
        StyleKeyProperty = TextElementAttachedProperties.StyleKeyProperty.AddOwner(TypeOfThis);
    }

    private void HandleAnySerializationChanged(bool inSerialization)
    {
        if (!inSerialization)
        {
            SetRevertCoerceAfterSerializationProcess();
        }
        CoerceValue(TextElement.FontSizeProperty);
        CoerceValue(Typography.VariantsProperty);
        ClearRevertCoerceAfterSerializationProcess();
    }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    protected CustomScriptRunBase()
    {
    }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    protected CustomScriptRunBase(string text)
        : base(text)
    {
    }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    protected CustomScriptRunBase(string text, TextPointer insertionPosition)
        : base(text, insertionPosition)
    {
    }

    private static object CoerceVariantsProperty(DependencyObject depObj, object value)
    {
        if (depObj is CustomScriptRunBase customScriptRunBase && customScriptRunBase.IsInSerialization)
        {
            return customScriptRunBase.FontVariantsForCopy;
        }
        return FontVariants.Normal;
    }

    protected abstract FontVariants FontVariantsForCopy { get; }

    private bool _deferInit;

    private static object CoerceCustomScriptFontSizeCallback(DependencyObject depObj, object value)
    {
        double num2;
        double num = (double)value;
        if (!(depObj is CustomScriptRunBase customScriptRunBase))
        {
            return value;
        }
        bool deferInit = customScriptRunBase._deferInit;
        customScriptRunBase._deferInit = false;
        ValueSource valueSource = DependencyPropertyHelper.GetValueSource(customScriptRunBase, TextElement.FontSizeProperty);
        if (customScriptRunBase.Parent == null && customScriptRunBase.ContentStart.DocumentStart.Parent == null)
        {
            customScriptRunBase.DataContextChanged += ScriptBaseRun_DataContextChanged;
            customScriptRunBase.FontSizeForReport = num;
            num2 = num * CustomScriptsSettings.FontRatio;
        }
        else if (customScriptRunBase.IsInSerialization && !customScriptRunBase.RevertCoerceAfterSerializationProcess)
        {
            num2 = customScriptRunBase.FontSizeForReport;
        }
        else if (customScriptRunBase.FontSizeToRestore.HasValue && DebugTools.DevAssert(EnvironmentScope.IsCurrentlyInsideUndoOrRedo, null).Passed)
        {
            customScriptRunBase.FontSizeForReport = customScriptRunBase.FontSizeToRestore.Value;
            num2 = customScriptRunBase.FontSizeToRestore.Value * CustomScriptsSettings.FontRatio;
        }
        else if (customScriptRunBase.RevertCoerceAfterSerializationProcess)
        {
            num2 = customScriptRunBase.FontSizeForReport * CustomScriptsSettings.FontRatio;
        }
        else if ((customScriptRunBase.WpfRecreateScripts && valueSource.IsCoerced) || (customScriptRunBase.WpfRecreateScripts && customScriptRunBase.HasLocalValue(TextElement.FontSizeProperty)))
        {
            customScriptRunBase.FontSizeForReport = num / CustomScriptsSettings.FontRatio;
            num2 = num;
        }
        else
        {
            if (deferInit && customScriptRunBase.DataContext is IRichTextBase richTextBase)
            {
                if (richTextBase.DequeueSplitFontSizeData() is double valueOrDefault)
                {
                    customScriptRunBase.FontSizeForReport = valueOrDefault;
                    num2 = customScriptRunBase.FontSizeForReport * CustomScriptsSettings.FontRatio;
                    goto CheckRatio;
                }
            }
            customScriptRunBase.FontSizeForReport = num;
            num2 = num * CustomScriptsSettings.FontRatio;
        }
        goto CheckRatio;

    CheckRatio:
        if (num2 < CustomScriptsSettings.FontRatio)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            num2 = CustomScriptsSettings.FontRatio;
        }
        return num2;
    }

    private static void ScriptBaseRun_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is CustomScriptRunBase customScriptRunBase && e.OldValue == null && e.NewValue != null)
        {
            customScriptRunBase._deferInit = true;
            customScriptRunBase.DataContextChanged -= ScriptBaseRun_DataContextChanged;
            customScriptRunBase.CoerceValue(TextElement.FontSizeProperty);
        }
    }

    private static readonly DependencyPropertyKey FontSizeForReportPropertyKey;

    public static readonly DependencyProperty FontSizeForReportProperty;

    public double FontSizeForReport
    {
        get
        {
            return (double)GetValue(FontSizeForReportProperty);
        }
        private set
        {
            SetValue(FontSizeForReportPropertyKey, value);
        }
    }

    [MirrorVerified("PtcControlsTextCustomScriptRunBaseMechanicalDifferentialTests")]
    public double? FontSizeToRestore { private get; set; }

    public static bool ToggleRangeText(System.Windows.Documents.TextRange range, ScriptState switchToScript, double reviveFontSize = double.NaN)
    {
        if (!(range.Start.DocumentStart.Parent is FlowDocument flowDocument && flowDocument.Parent is TextBoxBase parent))
        {
            return false;
        }
        using (parent.DeclareChangeBlock())
        {
            TextPointer positionAtOffset = range.Start.GetPositionAtOffset(0);
            int offsetToPosition = flowDocument.ContentStart.GetOffsetToPosition(range.Start);
            int offsetToPosition2 = flowDocument.ContentEnd.GetOffsetToPosition(range.End);
            if (positionAtOffset?.Parent is Run run && run.ContentStart.CompareTo(positionAtOffset) < 0)
            {
                CustomScriptServiceFunctions.SplitRun(positionAtOffset);
            }
            TextPointer positionAtOffset2 = flowDocument.ContentEnd.GetPositionAtOffset(offsetToPosition2);
            if (positionAtOffset2?.Parent is Run run2 && run2.ContentEnd.CompareTo(positionAtOffset2) > 0)
            {
                CustomScriptServiceFunctions.SplitRun(positionAtOffset2);
            }
            positionAtOffset2 = flowDocument.ContentEnd.GetPositionAtOffset(offsetToPosition2);
            positionAtOffset = flowDocument.ContentStart.GetPositionAtOffset(offsetToPosition);
            List<Run> list = new System.Windows.Documents.TextRange(positionAtOffset, positionAtOffset2).EnumerateRunsInRange().ToList();
            switch (switchToScript)
            {
                case ScriptState.Subscript:
                    list.Each((Run sourceRun) =>
                    {
                        CustomScriptServiceFunctions.ConvertScriptLevelInternal(sourceRun, typeof(SubscriptRun), reviveFontSize);
                    });
                    break;
                case ScriptState.None:
                    list.Where((Run run3) => run3 is ICustomFontSizeOwner).Each((Run sourceRun) =>
                    {
                        CustomScriptServiceFunctions.ConvertScriptLevelInternal(sourceRun, typeof(Run), reviveFontSize);
                    });
                    break;
                case ScriptState.Superscript:
                    list.Each((Run sourceRun) =>
                    {
                        CustomScriptServiceFunctions.ConvertScriptLevelInternal(sourceRun, typeof(SuperscriptRun), reviveFontSize);
                    });
                    break;
                default:
                    DebugTools.FailAssertion($"Handling to {switchToScript} should be added.");
                    break;
            }
        }
        return true;
    }

    public static void RecoverSubSuperscriptRunsInRange(System.Windows.Documents.TextRange range)
    {
        RecoverCustomScriptRunsInRange<SubscriptRun>(range, ShouldConvertToSubscript);
        RecoverCustomScriptRunsInRange<SuperscriptRun>(range, ShouldConvertToSuperscript);

        static bool ShouldConvertToSubscript(Run run)
        {
            if (run.BaselineAlignment != SubscriptRun.DefaultAlignment)
            {
                return run.Typography.Variants == FontVariants.Subscript;
            }
            return true;
        }
        static bool ShouldConvertToSuperscript(Run run)
        {
            if (run.BaselineAlignment != SuperscriptRun.DefaultAlignment)
            {
                return run.Typography.Variants == FontVariants.Superscript;
            }
            return true;
        }
    }

    internal static List<T> RecoverCustomScriptRunsInRange<T>(System.Windows.Documents.TextRange range, Func<Run, bool> shouldConvertPredicate) where T : CustomScriptRunBase
    {
        List<Run> list = range.EnumerateRunsInRange().Where(shouldConvertPredicate).ToList();
        SetSpanPropToRunIfNeeded(list);
        List<T> list2 = list.Select((Run run) => CustomScriptServiceFunctions.ConvertScriptLevelInternal(run, typeof(T), double.NaN)).Cast<T>().ToList();
        DebugTools.DevAssert(list.Count == list2.Count, null);
        return list2;
    }

    private static void SetSpanPropToRunIfNeeded(List<Run> runsToConvert)
    {
        foreach (Run item in runsToConvert)
        {
            if (!item.HasLocalValue(TextElement.FontSizeProperty) && item.Parent is Span span && span.HasLocalValue(TextElement.FontSizeProperty))
            {
                item.SetValue(TextElement.FontSizeProperty, span.FontSize);
            }
        }
    }
}
