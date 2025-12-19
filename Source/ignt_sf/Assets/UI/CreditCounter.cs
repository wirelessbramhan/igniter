using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports
{
    public class CreditCounter : MonoBehaviour
    {
        public FloatVariableSO PlayerCredits;
        [SerializeField]
        protected TextMeshProUGUI CreditsText;

        void Update()
        {
            CreditsText.text = PlayerCredits.Value.ToString();
        }
    }
}
