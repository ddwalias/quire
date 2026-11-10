using System.IO;
using System.Windows;
using System.Windows.Media;
using Quire.Core;

namespace Quire.Controls.ExcelComponent;

public partial class OleObjectControl
{
    public bool IsEmpty => false;

    public bool PreserveAspectRatio => OleObjectAutoResize;

    public FrameworkElement GetAdornedElement()
    {
        return View;
    }

    public virtual string GetHelpContext()
    {
        return "OleObjectControl";
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+SnapToPixelHelper, Ptc.Controls.ExcelComponent")]
    private partial class SnapToPixelHelper : ISnapToPixelHelper
    {
        private readonly OleObjectControl _oleObject;

        public SnapToPixelHelper(OleObjectControl oleObject)
        {
            _oleObject = oleObject;
        }

        public GeneralTransform TransformFromPoint
        {
            get
            {
                return this.CreateTransformFromChildPoint(_oleObject.View.SnappingHelper);
            }
        }

        public bool HasPointThatSnapsToPixel
        {
            get
            {
                return _oleObject.View.HasPointThatSnapsToPixel();
            }
        }

        public UIElement ItemElement
        {
            get
            {
                return _oleObject;
            }
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+CreationMethodWithDialog, Ptc.Controls.ExcelComponent")]
    public partial class CreationMethodWithDialog : OleObjectControlCreationMethod
    {
        public CreationMethodWithDialog(Window winHandle)
        {
            WinHandle = winHandle;
        }

        public Window WinHandle { get; private set; }

        public override bool Visit(OleObjectControl control, string oleContainerPath)
        {
            return control.Accept(this, oleContainerPath);
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+CreationMethodWithProgId, Ptc.Controls.ExcelComponent")]
    public partial class CreationMethodWithProgId : OleObjectControlCreationMethod
    {
        public CreationMethodWithProgId(string progId)
        {
            ProgId = progId;
        }

        public string ProgId { get; private set; }

        public override bool Visit(OleObjectControl control, string oleContainerPath)
        {
            return control.Accept(this, oleContainerPath);
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+CreationMethodFromFile, Ptc.Controls.ExcelComponent")]
    public partial class CreationMethodFromFile : OleObjectControlCreationMethod
    {
        private string _filePath;

        public CreationMethodFromFile(string filePath, bool link)
        {
            FilePath = filePath;
            Link = link;
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            private set
            {
                DebugTools.DevAssert(File.Exists(value), "The entered file path {0} is incorrect or doesn't exist.", value);
                _filePath = value;
            }
        }

        public bool Link { get; private set; }

        public override bool Visit(OleObjectControl control, string oleContainerPath)
        {
            return control.Accept(this, oleContainerPath);
        }
    }

    private void CanExecuteOleObjectResetSize(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanResetSize;
    }

    private void CanExecuteUpdateOleObjectLink(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanUpdateLink;
    }

    private void CanToggleOleObjectAutoResize(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanToggleAutoResize;
    }

    private void ExecutedOleObjectResetSize(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        View.ViewModel.ExecutedOleResetSize();
    }

    private void ExecutedUpdateOleObjectLink(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        View.ViewModel.UpdateLink();
    }

    public void CloseOle()
    {
        if (IsOleOpen)
        {
            View.StartOleCloseOperation();
        }
    }

    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction)
    {
        Activate(isActive: true);
        return null;
    }

    public global::Quire.Controls.Core.IResizeData StoreResizeData(global::System.Windows.Size oldSize, global::System.Windows.Size newSize)
    {
        return View.ViewModel.StoreResizeData(oldSize, newSize);
    }

    public void RestoreResizeData(global::Quire.Controls.Core.IResizeData resizeData)
    {
        View.ViewModel.RestoreResizeData(resizeData);
    }

    public global::System.Windows.Size CoerceNewSize(global::System.Windows.Size newSize, global::Quire.Controls.Core.IResizeData resizeData)
    {
        return View.ViewModel.CoerceNewSize(newSize, resizeData);
    }
}
