using System;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Core;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPasteHandler
{
    public ResourceDictionary WorksheetMessages => _resourceDictionary;

    private static IWorksheetItemManager ItemManager => WorksheetViewModel.ItemManager;

    private static IControlFactory ControlCreator => WorksheetViewModel.ControlCreator;

    public bool ClipboardContainsRecognizedFormat
    {
        get
        {
            try
            {
                IClipboard current = WpfClipboard.Current;
                return current.HasPrimeObject() || current.HasSExpression() || current.HasXMLSpreadsheet() || current.HasText() || current.HasImage();
            }
            catch
            {
                return false;
            }
        }
    }

    public bool ClipboardContainsSingleEmbeddableItemForText => WpfClipboard.Current.HasSExpression();

    private string ErrorResourceKeyToMessage(string errorResourceKey)
    {
        if (!_resourceDictionary.Contains(errorResourceKey))
        {
            return errorResourceKey;
        }
        return _resourceDictionary[errorResourceKey].ToString();
    }
}
