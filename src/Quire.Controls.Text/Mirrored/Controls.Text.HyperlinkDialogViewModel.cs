#nullable disable
#pragma warning disable

using System;
using System.IO;
using Quire.Controls.Core;
using Quire.Core;
using Quire.IO;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class HyperlinkDialogViewModel
{
    private readonly HyperlinkContextValues _hyperlinkContextValues;
    private string _userInputAddress;
    private string _userInputText;
    private bool _isOkButtonEnabled;

    public HyperlinkDialogViewModel(HyperlinkContextValues hyperlinkContextValues)
        : base(ResourceProvider<HyperlinkDialogErrorsResourceDictionary>.Instance)
    {
        _hyperlinkContextValues = hyperlinkContextValues;
        if (UserCanModifyText)
        {
            UserInputText = ContextText;
        }
        UserInputAddress = ContextLinkAddress ?? string.Empty;
    }

    public bool UserCanModifyText => _hyperlinkContextValues.UserCanModifyText;

    public string ContextText => _hyperlinkContextValues.ContextText;

    private string ContextLinkAddress => _hyperlinkContextValues.ContextLinkAddress;

    public string UserInputAddress
    {
        get => _userInputAddress;
        set
        {
            if (_userInputAddress == value)
            {
                return;
            }
            _userInputAddress = value;
            ValidateDialogInputsState();
            OnPropertyChanged("UserInputAddress");
        }
    }

    public string UserInputText
    {
        get => _userInputText;
        set
        {
            if (_userInputText == value || DebugTools.DevAssert(UserCanModifyText, "In case of non-editable selection the text could not be set").Failed)
            {
                return;
            }
            _userInputText = value;
            OnPropertyChanged("UserInputText");
        }
    }

    public LinkEditOperation PerformedLinkEditOperation { get; private set; }

    private Uri UserInputUri { get; set; }

    public bool IsOkButtonEnabled
    {
        get => _isOkButtonEnabled;
        set
        {
            if (_isOkButtonEnabled == value)
            {
                return;
            }
            _isOkButtonEnabled = value;
            OnPropertyChanged("IsOkButtonEnabled");
        }
    }

    // Mathcad returns true when the address is not a usable URI (the result is an error flag, not a success flag).
    public bool TryCreateUriFromAddress(out Uri resultUri)
    {
        bool hasError;
        try
        {
            resultUri = EnumerableExtensions.IsNullOrEmpty(UserInputAddress) ? null
                : UrlUtils.TryCreateKnownUri(UserInputAddress, out resultUri) ? resultUri
                : new Uri(UserInputAddress, UriKind.RelativeOrAbsolute);
            hasError = false;
        }
        catch (ArgumentException)
        {
            hasError = true;
            resultUri = null;
            DebugTools.FailAssertion((string)null);
        }
        catch (UriFormatException)
        {
            hasError = true;
            resultUri = null;
        }

        if (resultUri != null && (!resultUri.IsAbsoluteUri || resultUri.IsFile))
        {
            try
            {
                Path.GetDirectoryName(UserInputAddress);
            }
            catch (ArgumentException)
            {
                hasError = true;
                resultUri = null;
            }
            catch (PathTooLongException)
            {
                hasError = true;
                resultUri = null;
            }
        }
        return hasError;
    }

    public HyperlinkEditState GetState()
    {
        if (!HasErrors)
        {
            return new HyperlinkEditState(
                PerformedLinkEditOperation,
                EnumerableExtensions.IsNullOrEmpty(UserInputText) && UserCanModifyText ? UserInputAddress : UserInputText,
                UserCanModifyText && UserInputText != ContextText,
                UserInputUri);
        }
        return default;
    }

    public void UpdatePerformedLinkOperation()
    {
        PerformedLinkEditOperation = EnumerableExtensions.IsNullOrEmpty(ContextLinkAddress)
            ? (EnumerableExtensions.IsNullOrEmpty(UserInputAddress) ? LinkEditOperation.None : LinkEditOperation.Insert)
            : (EnumerableExtensions.IsNullOrEmpty(UserInputAddress) ? LinkEditOperation.Delete : LinkEditOperation.Edit);
    }

    private void ValidateDialogInputsState()
    {
        UpdatePerformedLinkOperation();
        bool hasError = TryCreateUriFromAddress(out Uri uri);
        UserInputUri = uri;
        if (hasError)
        {
            SetError("UserInputAddress", HyperlinkInputValidationErrorKeys.InvalidUriSyntaxKey);
        }
        else
        {
            RemoveAllErrorsForProperty("UserInputAddress");
        }
        IsOkButtonEnabled = !hasError && PerformedLinkEditOperation != LinkEditOperation.None;
    }
}
