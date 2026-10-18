#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public static partial class WorksheetPropertiesManager
{
    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static readonly DependencyPropertyKey WorksheetPropertiesSourcePropertyKey;

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void SetWorksheetPropertiesSource(DependencyObject dObj, IWorksheetPropertiesSource propertiesSource) => dObj.SetValue(WorksheetPropertiesSourcePropertyKey, propertiesSource);

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static readonly DependencyPropertyKey WorksheetPropertiesChangeInfoKey;

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void SetWorksheetPropertiesChangeInfo(DependencyObject dObj, WorksheetPropertiesChangeInfo newInfo) => dObj.SetValue(WorksheetPropertiesChangeInfoKey, newInfo);

    partial class WorksheetPropertiesChangeInfo
    {
        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public FrameworkElement WorksheetFrameworkElement { get; private set; }
    }
}
