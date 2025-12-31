using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Gpp.Core
{
    public class CoroutineRunner : MonoBehaviour
    {
        private readonly Queue<Action> callbacks = new();
        private readonly object syncToken = new();

        private bool isRunning = true;

        public Coroutine Run(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void Stop(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public void Run(Action callback)
        {
            lock (syncToken)
            {
                callbacks.Enqueue(callback);
            }
        }

        private IEnumerator RunCallbacks()
        {
            while (isRunning)
            {
                yield return new WaitUntil(() => callbacks.Count > 0);

                Action callback;

                lock (syncToken)
                {
                    callback = callbacks.Dequeue();
                }

                callback();
            }
        }

        private void Awake()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }

            Application.logMessageReceived += delegate (string log, string trace, LogType type)
            {
                // if (type is not (LogType.Exception or LogType.Error)) return;
                // GppLog.ReportException(log, trace);
            };

            StartCoroutine(RunCallbacks());
        }

        private void OnDestroy()
        {
            isRunning = false;
        }
    }
}