using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

namespace Gpp.CommonUI
{
    public class ButtonClickController : MonoBehaviour
    {
        private Button[] buttons;
        public float disableDuration = 1f;
        public string[] excludeButtonName = System.Array.Empty<string>();

        private void Start()
        {
            buttons = GetComponentsInChildren<Button>(true);

            foreach (var button in buttons)
            {
                if(excludeButtonName.Contains(button.name) is false)
                    button.onClick.AddListener(() => OnButtonClick(button));
            }
        }

        private void OnButtonClick(Button clickedButton)
        {
            Selectable.Transition beforeTransition = clickedButton.transition;
            clickedButton.transition = Selectable.Transition.None;
            clickedButton.interactable = false;
            StartCoroutine(EnableButtonsAfterDelay(clickedButton, beforeTransition));
        }

        private IEnumerator EnableButtonsAfterDelay(Button clickedButton, Selectable.Transition beforeTransition)
        {
            yield return new WaitForSeconds(disableDuration);
            clickedButton.interactable = true;
            clickedButton.transition = beforeTransition;
        }
    }
}