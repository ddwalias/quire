#nullable disable
using System;
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class FindReplaceQuery
{
    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public FindOptions FindReplaceOption { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public string ValueToFind { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public string ValueToReplaceWith { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public List<Pair<int, int>> FindSubscriptPositions { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public List<Pair<int, int>> ReplaceSubscriptPositions { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public Nullable<FindReplaceStatus> Status { get; set; } = FindReplaceStatus.Empty;

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool IsFindContainSubscript { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool FindIsFullySubscript { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool IsReplaceContainSubscript { get; }

    [MirrorVerified("PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests")]
    public bool ReplaceIsFullySubscript { get; }

    public Action<bool> PostReplaceAndFindNext;

    // Release IL requires call on List.get_Count where C# emits callvirt; subscriptPositions?.Count emits call.
    private bool IsFullySubscript(List<Pair<int, int>> subscriptPositions, string text)
    {
        if (subscriptPositions?.Count == 1 && subscriptPositions[0].First == 0)
        {
            return subscriptPositions[0].Second == text.Length - 1;
        }
        return false;
    }

    public FindReplaceQuery(string oldValue, string newValue, FindOptions options, List<Pair<int, int>> findSubscriptPositions = null, List<Pair<int, int>> replaceSubscriptPositions = null)
    {
        ValueToFind = oldValue;
        ValueToReplaceWith = newValue;
        FindReplaceOption = options;
        FindSubscriptPositions = findSubscriptPositions;
        ReplaceSubscriptPositions = replaceSubscriptPositions;
        IsFindContainSubscript = FindSubscriptPositions?.Count > 0;
        FindIsFullySubscript = IsFullySubscript(findSubscriptPositions, ValueToFind);
        IsReplaceContainSubscript = ReplaceSubscriptPositions?.Count > 0;
        ReplaceIsFullySubscript = IsFullySubscript(replaceSubscriptPositions, ValueToReplaceWith);
    }
}
