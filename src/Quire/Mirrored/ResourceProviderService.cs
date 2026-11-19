#nullable disable

using System.Windows;
using Quire.Wpf;

namespace Quire;

public partial class ResourceProviderService
{
    public bool IsConnected { get; set; }

    public ResourceDictionary RichTextContextMenuItemsDictionary => ResourceProvider<RichTextContextMenuItemsDictionary>.Instance;

    public ResourceProviderService() { }
}
