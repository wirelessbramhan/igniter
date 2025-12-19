using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gpp.Core
{
    public class GppSyncContext : MonoBehaviour
    {
        private class TimerAction
        {
            public float Interval;
            public float LastInvokedTime;
            public Action InvokeAction;
        }
        private static readonly Queue<Action> runInUpdate = new();
        private static readonly List<Action> alwaysRunInUpdate = new();
        private static readonly List<TimerAction> timerActions = new();

        private float timer = 0.0f;

        public static void RunOnUnityThread(Action action)
        {
            if (runInUpdate == null)
            {
                return;
            }

            lock (runInUpdate)
            {
                runInUpdate.Enqueue(action);
            }
        }

        public static void AddAlwaysRunInUpdateMethod(Action action)
        {
            lock (alwaysRunInUpdate)
            {
                alwaysRunInUpdate.Add(action);
            }
        }

        public static void RemoveAlwaysRunInUpdateMethod(Action action)
        {
            lock (alwaysRunInUpdate)
            {
                alwaysRunInUpdate.Remove(action);
            }
        }

        public static void AddTimerAction(float interval, Action action)
        {
            lock (timerActions)
            {
                timerActions.Add(new TimerAction()
                {
                    Interval = interval,
                    LastInvokedTime = -interval,
                    InvokeAction = action
                });
            }
        }

        public static void RemoveTimerAction(Action action)
        {
            lock (timerActions)
            {
                timerActions.RemoveAll(x => x.InvokeAction == action);
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            lock (runInUpdate)
            {
                while (runInUpdate.Count > 0)
                {
                    var action = runInUpdate.Dequeue();
                    action?.Invoke();
                }
            }

            lock (alwaysRunInUpdate)
            {
                for (var i = 0; i < alwaysRunInUpdate.Count; i++)
                {
                    alwaysRunInUpdate[i].Invoke();
                }
            }

            lock (timerActions)
            {
                for (var i = 0; i < timerActions.Count; i++)
                {
                    var action = timerActions[i];
                    if (timer > action.Interval + action.LastInvokedTime)
                    {
                        action.LastInvokedTime = timer;
                        action.InvokeAction.Invoke();
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            GppSDK.DisconnectSdk();
        }
    }
}