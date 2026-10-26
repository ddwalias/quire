#nullable disable
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class MessagesDecorator : IAttachedBehavior
{
    private FrameworkElement _elem;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessagesDecoratorMechanicalDifferentialTests")]
    private readonly List<NodeMessageAdorner> _adornerList = new List<NodeMessageAdorner>();

    private readonly MessageList _messageList;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorMessagesDecoratorMechanicalDifferentialTests")]
    public IEnumerable<NodeMessageAdorner> AdornerList => _adornerList;

    public MessagesDecorator(MessageList list)
    {
        _messageList = list;
    }

    public void Attach(FrameworkElement elem)
    {
        if (DebugTools.AssertNotNull(elem, null).Failed)
        {
            return;
        }
        if (DebugTools.DevAssert(_elem == null, null).Failed)
        {
            Detach(_elem);
        }
        _elem = elem;
        CreateMessages();
        _messageList.RangeChanged += HandleMessageListChanged;
    }

    public void Detach(FrameworkElement elem)
    {
        DebugTools.DevAssert(_elem == elem, null);
        ClearMessages();
        _elem = null;
        _messageList.RangeChanged -= HandleMessageListChanged;
    }

    private void HandleMessageListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ClearMessages();
        CreateMessages();
    }

    private void CreateMessages()
    {
        foreach (MessageItem message in _messageList)
        {
            if (string.IsNullOrEmpty(message.Message))
            {
                string text = _elem.TryFindResource(message.MessageResourceKey) as string;
                if (DebugTools.AssertNotNull(text, null).Failed)
                {
                    text = message.MessageResourceKey;
                }
                message.Message = text;
            }
            _adornerList.Add(new NodeMessageAdorner(_elem, message.Message, message.MessageType));
        }
    }

    private void ClearMessages()
    {
        foreach (NodeMessageAdorner adorner in _adornerList)
        {
            adorner.Remove();
        }
        _adornerList.Clear();
    }
}
