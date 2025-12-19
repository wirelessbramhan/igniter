using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports
{
    public class PrizeCard : MonoBehaviour
    {
        public float prizeAmount;
        public TextMeshProUGUI amountText;
        public FloatVariableSO PlayerCoins;

        void Start()
        {
            amountText.text  = prizeAmount.ToString();
        }


        public void DeductCoins()
        {
            PlayerCoins.Value -= prizeAmount;
        }
        
    }
}
