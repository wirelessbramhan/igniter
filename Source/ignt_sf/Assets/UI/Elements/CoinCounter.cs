using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public class CoinCounter : MonoBehaviour
    {
        public FloatVariableSO UserCoins;
        [SerializeField]
        private TextMeshProUGUI CoinText;

        void Update()
        {
          CoinText.text = UserCoins.Value.ToString();  
        }
    }
}
