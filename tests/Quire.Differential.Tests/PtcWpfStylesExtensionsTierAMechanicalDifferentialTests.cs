using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Quire.Differential.Tests;

public class PtcWpfStylesExtensionsTierADifferentialTests
{
    [SkippableFact]
    public void GetSetterValue_060005a8()
    {
        Mathcad.Require();

        var originalAssembly = Mathcad.Load("Ptc.Core");
        var originalStyleType = Mathcad.Type("PresentationFramework", "System.Windows.Style");
        var originalSetterType = Mathcad.Type("PresentationFramework", "System.Windows.Setter");
        var originalTextElementType = Mathcad.Type("PresentationFramework", "System.Windows.Documents.TextElement");
        var originalSetterCollectionType = Mathcad.Type("PresentationFramework", "System.Windows.SetterBaseCollection");
        var originalSetterCtor = originalSetterType.GetConstructor([Mathcad.Type("WindowsBase", "System.Windows.DependencyProperty"), typeof(object)])!;
        var originalAdd = originalSetterCollectionType.GetMethod("Add", [originalSetterType.BaseType!])!;
        var originalGetter = (MethodInfo)originalAssembly.ManifestModule.ResolveMethod(0x060005a8)!;
        var quireGetter = typeof(global::Quire.Wpf.StylesExtensions).GetMethod("GetSetterValue")!;

        var qStyle = new Style(typeof(TextBlock));
        var mStyle = Activator.CreateInstance(originalStyleType, Mathcad.Type("PresentationFramework", "System.Windows.Controls.TextBlock"))!;
        object qFontSize = typeof(TextElement).GetField("FontSizeProperty", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
        object mFontSize = originalTextElementType.GetField("FontSizeProperty", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
        object qFontWeight = typeof(TextElement).GetField("FontWeightProperty", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
        object mFontWeight = originalTextElementType.GetField("FontWeightProperty", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;

        qStyle.Setters.Add(new Setter((DependencyProperty)qFontSize, 16.0));
        qStyle.Setters.Add(new Setter((DependencyProperty)qFontSize, 24.0));
        object mSetters = originalStyleType.GetProperty("Setters")!.GetValue(mStyle)!;
        originalAdd.Invoke(mSetters, [originalSetterCtor.Invoke([mFontSize, 16.0])]);
        originalAdd.Invoke(mSetters, [originalSetterCtor.Invoke([mFontSize, 24.0])]);

        Compare("last setter wins", qStyle, qFontSize, mStyle, mFontSize, quireGetter, originalGetter);
        Compare("missing property returns null", qStyle, qFontWeight, mStyle, mFontWeight, quireGetter, originalGetter);
    }

    private static void Compare(string caseId, Style qStyle, object qProperty, object mStyle, object mProperty,
        MethodInfo qMethod, MethodInfo mMethod)
    {
        MethodComparison.Compare(
            caseId,
            "Ptc.Wpf.StylesExtensions",
            qMethod, null, [qStyle, qProperty],
            mMethod, null, [mStyle, mProperty],
            Mechanical.Show, Mechanical.Show);
    }
}
