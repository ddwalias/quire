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

public static partial class CustomScriptServiceFunctions
{
    public static ScriptState RunTypeToScriptState(Run runToCheck) => runToCheck switch
    {
        ISubscript => ScriptState.Subscript,
        ISuperscript => ScriptState.Superscript,
        _ => ScriptState.None
    };

    internal static Run ConvertScriptLevelInternal(Run sourceRun, Type targetType, double reviveFontSize)
    {
        if (DebugTools.DevAssert(typeof(Run).IsAssignableFrom(targetType), null).Failed)
        {
            return null;
        }
        Run run;
        TextBoxBase parent;
        FlowDocument flowDocument = sourceRun.ElementStart.DocumentStart.Parent as FlowDocument;
        if (flowDocument != null)
        {
            parent = flowDocument.Parent as TextBoxBase;
            if (parent != null)
            {
                using (parent.DeclareChangeBlock())
                {
                    run = Activator.CreateInstance(targetType, sourceRun.Text, sourceRun.ElementStart) as Run;
                    if (run != null)
                    {
                        CopyLocalFormattingValues(sourceRun, run);
                        if (!double.IsNaN(reviveFontSize))
                        {
                            run.FontSize = reviveFontSize;
                        }
                        run.Typography.Variants = FontVariants.Normal;
                        // Release IL requires non-optimized object initializer evaluation stack shape
                        _ = (new System.Windows.Documents.TextRange(sourceRun.ElementStart, sourceRun.ElementEnd)
                        {
                            Text = string.Empty
                        }) ?? null;
                    }
                }
                goto Done;
            }
        }
        run = Activator.CreateInstance(targetType, sourceRun.Text) as Run;
        if (run != null)
        {
            CopyLocalFormattingValues(sourceRun, run);
        }
    Done:
        return run;
    }

    private static void CopyLocalFormattingValues<T>(T sourceObj, T targetObj) where T : TextElement
    {
        foreach (DependencyProperty item in ((IEnumerable<DependencyProperty>)TextFormat.ClearFormatPropsList).Concat(new DependencyProperty[2]
        {
            TextElementAttachedProperties.StyleKeyProperty,
            FrameworkContentElement.TagProperty
        }).ToList())
        {
            object obj = ((DependencyObject)sourceObj).ReadLocalValue(item);
            if (item.Equals(TextElement.FontSizeProperty))
            {
                if (sourceObj is ICustomFontSizeOwner)
                {
                    obj = ((DependencyObject)sourceObj).ReadLocalValue(CustomScriptRunBase.FontSizeForReportProperty);
                }
                else if (sourceObj is Run run)
                {
                    IRichTextBase richTextBase = run.TryFindAncestorIncludingSelf<IRichTextBase>();
                    if (richTextBase != null && richTextBase.IsOnDrop)
                    {
                        obj = run.FontSize / CustomScriptsSettings.FontRatio;
                    }
                }
            }
            if (obj == null || !obj.Equals(DependencyProperty.UnsetValue))
            {
                ((DependencyObject)targetObj).SetValue(item, obj);
            }
        }
    }

    internal static Run SplitRun(TextPointer position)
    {
        DependencyObject parent = position.Parent;
        return parent switch
        {
            SubscriptRun => SplitRun<SubscriptRun>(position),
            SuperscriptRun => SplitRun<SuperscriptRun>(position),
            Run => SplitRun<Run>(position),
            _ => null
        };
    }

    private static T SplitRun<T>(TextPointer position) where T : Run
    {
        if (!(position.Parent is Run run) || position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd || position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
        {
            return null;
        }
        string text = new System.Windows.Documents.TextRange(position, run.ElementEnd).Text;
        T val = (T)Activator.CreateInstance(typeof(T), text);
        CopyLocalFormattingValues(run, val);
        run.SiblingInlines?.InsertAfter(run, val);
        position.DeleteTextInRun(text.Length);
        return val;
    }
}
