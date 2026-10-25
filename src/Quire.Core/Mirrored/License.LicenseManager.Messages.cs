#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    private void OnDisplayLicenseMessage(LicenseCbArgs cbArgs)
    {
        if (cbArgs.LicenseIsValid.HasValue)
        {
            UpdateLicenseStatusAndRefreshTimer(cbArgs.LicenseIsValid.Value);
        }

        string message = global::System.String.IsNullOrEmpty(cbArgs.Message)
            ? null
            : global::System.Environment.NewLine + cbArgs.Message.Trim();
        string licenseMessageText = string.Empty;
        string messageText;
        if (cbArgs.MessageName != LicenseMessageName.MessageNoneE)
        {
            messageText = _licenseMessageDictionary[cbArgs.MessageName] as string;
            string messageSuffix = message;
            if (!global::System.String.IsNullOrEmpty(messageText)
                && !global::System.String.IsNullOrEmpty(messageSuffix))
            {
                messageText += messageSuffix;
            }
        }
        else
        {
            messageText = message;
        }

        if (!global::System.String.IsNullOrEmpty(messageText))
        {
            if (_licenseMessages.Count == 30)
            {
                _licenseMessages.RemoveAt(0);
            }

            string item = global::System.DateTime.Now.ToString() + " : " + messageText;
            _licenseMessages.Add(item);
        }

        for (int index = _licenseMessages.Count - 1; index >= 0; index--)
        {
            licenseMessageText += global::System.String.Format(
                "{0}{1}{1}",
                _licenseMessages[index],
                global::System.Environment.NewLine);
        }

        LicenseMessageText = licenseMessageText;
    }
}
