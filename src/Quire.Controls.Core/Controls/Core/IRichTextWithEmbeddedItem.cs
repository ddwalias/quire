#nullable disable
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Quire.Controls.Core.BookmarkTags;

namespace Quire.Controls.Core;

public partial interface IRichTextWithEmbeddedItem
{
    IEnumerable<object> EmbeddedObjects { get; }
    Rect FlowDocumentRect { get; }
    IEnumerable<TextPointer> Lines { get; }
    IBookmarksManager BookmarksManager { get; set; }
    void AddEmbeddedItem(IWhiteboardItemInText item, bool activate);
    void AddEmbeddedItem(IWhiteboardItemInText item, TextPointer textPointer = default);
    void RemoveEmbeddedItem(IWhiteboardItemInText item);
    int GetEmbeddedItemSymbolOffset(IWhiteboardItemInText item);
    void PasteItem(IWhiteboardItemInText item);
    void ReplacePlaceholderWithEmbeddedItem(IWhiteboardItemInText item, int symbolOffset);
    int GetEmbeddedObjectCaretOffset(object embeddedObject);
    IRichTextWithEmbeddedItem CloneRichText(IEngineeringDocument engineeringDocument);
    bool HasHyperlinkInMouseContext(object context);
    void CopyWholeRangeXamlToDataObject(IDataObject dataObject);
}
