using System;
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
}
