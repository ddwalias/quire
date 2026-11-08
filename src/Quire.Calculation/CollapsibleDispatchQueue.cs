using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Quire.Calculation;

internal partial class CollapsibleDispatchQueue
{
    private enum Status
    {
        Pending,
        Dispatched,
        Completed
    }

    // Member declaration order matches Mathcad member ordinals for closure naming
    public CollapsibleDispatchQueue(Dispatcher dispatcher)
    {
        _queueSet = new Dictionary<object, List<Pair<Action, Status>>>();
        _dispatcher = dispatcher;
        _worker = new BackgroundWorker();
        _worker.DoWork += Run;
        _worker.RunWorkerAsync();
    }

    public void Enqueue(object keyObject, Action action)
    {
        lock (_queueSet)
        {
            List<Pair<Action, Status>> list;
            if (!_queueSet.ContainsKey(keyObject))
            {
                list = new List<Pair<Action, Status>>();
                _queueSet[keyObject] = list;
            }
            else
            {
                list = _queueSet[keyObject];
            }

            lock (list)
            {
                list.RemoveAll(pair => pair.Second == Status.Pending);
                list.Add(new Pair<Action, Status>(action, Status.Pending));
                _waitForNonEmpty.Set();
            }
        }
    }

    private void Run(object sender, DoWorkEventArgs doWorkEventArgs)
    {
        while (true)
        {
            _waitForNonEmpty.Wait();
            object[] array;
            lock (_queueSet)
            {
                if (_queueSet.Count == 0)
                {
                    _waitForNonEmpty.Reset();
                    continue;
                }

                array = new object[_queueSet.Count];
                _queueSet.Keys.CopyTo(array, 0);
            }

            foreach (object key in array)
            {
                List<Pair<Action, Status>> queue;
                lock (_queueSet)
                {
                    if (_queueSet.ContainsKey(key))
                    {
                        queue = _queueSet[key];
                        lock (queue)
                        {
                            if (queue.IsNullOrEmpty())
                            {
                                _queueSet.Remove(key);
                                continue;
                            }
                        }
                        // IL requires goto to exit outer lock directly to outer label
                        goto IL_0103;
                    }
                    continue;
                }

                IL_0103:
                lock (queue)
                {
                    queue.RemoveAll(p => p.Second == Status.Completed);
                    if (queue.Exists(p => p.Second == Status.Dispatched))
                    {
                        continue;
                    }

                    Pair<Action, Status> taskToDispatch = queue.FindLast(p => p.Second == Status.Pending);
                    queue.RemoveAll(p => p.Second == Status.Pending);
                    if (taskToDispatch.First == null)
                    {
                        continue;
                    }

                    taskToDispatch.Second = Status.Dispatched;
                    queue.Add(taskToDispatch);
                    Action action = () =>
                    {
                        taskToDispatch.First();
                        lock (queue)
                        {
                            queue.Remove(taskToDispatch);
                            taskToDispatch.Second = Status.Completed;
                            queue.Add(taskToDispatch);
                        }
                    };
                    _dispatcher.BeginInvoke((DispatcherPriority)10, (Delegate)action);
                }
            }
        }
    }

    private readonly Dictionary<object, List<Pair<Action, Status>>> _queueSet;
    private readonly Dispatcher _dispatcher;
    private readonly BackgroundWorker _worker;
    private readonly ManualResetEventSlim _waitForNonEmpty = new ManualResetEventSlim(false);
}
