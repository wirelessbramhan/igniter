using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class PlayerCard : MonoBehaviour
    {
        public Image PlayerIcon;

        public FloatVariableSO PlayerCredits;
        public float creditValue, pointsvalue;
        public TextMeshProUGUI creditsText, pointsText, playerName, lastMatchStatus;

        public void SetData(PlayerData data)
        {
            playerName.text = data.Name;
            creditsText.text = data.CreditCost.ToString();
            pointsText.text = data.Pointsvalue.ToString();
            lastMatchStatus.gameObject.SetActive(data.HasPlayedLastMatch);
        }

        public void DeductCost()
        {
            PlayerCredits.Value -= creditValue;
        }
    }
}
