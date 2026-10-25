#nullable disable
using System;
using System.Collections.Generic;
using Quire;
using Quire.OLE;

namespace Quire.Controls.Core.Ole;

public static partial class OleContainerMessageKeysDict
{
    public static readonly Dictionary<OleResult, string> UpdateMessageKeys;

    public static readonly Dictionary<OleResult, string> ActivateMessageKeys;

    public static readonly Dictionary<OleResult, string> ResizeMessageKeys;

    static OleContainerMessageKeysDict()
    {
        UpdateMessageKeys = new Dictionary<OleResult, string>
        {
            {
                OleResult.CantOpenFile,
                "update_ole_broken_link_warning"
            },
            {
                OleResult.MissingData,
                "activate_ole_missing_data"
            },
            {
                OleResult.AppNotRegistered,
                "update_ole_app_not_registered_warning"
            },
            {
                OleResult.UnknownFailure,
                "update_ole_update_failed_warning"
            }
        };
        ActivateMessageKeys = new Dictionary<OleResult, string>
        {
            {
                OleResult.CantOpenFile,
                "activate_ole_broken_link_warning"
            },
            {
                OleResult.MissingData,
                "activate_ole_missing_data"
            },
            {
                OleResult.AppNotRegistered,
                "activate_ole_app_not_registered_warning"
            },
            {
                OleResult.UnknownFailure,
                "server_busy_activate_warning"
            }
        };
        ResizeMessageKeys = new Dictionary<OleResult, string>
        {
            {
                OleResult.CantOpenFile,
                "server_busy_resize_warning"
            },
            {
                OleResult.MissingData,
                "activate_ole_missing_data"
            },
            {
                OleResult.AppNotRegistered,
                "server_busy_resize_warning"
            },
            {
                OleResult.UnknownFailure,
                "server_busy_resize_warning"
            }
        };
        ValidateMessageKeysDict(UpdateMessageKeys);
        ValidateMessageKeysDict(ActivateMessageKeys);
        ValidateMessageKeysDict(ResizeMessageKeys);
    }

    private static void ValidateMessageKeysDict(Dictionary<OleResult, string> dict)
    {
        foreach (OleResult value in Enum.GetValues(typeof(OleResult)))
        {
            if (value != OleResult.Success && !DebugTools.DevAssert(dict.ContainsKey(value), "The key: {0} is missing in the dictionary.", value).Failed)
            {
                DebugTools.DevAssert(dict[value].IsNotEmpty(), "The key: {0} has no or empty value in the dictionary.", value);
            }
        }
    }
}
