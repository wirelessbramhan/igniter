using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gpp.CommonUI.Legal
{
    internal class GppToggleTouchPanel : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Toggle toggle;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (toggle == null)
            {
                return;
            }

            toggle.isOn = !toggle.isOn;
        }
    }
}