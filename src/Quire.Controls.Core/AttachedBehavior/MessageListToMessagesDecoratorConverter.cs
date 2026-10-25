#nullable disable
using System;
using System.Globalization;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class MessageListToMessagesDecoratorConverter
{
    public static readonly MessageListToMessagesDecoratorConverter Instance = new MessageListToMessagesDecoratorConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new MessagesDecorator((MessageList)value);
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageListToMessagesDecoratorConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessageListToMessagesDecoratorConverterMechanicalDifferentialTests")]
    public MessageListToMessagesDecoratorConverter() { }
}
