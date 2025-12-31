using UnityEngine;

namespace Gpp.CommonUI.Toast
{
    public class GppToastMessage
    {
        internal GameObject Prefab { get; set; }
        internal string Message { get; set; }
        internal string SubMessage { get; set; }
        internal float AnimSec { get; set; }
        internal GppToastPosition Position { get; set; }

        internal GppToastMessage(string message, string subMessage = null, GppToastPosition position = GppToastPosition.BOTTOM, GameObject prefab = null, float animSec = 2.0f)
        {
            Message = message;
            SubMessage = subMessage;
            Position = position;
            AnimSec = animSec;
            Prefab = prefab;
        }
    }
}