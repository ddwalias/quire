#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    public object GetXamlObject(string fileName)
    {
        object result;
        try
        {
            global::System.Xml.XmlReader reader = global::System.Xml.XmlReader.Create(fileName);
            try
            {
                result = global::System.Windows.Markup.XamlReader.Load(reader);
            }
            finally
            {
                if (reader != null)
                {
                    ((global::System.IDisposable)reader).Dispose();
                }
            }
        }
        catch (global::System.Exception)
        {
            result = null;
        }

        return result;
    }

    private static void OnActiveDocumentChanged(
        global::System.Windows.DependencyObject dependencyObject,
        global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
        SpiritMainWindow window = dependencyObject as SpiritMainWindow;
        DebugTools.DevAssert(window != null, null);
        window.OnActiveDocumentChanged(e);
    }

    public static global::Quire.Controls.Core.IEngineeringDocument GetDocumentForTab(
        global::AvalonDock.Layout.LayoutAnchorable dockWindow)
    {
        if (dockWindow != null && dockWindow.Content is global::Quire.Controls.ScrollViewerGrid scrollViewerGrid)
        {
            return scrollViewerGrid.EngineeringDocumentInstance;
        }

        return null;
    }

    internal static global::AvalonDock.Layout.LayoutAnchorable GetTabForDocument(
        global::Quire.Controls.Core.IEngineeringDocument document)
    {
        return global::Quire.Wpf.WpfUtils.FindLogicalAncestor<global::AvalonDock.Layout.LayoutAnchorable>(
            (global::System.Windows.DependencyObject)(document is global::System.Windows.DependencyObject
                ? document
                : null));
    }

    private bool CanUseTemplateForNewDocument(string templateName, out bool isTemplateIncompatible)
    {
        isTemplateIncompatible = false;
        if (!DoesTemplateFileExists(templateName))
        {
            return false;
        }

        if (!global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable.TryGetFormatVersionFromFile(
                templateName,
                out global::System.Version formatVersion))
        {
            return false;
        }

        isTemplateIncompatible = !global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable
            .IsFormatVersionCompatible(formatVersion);
        return !isTemplateIncompatible;
    }

    private string ChooseDefaultTemplateIfAnyForNewDocument()
    {
        string templateName = string.Empty;
        if (CanGetDefaultBuiltInTemplateIfPossible(ref templateName, out bool isTemplateIncompatible))
        {
            return templateName;
        }

        if (!string.IsNullOrEmpty(templateName) &&
            !templateName.Equals(DefaultPathToDefaultTemplateForNewDocument))
        {
            if (isTemplateIncompatible)
            {
                SetAndDisplayWarning(
                    TryFindResource("Application.TemplateFileVersionNotCompatible") as string,
                    templateName);
            }
            else
            {
                SetAndDisplayWarning(
                    TryFindResource("Application.UserDefinedDefaultTemplateDoesNotExists") as string,
                    templateName);
            }
        }

        if (CanGetDefaultBuildInTemplateFromDefaultLocationIfPossible(ref templateName))
        {
            return templateName;
        }

        return null;
    }

    private void HandleGoToFind(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        global::Quire.Wpf.WpfUtils.TryFindVisualAncestor<SpiritMainWindow>(
            ActiveDocument as global::System.Windows.DependencyObject)?.FindBar.FindRichBox.Focus();
    }

    private void LaunchCustomMarginsDialog(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        if (DebugTools.DevAssert(args.Parameter is global::System.Windows.FrameworkElement, null).Failed)
        {
            return;
        }

        CustomMarginsDialog.ShowDialog(
            this,
            (global::System.Windows.FrameworkElement)args.Parameter,
            ActiveEngineeringDoc.Worksheet.PageLayout);
    }

    private void LaunchSystemUnitDialog(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        SystemUnitEditDialog.ShowDialog(
            this,
            args.Parameter as global::System.ComponentModel.ICollectionView,
            global::Quire.Controls.VisualCGSUnitDescriptorTable.Instance,
            global::Quire.Calculation.Core.DefaultUnitDescriptorTable.Instance);
    }

}
