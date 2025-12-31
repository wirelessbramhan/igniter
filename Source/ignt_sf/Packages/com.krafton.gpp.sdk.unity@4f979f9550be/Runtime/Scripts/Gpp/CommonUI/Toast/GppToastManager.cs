using System.Collections;
using System.Collections.Generic;
using Gpp.Log;
using UnityEngine;

namespace Gpp.CommonUI.Toast
{
    public class GppToastManager : MonoBehaviour
    {
        private static readonly object lockObj = new();
        private readonly Queue<GppToastMessage> toastQueue = new();
        private bool isShowing;

        public void ShowToast(GppToastMessage message)
        {
            lock (lockObj)
            {
                toastQueue.Enqueue(message);
                if (!isShowing)
                {
                    StartCoroutine(ShowToasts());
                }
            }
        }

        private IEnumerator ShowToasts()
        {
            isShowing = true;
            while (true)
            {
                GppToastMessage toastMessage;

                lock (lockObj)
                {
                    if (toastQueue.Count == 0)
                    {
                        isShowing = false;
                        break;
                    }

                    toastMessage = toastQueue.Dequeue();
                }

                var toastPrefab = toastMessage.Prefab;
                if (toastPrefab is null)
                {
                    GppLog.Log("Toast Prefab is empty.");
                    break;
                }

                var toastObject = Instantiate(toastPrefab);
                var toastUI = toastObject.GetComponent<IGppToastUI>();
                toastUI.SetToastMessage(toastMessage);
                var isClosed = false;
                toastUI.SetOnClickCloseListener(() => isClosed = true);
                var waitTime = toastMessage.AnimSec + 2;
                var startTime = Time.time;
                yield return new WaitUntil(() => isClosed || Time.time >= startTime + waitTime);
            }

            Destroy(gameObject);
        }
    }
}