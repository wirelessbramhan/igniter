using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public class PrizeCard : MonoBehaviour
    {
        public float prizeAmount;
        public TextMeshProUGUI amountText;
        public FloatVariableSO PlayerCoins;

        void OnValidate()
        {
            Configure();
        }

        void Start()
        {
            Configure();  
        }

        private void Configure()
        {
            amountText.text  = prizeAmount.ToString();
        }


        public void DeductCoins()
        {
            PlayerCoins.Value -= prizeAmount;
        }
        
    }
}
