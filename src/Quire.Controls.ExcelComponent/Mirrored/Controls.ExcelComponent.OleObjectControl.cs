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
}
