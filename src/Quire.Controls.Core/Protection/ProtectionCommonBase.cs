#nullable disable
using System;

namespace Quire.Controls.Core.Protection;

public abstract partial class ProtectionCommonBase
{
    protected IProtectableModel _model;

    protected ProtectionData _protectionData;

    protected Action<string> _showWarningDialog;

    protected ProtectionCommonBase(IProtectableModel model, Action<string> showWarningDialog)
    {
        _showWarningDialog = showWarningDialog;
        _model = model;
    }

    protected void Unprotect()
    {
        _ = _model.ProtectionData.HasPassword;
        _model.RemoveProtection();
    }

    protected void Protect()
    {
        _model.SetProtection(_protectionData);
        if (_model.HasOpenDescendantOleObjects)
        {
            _showWarningDialog("ActiveOleWarningMessage");
        }
    }
}
