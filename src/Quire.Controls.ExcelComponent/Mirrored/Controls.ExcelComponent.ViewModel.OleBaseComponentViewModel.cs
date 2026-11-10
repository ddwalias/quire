using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Undo;

namespace Quire.Controls.ExcelComponent.ViewModel;

public abstract partial class OleBaseComponentViewModel
{
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.ViewModel.OleBaseComponentViewModel+ToggleSectionVisibilityCommand, Ptc.Controls.ExcelComponent")]
    public partial class ToggleSectionVisibilityCommand : IUndoableCommand
    {
        private readonly OleBaseComponentViewModel _oleBaseComponentViewModel;
        private readonly Action<bool> _setVisibility;
        private readonly bool _originalVisibility;
        private readonly string _name;

        public ToggleSectionVisibilityCommand(OleBaseComponentViewModel oleBaseComponentViewModel, bool isInputSection)
        {
            _oleBaseComponentViewModel = oleBaseComponentViewModel;
            _name = string.Format("Toggle {0} Section State", isInputSection ? "Input" : "Output");
            _originalVisibility = isInputSection ? _oleBaseComponentViewModel.IsInputSectionVisible : _oleBaseComponentViewModel.IsOutputSectionVisible;
            _setVisibility = isInputSection
                ? visibility => _oleBaseComponentViewModel.IsInputSectionVisible = visibility
                : visibility => _oleBaseComponentViewModel.IsOutputSectionVisible = visibility;
        }

        public string Name => _name;

        public bool Do()
        {
            _setVisibility(!_originalVisibility);
            return true;
        }

        public void Undo()
        {
            _setVisibility(_originalVisibility);
        }

        public void Redo()
        {
            Do();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.ViewModel.OleBaseComponentViewModel+PasteOleComponentItemsCommand, Ptc.Controls.ExcelComponent")]
    public partial class PasteOleComponentItemsCommand : PasteMathboardItemsCommand
    {
        public PasteOleComponentItemsCommand(IOleBaseComponentViewModel viewModel, string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
            : base(viewModel, commandName, pasteItemsAtLocation)
        {
        }

        private OleBaseComponentViewModel OleBaseComponentViewModel => _viewModel as OleBaseComponentViewModel;

        protected override bool DoWithResize()
        {
            double inputsHeight;
            Point insertionPoint = OleBaseComponentViewModel.InsertionPoint;
            if (insertionPoint.Y < OleBaseComponentViewModel.OleContainerItemTop)
            {
                inputsHeight = OleBaseComponentViewModel.InputSectionActualHeight;
                _itemsAtLocation.Values.Each(itemLocation =>
                {
                    inputsHeight = Math.Max(inputsHeight, itemLocation.Y + 1 + OleBaseComponentViewModel.Grid.GridCellSize.Height / 2);
                });
                if (inputsHeight > OleBaseComponentViewModel.InputSectionActualHeight)
                {
                    OleBaseComponentViewModel.OleContainerItemTop = inputsHeight;
                    inputsHeight = OleBaseComponentViewModel.OleContainerItemTop;
                    double inputsHeightDelta = inputsHeight - OleBaseComponentViewModel.InputSectionActualHeight;
                    OleBaseComponentViewModel.InputSectionActualHeight = inputsHeight;
                    Size newSize = new Size(OleBaseComponentViewModel.ContentWidth, OleBaseComponentViewModel.ContentHeight + inputsHeightDelta);
                    OleBaseComponentViewModel.FireChildCausedResize(OleBaseComponentViewModel.ContentSize, newSize);
                    CommandBuffer.ExecuteAndAttach(new WhiteboardViewModel.MoveItemsCommand(ViewModel, new Vector(0.0, inputsHeightDelta), true, OleBaseComponentViewModel.OutputItems));
                }
            }
            return base.DoWithResize();
        }
    }
}
