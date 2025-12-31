using System;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    public class WindowStart : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<Animator>().Play("SwipeRight");
        }
    }
}
