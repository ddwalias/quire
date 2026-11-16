using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Quire.Controls;

public partial class EquationCommands
{
    public partial class CommitCommandParameter
    {
        public bool IsAsynchronous { get; }

        public bool IsForce { get; set; }

        public bool IsForceControlOnly { get; set; }

        public CommitCommandParameter(bool isAsynchronous)
        {
            IsAsynchronous = isAsynchronous;
        }

        public CommitCommandParameter(bool isAsynchronous, bool force, bool isForceControlOnly)
        {
            IsAsynchronous = isAsynchronous;
            IsForce = force;
            IsForceControlOnly = isForceControlOnly;
        }
    }

    public static IEnumerable<KeyGesture> GetCommandKeyGestures(RoutedCommand command)
    {
        IEnumerable<KeyGesture> gestures = command.InputGestures.OfType<KeyGesture>();
        DebugTools.DevAssert(gestures.IsNotEmpty(), null);
        return gestures;
    }

    public static KeyGesture GetCommandFirstKeyGesture(RoutedCommand command)
    {
        return GetCommandKeyGestures(command).FirstOrDefault();
    }
}
