using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RunInMainThread
{
    /// <summary>
    /// 线程调度，将其它线程中的代码放置在主线程中执行
    /// </summary>
    public sealed class EventDispatcher : MonoBehaviour
    {
        private static EventDispatcher instance;

        private int _lock;
        private bool _isRun;
        private Queue<Action> _queWaitAction = new Queue<Action>();
        private Queue<Action> _queExecute;

        public static void Run(Action action)
        {
            instance.Runner(action);
        }

        private void Runner(Action action)
        {
            while (true)
            {
                //以原子操作的形式，将 32 位有符号整数设置为指定的值并返回原始值。
                if (0 == Interlocked.Exchange(ref _lock, 1))
                {
                    //acquire lock
                    _queWaitAction.Enqueue(action);
                    _isRun = true;
                    //exist
                    Interlocked.Exchange(ref _lock, 0);
                    break;
                }
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (_isRun)
            {
                _queExecute = null;
                //主线程不推荐使用lock关键字，防止block 线程，以至于deadlock
                if (0 == Interlocked.Exchange(ref _lock, 1))
                {
                    _queExecute = new Queue<Action>(_queWaitAction.Count);
                    while (_queWaitAction.Count != 0)
                    {
                        _queExecute.Enqueue(_queWaitAction.Dequeue());
                    }
                    //finished
                    _isRun = false;
                    //release
                    Interlocked.Exchange(ref _lock, 0);
                }
                //not block
                if (_queExecute != null)
                {
                    while (_queExecute.Count != 0)
                    {
                        _queExecute.Dequeue()();
                    }
                }
            }
        }
    }
}
//用法: RunInMainThread.Dispatcher.Run(() => {/*函数体*/})