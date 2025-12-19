using UnityEngine;

namespace ignt.sports.cricket.network
{
    public class DebugButton : MonoBehaviour
    {
        public GameObject DebugPanel;
        public void ToggleDebug()
        {
            DebugPanel.SetActive(!DebugPanel.activeSelf);
            
        }
    }
}
