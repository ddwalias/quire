using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetPasteHandler
{
    ResourceDictionary WorksheetMessages { get; }

    bool ClipboardContainsRecognizedFormat { get; }

    bool ClipboardContainsSingleEmbeddableItemForText { get; }

    IDictionary<IWhiteboardItem, Vector> ProcessClipboardContents(XmlLanguage proofingLanguage, TextFormat currentTextFormat);

    IWhiteboardItemInText ProcessSingleEmbeddableItemForTextInClipboard();
}
